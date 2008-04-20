//System NS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

//MoonView NS
using MoonView.Path;

namespace MoonView.Forms
{
    public partial class ViewForm : Form
    {
        public class FileComparer : IComparer<IFileInfo>
        {
            public int Compare(IFileInfo fileX, IFileInfo fileY)
            {
                return Utility.CompareStringXp(fileX.Name, fileY.Name);
            }
        }

        //Values
        int _currIndex = 0;
        double _zoomRatio = 1.0;
        Ratio.RatioType _ratioType = Ratio.RatioType.FitImage;

        //Object
        FileComparer _comparer = new FileComparer();
        IDirectoryInfo _dirInfo;

        //Collection
        List<IFileInfo> _fileInfoList = new List<IFileInfo>();

        public ViewForm()
        {
            InitializeComponent();

            this.pictureBox1.Size = this.panel1.Size;
            lock (Utility.EmptyIconLarge)
                this.pictureBox1.ErrorImage = Utility.EmptyIconLarge; //Set error icon
        }

        private void Control_Resize(object sender, EventArgs e)
        {
            //Resize
            //if (_ratioType == Ratio.RatioType.NoResize)
            //    return;
            //if (_currSurf == null)
            //    return;
            //UpdateImage();

            //this.pictureBox1.Size = this.panel1.Size;
            Console.WriteLine("{0} resized", sender);
            ResizePictureBox();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        public void ShowImage(IFileInfo fileInfo)
        {
            _dirInfo = fileInfo.Directory;
            _fileInfoList.Clear();
            _currIndex = 0;
            IFileInfo[] fileInfoArray = _dirInfo.Files;
            for (int i = 0; i < fileInfoArray.Length; i++)
            {
                IFileInfo thisFileInfo = fileInfoArray[i];
                if (Utility.IsSupported(thisFileInfo.Extension))
                    _fileInfoList.Add(thisFileInfo);
            }
            _fileInfoList.Sort(_comparer);
            for (int i = 0; i < _fileInfoList.Count; i++)
            {
                IFileInfo thisFileInfo = _fileInfoList[i];
                if (fileInfo.Name.Equals(thisFileInfo.Name))
                    _currIndex = i;
            }
            UpdateButtonStatus();
            ShowImage(_currIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void ShowImage(int index)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(_fileInfoList[index].ReadAllBytes()))
                    this.pictureBox1.Image = Image.FromStream(ms, true, true);
            }
            catch (Exception)
            {
                this.pictureBox1.Image = this.pictureBox1.ErrorImage;
            }
            this.pictureBox1.Location = Point.Empty;
            this.panel1.VerticalScroll.Value = 0;
            this.panel1.HorizontalScroll.Value = 0;
            this.panel1.Focus();
            ResizePictureBox();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResizePictureBox()
        {
            //TODO limit zoom ratio
            double scale = Ratio.CalculateScale(pictureBox1.Image.Size, panel1.Size, _ratioType);
            int width = (int)(this.pictureBox1.Image.Width * scale * _zoomRatio);
            int height = (int)(this.pictureBox1.Image.Height * scale * _zoomRatio);
            //
            this.pictureBox1.Width = width;
            this.pictureBox1.Height = height;
            //
            this.pictureBox1.Location = new Point(0, 0);
            if (this.panel1.Width > this.pictureBox1.Width)
                this.pictureBox1.Location = new Point((int)((this.panel1.Width - this.pictureBox1.Width) / 2), this.pictureBox1.Location.Y);
            if (this.panel1.Height > this.pictureBox1.Height)
                this.pictureBox1.Location = new Point(this.pictureBox1.Location.X, (int)((this.panel1.Height - this.pictureBox1.Height) / 2));
            //
            this.Visible = true;
            this.panel1.Focus();
            //
            this.toolStripStatusLabel1.Text = string.Format("[{0}/{1}] {2} {3} x {4} px {5:f1}%",
                _currIndex + 1, //Index start at 1 for gui
                _fileInfoList.Count,
                _fileInfoList[_currIndex].Name,
                width,
                height,
                _zoomRatio * 100);

            #region old code
            //this.pictureBox1.Image = ImageLibrary.SurfaceToBitmap(_currSurf);
            //this.pictureBox1.Size = _currSurf.Size;
            //this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //this.Visible = true;

            //Rescale image
            //double scale = Ratio.CalculateScale(_currSurf.Size, this.panel1.Size, _ratioType);
            //scale = scale * _zoomRatio;
            //Surface scaledSurf = _currSurf.CreateScaledSurface(scale);
            //Size newSize;
            //if (this.panel1.Size.Width > scaledSurf.Size.Width)
            //{
            //    Surface tempSurface = new Surface(this.panel1.Size.Width, scaledSurf.Size.Height);
            //    tempSurface.Fill(Color.White);
            //    tempSurface.Blit(scaledSurf, new Point((this.panel1.Size.Width - scaledSurf.Size.Width) / 2, 0));
            //    this.pictureBox1 = ImageLibrary.SurfaceToBitmap(tempSurface);
            //    newSize = tempSurface.Size;
            //    tempSurface.Dispose();
            //}
            //else
            //{
            //    this.surfaceControl1.Blit(scaledSurf);
            //    newSize = scaledSurf.Size;
            //}
            ////Dispose temporary surface
            //scaledSurf.Dispose();
            ////Dummy control object for scollbar 
            //this.pictureBox1.Size = new Size(newSize.Width - 10, newSize.Height - 10);
            ////Show window
            //this.Visible = true;
            ////Update status label

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                this.Visible = false;
                e.Cancel = true;
            }
        }

        private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            VScrollProperties vScrollProp;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    ShowPrevImage(null, null);
                    break;
                case Keys.Right:
                    ShowNextImage(null, null);
                    break;
                case Keys.Up: //Key up
                    vScrollProp = this.panel1.VerticalScroll;
                    if (vScrollProp.Value - 100 > vScrollProp.Minimum)
                        vScrollProp.Value -= 100;
                    else
                        vScrollProp.Value = vScrollProp.Minimum;
                    break;
                case Keys.Down: //Key down
                    vScrollProp = this.panel1.VerticalScroll;
                    if (vScrollProp.Value + 100 < vScrollProp.Maximum)
                        vScrollProp.Value += 100;
                    else
                        vScrollProp.Value = vScrollProp.Maximum;
                    break;
                case Keys.Add:
                    ZoomIn(null, null);
                    break;
                case Keys.Subtract:
                    ZoomOut(null, null);
                    break;
                default:
                    break;
            }
        }

        private void UpdateButtonStatus()
        {
            PreviousButton.Enabled = !(_currIndex == 0);
            NextButton.Enabled = (_currIndex < _fileInfoList.Count - 1);
        }

        private void ShowPrevImage(object sender, EventArgs e)
        {
            if (_currIndex - 1 < 0)
                return;
            _currIndex--;
            UpdateButtonStatus();
            ShowImage(_currIndex);
        }

        private void ShowNextImage(object sender, EventArgs e)
        {
            if (_currIndex + 1 >= _fileInfoList.Count)
                return;
            _currIndex++;
            UpdateButtonStatus();
            ShowImage(_currIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomIn(object sender, EventArgs e)
        {
            _zoomRatio = _zoomRatio * 1.1;
            ResizePictureBox();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomOut(object sender, EventArgs e)
        {
            _zoomRatio = _zoomRatio / 1.1;
            ResizePictureBox();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BestFit(object sender, EventArgs e)
        {
            _ratioType = Ratio.RatioType.FitImage;
            _zoomRatio = 1;
            ResizePictureBox();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoResize(object sender, EventArgs e)
        {
            _ratioType = Ratio.RatioType.NoResize;
            _zoomRatio = 1;
            ResizePictureBox();
        }

        Point _lastMousePt;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs mouseEvtArgs)
        {
            if (mouseEvtArgs.Button != MouseButtons.Left)
                return;

            //Important: The mouse position is in respect of pictureBox not panel or window
            int deltaX = (int)(mouseEvtArgs.X + pictureBox1.Location.X - _lastMousePt.X);
            int deltaY = (int)(mouseEvtArgs.Y + pictureBox1.Location.Y - _lastMousePt.Y);

            _lastMousePt = new Point(mouseEvtArgs.X + pictureBox1.Location.X,
                                     mouseEvtArgs.Y + pictureBox1.Location.Y);

            int vertValue = this.panel1.VerticalScroll.Value - deltaY;
            int horzValue = this.panel1.HorizontalScroll.Value - deltaX;

            if (horzValue > this.panel1.HorizontalScroll.Maximum)
                this.panel1.HorizontalScroll.Value = this.panel1.HorizontalScroll.Maximum;
            else if (horzValue < this.panel1.HorizontalScroll.Minimum)
                this.panel1.HorizontalScroll.Value = this.panel1.HorizontalScroll.Minimum;
            else
                this.panel1.HorizontalScroll.Value = horzValue;

            if (vertValue > this.panel1.VerticalScroll.Maximum)
                this.panel1.VerticalScroll.Value = this.panel1.VerticalScroll.Maximum;
            else if (vertValue < this.panel1.VerticalScroll.Minimum)
                this.panel1.VerticalScroll.Value = this.panel1.VerticalScroll.Minimum;
            else
                this.panel1.VerticalScroll.Value = vertValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Important: The mouse position is in respect of pictureBox not panel or window
                _lastMousePt = new Point(e.Location.X + pictureBox1.Location.X,
                                     e.Location.Y + pictureBox1.Location.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender == this.pictureBox1)
                this.panel1.Focus();
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show((Control)sender, e.Location);
            if (e.Button == MouseButtons.Middle)
                ShowNextImage(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            if (this.WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeWindow(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoMaximizeWindow(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void FitWidthButton_Click(object sender, EventArgs e)
        {
            _ratioType = Ratio.RatioType.FitWidth;
            _zoomRatio = 1;
            ResizePictureBox();
        }
    }
}
