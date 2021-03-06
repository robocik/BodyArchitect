﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Model
{
    public partial class ProfileInformationDTO : IPerson
    {
        public ProfileInformationDTO()
        {
            this.InvitationsField=new List<FriendInvitationDTO>();
            this.FriendsField=new List<UserSearchDTO>();
            this.FavoriteUsersField=new List<UserSearchDTO>();
        }
        public UserSearchDTO GetUser(Guid globalId)
        {
            return (from user in this.Friends.Union(this.FavoriteUsers) where user.GlobalId == globalId select user).SingleOrDefault();
        }

        public Gender Gender
        {
            get { return User.Gender; }
        }
    }
}
