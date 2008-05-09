using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using MoonView.Path;

namespace MoonView
{
    public class Navigation
    {
        //Control
        ToolStripButton _backBtn;
        ToolStripButton _fwdBtn;
        ToolStripButton _upBtn;
        ToolStripButton _refreshBtn;

        //Values
        int _currIndex = -1;

        //Object
        MoonViewForm _parent;

        //Collection
        List<IDirectoryInfo> _dirInfoHistory;

        public Navigation(MoonViewForm parent, ToolStripButton backBtn, ToolStripButton fwdBtn, ToolStripButton upBtn, ToolStripButton refreshBtn)
        {
            _dirInfoHistory = new List<IDirectoryInfo>();
            _backBtn = backBtn;
            _fwdBtn = fwdBtn;
            _upBtn = upBtn;
            _refreshBtn = refreshBtn;

            _parent = parent;
            _backBtn.Click += new EventHandler(_backBtn_Click);
            _fwdBtn.Click += new EventHandler(_fwdBtn_Click);
            _upBtn.Click += new EventHandler(_upBtn_Click);
            _refreshBtn.Click += new EventHandler(_refreshBtn_Click);
        }

        public IDirectoryInfo CurrentDirectoryInfo
        {
            get
            {
                if (_dirInfoHistory.Count == 0)
                    return null;
                return _dirInfoHistory[_currIndex];
            }
        }

        void _refreshBtn_Click(object sender, EventArgs e)
        {
            _parent.ShowDirectory(this, CurrentDirectoryInfo);
        }

        void _upBtn_Click(object sender, EventArgs e)
        {
            IDirectoryInfo parentDir = _dirInfoHistory[_currIndex].Parent;
            AddDirectoryInfo(parentDir);
            UpdateButtonStatus();
            _parent.ShowDirectory(this, parentDir);
        }

        void _fwdBtn_Click(object sender, EventArgs e)
        {
            _currIndex++;
            UpdateButtonStatus();
            ShowCurrentDirInfo();
        }

        void _backBtn_Click(object sender, EventArgs e)
        {
            _currIndex--;
            UpdateButtonStatus();
            ShowCurrentDirInfo();
        }

        void ShowCurrentDirInfo()
        {
            if (_dirInfoHistory.Count == 0 || _dirInfoHistory.Count <= _currIndex)
                return;
            _parent.ShowDirectory(this, _dirInfoHistory[_currIndex]);
        }

        public void AddDirectoryInfo(IDirectoryInfo dirInfo)
        {
            //Remove old directoryInfo, if back button has been clicked
            while (_dirInfoHistory.Count > (_currIndex + 1))
                _dirInfoHistory.RemoveAt(_dirInfoHistory.Count - 1);
            //Add directoryInfo
            _currIndex = _dirInfoHistory.Count;
            _dirInfoHistory.Add(dirInfo);
            UpdateButtonStatus();
        }

        IDirectoryInfo GetLastDirectory()
        {
            if (_dirInfoHistory.Count == 0)
                return null;
            return _dirInfoHistory[_dirInfoHistory.Count - 1];
        }

        void UpdateButtonStatus()
        {
            //Back button
            if (_dirInfoHistory.Count == 0 || _currIndex == 0)
                _backBtn.Enabled = false;
            else
                _backBtn.Enabled = true;
            //Forward button
            if (_currIndex != _dirInfoHistory.Count - 1 && _dirInfoHistory.Count > 0)
                _fwdBtn.Enabled = true;
            else
                _fwdBtn.Enabled = false;
            //Up button
            _upBtn.Enabled = false;
            if (_currIndex < _dirInfoHistory.Count && _currIndex > -1)
            {
                if (_dirInfoHistory[_currIndex].Parent != null)
                    _upBtn.Enabled = true;
            }
        }
    }
}
