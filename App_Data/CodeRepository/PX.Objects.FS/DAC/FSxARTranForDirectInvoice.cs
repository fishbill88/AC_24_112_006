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
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.SO.DAC.Projections;
using System;

namespace PX.Objects.FS.DAC
{
	// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
	public sealed class FSxARTranForDirectInvoice : PXCacheExtension<ARTranForDirectInvoice>, IFSRelatedDoc
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
		}

		#region RelatedDocument
		public abstract class relatedDocument : PX.Data.BQL.BqlString.Field<relatedDocument> { }

		/// <inheritdoc cref="FSxARTran.RelatedDocument"/>
		[FSRelatedDocument(typeof(ARTran))]
		[PXUIField(DisplayName = "Related Document", Enabled = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string RelatedDocument { get; set; }
		#endregion

		#region SrvOrdType
		public abstract class srvOrdType : PX.Data.BQL.BqlString.Field<srvOrdType> { }

		/// <inheritdoc cref="FSxARTran.SrvOrdType"/>
		[PXDBString(4, IsFixed = true, BqlField = typeof(FSxARTran.srvOrdType))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Order Type", FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string SrvOrdType { get; set; }
		#endregion

		#region ServiceOrderRefNbr
		public abstract class serviceOrderRefNbr : PX.Data.BQL.BqlString.Field<serviceOrderRefNbr> { }

		/// <inheritdoc cref="FSxARTran.ServiceOrderRefNbr"/>
		[PXDBString(15, IsUnicode = true, BqlField = typeof(FSxARTran.serviceOrderRefNbr))]
		[PXUIField(DisplayName = "Service Order Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string ServiceOrderRefNbr { get; set; }
		#endregion

		#region ServiceOrderLineNbr
		public abstract class serviceOrderLineNbr : PX.Data.BQL.BqlInt.Field<serviceOrderLineNbr> { }

		/// <inheritdoc cref="FSxARTran.ServiceOrderLineNbr"/>
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Order Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public Int32? ServiceOrderLineNbr { get; set; }
		#endregion

		#region AppointmentRefNbr
		public abstract class appointmentRefNbr : PX.Data.BQL.BqlString.Field<appointmentRefNbr> { }

		/// <inheritdoc cref="FSxARTran.AppointmentRefNbr"/>
		[PXDBString(20, IsUnicode = true, BqlField = typeof(FSxARTran.appointmentRefNbr))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Appointment Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string AppointmentRefNbr { get; set; }
		#endregion

		#region AppointmentLineNbr
		public abstract class appointmentLineNbr : PX.Data.BQL.BqlInt.Field<appointmentLineNbr> { }

		/// <inheritdoc cref="FSxARTran.AppointmentLineNbr"/>
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Appointment Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public Int32? AppointmentLineNbr { get; set; }
		#endregion

		#region ServiceContractRefNbr
		public abstract class serviceContractRefNbr : PX.Data.BQL.BqlString.Field<serviceContractRefNbr> { }

		/// <inheritdoc cref="FSxARTran.ServiceContractRefNbr"/>
		[PXDBString(15, IsUnicode = true, BqlField = typeof(FSxARTran.serviceContractRefNbr))]
		[PXUIField(DisplayName = "Service Contract Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string ServiceContractRefNbr { get; set; }
		#endregion

		#region ServiceContractPeriodID
		public abstract class serviceContractPeriodID : PX.Data.BQL.BqlInt.Field<serviceContractPeriodID> { }

		/// <inheritdoc cref="FSxARTran.ServiceContractPeriodID"/>
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Contract Period", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public int? ServiceContractPeriodID { get; set; }
		#endregion
	}
}
