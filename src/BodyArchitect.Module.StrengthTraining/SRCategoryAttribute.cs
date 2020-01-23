using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BodyArchitect.Module.StrengthTraining
{
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
}
