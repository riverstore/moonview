using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Drawing;

using SdlDotNet.Input;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;

using SdlDotNet.Windows;
using System.Diagnostics;

namespace MoonView.Thumbnail
{
    /// <summary>
    /// Handle graphical rendering via SDL
    /// </summary>
    public class Thumbnail
    {
        SurfaceControl _parent;
        Surface _surface;
        Size _size;
        Size _boxSize = new Size(200, 200);
        SpriteCollection _spriteCln = new SpriteCollection();

        /// <summary>
        /// Size of image box
        /// </summary>
        public Size BoxSize
        {
            get { return _boxSize; }
        }

        public Size Size
        {
            get { return _size; }
        }

        public Surface Surface
        {
            get { return _surface; }
        }

        public Sprite[] Sprites
        {
            get
            {
                Sprite[] spriteArray = new Sprite[_spriteCln.Count];
                _spriteCln.CopyTo(spriteArray, 0);
                return spriteArray;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        public Thumbnail(SurfaceControl parent)
        {
            _parent = parent;
        }

        public void Clear()
        {
            lock (_spriteCln)
                _spriteCln.Clear();
        }

        public void Add(ImageBox imageBox)
        {
            lock (_spriteCln)
                _spriteCln.Add(imageBox);
        }

        public void Remove(ImageBox imageBox)
        {
            lock (_spriteCln)
                _spriteCln.Remove(imageBox);
        }

        /// <summary>
        /// Rearrange the image box 
        /// </summary>
        private bool Rearrange()
        {
            bool redraw = false;
            int width = _parent.Width;

            lock (_spriteCln)
            {
                int colPerRow = (int)Math.Floor(((double)width) / _boxSize.Width);
                if (colPerRow == 0)
                    colPerRow = 1;

                int row = 0;
                int col = 0;
                for (int i = 0; i < _spriteCln.Count; i++)
                {
                    Sprite imageBox = _spriteCln[i];
                    imageBox.Position = new Point(col * _boxSize.Width, row * _boxSize.Height); //Set new position

                    col++;
                    if (col >= colPerRow)
                    {
                        col = 0;
                        row++;
                    }
                }

                width = colPerRow * _boxSize.Width;
                int height = (col == 0 && row > 0 ? row - 1 : row) * _boxSize.Height;

                if (_size.Width != width || _size.Height != height)
                {
                    redraw = true; //need to redraw
                    _size.Width = width;
                    _size.Height = height;
                }
            }
            return redraw;
        }

        public Surface Render(Rectangle rect)
        {
            return Render(rect, false);
        }

        //public Surface UpdateImage(ImageBox imageBox)
        //{
        //    //_surface.Update(
        //}

        /// <summary>
        /// Render surface
        /// </summary>
        /// <returns></returns>
        public Surface Render(Rectangle rect, bool forceRender)
        {
            //Create temporary surface
            Surface tempSurface = new Surface(rect.Width, _size.Height);
            tempSurface.Fill(Color.White);

            //Render surface
            if (Rearrange() || _surface == null || forceRender) //Check is redraw necessary
            {
                lock (_spriteCln)
                {
                    Surface oldSurface = _surface;
                    _surface = new Surface(_size);
                    if (oldSurface != null)
                        oldSurface.Dispose();   //Clean up old surface memory
                    _surface.Fill(Color.White);
                    Collection<Rectangle> rects = _surface.Blit(_spriteCln);
                    _surface.Update(rects);
                }
            }

            //Blit surface to temporary surface
            tempSurface.Blit(_surface);
            tempSurface.Fill(new Rectangle(_size.Width, 0, rect.Width - _size.Width, _size.Height)
                , Color.White); //Fill the gap with white

            Debug.WriteLine("Render size: " + tempSurface.Width + "," + tempSurface.Height);
            return tempSurface;
        }
    }
}
