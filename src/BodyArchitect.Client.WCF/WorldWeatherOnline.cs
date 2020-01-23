using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BodyArchitect.Portable;
using BodyArchitect.Service.V2.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BodyArchitect.Client.WCF
{
    public class WeatherRetrievedEventArgs : EventArgs
    {
        public WeatherRetrievedEventArgs(WeatherDTO weather)
        {
            Weather = weather;
        }

        public WeatherDTO Weather { get; private set; }
    }

    public class WorldWeatherOnline
    {
        private const string APIKey = "c0dc594ca4185715132103";


        private static IPAddress getExternalIP()
        {
            String direction = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }

            //Search for the ip in the html
            int first = direction.IndexOf("Address: ") + 9;
            int last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);
            IPAddress ip = IPAddress.Parse(direction);
            return ip;
        }

        private static IPAddress getExternalIP_SecondMethod()
        {
            WebRequest request = WebRequest.Create("http://ifconfig.me/ip");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                var result = stream.ReadToEnd();
                return IPAddress.Parse(result.Trim('\n'));
            }
        }

        IPAddress getIpAddress()
        {
            IPAddress ip = null;
            try
            {
                ip = getExternalIP();
            }
            catch (Exception)
            {
                ip = getExternalIP_SecondMethod();
            }
            return ip;
        }

        public WeatherDTO LoadWeather()
        {
            var ip = getIpAddress();

            string weatherUrl = "http://free.worldweatheronline.com/feed/weather.ashx?q={0}&num_of_days=1&format=json&key={1}";
            //InvariantCulture is to force . (dot) for latitude and longitude
            string url = string.Format(weatherUrl, ip, APIKey);

            return invokeWeatherWebService(url);
        }

        public WeatherDTO LoadWeather(GPSPoint location)
        {
            string weatherUrl = "http://free.worldweatheronline.com/feed/weather.ashx?q={0},{1}&num_of_days=1&format=json&key={2}";
            double latitude = location.Latitude;
            double longitude = location.Longitude;
            //InvariantCulture is to force . (dot) for latitude and longitude
            string url = string.Format(weatherUrl, latitude.ToString(CultureInfo.InvariantCulture), longitude.ToString(CultureInfo.InvariantCulture), APIKey);

            return invokeWeatherWebService(url);
        }

        private static WeatherDTO invokeWeatherWebService(string url)
        {
            var request = HttpWebRequest.Create(url);
            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("iso-8859-1")))
            {
                string contents = reader.ReadToEnd();
                var obj = (JObject) JsonConvert.DeserializeObject(contents);
                var tempC = obj["data"]["current_condition"][0]["temp_C"];
                var weatherCode = obj["data"]["current_condition"][0]["weatherCode"];
                WeatherDTO weather = new WeatherDTO();
                weather.Temperature = tempC.Value<float>();
                weather.Condition = (WeatherCondition) weatherCode.Value<int>();
                return weather;
            }
        }
    }
}
