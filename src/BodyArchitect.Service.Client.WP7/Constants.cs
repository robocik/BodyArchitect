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

namespace BodyArchitect.Service.Client.WP7
{
    public class Constants
    {
        public const int ShowRateMeLinkAfterRuns = 20;
        public const double MaxRatingValue=6;
        public const float EmptyButtonOpacity = 0.7f;
        public const string ApplicationVersion = "4.0.0.6";
        public const string ServiceVersion = "4.5.0.0";
        public const string TipsFileName = "Tips.xml";
        public const string StateFileName = "StateCache.xml";
        public const string PageStateFileName = "PageStateCache.xml";
#if FREE 
        public const string MarketplaceId = "de4f438e-c232-470a-85b6-693a98a788e7";
#else
        public const string MarketplaceId = null;
#endif

        public const string CompanyName = "Quasar Development";
        public const string ApplicationName = "BodyArchitect";
        public const string TutorialUrl = "http://www.youtube.com/embed/vb2iWbXxg5k?feature=player_detailpage";
        public const string VersionChangesVideoUrl = "http://www.youtube.com/embed/67y0WPI_TvQ?feature=player_detailpage";
    }
}
