using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SevenZip;

namespace MoonView.FileSystem
{
	public class SevenZipDirectoryInfo : IDirectoryInfo
    {
		//Value
		string _sevenZipPath;
		IDirectoryInfo _parentDir;
        DateTime _lastModifiedTime;
		
		//Collection
		List<IFileInfo> _fileInfos = new List<IFileInfo>();
		List<IDirectoryInfo> _dirInfos = new List<IDirectoryInfo>();
		
        /// <summary>
        /// Directory name
        /// </summary>
        public string Name { 
        	get { return System.IO.Path.GetFileName(_sevenZipPath); }
        }

        /// <summary>
        /// Full path of the directory
        /// </summary>
        public string FullPath { 
        	get { return _sevenZipPath; }
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
        public IDirectoryInfo Parent { 
        	get { return _parentDir; }
        }

        /// <summary>
        /// Get directories within this directory
        /// </summary>
        public IDirectoryInfo[] Directories { 
        	get { return _dirInfos.ToArray(); }
        }

        /// <summary>
        /// Get files within this directory
        /// </summary>
        public IFileInfo[] Files { 
        	get { return _fileInfos.ToArray(); }
        }
        
        public SevenZipDirectoryInfo(BaseFileInfo fileInfo)
        {
        	//TODO Add directory support
        	_sevenZipPath = fileInfo.FullPath;
            _lastModifiedTime = fileInfo.LastModifiedTime;
        	_parentDir = fileInfo.Directory;

            //Read content table
            using (FileStream fs = File.OpenRead(_sevenZipPath))
            {
                ArchiveDatabaseEx archivedatabaseex;
                new SzIn().szArchiveOpen(fs, out archivedatabaseex);

                for (int i = 0; i < archivedatabaseex.Database.NumFiles; i++)
                {
                    FileItem file = archivedatabaseex.Database.Files[i];
                    if (file.IsDirectory)
                        continue;
                    _fileInfos.Add(new SevenZipFileInfo(this, _sevenZipPath, (uint) i, file));
                }
            }
        }
        
        public int CompareTo(IDirectoryInfo dirInfo) 
	    {
	    	return Name.CompareTo(dirInfo.Name);
        }
    }
}
