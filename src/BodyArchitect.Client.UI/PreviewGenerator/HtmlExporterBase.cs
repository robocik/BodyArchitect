
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.PreviewGenerator
{
    [Serializable]
    public abstract class HtmlExporterBase : IDisposable, IHtmlProvider
    {
        //private bool initialPrintBackground;
        private string title;
        private bool printHeaders = true;
        

        protected HtmlExporterBase(string title)
        {
            //initialPrintBackground = PrintBackground;
            this.title = title;
        }


        protected string normalizeString(string text)
        {
            if(text!=null)
            {
                return text.Replace("\n", "<br/>");
            }
            return text;
        }

        

        [SRDescription("TrainingPlanHtmlExporter.PrintHeaders.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintHeaders.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintHeaders
        {
            get { return printHeaders; }
            set { printHeaders = value; }
        }

        //[SRDescription("TrainingPlanHtmlExporter.PrintBackground.Description")]
        //[SRDisplayName("TrainingPlanHtmlExporter.PrintBackground.DisplayName")]
        //[SRCategory("PrintOptions")]
        //[DefaultValue(false)]
        //public bool PrintBackground
        //{
        //    get
        //    {
        //        try
        //        {
        //            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Internet Explorer").OpenSubKey("Main");

        //            //Get the current setting so that we can revert it after printjob
        //            object defaultPrintBackground = regKey.GetValue("Print_Background");
        //            return object.Equals((string)defaultPrintBackground, "yes");
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionHandler.Default.Process(ex);
        //        }
        //        return false;
        //    }
        //    set
        //    {
        //        try
        //        {
        //            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Internet Explorer").OpenSubKey("Main", true);
        //            regKey.SetValue("Print_Background", value ? "yes" : "no");
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionHandler.Default.Process(ex);
        //        }

        //    }
        //}

        public void Dispose()
        {
            //PrintBackground = initialPrintBackground;
        }

        #region Implementation of IHtmlProvider

        public string GetHtml()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/>");
            buildHtmlHead(builder);
            builder.AppendLine("<body>");
            builder.AppendFormat("<h1>{0}</h1>", title);
            builder.AppendLine("<table class='main'>");

            Build(builder);

            builder.AppendLine("</table>");
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            return builder.ToString();
        }

        [CategoryAttribute("Name")]
        public abstract string Title { get; }

        public abstract Guid GlobalId { get; }
        public abstract string Language { get; }

        private void buildHtmlHead(StringBuilder builder)
        {
            builder.AppendFormat("<link rel='stylesheet' href='{0}' type='text/css' />", Path.Combine(Helper.StartUpPath, "Css\\TrainingPlan.css"));
            builder.AppendFormat("<title>{0}</title>", title);
        }

        protected void addHeader(string title, StringBuilder builder)
        {
            if (PrintHeaders)
            {
                builder.AppendFormat("<tr><th class='label' colspan='2'>{0}</th></tr>", title);
            }
        }

        protected void addGroupTable(StringBuilder builder, string header, HtmlProcessMethod method)
        {
            builder.AppendLine("<tr><td><table cellspacing='0' cellpadding='6' class='group'>");
            addHeader(header, builder);
            method(builder);
            builder.AppendLine("</table></td></tr>");
        }

        protected delegate void HtmlProcessMethod(StringBuilder builder);
        #endregion

        protected abstract void Build(StringBuilder builder);
    }
}
