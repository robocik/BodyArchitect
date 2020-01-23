using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NHibernate.Dialect;

namespace BodyArchitect.DataAccess.NHibernate
{
    public class FixedMySql5Dialect : MySQL5Dialect
    {
        public FixedMySql5Dialect()
        {
            RegisterColumnType(DbType.String, 255, "VARCHAR($l)");
            RegisterColumnType(DbType.String, 65000, "TEXT");
            RegisterColumnType(DbType.String, "LONGTEXT"); 
            RegisterColumnType(DbType.Binary, "LONGBLOB"); 
            RegisterColumnType(DbType.Guid, "CHAR(36)"); 

         }

        public override string TableTypeString
        {
            get { return "ENGINE='InnoDb'"; }
        }

        

    }
}
