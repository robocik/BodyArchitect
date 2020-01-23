using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using SourceGrid;
using SourceGrid.Cells.Editors;
using SourceGrid.Cells.Views;

namespace BodyArchitect.Controls.SourceGridExtension
{
    public class TimeEditView : Cell
    {
        protected override void PrepareVisualElementText(CellContext context)
        {
            base.PrepareVisualElementText(context);
            if (context.Cell.Editor.EnableEdit && context.Value !=null)
            {
                var value = (DateTime) context.Value;
                //context.Value = "test";
                var timeEditControl = ((TimeEditEditor) context.Cell.Editor).Control;
                if (value.TimeOfDay != TimeSpan.Zero)
                {
                    timeEditControl.Time = value;
                    ElementText.Value = timeEditControl.Text;
                    return;
                }
            }
            ElementText.Value = string.Empty;
        }
    }


    public class TimeEditEditor : EditorControlBase
    {
        public TimeEditEditor()
            : base(typeof(DateTime))
        {
        }



        protected override Control CreateControl()
        {

            TimeEdit editor = new TimeEdit();
            editor.AutoSize = false;
            //editor.Properties.TimeFormat = TimeFormat.HourMin;
            editor.Properties.EditMask = "hh:mm tt";
            return editor;
        }

        public new TimeEdit Control
        {
            get
            {
                return (TimeEdit)base.Control;
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
                Control.Time = (DateTime) editValue;
            }
            Control.SelectAll();
        }

        /// <summary>
        /// Returns the value inserted with the current editor control
        /// </summary>
        /// <returns></returns>
        public override object GetEditedValue()
        {
            return Control.Time;
        }

        protected override void OnSendCharToEditor(char key)
        {
            Control.Text = key.ToString();
            //if (Control.Text != null)
            //Control.SelectionStart = Control.Text.Length;
        }
    }
}
