using System;
using System.Collections.Generic;
using System.Drawing;


namespace BodyArchitect.Client.WCF.Plugins
{
    public interface IMainWindow
    {
        //XtraTabPage AddTabPage(IMainTabControl userControl, string title, Image icon,bool show);
    }

    public interface IMainTabControl
    {
        void Fill();

        void RefreshView();
    }

}
