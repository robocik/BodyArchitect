using System;
using System.Collections.ObjectModel;
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
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class PeopleViewModel:ViewModelBase
    {
        private ObservableCollection<UserViewModel> friends;
        private ObservableCollection<UserViewModel> favorites;

        public PeopleViewModel()
        {
            friends = new ObservableCollection<UserViewModel>();
            foreach (var friend in ApplicationState.Current.ProfileInfo.Friends)
            {
                friends.Add(new UserViewModel(friend));
            }
            favorites = new ObservableCollection<UserViewModel>();
            foreach (var favoriteUser in ApplicationState.Current.ProfileInfo.FavoriteUsers)
            {
                favorites.Add(new UserViewModel(favoriteUser));
            }
        }

        public UserViewModel SelectedUser { get; set; }

        public bool HasFriends
        {
            get { return friends.Count > 0; }
        }

        public ObservableCollection<UserViewModel> Friends
        {
            get { return friends; }
        }

        public bool HasFavorites
        {
            get { return favorites.Count > 0; }
        }

        public ObservableCollection<UserViewModel> Favorites
        {
            get { return favorites; }
        }
    }
}
