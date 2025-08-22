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
using PX.Objects.PM;
using System;
using System.Collections;

namespace PX.Objects.CN.ProjectAccounting.AR.GraphExtensions
{
	public class ARPaymentEntryExt : PXGraphExtension<ARPaymentEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.construction>();
		}

		protected virtual void ARAdjust_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			ARAdjust adjustment = (ARAdjust)e.Row;

			PMProforma proforma = PXSelect<PMProforma, Where<PMProforma.aRInvoiceDocType, Equal<Required<ARAdjust.adjdDocType>>,
				And<PMProforma.aRInvoiceRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>>>>.Select(Base, adjustment.AdjdDocType, adjustment.AdjdRefNbr);

			if (proforma != null && proforma.Corrected == true && proforma.Status != ProformaStatus.Closed)
			{
				if (Base.Document.Current.DocType == ARDocType.CreditMemo)
				{
					sender.RaiseExceptionHandling<ARAdjust.adjdRefNbr>(adjustment, adjustment.AdjdRefNbr, new PXSetPropertyException(PX.Objects.PM.Messages.CannotReverseInvoice, adjustment.AdjdRefNbr, proforma.RefNbr));
				}
			}
		}
			
	}
}
