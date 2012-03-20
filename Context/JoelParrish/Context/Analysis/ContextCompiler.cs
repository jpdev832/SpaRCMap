using System;

namespace JoelParrish.Context.Analysis
{
    public interface ContextCompiler
    {
        void compile(ContextEntity data);
        object getContext();
    }
}
