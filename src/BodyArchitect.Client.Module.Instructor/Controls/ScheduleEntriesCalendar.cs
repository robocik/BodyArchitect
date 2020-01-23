using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Module.Instructor.ViewModel;
using BodyArchitect.Client.UI.Controls.Calendar;
using BodyArchitect.Client.UI.Controls.Calendar.Common;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{

    public enum ScheduleColorMode
    {
        Activities,
        CustomerGroups,
        MyPlaces
    }

    public class ScheduleEntriesCalendar : Calendar
    {
        public ScheduleColorMode ColorMode
        {
            get { return (ScheduleColorMode)GetValue(ColorModeProperty); }
            set
            {
                SetValue(ColorModeProperty, value);
            }
        }


        public static readonly DependencyProperty ColorModeProperty =
            DependencyProperty.Register("ColorMode", typeof(ScheduleColorMode), typeof(ScheduleEntriesCalendar), new UIPropertyMetadata(ScheduleColorMode.Activities, OnColorModeChanged));

        private static void OnColorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var ctrl = (ScheduleEntriesCalendar)d;
            ctrl.RefreshAppointments();
        }


        protected override void OnDragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("ChampionshipItem") || e.Data.GetDataPresent("MyPlaceDTO") || e.Data.GetDataPresent("CustomerGroupDTO") || e.Data.GetDataPresent("ActivityDTO")) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey &&
        (e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effects = DragDropEffects.Copy;
            }
            else if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("ChampionshipItem") || e.Data.GetDataPresent("MyPlaceDTO") || e.Data.GetDataPresent("CustomerGroupDTO") || e.Data.GetDataPresent("ActivityDTO")) && (e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effects = DragDropEffects.Move;
            }
            
            e.Handled = true;
            base.OnDragOver(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {

            e.Effects = DragDropEffects.None;
            if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("ChampionshipItem") || e.Data.GetDataPresent("MyPlaceDTO") || e.Data.GetDataPresent("CustomerGroupDTO") || e.Data.GetDataPresent("ActivityDTO")) && (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey &&
        (e.AllowedEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
                if ((e.Data.GetDataPresent("myFormat") || e.Data.GetDataPresent("ChampionshipItem") || e.Data.GetDataPresent("MyPlaceDTO") || e.Data.GetDataPresent("CustomerGroupDTO") || e.Data.GetDataPresent("ActivityDTO")) && (e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    e.Effects = DragDropEffects.Move;

                }
            base.OnDragEnter(e);
        }
    }
}
