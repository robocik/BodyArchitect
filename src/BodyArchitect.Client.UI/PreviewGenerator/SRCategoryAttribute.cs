using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace BodyArchitect.Client.UI.PreviewGenerator
{
    /// <summary>
    /// Localized description attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class SRDescriptionAttribute : DescriptionAttribute
    {
        /// <summary>
        /// Construct the description attribute
        /// </summary>
        /// <param name="text"></param>
        public SRDescriptionAttribute(string text)
            : base(text)
        {
            _localized = false;
        }

        /// <summary>
        /// Override the return of the description text to localize the text
        /// </summary>
        public override string Description
        {
            get
            {
                if (!_localized)
                {
                    _localized = true;
                    this.DescriptionValue = SR.GetString(this.DescriptionValue);
                }

                return base.Description;
            }
        }

        /// <summary>
        /// Store a flag indicating whether this has been localized
        /// </summary>
        private bool _localized;
    }
    /// <summary>
    /// Localized version of the Category attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class SRCategoryAttribute : CategoryAttribute
    {
        /// <summary>
        /// Construct the attribute
        /// </summary>
        /// <param name="name"></param>
        public SRCategoryAttribute(string name) : base(name) { }

        /// <summary>
        /// Return the localized version of the passed string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override string GetLocalizedString(string value)
        {
            return SR.GetString(value);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class SRDisplayNameAttribute : DisplayNameAttribute
    {
        /// <summary>
        /// Store a flag indicating whether this has been localized
        /// </summary>
        private bool _localized;

        /// <summary>
        /// Construct the attribute
        /// </summary>
        /// <param name="name"></param>
        public SRDisplayNameAttribute(string name) : base(name) { }


        public override string DisplayName
        {
            get
            {
                if (!_localized)
                {
                    _localized = true;

                    this.DisplayNameValue = SR.GetString(this.DisplayName);
                }

                return base.DisplayName;
            }
        }
    }

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
            _rm = new System.Resources.ResourceManager("BodyArchitect.Client.Resources.Localization.Strings", Assembly.Load(new AssemblyName("BodyArchitect.Client.Resources")));
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
