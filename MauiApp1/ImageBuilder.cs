using SkiaSharp;
using System.Runtime.InteropServices;

namespace MauiApp1
{
    internal class ImageBuilder
    {

        private FileResult source;
        private String extension;
        //trueの時画像はモノクロに
        private Boolean MONOCOLOR = false;
        //画像の角度は90*ROTATENUMで回転
        private int ROTATENUM = 0;

        public ImageBuilder(FileResult source) {
            //image = ImageSource.FromFile("dotnet_bot.png");
            this.source = source;
            if (source.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase))
            {
                extension = ".jpg";
            }
            else if(source.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                extension = ".png";
            }
        }

        //ゲッター・セッター
        public FileResult getSource()
        {
            return this.source;
        }
        public String getExtension() { 
            return extension;
        }

        public ImageSource getImage()
        {
            SKData prosessedData = SKData.Create(source.FullPath);

            if (MONOCOLOR)
            {
                prosessedData = changeColor(prosessedData);
            }
            if (ROTATENUM != 0)
            {
                prosessedData = Rotate(prosessedData);
            }

            return ImageSource.FromStream(() => prosessedData.AsStream());

        }
        public void setMONOCOLOR(Boolean result)
        {
            MONOCOLOR = result;
            return;
        }
        public void setRotate(int dir)
        {
            //1の時右に90°回転、-1の時左に90°回転
            ROTATENUM += dir;
            if (ROTATENUM < 0)
            {
                ROTATENUM = 3;
            }
            else if(ROTATENUM > 3)
            {
                ROTATENUM = 0;
            }
            return;
        }

        //画像を保存
        public bool saveFile(String outputPath)
        {
            SKData prosessedData = SKData.Create(source.FullPath);

            if (MONOCOLOR)
            {
                prosessedData = changeColor(prosessedData);
            }
            if (ROTATENUM != 0)
            {
                prosessedData = Rotate(prosessedData);
            }

            try
            {
                var bitmap = SKBitmap.Decode(prosessedData);
                var bitmap_byte = bitmap.Bytes;

                SKBitmap new_bitmap = new SKBitmap(bitmap.Width, bitmap.Height);
                IntPtr ptr = new_bitmap.GetPixels();
                Marshal.Copy(bitmap_byte, 0, ptr, bitmap_byte.Length);
                var stream = File.OpenWrite(outputPath);
                new_bitmap.Encode(stream, SKEncodedImageFormat.Png, 80);
                stream.Dispose();
            }catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        private SKData changeColor(SKData data)
        {
            //画像の色返還
            SKBitmap resourceBitmap = SKBitmap.Decode(data);
            SKBitmap resultBitmap = new SKBitmap(resourceBitmap.Width, resourceBitmap.Height);
            SKCanvas canvas = new(resultBitmap);
            SKImage resourceImage = SKImage.FromBitmap(resourceBitmap);

            float red = 0.14f;
            float blue = 0.51f;
            float grren = 0.05f;

            using (SKPaint paint = new SKPaint())
            {

                //明度の設定
                paint.ColorFilter =
                    SKColorFilter.CreateColorMatrix(new float[]
                    {
                        red, blue, grren, 0, 0,
                        red, blue, grren, 0, 0,
                        red, blue, grren, 0, 0,
                        0,   0,    0,     1, 0
                    });

                canvas.DrawImage(resourceImage, 0, 0, paint: paint);
            }

            SKImage resultImage = SKImage.FromBitmap(resultBitmap);
            return resultImage.Encode(SKEncodedImageFormat.Png, 100);
        }

        private SKData Rotate(SKData data)
        {
            //画像の回転

            SKBitmap resourceBitmap = SKBitmap.Decode(data);
            SKBitmap resultBitmap = new SKBitmap(resourceBitmap.Width, resourceBitmap.Height);
            SKCanvas canvas = new(resultBitmap);
            SKImage resourceImage = SKImage.FromBitmap(resourceBitmap);

            canvas.RotateDegrees(90*ROTATENUM,resourceBitmap.Width / 2, resourceBitmap.Height / 2);
            canvas.DrawImage(resourceImage, 0, 0);
            

            SKImage resultImage = SKImage.FromBitmap(resultBitmap);
            return resultImage.Encode(SKEncodedImageFormat.Png, 100);
        }
    }
}
