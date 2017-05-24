using System.Linq;
using System.Windows;
using Printer_Status.Printers;

namespace Printer_Status.Helpers
{
    /// <summary>
    /// Helper class for managing windows.
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// Find the first Window of type <typeparamref name="TWindow"/> with the title <paramref name="title"/>
        /// </summary>
        /// <typeparam name="TWindow">The type of window to find.</typeparam>
        /// <param name="title">The title of the window to find.</param>
        /// <returns>The window object of the first window if found, otherwise null.</returns>
        public static Window FirstWindow<TWindow>(string title = null) where TWindow : Window
        {
            //Get a list of windows of type TWindow.
            var windows = Application.Current.Windows.OfType<TWindow>().ToList();
            //If a title was specfified, filter the list to those with that title.
            if (!string.IsNullOrEmpty(title)) windows = windows.Where(win => win.Title == title).ToList();
            //If there are any windows in the list, return the first window (there should only ever be one).
            //Otherwise return null.
            return windows.Any() ? windows.First() : null;
        }

        /// <summary>
        /// Focuses an already existing or otherwise new DetailWindow for <paramref name="printer"/>.
        /// </summary>
        /// <param name="printer">The printer to show a DetailWindow for.</param>
        public static void ShowOrFocus(Printer printer)
        {
            //Get the first window with a DetailWindow-style title.
            var window = FirstWindow<DetailWindow>($"{printer.IpAddress} - Printer Details");
            //If a window was found, focus it.
            if (window != null) window.Focus();
            //Otherwise:
            else
            {
                //Create a new DetailWindow for the printer.
                window = new DetailWindow(printer);
                //Show the window.
                window.Show();
            }
        }
        /// <summary>
        /// Focuses an already existing or otherwise new <typeparamref name="TWindow"/> with the title <paramref name="title"/>
        /// </summary>
        /// <typeparam name="TWindow">The type of Window to show.</typeparam>
        /// <param name="title">The title of the window to show.</param>
        public static void ShowOrFocus<TWindow>(string title = null) where TWindow : Window, new()
        {
            //Get the first window of this type.
            var window = FirstWindow<TWindow>(title);
            //If a window was found, focus it.
            if (window != null) window.Focus();
            //Otherwise:
            else
            {
                //Create a new window of type TWindow.
                window = new TWindow();
                //Show the window.
                window.Show();
            }
        }
    }
}