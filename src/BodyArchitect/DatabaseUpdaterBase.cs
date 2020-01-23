using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace BodyArchitect
{
    abstract class DatabaseUpdaterBase
    {
        #region Helper methods

        protected float? getFloat(SqlCeDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return (float)reader.GetFloat(index);
            }
            return null;
        }

        protected Guid? getGuid(SqlCeDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetGuid(index);
            }
            return null;
        }

        protected int? getInt32(SqlCeDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetInt32(index);
            }
            return null;
        }

        protected DateTime? getDateTime(SqlCeDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetDateTime(index);
            }
            return null;
        }

        protected bool? getBoolean(SqlCeDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetBoolean(index);
            }
            return null;
        }

        protected string getString(SqlCeDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetString(index);
            }
            return null;
        }

        #endregion
    }
}
