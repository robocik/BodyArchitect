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

namespace BodyArchitect.Service.V2.Model
{
    public partial class WymiaryDTO
    {
        public WymiaryDTO()
        {
            Time=new BATimeDTO();
        }

        public virtual bool IsEmpty
        {
            get
            {
                return Height == 0 && Weight == 0 && LeftBiceps == 0 && RightBiceps == 0 && Klatka == 0 && RightUdo == 0 && LeftUdo == 0 && RightForearm == 0 && LeftForearm == 0 && Pas == 0;
            }
        }

        public static readonly WymiaryDTO Empty = new WymiaryDTO();
    }
}
