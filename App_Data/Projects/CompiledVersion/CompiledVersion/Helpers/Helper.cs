using PX.Data;
using System;

namespace CompiledVersion.Helpers
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
