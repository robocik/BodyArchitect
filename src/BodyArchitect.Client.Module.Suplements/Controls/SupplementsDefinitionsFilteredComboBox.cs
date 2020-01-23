using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    class SupplementsDefinitionsFilteredComboBox : CachedFilteredComboBox<SupplementCycleDefinitionDTO>
    {
        protected override Common.ObjectsCacheBase<SupplementCycleDefinitionDTO> GetCache()
        {
            return SupplementsCycleDefinitionsReposidory.Instance;
        }

        protected override bool FilterImplementation(object value)
        {
            var exercise = (SupplementCycleDefinitionDTO)value;
            string text = this.Text.ToLower();
            return exercise.Name.ToLower().Contains(text);
        }
    
    }
}
