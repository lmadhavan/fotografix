using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MenuBar = Microsoft.UI.Xaml.Controls.MenuBar;

namespace Fotografix.Uwp
{
    /// <summary>
    /// This is a workaround for https://github.com/microsoft/microsoft-ui-xaml/issues/5437.
    /// Because accelerators tied to menu item do reliably invoke command bindings, we duplicate
    /// all the accelerators and invoke the command directly.
    /// </summary>
    public static class KeyboardAcceleratorWorkaround
    {
        public static void CreateShadowAccelerators(this MenuBar menuBar, UIElement target)
        {
            foreach (var menu in menuBar.Items)
            {
                ProcessItems(menu.Items, target);
            }
        }

        private static void ProcessItems(IList<MenuFlyoutItemBase> items, UIElement target)
        {
            foreach (var item in items)
            {
                if (item is MenuFlyoutSubItem i)
                {
                    ProcessItems(i.Items, target);
                }
                else if (item is MenuFlyoutItem fi)
                {
                    ProcessItem(fi, target);
                }
            }
        }

        private static void ProcessItem(MenuFlyoutItem fi, UIElement target)
        {
            foreach (var accel in fi.KeyboardAccelerators)
            {
                var shadowAccel = new KeyboardAccelerator
                {
                    Modifiers = accel.Modifiers,
                    Key = accel.Key
                };

                shadowAccel.Invoked += (s, e) =>
                {
                    if (fi.Command.CanExecute(fi.CommandParameter))
                    {
                        fi.Command.Execute(fi.CommandParameter);
                    }

                    e.Handled = true;
                };

                target.KeyboardAccelerators.Add(shadowAccel);
            }
        }
    }
}
