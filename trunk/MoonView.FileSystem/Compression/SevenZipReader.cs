using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SysPath = System.IO.Path;

using SevenZip;
using MoonView.FileSystem;

namespace MoonView.FileSystem.Compression
{
    /// <summary>
    /// Multiple access to Unrar class may create memory corruption with unrar.dll (Need more research)
    /// This class only allows single instance to access Unrar class, which eliminate the memory corruption exception
    /// </summary>
    public class SevenZipReader
    {
        static object _readLock = new object();

        public static byte[] GetBytes(string sevenZipPath, FileItem fileItem)
        {
            lock (_readLock)
            {
                //Create temporary directory
                string tempDir = SysPath.Combine(Environment.CurrentDirectory, "Temp");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);

                //TODO Will create problems if file name from two different archive are the same
                //Return file if already extracted
                string tempFile = SysPath.Combine(tempDir, fileItem.Name.Replace("\0", ""));
                if (File.Exists(tempFile))
                    return File.ReadAllBytes(tempFile);

                //Extract file
                using (FileStream inStream = File.OpenRead(sevenZipPath))
                {
                    ArchiveDatabaseEx archivedatabaseex;
                    new SzIn().szArchiveOpen(inStream, out archivedatabaseex);

                    uint fileIndex = 0;
                    foreach (FileItem item in archivedatabaseex.Database.Files)
                    {
                        if (item.Name.Equals(fileItem.Name))
                            break;
                        fileIndex = fileIndex + 1;
                    }
                    if (fileIndex == archivedatabaseex.Database.Files.Length)
                        throw new Exception("File is not found within files list");

                    uint folderIndex = archivedatabaseex.FileIndexToFolderIndexMap[fileIndex];

                    Dictionary<string, byte[]> outDict = SzExtract.ExtractFolder(inStream, archivedatabaseex, folderIndex);
                    foreach (string fileName in outDict.Keys)
                    {
                        string outPath = SysPath.Combine(tempDir, fileName);
                        string dirPath = SysPath.GetDirectoryName(outPath);
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);                      
                        File.WriteAllBytes(outPath, outDict[fileName]);
                    }
                }
                return File.ReadAllBytes(tempFile);
            }
        }
    }
}
