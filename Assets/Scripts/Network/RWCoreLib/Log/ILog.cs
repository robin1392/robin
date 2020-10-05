using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWCoreLib.Log
{
    public interface ILog
    {
        void Info(string log);
        void Fatal(string log);
        void Error(string log);
        void Warn(string log);
        void Debug(string log);
    }
}
