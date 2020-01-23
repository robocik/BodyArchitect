using System;
using System.Windows;
using System.Windows.Media.Imaging;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class MoodControl
    {
        public MoodControl()
        {
            InitializeComponent();
            updateMood(Mood.Normal);
        }

        public static readonly DependencyProperty MoodProperty =
                DependencyProperty.Register("Mood",
                typeof(Mood), typeof(MoodControl),
                new PropertyMetadata(Mood.Normal, OnMoodPropertyChanged));

        public Mood Mood
        {
            get
            {
                return (Mood)GetValue(MoodProperty);
            }
            set { SetValue(MoodProperty, value); }
        }

        static void OnMoodPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var img = (MoodControl)obj;
            var mood = (Mood)args.NewValue;
            img.updateMood(mood);
        }

        private void updateMood( Mood mood)
        {
            Mood = mood;
            imgMood.Source = new BitmapImage(new Uri(getMoodImage(mood), UriKind.RelativeOrAbsolute));
            tbMood.Text = EnumLocalizer.Default.Translate(mood);
        }

        string getMoodImage(Mood mood)
        {
            if (mood == Mood.Bad)
            {
                return "/Images/Mood_Bad32.png";
            }
            if (mood == Mood.Good)
            {
                return "/Images/Mood_Good32.png";
            }
            return "/Images/Mood_Normal32.png";
        }

        private void btnMood_Click(object sender, RoutedEventArgs e)
        {
            if (Mood == Mood.Normal)
            {
                Mood = Mood.Good;
            }else if (Mood == Mood.Good)
            {
                Mood=Mood.Bad;
            }
            else
            {
                Mood = Mood.Normal;
            }
        }
    }
}
