using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using System;

namespace PX.Objects.CR
{
    public sealed class CROpportunityExt2 : PXCacheExtension<CROpportunity>
    {
        public static bool IsActive() => true;

        #region UsrReferralSource
        public abstract class usrReferralSource : BqlString.Field<usrReferralSource> { }

        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Referral Source")]
        public string UsrReferralSource { get; set; }
        #endregion

        #region UsrReleaseHold
        public abstract class usrReleaseHold : BqlDateTime.Field<usrReleaseHold> { }

        [PXDBDate]
        [PXUIField(DisplayName = "Release Hold Date")]
        public DateTime? UsrReleaseHold { get; set; }
        #endregion

        #region UsrServicesEstimate
        public abstract class usrServicesEstimate : BqlDecimal.Field<usrServicesEstimate> { }

        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Services Estimate")]
        public Decimal? UsrServicesEstimate { get; set; }
        #endregion

        #region UsrActivityNote
        public abstract class usrActivityNote : BqlString.Field<usrActivityNote> { }

        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Activity Note")]
        public string UsrActivityNote { get; set; }
        #endregion
    }
}
