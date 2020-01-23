using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.Suplements.Model
{

    public static class SuplementsReposidory
    {
        private static IDictionary<Guid, SuplementDTO> dictSuplements;

        private static ManualResetEvent plansEvent = new ManualResetEvent(false);

        static SuplementsReposidory()
        {
            UserContext.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            loadSuplements();
        }

        static void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (e.Status == LoginStatus.Logged)
            {
                ClearCache();
            }
        }

        public static void ClearCache()
        {
            if (dictSuplements != null)
            {
                loadSuplements();
            }
        }
        public static IList<SuplementDTO> Suplements
        {
            get
            {
                EnsureSuplementsLoaded();
                return new List<SuplementDTO>(dictSuplements.Values);
            }
        }

        public static void EnsureSuplementsLoaded()
        {
            if (dictSuplements == null)
            {
                dictSuplements = new Dictionary<Guid, SuplementDTO>();
                if (ServicesManager.IsDesignMode)
                {
                    return;
                }


            }
            plansEvent.WaitOne();
        }

        private static void loadSuplements()
        {
            plansEvent.Reset();
            dictSuplements = null;
            var task=Task.Factory.StartNew(delegate
            {
                ControlHelper.EnsureThreadLocalized();
                PagedResultRetriever retriever = new PagedResultRetriever();
                var res = retriever.GetAll(delegate(PartialRetrievingInfo pageInfo)
                {
                    try
                    {
                        return ServiceManager.GetSuplements(pageInfo);
                    }
                    catch (Exception)
                    {
                        plansEvent.Set();
                        throw;
                    }
                    
                });
                dictSuplements = res.ToDictionary(t => t.SuplementId);

                plansEvent.Set();
            }, null);
            LogUnhandledExceptions(task);
        }

        private static Task LogUnhandledExceptions(Task task)
        {
            task.ContinueWith(delegate(Task t)
            {
                ExceptionHandler.Default.Process(t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }
        

        public static SuplementDTO GetSuplement(Guid id)
        {
            EnsureSuplementsLoaded();
            if (dictSuplements.ContainsKey(id))
            {
                return dictSuplements[id];
            }
            return SuplementDTO.Removed;
        }

    }
}
