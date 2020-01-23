using System;
using System.Collections.Generic;
using System.Text;

namespace BodyArchitect.Controls.External
{
	public interface IPropertyDialogPage
	{
		void BeforeDeactivated(object dataObject);
		void BeforeActivated(object dataObject);
	}
}
