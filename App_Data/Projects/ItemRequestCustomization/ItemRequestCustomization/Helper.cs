using PX.Data;
using System;

namespace ItemRequestCustomization
{
    public static class Helper
    {
        public static PXAdapter StartLongOperation(this PXGraph graph, PXAdapter adapter, Action method)
        {
            PXLongOperation.StartOperation(graph, delegate ()
            {
                method();
            });

            return adapter;
        }
    }
}
