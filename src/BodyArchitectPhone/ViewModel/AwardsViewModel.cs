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
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class AwardsViewModel:ViewModelBase
    {
        private UserSearchDTO user;

        public AwardsViewModel(UserSearchDTO user)
        {
            this.user = user;
        }

        public string GreenStar
        {
            get
            {
                var star = Achievements.GetGreenStar(user);
                if (star == AchievementStar.Gold)
                {
                    return @"/Images/Stars/greenGoldStar32.png";
                }
                if (star == AchievementStar.Silver)
                {
                    return @"/Images/Stars/greenSilverStar32.png";
                }
                if (star == AchievementStar.Bronze)
                {
                    return @"/Images/Stars/greenBrownStar32.png";
                }
                return null;
            }
        }

        public string RedStar
        {
            get
            {
                var star = Achievements.GetRedStar(user);
                if (star == AchievementStar.Gold)
                {
                    return @"/Images/Stars/redGoldStart32.png";
                }
                if (star == AchievementStar.Silver)
                {
                    return @"/Images/Stars/redSilverStar32.png";
                }
                if (star == AchievementStar.Bronze)
                {
                    return @"/Images/Stars/redBrownStar32.png";
                }
                return null;
            }
        }

        public string BlueStar
        {
            get
            {
                var star = Achievements.GetBlueStar(user);
                if (star == AchievementStar.Gold)
                {
                    return @"/Images/Stars/blueGoldStar32.png";
                }
                if (star == AchievementStar.Silver)
                {
                    return @"/Images/Stars/blueSilverStar32.png";
                }
                if (star == AchievementStar.Bronze)
                {
                    return @"/Images/Stars/blueBrownStar32.png";
                }
                return null;
            }
        }
    }
}
