using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace BodyArchitect.Client.Common
{
    public interface ICheckable
    {
        event EventHandler IsCheckedChanged;

        string Text { get; set; }

        bool IsChecked
        {
            get; set;
        }
    }
    public class ListItem<T>:INotifyPropertyChanged
    {
        public ListItem(string text,  T value):this()
        {
            Text = text;
            Value = value;
        }

        public ListItem()
        {
            Brush = Brushes.Transparent;
        }

        private Brush _brush;
        public Brush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                onPropertyChanged("Brush");
            }
        }

        private string _group;
        public string Group
        {
            get { return _group; }
            set
            {
                _group = value;
                onPropertyChanged("Group");
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                onPropertyChanged("Text");
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                onPropertyChanged("Description");
            }
        }

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                onPropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void onPropertyChanged(string propertyName)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class IntListItem<T>: ListItem<T>
    {
        public IntListItem()
        {
        }

        public int IntValue { get; set; }

        public IntListItem(string text, T value) : base(text, value)
        {
        }
    }

    public class CheckListItem<T> : ListItem<T>, ICheckable
    {
        public CheckListItem(string text, T value)
            : base(text, value)
        {
        }

        public CheckListItem()
        {
        }

        public event EventHandler IsCheckedChanged;

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                if(IsCheckedChanged!=null)
                {
                    IsCheckedChanged(this, EventArgs.Empty);
                }
            }
        }
    }

    public class ImageSourceListItem<T> : ListItem<T>
    {
        public ImageSourceListItem(string text, ImageSource image, T value)
            : base(text, value)
        {
            Image = image;
        }

        public ImageSourceListItem()
        {
        }

        public ImageSource Image { get; set; }
    }

    public class ImageListItem<T> : ListItem<T>
    {
        public ImageListItem(string text, string image, T value):base(text,value)
        {
            Image = image;
        }

        public ImageListItem()
        {
        }

        public string Image { get; set; }
    }
}
