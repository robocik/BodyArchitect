using System;
using System.Windows.Controls;
using BodyArchitect.DataAccess.Converter.V4_V5;

namespace BodyArchitect.Admin
{
    class DbConverterCallback : IBADatabaseCallback
    {
        private System.Windows.Controls.ListBox lstResults;
        private ListBox lstResultsDetails;
        private ProgressBar progress;

        public DbConverterCallback(ListBox lstResults, ListBox lstResultsDetails, ProgressBar progress)
        {
            this.lstResults = lstResults;
            this.lstResultsDetails = lstResultsDetails;
            this.progress = progress;
        }
        public void ConvertProgressChanged(BADatabaseCallbackParam param)
        {
            lstResults.Dispatcher.BeginInvoke(new Action(()=>
            {
                if (!string.IsNullOrEmpty(param.MainOperation))
                {
                    lstResults.Items.Add(string.Format("{0}:{1}", DateTime.Now, param.MainOperation));
                }
                if (!string.IsNullOrEmpty(param.DetailOperation))
                {
                    lstResultsDetails.Items.Add(string.Format("{0}:{1}", DateTime.Now, param.DetailOperation));
                }
                if (param.Max > 0)
                {
                    progress.Maximum = param.Max;
                    progress.Value = param.Current;
                }
            }));
            
        }
    }
}
