using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BodyArchitect.Client.Common
{
    public class HideableColumnGridSplitter : GridSplitter
    {
        private GridLength? _width;

        public HideableColumnGridSplitter()
        {
            this.IsVisibleChanged += HideableGridSplitter_IsVisibleChanged;
            Loaded += new RoutedEventHandler(HideableColumnGridSplitter_Loaded);
        }

        void HideableColumnGridSplitter_Loaded(object sender, RoutedEventArgs e)
        {
            internalVisibbleChange();
        }

        void HideableGridSplitter_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            internalVisibbleChange();
        }

        public void UpdateVisbility()
        {
            internalVisibbleChange();
        }
        private void internalVisibbleChange()
        {
            var parent = base.Parent as Grid;


            if (parent == null)
            {
                return;
            }

            int columnIndex = Grid.GetColumn(this);

            if (columnIndex + 1 >= parent.ColumnDefinitions.Count)
            {
                return;
            }

            var lastColumn = parent.ColumnDefinitions[columnIndex + 1];

            if (!IsVisible && lastColumn.Width.Value == 0)
            {
                return;
            }
            if (this.Visibility == Visibility.Visible)
                lastColumn.Width = _width ?? lastColumn.Width;
            else
            {
                _width = lastColumn.Width;
                lastColumn.Width = new GridLength(0);
            }
        }
    }
}
