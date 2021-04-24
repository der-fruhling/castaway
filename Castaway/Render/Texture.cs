using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Castaway.Assets;
using Castaway.Native;

namespace Castaway.Render
{
    public class BoundTexture : IDisposable
    {
        public readonly uint BindLocation;
        public readonly uint Texture;

        public BoundTexture(uint bindLocation, uint texture)
        {
            BindLocation = bindLocation;
            Texture = texture;
            GL.BindTexture(BindLocation, Texture);
            ShaderManager.ActiveHandle.SetProperty("Textures[IsActive]", 1);
        }

        public void Dispose()
        {
            ShaderManager.ActiveHandle.SetProperty("Textures[IsActive]", 0);
            GL.BindTexture(BindLocation, 0);
        }
    }
    
    public unsafe class Texture
    {
        private readonly Bitmap _image;
        private uint _tex;

        public bool IsSetUp { get; private set; }
        
        public Texture(string path)
        {
            using var s = File.OpenRead(path);
            _image = new Bitmap(s);
        }

        public Texture(int width, int height, Color color)
        {
            _image = new Bitmap(width, height);
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    _image.SetPixel(i, j, color);
        }

        public void Setup(bool filterNearest = false, bool rgba = true)
        {
            var pixels = new float[_image.Width * _image.Height * (rgba ? 4 : 3)];
            for (var i = 0; i < _image.Width; i++)
            {
                for (var j = 0; j < _image.Height; j++)
                {
                    var l = (_image.Width * j + i) * (rgba ? 4 : 3);
                    var c = _image.GetPixel(i, j);
                    pixels[l + 0] = c.R / 256f;
                    pixels[l + 1] = c.G / 256f;
                    pixels[l + 2] = c.B / 256f;
                    if(rgba) pixels[l + 3] = c.A;
                }
            }
            fixed(uint* p = &_tex) GL.GenTextures(1, p);
            GL.BindTexture(GL.TEXTURE_2D, _tex);
            GL.TextureParam(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.REPEAT);
            GL.TextureParam(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.REPEAT);
            GL.TextureParam(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, filterNearest ? GL.NEAREST : GL.LINEAR);
            GL.TextureParam(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, filterNearest ? GL.NEAREST : GL.LINEAR);
            fixed(float* p = pixels)
                GL.LoadTexture2D(GL.TEXTURE_2D, 0, GL.RGBA, (uint) _image.Width,
                    (uint) _image.Height, 0, rgba ? GL.RGBA : GL.RGB, GL.FLOAT, p);
            IsSetUp = true;
        }

        public BoundTexture Use() => new BoundTexture(GL.TEXTURE_2D, _tex);

        public void Bind(uint to)
        {
            if (to >= 32) throw new ApplicationException("OpenGL only has 32 other texture slots.");
            GL.BindTexture(GL.TEXTURE0 + to, _tex);
        }

        public class Loader : IAssetLoader
        {
            public IEnumerable<string> FileExtensions { get; } = new[] {
                "bmp", "gif", "exif", "jpg", "jpeg", "png",
                "tiff", "tif"};

            public object LoadFile(string path)
            {
                return new Texture(path);
            }
        }
    }
}