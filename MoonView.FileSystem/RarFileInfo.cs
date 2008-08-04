using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MoonView.FileSystem.Compression;
using ICSharpCode.SharpZipLib.Zip;

namespace MoonView.FileSystem
{
	using Unrar = Schematrix.Unrar;
	
	public class RarFileInfo : IFileInfo
    {
		//Value
		string _rarPath;
		string _filePath;
        long _size;
		IDirectoryInfo _parentDir;
        DateTime _lastModifiedTime;
		
    	/// <summary>
        /// Get the directory that holds the file
        /// </summary>
        public IDirectoryInfo Directory { 
        	get { return _parentDir; }
        }

        /// <summary>
        /// Get file name
        /// </summary>
        public string Name { 
        	get { return System.IO.Path.GetFileName(_filePath); }
        }

        /// <summary>
        /// Get full file path
        /// </summary>
        public string FullPath {
        	get { return _filePath; }
        }

        /// <summary>
        /// Get extension of the file
        /// </summary>
        public string Extension { 
        	get { return  System.IO.Path.GetExtension(_filePath); }
        }

        /// <summary>
        /// Get last modified time
        /// </summary>
        public DateTime LastModifiedTime
        {
            get { return _lastModifiedTime; }
        }

        /// <summary>
        /// Get file size in bytes
        /// </summary>
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
            byte[] buffer = UnrarReader.GetBytes(_rarPath, _filePath);
            if (buffer.Length > 0)
                return buffer;
            //TODO Should return empty byte array
            //lock (Utility.EmptyIconLarge)
            //    return Utility.BitmapToByteArray(Utility.EmptyIconLarge);
            return null;
        }
        
        public RarFileInfo(IDirectoryInfo parentDir, string rarPath, Schematrix.RARFileInfo rarFileInfo)
        {
        	_parentDir = parentDir;
        	_rarPath = rarPath; //The rar archive file path
            _lastModifiedTime = rarFileInfo.FileTime;
            _size = rarFileInfo.UnpackedSize;
        	_filePath = rarFileInfo.FileName; //Path of the file within the rar archive
        }
        
	    public int CompareTo(IFileInfo fileInfo) 
	    {
	    	return Name.CompareTo(fileInfo.Name);
        }  
    }
}
