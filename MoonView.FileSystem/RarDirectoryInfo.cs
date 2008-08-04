using System;
using System.Collections.Generic;
using System.Text;

using MoonView.FileSystem.Compression;
using ICSharpCode.SharpZipLib.Zip;

namespace MoonView.FileSystem
{
	using Unrar = Schematrix.Unrar;
	
	public class RarDirectoryInfo : IDirectoryInfo
    {
		//Value
		string _rarPath;
		IDirectoryInfo _parentDir;
        DateTime _lastModifiedTime;
		
		//Collection
		List<IFileInfo> _fileInfoList = new List<IFileInfo>();
		List<IDirectoryInfo> _dirInfoList = new List<IDirectoryInfo>();
		
        /// <summary>
        /// Directory name
        /// </summary>
        public string Name { 
        	get { return System.IO.Path.GetFileName(_rarPath); }
        }

        /// <summary>
        /// Full path of the directory
        /// </summary>
        public string FullPath { 
        	get { return _rarPath; }
        }

        /// <summary>
        /// Parent directory info
        /// </summary>
        public IDirectoryInfo Parent { 
        	get { return _parentDir; }
        }

        /// <summary>
        /// Get directories within this directory
        /// </summary>
        public IDirectoryInfo[] Directories { 
        	get { return _dirInfoList.ToArray(); }
        }

        /// <summary>
        /// Get files within this directory
        /// </summary>
        public IFileInfo[] Files { 
        	get { return _fileInfoList.ToArray(); }
        }

        /// <summary>
        /// Last modified time
        /// </summary>
        public DateTime LastModifiedTime
        {
            get { return _lastModifiedTime; }
        } 

        public RarDirectoryInfo(BaseFileInfo fileInfo)
        {
        	_rarPath = fileInfo.FullPath;
        	_parentDir = fileInfo.Directory;
            _lastModifiedTime = fileInfo.LastModifiedTime;

            foreach (Schematrix.RARFileInfo rarFInfo in UnrarReader.GetFiles(_rarPath))
                _fileInfoList.Add(new RarFileInfo(this, _rarPath, rarFInfo));

        	//TODO Directory support
        }
        
        public int CompareTo(IDirectoryInfo dirInfo) 
	    {
	    	return Name.CompareTo(dirInfo.Name);
        }        
    }
}
