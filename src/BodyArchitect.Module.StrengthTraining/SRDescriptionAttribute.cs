using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BodyArchitect.Module.StrengthTraining
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
}
