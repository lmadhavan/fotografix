﻿namespace Fotografix.Editor
{
    public interface ICommandDispatcher
    {
        void Dispatch<T>(T command);
    }
}