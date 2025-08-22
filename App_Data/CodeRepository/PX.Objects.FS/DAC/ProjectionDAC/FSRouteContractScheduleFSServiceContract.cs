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

using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.FS
{
	/// <exclude/>    // Acuminator disable once PX1094 NoPXHiddenOrPXCacheNameOnDac - legacy code
	// Acuminator disable once PX1094 NoPXHiddenOrPXCacheNameOnDac - legacy code
	[Serializable]
    [PXProjection(typeof(
		Select2<FSRouteContractSchedule,
		InnerJoin<FSServiceContract,
			On<FSRouteContractSchedule.entityID, Equal<FSServiceContract.serviceContractID>,
			And<FSRouteContractSchedule.entityType, Equal<FSRouteContractSchedule.entityType.Contract>>>,
		InnerJoin<Customer,
			On<Customer.bAccountID, Equal<FSServiceContract.customerID>>,
		LeftJoin<Location,
			On<Location.locationID, Equal<FSServiceContract.customerLocationID>>>>>,
		Where<FSServiceContract.recordType, Equal<FSServiceContract.recordType.RouteServiceContract>,
			And<FSServiceContract.status, Equal<FSServiceContract.status.Active>>>>))]
	[PXGroupMask(typeof(InnerJoin<AR.Customer, On<AR.Customer.bAccountID, Equal<FSRouteContractScheduleFSServiceContract.customerID>, And<Match<AR.Customer, Current<AccessInfo.userName>>>>>))]
	public class FSRouteContractScheduleFSServiceContract : FSRouteContractSchedule
    {
		#region CustomerLocationID
		public new abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }

		[LocationID(typeof(
						Where<
							Location.bAccountID, Equal<Optional<customerID>>,
							And<Location.isActive, Equal<True>, And<MatchWithBranch<Location.cBranchID>>>>),
					DescriptionField = typeof(Location.descr),
					Visibility = PXUIVisibility.SelectorVisible,
					CacheGlobal = true,
					BqlField = typeof(FSServiceContract.customerLocationID))]
		public override int? CustomerLocationID { get; set; }
		#endregion

		#region ServiceContractRefNbr
		public abstract class serviceContractRefNbr : PX.Data.BQL.BqlString.Field<serviceContractRefNbr> { }

        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(FSServiceContract.refNbr))]
		[PXUIField(DisplayName = "Service Contract ID", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true)]
		[PXSelector(
			typeof(Search<FSServiceContract.refNbr,
				   Where<
					   FSServiceContract.recordType, Equal<ListField_RecordType_ContractSchedule.RouteServiceContract>>,
				   OrderBy<Desc<FSServiceContract.refNbr>>>))]
		[AutoNumber(typeof(Search<FSSetup.serviceContractNumberingID>), typeof(AccessInfo.businessDate))]
		public virtual string ServiceContractRefNbr { get; set; }
		#endregion

		#region CustomerContractNbr
		public abstract class customerContractNbr : PX.Data.IBqlField
		{
		}

		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(FSServiceContract.customerContractNbr))]
        [PXUIField(DisplayName = "Customer Contract Nbr.")]
        public virtual string CustomerContractNbr { get; set; }
        #endregion

        #region DocDesc
        public abstract class docDesc : PX.Data.BQL.BqlString.Field<docDesc> { }

        [PXDBString(255, IsUnicode = true, BqlField = typeof(FSServiceContract.docDesc))]
        [PXUIField(DisplayName = "Description")]
        public virtual string DocDesc { get; set; }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Schedule ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(
			typeof(Search<FSRouteContractScheduleFSServiceContract.refNbr,
				   Where<
						FSRouteContractScheduleFSServiceContract.entityID, Equal<Current<FSRouteContractScheduleFSServiceContract.entityID>>,
						And<FSRouteContractScheduleFSServiceContract.entityType, Equal<FSRouteContractScheduleFSServiceContract.entityType.Contract>>>,
				   OrderBy<Desc<FSRouteContractScheduleFSServiceContract.refNbr>>>),
			CacheGlobal = true)]
		public override string RefNbr { get; set; }
		#endregion

		#region FormCaptionDescription
		[PXString]
		public override string FormCaptionDescription { get; set; }
		#endregion

	}
}
