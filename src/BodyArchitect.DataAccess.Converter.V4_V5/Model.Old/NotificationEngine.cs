using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;

namespace BodyArchitect.Model.Old
{
    public delegate void NotificationEngineDelegate(Type objectTYpe,object id);

    public class NotificationEngine
    {
        private static NotificationEngine instance;
        Dictionary<Type,List<NotificationEngineDelegate>> notificationList = new Dictionary<Type, List<NotificationEngineDelegate>>();

        private NotificationEngine()
        {

        }

        public void RegisterForNotification(Type objectType,NotificationEngineDelegate notificationDelegate)
        {
            if(!notificationList.ContainsKey(objectType))
            {
                notificationList[objectType]=new List<NotificationEngineDelegate>();
            }
            notificationList[objectType].Add(notificationDelegate);
        }

        public void UnregisterForNotification(Type objectType, NotificationEngineDelegate notificationDelegate)
        {
            if (notificationList.ContainsKey(objectType))
            {
                notificationList[objectType].Remove(notificationDelegate);
            }
        }

        public void Notify<T>() 
        {
            Notify(typeof(T), null);
        }

        public void Notify<T>(object id) 
        {
            Notify(typeof (T), id);
        }

        public void Notify(Type objectType,object id)
        {
            List<NotificationEngineDelegate> delegates;
            if (notificationList.TryGetValue(objectType, out delegates))
            {
                foreach (NotificationEngineDelegate notificationEngineDelegate in delegates)
                {
                    notificationEngineDelegate.Invoke(objectType,id);
                }
            }
        }

        public static NotificationEngine Instance
        {
         get
         {
             if(instance==null)
             {
                 instance=new NotificationEngine();

             }
             return instance;
         }
        }
    }
}
