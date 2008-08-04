using System;
using System.Collections.Generic;
using System.Text;

namespace MoonView.FileSystem
{
	public interface IDirectoryInfo : IComparable<IDirectoryInfo>, IFSInfo
    {
        /// <summary>
        /// Parent directory info
        /// </summary>
        IDirectoryInfo Parent { get; }

        /// <summary>
        /// Get directories within this directory
        /// </summary>
        IDirectoryInfo[] Directories { get; } 

        /// <summary>
        /// Get files within this directory
        /// </summary>
        IFileInfo[] Files { get; }    
    }
}
