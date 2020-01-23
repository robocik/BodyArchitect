using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Diagnostics;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BodyArchitect.Controls.External
{

	internal class ColumnConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
		{
            if (destType == typeof(InstanceDescriptor) || destType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo info, object value, Type destType)
		{
            if (destType == typeof(string))
            {
				TreeListColumn col = (TreeListColumn)value;
                return String.Format("{0}, {1}", col.Caption, col.Fieldname);
            }
			if (destType == typeof(InstanceDescriptor) && value is TreeListColumn)
			{
				TreeListColumn col = (TreeListColumn)value;
				ConstructorInfo cinfo = typeof(TreeListColumn).GetConstructor(new Type[] { typeof(string), typeof(string)});
				return new InstanceDescriptor(cinfo, new object[] {col.Fieldname, col.Caption}, false);
			}
			return base.ConvertTo(context, info, value, destType);
		}
	}
	class ColumnsTypeConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(TreeListColumnCollection))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
				return "(Columns Collection)";
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

}
