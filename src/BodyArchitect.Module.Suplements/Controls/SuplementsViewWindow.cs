using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Logger;
using BodyArchitect.Module.Suplements.Model;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.Suplements.Controls
{
    public partial class SuplementsViewWindow : BaseWindow
    {
        public SuplementsViewWindow()
        {
            InitializeComponent();
            this.defaultToolTipController1.DefaultController.Active = UserContext.Settings.GuiState.ShowToolTips;
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.defaultToolTipController1.DefaultController, this.lvSuplements, null, SuplementsEntryStrings.SuplementsViewWindow_SuplementsListView);
            ControlHelper.AddSuperTip(this.btnAdd, btnAdd.Text, SuplementsEntryStrings.SuplementsViewWindow_AddBTN);
            ControlHelper.AddSuperTip(this.btnEdit, btnEdit.Text, SuplementsEntryStrings.SuplementsViewWindow_EditBTN);
            ControlHelper.AddSuperTip(this.btnDelete, btnDelete.Text, SuplementsEntryStrings.SuplementsViewWindow_DeleteBTN);
        }

        public void Fill()
        {
            lvSuplements.Items.Clear();
            SuplementsReposidory.ClearCache();
            foreach (SuplementDTO suplement in SuplementsReposidory.Suplements)
            {
                ListViewItem item = new ListViewItem(suplement.Name);
                item.Tag = suplement;
                lvSuplements.Items.Add(item);
            }
            updateButtons();
        }

        private void lvSuplements_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = lvSuplements.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                viewSuplement((SuplementDTO)item.Tag);
            }
        }

        void viewSuplement(SuplementDTO suplement)
        {
            SuplementTypeEditor dlg = new SuplementTypeEditor();
            dlg.Fill(suplement);
            if(dlg.ShowDialog(this)==System.Windows.Forms.DialogResult.OK)
            {
                Fill();
            }
        }

        void updateButtons()
        {
            btnEdit.Enabled = lvSuplements.SelectedIndices.Count == 1;
            btnDelete.Enabled = lvSuplements.SelectedIndices.Count > 0;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
        }
        private void lvSuplements_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var suplement = (SuplementDTO)lvSuplements.SelectedItems[0].Tag;
            viewSuplement(suplement);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteSuplements();
        }

        private void deleteSuplements()
        {
            if(lvSuplements.SelectedItems.Count==0)
            {
                return;
            }
            if (FMMessageBox.AskYesNo(SuplementsEntryStrings.QDeleteSuplementType) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
            try
            {
                //using (var scope = new TransactionScope())
                //{
                //    foreach (var selectedItem in lvSuplements.SelectedItems)
                //    {
                //        Suplement suplement = (Suplement)((ListViewItem)selectedItem).Tag;
                //        suplement.Delete();

                //    }
                //    scope.VoteCommit();
                //}
                MessageBox.Show("Must be implemented");
                Fill();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex, SuplementsEntryStrings.ErrorCannotDeleteSuplementType, ErrorWindow.EMailReport);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            viewSuplement(new SuplementDTO());
        }

        private void lvSuplements_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteSuplements();
            }
        }
    }
}