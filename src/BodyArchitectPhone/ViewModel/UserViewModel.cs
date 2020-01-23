using System;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BugSense;


namespace BodyArchitect.WP7.ViewModel
{
    public class UserViewModel:ViewModelBase
    {
        private UserSearchDTO user;
        public event EventHandler OperationCompleted;

        public UserViewModel()
        {
            
        }
        public UserViewModel(UserSearchDTO user)
        {
            this.user = user;
        }

        public UserSearchDTO User
        {
            get { return user; }
            set { user = value; }
        }

        public string UserName
        {
            get { return user.UserName; }
            set{}
        }

        public bool HasStatus
        {
            get
            {
                return !string.IsNullOrEmpty(user.Statistics.Status.Status);
            }
            set{}
        }

        public string Status
        {
            get { return string.Format("„{0}”", user.Statistics.Status.Status); }
            set { }
        }

        public bool HasAwards
        {
            get
            {
                return Achievements.GetGreenStar(user) != AchievementStar.None || Achievements.GetRedStar(user) != AchievementStar.None ||
                       Achievements.GetBlueStar(user) != AchievementStar.None;
            }
        }

        public Visibility IsFavorite
        {
            get { return user.IsFavorite ? Visibility.Visible : Visibility.Collapsed; }
            set { }
        }

        public Visibility IsInvited
        {
            get { return user.IsInvited ? Visibility.Visible : Visibility.Collapsed; }
            set { }
        }

        public Visibility IsFriend
        {
            get { return user.IsFriend?Visibility.Visible:Visibility.Collapsed; }
            set { }
        }

        public Visibility CanBeFriend
        {
            get { return !user.IsFriend && !user.IsInvited ? Visibility.Visible : Visibility.Collapsed; }
            set { }
        }

        public Visibility CanSendMessage
        {
            get { return !user.IsDeleted ? Visibility.Visible : Visibility.Collapsed; }
            set { }
        }

        public Visibility CanBeFavorite
        {
            get { return !user.IsFriend && !user.IsFavorite ? Visibility.Visible : Visibility.Collapsed; }
            set { }
        }

        public string USERNAME
        {
            get { return user.UserName.ToUpper(); }
            set { }
        }

        public string CountryName
        {
            get { return Country.GetCountry(user.CountryId).EnglishName; }
            set { }
        }

        public string CreatedDate
        {
            get { return user.CreationDate.ToRelativeDate(); }
            set { }
        }

        public string Gender
        {
            get { return EnumLocalizer.Default.Translate(user.Gender); }
            set { }
        }

        public PictureInfoDTO Picture
        {
            get
            {
                if (user.Picture != null)
                {
                    return user.Picture;
                }
                return PictureInfoDTO.Empty;
            }
            set { }
        }

        public WymiaryDTO Wymiary
        {
            get
            {
                if (user.ProfileInfo != null)
                {
                    return user.ProfileInfo.Wymiary;
                }
                return null;
            }
            set { }
        }

        public string LastEntry
        {
            get
            {
                if(user.Statistics.LastEntryDate.HasValue)
                {
                    return user.Statistics.LastEntryDate.Value.ToCalendarDate();
                }
                return string.Empty;
            }
            set { }
        }

        public string LastEntryLink
        {
            get
            {
                if (user.Statistics.LastEntryDate.HasValue)
                {
                    return string.Format(ApplicationStrings.UserViewModel_LastEntryLink, LastEntry);
                }
                return ApplicationStrings.UserViewModel_LastEntryLink_Empty;
            }
            set { }
        }

        public bool IsCalendarAccessible
        {
            get
            {
                 return user.HaveAccess(user.Privacy.CalendarView);
            }
            set { }
        }

        void onOperationCompleted()
        {
            if(OperationCompleted!=null)
            {
                OperationCompleted(this, EventArgs.Empty);
            }
        }

        public bool HasAbout
        {
            get { return user.ProfileInfo != null && !string.IsNullOrEmpty(user.ProfileInfo.AboutInformation); }
            set { }
        }

        public string About
        {
            get
            {
                if (user.ProfileInfo != null && !string.IsNullOrEmpty(user.ProfileInfo.AboutInformation))
                {
                    return user.ProfileInfo.AboutInformation;
                }
                return ApplicationStrings.UserViewModel_About_Empty;
            }
            set { }
        }

        public void LoadDetails()
        {
            var m = new ServiceManager<GetProfileInformationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetProfileInformationCompletedEventArgs> operationCompleted)
            {
                GetProfileInformationCriteria data = new GetProfileInformationCriteria();
                data.UserId = user.GlobalId;
                client1.GetProfileInformationCompleted -= operationCompleted;
                client1.GetProfileInformationCompleted += operationCompleted;
                client1.GetProfileInformationAsync(ApplicationState.Current.SessionData.Token, data);


            });

            m.OperationCompleted += (s, a) =>
            {
                if (a.Error != null)
                {
                    onOperationCompleted();
                    BAMessageBox.ShowError(ApplicationStrings.UserViewModel_ErrRetrieveProfileDetails);
                }
                else
                {
                    user.ProfileInfo = a.Result.Result;
                    NotifyPropertyChanged("About");
                    NotifyPropertyChanged("HasMeasurements");
                    NotifyPropertyChanged("Wymiary");
                    NotifyPropertyChanged("HasAbout");
                    onOperationCompleted();
                }
            };

            if (!m.Run())
            {
                onOperationCompleted();
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

        public bool HasMeasurements
        {
            get { return user.ProfileInfo != null && user.ProfileInfo.Wymiary != null && !user.ProfileInfo.Wymiary.IsEmpty; }
            set { }
        }

        public bool HasAccessToMeasurements
        {
            get { return user.HaveAccess(user.Privacy.Sizes); }
            set { }
        }

        public void RejectFriendship()
        {
            //var m = new ServiceManager<InviteFriendOperationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<InviteFriendOperationCompletedEventArgs> operationCompleted)
            //{
            //    InviteFriendOperationData data = new InviteFriendOperationData();
            //    data.Operation = InviteFriendOperation.Reject;
            //    data.User = user;
            //    client1.InviteFriendOperationCompleted -= operationCompleted;
            //    client1.InviteFriendOperationCompleted += operationCompleted;
            //    client1.InviteFriendOperationAsync(ApplicationState.Current.SessionData.Token, data);


            //});

            //m.OperationCompleted += (s, a) =>
            //{
            //    FaultException<BAServiceException> faultEx = a.Error as FaultException<BAServiceException>;
            //    if (a.Error != null && faultEx.Detail.ErrorCode != ErrorCode.ObjectNotFound)
            //    {
            //        onOperationCompleted();
            //        MessageBox.Show(ApplicationStrings.UserViewModel_ErrOperation);
            //    }
            //    else
            //    {
            //        ApplicationState.Current.ProfileInfo.Friends.Remove(user);
            //        NotifyPropertyChanged("IsFriend");
            //        NotifyPropertyChanged("IsCalendarAccessible");
            //        NotifyPropertyChanged("HasAccessToMeasurements");
            //        onOperationCompleted();
            //    }
            //};

            //m.Run();
            friendshipOperation(InviteFriendOperation.Reject);
        }

        public void InviteFriendship()
        {
            friendshipOperation(InviteFriendOperation.Invite);
        }

        public void friendshipOperation(InviteFriendOperation operation)
        {
            var m = new ServiceManager<InviteFriendOperationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<InviteFriendOperationCompletedEventArgs> operationCompleted)
            {
                InviteFriendOperationData data = new InviteFriendOperationData();
                data.Operation = operation;
                data.User = user;
                client1.InviteFriendOperationCompleted -= operationCompleted;
                client1.InviteFriendOperationCompleted += operationCompleted;
                client1.InviteFriendOperationAsync(ApplicationState.Current.SessionData.Token, data);


            });

            m.OperationCompleted += (s, a) =>
            {
                FaultException<BAServiceException> faultEx = a.Error as FaultException<BAServiceException>;
                if (a.Error != null && (faultEx ==null || faultEx.Detail.ErrorCode != ErrorCode.ObjectNotFound))
                {
                    BugSenseHandler.Instance.SendExceptionAsync(a.Error);
                    onOperationCompleted();
                    BAMessageBox.ShowError(ApplicationStrings.UserViewModel_ErrOperation);
                }
                else
                {
                    if (operation == InviteFriendOperation.Reject)
                    {
                        ApplicationState.Current.ProfileInfo.Friends.Remove(user);
                        NotifyPropertyChanged("IsFriend");
                        NotifyPropertyChanged("IsCalendarAccessible");
                        NotifyPropertyChanged("HasAccessToMeasurements");
                    }
                    else
                    {
                        ApplicationState.Current.ProfileInfo.Invitations.Add(new FriendInvitationDTO(){CreatedDateTime = DateTime.UtcNow,InvitationType = InvitationType.Invite,Invited=user});
                    }
                    NotifyPropertyChanged("CanBeFriend");
                    onOperationCompleted();
                }
            };

            if (!m.Run())
            {
                onOperationCompleted();
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

        public void RemoveFromFavorites()
        {
            favoriteOperation(FavoriteOperation.Remove);
        }

        public void AddToFavorites()
        {
            favoriteOperation(FavoriteOperation.Add);
        }

        private void favoriteOperation(FavoriteOperation operation)
        {
            var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
                                                                    {
                                                                        client1.UserFavoritesOperationAsync(ApplicationState.Current.SessionData.Token, user, operation);
                                                                        client1.UserFavoritesOperationCompleted -= operationCompleted;
                                                                        client1.UserFavoritesOperationCompleted += operationCompleted;

                                                                    });


            m.OperationCompleted += (s, a) =>
                                        {

                                            if (a.Error != null)
                                            {
                                                BugSenseHandler.Instance.SendExceptionAsync(a.Error);
                                                onOperationCompleted();
                                                BAMessageBox.ShowError(ApplicationStrings.UserViewModel_ErrRemoveUserFromFavorites);
                                                return;
                                            }

                                            if (operation==FavoriteOperation.Remove)
                                            {
                                                ApplicationState.Current.ProfileInfo.FavoriteUsers.Remove(user);    
                                            }
                                            else
                                            {
                                                ApplicationState.Current.ProfileInfo.FavoriteUsers.Add(user);    
                                            }
                                            

                                            NotifyPropertyChanged("IsFavorite");
                                            NotifyPropertyChanged("CanBeFavorite");
                                            
                                            onOperationCompleted();
                                        };
            if (!m.Run())
            {
                onOperationCompleted();
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }
    }
}
