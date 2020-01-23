using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit;

namespace BodyArchitect.Client.UI.Controls.WatermarkExtension
{
    public sealed class WatermarkCheckComboBoxBehavior
    {
        private readonly CheckComboBox m_ComboBox;
        private TextBlockAdorner m_TextBlockAdorner;

        private WatermarkCheckComboBoxBehavior(CheckComboBox comboBox)
        {
            if (comboBox == null)
                throw new ArgumentNullException("comboBox");

            m_ComboBox = comboBox;
        }

        #region Behavior Internals

        private static WatermarkCheckComboBoxBehavior GetWatermarkCheckComboBoxBehavior(DependencyObject obj)
        {
            return (WatermarkCheckComboBoxBehavior)obj.GetValue(WatermarkCheckComboBoxBehaviorProperty);
        }

        private static void SetWatermarkCheckComboBoxBehavior(DependencyObject obj, WatermarkCheckComboBoxBehavior value)
        {
            obj.SetValue(WatermarkCheckComboBoxBehaviorProperty, value);
        }

        private static readonly DependencyProperty WatermarkCheckComboBoxBehaviorProperty =
            DependencyProperty.RegisterAttached("WatermarkCheckComboBoxBehavior",
                typeof(WatermarkCheckComboBoxBehavior), typeof(WatermarkCheckComboBoxBehavior), new UIPropertyMetadata(null));

        public static bool GetEnableWatermark(CheckComboBox obj)
        {
            return (bool)obj.GetValue(EnableWatermarkProperty);
        }

        public static void SetEnableWatermark(CheckComboBox obj, bool value)
        {
            obj.SetValue(EnableWatermarkProperty, value);
        }

        public static readonly DependencyProperty EnableWatermarkProperty =
            DependencyProperty.RegisterAttached("EnableWatermark", typeof(bool),
                typeof(WatermarkCheckComboBoxBehavior), new UIPropertyMetadata(false, OnEnableWatermarkChanged));

        private static void OnEnableWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                var enabled = (bool)e.OldValue;

                if (enabled)
                {
                    var comboBox = (CheckComboBox)d;
                    var behavior = GetWatermarkCheckComboBoxBehavior(comboBox);
                    behavior.Detach();

                    SetWatermarkCheckComboBoxBehavior(comboBox, null);
                }
            }

            if (e.NewValue != null)
            {
                var enabled = (bool)e.NewValue;

                if (enabled)
                {
                    var comboBox = (CheckComboBox)d;
                    var behavior = new WatermarkCheckComboBoxBehavior(comboBox);
                    behavior.Attach();

                    SetWatermarkCheckComboBoxBehavior(comboBox, behavior);
                }
            }
        }

        private void Attach()
        {
            m_ComboBox.Loaded += ComboBoxLoaded;
            m_ComboBox.DragEnter += ComboBoxDragEnter;
            m_ComboBox.DragLeave += ComboBoxDragLeave;
        }

        private void Detach()
        {
            m_ComboBox.Loaded -= ComboBoxLoaded;
            m_ComboBox.DragEnter -= ComboBoxDragEnter;
            m_ComboBox.DragLeave -= ComboBoxDragLeave;
        }

        private void ComboBoxDragLeave(object sender, DragEventArgs e)
        {
            UpdateAdorner();
        }

        private void ComboBoxDragEnter(object sender, DragEventArgs e)
        {
            m_ComboBox.TryRemoveAdorners<TextBlockAdorner>();
        }

        private void ComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        #endregion

        #region Attached Properties

        #region Label

        public static string GetLabel(CheckComboBox obj)
        {
            return (string)obj.GetValue(LabelProperty);
        }

        public static void SetLabel(CheckComboBox obj, string value)
        {
            obj.SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.RegisterAttached("Label", typeof(string), typeof(WatermarkCheckComboBoxBehavior));

        #endregion

        #region LabelStyle

        public static Style GetLabelStyle(CheckComboBox obj)
        {
            return (Style)obj.GetValue(LabelStyleProperty);
        }

        public static void SetLabelStyle(CheckComboBox obj, Style value)
        {
            obj.SetValue(LabelStyleProperty, value);
        }

        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.RegisterAttached("LabelStyle", typeof(Style),
                                                typeof(WatermarkCheckComboBoxBehavior));

        #endregion

        #endregion

        private void Init()
        {
            m_TextBlockAdorner = new TextBlockAdorner(m_ComboBox, GetLabel(m_ComboBox), GetLabelStyle(m_ComboBox));
            UpdateAdorner();

            DependencyPropertyDescriptor focusProp = DependencyPropertyDescriptor.FromProperty(UIElement.IsFocusedProperty, typeof(CheckComboBox));
            if (focusProp != null)
            {
                focusProp.AddValueChanged(m_ComboBox, (sender, args) => UpdateAdorner());
            }

            DependencyPropertyDescriptor focusKeyboardProp = DependencyPropertyDescriptor.FromProperty(UIElement.IsKeyboardFocusedProperty, typeof(CheckComboBox));
            if (focusKeyboardProp != null)
            {
                focusKeyboardProp.AddValueChanged(m_ComboBox, (sender, args) => UpdateAdorner());
            }

            DependencyPropertyDescriptor focusKeyboardWithinProp = DependencyPropertyDescriptor.FromProperty(UIElement.IsKeyboardFocusWithinProperty, typeof(CheckComboBox));
            if (focusKeyboardWithinProp != null)
            {
                focusKeyboardWithinProp.AddValueChanged(m_ComboBox, (sender, args) => UpdateAdorner());
            }

            DependencyPropertyDescriptor textProp = DependencyPropertyDescriptor.FromProperty(CheckComboBox.TextProperty, typeof(CheckComboBox));
            if (textProp != null)
            {
                textProp.AddValueChanged(m_ComboBox, (sender, args) => UpdateAdorner());
            }

            DependencyPropertyDescriptor selectedIndexProp = DependencyPropertyDescriptor.FromProperty(Selector.SelectedIndexProperty, typeof(CheckComboBox));
            if (selectedIndexProp != null)
            {
                selectedIndexProp.AddValueChanged(m_ComboBox, (sender, args) => UpdateAdorner());
            }

            DependencyPropertyDescriptor selectedItemProp = DependencyPropertyDescriptor.FromProperty(Selector.SelectedItemProperty, typeof(CheckComboBox));
            if (selectedItemProp != null)
            {
                selectedItemProp.AddValueChanged(m_ComboBox, (sender, args) => UpdateAdorner());
            }
        }

        private void UpdateAdorner()
        {
            if (!string.IsNullOrEmpty(m_ComboBox.Text) ||
                m_ComboBox.IsFocused ||
                m_ComboBox.IsKeyboardFocused ||
                m_ComboBox.IsKeyboardFocusWithin ||
                //m_ComboBox.SelectedIndex != -1 ||
                m_ComboBox.SelectedItem != null)
            {
                // Hide the Watermark Label if the adorner layer is visible
                m_ComboBox.ToolTip = GetLabel(m_ComboBox);
                m_ComboBox.TryRemoveAdorners<TextBlockAdorner>();
            }
            else
            {
                // Show the Watermark Label if the adorner layer is visible
                m_ComboBox.ToolTip = null;
                m_ComboBox.TryAddAdorner<TextBlockAdorner>(m_TextBlockAdorner);
            }
        }
    }
}
