using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace HiLib
{
    public class IconCache
    {
        private static Dictionary<string, ImageSource> _iconDictionary = new Dictionary<string, ImageSource>();
        private static HashSet<string> _ignoreSet = new HashSet<string>();

        public IconCache()
        {
            _ignoreSet.Add(".exe");
            _ignoreSet.Add(".ico");
            _ignoreSet.Add(".bmp");
    
        }

        private ImageSource GetIcon(string path)
        {
            ushort uicon;
            StringBuilder sb = new StringBuilder(path);
            IntPtr hInst = default(IntPtr);
            IntPtr handle = HiLib.Win32.ExtractAssociatedIcon(hInst, sb, out uicon);
            Icon icon = System.Drawing.Icon.FromHandle(handle);

            using (Bitmap bmp = icon.ToBitmap())
            {
                var stream = new MemoryStream();
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return BitmapFrame.Create(stream);
            }        
        }

        public ImageSource GetIconByPath(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var ext = System.IO.Path.GetExtension(path);

                if (_iconDictionary.ContainsKey(ext))
                {
                    return _iconDictionary[ext];
                }
                else if (!_ignoreSet.Contains(ext))
                {
                    _iconDictionary.Add(ext, GetIcon(path));
                    return _iconDictionary[ext];
                }
                else 
                {
                    return GetIcon(path);
                }
            }
            else
            {
                return null;
            }
        }


    }
}
