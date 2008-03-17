using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Input;

namespace MoonView
{
    public class SdlImageBox : Sprite
    {
        Color _bgColor = Color.White;
        int _border = 1;
        Surface _image;
        bool _selected = false;

        public Color BgColor
        {
            get { return _bgColor; }
            set
            {
                if (_bgColor == value)
                    return;
                //_bgColor == value;
                //Redraw image
                Redraw();
            }
        }

        public int Border
        {
            get { return _border; }
            set
            {
                if (_border == value)
                    return;
                _border = value;
                //Redraw image
                Redraw();
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected == value)
                    return;
                _selected = value;
                //Redraw image
                Redraw();
            }
        }

        //Cannot override
        //public override Size Size
        //{
        //    get { return base.Size; }
        //    set
        //    {
        //        if (base.Size.Height == value.Height && base.Size.Width == value.Width)
        //            return;
        //        base.Size = value;
        //        //Redraw image
        //        Redraw();
        //    }
        //}

        public Surface Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public SdlImageBox(byte[] byteArray)
            : base()
        {
            _image = new Surface(byteArray);
            //base.Size = new Size(250, 270);
        }

        public SdlImageBox(Bitmap bitmap)
            : base()
        {
            _image = new Surface(bitmap);
            //base.Size = new Size(250, 270);
        }

        public void Redraw()
        {
            //this.Clear();

            //Create background
            //Surface bg = new Surface(_size);
            //bg.Fill(_bgColor);
            //Resize image

            //
        }

        public override void Update(MouseButtonEventArgs args)
        {

        }
    }
}
