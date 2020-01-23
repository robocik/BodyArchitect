using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using SourceGrid.Cells.Editors;

namespace BodyArchitect.Controls.Basic
{
    public class LookUpEditEditorBase<T> : EditorControlBase where T : LookUpEdit,new()
    {
        #region Constructor
        /// <summary>
        /// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
        /// </summary>
        /// <param name="p_Type">The type of this model</param>
        public LookUpEditEditorBase(Type p_Type)
            : base(p_Type)
        {
            
        }

        /// <summary>
        /// Construct a Model. Based on the Type specified the constructor populate AllowNull, DefaultValue, TypeConverter, StandardValues, StandardValueExclusive
        /// </summary>
        /// <param name="p_Type">The type of this model</param>
        /// <param name="p_StandardValues"></param>
        /// <param name="p_StandardValueExclusive">True to not allow custom value, only the values specified in the standardvalues collection are allowed.</param>
        public LookUpEditEditorBase(Type p_Type, ICollection p_StandardValues, bool p_StandardValueExclusive)
            : base(p_Type)
        {
            StandardValues = p_StandardValues;
            StandardValuesExclusive = p_StandardValueExclusive;
        }
        #endregion

        

        
        #region Edit Control
        /// <summary>
        /// Create the editor control
        /// </summary>
        /// <returns></returns>
        protected override Control CreateControl()
        {
            T editor = new T();
            editor.PreviewKeyDown += new PreviewKeyDownEventHandler(editor_PreviewKeyDown);

            return editor;
        }

        void editor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyValue==13)
            {
                Control.SendKey(new KeyEventArgs(Keys.Enter));
            }
        }

        protected override void OnConvertingValueToObject(DevAge.ComponentModel.ConvertingObjectEventArgs e)
        {
            e.Value = Control.Text;
            base.OnConvertingValueToObject(e);
        }
        protected override void OnConvertingValueToDisplayString(DevAge.ComponentModel.ConvertingObjectEventArgs e)
        {
            Control.Properties.ForceInitialize();
            if (e.Value != null)
            {
                Control.EditValue = e.Value;
            }
            base.OnConvertingValueToDisplayString(e);
        }



        /// <summary>
        /// Gets the control used for editing the cell.
        /// </summary>
        public new T Control
        {
            get
            {
                return (T)base.Control;
            }
        }

        public object SelectedItem
        {
            get
            {
                return Control.Properties.GetDataSourceRowByKeyValue(Control.EditValue);
            }
        }
        #endregion

        /// <summary>
        /// Set the specified value in the current editor control.
        /// </summary>
        /// <param name="editValue"></param>
        public override void SetEditValue(object editValue)
        {
            if (editValue is string && IsStringConversionSupported() )
            {
                //Control.SelectedIndex = -1;
                Control.Text = (string)editValue;
                Control.SelectionLength = 0;
                if (Control.Text != null)
                    Control.SelectionStart = Control.Text.Length;
                else
                    Control.SelectionStart = 0;
            }
            else
            {
                //Control.SelectedIndex = -1;
                //Control.Value = editValue;
                //Control.SelectAll();
            }
        }

        /// <summary>
        /// Returns the value inserted with the current editor control
        /// </summary>
        /// <returns></returns>
        public override object GetEditedValue()
        {
            return Control.EditValue;
        }

        protected override void OnSendCharToEditor(char key)
        {
            Control.Text = key.ToString();
            if (Control.Text != null)
                Control.SelectionStart = Control.Text.Length;
        }
    }
}
