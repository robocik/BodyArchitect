using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Service.V2.Model.Localization;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    [Serializable]
    [DataContract(Namespace = Const.ServiceNamespace)]
    [DebuggerDisplay("Shortcut={Shortcut},Name = {Name}")]
    [KnownType(typeof(ExerciseDTO))]
    public class ExerciseLightDTO : BAGlobalObject, IHasName
    {
        private ExerciseDifficult difficult = ExerciseDifficult.One;

        public ExerciseLightDTO()
        {
            Met = 5;
        }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "ExerciseDTO_Name_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(1, Constants.NameColumnLength, MessageTemplateResourceName = "ExerciseDTO_Name_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Name
        {
            get;
            set;
        }
        [DataMember]
        public decimal Met { get; set; }

        //UTC
        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public virtual Guid? ProfileId { get; set; }

        [DataMember]
        public bool UseInRecords { get; set; }

        [DataMember]
        public MechanicsType MechanicsType
        {
            get;
            set;
        }

        [DataMember]
        public ExerciseForceType ExerciseForceType
        {
            get;
            set;
        }

        [DataMember]
        public ExerciseType ExerciseType
        {
            get;
            set;
        }

        [DataMember]
        [NotNullValidator(MessageTemplateResourceName = "ExerciseDTO_Shortcut_NotNull", MessageTemplateResourceType = typeof(ValidationStrings))]
        [StringLengthValidator(1, 20, MessageTemplateResourceName = "ExerciseDTO_Shortcut_Length", MessageTemplateResourceType = typeof(ValidationStrings))]
        public string Shortcut
        {
            get;
            set;
        }


        [DataMember]
        public ExerciseDifficult Difficult
        {
            get { return difficult; }
            set { difficult = value; }
        }

    }
}
