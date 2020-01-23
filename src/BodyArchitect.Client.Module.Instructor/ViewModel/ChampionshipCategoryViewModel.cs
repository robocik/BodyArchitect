using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ChampionshipCategoryViewModel:ViewModelBase
    {
        private ChampionshipCategoryDTO category;

        public ChampionshipCategoryViewModel(ChampionshipCategoryDTO category)
        {
            this.category = category;
        }

        public string Gender
        {
            get { return EnumLocalizer.Default.Translate(Category.Gender); }
        }

        public string Type
        {
            get { return EnumLocalizer.Default.Translate(Category.Type); }
        }

        public string Age
        {
            get
            {
                return EnumLocalizer.Default.Translate(Category.Category);
            }
        }

        public string Official
        {
            get { return category.IsOfficial ? InstructorStrings.ChampionshipCategoryViewModel_Category_Official : InstructorStrings.ChampionshipCategoryViewModel_Category_NotOfficial; }
        }

        public string AgeStrict
        {
            get { return Category.IsAgeStrict ? InstructorStrings.ChampionshipCategoryViewModel_AgeStrict : InstructorStrings.ChampionshipCategoryViewModel_AgeLoose; }
        }

        public ChampionshipCategoryDTO Category
        {
            get { return category; }
        }
    }
}
