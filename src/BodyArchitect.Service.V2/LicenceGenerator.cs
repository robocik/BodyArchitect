using System;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2
{
    

    public class LicenceGenerator
    {
        private const string BALicenceEncryptionKey = "KJSVRUJzj3Fm46aF/J0wcIjjhjd6ykX7";

        public string GenerateLicenceKey(int points)
        {
            string rawKey = string.Format("{0}|{1}", points, Guid.NewGuid());
            return CryptographyHelper.Encrypt(rawKey, Convert.FromBase64String(BALicenceEncryptionKey));
        }
        
        public LicenceInfo GetLicence(string key)
        {
            var decryptedData = CryptographyHelper.Decrypt(key, Convert.FromBase64String(BALicenceEncryptionKey));
            var splitedKey = decryptedData.Split('|');

            LicenceInfo info = new LicenceInfo();
            info.BAPoints = int.Parse(splitedKey[0]);
            info.Id=new Guid(splitedKey[1]);
            return info;
        }
    }
}
