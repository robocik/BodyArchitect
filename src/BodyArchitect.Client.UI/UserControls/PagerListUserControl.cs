using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    public abstract class PagerListUserControl<T>:usrBaseControl
    {
        private PartialRetrievingInfo pagerInfo;
        private CancellationTokenSource currentTask;
        private PagedResult<T> result;

        private ObservableCollection<T> items = new ObservableCollection<T>();

        protected PagerListUserControl()
        {
            DataContext = this;
        }

        private bool searchEnabled;
        private bool moreResultsEnabled;
        private string searchStatus;

        public string SearchStatus
        {
            get { return searchStatus; }
            set
            {
                searchStatus = value;
                NotifyOfPropertyChange(() => SearchStatus);
            }
        }
        public bool SearchEnabled
        {
            get { return searchEnabled; }
            set
            {
                searchEnabled = value;
                NotifyOfPropertyChange(() => SearchEnabled);
            }
        }
        public bool MoreResultsEnabled
        {
            get { return moreResultsEnabled; }
            set
            {
                moreResultsEnabled = value;
                NotifyOfPropertyChange(() => MoreResultsEnabled);
            }
        }

        protected abstract PagedResult<T> RetrieveItems(PartialRetrievingInfo pagerInfo);
        
        public void DoSearch(object param=null)
        {
            BeforeSearch(param);
            Items.Clear();
            SearchStatus = string.Empty;
            pagerInfo = new PartialRetrievingInfo();
            fillPage(0);

        }

        protected abstract void BeforeSearch(object param = null);

        [DebuggerStepThrough]
        void throwCancel(OperationContext ctx)
        {
            ctx.CancellatioToken.ThrowIfCancellationRequested();
        }

        protected void fillPage(int pageIndex)
        {
            currentTask = ParentWindow.RunAsynchronousOperation(delegate(OperationContext ctx)
            {
                if (ctx.CancellatioToken == null || ctx.CancellatioToken.IsCancellationRequested || pagerInfo == null)
                {
                    return;
                }
                pagerInfo.PageIndex = pageIndex;
                pagerInfo.PageSize = 50;//todo: 10 wystarczy?
                result = RetrieveItems(pagerInfo);

                throwCancel(ctx);

                Dispatcher.BeginInvoke(new Action(delegate
                {
                    foreach (T searchDto in result.Items)
                    {
                        Items.Add(searchDto);
                    }
                    FillResults(this.Items);
                    SearchStatus = string.Format("PartialLoadedStatus".TranslateStrings(), Items.Count, result.AllItemsCount);
                }), null);


            }, asyncOperationStateChange, null);
        }

        public void MoreResults()
        {
            fillPage(pagerInfo.PageIndex + 1);
        }

        public virtual void ClearContent()
        {
            Items.Clear();
            MoreResultsEnabled = false;
            SearchStatus = string.Empty;
            if (currentTask != null && !currentTask.IsCancellationRequested)
            {
                currentTask.Cancel();
                currentTask = null;
            }
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            SearchEnabled = newStatus == LoginStatus.Logged;
            MoreResultsEnabled = false;
            ClearContent();
        }

        protected abstract void FillResults(ObservableCollection<T> result);

        protected virtual void ChangeOperationState(bool startLoginOperation)
        {
            
        }

        public ObservableCollection<T> Items
        {
            get { return items; }
        }

        void changeOperationState(bool startLoginOperation)
        {
            SearchEnabled = UserContext.Current.LoginStatus == LoginStatus.Logged && !startLoginOperation;
            MoreResultsEnabled = UserContext.Current.LoginStatus == LoginStatus.Logged && !startLoginOperation && (result != null && Items.Count < result.AllItemsCount);
            ChangeOperationState(startLoginOperation);
        }

        void asyncOperationStateChange(OperationContext context)
        {
            bool startLoginOperation = context.State == OperationState.Started;

            changeOperationState(startLoginOperation);
        }
    }
}
