using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Module.A6W.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.A6W.Controls
{
    /// <summary>
    /// Interaction logic for usrA6WPartialCompleted.xaml
    /// </summary>
    public partial class usrA6WPartialCompleted
    {
        private A6WEntryDTO entry;

        public usrA6WPartialCompleted()
        {
            InitializeComponent();
        }

        void showSetControls(int dayNumber)
        {
            A6WDay day = A6WManager.Days[dayNumber - 1];
            if (day.SetNumber < 2)
            {
                lblSet2.Visibility = System.Windows.Visibility.Collapsed;
                txtSet2.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (day.SetNumber < 3)
            {
                lblSet3.Visibility = System.Windows.Visibility.Collapsed;
                txtSet3.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        public void Fill(A6WEntryDTO entry)
        {
            this.entry = entry;
            if (entry.Set1.HasValue)
            {
                txtSet1.Value = entry.Set1.Value;
            }
            if (entry.Set2.HasValue)
            {
                txtSet2.Value = entry.Set2.Value;
            }
            if (entry.Set3.HasValue)
            {
                txtSet3.Value = entry.Set3.Value;
            }
            showSetControls(entry.Day.DayNumber);
        }

        public void Save(A6WEntryDTO entry)
        {
            save1(entry);
            save2(entry);
            save3(entry);
        }

        private void save3(A6WEntryDTO entry)
        {
            if (txtSet3.Visibility == Visibility.Visible && txtSet3.Value > 0)
            {
                entry.Set3 = (int)txtSet3.Value;
            }
            else
            {
                entry.Set3 = null;
            }
        }

        private void save2(A6WEntryDTO entry)
        {
            if (txtSet2.Visibility == Visibility.Visible && txtSet2.Value > 0)
            {
                entry.Set2 = (int)txtSet2.Value;
            }
            else
            {
                entry.Set2 = null;
            }
        }

        private void save1(A6WEntryDTO entry)
        {
            if (txtSet1.Visibility==Visibility.Visible && txtSet1.Value > 0)
            {
                entry.Set1 = (int)txtSet1.Value;
            }
            else
            {
                entry.Set1 = null;
            }
        }

        public bool Validate(A6WEntryDTO entry)
        {
            //A6WDay day = A6WManager.Weeks[entry.DayNumber - 1];
            A6WDay day = entry.Day;
            if (day.RepetitionNumber < txtSet1.Value)
            {
                BAMessageBox.ShowError(A6WEntryStrings.ErrorSetTooManyRepetitions, 1, entry.Day.DayNumber, day.RepetitionNumber);
                return false;
            }
            if (day.RepetitionNumber < txtSet2.Value)
            {
                BAMessageBox.ShowError(A6WEntryStrings.ErrorSetTooManyRepetitions, 2, entry.Day.DayNumber, day.RepetitionNumber);
                return false;
            }
            if (day.RepetitionNumber < txtSet3.Value)
            {
                BAMessageBox.ShowError(A6WEntryStrings.ErrorSetTooManyRepetitions, 3, entry.Day.DayNumber, day.RepetitionNumber);
                return false;
            }
            return true;
        }

        private void txtSet1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (entry != null)
            {
                save1(entry);
                SetModifiedFlag();
            }
        }

        public void SetModifiedFlag()
        {
            if (Parent != null)
            {
                var mainWnd=UIHelper.FindVisualParent<TrainingDayWindow>(Parent);
                if (mainWnd != null)
                {
                    mainWnd.SetModifiedFlag();
                }
            }
        }

        private void txtSet2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (entry != null)
            {
                save2(entry);
                SetModifiedFlag();
            }
        }

        private void txtSet3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (entry != null)
            {
                save3(entry);
                SetModifiedFlag();
            }
        }
    }
}
