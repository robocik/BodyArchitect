using BodyArchitect.Service.Client.WP7;

namespace BodyArchitect.WP7.Utils
{
    public class UIHelper
    {
        public string SpeedType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Cm)
                {
                    return "km/h";
                }
                else
                {
                    return "mph";
                }
            }
        }
        public string AltitudeType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Cm)
                {
                    return "m";
                }
                else
                {
                    return "ft";
                }
            }
        }

        public string TemperatureType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Inchs)
                {
                    return "°F";
                }
                return "°C";
            }
        }

        public string PaceType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Cm)
                {
                    return "min/km";
                }
                else
                {
                    return "min/mi";
                }
            }
        }

        public string DistanceType
        {
            get
            {
                if (ApplicationState.Current.ProfileInfo.Settings.LengthType == Service.V2.Model.LengthType.Cm)
                {
                    return "km";
                }
                else
                {
                    return "mi";
                }
            }
        }
    }
}
