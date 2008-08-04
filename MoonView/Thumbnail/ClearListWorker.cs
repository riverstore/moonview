using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MoonView.Thumbnail
{
    public delegate void ClearListCompleted(ThumbnailView thumbnailView, ThumbnailWorkerState state);

    public class ClearListWorker
    {
        event ClearListCompleted _clearListCompleted;
        ThumbnailView _thumbnailView = null;
        ThumbnailWorkerState _tnvState = null;
        BackgroundWorker _bgWorker;

        public event ClearListCompleted OnCompleted
        {
            add { _clearListCompleted += value; }
            remove { _clearListCompleted -= value; }
        }

        public ClearListWorker()
        {
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += new DoWorkEventHandler(_bgWorker_DoWork);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);
        }

        public void Run(ThumbnailView thumbnailView, ThumbnailWorkerState state)
        {


            foreach (ClearListCompleted instance in _clearListCompleted.GetInvocationList())
                instance(_thumbnailView, _tnvState);

            //_thumbnailView = thumbnailView;
            //_tnvState = state;
            //_bgWorker.RunWorkerAsync();
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Do nothing
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            //Clear list
            _thumbnailView.BeginUpdate();
            _thumbnailView.Items.Clear();
            _thumbnailView.LargeImageList.Images.Clear();
            _thumbnailView.SmallImageList.Images.Clear();
            _thumbnailView.EndUpdate();
            //Call completed delegates
            foreach (ClearListCompleted instance in _clearListCompleted.GetInvocationList())
                instance(_thumbnailView, _tnvState);
        }
    }
}
