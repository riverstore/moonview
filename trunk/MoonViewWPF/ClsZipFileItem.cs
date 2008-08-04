/// Copyright (C) 2008 Matthew Ng
/// 
/// This program is free software; you can redistribute it and/or
/// modify it under the terms of the GNU General Public License
/// as published by the Free Software Foundation; either version 2
/// of the License, or (at your option) any later version.
/// 
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with this program; if not, write to the Free Software
/// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Media.Imaging;

namespace MoonView
{
    public class ClsZipFileItem : ClsFileItem
    {
        public string _zipPath;
        public ZipEntry _entry;

        public override string FileName
        {
            get { return _entry.Name; }
        }

        public override string Extension
        {
            get { return System.IO.Path.GetExtension(_entry.Name).Replace(".", "").ToUpper(); }
        }

        public override Stream Stream
        {
            get
            {
                MemoryStream ms = null;
                using (ZipInputStream zs = new ZipInputStream(File.OpenRead(_zipPath)))
                {
                    ZipEntry entry;
                    while ((entry = zs.GetNextEntry()) != null)
                    {
                        if (!entry.IsFile || entry.Name != _entry.Name)
                            continue;

                        ms = new MemoryStream((int)entry.Size);
                        byte[] buffer = new byte[8192];
                        while (true)
                        {
                            int size = zs.Read(buffer, 0, buffer.Length);
                            if (size > 0)
                                ms.Write(buffer, 0, size);
                            else
                                break;
                        }
                        ms.Position = 0;
                    }
                }
                if (ms == null)
                    ms = new MemoryStream(0);
                return ms;
            }
        }

        public ClsZipFileItem(string zipPath, ZipEntry entry)
            : base(entry.Name, entry.DateTime)
        {
            _path = entry.Name;
            _entry = entry;
            _zipPath = zipPath;
        }
    }
}
