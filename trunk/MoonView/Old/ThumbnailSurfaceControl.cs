using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.IO;
using SdlDotNet.Windows;
using SdlDotNet.Graphics;

using System.Threading;
using System.Diagnostics;

using MoonView.Path;
using System.Windows.Forms;

namespace MoonView.Thumbnail
{
    public class ThumbnailSurfaceControl : SurfaceControl
    {
        bool _loading = false;
        bool _cancel = false;
        Thumbnail _thumbnail;
        Size _boxSize = new Size(200, 200);

        Panel _panel;
        VScrollBar _vScrollBar;

        object _showDirLock = new object();

        /// <summary>
        /// Constructor
        /// </summary>        
        public ThumbnailSurfaceControl()
            : base()
        {
            this.DoubleBuffered = true;
            _thumbnail = new Thumbnail(this);
        }

        public void SetControl(VScrollBar vScrollBar, Panel panel)
        {
            _panel = panel;
            _vScrollBar = vScrollBar;
            _vScrollBar.ValueChanged += new EventHandler(_vScrollBar_ValueChanged);
            //Add event after vscrollbar is set
            this.Click += new EventHandler(ThumbnailSurfaceControl_Click);
            this.SizeChanged += new EventHandler(ThumbnailSurfaceControl_SizeChanged);
        }

        void _vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            //if (_loading)
            //    return;
            //throw new NotImplementedException();
            //this.Blit(_thumbnail.Render(new Rectangle(new Point(0, _vScrollBar.Value), this.Size)));
            this.Location = new Point(0, -_vScrollBar.Value);
            Debug.WriteLine("VScrollBar.ValueChanged: " + _vScrollBar.Value);
        }

        #region Events
        void ThumbnailSurfaceControl_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void ThumbnailSurfaceControl_SizeChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            //this.Blit(_thumbnail.Render(new Rectangle(new Point(0, _vScrollBar.Value), this.Size)));
            //if (_thumbnail.Size.Height != _vScrollBar.Maximum)
            //    SetVScrollMaximum(_thumbnail.Size.Height - Size.Height);
            Debug.WriteLine("ThumbnailSurfaceControl_SizeChanged: " + this.Size.ToString());
        }
        #endregion


        public void ShowDirectory(IDirectoryInfo dirInfo)
        {
            if (_loading) //Cancel loading for new request
                _cancel = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ShowImageThread), dirInfo);
        }

        delegate void SetIntCallback(int value);
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

                //Update 
                UpdateImage(false);
                //SetVScrollMaximum(_thumbnail.Size.Height - Size.Height);

                //Load image
                foreach (ImageBox box in _thumbnail.Sprites)
                {
                    if (_cancel)
                        break;
                    box.LoadImage();
                    UpdateImage(true);
                }
                _loading = false; //End loading
                _cancel = false; //Reset cancel flag
            }
        }

        void UpdateImage(bool forceRender)
        {
            Surface tempSurf = _thumbnail.Render(new Rectangle(new Point(0, 0), this.Size), forceRender);
            this.Blit(tempSurf);
            SetHeight(_thumbnail.Size.Height);
            SetVScrollMaximum(_thumbnail.Size.Height - _panel.Height);
        }

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

        private void SetVScrollMaximum(int value)
        {
            if (_vScrollBar.InvokeRequired)
            {
                SetIntCallback d = new SetIntCallback(SetVScrollMaximum);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this._vScrollBar.Maximum = value;
            }
        }
    }
}
