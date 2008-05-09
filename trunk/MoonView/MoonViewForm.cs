//System NS
using System;
using System.IO;
using SysPath = System.IO.Path;
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
        AboutBox _aboutBox;

        /// <summary>
        /// Constructor
        /// </summary>
        public MoonViewForm(string[] args)
        {
            InitializeComponent();
            Utility.LoadConfig();
            Utility.Initialise();

            _aboutBox = new MoonView.Forms.AboutBox();
            this.directoryTreeView1.Initialise(this);
            this.thumbnailView1.Initialise(this);
            _viewForm = new ViewForm();
            _navigation = new Navigation(this, BackButton, ForwardButton, UpButton, RefreshButton);

            this.Top = Utility.Config.Top;
            this.Left = Utility.Config.Left;
            this.Size = Utility.Config.Size;

            if (args.Length < 1)
                Open(this, Utility.Config.LastDirectoryPath);
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
                            //TODO Refactor combine with ShowArchive
                        case ".zip":
                            ZipDirectoryInfo zipDirInfo = new ZipDirectoryInfo(new BaseFileInfo(path));
                            ShowDirectory(sender, zipDirInfo);
                            return;
                        case ".rar":
                            RarDirectoryInfo rarDirInfo = new RarDirectoryInfo(new BaseFileInfo(path));
                            ShowDirectory(sender, rarDirInfo);
                            return;
                        case ".7z":
                            SevenZipDirectoryInfo sevenZipInfo = new SevenZipDirectoryInfo(new BaseFileInfo(path));
                            ShowDirectory(sender, sevenZipInfo);
                            break;
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
                case ".7z":
                    SevenZipDirectoryInfo sevenZipInfo = new SevenZipDirectoryInfo((BaseFileInfo)fileInfo);
                    ShowDirectory(sender, sevenZipInfo);
                    break;
                default:
                    break;
            }
        }

        public void ShowProgress(int value)
        {
            toolStripProgressBar1.Value = value;
        }

        private void Exit()
        {
            //TODO Remove temporary files
            thumbnailView1.AbortLoading();
            if (thumbnailView1.IsBusy)
                Thread.Sleep(100);

            Utility.Config.Top = this.Top;
            Utility.Config.Left = this.Left;

            Utility.Config.Size = this.Size;
            Utility.Config.LastDirectoryPath = thumbnailView1.CurrentDirectoryInfo.FullPath;
            Utility.SaveConfig();

            //Clean up
            string tempDir = SysPath.Combine(Environment.CurrentDirectory, "Temp");
            try
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            Application.Exit();
        }

        /// <summary>
        /// Handle main form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoonViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Exit();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _aboutBox.Show();
        }
    }
}
