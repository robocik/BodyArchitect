using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for SaveScheduleEntriesWindow.xaml
    /// </summary>
    public partial class SaveScheduleEntriesWindow
    {
        private SaveScheduleEntriesViewModel viewModel;
        public SaveScheduleEntriesWindow(SaveScheduleEntryRangeParam param)
        {
            viewModel = new SaveScheduleEntriesViewModel(this, param);
            DataContext = viewModel;
            InitializeComponent();
            Binding binding = new Binding("CanSave");
            usrProgressIndicatorButtons1.OkButton.SetBinding(Button.IsEnabledProperty, binding);
            
            
        }

        private void btnSave_Click(object sender, UI.Controls.CancellationSourceEventArgs e)
        {
            viewModel.Save();
        }

        public IList<ScheduleEntryDTO> Result
        {
            get { return viewModel.Result; }
        }

    }

    public class SaveScheduleEntriesViewModel:ViewModelBase
    {
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private IBAWindow parent;
        private SaveScheduleEntryRangeParam param;
        public SaveScheduleEntriesViewModel(IBAWindow parent, SaveScheduleEntryRangeParam param)
        {
            this.parent = parent;
            this.param = param;
        }

        public IList<ScheduleEntryDTO> Result { get; private set; }

        public DateTime? FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                NotifyOfPropertyChange(()=>FromDate);
                NotifyOfPropertyChange(() => CanSave);
            }
        }
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                NotifyOfPropertyChange(() => ToDate);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public bool CanSave
        {
            get { return FromDate.HasValue && ToDate.HasValue && FromDate < ToDate && (ToDate.Value-FromDate.Value).TotalDays >= 6; }
        }

        public void Save()
        {
            if(!CanSave)
            {
                return;
            }
            param.CopyStart = FromDate.Value.ToUniversalTime();
            param.CopyEnd = ToDate.Value.AddDays(1).ToUniversalTime();

            try
            {
                Result = ServiceManager.SaveScheduleEntriesRange(param).Cast<ScheduleEntryDTO>().ToList();
            }
            catch (ArgumentException ex)
            {
                parent.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "ErrCopyRangeIsNotValidRequired".TranslateInstructor(), ErrorWindow.MessageBox), null);
                return;
            }
            catch (LicenceException ex)
            {
                parent.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "ErrInstructorAccountRequired".TranslateInstructor(), ErrorWindow.MessageBox), null);
                return;
            }
            catch (AlreadyOccupiedException ex)
            {
               parent.SynchronizationContext.Send(state => ExceptionHandler.Default.Process(ex, "AlreadyOccupiedException_SaveScheduleEntriesWindow_Save".TranslateInstructor(), ErrorWindow.MessageBox), null);
                return;
            }
            
            parent.SynchronizationContext.Send(delegate
            {
                BaseWindow wnd = (BaseWindow)parent;
                wnd.ThreadSafeClose(true);
            }, null);
        }
    }
}
