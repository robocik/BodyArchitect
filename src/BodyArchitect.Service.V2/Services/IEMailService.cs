using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;

namespace BodyArchitect.Service.V2.Services
{
    public interface IEMailService
    {
        void SendEMail(Profile profile, string subject, string message,params object[] args);
        void NewSendEMail(Profile profile, string subject, string message);
    }
}
