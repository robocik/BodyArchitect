using System;
using System.Windows.Forms;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls.Forms
{
    public partial class HtmlPreviewWindow : BaseWindow
    {
        public HtmlPreviewWindow()
        {
            InitializeComponent();
        }

        private IHtmlProvider CurrentHtmlProvider
        {
            get { return (IHtmlProvider) propertyGrid1.SelectedObject; }
        }

        public void Fill(IHtmlProvider htmlProvider)
        {
            propertyGrid1.SelectedObject = htmlProvider;
            webBrowser1.DocumentText = CurrentHtmlProvider.GetHtml();
        }

        private void tbPrint_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }

        private void tbPrintPreview_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            webBrowser1.Document.OpenNew(true);
            webBrowser1.Document.Write(CurrentHtmlProvider.GetHtml());
        }

        private void tbBack_Click(object sender, EventArgs e)
        {
            Fill(CurrentHtmlProvider);
        }
    }
}