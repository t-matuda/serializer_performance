using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utf8JsonTest
{
    public class SwEx : IDisposable
    {
        private Stopwatch _sw;
        private string _msg;

        public SwEx(string msg)
        {
            _sw = Stopwatch.StartNew();
            _msg = msg;
        }

        public void Dispose()
        {
            _sw.Stop();
            Console.WriteLine("{0},{1}[ms]", _msg, _sw.ElapsedMilliseconds);
        }
    }
}
