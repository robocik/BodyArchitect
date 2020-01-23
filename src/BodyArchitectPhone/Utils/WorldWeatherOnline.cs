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

namespace BodyArchitect.WP7.Utils
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

        public event EventHandler<WeatherRetrievedEventArgs> WeatherLoaded;

        public void BeginLoadWeather(GPSPoint location)
        {
            string weatherUrl = "http://free.worldweatheronline.com/feed/weather.ashx?q={0},{1}&num_of_days=1&format=json&key={2}";
            double latitude = location.Latitude;
            double longitude = location.Longitude;
            //InvariantCulture is to force . (dot) for latitude and longitude
            string url = string.Format(weatherUrl, latitude.ToString(CultureInfo.InvariantCulture), longitude.ToString(CultureInfo.InvariantCulture), APIKey);

            var request = HttpWebRequest.Create(url);
            request.BeginGetResponse((iar) =>
            {
                try
                {
                    var response = request.EndGetResponse(iar);

                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("iso-8859-1")))
                    {
                        string contents = reader.ReadToEnd();
                        var obj = (JObject)JsonConvert.DeserializeObject(contents);
                        var tempC = obj["data"]["current_condition"][0]["temp_C"];
                        var weatherCode = obj["data"]["current_condition"][0]["weatherCode"];
                        WeatherDTO weather = new WeatherDTO();
                        weather.Temperature = tempC.Value<float>();
                        weather.Condition = (WeatherCondition)weatherCode.Value<int>();
                        if (WeatherLoaded != null)
                        {
                            WeatherLoaded(this, new WeatherRetrievedEventArgs(weather));
                        }
                    }
                }
                catch (Exception)
                {
                    if (WeatherLoaded != null)
                    {
                        WeatherLoaded(this, new WeatherRetrievedEventArgs(new WeatherDTO()));
                    }
                }
            }, null);
        }
    }
}
