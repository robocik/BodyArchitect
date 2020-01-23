using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BodyArchitect.Module.Blog.Options
{
    partial class BlogSettings
    {
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public global::System.Collections.Hashtable BlogEntriesVisits
        {
            get
            {
                return ((global::System.Collections.Hashtable)(this["BlogEntriesVisits"]));
            }
            set
            {
                this["BlogEntriesVisits"] = value;
            }
        }

        public void SetVisitDate(DateTime trainingDayDate,int loggedProfileId,int profileId,DateTime visitDate)
        {
            string key = loggedProfileId+profileId.ToString();
            if (Default.BlogEntriesVisits == null)
            {
                Hashtable table = new Hashtable();
                Default.BlogEntriesVisits = table;
            }
            if (!Default.BlogEntriesVisits.ContainsKey(key))
            {
                Default.BlogEntriesVisits[key] = new Hashtable();
            }
            ((Hashtable)Default.BlogEntriesVisits[key])[trainingDayDate] = visitDate;
        }

        public DateTime GetVisitDate(DateTime trainingDayDate, int loggedProfileId, int profileId)
        {
            string key = loggedProfileId + profileId.ToString();
            if (BlogSettings.Default.BlogEntriesVisits != null)
            {
                Hashtable dict = BlogSettings.Default.BlogEntriesVisits;
                if (dict.ContainsKey(key))
                {
                    Hashtable datesTable = (Hashtable)dict[key];
                    if(datesTable.ContainsKey(trainingDayDate))
                    {
                        return (DateTime) datesTable[trainingDayDate];
                    }
                }
            }
            return DateTime.MinValue;
        }
    }
}
