using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class LoginDataMapping: ClassMap<LoginData>
    {
        public LoginDataMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.ApplicationLanguage).Length(3).Not.Nullable();
            Map(x => x.ApplicationVersion).Length(10).Not.Nullable();
            Map(x => x.ClientInstanceId).Not.Nullable();
            Map(x => x.ProfileId).Not.Nullable();
            Map(x => x.LoginDateTime).Not.Nullable();
            Map(x => x.Platform).CustomType<PlatformType>().Not.Nullable();
            Map(x => x.PlatformVersion).Length(100);
        }
    }
}
