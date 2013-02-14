using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MyrUtil
{
    public static class Extensions
    {

        public static Image Scale(this Image img, int maxWidth, int maxHeight)
        {
            double scale = 1;

            if (img.Width > maxWidth || img.Height > maxHeight)
            {
                double scaleW, scaleH;

                scaleW = maxWidth / (double)img.Width;
                scaleH = maxHeight / (double)img.Height;

                scale = scaleW < scaleH ? scaleW : scaleH;
            }

            return img.Resize((int)(img.Width * scale), (int)(img.Height * scale));
        }

        public static Image Scale(this Image img, Size maxDimensions)
        {
            return img.Scale(maxDimensions.Width, maxDimensions.Height);
        }

        public static Image Resize(this Image img, int width, int height)
        {
            return img.GetThumbnailImage(width, height, null, IntPtr.Zero);
        }
    }
}
