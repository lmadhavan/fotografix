using System;
using System.Drawing;

namespace Fotografix
{
    public interface IBitmap : IDisposable
    {
        Size Size { get; }
    }
}