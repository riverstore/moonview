/// Author:  Matthew Ng
/// Licence: GPL v2

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Schematrix;
//using MoonView.Path;

namespace MoonView.Compression
{
    /// <summary>
    /// Multiple access to Unrar class may create memory corruption with unrar.dll (Need more research)
    /// This class only allows single instance to access Unrar class, which eliminate the memory corruption exception
    /// </summary>
    public class ClsUnrarReader
    {
        static object _readLock = new object();

        public static byte[] GetBytes(string rarPath, string filePath)
        {
            lock (_readLock)
            {
                using (ClsUnrar unrar = new ClsUnrar())
                {
                    try
                    {
                        //Unrar file in temporary directory
                        string tempDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
                        unrar.DestinationPath = tempDir;
                        unrar.Open(rarPath, ClsUnrar.OpenMode.Extract);
                        // Get destination from user
                        while (unrar.ReadHeader())
                        {
                            if (unrar.CurrentFile.FileName.Equals(filePath))
                            {
                                unrar.Extract();
                                break;
                            }
                            unrar.Skip();
                        }
                        unrar.Close();
                        // 
                        string tempPath = System.IO.Path.Combine(tempDir, filePath);
                        byte[] buffer = File.ReadAllBytes(tempPath);
                        File.Delete(tempPath); //Remove file
                        return buffer;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                return new byte[0];
            }
        }

        public static RARFileInfo[] GetFiles(string rarPath)
        {
            lock (_readLock)
            {
                List<RARFileInfo> rarFileInfoList = new List<RARFileInfo>();
                using (ClsUnrar unrar = new ClsUnrar())
                {
                    try
                    {
                        unrar.Open(rarPath, ClsUnrar.OpenMode.List);

                        while (unrar.ReadHeader())
                        {
                            if (!unrar.CurrentFile.IsDirectory)
                                rarFileInfoList.Add(unrar.CurrentFile);
                            unrar.Skip();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    return rarFileInfoList.ToArray();
                }
            }
        }
    }
}
