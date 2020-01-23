using System;
using System.Windows.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.UI
{
    public interface IHasFloatingPane
    {
        Control GetContentForPane(string paneId);
    }

    public interface IControlView
    {
        void Fill();

        void RefreshView();

        UserDTO User { get; }

        CustomerDTO Customer { get; }

        string Header { get; }

        Uri HeaderIcon { get;  }

        string HeaderToolTip { get; }

        bool IsModified { get; set; }

        bool IsInProgress { get; }

        AccountType AccountType { get; }

        
    }
}
