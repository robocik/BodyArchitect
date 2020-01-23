using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class SynchronizationPage : AnimatedBasePage
    {
        private SynchronizationViewModel viewModel;

        public SynchronizationPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new TurnstileFeatherBackwardOutAnimator() { ListBox = lstItems, RootElement = LayoutRoot };
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if(viewModel.IsInProgress && BAMessageBox.Ask(ApplicationStrings.SynchronizationPage_QOnBackWhenSyncing)==MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel=new SynchronizationViewModel(ApplicationState.Current);
            viewModel.SynchronizationCompleted += delegate
            {
#if RELEASE
                                                            //in normal working we should save the state after sync to be sure that even crash of the app will not destroy the sync data
                                                          ApplicationState.Current.SaveState(false);
#endif
                                                          Dispatcher.BeginInvoke(
                                                              delegate
                                                                  {
                                                                      btnSync.Content =ApplicationStrings.SynchronizationPage_SynchronizeButton;
                                                                      updateGui();
                                                                      lpMergeBehavior.IsEnabled = true;
                                                                  });

                                                      };
            DataContext = viewModel;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (viewModel != null)
            {//when we leave this page we cancel the sync operation
                viewModel.Cancel();
            }
            base.OnNavigatedFrom(e);
        }
        void updateGui()
        {
            lblNoEntries.Visibility = viewModel.Items.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            btnSync.IsEnabled = viewModel.Items.Count > 0;
            
        }

        private void btnSynchronize_Click(object sender, RoutedEventArgs e)
        {
            if(ApplicationState.Current.IsOffline)
            {
                BAMessageBox.ShowError(ApplicationStrings.SynchronizationPage_ErrSynchronizeInOffline);
                return;
            }
            if (!viewModel.IsInProgress)
            {
                btnSync.Content = ApplicationStrings.SynchronizationPage_StopButton;
                lpMergeBehavior.IsEnabled = false;
                viewModel.Synchronize();
            }
            else
            {
                viewModel.Cancel();
                
            }
        }
    }
}