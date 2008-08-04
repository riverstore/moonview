using System;
using System.Collections.Generic;
using System.Text;

namespace MoonView.FileSystem
{
    public interface IFSInfo
    {
        /// <summary>
        /// Directory name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Full path of the directory
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// Get last modified time
        /// </summary>
        DateTime LastModifiedTime { get; }
    }
}
