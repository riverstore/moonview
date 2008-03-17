﻿//System NS
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MoonView
{
    public static class Utility
    {
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
            LoadMimeIcons("zip",@"mimetypes\zip.png");
        }

        private static void LoadMimeIcons(string extension, string filePath)
        {
            Bitmap smallIcon = new Bitmap(System.IO.Path.Combine(SMALL_ICON_DIRECTORY, filePath));
            Bitmap largeIcon = new Bitmap(System.IO.Path.Combine(LARGE_ICON_DIRECTORY, filePath));
            _smallMimeIconDict.Add(extension, smallIcon);
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
            switch (extension.ToLower())
            {
                case ".bmp":
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
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
            switch (extension.ToLower())
            {
                case ".zip":
                case ".rar":
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
    }
}