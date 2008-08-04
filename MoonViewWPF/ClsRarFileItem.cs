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

using MoonView.Compression;
using Schematrix;
using System.IO;

namespace MoonView
{
    /// <summary>
    /// 
    /// </summary>
    public class ClsRarFileItem : ClsFileItem
    {
        RARFileInfo _rarFileInfo;

        public override string FileName
        {
            get { return _rarFileInfo.FileName; }
        }

        public override string Extension
        {
            get { return System.IO.Path.GetExtension(_rarFileInfo.FileName).Replace(".", "").ToUpper(); }
        }

        public override Stream Stream
        {
            get
            {
                MemoryStream ms = new MemoryStream(ClsUnrarReader.GetBytes(_path, _rarFileInfo.FileName));
                ms.Position = 0;
                if (ms == null)
                    ms = new MemoryStream(0);
                return ms;
            }
        }

        public ClsRarFileItem(string rarPath, RARFileInfo rarFileInfo)
            : base(rarFileInfo.FileName, rarFileInfo.FileTime)
        {
            _path = rarPath;
            _rarFileInfo = rarFileInfo;
            //_fileName = fileName;
        }
    }
}
