using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SuperCalculator
{
    class DataFifo
    {
        private List<double[]> innerList = new List<double[]>();
        private object syncRoot = new object();
        private ManualResetEvent hasData = new ManualResetEvent(false);

        public void Put( double[] data )
        {
            lock (syncRoot)
            {
                innerList.Add( data );
                hasData.Set();
            }
            innerList.Add( data );
        }

        public bool TryGet(out double[] data)
        {
            data = null;

            if (hasData.WaitOne())
            {
                lock (syncRoot)
                {
                    if (innerList.Count > 0)
                    {
                        // Egy kis mesterséges késleltetés, ami nem befolyásolhatja, 
                        // hogy helyesen mûködik-e a FIFO.
                        data = innerList[0];
                        innerList.RemoveAt(0);
                        if (innerList.Count == 0) hasData.Reset();
                        return true;
                    }
                    else return false;
                }
            }
            return false;
        }
    }
}
