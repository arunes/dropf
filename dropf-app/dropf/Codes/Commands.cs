using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace dropf
{
    public static class Commands
    {
        // Drop.xaml commands
        public static readonly RoutedUICommand NIconDoubleClick = new RoutedUICommand("Notify Double Clicked", "NIconDoubleClick", typeof(Drop));
    }
}
