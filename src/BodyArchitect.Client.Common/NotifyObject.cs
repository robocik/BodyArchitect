using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Common
{
    public class NotifyObjectBase : BAGlobalObject
    {
        public DateTime DateTime { get; private set; }

        public NotifyObjectBase(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }
    public class NotifyObject : NotifyObjectBase
    {
        private Action<Object> clickEvent;
        private Action<Object> deleteEvent;

        public NotifyObject(string message, string title, DateTime utcDateTime)
            : base(utcDateTime)
        {
            this.message = message;
            this.title = title;
        }

        public ImageSource Image { get; set; }

        

        private string title;
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        private string message;
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        public Action<Object> ClickEvent
        {
            get { return clickEvent; }
            set { clickEvent = value; }
        }

        public Action<Object> DeleteEvent
        {
            get { return deleteEvent; }
            set { deleteEvent = value; }
        }
    }
}
