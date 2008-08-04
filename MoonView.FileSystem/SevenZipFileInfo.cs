using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SevenZip;

namespace MoonView.FileSystem
{
    public class SevenZipFileInfo : IFileInfo
    {
        //Value
        string _sevenZipPath;
        string _fullPath;
        long _size;
        IDirectoryInfo _parentDir;

        uint _fileIndex;
        FileItem _fileItem;

        /// <summary>
        /// Get the directory that holds the file
        /// </summary>
        public IDirectoryInfo Directory
        {
            get { return _parentDir; }
        }

        /// <summary>
        /// Get file name
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileName(_fullPath); }
        }

        /// <summary>
        /// Get full file path
        /// </summary>
        public string FullPath
        {
            get { return _fullPath; }
        }

        /// <summary>
        /// Get extension of the file
        /// </summary>
        public string Extension
        {
            get { return System.IO.Path.GetExtension(_fullPath); }
        }

        /// <summary>
        /// Get last modified time
        /// </summary>
        public DateTime LastModifiedTime
        {
            get { return DateTime.MinValue; }
        }

        public long Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Read all bytes of the file
        /// </summary>
        /// <returns></returns>
        public byte[] ReadAllBytes()
        {
            return Compression.SevenZipReader.GetBytes(_sevenZipPath, _fileItem);
        }

        public SevenZipFileInfo(IDirectoryInfo parentDir, string sevenZipPath, uint fileIndex, FileItem fileItem)
        {
            _parentDir = parentDir;
            _sevenZipPath = sevenZipPath;
            _fileIndex = fileIndex;
            _fileItem = fileItem;
            _fullPath = fileItem.Name.Replace("\0", "");
            _size = (long)fileItem.Size;
        }

        public int CompareTo(IFileInfo fileInfo)
        {
            return Name.CompareTo(fileInfo.Name);
        }
    }
}
