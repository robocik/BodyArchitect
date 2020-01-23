using System;
using System.IO;
using System.Reflection;
using System.Windows.Shell;

namespace BodyArchitect.Client.UI.WindowsSevenIntegration
{
    public static class JumplistHelper
    {
        private static JumpList _jumpList;
        private static string _systemPath;
        private static string _assemblyLocation;
        private static bool _initialized = false;

        public static void Initialize()
        {
            if (!_initialized)
            {
                _jumpList = new JumpList();
                _systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
                _assemblyLocation = Assembly.GetEntryAssembly().Location;

                //Here design your jumplist
                AddJumpItem(_systemPath, "calc.exe", "Kalkulator", "", null, "Kalkulator"); //Will add jumpitem with tooltip
                AddJumpItem(_systemPath, "notepad.exe", "Notatnik", "", null, "Notatnik");

                AddJumpItem("","","",""); //Separator
                
                AddJumpItem("http://www.google.pl", "", "Google", "", "Links"); //Will add jumpitem to category: Links

                AddJumpItem(_assemblyLocation, "", "TEST", "--test");
                AddJumpItem(_assemblyLocation, "", "Not Test", "--not-test");
                AddJumpItem(_assemblyLocation, "", "Something Else", "--different");

                //Apply jumplist
                RefreshJumplist();

                _initialized = true;
            }
        }

        //TODO: If necessary: implement CurrentWorkingDirectory
        /// <summary>
        /// Create JumpList entry. After adding all new item call <see cref="RefreshJumplist()"/> to apply new Jumplist.
        /// </summary>
        /// <param name="dirPath">Path to executable (use <see cref="_assemblyLocation"/> to call method in this application.
        /// Can also be used to open webpageif You put url here.</param>
        /// <param name="file">Executable name and extension or empty if using webpage or this assembly.</param>
        /// <param name="showingName">Name than will appear in jumplist.</param>
        /// <param name="arguments">Arguments that will be passed to application.</param>
        /// <param name="category">Custom category to put item into. It will be created if it doesn't exist.
        /// Use null to add item to default category.</param>
        /// <param name="tooltip">Tooltip that will appear if jumplist item is pointed by cursor.</param>
        private static void AddJumpItem(string dirPath, string file, string showingName, string arguments, string category = null, string tooltip = null)
        {
            var task = new JumpTask
                           {
                               ApplicationPath = Path.Combine(dirPath, file),
                               Title = showingName,
                               Arguments = arguments,
                               IconResourceIndex = 0,
                               IconResourcePath = Path.Combine(dirPath, file),
                               CustomCategory = category,
                               Description = tooltip
                           };
            _jumpList.JumpItems.Add(task);
        }

        private static void RefreshJumplist()
        {
            _jumpList.Apply();
        }
    }
}
