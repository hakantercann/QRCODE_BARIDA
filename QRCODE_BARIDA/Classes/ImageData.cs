using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Drawing;

namespace QRCODE_BARIDA.Classes
{
    public static class ImageData
    {
        public static byte[] pathToImage(string path)
        {
            byte[] data = null;
    FileInfo fInfo = new FileInfo(path);
            long numBytes = fInfo.Length;

            FileStream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fStream);

            data = br.ReadBytes((int)numBytes);
            return data;

        }
        public static string saveImageasFile(Bitmap bi)
        {
            /*         RenderTargetBitmap bmp = new RenderTargetBitmap(300, 300, 96, 96, PixelFormats.Pbgra32);
                     JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                     jpg.Frames.Add(BitmapFrame.Create(bi));
                     using (Stream stm = File.Create("C:\\Users\\hakan\\source\\repos\\QRCODE_BARIDA\\QRCODE_BARIDA\\logimages\\im.jpg"))
                     {
                         jpg.Save(stm);

                     }*/
            return "a";
        }
    }
}

