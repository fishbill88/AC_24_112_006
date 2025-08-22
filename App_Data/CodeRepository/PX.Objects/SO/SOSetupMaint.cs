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

using System.Collections;
using System.Collections.Generic;

using PX.Data;

using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;

namespace PX.Objects.SO
{
	public class SOSetupMaint : PXGraph<SOSetupMaint> // SO101000
	{
		public PXSave<SOSetup> Save;
		public PXCancel<SOSetup> Cancel;

		public PXSelect<SOSetup> sosetup;

		#region Well-known Extensions

		// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
		public class ApprovalsExtension : PXGraphExtension<SOSetupMaint> // corresponds to the "Approval" tab
		{
			//public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>();

			#region Views
			public PXSelect<SOSetupApproval> SetupApproval;
			public PXSelect<SOSetupInvoiceApproval> SetupInvoiceApproval;
			#endregion

			#region Event Handlers
			protected virtual void _(Events.RowInserted<SOSetup> e)
			{
				if (e.Row?.OrderRequestApproval != null)
					SyncOrderApprovalsWithOrderRequestApprovalFlag(e.Row.OrderRequestApproval);
			}

			protected virtual void _(Events.RowUpdated<SOSetup> e)
			{
				if (!e.Cache.ObjectsEqual<SOSetup.orderRequestApproval>(e.Row, e.OldRow))
					SyncOrderApprovalsWithOrderRequestApprovalFlag(e.Row.OrderRequestApproval);
			}

			protected virtual void _(Events.RowPersisting<SOSetup> e)
			{
				var checkIntercompanyFields = PXAccess.FeatureInstalled<FeaturesSet.interBranch>() ? PXPersistingCheck.Null
					: PXPersistingCheck.Nothing;
				PXDefaultAttribute.SetPersistingCheck<SOSetup.dfltIntercompanyOrderType>(e.Cache, e.Row, checkIntercompanyFields);
				PXDefaultAttribute.SetPersistingCheck<SOSetup.dfltIntercompanyRMAType>(e.Cache, e.Row, checkIntercompanyFields);
			}

			protected virtual void _(Events.RowInserted<SOSetupApproval> e)
			{
				SyncOrderRequestApprovalFlagWithOrderApprovals(e.Row);
			}

			protected virtual void _(Events.RowUpdated<SOSetupApproval> e)
			{
				SyncOrderRequestApprovalFlagWithOrderApprovals(e.Row);
			}
			#endregion

			private void SyncOrderApprovalsWithOrderRequestApprovalFlag(bool? newOrderRequestApproval)
			{
				PXResultset<SOSetupApproval> orderApprovals = PXSelect<SOSetupApproval>.Select(Base, null);
				foreach (SOSetupApproval approval in orderApprovals)
				{
					approval.IsActive = newOrderRequestApproval;
					SetupApproval.Update(approval);
				}
			}

			private void SyncOrderRequestApprovalFlagWithOrderApprovals(SOSetupApproval approval)
			{
				var primaryView = Base.sosetup;
				if (approval.IsActive == true && primaryView.Current.OrderRequestApproval != true)
				{
					primaryView.Current.OrderRequestApproval = true;
					primaryView.UpdateCurrent();
				}
			}
		}

		// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
		public class NotificationSetupExtension : PXGraphExtension<SOSetupMaint> // corresponds to the "Mailing & Printing" tab
		{
			#region Views
			public CRNotificationSetupList<SONotification> Notifications;

			public
				PXSelect<NotificationSetupRecipient,
				Where<NotificationSetupRecipient.setupID, Equal<Current<SONotification.setupID>>>>
				Recipients;
			#endregion

			#region DAC Overrides
			[PXDBString(10)]
			[PXDefault]
			[VendorContactType.ClassList]
			[PXUIField(DisplayName = "Contact Type")]
			[PXCheckDistinct(typeof(NotificationSetupRecipient.contactID),
				Where = typeof(Where<NotificationSetupRecipient.setupID, Equal<Current<NotificationSetupRecipient.setupID>>>))]
			protected virtual void _(Events.CacheAttached<NotificationSetupRecipient.contactType> e) { }

			[PXDBInt]
			[PXUIField(DisplayName = "Contact ID")]
			[PXNotificationContactSelector(typeof(NotificationSetupRecipient.contactType), typeof(
				Search2<Contact.contactID,
				LeftJoin<EPEmployee, On<
					EPEmployee.parentBAccountID, Equal<Contact.bAccountID>,
					And<EPEmployee.defContactID, Equal<Contact.contactID>>>>,
				Where<
					Current<NotificationSetupRecipient.contactType>, Equal<NotificationContactType.employee>,
					And<EPEmployee.acctCD, IsNotNull>>>))]
			protected virtual void _(Events.CacheAttached<NotificationSetupRecipient.contactID> e) { }
			#endregion
		}

		// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
		public class PickPackShipExtension : PXGraphExtension<SOSetupMaint> // corresponds to the "Warehouse Management" tab
		{
			//public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.wMSFulfillment>();

			#region Views
			public
				PXSelect<SOPickPackShipSetup,
				Where<SOPickPackShipSetup.branchID, Equal<Current<AccessInfo.branchID>>>>
				PickPackShipSetup;
			protected virtual IEnumerable pickPackShipSetup() => EnsurePickPackShipSetup();

			public
				PXSelect<SOPickPackShipUserSetup,
				Where<SOPickPackShipUserSetup.isOverridden, Equal<False>>>
				PickPackShipUserSetups;
			#endregion

			#region Event Handlers
			protected virtual void _(Events.FieldVerifying<SOPickPackShipSetup, SOPickPackShipSetup.showPickTab> e)
			{
				if ((bool)e.NewValue != true && PickPackShipSetup.Current?.ShowPackTab != true &&
					PickPackShipSetup.Current?.ShowShipTab != true) e.NewValue = true;
			}

			protected virtual void _(Events.FieldVerifying<SOPickPackShipSetup, SOPickPackShipSetup.showPackTab> e)
			{
				if ((bool)e.NewValue != true && PickPackShipSetup.Current?.ShowPickTab != true &&
					PickPackShipSetup.Current?.ShowShipTab != true) e.NewValue = true;
			}

			protected virtual void _(Events.FieldVerifying<SOPickPackShipSetup, SOPickPackShipSetup.showShipTab> e)
			{
				if ((bool)e.NewValue != true && PickPackShipSetup.Current?.ShowPickTab != true &&
					PickPackShipSetup.Current?.ShowPackTab != true) e.NewValue = true;
			}

			protected virtual void _(Events.RowUpdated<SOPickPackShipSetup> e)
			{
				if (e.Row != null)
					foreach (SOPickPackShipUserSetup userSetup in PickPackShipUserSetups.Select())
						PickPackShipUserSetups.Update(userSetup.ApplyValuesFrom(e.Row));
			}

			protected virtual void _(Events.RowSelected<SOPickPackShipSetup> e)
			{
				if (e.Row != null && e.Row.ShowPickTab == false && PXAccess.FeatureInstalled<FeaturesSet.wMSAdvancedPicking>())
					e.Cache.RaiseExceptionHandling<SOPickPackShipSetup.showPickTab>(e.Row, e.Row.ShowPickTab, new PXSetPropertyException(Messages.CannotPickWaveBatchWorksheets, PXErrorLevel.Warning));
			}
			#endregion

			private IEnumerable<SOPickPackShipSetup> EnsurePickPackShipSetup()
			{
				SOPickPackShipSetup result = new PXSelect<SOPickPackShipSetup,
					Where<SOPickPackShipSetup.branchID, Equal<Current<AccessInfo.branchID>>>>(Base).Select();

				if (result == null) result = PickPackShipSetup.Insert();

				return new SOPickPackShipSetup[] { result };
			}
		}
		#endregion
	}
}
