using System.Windows.Controls;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrApplicationName.xaml
    /// </summary>
    public partial class usrApplicationName : UserControl
    {
        public usrApplicationName()
        {
            InitializeComponent();
        }

        public void Fill(EntryObjectDTO entry)
        {
            this.SetVisible(!string.IsNullOrWhiteSpace(entry.ApplicationName));
            lblApplicationName.Text = entry.ApplicationName;
        }
    }
}
