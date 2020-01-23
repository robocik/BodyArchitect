using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using SourceGrid;
using SourceGrid.Cells.Editors;

namespace BodyArchitect.Controls.SourceGridExtension
{
    public class MemoExEditEditor : EditorControlBase
    {
        public MemoExEditEditor()
            : base(typeof(string))
        {
        }

        protected override Control CreateControl()
        {
            MemoExEdit editor = new MemoExEdit();
            editor.AutoSize = false;

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
            Control.Text = (string)editValue;
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
