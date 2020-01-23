using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace BodyArchitect.Client.Common
{
    /// <summary>
    /// Represents a WPF color name with hex representations so Silverlight apps can access the full spectrum.
    /// 
    /// Sources
    /// 
    /// Various pieces of code and ideas came from the following:
    /// 
    /// stackoverflow question
    /// http://stackoverflow.com/questions/1203843/accessing-all-the-many-colors-of-wpf-but-in-silverlight
    /// 
    /// Silverlight Forum post
    /// http://silverlight.net/forums/t/13225.aspx
    /// 
    /// WPF Colors
    /// http://msdn.microsoft.com/en-us/library/system.windows.media.colors_members.aspx
    /// 
    /// Silverlight colors
    /// http://msdn.microsoft.com/en-us/library/system.windows.media.colors_members%28VS.95%29.aspx
    /// 
    /// </summary>
    public class ColorName
    {
        /// <summary>
        /// Get System.Windows.Media.Color from ColorName.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator Color(ColorName color)
        {
            uint value = color;

            return (Color.FromArgb(
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value)
            );
        }

        public static implicit operator Brush(ColorName color)
        {
            return (new SolidColorBrush(color));
        }

        public static implicit operator uint(ColorName color)
        {
            return (color._colorValue);
        }

        public static implicit operator ColorName(uint color)
        {
            return (new ColorName(color));
        }

        private readonly uint _colorValue;

        public ColorName(uint color)
        {
            _colorValue = color;
        }

        /// <summary>
        /// Create ColorName object given a WPF color name.
        /// </summary>
        /// <param name="color"></param>
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="NotSupportedException" />
        /// <exception cref="TargetException" />
        /// <exception cref="FieldAccessException" />
        /// <exception cref="MethodAccessException" />
        public ColorName(string color)
        {
            if (string.IsNullOrEmpty(color)) throw new ArgumentOutOfRangeException();

            if (color[0] == '#')	// see if first position is the hash character
            {
                _colorValue = GetColorFromHexString(color);
            }
            else
            {
                var fieldInfo = GetType().GetField(color);
                _colorValue = (ColorName)fieldInfo.GetValue(this);
            }
        }

        /// <summary>
        /// Get uint representation of Color from hex-formatted string.
        /// </summary>
        /// <param name="hexFormattedColor">A hex-formatted string value in the format #FFAABBCC or #AABBCC.</param>
        /// <returns></returns>
        private static uint GetColorFromHexString(string hexFormattedColor)
        {
            if (hexFormattedColor[0] != '#') throw new ArgumentOutOfRangeException(hexFormattedColor, "The color value is not in the expected hex format.");

            var hexColor = hexFormattedColor.Substring(1);	// take out the leading '#'

            switch (hexColor.Length)
            {
                case 6:

                    // convert, add default alpha value
                    return (UInt32.Parse("FF" + hexColor, System.Globalization.NumberStyles.HexNumber));

                case 8:

                    return (UInt32.Parse(hexColor, System.Globalization.NumberStyles.HexNumber));

                default:

                    throw new ArgumentOutOfRangeException(hexColor, "The color value is not in the expected hex format.");
            }
        }

        public static readonly ColorName AliceBlue = 0xFFF0F8FF;
        public static readonly ColorName AntiqueWhite = 0xFFFAEBD7;
        public static readonly ColorName Aqua = 0xFF00FFFF;
        public static readonly ColorName Aquamarine = 0xFF7FFFD4;
        public static readonly ColorName Azure = 0xFFF0FFFF;
        public static readonly ColorName Beige = 0xFFF5F5DC;
        public static readonly ColorName Bisque = 0xFFFFE4C4;
        public static readonly ColorName Black = 0xFF000000;
        public static readonly ColorName BlanchedAlmond = 0xFFFFEBCD;
        public static readonly ColorName Blue = 0xFF0000FF;
        public static readonly ColorName BlueViolet = 0xFF8A2BE2;
        public static readonly ColorName Brown = 0xFFA52A2A;
        public static readonly ColorName BurlyWood = 0xFFDEB887;
        public static readonly ColorName CadetBlue = 0xFF5F9EA0;
        public static readonly ColorName Chartreuse = 0xFF7FFF00;
        public static readonly ColorName Chocolate = 0xFFD2691E;
        public static readonly ColorName Coral = 0xFFFF7F50;
        public static readonly ColorName CornflowerBlue = 0xFF6495ED;
        public static readonly ColorName Cornsilk = 0xFFFFF8DC;
        public static readonly ColorName Crimson = 0xFFDC143C;
        public static readonly ColorName Cyan = 0xFF00FFFF;
        public static readonly ColorName DarkBlue = 0xFF00008B;
        public static readonly ColorName DarkCyan = 0xFF008B8B;
        public static readonly ColorName DarkGoldenrod = 0xFFB8860B;
        public static readonly ColorName DarkGray = 0xFFA9A9A9;
        public static readonly ColorName DarkGreen = 0xFF006400;
        public static readonly ColorName DarkKhaki = 0xFFBDB76B;
        public static readonly ColorName DarkMagenta = 0xFF8B008B;
        public static readonly ColorName DarkOliveGreen = 0xFF556B2F;
        public static readonly ColorName DarkOrange = 0xFFFF8C00;
        public static readonly ColorName DarkOrchid = 0xFF9932CC;
        public static readonly ColorName DarkRed = 0xFF8B0000;
        public static readonly ColorName DarkSalmon = 0xFFE9967A;
        public static readonly ColorName DarkSeaGreen = 0xFF8FBC8F;
        public static readonly ColorName DarkSlateBlue = 0xFF483D8B;
        public static readonly ColorName DarkSlateGray = 0xFF2F4F4F;
        public static readonly ColorName DarkTurquoise = 0xFF00CED1;
        public static readonly ColorName DarkViolet = 0xFF9400D3;
        public static readonly ColorName DeepPink = 0xFFFF1493;
        public static readonly ColorName DeepSkyBlue = 0xFF00BFFF;
        public static readonly ColorName DimGray = 0xFF696969;
        public static readonly ColorName DodgerBlue = 0xFF1E90FF;
        public static readonly ColorName Firebrick = 0xFFB22222;
        public static readonly ColorName FloralWhite = 0xFFFFFAF0;
        public static readonly ColorName ForestGreen = 0xFF228B22;
        public static readonly ColorName Fuchsia = 0xFFFF00FF;
        public static readonly ColorName Gainsboro = 0xFFDCDCDC;
        public static readonly ColorName GhostWhite = 0xFFF8F8FF;
        public static readonly ColorName Gold = 0xFFFFD700;
        public static readonly ColorName Goldenrod = 0xFFDAA520;
        public static readonly ColorName Gray = 0xFF808080;
        public static readonly ColorName Green = 0xFF008000;
        public static readonly ColorName GreenYellow = 0xFFADFF2F;
        public static readonly ColorName Honeydew = 0xFFF0FFF0;
        public static readonly ColorName HotPink = 0xFFFF69B4;
        public static readonly ColorName IndianRed = 0xFFCD5C5C;
        public static readonly ColorName Indigo = 0xFF4B0082;
        public static readonly ColorName Ivory = 0xFFFFFFF0;
        public static readonly ColorName Khaki = 0xFFF0E68C;
        public static readonly ColorName Lavender = 0xFFE6E6FA;
        public static readonly ColorName LavenderBlush = 0xFFFFF0F5;
        public static readonly ColorName LawnGreen = 0xFF7CFC00;
        public static readonly ColorName LemonChiffon = 0xFFFFFACD;
        public static readonly ColorName LightBlue = 0xFFADD8E6;
        public static readonly ColorName LightCoral = 0xFFF08080;
        public static readonly ColorName LightCyan = 0xFFE0FFFF;
        public static readonly ColorName LightGoldenrodYellow = 0xFFFAFAD2;
        public static readonly ColorName LightGray = 0xFFD3D3D3;
        public static readonly ColorName LightGreen = 0xFF90EE90;
        public static readonly ColorName LightPink = 0xFFFFB6C1;
        public static readonly ColorName LightSalmon = 0xFFFFA07A;
        public static readonly ColorName LightSeaGreen = 0xFF20B2AA;
        public static readonly ColorName LightSkyBlue = 0xFF87CEFA;
        public static readonly ColorName LightSlateGray = 0xFF778899;
        public static readonly ColorName LightSteelBlue = 0xFFB0C4DE;
        public static readonly ColorName LightYellow = 0xFFFFFFE0;
        public static readonly ColorName Lime = 0xFF00FF00;
        public static readonly ColorName LimeGreen = 0xFF32CD32;
        public static readonly ColorName Linen = 0xFFFAF0E6;
        public static readonly ColorName Magenta = 0xFFFF00FF;
        public static readonly ColorName Maroon = 0xFF800000;
        public static readonly ColorName MediumAquamarine = 0xFF66CDAA;
        public static readonly ColorName MediumBlue = 0xFF0000CD;
        public static readonly ColorName MediumOrchid = 0xFFBA55D3;
        public static readonly ColorName MediumPurple = 0xFF9370DB;
        public static readonly ColorName MediumSeaGreen = 0xFF3CB371;
        public static readonly ColorName MediumSlateBlue = 0xFF7B68EE;
        public static readonly ColorName MediumSpringGreen = 0xFF00FA9A;
        public static readonly ColorName MediumTurquoise = 0xFF48D1CC;
        public static readonly ColorName MediumVioletRed = 0xFFC71585;
        public static readonly ColorName MidnightBlue = 0xFF191970;
        public static readonly ColorName MintCream = 0xFFF5FFFA;
        public static readonly ColorName MistyRose = 0xFFFFE4E1;
        public static readonly ColorName Moccasin = 0xFFFFE4B5;
        public static readonly ColorName NavajoWhite = 0xFFFFDEAD;
        public static readonly ColorName Navy = 0xFF000080;
        public static readonly ColorName OldLace = 0xFFFDF5E6;
        public static readonly ColorName Olive = 0xFF808000;
        public static readonly ColorName OliveDrab = 0xFF6B8E23;
        public static readonly ColorName Orange = 0xFFFFA500;
        public static readonly ColorName OrangeRed = 0xFFFF4500;
        public static readonly ColorName Orchid = 0xFFDA70D6;
        public static readonly ColorName PaleGoldenrod = 0xFFEEE8AA;
        public static readonly ColorName PaleGreen = 0xFF98FB98;
        public static readonly ColorName PaleTurquoise = 0xFFAFEEEE;
        public static readonly ColorName PaleVioletRed = 0xFFDB7093;
        public static readonly ColorName PapayaWhip = 0xFFFFEFD5;
        public static readonly ColorName PeachPuff = 0xFFFFDAB9;
        public static readonly ColorName Peru = 0xFFCD853F;
        public static readonly ColorName Pink = 0xFFFFC0CB;
        public static readonly ColorName Plum = 0xFFDDA0DD;
        public static readonly ColorName PowderBlue = 0xFFB0E0E6;
        public static readonly ColorName Purple = 0xFF800080;
        public static readonly ColorName Red = 0xFFFF0000;
        public static readonly ColorName RosyBrown = 0xFFBC8F8F;
        public static readonly ColorName RoyalBlue = 0xFF4169E1;
        public static readonly ColorName SaddleBrown = 0xFF8B4513;
        public static readonly ColorName Salmon = 0xFFFA8072;
        public static readonly ColorName SandyBrown = 0xFFF4A460;
        public static readonly ColorName SeaGreen = 0xFF2E8B57;
        public static readonly ColorName SeaShell = 0xFFFFF5EE;
        public static readonly ColorName Sienna = 0xFFA0522D;
        public static readonly ColorName Silver = 0xFFC0C0C0;
        public static readonly ColorName SkyBlue = 0xFF87CEEB;
        public static readonly ColorName SlateBlue = 0xFF6A5ACD;
        public static readonly ColorName SlateGray = 0xFF708090;
        public static readonly ColorName Snow = 0xFFFFFAFA;
        public static readonly ColorName SpringGreen = 0xFF00FF7F;
        public static readonly ColorName SteelBlue = 0xFF4682B4;
        public static readonly ColorName Tan = 0xFFD2B48C;
        public static readonly ColorName Teal = 0xFF008080;
        public static readonly ColorName Thistle = 0xFFD8BFD8;
        public static readonly ColorName Tomato = 0xFFFF6347;
        public static readonly ColorName Transparent = 0x00FFFFFF;
        public static readonly ColorName Turquoise = 0xFF40E0D0;
        public static readonly ColorName Violet = 0xFFEE82EE;
        public static readonly ColorName Wheat = 0xFFF5DEB3;
        public static readonly ColorName White = 0xFFFFFFFF;
        public static readonly ColorName WhiteSmoke = 0xFFF5F5F5;
        public static readonly ColorName Yellow = 0xFFFFFF00;
        public static readonly ColorName YellowGreen = 0xFF9ACD32;
    }

    public static class EntryObjectColors
    {
        public static readonly Brush StrengthTraining = ColorName.LightBlue;

        public static readonly Brush GPSTracker = ColorName.YellowGreen;

        public static readonly Brush A6W = ColorName.AntiqueWhite;

        public static readonly Brush Blog = ColorName.DarkOrange;

        public static readonly Brush Measurements = ColorName.LightCoral;

        public static readonly Brush Supplements = ColorName.LightGreen;

    }
}
