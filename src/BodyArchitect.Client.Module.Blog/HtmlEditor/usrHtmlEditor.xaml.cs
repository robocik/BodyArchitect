using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Blog;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrHtmlEditor.xaml
    /// </summary>
    public partial class usrHtmlEditor
    {

        public event EventHandler IsModifiedChanged;

        private bool isModified;
        public bool IsModified
        {
            get { return isModified; }
            set
            {
                if (isModified != value)
                {
                    isModified = value;
                    if (IsModifiedChanged != null)
                    {
                        IsModifiedChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public usrHtmlEditor()
        {
            InitializeComponent();
            DataContext = htmlEditor1;
            ReadOnly = true;
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Paste, htmlEditor1.PasteExecuted,htmlEditor1.PasteCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Cut, htmlEditor1.CutExecuted, htmlEditor1.CutCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Copy, htmlEditor1.CopyExecuted, htmlEditor1.CopyCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Bold, htmlEditor1.BoldExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Italic, htmlEditor1.ItalicExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Underline, htmlEditor1.UnderlineExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Superscript, htmlEditor1.SuperscriptExecuted, htmlEditor1.SuperscriptCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Subscript, htmlEditor1.SubscriptExecuted, htmlEditor1.SubscriptCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.ClearStyle, htmlEditor1.ClearStyleExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Outdent, htmlEditor1.OutdentExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Indent, htmlEditor1.IndentExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.BubbledList, htmlEditor1.BubbledListExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.NumericList, htmlEditor1.NumericListExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.JustifyCenter, htmlEditor1.JustifyCenterExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.JustifyFull, htmlEditor1.JustifyFullExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.JustifyLeft, htmlEditor1.JustifyLeftExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.JustifyRight, htmlEditor1.JustifyRightExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Undo, htmlEditor1.UndoExecuted, htmlEditor1.UndoCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.Redo, htmlEditor1.RedoExecuted, htmlEditor1.RedoCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.InsertTable, htmlEditor1.InsertTableExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.InsertImage, htmlEditor1.InsertImageExecuted, htmlEditor1.EditingCommandCanExecute));
            CommandBindings.Add(new CommandBinding(HtmlEditingCommands.InsertHyperlink, htmlEditor1.InsertHyperlinkExecuted, htmlEditor1.EditingCommandCanExecute));
        }

        

        public bool ReadOnly
        {
            get { return htmlEditor1.ReadOnly; }
            set
            {
                htmlEditor1.ReadOnly = value;
            }
        }

        public string GetHtml()
        {
            if (!Dispatcher.CheckAccess())
            {
                return (string)Dispatcher.Invoke(new Func<string>(GetHtml));
            }
            else
            {
                return htmlEditor1.ContentHtml;
            }
        }

        public void SetHtml(string html)
        {
            htmlEditor1.ContentHtml = html;
        }

        public void ClearContent()
        {
            htmlEditor1.ContentHtml = "";
        }

        private void btnMoreColors_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            e.Handled = true;
            ColorDialog  dlg = new ColorDialog();
            if(dlg.ShowDialog()==DialogResult.OK)
            {
                var brush = new SolidColorBrush(dlg.Color.ToMediaColor());
                htmlEditor1.SetFontColor(brush);
            }
        }

        private void btnAutomaticFontColor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            e.Handled = true;
            htmlEditor1.SetFontColor(htmlEditor1.AutomaticFontColors[0]);
        }

        private void btnAutomaticFontBacgroundColor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            e.Handled = true;
            htmlEditor1.SetFontBackgroundColor(htmlEditor1.AutomaticFontBackgroundColors[0]);
        }


        private void InsertTableGallery_SelectionChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var obj=new TableObject
                {
                    Columns = htmlEditor1.SelectedTableData.ColumnCount,
                    Rows = htmlEditor1.SelectedTableData.RowCount,
                    Border = 1,
                    Width = 100,
                    Height = 100,
                    WidthUnit = Unit.Percentage,
                    HeightUnit = Unit.Pixel,
                    SpacingUnit = Unit.Pixel,
                    PaddingUnit = Unit.Pixel,
                    HeaderOption = TableHeaderOption.Default,
                    Alignment = TableAlignment.Default
                };
            htmlEditor1.InsertHtmlTable(obj);
        }

        private void htmlEditor1_IsModifiedChanged(object sender, EventArgs e)
        {
            if(IsModifiedChanged!=null)
            {
                IsModifiedChanged(this, EventArgs.Empty);
            }
        }

    }
}
