using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BodyArchitect.Client.UI.Controls
{
    public class ImageButtonExt
    {
         #region Image dependency property

        /// <summary>
        /// An attached dependency property which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static readonly DependencyProperty ImageProperty;

        /// <summary>
        /// Gets the <see cref="ImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static ImageSource GetImage(DependencyObject obj)
        {
          return (ImageSource) obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="ImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static void SetImage(DependencyObject obj, ImageSource value)
        {
          obj.SetValue(ImageProperty, value);
        }

        #endregion


        #region EmptyListMessage dependency property

        /// <summary>
        /// An attached dependency property which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static readonly DependencyProperty EmptyListMessageProperty;

        /// <summary>
        /// Gets the <see cref="EmptyListMessageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static String GetEmptyListMessage(DependencyObject obj)
        {
            return (string)obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="EmptyListMessageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static void SetEmptyListMessage(DependencyObject obj, string value)
        {
            obj.SetValue(ImageProperty, value);
        }

        #endregion

        #region IsLoading dependency property

        /// <summary>
        /// An attached dependency property which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty;

        /// <summary>
        /// Gets the <see cref="IsLoadingProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static bool GetIsLoading(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLoadingProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="EmptyListMessageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static void SetIsLoading(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLoadingProperty, value);
        }

        #endregion

        static ImageButtonExt()
        {
          //register attached dependency property
            var metadata = new FrameworkPropertyMetadata((ImageSource)null);
          ImageProperty = DependencyProperty.RegisterAttached("Image",
                                                              typeof (ImageSource),
                                                              typeof(ImageButtonExt), metadata);

          metadata = new FrameworkPropertyMetadata((string)null);
          EmptyListMessageProperty = DependencyProperty.RegisterAttached("EmptyListMessage",
                                                              typeof(string),
                                                              typeof(ImageButtonExt), metadata);

          metadata = new FrameworkPropertyMetadata(false);
          IsLoadingProperty = DependencyProperty.RegisterAttached("IsLoading",
                                                              typeof(bool),
                                                              typeof(ImageButtonExt), metadata);
        }
    }
}
