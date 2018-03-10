using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Graphics
{
    public class BitmapLock
    {
        private Bitmap _bitmap;
        private BitmapData _bitmapData;
        private IntPtr _ptr;
        private byte[] _rgbValues;
        private int _width, _height, _bytesPerPixel;

        public int Width => _width;

        public int Height => _height;

        public BitmapLock(Bitmap bitmap, ImageLockMode lockMode)
        {
            _bitmap = bitmap;
            _width = _bitmap.Width;
            _height = _bitmap.Height;
            
            _bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _width, _height), lockMode,
                _bitmap.PixelFormat);

            _ptr = _bitmapData.Scan0;
            int bytesCount = Math.Abs(_bitmapData.Stride) * _height;
            _bytesPerPixel = Math.Abs(_bitmapData.Stride) / _width;
            _rgbValues = new byte[bytesCount];
            Marshal.Copy(_ptr, _rgbValues, 0, bytesCount);            
        }

        public Bitmap Release()
        {
            Marshal.Copy(_rgbValues, 0, _ptr, _rgbValues.Length);
            _bitmap.UnlockBits(_bitmapData);
            return _bitmap;
        }

        public Color GetPixel(int x, int y)
        {
            int index = (y * _width + x) * _bytesPerPixel;
            Color color = Color.FromArgb(_rgbValues[index + 3], _rgbValues[index + 2], _rgbValues[index + 1],
                _rgbValues[index]);
            return color;
        }

        public void SetPixel(int x, int y, Color color)
        {
            int index = (y * _width + x) * _bytesPerPixel;
            _rgbValues[index] = color.B;
            _rgbValues[index + 1] = color.G;
            _rgbValues[index + 2] = color.R;
            _rgbValues[index + 3] = color.A;
        }
    }
}
