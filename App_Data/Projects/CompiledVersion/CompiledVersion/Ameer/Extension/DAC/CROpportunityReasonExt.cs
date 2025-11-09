using PX.Data;
using PX.Data;
using PX.Objects.CR;
using System;

namespace CompiledVersion.DAC
{
    public sealed class CROpportunityReasonExt : PXCacheExtension<CROpportunity>
    {
        public static bool IsActive() => false;

        #region UsrResolution
        public abstract class usrResolution : PX.Data.BQL.BqlString.Field<usrResolution> { }

        [PXString(2, IsFixed = true)]
        [PXUIField(DisplayName = "Reason", Required = false)]
        public string UsrResolution { get; set; }
        #endregion
    }
}


