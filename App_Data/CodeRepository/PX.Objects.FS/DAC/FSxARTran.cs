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
using System;

namespace PX.Objects.FS.DAC
{
	// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
	public sealed class FSxARTran : PXCacheExtension<ARTran>, IFSRelatedDoc
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
		}

		#region SrvOrdType
		public abstract class srvOrdType : PX.Data.BQL.BqlString.Field<srvOrdType> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(4, IsFixed = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Order Type", FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string SrvOrdType { get; set; }
		#endregion
		#region AppointmentRefNbr
		public abstract class appointmentRefNbr : PX.Data.BQL.BqlString.Field<appointmentRefNbr> { }

		/// <summary>
		/// Appointment ref nbr
		/// </summary>
		[PXDBString(20, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Appointment Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string AppointmentRefNbr { get; set; }
		#endregion
		#region AppointmentLineNbr
		public abstract class appointmentLineNbr : PX.Data.BQL.BqlInt.Field<appointmentLineNbr> { }

		/// <summary>
		/// Appointment line nbr
		/// </summary>
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Appointment Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public Int32? AppointmentLineNbr { get; set; }
		#endregion
		#region ServiceOrderRefNbr
		public abstract class serviceOrderRefNbr : PX.Data.BQL.BqlString.Field<serviceOrderRefNbr> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(15, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Order Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string ServiceOrderRefNbr { get; set; }
		#endregion
		#region ServiceOrderLineNbr
		public abstract class serviceOrderLineNbr : PX.Data.BQL.BqlInt.Field<serviceOrderLineNbr> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Order Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public Int32? ServiceOrderLineNbr { get; set; }
		#endregion
		#region ServiceContractRefNbr
		public abstract class serviceContractRefNbr : PX.Data.BQL.BqlString.Field<serviceContractRefNbr> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(15, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Contract Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string ServiceContractRefNbr { get; set; }
		#endregion
		#region ServiceContractPeriodID
		public abstract class serviceContractPeriodID : PX.Data.BQL.BqlInt.Field<serviceContractPeriodID> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Contract Period", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public int? ServiceContractPeriodID { get; set; }
		#endregion

		#region Equipment Customization
		#region EquipmentAction
		public abstract class equipmentAction : ListField_EquipmentAction
		{
		}

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(2, IsFixed = true)]
		[equipmentAction.ListAtrribute]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Equipment Action", Visible = false, FieldClass = FSSetup.EquipmentManagementFieldClass, Enabled = false)]
		public string EquipmentAction { get; set; }
		#endregion
		#region ReplaceSMEquipmentID
		public abstract class replaceSMEquipmentID : PX.Data.BQL.BqlInt.Field<replaceSMEquipmentID> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBInt]
		[PXUIField(DisplayName = "Suspended Target Equipment ID", FieldClass = FSSetup.EquipmentManagementFieldClass, Enabled = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<FSEquipment.SMequipmentID>), SubstituteKey = typeof(FSEquipment.refNbr))]
		public int? ReplaceSMEquipmentID { get; set; }
		#endregion
		#region SMEquipmentID
		public abstract class sMEquipmentID : PX.Data.BQL.BqlInt.Field<sMEquipmentID> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBInt]
		[PXUIField(DisplayName = "Target Equipment ID", Visible = false, FieldClass = FSSetup.EquipmentManagementFieldClass, Enabled = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[FSSelectorExtensionMaintenanceEquipment(typeof(ARTran.customerID))]
		public int? SMEquipmentID { get; set; }
		#endregion
		#region NewEquipmentLineNbr
		public abstract class newEquipmentLineNbr : PX.Data.BQL.BqlInt.Field<newEquipmentLineNbr> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBInt]
		[PXUIField(DisplayName = "Model Equipment Line Nbr.", Visible = false, FieldClass = FSSetup.EquipmentManagementFieldClass, Enabled = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[FSSelectorNewTargetEquipmentSOInvoice(ValidateValue = false)]
		public int? NewEquipmentLineNbr { get; set; }
		#endregion
		#region ComponentID
		public abstract class componentID : PX.Data.BQL.BqlInt.Field<componentID> { }

		/// <summary>
		/// Component ID
		/// </summary>
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Component ID", FieldClass = FSSetup.EquipmentManagementFieldClass, Enabled = false)]
		[PXSelector(typeof(Search<FSModelTemplateComponent.componentID>), SubstituteKey = typeof(FSModelTemplateComponent.componentCD))]
		public int? ComponentID { get; set; }
		#endregion
		#region EquipmentComponentLineNbr
		public abstract class equipmentComponentLineNbr : PX.Data.BQL.BqlInt.Field<equipmentComponentLineNbr> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Component Line Nbr.", Visible = false, FieldClass = FSSetup.EquipmentManagementFieldClass, Enabled = false)]
		[FSSelectorEquipmentLineRefSOInvoice]
		public int? EquipmentComponentLineNbr { get; set; }
		#endregion
		#region Comment
		public abstract class comment : PX.Data.BQL.BqlString.Field<comment> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(int.MaxValue, IsUnicode = false)]
		[PXUIField(DisplayName = "Equipment Action Comment", FieldClass = FSSetup.EquipmentManagementFieldClass, Visible = false, Enabled = false)]
		[SkipSetExtensionVisibleInvisible]
		public string Comment { get; set; }
		#endregion
		#endregion

		#region RelatedDocument
		public abstract class relatedDocument : PX.Data.BQL.BqlString.Field<relatedDocument> { }

		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[FSRelatedDocument(typeof(ARTran))]
		[PXUIField(DisplayName = "Related Svc. Doc. Nbr.", Enabled = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
		public string RelatedDocument { get; set; }
		#endregion

		#region IsFSRelated
		public abstract class isFSRelated : PX.Data.BQL.BqlString.Field<isFSRelated> { }

		/// <summary>
		/// Is FS related
		/// </summary>
		[PXBool]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public bool? IsFSRelated
		{
			get
			{
				return string.IsNullOrEmpty(AppointmentRefNbr) == false
					|| string.IsNullOrEmpty(ServiceOrderRefNbr) == false
					|| string.IsNullOrEmpty(ServiceContractRefNbr) == false;
			}
		}
		#endregion
	}
}
