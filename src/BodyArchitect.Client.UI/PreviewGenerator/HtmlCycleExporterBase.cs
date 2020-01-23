using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BodyArchitect.Service.V2.Model.TrainingPlans;

namespace BodyArchitect.Client.UI.PreviewGenerator
{
    [Serializable]
    public abstract class HtmlCycleExporterBase:HtmlExporterBase
    {
        private bool printInfo = true;
        private bool printDescription = true;
        private bool useAlternateRows = true;
        private bool printEntriesComment = true;

        protected HtmlCycleExporterBase(string title) : base(title)
        {
        }

        [SRDescription("TrainingPlanHtmlExporter.UseAlternateRows.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.UseAlternateRows.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool UseAlternateRows
        {
            get { return useAlternateRows; }
            set { useAlternateRows = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintEntriesComment.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintEntriesComment.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintEntriesComment
        {
            get { return printEntriesComment; }
            set { printEntriesComment = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintInfo.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintInfo.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintInfo
        {
            get { return printInfo; }
            set { printInfo = value; }
        }

        [SRDescription("TrainingPlanHtmlExporter.PrintDescription.Description")]
        [SRDisplayName("TrainingPlanHtmlExporter.PrintDescription.DisplayName")]
        [SRCategory("PrintOptions")]
        [DefaultValue(true)]
        public bool PrintDescription
        {
            get { return printDescription; }
            set { printDescription = value; }
        }

        protected override void Build(StringBuilder builder)
        {
            PrepareData();

            if (PrintInfo)
            {
                BuildInfo(builder);
            }
            if (PrintDescription)
            {
                BuildDescription(builder);
            }
            BuildDetails(builder);
        }

        protected virtual void PrepareData()
        {
            
        }

        protected abstract void BuildDetails(StringBuilder builder);

        protected abstract void BuildInfo(StringBuilder builder);

        protected abstract void BuildDescription(StringBuilder builder);
    }
}
