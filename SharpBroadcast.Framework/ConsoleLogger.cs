using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class ConsoleLogger : IServerLogger
    {
        public void Info(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Debug(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Warn(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Error(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
