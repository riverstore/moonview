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

using ICSharpCode.SharpZipLib.Zip;
using MoonView.Compression;
using Schematrix;
using System.IO;
using System.Collections.ObjectModel;

namespace MoonView
{
    /// <summary>
    /// 
    /// </summary>
    public class ClsFileSystemItemCollection : ObservableCollection<ClsFileSystemItem>
    {
        private string _path;
        private string _parentPath;

        public string ParentPath
        {
            get { return _parentPath; }
        }

        /// <summary>Path</summary>
        public string Path
        {
            get { return _path; }
        }

        public void Update(string path)
        {
            if (Directory.Exists(path))
            {
                AddParentDir(path);
                //Load directories
                foreach (DirectoryInfo di in (new DirectoryInfo(path)).GetDirectories())
                {
                    if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden &&
                        (di.Attributes & FileAttributes.System) == FileAttributes.System)
                        continue;
                    this.Add(new ClsDirectoryItem(di.FullName));
                }
                //Load files
                foreach (FileInfo fi in (new DirectoryInfo(path)).GetFiles())
                {
                    if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden &&
                        (fi.Attributes & FileAttributes.System) == FileAttributes.System)
                        continue;
                    this.Add(new ClsFileItem(fi.FullName));
                }
            }
            else if (System.IO.Path.GetExtension(path).ToLower().Equals(".zip"))
            {
                AddParentDir(path);

                using (FileStream fs = File.OpenRead(path))
                {
                    using (ZipInputStream zs = new ZipInputStream(fs))
                    {
                        ZipEntry entry;
                        while ((entry = zs.GetNextEntry()) != null)
                        {
                            if (!entry.IsFile)
                                continue;
                            this.Add(new ClsZipFileItem(path, entry));
                            //_fileInfos.Add(new ZipFileInfo(this, _zipPath, entry));
                            //_fileInfos.Sort();
                        }
                    }
                }
            }
            else if (System.IO.Path.GetExtension(path).ToLower().Equals(".rar"))
            {
                AddParentDir(path);

                foreach (RARFileInfo rarFInfo in ClsUnrarReader.GetFiles(path))
                    this.Add(new ClsRarFileItem(path, rarFInfo));
                //_fileInfoList.Add(new RarFileInfo(this, _rarPath, rarFInfo));
            }
        }

        private void AddParentDir(string path)
        {
            this.Clear();

            _path = path;

            DirectoryInfo parentDI = (new DirectoryInfo(path)).Parent;
            if (parentDI != null && parentDI.Parent != null)
            {
                _parentPath = parentDI.FullName;
                this.Add(new ClsDirectoryItem("..", _parentPath));
            }
            else
                _parentPath = null;
        }
    }
}
