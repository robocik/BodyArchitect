using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace BodyArchitectClientsManager
{
    class DBConnection
    {
        private MySqlConnection sqlConn;

        public void sqlConnect()
        {
            this.sqlConn = null;
            this.sqlConn = new MySqlConnection();

            sqlConn.ConnectionString = "Server=MYSQL5016.Smarterasp.net;Database=db_9f6221_body;Uid=9f6221_body;Pwd=X!nq13P*_BA;";

            try
            {
                sqlConn.Open();
                Console.WriteLine("Connection Open");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public ClientsInfo get_data(DateTime? fromDate,bool withAllPaid=true)
        {
            ClientsInfo info = new ClientsInfo();

            try
            {
                MySqlDataReader sqlReader;
                string query ="SELECT  UserName, logindata.* FROM logindata LEFT JOIN profile ON (logindata.ProfileId = profile.GlobalId);";
                if(fromDate.HasValue)
                {
                    query = "SELECT  UserName, logindata.* FROM logindata LEFT JOIN profile ON (logindata.ProfileId = profile.GlobalId) WHERE LoginDateTime>?fromDate ";
                    if(withAllPaid)
                    {
                        query += " OR (SUBSTRING(ApplicationVersion,1,4)='Full' AND ApplicationVersion<>'Full 1.0.0')";
                    }
                }

                query += " ORDER BY LoginDateTime DESC";
                MySqlCommand sqlCommand = new MySqlCommand(query, sqlConn);
                sqlCommand.CommandTimeout = 500;
                if (fromDate.HasValue)
                {
                    sqlCommand.Parameters.Add("?fromDate", MySqlDbType.DateTime).Value = fromDate.Value;
                }
                sqlReader = sqlCommand.ExecuteReader();

                //sqlReader.Read();
                while (sqlReader.Read())
                {
                    ClientInstance instance = new ClientInstance(sqlReader);
                    info.Clients.Add(instance);
                }

                return info;
            }
            catch (Exception e)
{
                Console.WriteLine(e.ToString());
                return null;
            }
        }


        public void sqlDisconnect()
        {
            try
            {
                sqlConn.Close();
                Console.WriteLine("Connection Close");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }



    }
}
