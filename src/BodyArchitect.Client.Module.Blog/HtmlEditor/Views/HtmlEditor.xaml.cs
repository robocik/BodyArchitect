using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.XPath;
using mshtml;

namespace BodyArchitect.Client.Module.Blog
{
    public partial class HtmlEditor : INotifyPropertyChanged
    {
        #region Fields

        List<Brush> standardColors = new List<Brush>();
        List<Brush> themeColors = new List<Brush>();
        List<Brush> automaticFontColors = new List<Brush>();
        List<Brush> automaticFontBackgroundColors = new List<Brush>();
        
        private HtmlDocument htmldoc;
        private Window hostedWindow;
        private DispatcherTimer styleTimer;
        private Dictionary<string, ImageObject> imageDic;
        private string stylesheet;
        bool isDocReady;
        List<RowColumnCount> tableData=new List<RowColumnCount>();
        
        #endregion

        #region Constructor

        public HtmlEditor()
        {
            InitializeComponent();
            InitContainer();
            InitStyles();
            InitEvents();
            InitTimer();
            AllowHtmlCode = false;
            fillColorsPicker();
            fillTablePicker();
        }

        void fillTablePicker()
        {
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    tableData.Add(new RowColumnCount() { RowCount = i, ColumnCount = j });
                }
            }
        }

        void fillColorsPicker()
        {
            float percent10 = 0.90f;
            float percent25 = 0.75f;
            float percent40 = 0.60f;
            float percent55 = 0.45f;
            float percent70 = 0.30f;

            automaticFontColors.Add(Brushes.Black);
            automaticFontBackgroundColors.Add(Brushes.Transparent);

            themeColors.Add(new SolidColorBrush(Brushes.White.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Black.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Tan.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.DarkBlue.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Blue.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Red.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Olive.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Purple.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Aqua.Color * percent10));
            themeColors.Add(new SolidColorBrush(Brushes.Orange.Color * percent10));

            themeColors.Add(new SolidColorBrush(Brushes.White.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Black.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Tan.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.DarkBlue.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Blue.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Red.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Olive.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Purple.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Aqua.Color * percent25));
            themeColors.Add(new SolidColorBrush(Brushes.Orange.Color * percent25));

            themeColors.Add(new SolidColorBrush(Brushes.White.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Black.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Tan.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.DarkBlue.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Blue.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Red.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Olive.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Purple.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Aqua.Color * percent40));
            themeColors.Add(new SolidColorBrush(Brushes.Orange.Color * percent40));

            themeColors.Add(new SolidColorBrush(Brushes.White.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Black.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Tan.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.DarkBlue.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Blue.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Red.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Olive.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Purple.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Aqua.Color * percent55));
            themeColors.Add(new SolidColorBrush(Brushes.Orange.Color * percent55));

            themeColors.Add(new SolidColorBrush(Brushes.White.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Black.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Tan.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.DarkBlue.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Blue.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Red.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Olive.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Purple.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Aqua.Color * percent70));
            themeColors.Add(new SolidColorBrush(Brushes.Orange.Color * percent70));


            standardColors.Add(Brushes.DarkRed);
            standardColors.Add(Brushes.Red);
            standardColors.Add(Brushes.Orange);
            standardColors.Add(Brushes.Yellow);
            standardColors.Add(Brushes.LightGreen);
            standardColors.Add(Brushes.Green);
            standardColors.Add(Brushes.LightBlue);
            standardColors.Add(Brushes.Blue);
            standardColors.Add(Brushes.DarkBlue);
            standardColors.Add(Brushes.Purple);
        }

        #endregion       

        #region Events

        #region Document Ready Event

        public static readonly RoutedEvent DocumentReadyEvent =
            EventManager.RegisterRoutedEvent("DocumentReady", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(HtmlEditor));

        /// <summary>
        /// Raise when the document is ready.
        /// </summary>
        public event RoutedEventHandler DocumentReady
        {
            add { base.AddHandler(DocumentReadyEvent, value); }
            remove { base.RemoveHandler(DocumentReadyEvent, value); }
        }

        #endregion

        #region Document State Changed Event

        public static readonly RoutedEvent DocumentStateChangedEvent =
            EventManager.RegisterRoutedEvent("DocumentStateChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(HtmlEditor));

        /// <summary>
        /// Raise when the state of document is changed.
        /// </summary>
        public event RoutedEventHandler DocumentStateChanged
        {
            add { base.AddHandler(DocumentStateChangedEvent, value); }
            remove { base.RemoveHandler(DocumentStateChangedEvent, value); }
        }

        #endregion

        #endregion

        #region Initalize Inner Events

        private void InitEvents()
        {
            this.Loaded += new RoutedEventHandler(OnHtmlEditorLoaded);
            this.Unloaded += new RoutedEventHandler(OnHtmlEditorUnloaded);
        }

        private void OnCodeModeChecked(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.Source;
        }

        private void OnCodeModeUnchecked(object sender, RoutedEventArgs e)
        {
            EditMode = EditMode.Visual;
        }

        private void OnHtmlEditorLoaded(object sender, RoutedEventArgs e)
        {
            imageDic = new Dictionary<string, ImageObject>();
            this.hostedWindow = this.GetParentWindow();
            styleTimer.Start();
        }

        private void OnHtmlEditorUnloaded(object sender, RoutedEventArgs e)
        {
            styleTimer.Stop();
        }


        #endregion

        #region Initalize Editors
   
        RoutedEventArgs DocumentStateChangedEventArgs = new RoutedEventArgs(HtmlEditor.DocumentStateChangedEvent);

        private void InitContainer()
        {
            LoadStylesheet();
            VisualEditor.Navigated += this.OnVisualEditorDocumentNavigated;
            VisualEditor.StatusTextChanged += this.OnVisualEditorStatusTextChanged;
            VisualEditor.DocumentText = String.Empty;
        }

        private void OnVisualEditorStatusTextChanged(object sender, EventArgs e)
        {
            if (Document == null) return;

            RaiseEvent(DocumentStateChangedEventArgs);
            if (Document.State == HtmlDocumentState.Complete)
            {
                if (isDocReady)
                {
                    Dispatcher.BeginInvoke(new Action(this.NotifyBindingContentChanged));
                }
                else
                {
                    isDocReady = true;                    
                    RaiseEvent(new RoutedEventArgs(HtmlEditor.DocumentReadyEvent));
                }
            }
        }

        private void OnVisualEditorDocumentNavigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {            
            VisualEditor.Document.ContextMenuShowing += this.OnDocumentContextMenuShowing;
            htmldoc = new HtmlDocument(VisualEditor.Document);
            //((IHTMLDocument2)VisualEditor.Document.DomDocument).designMode = "ON";
            SetStylesheet();
            SetInitialContent();
            VisualEditor.Document.Body.SetAttribute("contenteditable", (!ReadOnly).ToString());
            VisualEditor.Document.Focus();
        }

        private void OnDocumentContextMenuShowing(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            EditingContextMenu.IsOpen = true;
            e.ReturnValue = false;
        }
        
        /// <summary>
        /// Set style for visual editor.
        /// </summary>
        private void SetStylesheet()
        {
            if (stylesheet != null && VisualEditor.Document != null)
            {
                HTMLDocument hdoc = (HTMLDocument)VisualEditor.Document.DomDocument;
                IHTMLStyleSheet hstyle = hdoc.createStyleSheet("", 0);
                hstyle.cssText = stylesheet;
            }
        }

        /// <summary>
        /// Set the inital content of editor
        /// </summary>
        private void SetInitialContent()
        {
            if (myBindingContent != null)
                VisualEditor.Document.Body.InnerHtml = myBindingContent;
        }

        /// <summary>
        /// Get the content from editor.
        /// </summary>
        private string GetEditContent()
        {
            switch (mode)
            {
                case EditMode.Visual:
                    return VisualEditor.Document.Body.InnerHtml;
                default:
                    return CodeEditor.Text;
            }
        }

        #endregion

        #region Initalize Timer

        private void InitTimer()
        {
            styleTimer = new DispatcherTimer();
            styleTimer.Interval = TimeSpan.FromMilliseconds(200);
            styleTimer.Tick += new EventHandler(OnTimerTick);
        }

        protected void onPropertyChanged(string name)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(name));
            }
        }

        private bool isBold;
        public bool IsBold
        {
            get { return isBold; }
            set 
            {
                isBold = value;
                onPropertyChanged("IsBold");
            }
        }

        private bool isItalic;
        public bool IsItalic
        {
            get { return isItalic; }
            set
            {
                isItalic = value;
                onPropertyChanged("IsItalic");
            }
        }

        private bool isBulletList;
        public bool IsBulletList
        {
            get { return isBulletList; }
            set
            {
                isBulletList = value;
                onPropertyChanged("IsBulletList");
            }
        }

        private bool isNumberedList;
        public bool IsNumberedList
        {
            get { return isNumberedList; }
            set
            {
                isNumberedList = value;
                onPropertyChanged("IsNumberedList");
            }
        }

        private bool isJustifyLeft;
        public bool IsJustifyLeft
        {
            get { return isJustifyLeft; }
            set
            {
                isJustifyLeft = value;
                onPropertyChanged("IsJustifyLeft");
            }
        }

        private bool isJustifyRight;
        public bool IsJustifyRight
        {
            get { return isJustifyRight; }
            set
            {
                isJustifyRight = value;
                onPropertyChanged("IsJustifyRight");
            }
        }

        private bool isJustifyCenter;
        public bool IsJustifyCenter
        {
            get { return isJustifyCenter; }
            set
            {
                isJustifyCenter = value;
                onPropertyChanged("IsJustifyCenter");
            }
        }

        private bool isJustifyStretch;
        public bool IsJustifyStretch
        {
            get { return isJustifyStretch; }
            set
            {
                isJustifyStretch = value;
                onPropertyChanged("IsJustifyStretch");
            }
        }

        private bool isUnderline;
        public bool IsUnderline
        {
            get { return isUnderline; }
            set
            {
                isUnderline = value;
                onPropertyChanged("IsUnderline");
            }
        }

        

        private bool isSubscript;
        public bool IsSubscript
        {
            get { return isSubscript; }
            set
            {
                isSubscript = value;
                onPropertyChanged("IsSubscript");
            }
        }

        private bool isSuperscript;
        public bool IsSuperscript
        {
            get { return isSuperscript; }
            set
            {
                isSuperscript = value;
                onPropertyChanged("IsSuperscript");
            }
        }

        private FontFamily selectedFontFamily;
        public FontFamily SelectedFontFamily
        {
            get { return selectedFontFamily; }
            set
            {
                selectedFontFamily = value;
                SetFontFamily(value);
                onPropertyChanged("SelectedFontFamily");
            }
        }

        private Brush selectedFontBrush;
        public Brush SelectedFontBrush
        {
            get { return selectedFontBrush; }
            set
            {
                selectedFontBrush = value;
                SetFontColor(value);
                onPropertyChanged("SelectedFontBrush");
            }
        }

        private RowColumnCount selectedTableData;
        public RowColumnCount SelectedTableData
        {
            get { return selectedTableData; }
            set
            {
                selectedTableData = value;
                onPropertyChanged("SelectedTableData");
            }
        }

        private Brush selectedFontBackgroundBrush;
        public Brush SelectedFontBackgroundBrush
        {
            get { return selectedFontBackgroundBrush; }
            set
            {
                selectedFontBackgroundBrush = value;
                SetFontBackgroundColor(value);
                onPropertyChanged("SelectedFontBackgroundBrush");
            }
        }

        private FontSize selectedFontSize;
        public FontSize SelectedFontSize
        {
            get { return selectedFontSize; }
            set
            {
                selectedFontSize = value;
                SetFontSize(selectedFontSize);
                onPropertyChanged("SelectedFontSize");
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (htmldoc.State != HtmlDocumentState.Complete) return;
            IsBold= htmldoc.IsBold();
            IsItalic= htmldoc.IsItalic();
            IsUnderline = htmldoc.IsUnderline();
            IsSubscript = htmldoc.IsSubscript();
            IsSuperscript = htmldoc.IsSuperscript();
            IsBulletList = htmldoc.IsBulletsList();
            IsNumberedList = htmldoc.IsNumberedList();
            IsJustifyLeft = htmldoc.IsJustifyLeft();
            IsJustifyRight = htmldoc.IsJustifyRight();
            IsJustifyCenter = htmldoc.IsJustifyCenter();
            IsJustifyStretch = htmldoc.IsJustifyFull();

            SelectedFontFamily= htmldoc.GetFontFamily();
            SelectedFontSize= htmldoc.GetFontSize();
            
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region Initialize Styles

        /// <summary>
        /// Initalize font families and font sizes of visual editor.
        /// Initalize font family and font size of code editor.
        /// </summary>
        private void InitStyles()
        {            
            //ConfigProvider.Load();
            List<FontFamily> families = new List<FontFamily>();
            //List<FontSize> sizes = new List<FontSize>();
            FontFamily srcfamily = new FontFamily("Times New Roman");
            int srcsize = 10;

            try
            {
                // read configuration from file
                using (XmlReader reader = XmlTextReader.Create(ConfigPath))
                {
                    XPathDocument xmlDoc = new XPathDocument(reader);
                    XPathNavigator navDoc = xmlDoc.CreateNavigator();
                    XPathNodeIterator it;
                    // read visualmode/fontfamilies section
                    it = navDoc.Select(VisualFontFamiliesPath);
                    while (it.MoveNext())
                    {
                        FontFamily ff = new FontFamily(it.Current.Value);
                        families.Add(ff);
                    }
                    // read sourcemode/fontfamily section
                    it = navDoc.Select(SourceFontFamilyPath);
                    while (it.MoveNext())
                    {
                        srcfamily = new FontFamily(it.Current.Value);
                        break;
                    }
                    // read sourcemode/fontsize section
                    it = navDoc.Select(SourceFontSizePath);
                    while (it.MoveNext())
                    {
                        srcsize = it.Current.ValueAsInt;
                        break;
                    }
                }
                // set font families
                FontFamilies= new ReadOnlyCollection<FontFamily>(families);
            }
            catch (Exception)
            {

            }

            // set font sizes

            FontSizes = GetDefaultFontSizes();
            // set style of code editor
            CodeEditor.FontFamily = srcfamily;
            CodeEditor.FontSize = srcsize;
        }

        private ReadOnlyCollection<FontFamily> fontFamilies;
        private ReadOnlyCollection<FontSize> fontSizes;

        public ReadOnlyCollection<FontSize> FontSizes
        {
            get { return fontSizes; }
            private set { fontSizes = value; }
        }

        public ReadOnlyCollection<FontFamily> FontFamilies
        {
            get { return fontFamilies; }
            private set { fontFamilies = value; }
        }

        private ReadOnlyCollection<FontSize> GetDefaultFontSizes()
        {
            List<FontSize> ls = new List<FontSize>()
            {
                BodyArchitect.Client.Module.Blog.FontSize.XXSmall,
                BodyArchitect.Client.Module.Blog.FontSize.XSmall,
                BodyArchitect.Client.Module.Blog.FontSize.Small,
                BodyArchitect.Client.Module.Blog.FontSize.Middle,
                BodyArchitect.Client.Module.Blog.FontSize.Large,
                BodyArchitect.Client.Module.Blog.FontSize.XLarge,
                BodyArchitect.Client.Module.Blog.FontSize.XXLarge
            };
            return new ReadOnlyCollection<FontSize>(ls);
        } 

        /// <summary>
        /// Invoke when selected font family changed
        /// </summary>
        //private void OnFontFamilyChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    FontFamily selectedFontFamily = (FontFamily)FontFamilyList.SelectedValue;
        //    SetFontFamily(selectedFontFamily);
        //}

        public void SetFontFamily(FontFamily fontFamily)
        {
            if (htmldoc != null && fontFamily!=null)
            {
                FontFamily selectionFontFamily = htmldoc.GetFontFamily();
                if (fontFamily != selectionFontFamily) htmldoc.SetFontFamily(fontFamily);
            }
        }

        public void SetFontSize(FontSize fontSize)
        {
            if (htmldoc != null && fontSize!=null)
            {
                FontSize selectionFontSize = htmldoc.GetFontSize();
                if (fontSize != selectionFontSize) htmldoc.SetFontSize(fontSize);
            }
        }

        public void SetFontColor(Brush brush)
        {
            SolidColorBrush colorBrush = (SolidColorBrush) brush;
            htmldoc.SetFontColor(colorBrush.Color);
        }

        public void SetFontBackgroundColor(Brush brush)
        {
            SolidColorBrush colorBrush = (SolidColorBrush)brush;
            htmldoc.SetLineColor(colorBrush.Color);
        }

        /// <summary>
        /// Invoke when selected font size changed
        /// </summary>
        //private void OnFontSizeChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    FontSize selectedFontSize = (FontSize)FontSizeList.SelectedValue;
        //    SetFontSize(selectedFontSize);
        //}

        private void LoadStylesheet()
        {
            try
            {
                using (StreamReader reader = new StreamReader(StylesheetPath))
                {
                    stylesheet = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }            
        }

        private static readonly string ConfigPath = "smithhtmleditor.config.xml";
        private static readonly string StylesheetPath = "smithhtmleditor.stylesheet.css";
        private static readonly string VisualFontFamiliesPath = @"/smithhtmleditor/visualmode/fontfamilies/add/@value";
        private static readonly string SourceFontFamilyPath = @"/smithhtmleditor/sourcemode/fontfamily/@value";
        private static readonly string SourceFontSizePath = @"/smithhtmleditor/sourcemode/fontsize/@value";

        #endregion

        #region Properties

        public List<Brush> StandardColors
        {
            get { return standardColors; }
        }

        public List<Brush> ThemeColors
        {
            get { return themeColors; }
        }

        public List<Brush> AutomaticFontColors
        {
            get { return automaticFontColors; }
        }

        public List<Brush> AutomaticFontBackgroundColors
        {
            get { return automaticFontBackgroundColors; }
        }

        public List<RowColumnCount> TableData
        {
            get { return tableData; }
        }
        
        private bool _readOnly = false;
        public bool ReadOnly
        {
            get
            {
                if (VisualEditor.ActiveXInstance == null)
                {
                    return true;
                }
                return _readOnly;
            }
            set
            {
                _readOnly = value;
                var doc = VisualEditor.Document.DomDocument as IHTMLDocument2;
                doc.designMode = value ? "Off" : "On";
                //SetEditMode(false);

                //if (VisualEditor.Document != null)
                //{
                //    var instance =
                //        Microsoft.VisualBasic.CompilerServices.NewLateBinding.LateGet(
                //            VisualEditor.ActiveXInstance,
                //            null,
                //            @"Document",
                //            new object[0],
                //            null,
                //            null,
                //            null);

                //    var objArray1 = new object[] { value ? @"On" : @"Off" };

                //    Microsoft.VisualBasic.CompilerServices.NewLateBinding.LateSetComplex(
                //        instance,
                //        null,
                //        @"designMode",
                //        objArray1,
                //        null,
                //        null,
                //        false,
                //        true);
                //}
                onPropertyChanged("ReadOnly");
            }
        }

        private void SetEditMode(bool edit)
        {
            if (edit)
            {
                VisualEditor.Document.ExecCommand("BrowseMode", false, null);
            }
            else
            {
                //webBrowserBody.Document.BackColor = Color.White;
                string tempText = VisualEditor.DocumentText;
                VisualEditor.Document.ExecCommand("EditMode", false, null);
                VisualEditor.Document.OpenNew(true);
                VisualEditor.DocumentText = tempText;

            }
            VisualEditor.Document.ExecCommand("LiveResize", false, null);
        }

        #region EditMode Dependency Property

        private EditMode mode;

        /// <summary>
        /// Get or set the edit mode of editor.
        /// This is a dependency property.
        /// </summary>
        public EditMode EditMode
        {
            get { return (EditMode)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register("EditMode", typeof(EditMode), typeof(HtmlEditor),
                new FrameworkPropertyMetadata(EditMode.Visual, new PropertyChangedCallback(OnEditModeChanged)));

        private static void OnEditModeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditor editor = (HtmlEditor)sender;
            if ((EditMode)e.NewValue == EditMode.Visual) editor.SetVisualMode();
            else editor.SetSourceMode();
        }

        /// <summary>
        /// Set the editor to visual mode.
        /// </summary>
        private void SetVisualMode()
        {
            if (mode != EditMode.Visual)
            {
                BrowserHost.Visibility = Visibility.Visible;
                CodeEditor.Visibility = Visibility.Hidden;

                //FontFamilyList.IsEnabled = true;
                //FontSizeList.IsEnabled = true;
                //ToggleFontColor.IsEnabled = true;
                //ToggleLineColor.IsEnabled = true;

                VisualEditor.Document.Body.InnerHtml = GetEditContent();
                mode = EditMode.Visual;
            }
        }

        /// <summary>
        /// Set the editor to source mode.
        /// </summary>
        private void SetSourceMode()
        {
            if (mode != EditMode.Source)
            {
                BrowserHost.Visibility = Visibility.Hidden;
                CodeEditor.Visibility = Visibility.Visible;

                //FontFamilyList.IsEnabled = false;
                //FontSizeList.IsEnabled = false;
                //ToggleFontColor.IsEnabled = false;
                //ToggleLineColor.IsEnabled = false;

                CodeEditor.Text = GetEditContent();
                mode = EditMode.Source;
            }
        }

        #endregion        

        #region BindingContent Dependency Property

        private string myBindingContent = string.Empty;

        public string BindingContent
        {
            get { return (string)GetValue(BindingContentProperty); }
            set { SetValue(BindingContentProperty, value); }
        }

        public static readonly DependencyProperty BindingContentProperty =
            DependencyProperty.Register("BindingContent", typeof(string), typeof(HtmlEditor),
                new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnBindingContentChanged)));

        private static void OnBindingContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HtmlEditor editor = (HtmlEditor)sender;
            editor.myBindingContent = (string)e.NewValue;
            editor.ContentHtml = editor.myBindingContent;            
        }

        private void NotifyBindingContentChanged()
        {
            if (myBindingContent != this.ContentHtml)
            {
                BindingContent = this.ContentHtml;
            }
        }

        #endregion

        public bool AllowHtmlCode
        {
            get { return false; }
            set
            {
                
            }
        }

        /// <summary>
        /// 获取字数统计。
        /// Get word count.
        /// </summary>
        public int WordCount
        {
            get
            {
                // get word count in code editor when source mode is on
                //if (ToggleCodeMode.IsChecked == true)
                //{
                //    WordCounter counter = WordCounter.Create();
                //    return counter.Count(CodeEditor.Text);
                //}
                // get word count in html editor when visual mode is on
                //else
                if (htmldoc != null && htmldoc.Content != null)
                {
                    WordCounter counter = WordCounter.Create();
                    return counter.Count(htmldoc.Text);
                }
                return 0;
            }
        }

        /// <summary>
        /// 获取或设置编辑器中的HTML内容。
        /// Get or set the html content.
        /// </summary>
        public string ContentHtml
        {
            get
            {
                //if (ToggleCodeMode.IsChecked == true)
                //    VisualEditor.Document.Body.InnerHtml = CodeEditor.Text;
                return VisualEditor.Document.Body.InnerHtml;
            }
            set
            {
                value = (value != null ? value : string.Empty);
                BindingContent = value;
                if (VisualEditor.Document != null && VisualEditor.Document.Body != null)
                    VisualEditor.Document.Body.InnerHtml = value;
                
                //if (ToggleCodeMode.IsChecked == true)
                //    CodeEditor.Text = VisualEditor.Document.Body.InnerHtml;
            }
        }

        /// <summary>
        /// 获取编辑器中的文本内容。
        /// Get the text content.
        /// </summary>
        public string ContentText
        {
            get
            {
                //if (ToggleCodeMode.IsChecked == true)
                //    VisualEditor.Document.Body.InnerHtml = CodeEditor.Text;
                return VisualEditor.Document.Body.InnerText;
            }
        }

        /// <summary>
        /// 获取HTML文档对象。
        /// Get the html document of editor.
        /// </summary>
        public HtmlDocument Document
        {
            get { return htmldoc; }
        }

        /// <summary>
        /// 获取一个值，撤销命令是否可执行。
        /// Get a value that indicated if the undo command is enabled.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return
                    mode == EditMode.Visual && 
                    htmldoc != null && 
                    htmldoc.QueryCommandEnabled("Undo");
            }
        }

        /// <summary>
        /// 获取一个值，指示重做命令是否可执行。
        /// Get a value that indicated if the redo command is enabled.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return 
                    mode == EditMode.Visual && 
                    htmldoc != null && 
                    htmldoc.QueryCommandEnabled("Redo");
            }
        }

        /// <summary>
        /// 获取一个值，指示剪切命令是否可执行。
        /// Get a value that indicated if the cut command is enabled.
        /// </summary>
        public bool CanCut
        {
            get
            {
                return
                    mode == EditMode.Visual && 
                    htmldoc != null && 
                    htmldoc.QueryCommandEnabled("Cut");
            }
        }

        /// <summary>
        /// 获取一个值，指示复制命令是否可执行。
        /// Get a value that indicated if the copy command is enabled.
        /// </summary>
        public bool CanCopy
        {
            get
            {
                return 
                    mode == EditMode.Visual && 
                    htmldoc != null && 
                    htmldoc.QueryCommandEnabled("Copy");
            }
        }

        /// <summary>
        /// 获取一个值，指示粘贴命令是否可执行。
        /// Get a value that indicated if the paste command is enabled.
        /// </summary>
        public bool CanPaste
        {
            get
            {
                return 
                    mode == EditMode.Visual && 
                    htmldoc != null && 
                    htmldoc.QueryCommandEnabled("Paste");
            }
        }

        /// <summary>
        /// 获取一个值，指示删除命令是否可执行。
        /// Get a value that indicated if the delete command is enabled.
        /// </summary>
        public bool CanDelete
        {
            get
            {
                return 
                    mode == EditMode.Visual && 
                    htmldoc != null && 
                    htmldoc.QueryCommandEnabled("Delete");
            }
        }

        #endregion

        #region Execute Commands

        /// <summary>
        /// 执行撤销命令。
        /// Execute the undo command.
        /// </summary>
        public void Undo()
        {
            if (htmldoc != null) 
                htmldoc.ExecuteCommand("Undo", false, null);
        }

        /// <summary>
        /// 执行重做命令。
        /// Execute the redo command.
        /// </summary>
        public void Redo()
        {
            if (htmldoc != null) 
                htmldoc.ExecuteCommand("Redo", false, null);
        }

        /// <summary>
        /// 执行剪切命令。
        /// Execute the cut command.
        /// </summary>
        public void Cut()
        {
            if (htmldoc != null) 
                htmldoc.ExecuteCommand("Cut", false, null);
        }

        /// <summary>
        /// 执行复制命令。
        /// Execute the copy command.
        /// </summary>
        public void Copy()
        {
            if (htmldoc != null)
                htmldoc.ExecuteCommand("Copy", false, null);
        }

        /// <summary>
        /// 执行粘贴命令。
        /// Execute the paste command.
        /// </summary>
        public void Paste()
        {
            if (htmldoc != null) 
                htmldoc.ExecuteCommand("Paste", false, null);
        }

        /// <summary>
        /// 执行删除命令。
        /// Execute the delete command.
        /// </summary>
        public void Delete()
        {
            if (htmldoc != null) 
                htmldoc.ExecuteCommand("Delete", false, null);
        }

        /// <summary>
        /// 执行全选命令。
        /// Execute the select all command.
        /// </summary>
        public void SelectAll()
        {
            if (htmldoc != null) 
                htmldoc.ExecuteCommand("SelectAll", false, null);
        }

        #endregion

        #region Command Event Bindings

        public void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }

        public void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanUndo;
            if (IsModifiedChanged != null)
            {
                IsModifiedChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsModifiedChanged;

        public void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }

        public void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanRedo;
        }

        public void CutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Cut();
        }

        public void CutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanCut;
        }

        public void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
        }

        public void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanCopy;
        }

        public void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Paste();
        }

        public void PasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanPaste;
        }

        public void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Delete();
        }

        public void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanDelete;
        }

        public void SelectAllExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectAll();
        }

        public void BoldExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Bold();
        }

        public void ItalicExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Italic();
        }

        public void UnderlineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Underline();
        }

        public void SubscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Subscript();
        }

        public void SubscriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mode == EditMode.Visual && htmldoc != null && htmldoc.CanSubscript());
        }

        public void SuperscriptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Superscript();
        }

        public void SuperscriptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mode == EditMode.Visual && htmldoc != null && htmldoc.CanSuperscript());
        }

        public void ClearStyleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.ClearStyle();
        }

        public void IndentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Indent();
        }

        public void OutdentExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.Outdent();
        }

        public void BubbledListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.BulletsList();
        }

        public void NumericListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.NumberedList();
        }

        public void JustifyLeftExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyLeft();
        }

        public void JustifyRightExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyRight();
        }

        public void JustifyCenterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyCenter();
        }

        public void JustifyFullExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null) htmldoc.JustifyFull();
        }

        public void EditingCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (htmldoc != null && mode == EditMode.Visual);
        }

        /// <summary>
        /// 插入超链接事件
        /// </summary>
        public void InsertHyperlinkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                HyperlinkDialog d = new HyperlinkDialog();
                d.Owner = this.hostedWindow;
                d.Model = new HyperlinkObject { URL = "http://", Text = htmldoc.Selection.Text };
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertHyperlick(d.Model);
                }
            }
        }

        /// <summary>
        /// 插入图像事件
        /// </summary>
        public void InsertImageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                ImageDialog d = new ImageDialog();
                d.Owner = this.hostedWindow;
                if (d.ShowDialog() == true)
                {
                    htmldoc.InsertImage(d.Model);
                    imageDic[d.Model.ImageUrl] = d.Model;
                }
            }
        }

        /// <summary>
        /// 插入表格事件
        /// </summary>
        public void InsertTableExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (htmldoc != null)
            {
                TableDialog d = new TableDialog();
                d.Owner = this.hostedWindow;
                if (d.ShowDialog() == true)
                {
                    InsertHtmlTable(d.Model);
                }
            }
        }

        public void InsertHtmlTable(TableObject obj)
        {
            htmldoc.InsertTable(obj);
        }

        private void InsertCodeBlockExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        } 

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
