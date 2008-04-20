using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

using MoonView.Path;

namespace MoonView.Thumbnail
{
    public delegate void LoadImageCompleted();

    public class LoadImageWorker
    {
        //Events
        event LoadImageCompleted _loadImageCompleted;

        //Threading
        bool _updating = false;
        object _updateLock = new object();

        //Values
        const int BORDER_WIDTH = 1;
        const int MAX_CONCURRENT_LOADING = 2;

        //Object
        ThumbnailView _thumbnailView;
        ThumbnailWorkerState _tnvState;
        BackgroundWorker _bgWorker;
        Queue<ThumbnailMetaItem> _metaItemQueue;
        Queue<ThumbnailMetaItem> _loadedMetaItemQueue;
        System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        public event LoadImageCompleted OnCompleted
        {
            add { _loadImageCompleted += value; }
            remove { _loadImageCompleted -= value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public LoadImageWorker()
        {
            _bgWorker = new BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.DoWork += new DoWorkEventHandler(_bgWorker_DoWork);
            _bgWorker.ProgressChanged += new ProgressChangedEventHandler(_bgWorker_ProgressChanged);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);

            _timer.Interval = 100;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thumbnailView"></param>
        /// <param name="state"></param>
        /// <param name="metaItemQueue"></param>
        public void Run(ThumbnailView thumbnailView, ThumbnailWorkerState state, Queue<ThumbnailMetaItem> metaItemQueue)
        {
            _thumbnailView = thumbnailView;
            _tnvState = state;
            _metaItemQueue = metaItemQueue;
            _loadedMetaItemQueue = new Queue<ThumbnailMetaItem>();
            _bgWorker.RunWorkerAsync();

            _timer.Start();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            UpdateImage(5);   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Thread> threadList = new List<Thread>();
            for (int i = 0; i < MAX_CONCURRENT_LOADING; i++)
            {
                Thread tempThread = new Thread(new ThreadStart(LoadImage));
                tempThread.Start();
                threadList.Add(tempThread);
            }

            //Wait until all images loaded
            foreach (Thread thread in threadList)
                thread.Join();

            while (!_tnvState.Cancel)
            {
                lock (_loadedMetaItemQueue)
                {
                    if (_loadedMetaItemQueue.Count == 0)
                        break;
                }
                Thread.Sleep(250);
            }
            _timer.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        void LoadImage()
        {
            //Load images
            while (true)
            {
                if (_tnvState.Cancel)
                    return;
                //
                ThumbnailMetaItem metaItem;
                lock (_metaItemQueue)
                {
                    if (_metaItemQueue.Count == 0)
                        return;
                    metaItem = _metaItemQueue.Dequeue();
                }
                //Load directory image 
                if (metaItem.FsInfo is IDirectoryInfo)
                {
                    metaItem.LargeIcon = Utility.FolderIconLarge;
                    metaItem.SmallIcon = Utility.FolderIconSmall;
                }
                //Load file image
                if (metaItem.FsInfo is IFileInfo)
                {
                    IFileInfo fileInfo = (IFileInfo)metaItem.FsInfo;
                    if (Utility.IsSupported(fileInfo.Extension))
                    {
                        byte[] imgBytes = fileInfo.ReadAllBytes();
                        metaItem.LargeIcon = BytesToThumbnailBitmap(imgBytes, _thumbnailView.LargeImageList.ImageSize);
                        metaItem.SmallIcon = BytesToThumbnailBitmap(imgBytes, _thumbnailView.SmallImageList.ImageSize);
                    }
                    else
                    {
                        metaItem.LargeIcon = Utility.LargeMimeIcon(fileInfo.Extension);
                        metaItem.SmallIcon = Utility.SmallMimeIcon(fileInfo.Extension);
                    }
                }
                lock (_loadedMetaItemQueue)
                    _loadedMetaItemQueue.Enqueue(metaItem);
                //lock (_updateLock)
                //{
                //    if (_updating)
                //        continue;
                //    _updating = true;
                //    _bgWorker.ReportProgress(0);
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //UpdateImage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (!_tnvState.Cancel)
            //    UpdateImage();
            foreach (LoadImageCompleted instance in _loadImageCompleted.GetInvocationList())
                instance();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateImage(int limit)
        {
            Queue<ThumbnailMetaItem> tempQueue = new Queue<ThumbnailMetaItem>();
            lock (_loadedMetaItemQueue)
            {
                int i = 0;
                while (_loadedMetaItemQueue.Count > 0)
                {
                    if (i > limit)
                        break;
                    tempQueue.Enqueue(_loadedMetaItemQueue.Dequeue());
                    i++;
                }
            }

            //_thumbnailView.BeginUpdate();
            while (tempQueue.Count > 0)
            {
                if (_tnvState.Cancel)
                    break;
                ThumbnailMetaItem metaItem = tempQueue.Dequeue();
                metaItem.ListViewItem.ImageIndex = _thumbnailView.LargeImageList.Images.Count;
                _thumbnailView.LargeImageList.Images.Add(metaItem.LargeIcon);
                _thumbnailView.SmallImageList.Images.Add(metaItem.SmallIcon);
            }
            //_thumbnailView.EndUpdate();
            //_thumbnailView.Refresh();            
            
            _updating = false;
            //Report progress
            //int index = _imageList.Images.Count;
            //int value = (int)(((double)index) / _imageList.Images.Count * 100);
            //_parent.ShowProgress(value);
        }

        private Bitmap BytesToThumbnailBitmap(byte[] bitmapBytes, Size imageSize)
        {            
            Bitmap newBitmap = new Bitmap(imageSize.Width, imageSize.Height);
            using (Graphics g = Graphics.FromImage((Image)newBitmap))
            {
                try
                {
                    Bitmap bitmap;
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bitmapBytes))
                        bitmap = new Bitmap(ms);
                    
                    double scale = Ratio.CalculateScale(bitmap.Size, newBitmap.Size, Ratio.RatioType.FitImage);
                    Size size = new Size((int)(bitmap.Size.Width * scale), (int)(bitmap.Size.Height * scale));

                    //Smoothing mode
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    //Draw image
                    g.DrawImage(bitmap, new Rectangle((imageSize.Width - size.Width) / 2, (imageSize.Height - size.Height) / 2, size.Width, size.Height));

                    //Add border
                    g.DrawRectangle(new Pen(Color.DarkGray, BORDER_WIDTH),
                                    new Rectangle(0, 0, imageSize.Width - 1, imageSize.Height - 1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return newBitmap;
        }
    }
}
