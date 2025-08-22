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
using PX.Objects.FS;
using System;

namespace PX.Objects.PM.TaxZoneExtension
{
	public class AppointmentEntryExt : ProjectRevenueTaxZoneExtension<AppointmentEntry>
	{
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(typeof(Default<FSAppointment.projectID>))]
		protected virtual void _(Events.CacheAttached<FSAppointment.taxZoneID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Replace)]
		[PXDBInt]
		[FSSrvOrdAddress2(typeof(Select<
			CR.Address,
			Where<True, Equal<False>>>))]
		protected virtual void _(Events.CacheAttached<FSServiceOrder.serviceOrderAddressID> e)
		{
		}

		public static bool IsActive()
		{
			if (!PXAccess.FeatureInstalled<FeaturesSet.projectAccounting>())
				return false;

			ProjectSettingsManager settings = new ProjectSettingsManager();
			return settings.CalculateProjectSpecificTaxes;
		}

		protected override DocumentMapping GetDocumentMapping()
		{
			return new DocumentMapping(typeof(FSAppointment))
			{
				ProjectID = typeof(FSAppointment.projectID)
			};
		}

		protected override void SetDefaultShipToAddress(PXCache sender, Document row)
		{
			var graph = (AppointmentEntry)sender?.Graph;
			if (graph == null || row == null || graph.ServiceOrderRelated == null) return;

			// To avoid redefaulting when the Appointment is generated from other documents. Eg. Service Order, etc.
			if (graph.ServiceOrder_Address?.Current == null) return;  

			graph.ServiceOrderRelated.Current.ProjectID = row.ProjectID;
			FSSrvOrdAddress2Attribute.DefaultRecord<FSServiceOrder.serviceOrderAddressID>(graph.ServiceOrderRelated.Cache, graph.ServiceOrderRelated.Current);
		}

		[Obsolete("This method is obsolete and will be removed in future versions.")]
		[PXOverride]
		public virtual string GetDefaultTaxZone(FSAppointment row, Func<FSAppointment, string> baseMethod)
		{
			PMProject project = PMProject.PK.Find(Base, row.ProjectID);
			if (project != null && !string.IsNullOrEmpty(project.RevenueTaxZoneID))
			{
				return project.RevenueTaxZoneID;
			}
			else
			{
				return baseMethod(row);
			}
		}

		[PXOverride]
		public virtual string GetDefaultTaxZone(int? billCustomerID, int? billLocationID, int? branchID, int? projectID, Func<int?, int?, int?, int?, string> baseMethod)
		{
			PMProject project = PMProject.PK.Find(Base, projectID);
			if (project != null && !string.IsNullOrEmpty(project.RevenueTaxZoneID))
			{
				return project.RevenueTaxZoneID;
			}
			else
			{
				return baseMethod(billCustomerID, billLocationID, branchID, projectID);
			}
		}
	}
}
