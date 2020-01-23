using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrHtmlEditor : DevExpress.XtraEditors.XtraUserControl
    {
        public usrHtmlEditor()
        {
            InitializeComponent();
        }

        public bool ReadOnly
        {
            get { return !htmlEditor1.WebBrowserDesignMode; }
            set
            {
                htmlEditor1.WebBrowserDesignMode =!value;
                toolStrip1.Visible = !ReadOnly;
            }
        }

        public string GetHtml()
        {
            if(InvokeRequired)
            {
                return (string)Invoke(new Func<string>(GetHtml));
            }
            else
            {
                return htmlEditor1.BodyHTML;
            }
        }

        public void SetHtml(string html)
        {
            htmlEditor1.SourceHTML = string.IsNullOrEmpty(html)?"<html><body></body></html>":html;
        }

        private void tbBold_Click(object sender, EventArgs e)
        {
            htmlEditor1.SelectionBold = tbBold.Checked;
        }

        private void tbItalic_Click(object sender, EventArgs e)
        {
            htmlEditor1.SelectionItalic = tbItalic.Checked;
        }

        private void tbUnderline_Click(object sender, EventArgs e)
        {
            htmlEditor1.SelectionUnderline = tbUnderline.Checked;
        }

        private void updateButtons(object sender, EventArgs e)
        {
            tbBold.Checked = htmlEditor1.SelectionBold;
            tbItalic.Checked = htmlEditor1.SelectionItalic;
            tbUnderline.Checked = htmlEditor1.SelectionUnderline;
            tbList.Checked = htmlEditor1.SelectionBullets;
            tbNumbered.Checked = htmlEditor1.SelectionNumbering;

            tbPaste.Enabled = htmlEditor1.CanPaste;
            tbCut.Enabled = htmlEditor1.CanCut;
            tbCopy.Enabled = htmlEditor1.CanCopy;

        }

        private void tbNumbered_Click(object sender, EventArgs e)
        {
            htmlEditor1.SelectionNumbering = tbNumbered.Checked;
        }

        private void tbList_Click(object sender, EventArgs e)
        {
            htmlEditor1.SelectionBullets = tbList.Checked;
        }

        public void ClearContent()
        {
            htmlEditor1.ClearContent();
        }

        private void tbCut_Click(object sender, EventArgs e)
        {
            htmlEditor1.Cut();
        }

        private void tbCopy_Click(object sender, EventArgs e)
        {
            htmlEditor1.Copy();
        }

        private void tbPaste_Click(object sender, EventArgs e)
        {
            htmlEditor1.Paste();
        }
    }
}
