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
using PX.Objects.AR.CCPaymentProcessing.Interfaces;
using PX.Objects.AR.CCPaymentProcessing.Repositories;
using PX.Objects.CC.PaymentProcessing;
using PX.Objects.CC.Utility;
using PX.Objects.Extensions.PaymentTransaction;
using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.CC.GraphExtensions
{
	public abstract class Level3Graph<TGraph, TPrimary, TDerived> : PXGraphExtension<TGraph>
		where TGraph : PXGraph, new()
		where TPrimary : class, IBqlTable, new()
		where TDerived : Level3Graph<TGraph, TPrimary, TDerived>
	{
		public PXSelectExtension<Payment> PaymentDoc;
		public PXSelectExtension<ExternalTransactionDetail> ExternalTransaction;

		private ICCPaymentProcessingRepository _repo;

		public PXAction<TPrimary> updateLevel3DataCCPayment;
		[PXUIField(DisplayName = "Send Level 3 Data", Visible = false, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		[ARMigrationModeDependentActionRestriction(
			restrictInMigrationMode: true,
			restrictForRegularDocumentInMigrationMode: true,
			restrictForUnreleasedMigratedDocumentInNormalMode: true)]
		public virtual IEnumerable UpdateLevel3DataCCPayment(PXAdapter adapter)
		{
			Level3Processing level3Processing = new Level3Processing(GetPaymentProcessingRepo());
			TGraph docGraph = PXGraph.CreateInstance<TGraph>();
			TDerived level3Graph = docGraph.GetExtension<TDerived>();
			level3Graph.PaymentDoc.Current = PaymentDoc.Current;			
			PXLongOperation.StartOperation(this, delegate ()
			{
				try
				{
					ARPayment arPayment = ARPayment.PK.Find(level3Graph.Base, level3Graph.PaymentDoc.Current.DocType, level3Graph.PaymentDoc.Current.RefNbr);
					level3Processing.UpdateLevel3Data(level3Graph.PaymentDoc.Current, arPayment.CCActualExternalTransactionID);
				}
				catch
				{
					throw;
				}
			});

			return adapter.Get();
		}

		public IEnumerable<ExternalTransactionDetail> GetExtTranDetails()
		{
			if (ExternalTransaction == null)
				yield break;
			foreach (ExternalTransactionDetail tran in ExternalTransaction.Select().RowCast<ExternalTransactionDetail>())
			{
				yield return tran;
			}
		}

		public IEnumerable<IExternalTransaction> GetExtTrans()
		{
			foreach (IExternalTransaction tran in GetExtTranDetails())
			{
				yield return tran;
			}
		}

		protected virtual ICCPaymentProcessingRepository GetPaymentProcessingRepo()
		{
			if (_repo == null)
			{
				_repo = CCPaymentProcessingRepository.GetCCPaymentProcessingRepository();
			}
			return _repo;
		}

		protected abstract PaymentMapping GetPaymentMapping();
		protected abstract ExternalTransactionDetailMapping GetExternalTransactionMapping();
	}
}
