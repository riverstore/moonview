using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

namespace MoonView.FileSystem
{
	public class ZipFileInfo : IFileInfo
    {
		//Value
		string _zipPath;
		string _fullPath;
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
        	get { return System.IO.Path.GetFileName(_fullPath); }
        }

        /// <summary>
        /// Get full file path
        /// </summary>
        public string FullPath {
        	get { return _fullPath; }
        }

        /// <summary>
        /// Get extension of the file
        /// </summary>
        public string Extension { 
        	get { return System.IO.Path.GetExtension(_fullPath); }
        }

        /// <summary>
        /// Get last modified time
        /// </summary>
        public DateTime LastModifiedTime
        {
            get { return _lastModifiedTime; }
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
        	using (ZipInputStream zs = new ZipInputStream(System.IO.File.OpenRead(_zipPath))) 
        	{
				ZipEntry entry;
				while ((entry = zs.GetNextEntry()) != null)
				{
					if (entry.Name != _fullPath || !entry.IsFile)
						continue;
					
					using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
					{
						byte[] buffer = new byte[2048];
						while (true) 
						{
							int size = zs.Read(buffer, 0, buffer.Length);
							if (size > 0)
								ms.Write(buffer, 0, size);
							else
								break;
						}
						ms.Position = 0;
						return ms.ToArray();
					}	
				}
				//TODO need alert for file not found
				return new byte[0];
        	}
        }

        public ZipFileInfo(IDirectoryInfo parentDir, string zipPath, ZipEntry entry)
        {
        	_parentDir = parentDir;
        	_zipPath = zipPath;
        	_fullPath = entry.Name;
            _lastModifiedTime = entry.DateTime;
            _size = entry.Size;
        }
        
	    public int CompareTo(IFileInfo fileInfo) 
	    {
	    	return Name.CompareTo(fileInfo.Name);
        }  
    }
}
