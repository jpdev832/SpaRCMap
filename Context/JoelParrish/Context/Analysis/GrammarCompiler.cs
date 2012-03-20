using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoelParrish.Context.Analysis
{
    public class GrammarCompiler : ContextCompiler
    {

        #region ContextCompiler Members

        public void compile(ContextEntity data)
        {
            string id = data.uid;

            //query against id
            //build grammar
        }

        public object getContext()
        {
            throw new NotImplementedException();
            //return grammar
        }

        #endregion
    }
}
