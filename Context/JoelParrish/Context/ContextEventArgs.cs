using System;

namespace JoelParrish.Context
{
    public class ContextEventArgs : EventArgs
    {
        public ContextEntity contextEntity;

        public ContextEventArgs(ContextEntity contextEntity)
        {
            this.contextEntity = contextEntity;
        }
    }
}
