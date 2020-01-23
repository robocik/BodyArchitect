using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Controls.Basic;
using BodyArchitect.Controls.Localization;
using SourceGrid.Cells.Editors;

namespace BodyArchitect.Module.StrengthTraining.Controls.SourceGridExtension
{
    class LookupEditEditor : LookUpEditEditorBase<ExerciseLookUp>
    {
        #region Constructor
        /// <summary>
        /// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
        /// </summary>
        /// <param name="p_Type">The type of this model</param>
        public LookupEditEditor(Type p_Type)
            : base(p_Type)
        {
            
        }

        /// <summary>
        /// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
        /// </summary>
        /// <param name="p_Type">The type of this model</param>
        /// <param name="p_StandardValues"></param>
        /// <param name="p_StandardValueExclusive">True to not allow custom value, only the values specified in the standardvalues collection are allowed.</param>
        public LookupEditEditor(Type p_Type, ICollection p_StandardValues, bool p_StandardValueExclusive)
            : base(p_Type, p_StandardValues, p_StandardValueExclusive)
        {

        }
        #endregion
    }
}
