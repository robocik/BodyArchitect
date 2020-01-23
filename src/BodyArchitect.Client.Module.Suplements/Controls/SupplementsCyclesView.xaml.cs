using System;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public partial class SupplementsCyclesView
    {
        public SupplementsCyclesView()
        {
            InitializeComponent();
            Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:SupplementsCyclesView_Header_Cycles");
        }

        public override void Fill()
        {
            if(PageContext==null)
            {
                PageContext = new PageContext();
            }
            usrDefinitionsList.Fill(PageContext);
            if (PageContext.SelectedItem.HasValue && SupplementsCycleDefinitionsReposidory.Instance.GetItem(PageContext.SelectedItem.Value) == null)
            {
                usrDefinitionsList.Fill(PageContext.SelectedItem.Value);
            }
        }

        public override void RefreshView()
        {
            usrDefinitionsList.RefreshView();
        }

        public override Service.V2.Model.AccountType AccountType
        {
            get { return Service.V2.Model.AccountType.PremiumUser; }
        }

        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/SupplementsCycle.png", UriKind.Absolute); }
        }
    }
}
