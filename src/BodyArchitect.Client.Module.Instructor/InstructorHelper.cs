using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor
{
    public static class InstructorHelper
    {
        public static bool IsCustomer(this UserDTO user)
        {
            return CustomersReposidory.Instance.Items.Count(x => x.Value.ConnectedAccount != null && x.Value.ConnectedAccount.GlobalId == user.GlobalId) == 1;
        }


        public static CustomerDTO GetConnectedCustomer(this UserDTO user)
        {
            return CustomersReposidory.Instance.Items.Where(x => x.Value.ConnectedAccount != null && x.Value.ConnectedAccount.GlobalId == user.GlobalId).Select(x => x.Value).SingleOrDefault();
        }

        public static string TranslateInstructor(this string key)
        {
            return EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Instructor:InstructorStrings:" + key);
        }

        public static string Translate(ChampionshipType type)
        {
            if (type == ChampionshipType.ZawodyWyciskanieSztangi)
            {
                return InstructorStrings.ChampionshipType_ZawodyWyciskanieSztangi;
            }
            else
            {
                return  InstructorStrings.ChampionshipType_Trojboj;
            }
        }
    }
}
