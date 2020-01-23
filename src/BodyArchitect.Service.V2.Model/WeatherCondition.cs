using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public enum WeatherCondition
    {
        NotSet,
        ModerateOrHeavySnowWithThunder=395,
        PatchyLightSnowWithThunder=392,
        ModerateOrHeavyRainAreaWithThunder=389,
        PatchyLightRainWithThunder=386,
        ModerateOrHeavyShowersOfIcePellets=377,
        LightShowersOfIcePellets=374,
        ModerateOrHeavySnowShowers=371,
        LightSnowShowers=368,
        ModerateOrHeavySleetShowers=365,
        LightSleetShowers=362,
        TorrentialRainShower=359,
        ModerateOrHeavyRainShower=356,
        LightRainShower=353,
        IcePellets=350,
        HeavySnow=338,
        PatchyHeavySnow=335,
        ModerateSnow=332,
        PatchyModerateSnow=329,
        LightSnow=326,
        PatchyLightSnow=323,
        ModerateOrHeavySleet=320,
        LightSleet=317,
        ModerateOrHeavyFreezingRain=314,
        LightFreezingRain=311,
        HeavyRain=308,
        HeavyRainAtTimes=305,
        ModerateRain=302,
        ModerateRainAtTimes=299,
        LightRain=296,
        PatchyLightRain=293,
        HeavyFreezingDrizzle=284,
        FreezingDrizzle=281,
        LightDrizzle=266,
        PatchyLightDrizzle=263,
        FreezingFog=260,
        Fog=248,
        Blizzard=230,
        BlowingSnow = 227,
        ThunderyOutbreaksNearby=200,
        PatchyFreezingDrizzleNearby=185,
        PatchySleet=182,
        PatchySnow=179,
        PatchyRain=176,
        Mist=143,
        Overcast=122,
        Cloudy=119,
        PartlyCloudy=116,
        ClearSunny=113
    }
}
