using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.WP7
{
    public class SuperSetViewManager
    {
        Dictionary<string, Color> usedSuperSetColors = new Dictionary<string, Color>();
        List<Color> superSetColors = new List<Color>() { Colors.Orange, Colors.Green, Colors.Blue, Colors.Red, Colors.Brown, Colors.Magenta };

        public Color GetSuperSetColor(string superSetGroup)
        {
            if (usedSuperSetColors.ContainsKey(superSetGroup))
            {
                return usedSuperSetColors[superSetGroup];
            }
            //we need to create a new color
            foreach (Color color in superSetColors)
            {
                if (!usedSuperSetColors.ContainsValue(color))
                {
                    usedSuperSetColors.Add(superSetGroup, color);
                    return color;
                }
            }
            return (Color)Application.Current.Resources["CustomForegroundColor"];
        }
    }
}
