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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.FS.DAC;
using PX.Objects.SO;
using PX.Objects.SO.DAC.Projections;
using PX.Objects.SO.GraphExtensions.SOInvoiceEntryExt;
using System;
using System.Collections;
using System.Linq;

namespace PX.Objects.FS
{
	public class SM_AddReturnLineToDirectInvoice : PXGraphExtension<AddReturnLineToDirectInvoice, SOInvoiceEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
		}

		public override void Initialize()
		{
			base.Initialize();

			Base1.arTranList.WhereAnd<Where<NotExists<
				SelectFrom<ARTran>
				.Where<ARTranForDirectInvoice.tranType.IsEqual<ARTran.tranType>
					.And<ARTranForDirectInvoice.refNbr.IsEqual<ARTran.refNbr>
					.And<Brackets<FSxARTran.appointmentRefNbr.IsNotNull
						.Or<FSxARTran.serviceOrderRefNbr.IsNotNull>
						.Or<FSxARTran.serviceContractRefNbr.IsNotNull>>>>>>>>();
		}

		[PXOverride]
		public IEnumerable AddARTran(PXAdapter adapter, Func<PXAdapter, IEnumerable> baseMethod)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.advancedSOInvoices>())
			{
				return baseMethod(adapter);
			}

			foreach (ARTranForDirectInvoice origTran in Base1.arTranList.Cache.Updated)
			{
				if (origTran.Selected != true) continue;

				var item = IN.InventoryItem.PK.Find(Base, origTran.InventoryID);

				if (item != null && item.StkItem == true)
				{
					throw new PXException(TX.Error.LineWithStockItemCannotBeAddedBecauseAdvancedSOInvoicesFeatureIsDisabled);
				}
			}

			return baseMethod(adapter);
		}
	}
}
