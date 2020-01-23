using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public class SupplementsFilteredComboBox: CachedFilteredComboBox<SuplementDTO>
    {
        public SupplementsFilteredComboBox()
        {
            //DisplayMemberPath = "Name";

            //ItemTemplate = (DataTemplate)Application.Current.Resources["SupplementsComboBoxItem"];
        }

        protected override Common.ObjectsCacheBase<SuplementDTO> GetCache()
        {
            return SuplementsReposidory.Instance;
        }

        protected override bool FilterImplementation(object value)
        {
            SuplementDTO supplement = (SuplementDTO) value;
            return supplement.Name.ToLower().Contains(this.Text.ToLower());
        }
    }
}
