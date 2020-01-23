using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace BodyArchitect.DataAccess.NHibernate
{
    /*
     honestly it'd be easiest to just pass a hex string through WCF, and convert it to a color on the silverlight side.

color to hex string:
 string hexColor = yourColor.A.ToString("X2") + yourColor.R.ToString("X2") + yourColor.G.ToString("X2") + yourColor.B.ToString("X2");

hex string to color:

 Color clientSideColor = Color.FromArgb(Convert.ToByte(hex.Substring(0, 2), 16), Convert.ToByte(hex.Substring(2, 2), 16), Convert.ToByte(hex.Substring(4, 2), 16), Convert.ToByte(hex.Substring(6, 2), 16));*/
    //public class ColorUserType : IUserType
    //{
    //    new public bool Equals(object x, object y)
    //    {
    //        if (ReferenceEquals(x, y)) return true;
    //        if (x == null || y == null) return false;
    //        return x.Equals(y);
    //    }

    //    public int GetHashCode(object x)
    //    {
    //        return x == null ? typeof(Color).GetHashCode() + 473 : x.GetHashCode();
    //    }

    //    public object NullSafeGet(IDataReader rs, string[] names, object owner)
    //    {
    //        var obj = NHibernateUtil.String.NullSafeGet(rs, names[0]);
    //        if (obj == null) return null;
    //        var colorString = (string)obj;
    //        return ColorTranslator.FromHtml(colorString);
    //    }

    //    public void NullSafeSet(IDbCommand cmd, object value, int index)
    //    {
    //        if (value == null)
    //        {
    //            ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
    //        }
    //        else
    //        {
    //            ((IDataParameter)cmd.Parameters[index]).Value = ColorTranslator.ToHtml((Color)value);
    //        }

    //    }

    //    public object DeepCopy(object value)
    //    {
    //        return value;
    //    }

    //    public object Replace(object original, object target, object owner)
    //    {
    //        return original;
    //    }

    //    public object Assemble(object cached, object owner)
    //    {
    //        return cached;
    //    }

    //    public object Disassemble(object value)
    //    {
    //        return value;
    //    }

    //    public SqlType[] SqlTypes
    //    {
    //        get { return new[] { new SqlType(DbType.StringFixedLength) }; }
    //    }

    //    public Type ReturnedType
    //    {
    //        get { return typeof(Color); }
    //    }

    //    public bool IsMutable
    //    {
    //        get { return true; }
    //    }
    //}
}
