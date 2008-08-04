/// Copyright (C) 2008 Matthew Ng
/// 
/// This program is free software; you can redistribute it and/or
/// modify it under the terms of the GNU General Public License
/// as published by the Free Software Foundation; either version 2
/// of the License, or (at your option) any later version.
/// 
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with this program; if not, write to the Free Software
/// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MoonView
{
    public static class Utility
    {
        #region Images
        //TODO Use hashtable for image
        static string SMALL_ICON_DIRECTORY = @"Icons\Vista-Inspirate_1.0\32x32";
        static string LARGE_ICON_DIRECTORY = @"Icons\Vista-Inspirate_1.0\96x96";
        static Dictionary<string, Bitmap> _smallMimeIconDict = new Dictionary<string, Bitmap>();
        static Dictionary<string, Bitmap> _largeMimeIconDict = new Dictionary<string, Bitmap>();

        public static Bitmap DiskIconSmall;
        //public static Bitmap FileIconSmall;
        public static Bitmap FolderIconSmall;
        public static Bitmap FolderIconLarge;
        //public static Bitmap ArchiveIconSmall;
        public static Bitmap EmptyIconLarge;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Bitmap SmallMimeIcon(string name)
        {
            name = name.Replace(".", "");
            if (_smallMimeIconDict.ContainsKey(name))
                return _smallMimeIconDict[name];
            return EmptyIconLarge;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Bitmap LargeMimeIcon(string name)
        {
            name = name.Replace(".", "");
            if (_largeMimeIconDict.ContainsKey(name))
                return _largeMimeIconDict[name];
            return EmptyIconLarge;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Initialise()
        {
            //TODO Need to cleanup icon image loading

            string exePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            SMALL_ICON_DIRECTORY = System.IO.Path.Combine(exePath, @"Icons\Vista-Inspirate_1.0\32x32");
            LARGE_ICON_DIRECTORY = System.IO.Path.Combine(exePath, @"Icons\Vista-Inspirate_1.0\96x96");

            FolderIconSmall = new Bitmap(System.IO.Path.Combine(exePath, @"Icons\Vista-Inspirate_1.0\32x32\filesystems\folder.png"));
            FolderIconLarge = new Bitmap(System.IO.Path.Combine(exePath, @"Icons\Vista-Inspirate_1.0\96x96\filesystems\folder.png"));
            DiskIconSmall = new Bitmap(System.IO.Path.Combine(exePath, @"Icons\Vista-Inspirate_1.0\32x32\devices\hdd_mount.png"));
            EmptyIconLarge = new Bitmap(System.IO.Path.Combine(exePath, @"Icons\Vista-Inspirate_1.0\96x96\mimetypes\empty.png"));

            //Image
            //LoadMimeIcons("jpg", @"mimetypes\jpg.png");
            //LoadMimeIcons("jpeg", @"mimetypes\jpeg.png");
            //LoadMimeIcons("bmp", @"mimetypes\bmp.png");
            //LoadMimeIcons("png", @"mimetypes\png.png");

            //Archive
            LoadMimeIcons("rar", @"mimetypes\rar.png");
            LoadMimeIcons("zip", @"mimetypes\zip.png");
        }

        private static void LoadMimeIcons(string extension, string filePath)
        {
            Bitmap smallIcon = new Bitmap(System.IO.Path.Combine(SMALL_ICON_DIRECTORY, filePath));
            Bitmap largeIcon = new Bitmap(System.IO.Path.Combine(LARGE_ICON_DIRECTORY, filePath));
            if (!_smallMimeIconDict.ContainsKey(extension))
                _smallMimeIconDict.Add(extension, smallIcon);
            if (!_largeMimeIconDict.ContainsKey(extension))
                _largeMimeIconDict.Add(extension, largeIcon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsSupported(string extension)
        {
            //TODO use sdldotnet checking
            switch (extension.Replace(".","").ToLower())
            {
                case "bmp":
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsArchive(string extension)
        {
            switch (extension.Replace(".","").ToLower())
            {
                case "zip":
                case "rar":
                //case "7z":
                    return true;
                default:
                    return false;
            }
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        private static Regex _reXp = new Regex(@"([\w\W]+?)([\d]+)([a-zA-Z\W]*?)\.([\w]+)", RegexOptions.IgnoreCase);
        public static int CompareStringXp(string strX, string strY)
        {
            strX = strX.Replace(" ", ""); //Remove space
            strY = strY.Replace(" ", ""); //Remove space
            Match matchX = _reXp.Match(strX);
            Match matchY = _reXp.Match(strY);

            if (!matchX.Success || !matchY.Success)
                return strX.CompareTo(strY);

            if (!matchX.Groups[1].Value.Equals(matchY.Groups[1].Value))
                return strX.CompareTo(strY);

            //Avoid problem with number is too large int.MaxValue = 2,147,483,647
            if (matchX.Groups[2].Length > 9 || matchY.Groups[2].Length > 9)
                return strX.CompareTo(strY);

            return int.Parse(matchX.Groups[2].Value) - int.Parse(matchY.Groups[2].Value);
        }
        #endregion

        #region Config

        static string CONFIG_FILE = "moonview.xml";
        static Config _config;

        public static Config Config
        {
            get { return _config; }
        }

        public static void LoadConfig()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                using (FileStream fs = File.OpenRead(CONFIG_FILE))
                    _config = (Config)xs.Deserialize(fs);
            }
            catch (Exception) //ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                _config = new Config();
            }
        }

        public static void SaveConfig()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                File.Delete(CONFIG_FILE);
                using (FileStream fs = File.OpenWrite(CONFIG_FILE))
                    xs.Serialize(fs, _config);
            }
            catch (Exception)// ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
        #endregion
    }
}
