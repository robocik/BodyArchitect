using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public static class BAHelper
    {
        public static readonly DateTime NullDateTime = DateTime.MinValue;

        public static string ToColorString(this Color yourColor)
        {
            string hexColor = yourColor.A.ToString("X2") + yourColor.R.ToString("X2") + yourColor.G.ToString("X2") + yourColor.B.ToString("X2");
            return hexColor;
        }

        public static Color FromColorString(this string hex)
        {
            Color clientSideColor = Color.FromArgb(Convert.ToByte(hex.Substring(0, 2), 16), Convert.ToByte(hex.Substring(2, 2), 16), Convert.ToByte(hex.Substring(4, 2), 16), Convert.ToByte(hex.Substring(6, 2), 16));
            return clientSideColor;
        }

    }
}
