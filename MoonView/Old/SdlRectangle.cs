using System;
using System.Collections.Generic;
using System.Text;

using SdlDotNet.Graphics;
using System.Drawing;

namespace MoonView
{
    public class SdlRectangle : Surface
    {
        int _border;
        Color _bgColor = Color.Transparent;
        Color _borderColor = Color.DarkBlue;

        public SdlRectangle(Size rectSize, int border, Color bgColor, Color borderColor): base(rectSize)
        {
            _bgColor = bgColor;
            _borderColor = borderColor;

            this.Fill(bgColor);
            //Create border
            

            //this.Fill(
        }
    }
}
