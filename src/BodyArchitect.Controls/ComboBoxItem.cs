
namespace BodyArchitect.Controls
{
    public class ComboBoxItem
    {
        private object tag;
        private string text;

        public ComboBoxItem(object tag, string text)
        {
            this.tag = tag;
            this.text = text;
        }

        public ComboBoxItem()
        {
        }

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public override string ToString()
        {
            return text;
        }
    }
}
