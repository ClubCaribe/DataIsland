using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace dataislandcommon.Services.Utilities
{
    public class ImageUtilitiesSingleton : dataislandcommon.Services.Utilities.IImageUtilitiesSingleton
    {
        public ImageFormat GetImageFormatFromExtension(string ext)
        {
            try
            {
                string workext = ext;
                if (workext.StartsWith("."))
                {
                    workext = workext.Substring(1);
                }
                switch (workext)
                {
                    case "jpg": return ImageFormat.Jpeg;
                    case "jpeg": return ImageFormat.Jpeg;
                    case "tif": return ImageFormat.Jpeg;
                    case "png": return ImageFormat.Png;
                    case "bmp": return ImageFormat.Bmp;
                    case "gif": return ImageFormat.Gif;
                }
            }
            catch
            {
            }
            return ImageFormat.Png;
        }

        public ImageFormat TranslateImageFormat(ImageFormat ext)
        {
            try
            {
                if (ext == ImageFormat.Png)
                {
                    return ext;
                }

                if (ext == ImageFormat.Jpeg)
                {
                    return ext;
                }

                if (ext == ImageFormat.Bmp)
                {
                    return ext;
                }

                if (ext == ImageFormat.Gif)
                {
                    return ext;
                }
            }
            catch
            {
            }
            return ImageFormat.Jpeg;
        }

        private byte[] GetPictureByte(Bitmap img)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Jpeg);
                    byte[] b = ms.GetBuffer();
                    ms.Close();
                    return b;
                }
            }
            catch
            {
                return null;
            }
        }

        public Bitmap ResizePictureSquare(Bitmap b, int sWidth)
        {
            int nwidth, nheight;
            if (b.Width < b.Height)
            {
                nwidth = sWidth;
                nheight = Math.Max(1, Math.Min(5000, (int)(sWidth * b.Height / b.Width)));

            }
            else
            {
                nheight = sWidth;
                nwidth = Math.Max(1, Math.Min(5000, (int)(sWidth * b.Width / b.Height)));
            }
            Bitmap bmPhoto = new Bitmap(sWidth, sWidth,
                                 PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(b.HorizontalResolution,
                                    b.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(b,
                    new Rectangle(0 - ((nwidth - sWidth) / 2), 0 - ((nheight - sWidth) / 2), nwidth, nheight),
                    new Rectangle(0, 0, b.Width, b.Height),
                    GraphicsUnit.Pixel);

                //grPhoto.Dispose();
            }
            return bmPhoto;
        }

        public Bitmap PutPictureInSquare(Bitmap b, int sWidth)
        {
            int nwidth, nheight;
            if (b.Width > b.Height)
            {
                nwidth = sWidth;
                nheight = Math.Max(1, Math.Min(5000, (int)(sWidth * b.Height / b.Width)));

            }
            else
            {
                nheight = sWidth;
                nwidth = Math.Max(1, Math.Min(5000, (int)(sWidth * b.Width / b.Height)));
            }

            Bitmap bmPhoto = new Bitmap(sWidth, sWidth,
                                 PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(b.HorizontalResolution,
                                    b.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(b,
                    new Rectangle((sWidth / 2) - (nwidth / 2), (sWidth / 2) - (nheight / 2), nwidth, nheight),
                    new Rectangle(0, 0, b.Width, b.Height),
                    GraphicsUnit.Pixel);

                //grPhoto.Dispose();
            }
            return bmPhoto;
        }

        public Bitmap FillPictureInSquare(Bitmap b, int sWidth, int sHeight)
        {
            int nwidth, nheight;

            nwidth = sWidth;
            nheight = Math.Max(1, Math.Min(5000, (int)((sWidth) * b.Height / b.Width)));

            //if (b.Width > b.Height)
            //{

            //    nwidth = sWidth;
            //    nheight = Math.Max(1, Math.Min(5000, (int)((sWidth) * b.Height / b.Width)));
            //}
            //else
            //{
            //    nheight = sHeight;
            //    nwidth = Math.Max(1, Math.Min(5000, (int)((sHeight) * b.Width / b.Height)));
            //}

            //if (nheight > sHeight)
            //{
            //    nheight = sHeight;
            //    nwidth = Math.Max(1, Math.Min(5000, (int)((sHeight) * b.Width / b.Height)));
            //}

            Bitmap bmPhoto = new Bitmap(sWidth, sHeight,
                                 PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(b.HorizontalResolution,
                                    b.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(b,
                    new Rectangle(0, 0, nwidth, nheight),
                    new Rectangle(0, 0, b.Width, b.Height),
                    GraphicsUnit.Pixel);

                //grPhoto.Dispose();
            }
            return bmPhoto;
        }

        public Bitmap ResizePictureSquare(Bitmap b, int sWidth, int sHeight)
        {
            int nwidth, nheight;
            if (b.Width > b.Height)
            {
                nwidth = sWidth;
                nheight = Math.Max(1, Math.Min(5000, (int)((sWidth) * b.Height / b.Width)));

            }
            else
            {
                nheight = sHeight;
                nwidth = Math.Max(1, Math.Min(5000, (int)((sHeight) * b.Width / b.Height)));
            }

            if (nheight > sHeight)
            {
                nheight = sHeight;
                nwidth = Math.Max(1, Math.Min(5000, (int)((sHeight) * b.Width / b.Height)));
            }

            Bitmap bmPhoto = new Bitmap(sWidth, sHeight,
                                 PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(b.HorizontalResolution,
                                    b.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(b,
                    new Rectangle(0 - ((nwidth - sWidth) / 2), 0 - ((nheight - sHeight) / 2), nwidth, nheight),
                    new Rectangle(0, 0, b.Width, b.Height),
                    GraphicsUnit.Pixel);

                //grPhoto.Dispose();
            }
            return bmPhoto;
        }

        public Bitmap ResizeToHeight(Bitmap b, int sHeight)
        {
            int nwidth, nheight;
            nheight = sHeight;
            nwidth = Math.Max(1, Math.Min(5000, (int)(sHeight * b.Width / b.Height)));
            return ResizePicture(b, nwidth, nheight, true);
        }

        public Bitmap ResizeToWidth(Bitmap b, int sWidth)
        {

            int nwidth, nheight;
            nwidth = sWidth;
            nheight = Math.Max(1, Math.Min(5000, (int)(sWidth * b.Height / b.Width)));
            return ResizePicture(b, nwidth, nheight, true);
        }

        public byte[] ResizeToWidth(byte[] bdata, int sWidth, string outputformat)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bdata))
                {
                    Bitmap b = (Bitmap)Image.FromStream(ms);
                    ms.Close();
                    ms.Dispose();
                    b = ResizeToWidth(b, sWidth);
                    byte[] outdata = TransformImageToByte((Image)b, outputformat);
                    return outdata;
                }
            }
            catch
            {
            }
            return null;

        }

        public Bitmap ResizePicture(Bitmap b, int sWidth, int sHeight)
        {
            int nwidth, nheight;
            if (b.Width > b.Height)
            {
                nwidth = sWidth;
                nheight = Math.Max(1, Math.Min(5000, (int)(sWidth * b.Height / b.Width)));

            }
            else
            {
                nheight = sHeight;
                nwidth = Math.Max(1, Math.Min(5000, (int)(sHeight * b.Width / b.Height)));
            }
            return ResizePicture(b, nwidth, nheight, true);
        }

        public Bitmap ResizePictureIfLarger(Bitmap b, int sWidth, int sHeight)
        {
            try
            {
                if ((b.Width > sWidth) || (b.Height > sHeight))
                {
                    return ResizePicture(b, sWidth, sHeight);
                }
            }
            catch
            {
            }
            return ResizePicture(b, b.Width, b.Height);
        }

        public Bitmap ResizePicture(Bitmap b, int nWidth, int nHeight, bool bBilinear)
        {
            Bitmap bmPhoto = new Bitmap(nWidth, nHeight,
                                 PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(b.HorizontalResolution,
                                    b.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBilinear;

                grPhoto.DrawImage(b,
                    new RectangleF(-0.5f, -0.5f, ((float)nWidth) + 0.5f, ((float)nHeight) + 0.5f),
                    new RectangleF(0.0f, 0.0f, (float)b.Width, (float)b.Height),
                    GraphicsUnit.Pixel);

                //grPhoto.Dispose();
            }
            return bmPhoto;

        }

        public Bitmap OffsetPicture(Bitmap b, int leftoffset, int topoffset)
        {
            Bitmap bmPhoto = new Bitmap(b.Width, b.Height,
                                 PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(b.HorizontalResolution,
                                    b.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(b,
                    new Rectangle(leftoffset, topoffset, b.Width, b.Height),
                    new Rectangle(0, 0, b.Width, b.Height),
                    GraphicsUnit.Pixel);

                //grPhoto.Dispose();
            }
            return bmPhoto;
        }

        public Bitmap ColorizePicture(Bitmap b, string color)
        {
            ColorPalette palette = b.Palette;
            Color cl = ColorTranslator.FromHtml("#ff0000");
            if (palette.Entries.Length > 0)
            {
                palette.Entries[0] = cl;
            }
            b.Palette = palette;
            return b;
        }

        

        public Bitmap ResizePicture(Bitmap b, int nWidth, int nHeight, int stencilwidth, int stencilheight, bool bBilinear)
        {
            Bitmap bmPhoto = new Bitmap(stencilwidth, stencilheight,
                                 PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(b.HorizontalResolution,
                                    b.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(b,
                    new Rectangle(0, 0, nWidth, nHeight),
                    new Rectangle(0, 0, nWidth, nHeight),
                    GraphicsUnit.Pixel);

                //grPhoto.Dispose();
            }
            return bmPhoto;

        }

        public byte[] TransformImageToByte(Image img, string extension)
        {
            try
            {
                byte[] buff = null;

                using (MemoryStream ms = new MemoryStream())
                {

                    img.Save(ms, GetImageFormatFromExtension(extension));
                    buff = ms.GetBuffer();
                    img.Dispose();
                    img = null;
                    if (buff != null)
                    {
                        return buff;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public Bitmap TransformPicture(Bitmap b, int width, int height, string type)
        {
            switch(type)
            {
                case "sqr":
                    {
                        return ResizePictureSquare(b, width);
                    }
                default:
                    {
                        return ResizePicture(b, width, height);
                    }
            }
        }

        public byte[] TransformImage(byte[] imgdata, string type, string outputformat)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imgdata))
                {
                    Bitmap b = (Bitmap)Image.FromStream(ms);
                    ms.Close();
                    return TransformImage(b, type, outputformat);
                }
            }
            catch
            {
            }
            return null;
        }

        public byte[] TransformImage(Image img, string type, string outputformat)
        {
            try
            {
                if ((type != "") && (type != "0"))
                {

                    Bitmap bmp = (Bitmap)img;
                    string[] operations = type.Split(new char[] { '|' });
                    foreach (string operation in operations)
                    {
                        List<string> operationdata = operation.Split(new char[] { '-' }).ToList();
                        if (operationdata.Count > 0)
                        {
                            if (operationdata[0] == "colorize")
                            {
                                try
                                {
                                    string color = operationdata[1];
                                    bmp = ColorizePicture(bmp, color);
                                }
                                catch
                                {
                                }
                            }
                            else if (operationdata[0] == "offset")
                            {
                                try
                                {
                                    int left, top;
                                    left = int.Parse(operationdata[1]);
                                    top = int.Parse(operationdata[2]);
                                    bmp = OffsetPicture(bmp, left, top);
                                }
                                catch
                                {
                                }
                            }
                            else if (operationdata[0] == "resizeiflarger")
                            {
                                try
                                {
                                    int square = int.Parse(operationdata[1]);

                                    bmp = ResizePictureIfLarger(bmp, square, square);
                                }
                                catch
                                {
                                }
                            }
                            else if (operationdata[0] == "fillpicture")
                            {
                                int w, h;
                                w = int.Parse(operationdata[1]);
                                h = int.Parse(operationdata[2]);
                                bmp = FillPictureInSquare(bmp, w, h);
                            }
                            else if (operationdata[0] == "resizesquare")
                            {
                                string dimension;
                                dimension = operationdata[1];
                                bmp = ResizePictureSquare(bmp, int.Parse(dimension));
                            }
                            else if (operationdata[0] == "resize")
                            {
                                string wth, hth;

                                wth = operationdata[1];
                                hth = operationdata[2];
                                bmp = ResizePictureSquare(bmp, int.Parse(wth), int.Parse(hth));
                            }
                            else if (operationdata[0] == "resizetoheight")
                            {
                                bmp = ResizeToHeight(bmp, int.Parse(operationdata[1]));
                            }
                            else if (operationdata[0] == "resizetowidth")
                            {
                                bmp = ResizeToWidth(bmp, int.Parse(operationdata[1]));
                            }
                            else if (operationdata[0] == "size")
                            {
                                bmp = ResizePicture(bmp, int.Parse(operationdata[1]), int.Parse(operationdata[1]));
                            }

                        }
                    }

                    return TransformImageToByte(bmp, outputformat);
                }
                else
                {
                    return TransformImageToByte(img, outputformat);
                }
            }
            catch
            {
            }
            return null;
        }

        public Bitmap GenerateImagePreview(string path)
        {
            try
            {
                using (Bitmap originalBitmap = (Bitmap)Image.FromFile(path))
                {
                    Bitmap preview = this.ResizePictureIfLarger(originalBitmap, 1200, 1200);
                    return preview;
                }
            }
            catch
            {
            }
            return null;
        }

        public byte[] GenerateImagePreviewAndGetImageData(string path, string extension)
        {
            using (Bitmap preview = this.GenerateImagePreview(path))
            {
                if (preview != null)
                {
                    byte[] imageData = this.TransformImageToByte(preview, extension);
                    return imageData;
                }
            }
            return null;
        }

        public void RotateFlip(Bitmap bmp, RotateFlipType transformType)
        {
            bmp.RotateFlip(transformType);
        }


    }
}
