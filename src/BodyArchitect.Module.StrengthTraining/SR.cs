using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

namespace BodyArchitect.Module.StrengthTraining
{
    /// <summary>
    /// Class which exposes string resources
    /// </summary>
    public sealed class SR
    {
        #region Instance methods and data

        /// <summary>
        /// Private constructor
        /// </summary>
        private SR()
        {
            _rm = new System.Resources.ResourceManager("BodyArchitect.Module.StrengthTraining.Localization.LocalizedPropertyGridStrings", this.GetType().Assembly);
        }

        /// <summary>
        /// Return the resource manager for the assembly
        /// </summary>
        private ResourceManager Resources
        {
            get { return _rm; }
        }

        /// <summary>
        /// Store the resource manager
        /// </summary>
        private ResourceManager _rm;

        #endregion

        #region Static methods and data

        /// <summary>
        /// Return the static loader instance
        /// </summary>
        /// <returns></returns>
        private static SR GetLoader()
        {
            if (null == _loader)
            {
                lock (_lock)
                {
                    if (null == _loader)
                        _loader = new SR();
                }
            }

            return _loader;
        }

        /// <summary>
        /// Get a string resource
        /// </summary>
        /// <param name="name">The resource name</param>
        /// <returns>The localized resource</returns>
        public static string GetString(string name)
        {
            SR loader = GetLoader();
            string localized = null;

            if (null != loader)
                localized = loader.Resources.GetString(name, null);

            return localized;
        }

        /// <summary>
        /// Get the localized string for a particular culture
        /// </summary>
        /// <param name="culture">The culture for which the string is desired</param>
        /// <param name="name">The resource name</param>
        /// <returns>The localized resource</returns>
        public static string GetString(CultureInfo culture, string name)
        {
            SR loader = GetLoader();
            string localized = null;

            if (null != loader)
                localized = loader.Resources.GetString(name, culture);

            return localized;
        }

        /// <summary>
        /// Cache the one and only instance of the loader
        /// </summary>
        private static SR _loader = null;

        /// <summary>
        /// Object used to lock
        /// </summary>
        private static object _lock = new object();

        #endregion
    }
}
