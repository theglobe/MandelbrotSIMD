using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Mandelbrot
{
    class MandelbrotGenerator
    {
        //private readonly Color[] palette = new Color[256];
        private readonly ColorPalette palette;
        public readonly int Width;
        public readonly int Height;

        private readonly int stepSize;
        private readonly Vector<double> incrementalVec;
        private readonly Vector<double> four = Vector<double>.One * 4;

        public MandelbrotGenerator(int width, int height)
        {
            Width = width;
            Height = height;

            stepSize = Vector<double>.Count;
            var initArray = Enumerable.Range(0, stepSize).ToArray();
            var initArrayD = Array.ConvertAll(initArray, item => (double)item);

            incrementalVec = new Vector<double>(initArrayD);

            var paletteArray = new Color[256];
            for (var r = 0; r < 8; r++)
                for (var g = 0; g < 4; g++)
                    for (var b = 0; b < 8; b++)
                    {
                        var value = r*32+g*8+b;
                        paletteArray[value] = Color.FromArgb(255, (int)(r * 36.4285714286), (int)(g * 85), (int)(b * 36.4285714286));
                    }

            palette = new Bitmap(width, height, PixelFormat.Format8bppIndexed).Palette;
            for (var i = 0; i < 256; i++)
                palette.Entries[i] = paletteArray[i];
        }

        public Image Generate(out ulong[] imageDataOut, double centerX, double centerY, double pixelToWorldScale, int numIterations)
        {
            var imageData = new ulong[Width * Height];
            var nThreads = 4;
            var nThreadsJ = 5;
            var tasks = new List<Task>();

            for (var i = 0; i < nThreads; i++)
                for (var j = 0; j < nThreadsJ; j++)
                {
                var index = i;
                var jndex = j;
                Action generate = () =>
                {
                    var x1 = (int) (index / (double)nThreads *Width);
                    var x2 = (int) ((index + 1)/ (double)nThreads * Width);
                    var y1 = (int)(jndex / (double)nThreadsJ * Height);
                    var y2 = (int)((jndex + 1) / (double)nThreadsJ * Height);

                    Generate(ref imageData, centerX, centerY, pixelToWorldScale, numIterations, x1, y1, x2, y2);
                };
                tasks.Add(Task.Factory.StartNew(generate));
            }

            Task.WaitAll(tasks.ToArray());

            var image = Array.ConvertAll(imageData, item => (byte)item);
            var bitmap = GetImage(Width, Height, image);

            imageDataOut = imageData;
            return bitmap;
        }
        public void Generate(ref ulong[] imageData, double centerX, double centerY, double pixelToWorldScale, int numIterations, int startX, int startY, int endX, int endY)
        {
            var worldLeft = centerX - Width  * pixelToWorldScale / 2  + startX * pixelToWorldScale;
            var worldTop = -centerY + Height * pixelToWorldScale / 2 - startY * pixelToWorldScale;

            var pixelToWorldScaleVec = new Vector<double>(pixelToWorldScale);

            var pixelToWorldScaleStepVec = pixelToWorldScaleVec * stepSize;
            var worldLeftVec = new Vector<double>(worldLeft);

            Vector<double> worldXstart = incrementalVec * pixelToWorldScaleVec + worldLeftVec;
            Vector<double> worldX = worldXstart;

            var worldY = new Vector<double>(worldTop);

            for (var y = startY; y < endY; y++)
            {
                var yStart = y * Width;
                for (var x = startX; x < endX; x += stepSize)
                {
                    Vector<double> zReal = Vector<double>.Zero, zImag = Vector<double>.Zero;

                    Vector<ulong> count = Vector<ulong>.Zero;
                    var iteration = 0;
                    while (iteration < numIterations)
                    {
                        var zReal2 = zReal * zReal;
                        var zImag2 = zImag * zImag;

                        var countable = Vector.LessThan(zReal2 + zImag2, four);
                        if (Vector.EqualsAll(countable, Vector<long>.Zero)) break;
                        count -= (Vector<ulong>) countable;

                        var zRealPrev = zReal;
                        zReal = zReal2 - zImag2 + worldX;
                        zImag = 2 * zRealPrev * zImag + worldY;

                        iteration++;
                    }
                    count.CopyTo(imageData, yStart + x);

                    worldX += pixelToWorldScaleStepVec;
                }

                worldX = worldXstart;
                worldY -= pixelToWorldScaleVec;
            }
        }

        private Image GetImage(int width, int height, byte[] data)
        {

            var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);

            var ptr = bmpData.Scan0;

            Marshal.Copy(data, 0, ptr, data.Length);
            bmp.UnlockBits(bmpData);
            bmp.Palette = palette;

            return bmp;
        }
    }
}
