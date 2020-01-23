using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Configuration;
using BodyArchitect.Settings.Model;

namespace BodyArchitect.Settings
{
    public class GuiStateSettings
    {
        static GuiStateSettings()
        {
            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                GuiState.Default.Upgrade();
            }
        }

        public bool StartTimerAfterOpeningWindow
        {
            get
            {
                return GuiState.Default.StartTimerAfterOpeningWindow;
            }
            set
            {
                GuiState.Default.StartTimerAfterOpeningWindow = value;
            }
        }

        public bool ShowRestTimeColumns
        {
            get
            {
                return GuiState.Default.ShowRestTimeColumns;
            }
            set
            {
                GuiState.Default.ShowRestTimeColumns = value;
            }
        }

        public bool SaveGUI
        {
            get
            {
                return GuiState.Default.SaveGUI;
            }
            set
            {
                GuiState.Default.SaveGUI = value;
            }
        }

        public bool ShowExerciseTypeColumn
        {
            get
            {
                return GuiState.Default.ShowExerciseTypeColumn;
            }
            set
            {
                GuiState.Default.ShowExerciseTypeColumn = value;
            }
        }

        public bool ShowRelativeDates
        {
            get
            {
                return GuiState.Default.ShowRelativeDates;
            }
            set
            {
                GuiState.Default.ShowRelativeDates = value;
            }
        }

        public int CalendarViewMode
        {
            get
            {
                return GuiState.Default.CalendarViewMode;
            }
            set { GuiState.Default.CalendarViewMode = value; }
        }


        public string Language
        {
            get
            {
                return GuiState.Default.Language;
            }
            set
            {
                GuiState.Default.Language = value;
            }
        }


        public bool ShowToolTips
        {
            get
            {
                return GuiState.Default.ShowToolTips;
            }
            set
            {
                GuiState.Default.ShowToolTips = value;
            }
        }

        public CalendarOptions CalendarOptions
        {
            get
            {
                if (GuiState.Default.CalendarOptions == null)
                {
                    GuiState.Default.CalendarOptions=new CalendarOptions();
                }
                return GuiState.Default.CalendarOptions;
            }
            set { GuiState.Default.CalendarOptions = value; }
        }

        public void Save()
        {
            GuiState.Default.Save();
        }


        public void SetValue(Form form,string name,object value)
        {
            GuiState.Default.WindowsState[form.Name + name] = value;
        }

        public object GetValue(Form form, string name)
        {
            if(GuiState.Default.WindowsState.ContainsKey(form.Name + name))
            {
                return GuiState.Default.WindowsState[form.Name + name];
            }
            return null;
        }
        public void SaveProcess(Form form)
        {
            if (GuiState.Default.WindowsState == null)
            {
                Hashtable table = new Hashtable();
                GuiState.Default.WindowsState = table;
            }
            GuiState.Default.WindowsState[form.Name + "Size"] = form.Size;
            GuiState.Default.WindowsState[form.Name + "WindowState"] = form.WindowState;

            GuiState.Default.WindowsState[form.Name + "Location"] = form.Location;
        }

        public void LoadProcess(Form form)
        {
            if (GuiState.Default.WindowsState != null)
            {
                Hashtable dict = GuiState.Default.WindowsState;
                if (form.FormBorderStyle==FormBorderStyle.Sizable && GuiState.Default.WindowsState.ContainsKey(form.Name + "Size"))
                {
                    form.Size = (Size)dict[form.Name + "Size"];
                }
                if(GuiState.Default.WindowsState.ContainsKey(form.Name + "WindowState") )
                {
                    form.WindowState = (FormWindowState) dict[form.Name + "WindowState"];
                }
                if (GuiState.Default.WindowsState.ContainsKey(form.Name + "Location"))
                {
                    form.Location = (Point)dict[form.Name + "Location"];
                }
            }
        }


    }

    public sealed partial class GuiState
    {
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public global::System.Collections.Hashtable WindowsState
        {
            get
            {
                return ((global::System.Collections.Hashtable)(this["WindowsState"]));
            }
            set
            {
                this["WindowsState"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [SettingsSerializeAs(SettingsSerializeAs.Binary)]
        public global::BodyArchitect.Settings.Model.CalendarOptions CalendarOptions
        {
            get
            {
                var item= ((global::BodyArchitect.Settings.Model.CalendarOptions)(this["CalendarOptions"]));
                if(item==null)
                {
                    item=CalendarOptions=new CalendarOptions();
                }
                return item;
            }
            set
            {
                this["CalendarOptions"] = value;
            }
        }
    }
}
