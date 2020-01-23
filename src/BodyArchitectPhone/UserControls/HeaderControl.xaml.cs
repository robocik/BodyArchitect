using System.Windows;
using System.Windows.Markup;

namespace BodyArchitect.WP7.UserControls
{
    [ContentProperty("Text")]
    public partial class HeaderControl 
    {
        public HeaderControl()
        {
            InitializeComponent();
        }

        public object Text
        {
            get
            {
                return contentPresenter.Content;
            }
            set { contentPresenter.Content=value; }
        }

        public static readonly DependencyProperty TextProperty =
                DependencyProperty.Register("Text",
                typeof(object), typeof(HeaderControl),
                new PropertyMetadata(null, OnSourceChanged));

        static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            HeaderControl img = (HeaderControl)obj;
            img.Text =  args.NewValue;
            //img.lblText1.Text = img.lblText2.Text = img.lblText3.Text = img.Text;
        }
    }
}
