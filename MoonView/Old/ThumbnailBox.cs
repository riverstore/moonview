using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using SdlDotNet.Graphics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

using System.Diagnostics;

using MoonView.Path;

namespace MoonView.Thumbnail
{
    public class ThumbnailBox : PictureBox
    {

        bool _loading = false;
        bool _cancel = false;
        Thumbnail _thumbnail;
        Size _boxSize = new Size(200, 200);
        VScrollBar _vScrollBar;
        object _showDirLock = new object();

        public ThumbnailBox()
            : base()
        {
            _thumbnail = new Thumbnail(this);
        }

        public void Resize(Size size)
        {
            this.Size = size;
            if (!_loading)
                UpdateImage(false);
        }


        #region ShowDirectory
        public void ShowDirectory(IDirectoryInfo dirInfo)
        {
            if (_loading) //Cancel loading for new request
                _cancel = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ShowImageThread), dirInfo);
        }

        /// <summary>
        /// Load images in background thread
        /// </summary>
        /// <param name="dirInfoObj"></param>
        private void ShowImageThread(object dirInfoObj)
        {
            IDirectoryInfo dirInfo = (IDirectoryInfo)dirInfoObj;

            lock (_showDirLock)
            {
                _loading = true; //Start loading
                //Create Grid
                foreach (IFileInfo fileInfo in dirInfo.Files)
                {
                    if (fileInfo.Extension != ".jpg")
                        continue;
                    ImageBox imgBox = new ImageBox(fileInfo, _boxSize);
                    _thumbnail.Add(imgBox);
                }
                //Show Grid
                UpdateImage(false);                
                //Load image
                foreach (ImageBox box in _thumbnail.Sprites)
                {
                    if (_cancel)
                        break;
                    box.LoadImage();
                    Image oldImage = this.Image;
                    UpdateImage(true);
                    oldImage.Dispose(); //Clean old image memory
                }
                _loading = false; //End loading
                _cancel = false; //Reset cancel flag
            }
        }

        private void UpdateImage(bool forceRender)
        {
            Surface tempSurface = _thumbnail.Render(new Rectangle(new Point(0, 0), this.Size), forceRender);
            this.Image = SurfaceToImage(tempSurface);
            tempSurface.Dispose(); //Clean old surface memory
            Debug.WriteLine(_thumbnail.Size.Height);
            SetHeight(_thumbnail.Size.Height); //Set height
        }
        #endregion

        delegate void SetIntCallback(int value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetHeight(int value)
        {
            if (value == 0)
                return;

            if (this.InvokeRequired)
            {
                SetIntCallback d = new SetIntCallback(SetHeight);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this.Height = value;
            }
        }


        private Image SurfaceToImage(Surface surface)
        {
            if (surface == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                surface.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                return Image.FromStream(ms);
            }
        }
    }
}
