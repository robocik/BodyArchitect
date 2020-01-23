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

namespace BodyArchitect.Service.Client.WP7.ModelExtensions
{
    public interface ICommentable
    {
        string Comment { get; set; }

        Guid InstanceId { get; set; }
    }
}
