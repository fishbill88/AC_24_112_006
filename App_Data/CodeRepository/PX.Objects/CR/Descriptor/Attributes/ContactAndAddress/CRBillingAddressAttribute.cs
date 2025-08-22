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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;

namespace PX.Objects.CR
{
	public class CRBillingAddressAttribute : CRAddressAttribute
	{
		public CRBillingAddressAttribute(Type SelectType)
			: base(typeof(CRBillingAddress.addressID), typeof(CRBillingAddress.isDefaultAddress), SelectType)
		{
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.FieldVerifying.AddHandler<CRBillingAddress.overrideAddress>(Record_Override_FieldVerifying);
			sender.Graph.RowInserted.AddHandler<CRBillingAddress>(CRBillingAddress_RowInserted);
		}


		protected virtual void CRBillingAddress_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			CRBillingAddress billAddress = e.Row as CRBillingAddress;
			if (billAddress == null) return;

			if (billAddress.OverrideAddress == true)
			{
				billAddress.IsValidated = false;
			}
		}

		public override void DefaultRecord(PXCache sender, object DocumentRow, object Row)
		{
			DefaultAddress<CRBillingAddress, CRBillingAddress.addressID>(sender, DocumentRow, Row);
		}
		public override void CopyRecord(PXCache sender, object DocumentRow, object SourceRow, bool clone)
		{
			CopyAddress<CRBillingAddress, CRBillingAddress.addressID>(sender, DocumentRow, SourceRow, clone);
		}
		public override void Record_IsDefault_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
		}

		public virtual void Record_Override_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			var newValue = (e.NewValue == null ? e.NewValue : (bool?)e.NewValue == false);
			// Acuminator disable once PX1044 ChangesInPXCacheInEventHandlers [Justification]
			Address_IsDefaultAddress_FieldVerifying<CRBillingAddress>(sender, new PXFieldVerifyingEventArgs(e.Row, newValue, e.ExternalCall));
		}

		protected override (PXView, object[]) GetViewWithParameters(PXCache sender, object DocumentRow, object ContactRow)
		{
			PXView view = null;
			object[] parm = null;

			if (sender.GetValue<CROpportunity.bAccountID>(DocumentRow) != null)
			{
				var id = sender.GetValue<CROpportunity.bAccountID>(DocumentRow);

				parm = new object[] { id, id };

				BqlCommand Select = new SelectFrom<Address>
					.InnerJoin<BAccount>
						.On<BAccount.bAccountID.IsEqual<Address.bAccountID>>
					.LeftJoin<Customer>
						.On<Customer.bAccountID.IsEqual<Address.bAccountID>
						.And<Customer.defBillAddressID.IsEqual<Address.addressID>>>
					.LeftJoin<CRBillingAddress>
						.On<CRBillingAddress.bAccountID.IsEqual<Address.bAccountID>
						.And<CRBillingAddress.bAccountAddressID.IsEqual<Address.addressID>>
						.And<CRBillingAddress.revisionID.IsEqual<Address.revisionID>>
						.And<CRBillingAddress.isDefaultAddress.IsEqual<True>>>
					.Where<
						Address.bAccountID.IsEqual<@P.AsInt>
						.And<Customer.bAccountID.IsNotNull.Or<BAccount.defAddressID.IsEqual<Address.addressID>>>>();

				view = sender.Graph.TypedViews.GetView(Select, false);
			}

			return (view, parm);
		}
	}
}
