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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrDateRange.xaml
    /// </summary>
    public partial class usrDateRange
    {
        public usrDateRange()
        {
            InitializeComponent();
        }



        public DateTime? DateFrom
        {
            get
            {
                if (dtpFrom.SelectedDate != DateTime.MinValue)
                {
                    return dtpFrom.SelectedDate;
                }
                return null;
            }
            set { dtpFrom.SelectedDate = value; }
        }

        public DateTime? DateTo
        {
            get
            {
                if (dtpTo.SelectedDate != DateTime.MinValue)
                {
                    return dtpTo.SelectedDate;
                }
                return null;
            }
            set { dtpTo.SelectedDate = value; }
        }
    }
}
