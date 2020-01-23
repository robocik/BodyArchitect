using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BodyArchitect.Client.Common
{
    public class HideableRowGridSplitter : GridSplitter
    {
        private GridLength? _width;

        public HideableRowGridSplitter()
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

            int columnIndex = Grid.GetRow(this);

            if (columnIndex + 1 >= parent.RowDefinitions.Count)
            {
                return;
            }

            var lastColumn = parent.RowDefinitions[columnIndex + 1];

            if (!IsVisible && lastColumn.Height.Value == 0)
            {
                return;
            }
            if (this.Visibility == Visibility.Visible)
                lastColumn.Height = _width ?? lastColumn.Height;
            else
            {
                _width = lastColumn.Height;
                lastColumn.Height = new GridLength(0);
            }
        }
    }
}
