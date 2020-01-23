using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.Views.MyPlace
{
    public class MyPlacesFilteredComboBox : CachedFilteredComboBox<MyPlaceDTO>
    {
        protected override Common.ObjectsCacheBase<MyPlaceDTO> GetCache()
        {
            return MyPlacesReposidory.GetCache(null);
        }


        protected override bool FilterImplementation(object value)
        {
            var item = (MyPlaceDTO)value;
            return item.Name.ToLower().Contains(this.Text.ToLower());
        }
    }

}
