using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using BodyArchitect.Client.Module.StrengthTraining;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    public enum ChampionshipGridGroupMode
    {
        None,
        ByWeightCategories,
        ByGroup,
        Gender
    }

    public class ChampionshipDataGrid:BADataGrid
    {
        public ChampionshipDataGrid()
        {
            CanUserSortColumns = true;
        }

        public void BuildColumns(ChampionshipType championshipType)
        {
            EnsureNotEditMode();
            Columns.Clear();

            DataGridTemplateColumn colOptions = new DataGridTemplateColumn();
            colOptions.Width = DataGridLength.Auto;
            colOptions.CellTemplate = (DataTemplate)this.FindResource("ButtonsColumn");
            colOptions.IsReadOnly = true;
            Columns.Add(colOptions);

            DataGridTemplateColumn colCustomer = new DataGridTemplateColumn();
            colCustomer.Width = DataGridLength.SizeToCells;
            colCustomer.MinWidth = 100;
            colCustomer.SortDirection = ListSortDirection.Ascending;
            colCustomer.SortMemberPath = "Customer.FullName";
            colCustomer.Header = InstructorStrings.ChampionshipDataGrid_Grid_Customer;
            colCustomer.CellTemplate = (DataTemplate)this.FindResource("customerColumn");
            colCustomer.IsReadOnly = true;
            Columns.Add(colCustomer);

            
            DataGridTemplateColumn colGroup = new DataGridTemplateColumn();
            colGroup.Header = InstructorStrings.ChampionshipDataGrid_Grid_Team;
            colGroup.SortMemberPath = "SelectedGroup.Text";
            colGroup.CellTemplate = (DataTemplate)this.FindResource("GroupViewColumn");
            colGroup.CellEditingTemplate = (DataTemplate)this.FindResource("GroupColumn");
            colGroup.Width = DataGridLength.SizeToCells;
            colGroup.MinWidth = 70;
            Columns.Add(colGroup);

            DataGridTemplateColumn colComment = new DataGridTemplateColumn();
            colComment.Header = InstructorStrings.ChampionshipDataGrid_Grid_Comment;
            colComment.SortMemberPath = "Comment";
            colComment.CellTemplate = (DataTemplate)this.FindResource("commentViewColumn");
            colComment.CellEditingTemplate = (DataTemplate)this.FindResource("commentEditColumn");
            colComment.Width = DataGridLength.SizeToHeader;
            colComment.MinWidth = 100;
            Columns.Add(colComment);

            DataGridTemplateColumn colWeight = new DataGridTemplateColumn();
            colWeight.Width = DataGridLength.SizeToCells;
            colWeight.SortMemberPath = "Weight";
            colWeight.MinWidth = 70;
            colWeight.Header = InstructorStrings.ChampionshipDataGrid_Grid_Weight;
            colWeight.CellTemplate = (DataTemplate)this.FindResource("wagaViewColumn");
            colWeight.CellEditingTemplate = (DataTemplate)this.FindResource("wagaEditColumn");
            Columns.Add(colWeight);

            DataGridTemplateColumn colExercise1 = new DataGridTemplateColumn();
            colExercise1.Width =new DataGridLength(1,DataGridLengthUnitType.Star);
            colExercise1.MinWidth = 100;
            colExercise1.HeaderTemplate = createExerciseHeader(ExercisesReposidory.Instance.BenchPress.Name);
            colExercise1.CellTemplate = createContentPresenterTemplate("exerciseCellView","Exercise1");
            colExercise1.CellEditingTemplate = createContentPresenterTemplate("exerciseCellEdit", "Exercise1");
            Columns.Add(colExercise1);

            if (championshipType == ChampionshipType.Trojboj)
            {
                DataGridTemplateColumn colExercise2 = new DataGridTemplateColumn();
                colExercise2.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                colExercise2.MinWidth = 60;
                colExercise2.HeaderTemplate = createExerciseHeader(ExercisesReposidory.Instance.Deadlift.Name);
                colExercise2.CellTemplate = createContentPresenterTemplate("exerciseCellView", "Exercise2");
                colExercise2.CellEditingTemplate = createContentPresenterTemplate("exerciseCellEdit", "Exercise2");
                Columns.Add(colExercise2);

                DataGridTemplateColumn colExercise3 = new DataGridTemplateColumn();
                colExercise3.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                colExercise3.MinWidth = 60;
                colExercise3.HeaderTemplate = createExerciseHeader(ExercisesReposidory.Instance.Sqad.Name);
                colExercise3.CellTemplate = createContentPresenterTemplate("exerciseCellView", "Exercise3");
                colExercise3.CellEditingTemplate = createContentPresenterTemplate("exerciseCellEdit", "Exercise3");
                Columns.Add(colExercise3);
            }
        }

        private DataTemplate createContentPresenterTemplate(string resource,string binding)
        {
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.ContentTemplateProperty, (DataTemplate)this.FindResource(resource));
            var b2 = new Binding(string.Format(binding));
            b2.Mode = BindingMode.OneWay;
            contentPresenter.SetValue(ContentPresenter.ContentProperty,b2);
            DataTemplate dataTemplate = new DataTemplate();
            dataTemplate.VisualTree = contentPresenter;
            return dataTemplate;
        }
        
        DataTemplate createExerciseHeader(string exerciseName)
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.SetValue(Grid.BackgroundProperty, Brushes.Transparent);
            var column1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            var column2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            var column3 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column3.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));

            var row1 = new FrameworkElementFactory(typeof(RowDefinition));
            row1.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Auto));
            var row2 = new FrameworkElementFactory(typeof(RowDefinition));
            row2.SetValue(RowDefinition.HeightProperty, new GridLength(1, GridUnitType.Auto));

            gridFactory.AppendChild(column1);
            gridFactory.AppendChild(column2);
            gridFactory.AppendChild(column3);
            gridFactory.AppendChild(row1);
            gridFactory.AppendChild(row2);

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.TextProperty, exerciseName);
            factory.SetValue(Grid.ColumnSpanProperty,3);
            factory.SetValue(TextBlock.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center);
            gridFactory.AppendChild(factory);

            FrameworkElementFactory factoryTry1 = new FrameworkElementFactory(typeof(TextBlock));
            factoryTry1.SetValue(TextBlock.TextProperty, InstructorStrings.ChampionshipView_Try1);
            factoryTry1.SetValue(Grid.ColumnProperty,0);
            factoryTry1.SetValue(Grid.RowProperty,1);
            factoryTry1.SetValue(TextBlock.MarginProperty, Application.Current.Resources["MarginMediumLeft"]);
            gridFactory.AppendChild(factoryTry1);

            FrameworkElementFactory factoryTry2 = new FrameworkElementFactory(typeof(TextBlock));
            factoryTry2.SetValue(TextBlock.TextProperty, InstructorStrings.ChampionshipView_Try2);
            factoryTry2.SetValue(Grid.ColumnProperty,1);
            factoryTry2.SetValue(Grid.RowProperty, 1);
            factoryTry1.SetValue(TextBlock.MarginProperty, Application.Current.Resources["MarginMediumLeft"]);
            gridFactory.AppendChild(factoryTry2);

            FrameworkElementFactory factoryTry3 = new FrameworkElementFactory(typeof(TextBlock));
            factoryTry3.SetValue(TextBlock.TextProperty, InstructorStrings.ChampionshipView_Try3);
            factoryTry3.SetValue(Grid.ColumnProperty, 2);
            factoryTry3.SetValue(Grid.RowProperty, 1);
            factoryTry1.SetValue(TextBlock.MarginProperty, Application.Current.Resources["MarginMediumLeft"]);
            gridFactory.AppendChild(factoryTry3);

            DataTemplate template = new DataTemplate();
            template.VisualTree = gridFactory;

            return template;
        }

        
        public void GroupItemsBy(ChampionshipGridGroupMode groupMode)
        {
            EnsureNotEditMode();
            try
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
                view.GroupDescriptions.Clear();
                view.SortDescriptions.Clear();
                if (groupMode == ChampionshipGridGroupMode.Gender)
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription("DisplayGender"));
                }
                if (groupMode == ChampionshipGridGroupMode.ByWeightCategories)
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription("DisplayWeightCategory"));
                    view.SortDescriptions.Add(new SortDescription("WeightCategory",ListSortDirection.Ascending));
                }
                else if (groupMode == ChampionshipGridGroupMode.ByGroup)
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription("DisplayGroup"));
                    view.SortDescriptions.Add(new SortDescription("SelectedGroup.Text", ListSortDirection.Ascending));
                }

                view.SortDescriptions.Add(new SortDescription("Customer.FullName", ListSortDirection.Descending));
            }
            catch
            {
            }

        }
    }
}
