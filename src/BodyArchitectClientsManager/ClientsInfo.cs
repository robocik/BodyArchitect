using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace BodyArchitectClientsManager
{
    public class ClientsInfo
    {
        List<ClientInstance> clients = new List<ClientInstance>();

        public List<ClientInstance> Clients 
        { 
            get { return clients; }
        }

        public Dictionary<Guid,List<ClientInstance>> GetUniqueInstances()
        {
            var cos = clients.GroupBy(doc => doc.InstanceId);
            return cos.ToDictionary(c => c.Key, c => c.ToList());
        }

        public Dictionary<string, List<ClientInstance>> GetUniqueUsers()
        {
            var cos = clients.GroupBy(doc => doc.Name).OrderBy(x=>x.Key);
            return cos.ToDictionary(c => c.Key, c => c.ToList());
        }

        public Dictionary<string, List<Tuple<string,int>>> GetOS(bool detailed=false)
        {
            var cos = clients.GroupBy(doc => getOS(doc.PlatformVersion, detailed)).OrderBy(x => x.Key);

            return cos.ToDictionary(c => c.Key, c => c.GroupBy(x => x.Name).Select(x => new Tuple<string, int>(x.Key, x.Count())).Distinct().ToList());
        }

        string getOS(string platform,bool detailed)
        {
            if (platform.Contains("CE 7.0.7004") || platform.Contains("CE 7.0.7008 "))
            {
                platform = "1: Windows Phone 7";
            }
            else if (platform.Contains("CE 7.0"))
            {
                platform = "2: Windows Phone 7 NoDo";
            }
            else if (platform.Contains("CE 7.10"))
            {
                platform = "3: Windows Phone 7 Mango";
            }
            if (detailed)
            {
                platform = platform.Replace("NT 5.1.2600", "XP");
                platform = platform.Replace("NT 5.1", "XP");
                platform = platform.Replace("NT 5.2", "XP x64/Server 2003");
                platform = platform.Replace("NT 6.0", "Vista/Server 2008");
                platform = platform.Replace("NT 6.1", "Windows 7/Server 2008 R2");
                platform = platform.Replace("NT 6.2", "Windows 8");
            }
            else
            {
                platform = platform.Replace("NT 5.1.2600", "XP");
                platform = platform.Replace("NT 5.1", "XP");
                platform = platform.Replace("NT 5.2", "XP x64/Server 2003");
                platform = platform.Replace("NT 6.0.6001", "Vista/Server 2008");
                platform = platform.Replace("NT 6.0.6000.0", "Vista/Server 2008");
                platform = platform.Replace("NT 6.0.6002", "Vista/Server 2008");
                platform = platform.Replace("NT 6.1.7601.0", "Windows 7/Server 2008 R2");
                platform = platform.Replace("NT 6.1.7601", "Windows 7/Server 2008 R2");
                platform = platform.Replace("NT 6.1.7600.0", "Windows 7/Server 2008 R2");

                platform = platform.Replace("NT 6.2.8400.0", "Windows 8");
                platform = platform.Replace("NT 6.2.8102.0", "Windows 8");
                platform = platform.Replace("NT 6.2.8250.0", "Windows 8");
                platform = platform.Replace("Dodatek ","");
            }
            return platform;
        }


        public List<ClientInstance> GetAllInstancesForClient(ClientInstance instance)
        {
            var cos = clients.Where(t => t.InstanceId == instance.InstanceId);
            return cos.ToList();
        }

        public bool IsFirstRun(ClientInstance clientInstance)
        {
            var res = clients.Where(t => t.Name == clientInstance.Name).Select(t => t).Min(t => t.Date);
            return clientInstance.Date == res;
        }

        public List<ClientInstance> GetAllInstancesForUser(string profile)
        {
            var cos = clients.Where(t => t.Name == profile).OrderBy(x=>x.Date);
            return cos.ToList();
        }

        public List<ClientInstance> GetEntriesForDay(DateTime date)
        {
            var cos = clients.Where(t => t.Date.Date == date.Date);
            return cos.ToList();
        }

        public List<ClientInstance> GetEntriesForPlatform(PlatformType platform)
        {
            var cos = clients.Where(t => t.Platform == platform);
            return cos.ToList();
        }

        public Dictionary<string, List<ClientInstance>> GetUsersForDay(DateTime date)
        {
            var cos = clients.Where(x => x.Date.Date == date.Date).GroupBy(doc => doc.Name);
            return cos.ToDictionary(c => c.Key, c => c.ToList());
        }

        public int GetFullVersionCount()
        {
            var cos = clients.Where(x => x.Version.StartsWith("Full") && x.Version != "Full 1.0.0").GroupBy(x=>x.Name);
            return cos.Count();
        }

        public List<string> GetFullVersionsProfiles()
        {
            var cos = clients.Where(x => x.Version.StartsWith("Full") && x.Version != "Full 1.0.0").GroupBy(x => x.Name).Select(x=>x.Key);
            var dict = cos.ToList();
            return dict;
        }
    }

    public enum PlatformType
    {
        Windows = 0,
        MacOS = 1,
        Android = 2,
        iPhone = 3,
        WindowsMobile = 4,
        WindowsPhone = 5,
        Linux = 6,
        Web = 7,
        Other = 8
    }

    public enum AccountType
    {
        User,
        PremiumUser,
        Instructor,
        Administrator
    }

    public class ClientInstance
    {
        public ClientInstance(MySqlDataReader item)
        {
            Name = item[0].ToString();
            Id = item[1].ToString();
            InstanceId = Guid.Parse(item[2].ToString());
            ProfileId = item[3].ToString();
            Platform = ((PlatformType)item[4]);
            PlatformVersion = item[5].ToString();      

            Language = item[6].ToString();
            Version = item[7].ToString();
            
            Date = (DateTime) item[9];
            AccountType = (AccountType)item[10];
                 
        }

        public ClientInstance(string line)
        {
            string[] tmp = line.Split(';');
            string dateFormat = "yyyy-MM-d H:m:s";
            if (tmp.Length == 2)
            {
                dateFormat = "yyyy-MM-d";
            }
            Date = DateTime.ParseExact(tmp[0], dateFormat, null);
            InstanceId = new Guid(tmp[1]);
            Version = "1.1.0.1";
            if (tmp.Length > 2)
            {
                Language = tmp[2];
                if (!string.IsNullOrEmpty(tmp[3]))
                {
                    Version = tmp[3];
                }
            }
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public Guid InstanceId { get; set; }
        public string ProfileId { get; set; }
        public DateTime Date { get; set; }
        public PlatformType Platform { get; set; }
        public string PlatformVersion { get; set; }
        public AccountType AccountType { get; set; }

        public override string ToString()
        {
            return InstanceId.ToString();
        }
    }
}
