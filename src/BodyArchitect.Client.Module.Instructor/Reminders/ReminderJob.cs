using System;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.Module.Instructor.Controls.Customers;
using BodyArchitect.Client.Module.Instructor.Converters;
using BodyArchitect.Client.Module.Instructor.Reminders;
using BodyArchitect.Client.Module.Instructor.Reminders.Controls;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using Quartz;

namespace BodyArchitect.Client.Module.Instructor
{
    public class ReminderJob:IJob
    {

        #region Implementation of IJob

        public void Execute(IJobExecutionContext context)
        {
            var item = (ReminderItemDTO)context.MergedJobDataMap["ReminderItemDTO"];
            if (NotificationsReposidory.Instance.GetItem(item.GlobalId) != null)
            {//this notification has been already shown so skip it
                return;
            }
            UIHelper.MainWindow.AddPerformanceMessage(item.Name + "," + item.DateTime);
            UIHelper.BeginInvoke(delegate
                                {
                                    NotifyObject obj = new NotifyObject(item.DateTime.ToLocalTime().ToString(), item.Name, item.DateTime);
                                    obj.Image = ReminderToIconConverter.GetReminderImage(item.Type).ToBitmap();
                                    obj.DeleteEvent=(o)=>
                                    {
                                        ServicePool.Add(new ReminderOperationServiceCommand(item, ReminderOperationType.CloseAfterShow));
                                        NotificationsReposidory.Instance.Remove(item.GlobalId);
                                    };
                                    obj.ClickEvent = (o) =>
                                            {
                                                if(!string.IsNullOrEmpty(item.ConnectedObject))
                                                {
                                                    var arr=item.ConnectedObject.Split(':');
                                                    if(arr[0]=="CustomerDTO")
                                                    {
                                                        Guid customerId;
                                                        PageContext pageContext = null;
                                                        if (Guid.TryParse(arr[1], out customerId))
                                                        {
                                                            pageContext = new PageContext(null, CustomersReposidory.Instance.GetItem(customerId));
                                                        }
                                                        //CustomersView customerView = new CustomersView();
                                                        //UIHelper.MainWindow.ShowView( customerView,true,true);
                                                        UIHelper.MainWindow.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/Customers/CustomersView.xaml"), () => pageContext, true);
                                                        return;
                                                    }
                                                    if (arr[0] == "ScheduleEntryDTO")
                                                    {
                                                        //ScheduleEntriesView view = new ScheduleEntriesView();
                                                        //UIHelper.MainWindow.ShowView(view, true, true);
                                                        ScheduleEntriesViewContext pageContext = new ScheduleEntriesViewContext(item.DateTime);
                                                        MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.Module.Instructor;component/Controls/ScheduleEntriesView.xaml"), () => pageContext, true);
                                                        return;
                                                    }
                                                    if (arr[0] == "EntryObjectDTO")
                                                    {
                                                        //ScheduleEntriesView view = new ScheduleEntriesView();
                                                        //UIHelper.MainWindow.ShowView(view, true, true);
                                                        //var day=ServiceManager.GetTrainingDay(new WorkoutDayGetOperation(){WorkoutDateTime = item.DateTime});
                                                        //var customer = day.CustomerId.HasValue?CustomersReposidory.Instance.GetItem(day.CustomerId.Value):null;
                                                        //var pageContext = new TrainingDayPageContext(null, customer, day, null);
                                                        var pageContext = new TrainingDayPageContext(null,null,null,null);
                                                        pageContext.DateTime = item.DateTime;
                                                        MainWindow.Instance.ShowPage(new Uri("pack://application:,,,/BodyArchitect.Client.UI;component/Windows/TrainingDayWindow.xaml"), () => pageContext, true);
                                                        return;
                                                    }
                                                }
                                                
                                                MainWindow.Instance.ShowDashboard(typeof(usrUserReminders),true);
                                                
                                            };
                                    obj.GlobalId = item.GlobalId;
                                    obj.Tag = item;
                                    NotificationsReposidory.Instance.Add(obj);

                                    UIHelper.MainWindow.ShowNotification(obj);

                                }, UIHelper.MainWindow.Dispatcher);

            
            
        }

        #endregion
    }

    
}
