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
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace MoonView
{
    public class Config
    {
        [XmlElement]
        private int _size_width;
        [XmlElement]
        private int _size_height;
        [XmlElement]
        private int _location_top;
        [XmlElement]
        private int _location_left;
        [XmlElement]
        private string _lastDirectoryPath;

        public int Top
        {
            get { return _location_top; }
            set { _location_top = value; }
        }

        public int Left
        {
            get { return _location_left; }
            set { _location_left = value; }
        }

        public Size Size
        {
            get { return new Size(_size_width, _size_height); }
            set
            {
                _size_width = value.Width;
                _size_height = value.Height;
            }
        }

        public string LastDirectoryPath
        {
            get { return _lastDirectoryPath; }
            set { _lastDirectoryPath = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Config()
        {
            this.Top = 20;
            this.Left = 20;
            this.Size = new Size(640, 480);
            this.LastDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }
    }
}
