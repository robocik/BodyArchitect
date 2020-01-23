using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    public abstract class usrEntryObjectUserControl:usrBaseControl
    {
        public abstract void Fill(EntryObjectDTO entry);
    }

    public abstract class usrEntryObjectDetailsBase : usrEntryObjectUserControl
    {
        public event EventHandler ObjectChanged;

        protected void onObjectChanged()
        {
            if (ObjectChanged != null)
            {
                ObjectChanged(this, EventArgs.Empty);
            }
        }

        public virtual bool ValidateControl(EntryObjectDTO entryDto)
        {
            return true;
        }

        public abstract void UpdateEntryObject(EntryObjectDTO entry);

        public virtual void UpdateReadOnly(bool readOnly)
        {
            
        }


        public void SetModifiedFlag()
        {
            onObjectChanged();
        }
    }
}
