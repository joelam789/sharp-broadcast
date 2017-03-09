using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBroadcast.Framework
{
    public interface IClientValidator
    {
        string Validate(string clientIp, string requestPath);
    }
}
