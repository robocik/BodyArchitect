using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class LoginDataMapping : ClassMapping<LoginData>
    {
        public LoginDataMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            ManyToOne(x=>x.ApiKey,map=>
                                      {
                                          map.NotNullable(false);
                                      });
            Property(b => b.ApplicationLanguage, y =>
            {
                y.Length(3);
                y.NotNullable(true);
            });
            Property(b => b.ApplicationVersion, y =>
            {
                y.Length(10);//TODO:increase
                y.NotNullable(true);
            });
            Property(b => b.ClientInstanceId, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.ProfileId, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.LoginDateTime, y =>
            {
                y.NotNullable(true);
                
            });
            Property(b => b.Platform, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.PlatformVersion, y =>
            {
                y.Length(100);
            });
        }
    }
}
