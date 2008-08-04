using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

namespace MoonView.FileSystem
{
    public class ZipDirectoryInfo : IDirectoryInfo
    {
        //Value
        string _zipPath;
        IDirectoryInfo _parentDir;
        DateTime _lastModifiedTime;

        //Collection
        List<IFileInfo> _fileInfos = new List<IFileInfo>();
        List<IDirectoryInfo> _dirInfos = new List<IDirectoryInfo>();

        /// <summary>
        /// Directory name
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileName(_zipPath); }
        }

        /// <summary>
        /// Full path of the directory
        /// </summary>
        public string FullPath
        {
            get { return _zipPath; }
        }

        /// <summary>
        /// Get last modified time
        /// </summary>
        public DateTime LastModifiedTime
        {
            get { return _lastModifiedTime; }
        }

        /// <summary>
        /// Parent directory info
        /// </summary>
        public IDirectoryInfo Parent
        {
            get { return _parentDir; }
        }

        /// <summary>
        /// Get directories within this directory
        /// </summary>
        public IDirectoryInfo[] Directories
        {
            get { return _dirInfos.ToArray(); }
        }

        /// <summary>
        /// Get files within this directory
        /// </summary>
        public IFileInfo[] Files
        {
            get { return _fileInfos.ToArray(); }
        }

        public ZipDirectoryInfo(BaseFileInfo fileInfo)
        {
            //TODO Add directory support
            _zipPath = fileInfo.FullPath;
            _lastModifiedTime = fileInfo.LastModifiedTime;
            _parentDir = fileInfo.Directory;

            using (FileStream fs = File.OpenRead(_zipPath))
            {
                using (ZipInputStream zs = new ZipInputStream(fs))
                {
                    ZipEntry entry;
                    while ((entry = zs.GetNextEntry()) != null)
                    {
                        if (!entry.IsFile)
                            continue;
                        _fileInfos.Add(new ZipFileInfo(this, _zipPath, entry));
                        _fileInfos.Sort();
                    }
                }
            }
        }

        public int CompareTo(IDirectoryInfo dirInfo)
        {
            return Name.CompareTo(dirInfo.Name);
        }
    }
}
