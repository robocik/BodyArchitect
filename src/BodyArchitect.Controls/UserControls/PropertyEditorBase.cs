using System;

// ====================================== '
// Programmer: Saeed Serpooshan, Jan 2007 '
// ====================================== '

using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace BodyArchitect.Controls.UserControls
{
    


    /// <summary>
    /// This is a UITypeEditor base class usefull for simple editing of control properties 
    /// in a DropDown or a ModalDialogForm window at design mode (in VisualStudio.Net IDE). 
    /// To use this, inherits a class from it and add this attribute to your control property(ies): 
    /// &lt;Editor(GetType(MyPropertyEditor), GetType(System.Drawing.Design.UITypeEditor))&gt;  
    /// </summary>
    public abstract class PropertyEditorBase : System.Drawing.Design.UITypeEditor
    {

        /// <summary>
        /// The driven class should provide its edit Control to be shown in the 
        /// DropDown or DialogForm window by means of this function. 
        /// If specified control be a Form, it is shown in a Modal Form, otherwise, it is shown as in a DropDown window. 
        /// This edit control should return its final value at GetEditedValue() method. 
        /// </summary>
        protected abstract Control GetEditControl(string PropertyName, object CurrentValue,object instance);

        /// <summary>The driven class should return the New Value for edited property at this function.</summary>
        /// <param name="EditControl">
        /// The control shown in DropDown window and used for editing. 
        /// This is the control you pass in GetEditControl() function.
        /// </param>
        /// <param name="OldValue">The original value of the property before editing through the DropDown window.</param>
        protected abstract object GetEditedValue(Control EditControl, string PropertyName, object OldValue);

        protected IWindowsFormsEditorService IEditorService;
        private Control withEventsField_m_EditControl;
        private Control m_EditControl
        {
            get { return withEventsField_m_EditControl; }
            set
            {
                if (withEventsField_m_EditControl != null)
                {
                    withEventsField_m_EditControl.PreviewKeyDown -= m_EditControl_PreviewKeyDown;
                }
                withEventsField_m_EditControl = value;
                if (withEventsField_m_EditControl != null)
                {
                    withEventsField_m_EditControl.PreviewKeyDown += m_EditControl_PreviewKeyDown;
                }
            }
        }

        private bool m_EscapePressed;
        /// <summary>
        /// Sets the edit style mode based on the type of EditControl: DropDown or Modal(Dialog). 
        /// Note that the driven class can also override this function and explicitly specify the EditStyle value.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            //try
            //{
            //    Control c = null;
            //    c = GetEditControl(context.PropertyDescriptor.Name, context.PropertyDescriptor.GetValue(context.Instance));
            //    if (c is Form)
            //    {
            //        return UITypeEditorEditStyle.Modal;
            //        //Using a Modal Form
            //    }
            //}
            //catch
            //{
            //}
            //Using a DropDown Window (This is the default style)
            return UITypeEditorEditStyle.DropDown;
        }

        //Displays the Custom UI (a DropDown Control or a Modal Form) for value selection.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            try
            {
                if (context != null && provider != null)
                {
                    //Uses the IWindowsFormsEditorService to display a drop-down UI in the Properties window:
                    IEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                    if (IEditorService != null)
                    {
                        string PropName = context.PropertyDescriptor.Name;
                        m_EditControl = this.GetEditControl(PropName, value,context.Instance);
                        //get Edit Control from driven class


                        if (m_EditControl != null)
                        {
                            m_EscapePressed = false;
                            //we should set this flag to False before showing the control

                            //show given EditControl
                            // => it will be closed if user clicks on outside area or we invoke IEditorService.CloseDropDown()
                            if (m_EditControl is Form)
                            {
                                IEditorService.ShowDialog((Form)m_EditControl);
                            }
                            else
                            {
                                IEditorService.DropDownControl(m_EditControl);
                            }

                            //return the Old Value (because user press Escape)
                            if (m_EscapePressed)
                            {
                                return value;
                                //get new (edited) value from driven class and return it
                            }
                            else
                            {
                                return GetEditedValue(m_EditControl, PropName, value);
                            }

                        }
                        //m_EditControl

                    }
                    //IEditorService

                }
                //context And provider

            }
            catch (Exception)
            {
                //we may show a MessageBox here...
            }
            return base.EditValue(context, provider, value);

        }

        /// <summary>
        /// Provides the interface for this UITypeEditor to display Windows Forms or to 
        /// display a control in a DropDown area from the property grid control in design mode.
        /// </summary>
        public IWindowsFormsEditorService GetIWindowsFormsEditorService()
        {
            return IEditorService;
        }

        /// <summary>Close DropDown window to finish editing</summary>
        public void CloseDropDownWindow()
        {
            if (IEditorService != null)
                IEditorService.CloseDropDown();
        }

        private void m_EditControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                m_EscapePressed = true;
        }

    }





}
