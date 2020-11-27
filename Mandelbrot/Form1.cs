using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Graphics
{
    public partial class Form1 : Form
    {
        private readonly List<Scene> _scene = new List<Scene>();
        private int _currentScene = 0;
        private readonly MandelbrotGenerator generator;

        private class Scene
        {
            private readonly MandelbrotGenerator Generator;

            public readonly double PosX = 0;
            public readonly double PosY = 0;
            public readonly double PixelToWorldScale = 4d/1080;
            public readonly int NumIterations = 256;

            public string RunTime;
            public Image Image;
            public ulong[] imageData;

            public Scene(MandelbrotGenerator generator)
            {
                Generator = generator;

                Generate();
            }

            public Scene(MandelbrotGenerator generator, double posX, double posY, double pixelToWorldScale, int numIterations)
            {
                PosX = posX;
                PosY = posY;
                PixelToWorldScale = pixelToWorldScale;
                NumIterations = numIterations;
                Generator = generator;

                Generate();
            }

            void Generate()
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var width = Generator.Width;
                var height = Generator.Height;
                if (width == 0 || height == 0) return;

                Image = Generator.Generate(out ulong[] imageDataTmp, PosX, PosY, PixelToWorldScale, NumIterations);
                imageData = imageDataTmp;

                stopwatch.Stop();
                RunTime = stopwatch.Elapsed.ToString();
            }
        }

        public Form1()
        {
            InitializeComponent();
            generator = new MandelbrotGenerator(pbCanvas.Width, pbCanvas.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _scene.Add(new Scene(generator));
            Regenerate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var sc = _scene[_currentScene];

            switch (e.KeyCode)
            {
                case Keys.Q:
                {
                    var pixelToWorldScale = sc.PixelToWorldScale / 2;
                    var numIterations = (int) (sc.NumIterations);
                    _scene.Add(new Scene(generator, sc.PosX, sc.PosY, pixelToWorldScale, numIterations));
                    _currentScene++;
                    break;
                }
                case Keys.A:
                {
                    var pixelToWorldScale = sc.PixelToWorldScale * 2;
                    var numIterations = (int)(sc.NumIterations);
                    _scene.Add(new Scene(generator, sc.PosX, sc.PosY, pixelToWorldScale, numIterations));
                    _currentScene++;
                    break;
                }
                case Keys.W:
                {
                    var numIterations = sc.NumIterations * 2;
                        _scene.Add(new Scene(generator, sc.PosX, sc.PosY, sc.PixelToWorldScale, numIterations));
                    _currentScene++;
                    break;
                }
                case Keys.S:
                {
                    var numIterations = sc.NumIterations / 2;
                        _scene.Add(new Scene(generator, sc.PosX, sc.PosY, sc.PixelToWorldScale, numIterations));
                    _currentScene++;
                    break;
                }
                case Keys.Left:
                {
                    if (_currentScene > 0) _currentScene--;
                    break;
                }
                case Keys.Right:
                {
                    if (_currentScene < _scene.Count - 1) _currentScene++;
                    break;
                }
            }
            Regenerate();
        }

        private void pbCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            var width = pbCanvas.Width;
            var height = pbCanvas.Height;
            var sc = _scene[_currentScene];

            var posX = e.X * sc.PixelToWorldScale + sc.PosX - width * sc.PixelToWorldScale / 2;
            var posY = e.Y * sc.PixelToWorldScale + sc.PosY - height * sc.PixelToWorldScale / 2;

            var pixelToWorldScale = (e.Button & MouseButtons.Right) != 0 ? sc.PixelToWorldScale / 2 : sc.PixelToWorldScale;
            //_scene.Add(new Scene(width, height, posX, posY, pixelToWorldScale, sc.NumIterations));
            AnimateToNewScene(posX, posY, pixelToWorldScale, sc.NumIterations);
            _currentScene++;
            Regenerate();
        }

        private void pbCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var sc = _scene[_currentScene];
            label1.Text = $"Time: {sc.RunTime} Pos: {sc.PosX}, {sc.PosY} Scale: {1 / sc.PixelToWorldScale} Iterations: {sc.NumIterations} Value: {sc.imageData[e.X + e.Y * pbCanvas.Width]}";
        }

        private void Regenerate()
        {
            var sc = _scene[_currentScene];
            pbCanvas.Image = sc.Image;
            label1.Text = $"Time: {sc.RunTime} Pos: {sc.PosX}, {sc.PosY} Scale: {1 / sc.PixelToWorldScale} Iterations: {sc.NumIterations}";
        }

        private void AnimateToNewScene(double posX, double posY, double pixelToWorldScale, int numIterations)
        {
            var numFrames = 10;
            var sc = _scene[_currentScene];
            var posXstep = (posX - sc.PosX) / numFrames;
            var posYstep = (posY - sc.PosY) / numFrames;
            var pixelToWorldScaleStep = (pixelToWorldScale - sc.PixelToWorldScale) / numFrames;
            var numIterationsStep = (numIterations - sc.NumIterations) / numFrames;

            var framePosX = sc.PosX;
            var framePosY = sc.PosY;
            var framePixelToWorldScale = sc.PixelToWorldScale;
            var frameNumIterations = sc.NumIterations;
            for (var i = 0; i < numFrames; i++)
            {
                var tempScene = new Scene(generator, framePosX, framePosY, framePixelToWorldScale, frameNumIterations);
                pbCanvas.Image = tempScene.Image;
                pbCanvas.Refresh();
                framePosX += posXstep;
                framePosY += posYstep;
                framePixelToWorldScale += pixelToWorldScaleStep;
                frameNumIterations += numIterationsStep;
                if (i == numFrames-1) _scene.Add(tempScene);
            }
        }
    }
}
