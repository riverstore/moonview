/// Copyright (C) 2008 Matthew Ng
/// 
/// This program is free software; you can redistribute it and/or
/// modify it under the terms of the GNU General Public License
/// as published by the Free Software Foundation; either version 2
/// of the License, or (at your option) any later version.
/// 
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with this program; if not, write to the Free Software
/// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonView
{
    public class ClsNavTracker
    {
        private int _currentIndex;
        private List<string> _pathList;

        public ClsNavTracker(string path)
        {
            _currentIndex = 0;
            _pathList = new List<string>();
            _pathList.Add(path);
        }

        public bool HasLastPath()
        {
            return (_pathList.Count > 0 && _currentIndex > 0);
        }

        public bool HasNextPath()
        {
            return (_currentIndex < _pathList.Count - 1);
        }

        public string GetLastPath()
        {
            if (HasLastPath())
                _currentIndex--;
            return _pathList[_currentIndex];
        }

        public string GetNextPath()
        {
            if (HasNextPath())
                _currentIndex++;
            return _pathList[_currentIndex];
        }

        public string GetCurrentPath()
        {
            return _pathList[_currentIndex];
        }

        public void AddCurrentPath(string path)
        {
            //Remove next paths
            for (int i = _currentIndex + 1; i < _pathList.Count; i++)
                _pathList.RemoveAt(i);
            
            //Add current path 
            if (!_pathList[_currentIndex].Equals(path))
            {
                _pathList.Add(path);
                _currentIndex = _pathList.Count - 1;
            }
        }
    }
}
