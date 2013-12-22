using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Srk.BetaSeriesApi
{
    public class TrackedQuery : IDisposable
    {

        public TrackedQuery(Action callback)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        internal void PreSet(string action, Dictionary<string, string> parameters, Dictionary<string, string> postParameters)
        {
            throw new NotImplementedException();
        }

        internal void PostSet()
        {
            throw new NotImplementedException();
        }
    }
}
