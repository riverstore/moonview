using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

namespace MoonView.FileSystem
{
    public class BaseDirectoryInfo : IDirectoryInfo
    {
        //string _fullPath; //Full directory path
        DirectoryInfo _dirInfo;
        //bool _showHidden = false;

        /// <summary>
        /// Directory name
        /// </summary>
        public string Name
        {
            get { return _dirInfo.Name; }
        }

        /// <summary>
        /// Full path of the directory
        /// </summary>
        public string FullPath
        {
            get { return _dirInfo.FullName; }
        }

        /// <summary>
        /// Get last modified time
        /// </summary>
        public DateTime LastModifiedTime {
            get { return _dirInfo.LastWriteTime; } 
        } 

        /// <summary>
        /// Parent directory info
        /// </summary>
        public IDirectoryInfo Parent
        {
            get {
                //return new BaseDirectoryInfo(_dirInfo.Parent); //Does not provide root fs
                char seperatorChar = System.IO.Path.DirectorySeparatorChar;
                string path = _dirInfo.FullName;
                if (path.LastIndexOf(seperatorChar) == path.Length - 1)
                    path = path.Substring(0, path.LastIndexOf(seperatorChar) - 1);                
                if (path.LastIndexOf(seperatorChar) < 0)
                    return null;
                int endIndex = path.LastIndexOf(seperatorChar);
                IDirectoryInfo dirInfo = new BaseDirectoryInfo(path.Substring(0, endIndex) + seperatorChar);
                return dirInfo;
            }
        }

        /// <summary>
        /// Get directories within this directory
        /// </summary>
        public IDirectoryInfo[] Directories
        {
            get { 
                //Retrieve directories
                List<IDirectoryInfo> dirInfoList = new List<IDirectoryInfo>();
                foreach (DirectoryInfo tempDirInfo in _dirInfo.GetDirectories())
                {
                    if ((tempDirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden &&
                        (tempDirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        continue;
                    dirInfoList.Add(new BaseDirectoryInfo(tempDirInfo));
                }
                return dirInfoList.ToArray();
            }
        }

        /// <summary>
        /// Get files within this directory
        /// </summary>
        public IFileInfo[] Files
        {
            get {
                List<IFileInfo> fileInfoList = new List<IFileInfo>();
                foreach (FileInfo tempFileInfo in _dirInfo.GetFiles())
                {
                    if ((tempFileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden &&
                        (tempFileInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        continue;
                    fileInfoList.Add(new BaseFileInfo(tempFileInfo));
                }
                return fileInfoList.ToArray();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fullPath"></param>
        public BaseDirectoryInfo(string fullPath)
        {
            _dirInfo = new DirectoryInfo(fullPath);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dirInfo"></param>
        public BaseDirectoryInfo(DirectoryInfo dirInfo)
        {
            _dirInfo = dirInfo;
        }
        
        public int CompareTo(IDirectoryInfo dirInfo) 
	    {
	    	return Name.CompareTo(dirInfo.Name);
        }
    }
}
