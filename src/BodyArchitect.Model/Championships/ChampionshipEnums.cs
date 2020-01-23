using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public enum ChampionshipTryResult
    {
        NotDone,
        Success,
        Fail
    }

    public enum ChampionshipType
    {
        ZawodyWyciskanieSztangi,
        Trojboj
    }

    public enum ChampionshipCategoryType
    {
        Weight,
        Open
    }

    public enum ChampionshipWinningCategories
    {
        Seniorzy,
        JuniorzyMlodsi,
        Juniorzy,
        Weterani1,
        Weterani2,
        Weterani3,
        Weterani4,
        MistrzMistrzow,
        Druzynowa
    }
}
