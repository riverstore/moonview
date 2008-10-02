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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MoonView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void OnApplicationStartup(object sender, StartupEventArgs args)
        {
            //ImageLoader.CreateThread();
            WinMain frmMain;
            if (args.Args.Length > 0)
                frmMain = new WinMain(args.Args[0]);
            else
                frmMain = new WinMain(null);    
            frmMain.Show();
            //frmMain.Photos = (PhotoCollection)(this.Resources["Photos"] as ObjectDataProvider).Data;
            //frmMain.Photos.Path = "E:\\My Documents\\My Pictures";
        }
    }
}
