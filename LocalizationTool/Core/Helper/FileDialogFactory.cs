using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationTool
{
    public static class FileDialogFactory<T> where T : FileDialog, new()
    {
        public static T CreateFileDialog(string title, string extension)
        {
            return new T() { Title = title, Filter = string.Format("Data Files(*.{0}) | *.{0}", extension), DefaultExt = extension, AddExtension = true };
        }

        public static T CreateFileDialog(string title)
        {
            return new T() { Title = title };
        }
    }
}
