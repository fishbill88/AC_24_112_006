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

using PX.Api;
using PX.Common;
using PX.Data;
using PX.Objects.CA.BankFeed;
using PX.Objects.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.CA.GraphExtensions
{
	public abstract class CABankFeedBase<TGraph, TPrimary> : PXGraphExtension<TGraph>
		where TGraph : PXGraph, new()
		where TPrimary : class, IBqlTable, new()
	{
		[InjectDependency]
		internal Func<string, BankFeedManager> BankFeedManagerProvider
		{
			get;
			set;
		}

		public PXSetup<CASetup> CASetup;

		public PXSelectJoin<CABankFeedDetail, InnerJoin<CABankFeed, On<CABankFeedDetail.bankFeedID, Equal<CABankFeed.bankFeedID>>>,
			Where<CABankFeed.status, In3<CABankFeedStatus.active, CABankFeedStatus.setupRequired>,
				And<CABankFeedDetail.cashAccountID, Equal<Required<CABankFeedDetail.cashAccountID>>>>> BankFeedDetail;

		public PXAction<TPrimary> retrieveTransactions;
		[PXUIField(DisplayName = "Retrieve Transactions", Visible = true)]
		[PXButton(CommitChanges = true)]
		public virtual IEnumerable RetrieveTransactions(PXAdapter adapter)
		{
			var ret = adapter.Get();
			var cashAccountId = GetCashAccountId();
			if (cashAccountId == null) return ret;

			var res = BankFeedDetail.Select(cashAccountId)
				.AsEnumerable()
				.Cast<PXResult<CABankFeedDetail, CABankFeed>>()
				.GroupBy(i => (CABankFeed)i, i => (CABankFeedDetail)i, GetEqualityComparerByField())
				.ToDictionary(i => i.Key, i => i.ToList());

			var feedWithLoginFailed = res.Where(i => i.Key.RetrievalStatus == CABankFeedRetrievalStatus.LoginFailed)
				.Select(i => i.Key).FirstOrDefault();

			if (feedWithLoginFailed != null)
			{
				UpdateFeed(feedWithLoginFailed);
				return ret;
			}

			ImportTransactions(res);

			return ret;
		}

		public PXAction<TPrimary> syncUpdateFeed;
		[PXUIField(DisplayName = "", Visible = false)]
		[PXButton]
		public virtual IEnumerable SyncUpdateFeed(PXAdapter adapter)
		{
			var ret = adapter.Get();
			var formProcRes = adapter.CommandArguments;
			var cashAccountId = GetCashAccountId();
			if (string.IsNullOrEmpty(formProcRes) || cashAccountId == null) return ret;

			PXResult<CABankFeedDetail, CABankFeed> result = (PXResult<CABankFeedDetail, CABankFeed>)BankFeedDetail.Select(cashAccountId);
			CABankFeed bankFeed = result;
			CABankFeedDetail bankfeedDetail = result;

			if (bankFeed == null) return ret;

			var guid = (Guid)Base.UID;
			var importToSingle = CASetup.Current.ImportToSingleAccount == true;
			var manager = GetSpecificManager(bankFeed);
			PXLongOperation.StartOperation(this, () =>
			{
				manager.ProcessUpdateResponseAsync(formProcRes, bankFeed).GetAwaiter().GetResult();
				ImportTransactions(bankFeed, new List<CABankFeedDetail>() { bankfeedDetail }, importToSingle, guid);
			});

			return ret;
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		protected virtual void ImportTransactions(CABankFeed bankFeed, CABankFeedDetail bankFeedDetail)
		{
			var guid = (Guid)Base.UID;
			var importToSingle = CASetup.Current.ImportToSingleAccount == true;
			PXLongOperation.StartOperation(this, () =>
			{
				ImportTransactions(bankFeed, new List<CABankFeedDetail>() { bankFeedDetail }, importToSingle, guid);
			});
		}

		protected virtual void ImportTransactions(IDictionary<CABankFeed, List<CABankFeedDetail>> bankFeedWithDetails)
		{
			var guid = (Guid)Base.UID;
			var importForOneCashAcc = CASetup.Current.ImportToSingleAccount == true
				|| bankFeedWithDetails.Any(i => i.Key.MultipleMapping == true);

			PXLongOperation.StartOperation(this, () =>
			{
				foreach (var kvp in bankFeedWithDetails)
				{
					ImportTransactions(kvp.Key, kvp.Value, importForOneCashAcc, guid);
				}
			});
		}

		protected virtual void UpdateFeed(CABankFeed bankFeed)
		{
			var dialogResult = Base.Views[Base.PrimaryView].Ask(Messages.UpdateBankFeedCredentials, MessageButtons.OKCancel);
			if (dialogResult == WebDialogResult.Cancel) return;
			
			var manager = GetSpecificManager(bankFeed);

			PXLongOperation.StartOperation(this, () =>
			{
				manager.UpdateAsync(bankFeed).GetAwaiter().GetResult();
			});
		}

		protected virtual void _(Events.RowSelected<CABankTran> e)
		{
			var detail = e.Row;
			var cache = e.Cache;
			if (detail == null) return;

			PXUIFieldAttribute.SetEnabled<CABankTranFeedSource.bankFeedAccountMapID>(cache, detail, false);
		}

		protected IEqualityComparer<CABankFeed> GetEqualityComparerByField()
		{
			return new FieldSubsetEqualityComparer<CABankFeed>(Base.Caches[typeof(CABankFeed)], typeof(CABankFeed.bankFeedID));
		}

		private static void ImportTransactions(CABankFeed bankFeed, List<CABankFeedDetail> bankFeedDetail, bool importToOneCashAccount, Guid guid)
		{
			Dictionary<int, string> lastUpdatedStatements;
			if (importToOneCashAccount)
			{
				lastUpdatedStatements = CABankFeedImport.ImportTransactionsForAccounts(bankFeed, bankFeedDetail, guid)
					.GetAwaiter().GetResult();
			}
			else
			{
				lastUpdatedStatements = CABankFeedImport.ImportTransactions(new List<CABankFeed>() { bankFeed }, guid).GetAwaiter().GetResult();
			}

			if (lastUpdatedStatements != null && lastUpdatedStatements.Count() > 0)
			{
				PXLongOperation.SetCustomInfo(new BankFeedRedirectToStatementCustomInfo(lastUpdatedStatements));
			}
		}

		private BankFeedManager GetSpecificManager(CABankFeed bankfeed)
		{
			return BankFeedManagerProvider(bankfeed.Type);
		}

		public abstract int? GetCashAccountId();
	}
}
