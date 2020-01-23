using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using SourceGrid;
using SourceGrid.Cells.Editors;

namespace BodyArchitect.Controls.SourceGridExtension
{
    public class MaskEditEditor : EditorControlBase
    {
        private string mask;

        public MaskEditEditor()
            : base(typeof(string))
        {
        }

        protected MaskEditEditor(Type type)
            : base(type)
        {
        }

        public string MaskRegEx
        {
            get { return mask; }
            set
            {
                mask = value;
                Control.Properties.Mask.EditMask = MaskRegEx;
            }
        }

        protected override Control CreateControl()
        {
            
            TextEdit editor = new TextEdit();
            editor.AutoSize = false;
            editor.Properties.Mask.EditMask = MaskRegEx;
            editor.Properties.Mask.UseMaskAsDisplayFormat = true;
            editor.Properties.Mask.AutoComplete = AutoCompleteType.Optimistic;
            editor.Properties.Mask.IgnoreMaskBlank = true;
            editor.Properties.Mask.SaveLiteral = true;
            editor.Properties.Mask.ShowPlaceHolders = true;
            editor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            return editor;
        }

        public new TextEdit Control
        {
            get
            {
                return (TextEdit)base.Control;
            }
        }
        /// <summary>
        /// This method is called just before the edit start. You can use this method to customize the editor with the cell informations.
        /// </summary>
        /// <param name="cellContext"></param>
        /// <param name="editorControl"></param>
        protected override void OnStartingEdit(CellContext cellContext, Control editorControl)
        {
            base.OnStartingEdit(cellContext, editorControl);

            TextEdit l_TxtBox = (TextEdit)editorControl;
            l_TxtBox.Properties.Appearance.TextOptions.WordWrap = cellContext.Cell.View.WordWrap
                                                                      ? WordWrap.Wrap
                                                                      : WordWrap.NoWrap;
            //l_TxtBox.Properties.Appearance.TextOptions.HAlignment = cellContext.Cell.View.TextAlignment==HorzAlignment.;

            //to set the scroll of the textbox to the initial position (otherwise the textbox use the previous scroll position)
            l_TxtBox.SelectionStart = 0;
            l_TxtBox.SelectionLength = 0;
        }

        /// <summary>
        /// Set the specified value in the current editor control.
        /// </summary>
        /// <param name="editValue"></param>
        public override void SetEditValue(object editValue)
        {
            if (editValue != null)
            {
                Control.EditValue = editValue.ToString();
            }
            Control.SelectAll();
        }

        /// <summary>
        /// Returns the value inserted with the current editor control
        /// </summary>
        /// <returns></returns>
        public override object GetEditedValue()
        {
            return Control.Text;
        }

        protected override void OnSendCharToEditor(char key)
        {
            Control.Text = key.ToString();
            //if (Control.Text != null)
            //Control.SelectionStart = Control.Text.Length;
        }
    }
}
