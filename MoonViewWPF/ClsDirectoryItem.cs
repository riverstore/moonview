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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MoonView
{
    public class ClsDirectoryItem : ClsFileSystemItem
    {
        string _dirName;
        List<ClsThumbnailImage> _thumbnailList;

        //Cache list
        List<ClsFileItem> _fileList;
        List<ClsDirectoryItem> _directoryList;

        public List<ClsThumbnailImage> ThumbnailImages
        {
            get {  return _thumbnailList; }
        }

        public string DirectoryName
        {
            get { return _dirName; }
        }

        public ClsDirectoryItem[] GetDirectories()
        {
            //(new DirectoryInfo).GetDirectories()
            if (_directoryList == null)
            {
                _directoryList = new List<ClsDirectoryItem>();
            }
            return _directoryList.ToArray();
        }

        public ClsFileItem[] GetFiles()
        {
            if (_fileList == null)
            {
                _fileList = new List<ClsFileItem>();

            }
            return _fileList.ToArray();
        }

        public ClsDirectoryItem(string path)
            : base(path)
        {
            string dirName;
            try
            {
                dirName = (new DirectoryInfo(path)).Name;
            }
            catch (Exception)
            {
                dirName = path.Substring(path.LastIndexOf(System.IO.Path.PathSeparator));
            }
            Initialise(dirName, path);
        }

        public ClsDirectoryItem(string dirName, string path)
            : base(path)
        {
            Initialise(dirName, path);
        }

        private void Initialise(string dirName, string path)
        {
            _path = path;
            _dirName = dirName;

            //Load thumbnail list
            _thumbnailList = new List<ClsThumbnailImage>();
            foreach (FileInfo fi in (new DirectoryInfo(_path)).GetFiles())
            {
                if (Utility.IsSupported(fi.Extension))
                {
                    _thumbnailList.Add(new ClsThumbnailImage(fi.FullName));
                    if (_thumbnailList.Count >= 4)
                        break;
                }
            }    
        }
    }
}
