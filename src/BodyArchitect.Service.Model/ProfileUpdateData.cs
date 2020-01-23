﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class ProfileUpdateData
    {
        [DataMember]
        [NotNullValidator]
        [ObjectValidator]
        public ProfileDTO Profile { get; set; }

        [DataMember]
        [ObjectValidator]
        public WymiaryDTO Wymiary { get; set; }
    }
}
