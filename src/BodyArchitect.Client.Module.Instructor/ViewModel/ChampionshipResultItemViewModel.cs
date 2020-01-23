using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ChampionshipResultsViewModel:ViewModelBase
    {
        public List<ChampionshipResultItemViewModel> items=new List<ChampionshipResultItemViewModel>();
        private ChampionshipCategoryDTO category;

        public ChampionshipResultsViewModel(IGrouping<ChampionshipCategoryDTO, ChampionshipResultItemDTO> @group)
        {
            this.category = @group.Key;
            var list = category.Category == ChampionshipWinningCategories.Druzynowa ? @group.OrderByDescending(x => x.Value).ThenBy(x => x.Position) : @group.OrderBy(x => x.Value).ThenBy(x => x.Position); 
            foreach (var championshipResultItemDto in list)
            {
                items.Add(new ChampionshipResultItemViewModel(championshipResultItemDto, Item));
            }
        }

        public string Category
        {
            get
            {
                var caption= string.Format("{0} - {1}", EnumLocalizer.Default.Translate(Item.Category), EnumLocalizer.Default.Translate(Item.Gender));
                if (!category.IsOfficial)
                {
                    caption += " ("+InstructorStrings.ChampionshipCategoryViewModel_Category_NotOfficial+")";
                }
                return caption;
            }
        }

        public List<ChampionshipResultItemViewModel> Items
        {
            get { return items; }
        }

        public ChampionshipCategoryDTO Item
        {
            get { return category; }
        }
    }

    public class ChampionshipResultItemViewModel:ViewModelBase
    {
        private ChampionshipResultItemDTO resultItem;
        private ChampionshipCategoryDTO category;

        public ChampionshipResultItemViewModel(ChampionshipResultItemDTO resultItem,ChampionshipCategoryDTO category)
        {
            this.category = category;
            this.resultItem = resultItem;
        }

        public CustomerDTO Customer
        {
            get { return CustomersReposidory.Instance.GetItem(ResultItem.Customer.CustomerId); }
        }


        public string FullName
        {
            get
            {
                if(ResultItem.Customer==null)
                {
                    return ResultItem.Group.Name;
                }
                var customer=CustomersReposidory.Instance.GetItem(ResultItem.Customer.CustomerId);
                if(customer!=null)
                {
                    return customer.FullName;
                }
                return Strings.DeletedObject;
            }
        }

        public string WeightCategory
        {
            get
            {
                if(category.Type==ChampionshipCategoryType.Open)
                {
                    return EnumLocalizer.Default.Translate(ChampionshipCategoryType.Open);
                }

                return ChampionshipExerciseViewModel.GetDisplayWeight(ResultItem.Value, category.Gender);
            }
        }

        public int Position
        {
            get { return ResultItem.Position + 1; }
        }

        public string Year
        {
            get
            {
                var customer = Customer;
                if (customer != null)
                {
                    return customer.Birthday.Value.Year.ToString();
                }
                return "";
            }
        }



        public string Weight
        {
            get { return ChampionshipExerciseViewModel.GetDisplayWeight(ResultItem.Customer.Weight, Gender.NotSet); }
        }

        public string Total
        {
            get
            {
                if (ResultItem.Customer != null)
                {
                    return ChampionshipExerciseViewModel.GetDisplayWeight(ResultItem.Customer.Total, Gender.NotSet);
                }
                return ResultItem.Value.ToString();
            }
        }

        public string Wilks
        {
            get
            {
                if (ResultItem.Customer != null)
                {
                    return string.Format(InstructorStrings.ChampionshipExerciseViewModel_Wilks, ResultItem.Customer.TotalWilks);
                }
                return ResultItem.Value.ToString("#.##");
            }
        }

        public ChampionshipResultItemDTO ResultItem
        {
            get { return resultItem; }
        }
    }
}
