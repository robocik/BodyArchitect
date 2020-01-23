using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.Controls
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {

        }

        private static ApplicationStrings localizedresources = new ApplicationStrings();

        public ApplicationStrings Localizedresources { get { return localizedresources; } }


        public ApplicationState State { get { return ApplicationState.Current; } }
    }
}
