using System;
using System.Collections.Generic;
using System.Text;

namespace MoonView.FileSystem
{
	public interface IFileInfo : IComparable<IFileInfo>, IFSInfo
    {
        /// <summary>
        /// Get the directory that holds the file
        /// </summary>
        IDirectoryInfo Directory { get;}

        /// <summary>
        /// Get extension of the file
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Get size of the file in bytes
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Read all bytes of the file
        /// </summary>
        /// <returns></returns>
        byte[] ReadAllBytes();
    }
}
