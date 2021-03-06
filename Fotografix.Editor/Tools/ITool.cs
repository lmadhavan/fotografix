﻿namespace Fotografix.Editor.Tools
{
    public interface ITool : IPointerEventListener
    {
        string Name { get; }

        void Activated(Image image);
        void Deactivated();
    }
}
