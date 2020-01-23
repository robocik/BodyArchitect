using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.BodyArchitect.WCF;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Users.Models;

namespace Orchand.BodyArchitect
{
    public class BAUser : IUser {
        private UserPart userPart;

        public BAUser( UserPart userPart, Token token) 
        {
            this.userPart = userPart;
            Token = token;
        }

        public ContentItem ContentItem
        {
            get { return userPart.ContentItem; }
        }

        public int Id {
            get { return userPart.Id; }
        }
        public string UserName
        {
            get { return userPart.UserName; }
        }
        public string PasswordHash
        {
            get { return userPart.Record.Password; }
        }

        public string Email 
        {
            get { return userPart.Email; }
        }

        public Token Token { get; private set; }
    }
}
