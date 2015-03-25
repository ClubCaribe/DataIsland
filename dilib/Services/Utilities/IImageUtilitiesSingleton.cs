using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace dataislandcommon.Services.Utilities
{
    public interface IImageUtilitiesSingleton
    {
        Bitmap ColorizePicture(Bitmap b, string color);
        Bitmap FillPictureInSquare(Bitmap b, int sWidth, int sHeight);
        ImageFormat GetImageFormatFromExtension(string ext);
        Bitmap OffsetPicture(Bitmap b, int leftoffset, int topoffset);
        Bitmap ResizePicture(Bitmap b, int nWidth, int nHeight, bool bBilinear, Color fillColor);
        Bitmap ResizePicture(Bitmap b, int nWidth, int nHeight, int stencilwidth, int stencilheight, bool bBilinear);
        Bitmap ResizePicture(Bitmap b, int sWidth, int sHeight, Color fillColor);
        Bitmap ResizePictureIfLarger(Bitmap b, int sWidth, int sHeight, Color fillColor);
        Bitmap ResizePictureSquare(Bitmap b, int sWidth);
        Bitmap PutPictureInSquare(Bitmap b, int sWidth);
        Bitmap ResizePictureSquare(Bitmap b, int sWidth, int sHeight, Color fillcolor);
        Bitmap ResizeToHeight(Bitmap b, int sHeight);
        byte[] ResizeToWidth(byte[] bdata, int sWidth, string outputformat);
        Bitmap ResizeToWidth(Bitmap b, int sWidth);
        Bitmap TransformPicture(Bitmap b, int width, int height, string type);
        byte[] TransformImage(byte[] imgdata, string type, string outputformat);
        byte[] TransformImage(Image img, string type, string outputformat);
        byte[] TransformImageToByte(Image img, string extension);
        ImageFormat TranslateImageFormat(ImageFormat ext);
        Bitmap GenerateImagePreview(string path);
        byte[] GenerateImagePreviewAndGetImageData(string path, string extension);
        void RotateFlip(Bitmap bmp, RotateFlipType transformType);
    }
}
