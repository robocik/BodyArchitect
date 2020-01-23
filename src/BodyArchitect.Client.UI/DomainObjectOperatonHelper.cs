using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Views;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI
{
    public static class DomainObjectOperatonHelper
    {
        public static string GetEntryObjectText(this EntryObjectDTO entry)
        {
            return string.IsNullOrWhiteSpace(entry.Name) ? EntryObjectLocalizationManager.Instance.GetString(entry.GetType(), EnumLocalizer.EntryObjectName) : entry.Name;
        }

        public static IEnumerable<TrainingDayDTO>  ToTrainingDays(this IEnumerable<EntryObjectDTO> entryObjects)
        {
            Dictionary<DateTime, TrainingDayDTO> days = new Dictionary<DateTime, TrainingDayDTO>();
            foreach (var entry in entryObjects)
            {
                if (!days.ContainsKey(entry.TrainingDay.TrainingDate))
                {
                    var day = new TrainingDayDTO();
                    day.TrainingDate = entry.TrainingDay.TrainingDate;
                    days.Add(entry.TrainingDay.TrainingDate, day);
                }
                days[entry.TrainingDay.TrainingDate].Objects.Add(entry);
            }
            return days.Values;
        }

        public static bool DeleteTrainingDay( TrainingDayDTO day,TrainingDayInfo dayInfo=null)
        {
            bool res = false;
            if (day != null && UIHelper.EnsurePremiumLicence())
            {

                if (BAMessageBox.AskYesNo(Strings.QRemoveTrainingDay, day.TrainingDate.ToShortDateString()) == MessageBoxResult.Yes)
                {
                    if (dayInfo != null)
                    {
                        dayInfo.IsProcessing = true;
                    }
                    PleaseWait.Run(delegate(MethodParameters par)
                    {
                        try
                        {
                            var param = new DeleteTrainingDayParam();
                            param.TrainingDayId = day.GlobalId;
                            ServiceManager.DeleteTrainingDay(param);

                            Guid? customerId = day.CustomerId;
                            Guid? userId = day.ProfileId;
                            var cache = TrainingDaysReposidory.GetCache(customerId, userId);
                            cache.Remove(day.TrainingDate);
                            res = true;
                        }
                        catch (TrainingIntegrationException te)
                        {
                            res = false;
                            par.CloseProgressWindow();
                            ExceptionHandler.Default.Process(te, Strings.ErrorCannotDeleteTrainingDayPartOfTraining, ErrorWindow.MessageBox);
                        }
                        catch (Exception te)
                        {
                            res = false;
                            par.CloseProgressWindow();
                            ExceptionHandler.Default.Process(te, Strings.ErrorRemoveTrainingDay, ErrorWindow.MessageBox);
                            
                        }
                        finally
                        {
                            if (dayInfo != null)
                            {
                                dayInfo.IsProcessing = false;
                            }
                        }
                    });

                }
            }
            return res;
        }

        public static TrainingDayPageContext CreateTrainingDayWindow(TrainingDayDTO day, UserDTO user, CustomerDTO customer, IEntryObjectBuilderProvider builder = null)
        {
            if (day.GlobalId == Constants.UnsavedGlobalId)
            {
                day.AllowComments = UserContext.Current.ProfileInformation.Settings.AllowTrainingDayComments;
                //set default entries for newly created TrainingDay
                var options = UserContext.Current.Settings.GuiState.CalendarOptions;
                foreach (var defaultEntry in options.DefaultEntries)
                {
                    if (defaultEntry.IsDefault==true)
                    {
                        var plugin = PluginsManager.Instance.GetEntryObjectProvider(defaultEntry.ModuleId);
                        if (plugin != null && plugin.EntryObjectType.CanBeManuallyAdded())
                        {
                            var entry=day.AddEntry(plugin.EntryObjectType);
                            if (builder != null)
                            {
                                builder.EntryObjectCreated(entry);
                            }
                            if (day.TrainingDate.IsFuture())
                            {//for entries in future set planned status
                                entry.Status = EntryObjectStatus.Planned;
                            }
                        }
                    }
                }
                //needed for SizeEntryDTO for example
                day.ChangeDate(day.TrainingDate);
            }
            TrainingDayPageContext context = new TrainingDayPageContext(user, customer, day,builder);
            return context;
        }
    }
}
