using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BodyArchitect.Controls.External
{
	public class FloatComboBox : NameObjectComboBox<float>
	{
	}
	public class NameObjectComboBox<T> : System.Windows.Forms.ComboBox
	{
		BodyArchitect.Controls.External.NameObjectCollection<T> m_items = new BodyArchitect.Controls.External.NameObjectCollection<T>();
		public new BodyArchitect.Controls.External.NameObjectCollection<T> Items
		{
			get { return m_items; }
			set
			{
				m_items = value;
				DataSource = m_items;
			}
		}
		public new BodyArchitect.Controls.External.NameObject<T> SelectedItem
		{
			get { return base.SelectedItem as BodyArchitect.Controls.External.NameObject<T>; }
			set { base.SelectedItem = value; } 
		}
		public NameObjectComboBox()
		{
			DisplayMember = "Name";
			ValueMember = "Object";
		}
		protected override void OnLeave(EventArgs e)
		{
			if (DataBindings.Count > 0)
				DataBindings[0].WriteValue();
			base.OnLeave(e);
		}
	}
}
