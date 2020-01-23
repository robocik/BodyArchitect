using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;

namespace BodyArchitect.Controls.UserControls
{
    /// <summary>
    /// GroupControl with expand/collapse capabilities.
    /// Collapsed size should be choosed carefuly as it gives a wrong display with some skins.
    /// <author>Alexandre MUTEL</author>
    /// <email>alexandre_mutelATyahoo.fr</email>
    /// </summary>
    public class ExpandGroupControl : BaGroupControl
    {
        private int _expandedSize;
        private int _collapsedSize;
        private bool _isCaptionClickable;
        private Cursor _captionCursor;
        private Image _contentBackgroundImage;
        private ImageLayout _contentBackgroundImageLayout;
        public event EventHandler ExpandedChanaged;

        public ExpandGroupControl()
        {
            _collapsedSize = 2;
            _isCaptionClickable = true;
            _captionCursor = Cursors.Hand;
            _contentBackgroundImage = null;
            _contentBackgroundImageLayout = ImageLayout.None;
        }

        [Category("Expand Behaviour"), Description("Cursor used when caption is clickable"), DefaultValue(typeof(Cursor), "Hand")]
        public Cursor CaptionCursor
        {
            get { return _captionCursor; }
            set { _captionCursor = value; }
        }

        [Category("Expand Behaviour"), Description("Specify if caption can be clickable for expand/collapse operation"), DefaultValue(true)]
        public bool IsCaptionClickable
        {
            get { return _isCaptionClickable; }
            set { _isCaptionClickable = value; }
        }

        [Category("Expand Behaviour"), Description("Set the expansion state. True is expanded, false is collapsed"), DefaultValue(true)]
        public bool Expanded
        {
            get { return ViewInfo.Expanded; }
            set
            {
                ViewInfo.Expanded = value;
                UpdateExpanded();
            }
        }

        [Localizable(true), Category("Expand Behaviour"), DefaultValue(null)]
        public Image ContentBackgroundImage
        {
            get { return _contentBackgroundImage; }
            set { _contentBackgroundImage = value; }
        }

        [Category("Expand Behaviour"), DefaultValue(ImageLayout.None)]
        public ImageLayout ContentBackgroundImageLayout
        {
            get { return _contentBackgroundImageLayout; }
            set { _contentBackgroundImageLayout = value; }
        }

        [Category("Expand Behaviour"), Description("Size of the group when expanded")]
        public int ExpandedSize
        {
            get { return _expandedSize; }
            set
            {
                _expandedSize = value;
                if (Expanded)
                    Size = new Size(Size.Width, value);
            }
        }

        [Category("Expand Behaviour"), Description("Size of the group when collapsed"), DefaultValue(2)]
        public int CollapsedSize
        {
            get { return _collapsedSize; }
            set { _collapsedSize = value; }
        }

        private Skin Skin
        {
            get { return CommonSkins.GetSkin(((SkinGroupObjectPainter)Painter).Provider); }
        }

        private bool IsCaptionHorizontal
        {
            get
            {
                return (CaptionLocation == Locations.Top || CaptionLocation == Locations.Bottom ||
                 CaptionLocation == Locations.Default);
            }

        }

        private int GroupSize
        {
            get
            {
                if (IsCaptionHorizontal)
                {
                    return Height;
                }
                return Width;
            }
            set
            {
                if (IsCaptionHorizontal)
                {
                    Height = value;
                }
                else
                {
                    Width = value;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_expandedSize != GroupSize && Expanded)
            {
                _expandedSize = GroupSize;
            } if (!Expanded && GroupSize != CalcTotalCollapsedSize())
            {
                UpdateExpanded();
            }
        }

        protected override void CheckInfoCore(GroupObjectInfoArgs info, bool performLayout)
        {
            base.CheckInfoCore(info, performLayout);
            info.StateIndex = 1;
            info.ButtonBounds = CalcGroupCaptionButtonBounds(info);
        }

        protected Rectangle CalcGroupCaptionButtonBounds(GroupObjectInfoArgs info)
        {
            Rectangle result = Rectangle.Empty;
            Rectangle captionClient = info.CaptionBounds;
            if (Painter is SkinGroupObjectPainter)
            {
                SkinElementInfo buttonInfo = new SkinElementInfo(Skin[CommonSkins.SkinGroupPanelExpandButton]);
                Size buttonSize =
                    ObjectPainter.CalcObjectMinBounds(info.Graphics, SkinElementPainter.Default, buttonInfo).Size;
                if (buttonSize.IsEmpty) return Rectangle.Empty;
                if (!IsCaptionHorizontal)
                {
                    int saveWidth = captionClient.Width;
                    captionClient.Width = captionClient.Height;
                    captionClient.Height = saveWidth;
                    saveWidth = captionClient.X;
                    captionClient.X = captionClient.Y;
                    captionClient.Y = saveWidth;
                }
                captionClient.Width -= 4;
                result = Skin[CommonSkins.SkinGroupPanelExpandButton].Offset.GetBounds(captionClient, buttonSize, SkinOffsetKind.Far);
                if (!IsCaptionHorizontal)
                {
                    int saveX = result.X;
                    if (CaptionLocation == Locations.Left)
                    {
                        result.X = result.Y;
                        result.Y = captionClient.Width - saveX;
                    }
                    else
                    {
                        result.X = captionClient.Y;
                        result.Y = saveX;
                    }
                }
            }
            else
            {
                Size btnSize = new ExplorerBarOpenCloseButtonObjectPainter().CalcObjectMinBounds(new ExplorerBarOpenCloseButtonInfoArgs(info.Cache, Rectangle.Empty, info.Appearance, ObjectState.Normal, true)).Size;
                result = captionClient;
                result.Size = btnSize;
                if (IsCaptionHorizontal)
                {
                    result.X = (captionClient.Right - result.Width) - 4;
                    result.Y += (captionClient.Height - result.Height) / 2;
                }
                else if (CaptionLocation == Locations.Left)
                {
                    result.X = (captionClient.Width - result.Width) / 2;
                    result.Y += captionClient.Top + 4;
                }
                else
                {
                    result.X = captionClient.X + (captionClient.Width - result.Width) / 2;
                    result.Y += (captionClient.Bottom - result.Width) - 4;
                }
            }
            return result;
        }

        private object buttonPressed;
        private int lastStateIndex = 1;

        public void UpdateHitState(GroupObjectInfoArgs info, Point pt)
        {
            info.ButtonState = ObjectState.Normal;

            info.StateIndex = 1;
            Cursor = Cursors.Default;
            if (info.CaptionBounds.Contains(pt) && IsCaptionClickable)
            {
                info.StateIndex = 0;
                Cursor = CaptionCursor;
            }
            if (lastStateIndex != info.StateIndex)
                Invalidate(info.CaptionBounds);
            lastStateIndex = info.StateIndex;

            if (info.ButtonBounds.Contains(pt))
            {
                info.ButtonState = ObjectState.Hot;
            }
            else
            {
                buttonPressed = null;
            }
            if (info.ButtonState == ObjectState.Hot && buttonPressed != null)
            {
                if ((MouseButtons)buttonPressed == MouseButtons.Left)
                {
                    info.ButtonState = ObjectState.Pressed;
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            buttonPressed = e.Button;
            UpdateHitState(ViewInfo, new Point(e.X, e.Y));
            Invalidate(ViewInfo.ButtonBounds);
        }

        private int CalcTotalCollapsedSize()
        {
            Size size = ViewInfo.CaptionBounds.Size;
            return ((IsCaptionHorizontal) ? size.Height : size.Width) + CollapsedSize;
        }

        private void UpdateExpanded()
        {
            buttonPressed = null;
            Invalidate(ViewInfo.ButtonBounds);
            Rectangle bounds = Bounds;
            if (ViewInfo.Expanded)
            {
                if (CaptionLocation == Locations.Bottom)
                {
                    bounds.Y = bounds.Y + bounds.Height - ExpandedSize;
                }
                else if (CaptionLocation == Locations.Right)
                {
                    bounds.X = bounds.X + bounds.Width - ExpandedSize;
                }

                if (IsCaptionHorizontal)
                    bounds.Size = new Size(Size.Width, ExpandedSize);
                else
                    bounds.Size = new Size(ExpandedSize, Size.Height);
            }
            else
            {
                
                Size size = Size;
                int newSize = CalcTotalCollapsedSize();
                if (IsCaptionHorizontal)
                    size.Height = newSize;
                else
                {
                    size.Width = newSize;
                }
                bounds.Size = size;

                if (CaptionLocation == Locations.Bottom)
                {
                    bounds.Y = bounds.Y + ExpandedSize - newSize;
                }
                else if (CaptionLocation == Locations.Right)
                {
                    bounds.X = bounds.X + ExpandedSize - newSize;
                }
            }
            Bounds = bounds;
            if (ExpandedChanaged != null)
            {
                ExpandedChanaged(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (ViewInfo.ButtonState == ObjectState.Pressed)
            {
                ViewInfo.Expanded = !ViewInfo.Expanded;
                ViewInfo.ButtonState = ObjectState.Normal;
                UpdateExpanded();
            }
            else if (ViewInfo.CaptionBounds.Contains(e.Location) && e.Button == MouseButtons.Left && IsCaptionClickable)
            {
                ViewInfo.Expanded = !ViewInfo.Expanded;
                ViewInfo.ButtonState = ObjectState.Normal;
                //ExpandedSize = Size.Height;//store last expanded size
                UpdateExpanded();

            }

        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateHitState(ViewInfo, e.Location);
            Invalidate(ViewInfo.ButtonBounds);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ContentBackgroundImage != null)
            {
                BackgroundImagePainter.DrawBackgroundImage(e.Graphics, ContentBackgroundImage, BackColor, ContentBackgroundImageLayout, ViewInfo.ControlClientBounds, ViewInfo.ControlClientBounds, Point.Empty, RightToLeft.Inherit);
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateHitState(ViewInfo, PointToClient(MousePosition));
            Invalidate(ViewInfo.ButtonBounds);
        }
    }
}
