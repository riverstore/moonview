using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

using MoonView.Path;

namespace MoonView.Thumbnail
{
    public delegate void LoadContentCompleted(ThumbnailView thumbnailView, ThumbnailWorkerState state, Queue<ThumbnailMetaItem> metaItemQueue);

    public class LoadContentWorker
    {
        event LoadContentCompleted _loadEmptyBoxesCompleted;
        ThumbnailView _thumbnailView;
        ThumbnailWorkerState _tnvState;

        bool _updating = false;

        //
        BackgroundWorker _bgWorker;
        System.Windows.Forms.Timer _timer;

        //Collection
        List<ThumbnailMetaItem> _thumbnailItemList = new List<ThumbnailMetaItem>();
        Queue<ThumbnailMetaItem> _metaItemQueue = new Queue<ThumbnailMetaItem>();

        public event LoadContentCompleted OnCompleted
        {
            add { _loadEmptyBoxesCompleted += value; }
            remove { _loadEmptyBoxesCompleted -= value; }
        }

        public LoadContentWorker()
        {
            _bgWorker = new BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.DoWork += new DoWorkEventHandler(_bgWorker_DoWork);
            _bgWorker.ProgressChanged += new ProgressChangedEventHandler(_bgWorker_ProgressChanged);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);

            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _timer_Tick(object sender, EventArgs e)
        {
            LoadContent(5);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thumbnailView"></param>
        /// <param name="state"></param>
        public void Run(ThumbnailView thumbnailView, ThumbnailWorkerState state)
        {
            _thumbnailView = thumbnailView;
            _tnvState = state;

            _timer.Start();
            _bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_tnvState.Cancel) return;

            CreateEmptyBitmap();

            //Show all directory and files (empty box)
            //Image emptyImage = GetThumbnailBitmap(null);

            //Show parent directory
            if (_tnvState.DirectoryInfo.Parent != null)
                this.AddItem("..", "Parent Folder", _tnvState.DirectoryInfo.Parent);

            //Show directories
            foreach (IDirectoryInfo dirInfo in _tnvState.DirectoryInfo.Directories)
            {
                if (_tnvState.Cancel) return;
                this.AddItem(dirInfo);
            }

            //Show files
            foreach (IFileInfo fileInfo in _tnvState.DirectoryInfo.Files)
            {
                if (_tnvState.Cancel) return;
                this.AddItem(fileInfo);
            }

            while (!_tnvState.Cancel)
            {
                lock (_metaItemQueue)
                {
                    if (_metaItemQueue.Count == 0)
                        break;
                }
                System.Threading.Thread.Sleep(250);
            }
            _timer.Stop();
        }

        void LoadContent(int limit)
        {
            List<ThumbnailMetaItem> tempList = new List<ThumbnailMetaItem>();
            lock (_metaItemQueue)
            {
                int i = 0;
                while (_metaItemQueue.Count > 0)
                {
                    if (i > limit)
                        break;
                    tempList.Add(_metaItemQueue.Dequeue());
                    i++;
                }
            }

            foreach (ThumbnailMetaItem metaItem in tempList)
            {
                _thumbnailView.BeginUpdate();
                _thumbnailView.Items.Add(metaItem.ListViewItem);
                if (_thumbnailView.View == View.Details) //Resize columns in Detail view
                    _thumbnailView.ResizeColumns();
                _thumbnailView.EndUpdate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //List<ThumbnailMetaItem> tempList = new List<ThumbnailMetaItem>();
            //lock (_metaItemQueue)
            //{
            //    while (_metaItemQueue.Count > 0)
            //        tempList.Add(_metaItemQueue.Dequeue());
            //}
            //foreach (ThumbnailMetaItem metaItem in tempList)
            //{
            //    _thumbnailView.BeginUpdate();
            //    _thumbnailView.Items.Add(metaItem.ListViewItem);
            //    if (_thumbnailView.View == View.Details) //Resize columns in Detail view
            //        _thumbnailView.ResizeColumns();
            //    _thumbnailView.EndUpdate();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _bgWorker_ProgressChanged(null, null); //Show any left over contents
            _thumbnailView.Sort(); //Sort items

            Queue<ThumbnailMetaItem> metaItemQueue = new Queue<ThumbnailMetaItem>();

            //TODO Add empty box image to index 0 of Large and Small imageList
            foreach (ThumbnailMetaItem metaItem in _thumbnailItemList)
            {
                if (_tnvState.Cancel)
                    break;
                metaItemQueue.Enqueue(metaItem);
            }
            _thumbnailItemList.Clear();

            //Call delegate
            foreach (LoadContentCompleted instance in _loadEmptyBoxesCompleted.GetInvocationList())
                instance(_thumbnailView, _tnvState, metaItemQueue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bitmap"></param>
        /// <param name="value"></param>
        void AddItem(IFSInfo fsInfo)
        {
            ListViewItem lvItem = new ListViewItem(fsInfo.Name, 0);
            if (fsInfo is IFileInfo)
                lvItem.SubItems.Add(((IFileInfo)fsInfo).Size.ToString()); //Size
            else
                lvItem.SubItems.Add(""); //Size
            if (fsInfo is IFileInfo)
                lvItem.SubItems.Add(((IFileInfo)fsInfo).Extension.Length > 0 ? ((IFileInfo)fsInfo).Extension + " File" : "File"); //Type
            else
                lvItem.SubItems.Add("Folder");
            lvItem.SubItems.Add(fsInfo.LastModifiedTime.ToString("G")); //Date

            _tnvState.ListViewDict.Add(lvItem, fsInfo);
            AddItem(new ThumbnailMetaItem(fsInfo, lvItem));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="fsInfo"></param>
        void AddItem(string name, string type, IFSInfo fsInfo)
        {
            ListViewItem lvItem = new ListViewItem(name, 0);
            lvItem.SubItems.Add(""); //Size
            lvItem.SubItems.Add(type); //Type
            lvItem.SubItems.Add(""); //Date

            _tnvState.ListViewDict.Add(lvItem, fsInfo);
            AddItem(new ThumbnailMetaItem(fsInfo, lvItem));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaItem"></param>
        void AddItem(ThumbnailMetaItem metaItem)
        {
            _thumbnailItemList.Add(metaItem);
            lock (_metaItemQueue)
                _metaItemQueue.Enqueue(metaItem);
        }

        /// <summary>
        /// 
        /// </summary>
        void CreateEmptyBitmap()
        {
            Size size = _thumbnailView.LargeImageList.ImageSize;
            Bitmap largeBox = new Bitmap(size.Width, size.Height);
            _thumbnailView.LargeImageList.Images.Add(largeBox);

            size = _thumbnailView.SmallImageList.ImageSize;
            Bitmap smallBox = new Bitmap(size.Width, size.Height);
            _thumbnailView.SmallImageList.Images.Add(smallBox);
        }
    }
}
