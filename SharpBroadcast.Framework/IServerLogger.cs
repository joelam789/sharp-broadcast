using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBroadcast.Framework
{
    public interface IServerLogger
    {
        void Info(string msg);

        void Debug(string msg);

        void Warn(string msg);

        void Error(string msg);
    }
}
