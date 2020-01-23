using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for usrActivityDetails.xaml
    /// </summary>
    public partial class usrActivityDetails : IEditableControl
    {
        private ActivityDTO activity;

        public usrActivityDetails()
        {
            InitializeComponent();
        }

        public void Fill(ActivityDTO activity)
        {
            Object = activity;
        }

        public ActivityDTO Activity
        {
            get { return activity; }
        }
        #region Implementation of IEditableControl

        public object Object
        {
            get { return DataContext; }
            set
            {
                activity = (ActivityDTO) value;
                DataContext = value;
            }
        }

        public bool ReadOnly
        {
            get; set;
        }

        public object Save()
        {
            return ServiceManager.SaveActivity(Activity);
        }

        #endregion
    }
}
