using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;
using ImageTools;

namespace BodyArchitect.WP7.Controls.Cache
{
    public static class CacheManager
    {

        static Cache pictureCache = new Cache();

        public static Cache PictureCache
        {
            get { return pictureCache; }
            set { pictureCache = value; }
        }

    }
}
