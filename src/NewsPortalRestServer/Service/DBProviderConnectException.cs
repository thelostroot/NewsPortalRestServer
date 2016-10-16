using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Service
{
    public class DBProviderConnectException : Exception
    {
        public DBProviderConnectException(string message) : base(message) { }
    }
}
