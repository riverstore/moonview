//System NS
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

//MoonView NS
using MoonView.Path;
using MoonView.Forms;
using MoonView.Thumbnail;

namespace MoonView
{
    public partial class MoonViewForm : Form
    {
        ViewForm _viewForm;
        Navigation _navigation;

        /// <summary>
        /// Constructor
        /// </summary>
        public MoonViewForm(string[] args)
        {
            InitializeComponent();
            Utility.Initialise();
            this.directoryTreeView1.Initialise(this);
            this.thumbnailView1.Initialise(this);
            _viewForm = new ViewForm();
            _navigation = new Navigation(this, BackButton, ForwardButton, UpButton, RefreshButton);

            if (args.Length < 1)
                Open(this, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            else
                Open(this, args[0]);
        }

        public void Open(object sender, string path)
        {
            if (IsCurrentDirectory(path))
                return;

            bool isDirectory = System.IO.Directory.Exists(path);
            bool isFile = System.IO.File.Exists(path);

            if (!isDirectory && !isFile)
                return;
            if (isDirectory)
            {
                ShowDirectory(sender, new BaseDirectoryInfo(path));
                return;
            }
            if (isFile)
            {
                string ext = System.IO.Path.GetExtension(path);
                if (Utility.IsSupported(ext))
                {
                    _viewForm.ShowImage(new BaseFileInfo(path));
                    if (sender == this)
                        ShowDirectory(this, new BaseDirectoryInfo(System.IO.Path.GetDirectoryName(path)));
                    return;
                }
                if (Utility.IsArchive(ext))
                {
                    BaseDirectoryInfo dirInfo =
                        new BaseDirectoryInfo(System.IO.Path.GetDirectoryName(path));
                    switch (ext)
                    {
                        case ".zip":
                            ZipDirectoryInfo zipDirInfo = new ZipDirectoryInfo(new BaseFileInfo(path));
                            ShowDirectory(sender, zipDirInfo);
                            return;
                        case ".rar":
                            RarDirectoryInfo rarDirInfo = new RarDirectoryInfo(new BaseFileInfo(path));
                            ShowDirectory(sender, rarDirInfo);
                            return;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Show directory
        /// </summary>
        /// <param name="dirInfo"></param>
        public void ShowDirectory(object sender, IDirectoryInfo dirInfo)
        {
            if (dirInfo == null)
                return;
            comboBox1.Text = dirInfo.FullPath;
            this.directoryTreeView1.PopulateNodes(dirInfo);
            this.thumbnailView1.ShowDirectory(dirInfo);
            if (sender != _navigation)
                _navigation.AddDirectoryInfo(dirInfo);
        }

        /// <summary>
        /// Show image in viewForm
        /// </summary>
        /// <param name="fileInfo"></param>
        public void ShowImage(IFileInfo fileInfo)
        {
            _viewForm.ShowImage(fileInfo);
        }

        /// <summary>
        /// Show images within archive file
        /// </summary>
        /// <param name="fileInfo"></param>
        public void ShowArchive(object sender, IFileInfo fileInfo)
        {
            if (IsCurrentDirectory(fileInfo.FullPath))
                return;
            if (!(fileInfo is BaseFileInfo))
                return;
            comboBox1.Text = fileInfo.FullPath;
            switch (fileInfo.Extension)
            {
                case ".zip":
                    ZipDirectoryInfo zipDirInfo = new ZipDirectoryInfo((BaseFileInfo)fileInfo);
                    ShowDirectory(sender, zipDirInfo);
                    break;
                case ".rar":
                    RarDirectoryInfo rarDirInfo = new RarDirectoryInfo((BaseFileInfo)fileInfo);
                    ShowDirectory(sender, rarDirInfo);
                    break;
                default:
                    break;
            }
        }

        public void ShowProgress(int value)
        {
            toolStripProgressBar1.Value = value;
        }

        /// <summary>
        /// Handle main form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoonViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
                _viewForm.Visible = false;

            //Cancel thumbnail loading and wait for cancellation completed
            if (thumbnailView1.IsBusy)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ExitWait)); //Using seperate thread to wait for all work exited
                //TODO Remove temporary files
                e.Cancel = true; //Cancel closing for allowing ExitWait thread run
            }
            else
                Application.Exit();
        }

        /// <summary>
        /// Threadpool method, wait for all background task completed before exit application
        /// </summary>
        /// <param name="stateObj"></param>
        private void ExitWait(object stateObj)
        {
            thumbnailView1.AbortLoading();
            if (thumbnailView1.IsBusy)
                Thread.Sleep(100);
            Application.Exit();
        }



        private void directoryTreeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private bool IsCurrentDirectory(string dirPath)
        {
            if (_navigation.CurrentDirectoryInfo == null)
                return false;
            if (_navigation.CurrentDirectoryInfo.FullPath.Equals(dirPath))
                return true;
            return false;
        }

        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == this.detailViewToolStripMenuItem)
                this.thumbnailView1.SetView(View.Details);

            if (sender == this.iconViewToolStripMenuItem)
                this.thumbnailView1.SetView(View.LargeIcon);
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Open(comboBox1, comboBox1.Text);
        }
    }
}
