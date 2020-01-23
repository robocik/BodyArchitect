using System.Windows;

namespace BodyArchitect.Client.Module.Blog
{
    internal static class FrameworkElementExtension
    {
        /// <summary>
        /// Get the window container of framework element.
        /// </summary>
        public static Window GetParentWindow(this FrameworkElement element)
        {
            DependencyObject dp = element;
            while (dp != null)
            {
                DependencyObject tp = LogicalTreeHelper.GetParent(dp);
                if (tp is Window) return tp as Window;
                else dp = tp;
            }
            return null;
        }
    }
}
