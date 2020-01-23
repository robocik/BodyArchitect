using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.ViewModel;

namespace BodyArchitect.WP7
{
    public class PagesState
    {
        public ObservableCollection<WorkoutPlanViewModel> SearchedPlans { get; set; }

        private static PagesState current;

        public static PagesState Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;
            }
        }

        public void SaveState()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Constants.PageStateFileName, FileMode.Create, store))
                    {
                        //DataContractSerializer serializer = new DataContractSerializer(typeof(ApplicationState));
                        //serializer.WriteObject(stream, this);
                        SilverlightSerializer.Serialize(this, stream);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static PagesState LoadState()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(Constants.StateFileName))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Constants.PageStateFileName, FileMode.Open, store))
                        {
                            //DataContractSerializer serializer = new DataContractSerializer(typeof(ApplicationState));
                            //return (ApplicationState)serializer.ReadObject(stream);
                            var state = (PagesState)SilverlightSerializer.Deserialize(stream);

                            return state;
                        }
                    }

                }

            }
            catch (Exception ex)
            {

            }

            return new PagesState();
        }
    }
}
