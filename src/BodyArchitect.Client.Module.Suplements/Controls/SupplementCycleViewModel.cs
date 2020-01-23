using BodyArchitect.Client.UI.Controls.PlansUI;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public class SupplementCycleViewModel : PlanViewModel
    {
 
        public SupplementCycleViewModel(SupplementCycleDefinitionDTO plan, FeaturedItem featured = FeaturedItem.None):base(plan,featured)
        {

        }

        public string IsLegalIcon
        {
            get
            {
                if (Cycle.CanBeIllegal)
                {
                    return @"pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/IllegalSupplement.gif";
                }
                return @"pack://application:,,,/BodyArchitect.Client.Module.Suplements;component/Resources/LegalSupplement.png";
            }
        }
        public string IsLegalToolTip
        {
            get
            {
                if (Cycle.CanBeIllegal)
                {
                    return "SupplementsCyclesList_ToolTip_CanBeIllegal".TranslateSupple();
                }
                return "SupplementsCyclesList_ToolTip_IsLegal".TranslateSupple();
            }
        }



        public SupplementCycleDefinitionDTO Cycle
        {
            get { return (SupplementCycleDefinitionDTO) Plan; }
        }

        protected override bool IsFavorite()
        {
            return Cycle.IsFavorite();
        }
    }
}
