using System;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Module.Suplements.Controls
{
    public partial class SuplementTypeEditor : BaseWindow
    {
        private SuplementDTO suplement;

        public SuplementTypeEditor()
        {
            InitializeComponent();
            txtName.Properties.MaxLength = Constants.NameColumnLength;
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtName, lblName.Text, SuplementsEntryStrings.SuplementTypeEditor_NameTXT);
            ControlHelper.AddSuperTip(this.txtComment, lblComment.Text, SuplementsEntryStrings.SuplementTypeEditor_CommentTXT);
        }

        void setReadOnly(bool isReadOnly)
        {
            txtName.Properties.ReadOnly = isReadOnly;
            txtComment.Properties.ReadOnly = isReadOnly;
            okButton1.Enabled = !isReadOnly;

        }
        public void Fill(SuplementDTO suplement)
        {
            bool isReadOnly=suplement.ProfileId == null && !suplement.IsNew;
            this.suplement = suplement;
            txtName.Text = suplement.Name;
            txtComment.Text = suplement.Comment;

            setReadOnly(isReadOnly);
        }

        public void UpdateEntry()
        {
            suplement.Name = txtName.Text;
            suplement.Comment = txtComment.Text;
            suplement.ProfileId = UserContext.CurrentProfile.Id;
            ServiceManager.SaveSuplement(suplement);
        }

        private void txtName_EditValueChanged(object sender, EventArgs e)
        {
            okButton1.Enabled = !string.IsNullOrWhiteSpace(txtName.Text);
        }

        private void okButton1_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateEntry();
            }
            //catch (ActiveRecordValidationException uniqueException)
            //{
            //    DialogResult = DialogResult.None;
            //    ExceptionHandler.Default.Process(uniqueException);
            //}
            catch (Exception ex)
            {
                DialogResult = System.Windows.Forms.DialogResult.None;
                ExceptionHandler.Default.Process(ex, SuplementsEntryStrings.ErrorDuringSuplementSaving, ErrorWindow.EMailReport);
            }
        }
    }
}