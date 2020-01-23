using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using ValidationResult = Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [HasSelfValidation]
    [Serializable]
    public class SupplementCycleDefinitionDTO : PlanBase
    {
        public SupplementCycleDefinitionDTO()
        {
            Weeks = new List<SupplementCycleWeekDTO>();
        }

        [DataMember]
        public WorkoutPlanPurpose Purpose { get; set; }

        //[DataMember]
        //[LanguageValidator(MessageTemplateResourceName = "SupplementCycleDefinitionDTO_Language", MessageTemplateResourceType = typeof(ValidationStrings))]
        //public string Language { get; set; }

        //[DataMember]
        //[Required(ErrorMessageResourceName = "SupplementCycleDefinitionDTO_Name_Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        //[StringLengthValidator(Constants.NameColumnLength, MessageTemplateResourceName = "SupplementCycleDefinitionDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        //public string Name { get; set; }


        [DataMember]
        [NotNullValidator]
        [ObjectCollectionValidator(typeof(SupplementCycleWeekDTO))]
        public List<SupplementCycleWeekDTO> Weeks { get; set; }

        [DataMember]
        public TrainingPlanDifficult Difficult { get; set; }



        //[DataMember]
        //[ValidatorComposition(CompositionType.Or)]
        //[StringLengthValidator(0, Constants.NameColumnLength, MessageTemplateResourceName = "SupplementCycleDefinitionDTO_Author_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        //[NotNullValidator(Negated = true)]
        //public string Author { get; set; }

        //[DataMember]
        //[ValidatorComposition(CompositionType.Or)]
        //[StringLengthValidator(0, Constants.UrlLength, MessageTemplateResourceName = "SupplementCycleDefinitionDTO_Url_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        //[NotNullValidator(Negated = true)]
        //public string Url { get; set; }

        [DataMember]
        public bool CanBeIllegal { get; set; }

        [DataMember]
        public string Comment
        {
            get;
            set;
        }

        public int GetTotalWeeks()
        {
            int maxWeek = 0;
            foreach (var day in Weeks)
            {
                maxWeek = Math.Max(maxWeek, day.CycleWeekEnd);
            }
            return maxWeek;
        }

        public int GetTotalDays()
        {
            return GetTotalWeeks()*7;
        }

        //public IList<SuplementDTO> GetSupplements()
        //{
        //    Dictionary<Guid,SuplementDTO> supplements = new Dictionary<Guid, SuplementDTO>();
        //    foreach (var week in Weeks)
        //    {
        //        foreach (var dosage in week.Dosages.OfType<SupplementCycleDosageDTO>())
        //        {
        //            if(!supplements.ContainsKey(dosage.Supplement.GlobalId))
        //            {
        //                supplements.Add(dosage.Supplement.GlobalId,dosage.Supplement);
        //            }
        //        }
        //    }
        //    return supplements.Values.ToList();
        //}

        [SelfValidation]
        public void Validate(ValidationResults results)
        {
            if (Weeks.Count == 0)
            {
                results.AddResult(new ValidationResult("Cycle must have at least one week defined", this, "Weeks", null, null));
            }
            foreach (var week in Weeks)
            {
                if(week.CycleWeekStart>week.CycleWeekEnd)
                {
                    results.AddResult(new ValidationResult("CycleWeekStart is greater than CycleWeekEnd", week, "CycleWeekStart", null, null));
                }
            }
        }

        public IList<SupplementCycleWeekDTO> GetWeek(int weekNumber)
        {
            return Weeks.Where(x => x.CycleWeekStart >= weekNumber && x.CycleWeekEnd <= weekNumber).ToList();
        }
    }
}
