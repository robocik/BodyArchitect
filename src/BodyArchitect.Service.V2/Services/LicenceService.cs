using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using AccountType = BodyArchitect.Model.AccountType;

namespace BodyArchitect.Service.V2.Services
{
    class LicenceService : ServiceBase
    {
        public LicenceService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration) : base(session, securityInfo, configuration)
        {
        }

        public void ImportLicence(string licenceKey)
        {
            Log.WriteWarning("ImportLicence:Username={0}, key={1}", SecurityInfo.SessionData.Profile.UserName, licenceKey);

            using (var tx = Session.BeginSaveTransaction())
            {
                var dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                LicenceGenerator generator = new LicenceGenerator();
                var licenceInfo=generator.GetLicence(licenceKey);
                if (licenceInfo != null)
                {
                    //now check if this licence key has been used in the past. If yes, then error. One licence key can be used only once!
                    var count=Session.QueryOver<BAPoints>().Where(x => x.Identifier == licenceInfo.Id.ToString()).RowCount();
                    if(count>0)
                    {
                        throw new AlreadyOccupiedException("This licence key has been already used.");
                    }
                    BAPoints dbLicence = new BAPoints();
                    dbLicence.Points = licenceInfo.BAPoints;
                    dbLicence.Identifier = licenceInfo.Id.ToString();
                    dbLicence.Type = BAPointsType.Serialkey;
                    dbLicence.ImportedDate = Configuration.TimerService.UtcNow;
                    dbLicence.Profile = dbProfile;
                    dbLicence.LoginData=SecurityInfo.LoginData;
                    Session.Save(dbLicence);

                    dbProfile.Licence.BAPoints += dbLicence.Points;
                    Session.Update(dbProfile);
                    tx.Commit();
                    //update currently logged user in SecurityManager
                    var currentAccountType = SecurityInfo.Licence.CurrentAccountType;
                    SecurityInfo.Licence = dbProfile.Licence.Map<LicenceInfoDTO>();
                    SecurityInfo.Licence.CurrentAccountType = currentAccountType;
                }

                

                
            }
        }
    }
}
