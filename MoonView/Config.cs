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
