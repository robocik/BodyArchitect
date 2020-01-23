using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Settings;
using Microsoft.Windows.Controls;
using BodyArchitect.Client.Module.StrengthTraining.Converters;
using Xceed.Wpf.Toolkit;
using TimeSpanConverter = BodyArchitect.Client.UI.Converters.TimeSpanConverter;
using TimeSpanUpDown = BodyArchitect.Client.UI.Controls.TimeSpanUpDown;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    public class StrengthTrainingDataGrid:BADataGrid
    {
        private StrengthTrainingEntryDTO entry;
        private StrengthTrainingViewModel viewModel;
        SuperSetViewManager superSetViewManager = new SuperSetViewManager();
        private GridGroupMode groupMode;
        private int currentSetsNumber;

        static StrengthTrainingDataGrid()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(StrengthTrainingDataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                    new ExecutedRoutedEventHandler(OnExecutedPaste),
                    new CanExecuteRoutedEventHandler(OnCanExecutePaste)));
        }

        public StrengthTrainingDataGrid()
        {
            LoadingRow += new EventHandler<DataGridRowEventArgs>(StrengthTrainingDataGrid_LoadingRow);
            CellEditEnding += grid_CellEditEnding;

        }


        public void Fill(StrengthTrainingEntryDTO entry,StrengthTrainingViewModel viewModel)
        {
            this.entry = entry;
            this.viewModel = viewModel;
            ItemsSource = viewModel.Items;
            
        }

        private void grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            viewModel.EnsureNewRowAdded();
        }

        void StrengthTrainingDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var item = e.Row.DataContext as StrengthTrainingItemViewModel;
            //if (!item.IsNew)
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }

            if(item!=null && !string.IsNullOrEmpty(item.Item.SuperSetGroup))
            {
                var color = superSetViewManager.GetSuperSetColor(item.Item.SuperSetGroup);
                Style style = new Style(typeof(DataGridRowHeader), (Style)FindResource(typeof(DataGridRowHeader)));
                style.Setters.Add(new Setter(DataGridRowHeader.BackgroundProperty, new SolidColorBrush(color)));
                style.Setters.Add(new Setter(DataGridRowHeader.ToolTipProperty, StrengthTrainingEntryStrings.StrengthTrainingDataGrid_ExerciseInSuperSetTip));
                e.Row.HeaderStyle = style;
            }
        }

        #region Clipboard Paste

        private static void OnCanExecutePaste(object target, CanExecuteRoutedEventArgs args)
        {
            ((StrengthTrainingDataGrid)target).OnCanExecutePaste(args);
        }

        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command query its state.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCanExecutePaste(CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = CurrentCell != null;
            args.Handled = true;
        }

        
        private static void OnExecutedPaste(object target, ExecutedRoutedEventArgs args)
        {
            ((StrengthTrainingDataGrid)target).OnExecutedPaste(args);
        }

        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command is executed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnExecutedPaste(ExecutedRoutedEventArgs args)
        {
            Debug.WriteLine("OnExecutedPaste begin");

            // parse the clipboard data            
            List<string[]> rowData = ClipboardHelper.ParseClipboardData();
            if(rowData==null)
            {
                return;
            }

            // call OnPastingCellClipboardContent for each cell
            int minRowIndex = Items.IndexOf(CurrentItem);
            int maxRowIndex = Items.Count - 1;
            int minColumnDisplayIndex = (SelectionUnit != DataGridSelectionUnit.FullRow) ? Columns.IndexOf(CurrentColumn) : 0;
            int maxColumnDisplayIndex = Columns.Count - 1;
            int rowDataIndex = 0;
            for (int i = minRowIndex; i <= maxRowIndex && rowDataIndex < rowData.Count; i++, rowDataIndex++)
            {
                int columnDataIndex = 0;
                var strengthTrainingItemViewModel =(StrengthTrainingItemViewModel) Items[i];
                for (int j = minColumnDisplayIndex; j < maxColumnDisplayIndex && columnDataIndex < rowData[rowDataIndex].Length; j++, columnDataIndex++)
                {
                    DataGridColumn column = ColumnFromDisplayIndex(j);
                    if(j==1/*ExerciseId column*/)
                    {
                        Guid exerciseId;
                        if(Guid.TryParse(rowData[rowDataIndex][columnDataIndex], out exerciseId))
                        {
                            strengthTrainingItemViewModel.Exercise = ExercisesReposidory.Instance.GetItem(exerciseId);    
                        }
                    }
                    else if (!column.IsReadOnly && (j < 2 || (!string.IsNullOrEmpty(rowData[rowDataIndex][columnDataIndex]) && !strengthTrainingItemViewModel.IsNew)))
                    {
                        column.OnPastingCellClipboardContent(strengthTrainingItemViewModel, rowData[rowDataIndex][columnDataIndex]);    
                    }
                    
                }
            }
            RefreshItems();
        }
        
        #endregion Clipboard Paste

        #region Move up/down

        public void MoveOneUp()
        {
            selectRowsFromCells();
            for (int i = 0; i < SelectedItems.Count; i++)
            {
                rowMoveUp((StrengthTrainingItemViewModel)SelectedItems[i]);
            }
            RefreshItems();
        }

        private void selectRowsFromCells()
        {
            if (SelectedItems.Count == 0)
            {
                foreach (var dataGridCellInfo in SelectedCells)
                {
                    var item = (StrengthTrainingItemViewModel) dataGridCellInfo.Item;
                    if (!item.IsNew)
                    {
                        SelectedItems.Add(item);
                    }
                }
            }
        }

        private bool rowMoveUp(StrengthTrainingItemViewModel dayRow)
        {
            if (dayRow == null)
            {
                return false;
            }

            int index = viewModel.Items.IndexOf(dayRow);
            int limit = 0;

            if (index > limit)
            {
                viewModel.Items.Move(index, index - 1);
                entry.RepositionEntry(index, index - 1);
                return true;
            }
            return false;
        }

        public void MoveOneDown()
        {
            selectRowsFromCells();
            for (int i = SelectedItems.Count - 1; i >= 0; i--)
            {
                rowMoveDown((StrengthTrainingItemViewModel)SelectedItems[i]);
            }
            RefreshItems();
        }

        private bool rowMoveDown(StrengthTrainingItemViewModel dayRow)
        {
            if (dayRow == null)
            {
                return false;
            }
            int index = viewModel.Items.IndexOf(dayRow);
            int limit = viewModel.Items.Count - 2;

            if (index < limit)
            {
                viewModel.Items.Move(index, index + 1);
                entry.RepositionEntry(index, index + 1);
                return true;
            }
            return false;
        }

        #endregion

        #region Build columns

//        DataTemplate createDynamicDataTemplate(string readOnlyTemplate, string editableTemplate, string readOnlyBinding)
//        {
//            string xaml = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
//<ContentPresenter Name='Presenter' Content='{Binding}' ContentTemplate='{DynamicResource " + readOnlyTemplate + @"}' />
//      <DataTemplate.Triggers>
//        <DataTrigger Binding='{Binding " + readOnlyBinding + @"}' Value='False'>
//          <Setter TargetName='Presenter' Property='ContentTemplate' Value='{DynamicResource " + editableTemplate + @"}' />
//        </DataTrigger>
//      </DataTemplate.Triggers>
//</DataTemplate>";
//            StringReader stringReader = new StringReader(xaml);
//            XmlReader xmlReader = XmlReader.Create(stringReader);
//            var dataTemplate = (DataTemplate)XamlReader.Load(xmlReader);
//            return dataTemplate;
//        }

//        DataTemplate createDynamicSetDataTemplate(string readOnlyTemplate, string editableTemplate, string readOnlyBinding,string cardioTemplate)
//        {
//            string xaml = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
//<ContentPresenter Name='Presenter' Content='{Binding}' ContentTemplate='{DynamicResource " + readOnlyTemplate + @"}' />
//      <DataTemplate.Triggers>
//        
//        
//        <DataTrigger Binding='{Binding " + readOnlyBinding + @"}' Value='False'>
//          <Setter TargetName='Presenter' Property='ContentTemplate' Value='{DynamicResource " + editableTemplate + @"}' />
//        </DataTrigger>
//<DataTrigger Binding='{Binding IsCardio}' Value='true'>
//          <Setter TargetName='Presenter' Property='ContentTemplate' Value='{DynamicResource " + cardioTemplate + @"}' />
//        </DataTrigger>
//      </DataTemplate.Triggers>
//</DataTemplate>";
//            StringReader stringReader = new StringReader(xaml);
//            XmlReader xmlReader = XmlReader.Create(stringReader);
//            var dataTemplate = (DataTemplate)XamlReader.Load(xmlReader);
//            return dataTemplate;
//        }

        DataTemplate createDynamicSetDataTemplate(string readOnlyTemplate, string editableTemplate, string cardioEditableTemplate,  string readOnlyBinding)
        {
            string xaml = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
<ContentPresenter Name='Presenter' Content='{Binding}' ContentTemplate='{DynamicResource " + readOnlyTemplate + @"}' />
      <DataTemplate.Triggers>
                
        <MultiDataTrigger>
            <MultiDataTrigger.Conditions>
                <Condition Binding='{Binding " + readOnlyBinding + @"}' Value='False' />
                <Condition Binding='{Binding IsCardio}' Value='True' />
            </MultiDataTrigger.Conditions>
            <Setter TargetName='Presenter' Property='ContentTemplate' Value='{DynamicResource " + cardioEditableTemplate + @"}' />
        </MultiDataTrigger>

        <MultiDataTrigger>
            <MultiDataTrigger.Conditions>
                <Condition Binding='{Binding " + readOnlyBinding + @"}' Value='False' />
                <Condition Binding='{Binding IsCardio}' Value='False' />
            </MultiDataTrigger.Conditions>
            <Setter TargetName='Presenter' Property='ContentTemplate' Value='{DynamicResource " + editableTemplate + @"}' />
        </MultiDataTrigger>
    </DataTemplate.Triggers>
</DataTemplate>";
            StringReader stringReader = new StringReader(xaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            var dataTemplate = (DataTemplate)XamlReader.Load(xmlReader);
            return dataTemplate;
        }

        private DataTemplate createEditable(int setNumber)
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.SetValue(Grid.BackgroundProperty, Brushes.Transparent);
            gridFactory.Name = "mainGrid";
            var column1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            var column2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto));

            gridFactory.AppendChild(column1);
            gridFactory.AppendChild(column2);


            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(WatermarkTextBox));
            Binding b = new Binding(setNumber + ".Value");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.Default;
            factory.SetValue(WatermarkTextBox.TextProperty, b);
            factory.SetValue(WatermarkTextBox.WatermarkProperty,StrengthTrainingEntryStrings.StrengthTrainingDataGrid_EmptySetCellWatermark);
            factory.SetValue(WatermarkTextBox.AutoSelectBehaviorProperty,AutoSelectBehavior.OnFocus);
            factory.SetValue(Masking.MaskProperty, string.Format(@"^[0-9]*x?[0-9]*[\{0}]?[0-9]*$", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));

            var buttonsPaneFactory = new FrameworkElementFactory(typeof(StackPanel));
            buttonsPaneFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            

            var btnDetails = new FrameworkElementFactory(typeof(Button));
            btnDetails.SetValue(Button.WidthProperty, 10.0);
            //btnDetails.SetValue(Button.HeightProperty, 10.0);
            btnDetails.SetValue(Button.IsTabStopProperty, false);
            
            btnDetails.SetValue(Button.ContentProperty, "..");
            btnDetails.SetValue(Button.PaddingProperty, new Thickness(0.0));
            btnDetails.SetValue(Button.StyleProperty, Application.Current.Resources["CloseableTabItemButtonStyle"]);


            btnDetails.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnAdditionalSetInfo_Click));
            btnDetails.SetValue(Button.ToolTipProperty, StrengthTrainingEntryStrings.usrStrengthTraining_Header_AdditionalInfo);
            buttonsPaneFactory.AppendChild(btnDetails);

           
            gridFactory.AppendChild(factory);
            gridFactory.AppendChild(buttonsPaneFactory);
            
            buttonsPaneFactory.SetValue(Grid.ColumnProperty, 1);
            // Create the template itself, and add the factory to it.
            DataTemplate cellEditingTemplate = new DataTemplate();
            cellEditingTemplate.VisualTree = gridFactory;
            
            return cellEditingTemplate;
        }

        void btnAdditionalSetInfo_Click(object sender, RoutedEventArgs e)
        {
            if(AdditionalSetInfoRequested!=null)
            {
                AdditionalSetInfoRequested(this, e);
            }
        }

        public event EventHandler<RoutedEventArgs> AdditionalSetInfoRequested;


        private DataTemplate createCardioEditable(int setNumber)
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.SetValue(Grid.BackgroundProperty, Brushes.Transparent);
            gridFactory.Name = "mainGrid";
            var column1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            var column2 = new FrameworkElementFactory(typeof(ColumnDefinition));
            column2.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto));

            gridFactory.AppendChild(column1);
            gridFactory.AppendChild(column2);

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TimeSpanUpDown));
            Binding b = new Binding(setNumber + ".CardioValue");
            b.Mode = BindingMode.TwoWay;
            b.Converter = new TimeSpanConverter();
            b.UpdateSourceTrigger = UpdateSourceTrigger.Default;
            factory.SetValue(DateTimeUpDown.ValueProperty, b);


            var buttonsPaneFactory = new FrameworkElementFactory(typeof(StackPanel));
            buttonsPaneFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);


            var btnDetails = new FrameworkElementFactory(typeof(Button));
            btnDetails.SetValue(Button.WidthProperty, 10.0);
            //btnDetails.SetValue(Button.HeightProperty, 10.0);
            btnDetails.SetValue(Button.IsTabStopProperty, false);

            btnDetails.SetValue(Button.ContentProperty, "..");
            btnDetails.SetValue(Button.PaddingProperty, new Thickness(0.0));
            btnDetails.SetValue(Button.StyleProperty, Application.Current.Resources["CloseableTabItemButtonStyle"]);


            btnDetails.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnAdditionalSetInfo_Click));
            btnDetails.SetValue(Button.ToolTipProperty, StrengthTrainingEntryStrings.usrStrengthTraining_Header_AdditionalInfo);
            buttonsPaneFactory.AppendChild(btnDetails);


            gridFactory.AppendChild(factory);
            gridFactory.AppendChild(buttonsPaneFactory);

            buttonsPaneFactory.SetValue(Grid.ColumnProperty, 1);
            // Create the template itself, and add the factory to it.
            DataTemplate cellEditingTemplate = new DataTemplate();
            cellEditingTemplate.VisualTree = gridFactory;

            return cellEditingTemplate;
        }

        private DataTemplate createReadOnly(int setNumber,bool forCardio=false)
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.SetValue(Grid.BackgroundProperty, Brushes.Transparent);
            gridFactory.Name = "mainGrid";
            

            var factory = new FrameworkElementFactory(typeof(StackPanel));
            factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            gridFactory.AppendChild(factory);

            var imgTimeData = new FrameworkElementFactory(typeof(Image));
            imgTimeData.SetValue(Image.SourceProperty, "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/StartTimer16.png".ToBitmap());
            imgTimeData.SetValue(Image.VisibilityProperty, Visibility.Collapsed);
            imgTimeData.SetValue(Image.WidthProperty, 12.0);
            imgTimeData.SetValue(Image.HeightProperty, 12.0);
            imgTimeData.SetValue(Image.MarginProperty, Application.Current.Resources["MarginSmallRight"]);
            var imgTooltipBinding = new Binding(setNumber + ".SetTime");
            imgTooltipBinding.Mode = BindingMode.OneWay;
            imgTimeData.SetValue(Image.ToolTipProperty,imgTooltipBinding);

            imgTimeData.Name = "imgTimeData";

            var imgRecord = new FrameworkElementFactory(typeof(Image));
            imgRecord.SetValue(Image.SourceProperty, "Records16.png".ToResourceString().ToBitmap());
            imgRecord.SetValue(Image.VisibilityProperty, Visibility.Collapsed);
            imgRecord.SetValue(Image.WidthProperty, 12.0);
            imgRecord.SetValue(Image.HeightProperty, 12.0);
            imgRecord.SetValue(Image.MarginProperty, Application.Current.Resources["MarginSmallRight"]);
            imgRecord.SetValue(Image.ToolTipProperty, "StrengthTrainingDataGrid_RecordSet_ToolTip".TranslateStrength());
            imgRecord.Name = "imgRecord";

            var tbSet = new FrameworkElementFactory(typeof(TextBlock));
            tbSet.SetValue(TextBlock.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center);
            factory.AppendChild(imgTimeData);
            factory.AppendChild(imgRecord);
            factory.AppendChild(tbSet);

            tbSet.Name = "txtBlock";
            var b = new Binding(setNumber + ".DisplayValue");
            b.Mode = BindingMode.OneWay;
            tbSet.SetValue(TextBlock.TextProperty, b);
            
            b = new Binding(setNumber + ".ToolTip");
            b.Mode = BindingMode.OneWay;
            tbSet.SetValue(TextBlock.ToolTipProperty, b);
            // Create the template itself, and add the factory to it.
            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = gridFactory;

            DataTrigger trigger = new DataTrigger();
            var binding = new Binding(setNumber + ".IsFromPlan");
            trigger.Binding = binding;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(Grid.BackgroundProperty, new SolidColorBrush(Colors.LightYellow), "mainGrid"));
            cellTemplate.Triggers.Add(trigger);

            trigger = new DataTrigger();
            binding = new Binding(setNumber + ".DisplayValue");
            trigger.Binding = binding;
            trigger.Value = "";
            if(forCardio)
            {
                trigger.Setters.Add(new Setter(TextBlock.TextProperty, "(00:00:00)", "txtBlock"));
            }
            else
            {
                trigger.Setters.Add(new Setter(TextBlock.TextProperty, "StrengthTrainingDataGrid_createReadOnly_RepsWeight".TranslateStrength(), "txtBlock"));    
            }
            
            trigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, Application.Current.Resources["DisabledForegroundBrush"], "txtBlock"));
            cellTemplate.Triggers.Add(trigger);

            trigger = new DataTrigger();
            binding = new Binding(setNumber + ".Set.IsRecord");
            trigger.Binding = binding;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(Image.VisibilityProperty, Visibility.Visible, "imgRecord"));
            cellTemplate.Triggers.Add(trigger);

            trigger = new DataTrigger();
            binding = new Binding(setNumber + ".ContainsTimeData");
            trigger.Binding = binding;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(Image.VisibilityProperty, Visibility.Visible, "imgTimeData"));
            cellTemplate.Triggers.Add(trigger);
            return cellTemplate;
        }


        DataTemplate createDisabled(int setNumber)
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            //factory.SetValue(TextBlock.BackgroundProperty, new SolidColorBrush(Colors.LightGray));
            //factory.SetValue(TextBlock.BackgroundProperty, new SolidColorBrush(Colors.Red));

            // Create the template itself, and add the factory to it.
            DataTemplate disabledTemplate = new DataTemplate();
            disabledTemplate.VisualTree = factory;
            return disabledTemplate;
        }

        DataGridTemplateColumn createSetColumnXaml(int setNumber)
        {
            string editableTemplateName = "EditableCellTemplate" + setNumber;
            string cardioEditableTemplateName = "CardioEditableCellTemplate" + setNumber;
            string readOnlyTemplateName = "ReadOnlyCellTemplate" + setNumber;
            string readOnlyCardioTemplateName = "ReadOnlyCardioCellTemplate" + setNumber;
            string disabledTemplateName = "DisabledCellTemplate" + setNumber;

            Resources.Add(editableTemplateName, createEditable(setNumber));
            Resources.Add(readOnlyTemplateName, createReadOnly(setNumber));
            Resources.Add(readOnlyCardioTemplateName, createReadOnly(setNumber,true));
            Resources.Add(disabledTemplateName, createDisabled(setNumber));
            Resources.Add(cardioEditableTemplateName, createCardioEditable(setNumber));

            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = string.Format("StrengthTrainingDataGrid_SetColumnHeader".TranslateStrength(), (setNumber + 1));
            col.Width = DataGridLength.Auto;
            col.ClipboardContentBinding = new Binding(setNumber + ".Value") { Mode = BindingMode.TwoWay };
            col.CellEditingTemplate = createDynamicSetDataTemplate(disabledTemplateName, editableTemplateName, cardioEditableTemplateName, setNumber + ".IsReadOnly");
            col.CellTemplate = createDynamicSetDataTemplate(disabledTemplateName, readOnlyTemplateName, readOnlyCardioTemplateName, setNumber + ".IsReadOnly");
            
            
            //col.PastingCellClipboardContent += new EventHandler<DataGridCellClipboardEventArgs>(col_PastingCellClipboardContent);
            return col;
        }

        private DataTemplate createRestColumn(int setNumber)
        {
            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            gridFactory.SetValue(Grid.BackgroundProperty, Brushes.Transparent);
            gridFactory.Name = "mainGrid";

            var factory = new FrameworkElementFactory(typeof(StackPanel));
            factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            
            gridFactory.AppendChild(factory);

            
            var tbSet = new FrameworkElementFactory(typeof(TextBlock));
            tbSet.SetValue(TextBlock.VerticalAlignmentProperty, System.Windows.VerticalAlignment.Center);
            factory.AppendChild(tbSet);

            tbSet.Name = "txtBlock";
            var b = new Binding(setNumber + ".RestTime");
            b.Mode = BindingMode.OneWay;
            tbSet.SetValue(TextBlock.TextProperty, b);

           
            // Create the template itself, and add the factory to it.
            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = gridFactory;


            var trigger = new DataTrigger();
            var binding = new Binding(setNumber + ".IsRestPause");
            trigger.Binding = binding;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(Grid.BackgroundProperty, Brushes.LightSalmon, "mainGrid"));
            cellTemplate.Triggers.Add(trigger);

            return cellTemplate;
        }

        public void BuildGridColumns(int setsNumber)
        {
            currentSetsNumber = setsNumber;
            EnsureNotEditMode();
            SelectedItems.Clear();
            Columns.Clear();
            DataGridTemplateColumn colDeleteButton = new DataGridTemplateColumn();
            colDeleteButton.Width = DataGridLength.SizeToCells;
            colDeleteButton.CellTemplate = (DataTemplate)this.FindResource("DeleteButtonColumn");
            colDeleteButton.IsReadOnly = true;
            colDeleteButton.Visibility = IsReadOnly
                                             ? System.Windows.Visibility.Collapsed
                                             : System.Windows.Visibility.Visible;
            Columns.Add(colDeleteButton);

            DataGridTemplateColumn colExercise = new DataGridTemplateColumn();
            colExercise.Header = "StrengthTrainingDataGrid_ExerciseColumnHeader".TranslateStrength();
            colExercise.CellTemplate = (DataTemplate)this.FindResource("ExerciseViewColumn");
            colExercise.CellEditingTemplate = (DataTemplate)this.FindResource("ExerciseColumn");
            colExercise.ClipboardContentBinding = new Binding("Exercise.GlobalId") { Mode = BindingMode.TwoWay, Converter = new GuidToStringConverter() };
            //colExercise.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            colExercise.Width = new DataGridLength(400, DataGridLengthUnitType.Pixel);
            Columns.Add(colExercise);

            if (ShowExerciseTypeColumn)
            {
                DataGridTextColumn colExerciseType = new DataGridTextColumn();
                colExerciseType.Header = "StrengthTrainingDataGrid_ExerciseTypeColumnHeader".TranslateStrength();
                Binding b = new Binding("ExerciseType");
                colExerciseType.Binding = b;
                colExerciseType.IsReadOnly = true;
                colExerciseType.Width = DataGridLength.Auto;
                Columns.Add(colExerciseType);
            }

            Resources.Clear();
            for (int i = 0; i < setsNumber; i++)
            {
                DataGridTemplateColumn colSet = createSetColumnXaml(i);
                Columns.Add(colSet);

                if (ShowRestColumns && i < setsNumber-1)
                {
                    var restTemplate = createRestColumn(i+1);//rest column shows RestTime value from previous set
                    DataGridTemplateColumn colRest = new DataGridTemplateColumn();
                    colRest.Header = "StrengthTrainingDataGrid_RestTimeColumnHeader".TranslateStrength();
                    colRest.IsReadOnly = true;
                    colRest.CellTemplate = restTemplate;
                    colRest.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);
                    Columns.Add(colRest);
                }
            }

            if (viewModel == null)
            {
                return;
            }
            foreach (var model in viewModel.Items)
            {

                model.EnsureSets();
            }

            
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
            view.Refresh();
        }

        #endregion

        #region Supersets

        public void AddSupersets()
        {
            selectRowsFromCells();
            string superSetGroup = Guid.NewGuid().ToString();
            var regions = SelectedItems;
            for (int i = regions.Count - 1; i >= 0; i--)
            {
                var item = (StrengthTrainingItemViewModel)regions[i];
                if (item != null)
                {
                    item.SuperSetGroup = superSetGroup;
                }
            }
            checkSuperSets(entry);
            RefreshItems();
        }

        void checkSuperSets(StrengthTrainingEntryDTO entry)
        {
            //get all entries with GroupName (superset)
            var entries = entry.Entries.Where(x => !string.IsNullOrEmpty(x.SuperSetGroup)).ToList();

            foreach (var item in entries)
            {
                if (entry.Entries.Count(x => x.SuperSetGroup == item.SuperSetGroup) < 2)
                {
                    item.SuperSetGroup = null;
                }
            }
        }

        public void RemoveSupersets()
        {
            selectRowsFromCells();
            var regions = SelectedItems;
            for (int i = regions.Count - 1; i >= 0; i--)
            {
                var item = (StrengthTrainingItemViewModel)regions[i];
                if (item != null)
                {
                    item.SuperSetGroup = null;
                }
            }
            checkSuperSets(entry);
            RefreshItems();
        }

        #endregion

        public GridGroupMode GroupMode
        {
            get { return groupMode; }
            set
            {
                if(groupMode!=value)
                {
                    groupMode = value;
                    groupItemsBy(value);
                }
                
            }
        }

        protected int StandardColumnsNumber
        {
            get
            {
                if(ShowExerciseTypeColumn)
                {
                    return 3;
                }
                return 2;
            }
        }

        public bool ShowExerciseTypeColumn
        {
            get { return UserContext.Current.Settings.GuiState.ShowExerciseTypeColumn; }
            set
            {
                if (UserContext.Current.Settings.GuiState.ShowExerciseTypeColumn != value)
                {

                    UserContext.Current.Settings.GuiState.ShowExerciseTypeColumn = value;
                    BuildGridColumns(currentSetsNumber);
                }
            }
        }

        public bool ShowRestColumns
        {
            get { return UserContext.Current.Settings.GuiState.ShowRestTimeColumns; }
            set
            {
                if (UserContext.Current.Settings.GuiState.ShowRestTimeColumns != value)
                {
                    UserContext.Current.Settings.GuiState.ShowRestTimeColumns = value;
                    BuildGridColumns(currentSetsNumber);
                }
            }
        }

        private void groupItemsBy(GridGroupMode groupMode)
        {
            EnsureNotEditMode();
            try
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
                view.GroupDescriptions.Clear();
                if (groupMode == GridGroupMode.ByExerciseType)
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription("ExerciseType"));
                }
                else if (groupMode == GridGroupMode.BySuperSets)
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription("SuperSetName"));
                }
                RefreshItems();
            }
            catch
            {
            }
            
        }

        //zwraca true gdy jest zaznaczona cellka z serią własnie do wprowadzenia (celka z (repsxweight))
        public bool IsNewSetSelected()
        {
            if (CurrentCell == null || CurrentCell.Column == null || GetCurrentSetIndex() < 0)
            {
                return false;
            }
            var item = CurrentCell.Item as StrengthTrainingItemViewModel;
            if (item != null && item.Item.Series.Count == GetCurrentSetIndex())
            {
                return true;
            }
            return false;
        }

        public SetViewModel GetCurrentSetCell()
        {
            if (CurrentCell.Column == null || GetCurrentSetIndex() < 0)
            {
                return null;
            }
            var item = CurrentCell.Item as StrengthTrainingItemViewModel;
            if (item == null)
            {
                return null;
            }
            var setViewModel = item.GetSetViewModel(GetCurrentSetIndex());
            return setViewModel;
        }



        public SetViewModel GetSetCellForCurrentRest()
        {
            if (CurrentCell.Column == null || GetSetIndexForCurrentRest() < 0)
            {
                return null;
            }
            var item = CurrentCell.Item as StrengthTrainingItemViewModel;
            if (item == null)
            {
                return null;
            }
            var setViewModel = item.GetSetViewModel(GetSetIndexForCurrentRest());
            return setViewModel;
        }

        public bool IsRestColumnSelected()
        {
            return CurrentCell.Column != null && GetSetIndexForCurrentRest() > -1 && GetCurrentSetIndex() == -1;
        }

        public int GetSetIndexForCurrentRest()
        {
            if (ShowRestColumns)
            {
                if ((CurrentCell.Column.DisplayIndex - StandardColumnsNumber) >= 0)
                {
                    return (CurrentCell.Column.DisplayIndex - StandardColumnsNumber)/2;
                }
                return -1;
            }
            return CurrentCell.Column.DisplayIndex - StandardColumnsNumber;
        }

        public int GetCurrentSetIndex()
        {
            if(ShowRestColumns)
            {
                if ((CurrentCell.Column.DisplayIndex - StandardColumnsNumber)%2==0)
                {
                    return (CurrentCell.Column.DisplayIndex - StandardColumnsNumber) / 2;    
                }
                else
                {
                    return -1;//rest column selected so threat this as no set selected
                }
                
            }
            return CurrentCell.Column.DisplayIndex - StandardColumnsNumber;
        }

    }

    public enum GridGroupMode
    {
        None,
        ByExerciseType,
        BySuperSets
    }

    /*This template selector could replace datatrigger for changing template for cell but there is a problem with refreshing after changing set's value
    public class SetTemplateSelector : DataTemplateSelector
    {
        private bool forViewCell;

        public SetTemplateSelector(bool forViewCell)
        {
            this.forViewCell = forViewCell;
        }

        private DataTemplate createEditable(int setNumber)
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(WatermarkTextBox));
            Binding b = new Binding(setNumber + ".Value");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.Default;
            factory.SetValue(WatermarkTextBox.TextProperty, b);

            factory.SetValue(WatermarkTextBox.WatermarkProperty, "(reps)x(weight)");
            factory.SetValue(Masking.MaskProperty, string.Format(@"^[0-9]*x?[0-9]*[\{0}]?[0-9]*$", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            // Create the template itself, and add the factory to it.
            DataTemplate cellEditingTemplate = new DataTemplate();
            cellEditingTemplate.VisualTree = factory;

            return cellEditingTemplate;
        }

        private DataTemplate createReadOnly(int setNumber)
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.Name = "txtBlock";
            var b = new Binding(setNumber + ".DisplayValue");
            b.Mode = BindingMode.OneWay;
            factory.SetValue(TextBlock.TextProperty, b);
            b = new Binding(setNumber + ".ToolTip");
            b.Mode = BindingMode.OneWay;
            factory.SetValue(TextBlock.ToolTipProperty, b);
            // Create the template itself, and add the factory to it.
            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = factory;

            DataTrigger trigger = new DataTrigger();
            var binding = new Binding(setNumber + ".IsFromPlan");
            trigger.Binding = binding;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(TextBlock.BackgroundProperty, new SolidColorBrush(Colors.LightYellow), "txtBlock"));
            cellTemplate.Triggers.Add(trigger);
            return cellTemplate;
        }

        DataTemplate createDisabled(int setNumber)
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.BackgroundProperty, new SolidColorBrush(Colors.LightGray));

            // Create the template itself, and add the factory to it.
            DataTemplate disabledTemplate = new DataTemplate();
            disabledTemplate.VisualTree = factory;
            return disabledTemplate;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ContentPresenter presenter = container as ContentPresenter;
            DataGridCell cell = presenter.Parent as DataGridCell;
            int setNumber = cell.Column.DisplayIndex - usrStrengthTraining.StandardColumnsNumber;
            StrengthTrainingItemViewModel viewModel = item as StrengthTrainingItemViewModel;
            if (viewModel != null)
            {
                var setViewModel = viewModel.GetSetViewModel(setNumber);
                if (setViewModel.IsReadOnly)
                {
                    return createDisabled(setNumber);
                }
                else
                {
                    return forViewCell ? createReadOnly(setNumber) : createEditable(setNumber);
                }
            }
            else
            {
                return createDisabled(setNumber);
            }
        }

    }
     * 
     * */
}
