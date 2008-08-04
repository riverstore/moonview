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
using System.Text;
using System.Drawing;

namespace MoonView
{
    public static class Ratio
    {
        public enum RatioType
        {
            NoResize,
            FitImage,
            FitWidth,
            FitHeight
        }

        public static double CalculateScale(Size orgSize, Size maxSize, RatioType type)
        {
            return CalculateScale(orgSize.Width, orgSize.Height, maxSize.Width, maxSize.Height, type);
        }

        public static double CalculateScale(int width, int height, int maxWidth, int maxHeight, RatioType type)
        {
            if (type == RatioType.NoResize)
                return 1;

            double ratio = ((double) width) / height; //original image ratio

            if (type == RatioType.FitImage)
            {
                if ((maxWidth / ratio) < maxHeight)
                    return ((double)maxWidth) / width; //fit height
                else
                    return ((double)maxHeight) / height; //fit width
            }
            else if (type == RatioType.FitWidth)
                return ((double)maxWidth) / width;
            else if (type == RatioType.FitHeight)        
                return ((double)maxHeight) / height;

            return 1;
        }
    }
}
