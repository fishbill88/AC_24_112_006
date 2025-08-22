/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using PX.Data;
using PX.Objects.CS;
using PX.Objects.SO;

namespace PX.Objects.FS
{
    public class FSxSOShipment : PXCacheExtension<SOShipment>
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
        }

		#region IsFSRelated
		public abstract class isFSRelated : PX.Data.BQL.BqlBool.Field<isFSRelated> { }
		[PXBool]
		[PXUnboundDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual bool? IsFSRelated { get; set; }
		#endregion

		#region Installed
		public abstract class installed : PX.Data.BQL.BqlBool.Field<installed> { }
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Installed", Enabled = false)]
		[PXUIVisible(typeof(Where<isFSRelated, Equal<True>>))]
		public virtual bool? Installed { get; set; }
		#endregion
	}
}
