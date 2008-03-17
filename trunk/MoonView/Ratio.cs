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
                return ((double)maxHeight) / height;
            else if (type == RatioType.FitHeight)
                return ((double)maxWidth) / width;

            return 1;
        }
    }
}
