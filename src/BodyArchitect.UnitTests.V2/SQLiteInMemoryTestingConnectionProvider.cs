using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.UnitTests.V2
{
    public class SQLiteInMemoryTestingConnectionProvider : NHibernate.Connection.DriverConnectionProvider
    {
        public static System.Data.IDbConnection Connection = null;

        public override System.Data.IDbConnection GetConnection()
        {
            if (Connection == null)
                Connection = base.GetConnection();

            return Connection;
        }

        public override void CloseConnection(System.Data.IDbConnection conn) { }
    }
}
