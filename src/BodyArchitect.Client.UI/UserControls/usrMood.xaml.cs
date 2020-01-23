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
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrMood.xaml
    /// </summary>
    public partial class usrMood
    {
        private List<ImageListItem<Mood>> _moods = new List<ImageListItem<Mood>>();
        private Mood selectedMood;
        private bool readOnly;
        public event EventHandler SelectedMoodChanged;

        public usrMood()
        {
            InitializeComponent();
            DataContext = this;
            Moods.Add(new ImageListItem<Mood>(EnumLocalizer.Default.Translate(Mood.Normal), "Mood_Normal32.png".ToResourceString(), Mood.Normal));
            Moods.Add(new ImageListItem<Mood>(EnumLocalizer.Default.Translate(Mood.Good), "Mood_Good32.png".ToResourceString(), Mood.Good));
            Moods.Add(new ImageListItem<Mood>(EnumLocalizer.Default.Translate(Mood.Bad), "Mood_Bad32.png".ToResourceString(), Mood.Bad));
        }

        public List<ImageListItem<Mood>> Moods
        {
            get { return _moods; }
        }

        public Mood SelectedMood
        {
            get { return selectedMood; }
            set
            {
                if (selectedMood != value)
                {
                    selectedMood = value;
                    NotifyOfPropertyChange(() => SelectedMood);
                    onSelectedMoodChanged();
                }
            }
        }

        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
                cmbStatus.IsEnabled = !readOnly;
            }
        }

        void onSelectedMoodChanged()
        {
            if (SelectedMoodChanged != null)
            {
                SelectedMoodChanged(this, EventArgs.Empty);
            }
        }
    }
}
