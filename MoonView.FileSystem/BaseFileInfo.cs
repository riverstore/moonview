using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MoonView.FileSystem
{
    public class BaseFileInfo : IFileInfo
    {
        FileInfo _fileInfo;

        /// <summary>
        /// Get directory that holds this file
        /// </summary>
        public IDirectoryInfo Directory
        {
            get { return new BaseDirectoryInfo(_fileInfo.Directory); }
        }

        /// <summary>
        /// Get name of this file
        /// </summary>
        public string Name
        {
            get { return _fileInfo.Name; }
        }

        /// <summary>
        /// Get extension of this file
        /// </summary>
        public string Extension
        {
            get { return _fileInfo.Extension; }
        }


        /// <summary>
        /// Get full path of the file
        /// </summary>
        public string FullPath
        {
            get { return _fileInfo.FullName; }
        }

        /// <summary>
        /// Get last modified time
        /// </summary>
        public DateTime LastModifiedTime
        {
            get { return _fileInfo.LastWriteTime; }
        }

        /// <summary>
        /// Get size of the file in bytes
        /// </summary>
        public long Size 
        {
            get { return _fileInfo.Length; }
        }

        public BaseFileInfo(string fullPath)
        {
            _fileInfo = new FileInfo(fullPath);
        }

        public BaseFileInfo(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public byte[] ReadAllBytes()
        {
            return System.IO.File.ReadAllBytes(_fileInfo.FullName);
        }
        
	    public int CompareTo(IFileInfo fileInfo) 
	    {
	    	return Name.CompareTo(fileInfo.Name);
        }
    }
}
