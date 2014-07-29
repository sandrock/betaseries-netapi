
namespace Srk.BetaseriesApiFactory.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TestApiFactory : ApiFactory
    {
        [DebuggerStepThrough]
        internal Response DoParseReponseFormat(string pre)
        {
            return base.ParseReponseFormat(pre);
        }

        [DebuggerStepThrough]
        internal void DoWriteEntities(ApiFactoryContext context, TextWriter text)
        {
            base.WriteEntities(context, text);
        }

        [DebuggerStepThrough]
        internal void DoWriteService(ApiFactoryContext context, TextWriter text)
        {
            base.WriteService(context, text);
        }
    }
}
