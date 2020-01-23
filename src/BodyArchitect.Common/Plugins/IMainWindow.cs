using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTab;

namespace BodyArchitect.Common.Plugins
{
    public interface IMainWindow
    {
        XtraTabPage AddTabPage(IMainTabControl userControl, string title, Image icon,bool show);
    }

    public interface IMainTabControl
    {
        void Fill();

        void RefreshView();
    }

}
