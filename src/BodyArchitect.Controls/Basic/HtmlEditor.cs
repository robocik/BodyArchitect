using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BodyArchitect.Controls.Basic
{
    public class HtmlEditor : Control
    {
        #region Constants

        protected const string DefaultHtml =
            @"<html>
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
</head>
<body></body>
</html>";

        protected const string JSFunctions =
            @"<script>
function getCommandValue(commandId){
    return document.queryCommandValue(commandId);
}
function getSelectionStart(){
    var range=document.selection.createRange().duplicate();
    var length=range.text.length;
    range.moveStart('character', -0x7FFFFFFF);//Move to the beginning
    return range.text.length-length;
}
function setSelection(start,length){
    var range=document.selection.createRange();
    range.collapse(true);
    range.moveStart('character', -0x7FFFFFFF);//Move to the beginning
    range.moveStart('character', start);
    range.moveEnd('character', length);
    range.select();
}
function setSelectedHtml(html){
    document.selection.createRange().pasteHTML(html);
}
function setSelectedText(text){
    document.selection.createRange().text=text;
}

var findRange;
function findString(text,settings){
    if (findRange!=null) 
        findRange.collapse(false);
    else
        findRange=document.body.createTextRange();

    var strFound=findRange.findText(text);
    if (strFound) findRange.select();
    else findRange=null;

   return strFound;
}
</script>";

        #endregion

        protected WebBrowser _webBrowser;

        public HtmlEditor()
        {
            _webBrowser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                WebBrowserShortcutsEnabled = true,
                ScriptErrorsSuppressed = true,
                AllowWebBrowserDrop = false
            };
            
            BackColor = Color.White;
            //  _webBrowser.AcceptsTab = _webBrowser.AcceptsReturn = true;

            HandleCreated += delegate
            {
                if (Controls.Count == 0)
                    SourceHTML = DefaultHtml;
            };
        }

        #region Delegates

        public delegate bool KeyPressEventHandler(HtmlEditor sender, KeyEventArgs keyData);

        #endregion
        
        #region Events

        /// <summary>
        /// Evento invocado cuando cambia el contenido del documento en edición
        /// </summary>
        public event EventHandler ContentChanged;

        /// <summary>
        /// Evento invocado cuando se actualiza el estado de la interfaz gráfica (cambia el
        /// puntero de edición, se pega un nuevo texto, etc.)
        /// </summary>
        public event EventHandler UpdateUI;

        /// <summary>
        /// Evento invocado cuando se presiona alguna tecla modificadora (Ctrl,Alt,Shift)
        /// Devuelve true cuando la tecla se ha manejado con código propio, false
        /// para usar el manejador por defecto
        /// </summary>
        public event KeyPressEventHandler HotkeyPress;

        #endregion

        #region Properties

        /// <summary>
        /// Enable or disable the design mode of the web browser control.
        /// </summary>
        /// <remarks>
        /// http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=153990&SiteID=1
        /// </remarks>
        [DefaultValue(false)]
        public bool WebBrowserDesignMode
        {
            get
            {
                if (DesignMode || _webBrowser.Document == null)
                {
                    return false;
                }
                else
                {
                    if (_webBrowser.ActiveXInstance == null)
                    {
                        return false;
                    }
                    else
                    {
                        var instance =
                            Microsoft.VisualBasic.CompilerServices.NewLateBinding.LateGet(
                                _webBrowser.ActiveXInstance,
                                null,
                                @"Document",
                                new object[0],
                                null,
                                null,
                                null);

                        var dm =
                            Convert.ToString(
                                Microsoft.VisualBasic.CompilerServices.NewLateBinding.LateGet(
                                    instance,
                                    null,
                                    @"designMode",
                                    new object[0],
                                    null,
                                    null,
                                    null));

                        return dm == @"On";
                    }
                }
            }
            set
            {
                if (!DesignMode)
                {
                    if (_webBrowser.Document != null)
                    {
                        var instance =
                            Microsoft.VisualBasic.CompilerServices.NewLateBinding.LateGet(
                                _webBrowser.ActiveXInstance,
                                null,
                                @"Document",
                                new object[0],
                                null,
                                null,
                                null);

                        var objArray1 = new object[] { value ? @"On" : @"Off" };

                        Microsoft.VisualBasic.CompilerServices.NewLateBinding.LateSetComplex(
                            instance,
                            null,
                            @"designMode",
                            objArray1,
                            null,
                            null,
                            false,
                            true);
                    }
                    //else
                    //{
                    //    _postPoneSetWebBrowserDesignMode = value;
                    //}
                }
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SourceHTML
        {
            set
            {
                if (DesignMode)
                    return;

                if (Controls.Count == 0)
                    Controls.Add(_webBrowser);

                _webBrowser.Navigate("about:blank");
                _webBrowser.Document.ContextMenuShowing += ShowContextMenu;

                //Wait document load
                for (int i = 0; i < 200 && _webBrowser.ReadyState != WebBrowserReadyState.Complete; i++)
                {
                    Application.DoEvents();
                    Thread.Sleep(5);
                }
                //Write the JS functions
                _webBrowser.Document.Write(JSFunctions);

                //Write html code
                _webBrowser.Document.Write(value);

                //Enable contentEditable
                _webBrowser.Document.Body.SetAttribute("contentEditable", "true");

                //Attacth events handlers
                _webBrowser.Document.AttachEventHandler("onmouseup", OnUpdateUI);
                _webBrowser.Document.AttachEventHandler("onblur", OnUpdateUI);

                _webBrowser.Document.AttachEventHandler("onkeyup", OnContentChanged);
                _webBrowser.Document.AttachEventHandler("onkeypress", OnContentChanged);
                _webBrowser.Document.AttachEventHandler("ondrop", OnContentChanged);
                _webBrowser.Document.AttachEventHandler("oncut", OnContentChanged);
                _webBrowser.Document.AttachEventHandler("onpaste", OnContentChanged);

                _webBrowser.Document.Body.KeyDown += DocumentKeyDown;

                //Invoke ContentChanged event
                OnContentChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string BodyHTML
        {
            get { return _webBrowser.Document.Body.InnerHtml; }
            set
            {
                _webBrowser.Document.Body.InnerHtml = value;
                OnContentChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get { return _webBrowser.Document.Body.InnerText; }
            set { _webBrowser.Document.Body.InnerText = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HtmlElement ActiveElement
        {
            get { return _webBrowser.Document.ActiveElement; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedHTML
        {
            get
            {
                return
                    _webBrowser.Document.InvokeScript("eval", new object[] { "document.selection.createRange().htmlText" })
                        .ToString();
            }
            set
            {
                _webBrowser.Document.InvokeScript("setSelectedHtml", new object[] { value });
                OnContentChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get
            {
                return
                    _webBrowser.Document.InvokeScript("eval", new object[] { "document.selection.createRange().text" }).
                        ToString();
            }
            set
            {
                _webBrowser.Document.InvokeScript("setSelectedText", new object[] { value });
                OnContentChanged(this, EventArgs.Empty);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font SelectionFont
        {
            get { return new Font(SelectionFontName, SelectionFontSize); }
            set
            {
                if (value != null)
                {
                    SelectionFontName = value.Name;
                    SelectionFontSize = (int)value.SizeInPoints;

                    SelectionBold = value.Bold;
                    SelectionItalic = value.Italic;
                    SelectionUnderline = value.Underline;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectionFontName
        {
            get { return QueryCommandValue("FontName").ToString(); }
            set { ExecCommand("FontName", false, value); }
        }


        /// <summary>
        /// Obtiene o establece el tamaño, en puntos, de la fuente del texto seleccionado
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionFontSize
        {
            get
            {
                object res = QueryCommandValue("FontSize");
                if (res is DBNull)
                    return 0;

                switch (Convert.ToInt32(res))
                {
                    case 1:
                        return 8;
                    case 2:
                        return 10;
                    case 3:
                        return 12;
                    case 4:
                        return 18;
                    case 5:
                        return 24;
                    case 6:
                        return 36;
                    case 7:
                        return 48;

                    default:
                        return 10;
                }
            }
            set
            {
                int size;
                if (value <= 8)
                    size = 1;
                else if (value <= 10)
                    size = 2;
                else if (value <= 12)
                    size = 3;
                else if (value <= 18)
                    size = 4;
                else if (value <= 24)
                    size = 5;
                else if (value <= 36)
                    size = 6;
                else
                    size = 7;

                ExecCommand("FontSize", false, size);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionBold
        {
            get { return Convert.ToBoolean(QueryCommandValue("Bold")); }
            set
            {
                if (SelectionBold != value)
                    ExecCommand("Bold", false, null);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionItalic
        {
            get { return Convert.ToBoolean(QueryCommandValue("Italic")); }
            set
            {
                if (SelectionItalic != value)
                    ExecCommand("Italic", false, null);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionUnderline
        {
            get { return Convert.ToBoolean(QueryCommandValue("Underline")); }
            set
            {
                if (SelectionUnderline != value)
                    ExecCommand("Underline", false, null);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionSubscript
        {
            get { return Convert.ToBoolean(QueryCommandValue("Subscript")); }
            set
            {
                if (SelectionSubscript != value)
                    ExecCommand("Subscript", false, null);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionSuperscript
        {
            get { return Convert.ToBoolean(QueryCommandValue("Superscript")); }
            set
            {
                if (SelectionSuperscript != value)
                    ExecCommand("Superscript", false, null);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionBullets
        {
            get { return Convert.ToBoolean(QueryCommandValue("InsertUnorderedList")); }
            set
            {
                if (SelectionBullets != value)
                    ExecCommand("InsertUnorderedList", false, null);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SelectionNumbering
        {
            get { return Convert.ToBoolean(QueryCommandValue("InsertOrderedList")); }
            set
            {
                if (SelectionNumbering != value)
                    ExecCommand("InsertOrderedList", false, null);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectionForeColor
        {
            get
            {
                try
                {
                    return ColorTranslator.FromHtml(QueryCommandValue("ForeColor").ToString());
                }
                catch
                {
                    return Color.Empty;
                }
            }
            set { ExecCommand("ForeColor", false, ColorTranslator.ToHtml(value)); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectionBackColor
        {
            get
            {
                try
                {
                    return ColorTranslator.FromHtml(QueryCommandValue("BackColor").ToString());
                }
                catch
                {
                    return Color.Empty;
                }
            }
            set { ExecCommand("BackColor", false, ColorTranslator.ToHtml(value)); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HorizontalAlignment SelectionAlignment
        {
            get
            {
                if (Convert.ToBoolean(QueryCommandValue("JustifyRight")))
                    return HorizontalAlignment.Right;
                if (Convert.ToBoolean(QueryCommandValue("JustifyCenter")))
                    return HorizontalAlignment.Center;

                return HorizontalAlignment.Left;
            }
            set
            {
                switch (value)
                {
                    case HorizontalAlignment.Left:
                        ExecCommand("JustifyLeft", false, null);
                        break;
                    case HorizontalAlignment.Center:
                        ExecCommand("JustifyCenter", false, null);
                        break;
                    case HorizontalAlignment.Right:
                        ExecCommand("JustifyRight", false, null);
                        break;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart
        {
            get { return (int)_webBrowser.Document.InvokeScript("getSelectionStart", null); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get { return SelectedText.Length; }
        }

        public bool IsContextMenuEnabled { get; set; }


        public override bool Focused
        {
            get { return _webBrowser.Focused; }
        }

        public int TextLength
        {
            get { return Text.Length; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Otorga el foco de entrada al control
        /// </summary>
        public new void Focus()
        {
            _webBrowser.Document.Focus();
        }

        public void InsertHtml(string html, bool replaceSelection)
        {
            if (replaceSelection)
                SelectedHTML = html;
            else
                SelectedHTML += html;
        }

        public bool CanCopy
        {
            get
            {
                return true;
                //return Convert.ToBoolean(true);
            }
        }
        public bool CanCut
        {
            get
            {
                return true;
                //return Convert.ToBoolean(true);
            }
        }
        public bool CanPaste
        {
            get
            {
                return true;
                //return Convert.ToBoolean(true);
            }
        }

        public void Copy()
        {
            ExecCommand("Copy", false, null);
        }

        public void Cut()
        {
            ExecCommand("Cut", false, null);
        }

        public void Paste()
        {
            ExecCommand("Paste", false, null);
        }

        public void SetSelectionTag(string tag)
        {
            try
            {
                ExecCommand("FormatBlock", false, "<" + tag + ">");
            }
            catch
            {
                SelectedHTML = string.Format("<{0}>{1}</{0}>", tag, SelectedHTML);
            }
        }

        public void Indent()
        {
            ExecCommand("Indent", false, null);
        }

        public void Outdent()
        {
            ExecCommand("Outdent", false, null);
        }

        public void ClearSelectionFormatting()
        {
            ExecCommand("RemoveFormat", false, null);
        }

        public void Undo()
        {
            ExecCommand("Undo", false, null);
        }

        public void Redo()
        {
            ExecCommand("Redo", false, null);
        }

        public void SelectAll()
        {
            ExecCommand("SelectAll", false, null);
        }

        public void DeleteSelection()
        {
            ExecCommand("Delete", false, null);
        }

        public void Select(int start, int lenght)
        {
            _webBrowser.Document.InvokeScript("setSelection", new object[] { start, lenght });
        }


        public void ClearContent()
        {
            if (_webBrowser.Document==null)
            {
                _webBrowser.DocumentText = "";  
            }
            //
            //_webBrowser.Document.ExecCommand("EditMode", false, null);
            _webBrowser.Document.OpenNew(true);
            //_webBrowser.DocumentText = "";
            
            Application.DoEvents();
        }

        public bool FindString(string text)
        {
            return FindString(text, true, false);
        }

        public bool FindString(string text, bool caseInsensitive, bool wholeWords)
        {
            int options = 0;
            if (caseInsensitive)
                options |= 4;
            if (wholeWords)
                options |= 2;

            return (bool)_webBrowser.Document.InvokeScript("findString", new object[] { text, options });
        }

        #endregion

        #region Private methods

        private void DocumentKeyDown(object sender, HtmlElementEventArgs e)
        {
            if (HotkeyPress != null)
            {
                Keys keys = Keys.None;

                if (e.CtrlKeyPressed)
                    keys = keys | Keys.Control;
                if (e.AltKeyPressed)
                    keys = keys | Keys.Alt;
                if (e.ShiftKeyPressed)
                    keys = keys | Keys.ShiftKey;

                if (keys != Keys.None)
                    e.ReturnValue = !HotkeyPress(this, new KeyEventArgs(keys | (Keys)e.KeyPressedCode));
            }
        }

        private void ShowContextMenu(object sender, HtmlElementEventArgs e)
        {
            if (ContextMenuStrip != null)
            {
                ContextMenuStrip.Show(this, e.MousePosition);
                e.ReturnValue = false;
            }
            else if (IsContextMenuEnabled == false)
            {
                e.ReturnValue = false;
            }
        }

        private void OnUpdateUI(object sender, EventArgs e)
        {
            if (UpdateUI != null)
                UpdateUI(sender, e);
        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            if (ContentChanged != null)
                ContentChanged(sender, e);
            OnUpdateUI(sender, e);
        }

        protected object QueryCommandValue(string commandId)
        {
            return _webBrowser.Document.InvokeScript("getCommandValue", new object[] { commandId });
        }


        protected void ExecCommand(string command, bool showUI, object value)
        {
            _webBrowser.Document.ExecCommand(command, showUI, value);
            OnContentChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Static Methods

        private static readonly Regex CleanXssRegex = new Regex("<script.*?/script>|\"javascript:",
                                                                RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                                RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static string CleanXSS(string html)
        {
            return CleanXssRegex.Replace(html, string.Empty);
        }

        #endregion
    }
}