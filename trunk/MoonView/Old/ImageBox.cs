//System NS
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

//SdlDotNet NS
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Graphics.Primitives;

using MoonView.Path;

namespace MoonView.Thumbnail
{
    public class ImageBox
    {
        static int BORDER = 1; //Border size

        public static Surface GetSurface(Size boxSize)
        {
            return GetSurface((byte[]) null, boxSize);
        }

        public static Surface GetSurface(string imgPath, Size boxSize)
        {
            return GetSurface(File.ReadAllBytes(imgPath), boxSize);
        }

        public static Surface GetSurface(byte[] imgBytes, Size boxSize)
        {
            Surface boxSurf = new Surface(boxSize);
            boxSurf.Fill(Color.White);  //Fill background to white color
            //Get and resize image
            if (imgBytes != null)
            {
                Surface imgSurf = new Surface(imgBytes);
                double scale = Ratio.CalculateScale(imgSurf.Size,
                                                    new Size(boxSize.Width, boxSize.Height),
                                                    Ratio.RatioType.FitImage); //Calculate ratio
                Surface scaledSurf = imgSurf.CreateScaledSurface(scale, true);
                imgSurf.Dispose();
                Point pt = new Point((boxSize.Width - scaledSurf.Width) / 2,      //Left point
                                     (boxSize.Height - scaledSurf.Height) / 2);   //Top point
                boxSurf.Blit(scaledSurf, pt);
                scaledSurf.Dispose();  //Clear imgSurf memory
            }
            //Draw border
            for(int i = 0; i < BORDER; i++)
                boxSurf.Draw(new Box(new Point(i, i), new Point(boxSize.Width - i - 1, boxSize.Height - i - 1)), Color.Gray);
            return boxSurf;
        }
    }
}
