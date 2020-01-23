using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Basic;
using DevExpress.XtraEditors.Controls;
using BodyArchitect.Module.Suplements.Model;

namespace BodyArchitect.Module.Suplements.Controls.SourceGridExtension
{
    class LookupEditEditor : LookUpEditEditorBase<LookUpEdit>
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

        #endregion

        protected override System.Windows.Forms.Control CreateControl()
        {
            var control= (LookUpEdit)base.CreateControl();
            control.Properties.ValueMember = "SuplementId";
            control.Properties.DropDownRows = 20;
            control.Properties.DisplayMember = "Name";
            control.Properties.Columns.AddRange(CreateColumns());
            control.Properties.NullValuePrompt = SuplementsEntryStrings.SelectSuplementType;
            control.Properties.DataSource = SuplementsReposidory.Suplements;
            control.Properties.ForceInitialize();
            return control;
        }

        public static LookUpColumnInfo[] CreateColumns()
        {
            return new LookUpColumnInfo[] { 
                    new LookUpColumnInfo("Name", SuplementsEntryStrings.ColumnSuplementName)
                };
        }
    }
}
