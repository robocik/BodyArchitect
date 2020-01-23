using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;

namespace BodyArchitectClientsManager
{
    public class ClientsFileParser
    {
        public ClientsInfo Parse(DateTime? fromDate,bool allPaid)
        {
            var dbConn = new DBConnection();
            dbConn.sqlConnect();
            var result = dbConn.get_data(fromDate,allPaid);
            dbConn.sqlDisconnect();
            return result;
        }

        private ClientsInfo processContent(string[] lines)
        {
            ClientsInfo info = new ClientsInfo();
            
            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    ClientInstance instance = new ClientInstance(line);
                    info.Clients.Add(instance);
                }

            }
            return info;
        }

        public ClientsInfo GetFromWWW()
        {
            string fileContent = getClientsFileFromWeb();
            StringReader re = new StringReader(fileContent);
            List<string> linesList = new List<string>();
            string line = null;
            while((line=re.ReadLine())!=null)
            {
                linesList.Add(line);
            }

            return processContent(linesList.ToArray());
        }

        string getClientsFileFromWeb()
        {
            string lcUrl = "http://update.bodyarchitectonline.com/clients.txt";
            WebClient wc = new WebClient();
            wc.Proxy = null;
            string html=wc.DownloadString(lcUrl);
            return html;
        }
    }
}
