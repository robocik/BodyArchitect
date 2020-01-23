using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BodyArchitect.Client.Common
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public virtual void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }

        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TreeItemViewModel:ViewModelBase
    {
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }
    }
}
