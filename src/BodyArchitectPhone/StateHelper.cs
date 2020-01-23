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
    public class StateHelper
    {
        private IDictionary<string, object> state;

        public StateHelper(IDictionary<string, object> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }
            this.state = state;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object value;
            if (state.TryGetValue(key, out value))
            {
                if (value is T)
                {
                    return (T)value;
                }
            }
            return defaultValue;
        }
    }
}
