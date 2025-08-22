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

using PX.Commerce.Amazon.Amazon.DAC;
using PX.Commerce.Amazon.API;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Client.Interface;
using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Amazon.API.Rest.Interfaces;
using PX.Commerce.Amazon.Sync.Interfaces;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CA;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.Sync.Processors
{
	[BCProcessor(typeof(BCAmazonConnector), BCEntitiesAttribute.NonOrderFee, AmazonCaptions.NonOrderFee, 240,
		IsInternal = false,
		Direction = SyncDirection.Import,
		PrimaryDirection = SyncDirection.Import,
		PrimarySystem = PrimarySystem.Extern,
		PrimaryGraph = typeof(PX.Objects.CA.CATranEntry),
		ExternTypes = new Type[] { typeof(NonOrderFee), typeof(NonOrderFeeGroup) },
		LocalTypes = new Type[] { typeof(CashTransaction), typeof(CashTransactionGroup) },
		DetailTypes = new String[] { BCEntitiesAttribute.CashTransaction },
		AcumaticaPrimaryType = typeof(CAAdj),
		AcumaticaPrimarySelect = typeof(Search<CAAdj.adjRefNbr>),
		URL = "payments/event/view?groupId={0}&transactionstatus=RELEASED&category=DEFAULT&resultsPerPage=10&pageNumber=1")]
	[BCProcessorDetail(EntityType = BCEntitiesAttribute.CashTransaction, EntityName = AmazonCaptions.CashTransaction, AcumaticaType = typeof(PX.Objects.CA.CAAdj))]
	public class AmazonNonOrderFeeProcessor : BCProcessorSingleBase<AmazonNonOrderFeeProcessor, AmazonNonOrderFeeEntityBucket, MappedNonOrderFee>, IProcessor
	{
		private IConverter<NonOrderFinancialEvents, IEnumerable<NonOrderFee>> _converter;

		private CASetup _cashPreferences;

		private IPaymentMethodFeeTypeHandler _feeTypeHandler;

		private INonOrderFinanceEventsDataProvider _financeEventsDataProvider;

		private INonOrderFeeGroupHandler _nonOrderFeeGroupHandler;

		private BCPaymentMethods _paymentMethod;

		private string _storeCurrency;

		[InjectDependency]
		private INonOrderFinancialEventsToNonOrderFeeConverterFactory ConverterFactory { get; set; }

		private string StoreCurrency => _storeCurrency ?? (_storeCurrency = this.GetStoreCurrency());

		[InjectDependency]
		private IPaymentMethodFeeTypeHandlerFactory FeeTypeHandlerFactory { get; set; }

		[InjectDependency]
		private IConverter<FeeComponent, NonOrderFee> FeeConverter { get; set; }

		[InjectDependency]
		private IConverter<ChargeComponent, NonOrderFee> ChargeConverter { get; set; }

		[InjectDependency]
		private INonOrderFinanceEventsDataProviderFactory FinanceEventsDataProviderFactory { get; set; }

		[InjectDependency]
		private INonOrderFeeGroupHandlerFactory NonOrderFeeGroupHandlerFactory { get; set; }

		[InjectDependency]
		private IStatementPeriodParser StatementPeriodParser { get; set; }

		private BCPaymentMethods PaymentMethod => _paymentMethod ?? (_paymentMethod = this.GetPaymentMethod());

		public override async Task Initialise(IConnector connector, ConnectorOperation operation, CancellationToken cancellationToken = default)
		{
			await base.Initialise(connector, operation, cancellationToken);

			var bindingAmazon = base.GetBindingExt<BCBindingAmazon>();
			var binding = base.GetBinding();
			var client = ((BCAmazonConnector)connector).GetRestClient(bindingAmazon, binding);
			_financeEventsDataProvider = this.FinanceEventsDataProviderFactory.CreateInstance(client);
			_converter = this.ConverterFactory.CreateInstance(this.FeeConverter, this.ChargeConverter);
			_feeTypeHandler = this.FeeTypeHandlerFactory.CreateInstance(binding.BindingID.Value, this, false);
			_cashPreferences = new PXSetup<CASetup>(this).Select();
			_nonOrderFeeGroupHandler = this.NonOrderFeeGroupHandlerFactory.CreateInstance(_financeEventsDataProvider, this.StatementPeriodParser);
		}

		#region Common
		public override Task<MappedNonOrderFee> PullEntity(string externID, string externalInfo, CancellationToken cancellationToken = default) => throw new NotImplementedException();

		public override Task<MappedNonOrderFee> PullEntity(Guid? localID, Dictionary<string, object> externalInfo, CancellationToken cancellationToken = default) => throw new NotImplementedException();
		#endregion

		#region Import
		public override async Task FetchBucketsForImport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default)
		{
			var bindingExt = base.GetBindingExt<BCBindingExt>();

			FinancialEventGroupsFilter filter = this.GetEventGroupsFilter(minDateTime, maxDateTime, bindingExt.SyncOrdersFrom);

			await Task.Yield();

			await foreach (FinancialEventGroup financialEventGroup in _financeEventsDataProvider.GetFinancialGroupsAsync(filter, this.StoreCurrency, cancellationToken))
			{
				NonOrderFeeGroup nonOrderFeeGroup = new NonOrderFeeGroup { FinancialEventGroup = financialEventGroup };
				MappedNonOrderFee mappedNonOrderFee = this.CreateMappedEntity(nonOrderFeeGroup);
				base.EnsureStatus(mappedNonOrderFee, SyncDirection.Import);
			}
		}

		private MappedNonOrderFee CreateMappedEntity(NonOrderFeeGroup nonOrderFeeGroup)
		{
			var financialEventGroup = nonOrderFeeGroup.FinancialEventGroup;
			var statementPeriod = this.StatementPeriodParser.PrepareStatementPeriod(financialEventGroup.FinancialEventGroupStart, financialEventGroup.FinancialEventGroupEnd);

			return new MappedNonOrderFee(nonOrderFeeGroup, nonOrderFeeGroup.FinancialEventGroup.FinancialEventGroupId, statementPeriod, financialEventGroup.FinancialEventGroupEnd.Value);
		}

		private FinancialEventGroupsFilter GetEventGroupsFilter(DateTime? minDateTime, DateTime? maxDateTime, DateTime? syncOrdersFrom)
		{
			var filter = new FinancialEventGroupsFilter();

			if (!minDateTime.HasValue)
				minDateTime = DateTime.UtcNow.AddDays(-ApiRestrictions.FinancialGroupDaysLimit);

			if (!maxDateTime.HasValue)
				maxDateTime = minDateTime.Value.AddDays(ApiRestrictions.FinancialGroupDaysLimit);

			if (maxDateTime >= DateTime.Now)
				maxDateTime = DateTime.UtcNow.AddMinutes(ApiRestrictions.FinancialGroupMinutesLimit);

			if ((maxDateTime.Value - minDateTime.Value).TotalDays > ApiRestrictions.FinancialGroupDaysLimit)
				throw new PXException(AmazonMessages.FinancialGroupFilterDateIsIncorrect);

			filter.FinancialEventGroupStartedAfter = minDateTime.Value;
			filter.FinancialEventGroupStartedBefore = maxDateTime.Value;

			return filter;
		}

		private BCPaymentMethods GetPaymentMethod()
		{
			BCBinding binding = base.GetBinding();
			BCPaymentMethods paymentMethod = BCPaymentMethodsMappingSlot
				.Get(binding.BindingID)
				.FirstOrDefault(paymentMethod => paymentMethod.Active.Value);

			if (paymentMethod is null)
				throw new PXException(AmazonMessages.OrderPaymentMethodIsMissing);

			return paymentMethod;
		}

		private string GetStoreCurrency()
		{
			var bindingAmazon = base.GetBindingExt<BCBindingAmazon>();
			AmazonUrlProvider.TryGetRegion(bindingAmazon.Region, out Region region);

			return region.Marketplaces.First(marketplace => marketplace.MarketplaceValue == bindingAmazon.Marketplace).Currency;
		}

		public override async Task MapBucketImport(AmazonNonOrderFeeEntityBucket bucket, IMappedEntity existing, CancellationToken cancellationToken = default)
		{
			//
			// TODO: The 'GetPaymentMethodMapping' is invoked to prepare validation only.The method should be refactored to separate data fetching from validation to avoid extra calls.
			//
			base.GetHelper<AmazonHelper>().GetPaymentMethodMapping(this.PaymentMethod.StorePaymentMethod, this.StoreCurrency, out _);
			NonOrderFeeGroup nonOrderFeeGroup = bucket.NonOrderFee.Extern;

			var nonOrderFees = this.ConvertEventsToNonOrderFees(nonOrderFeeGroup).ToList();

			_feeTypeHandler.AddMissedFeeTypes(nonOrderFees.Select(nonOrderFee => nonOrderFee.FeeDescription));

			string statementPeriod = this.StatementPeriodParser.PrepareStatementPeriod(nonOrderFeeGroup.FinancialEventGroup.FinancialEventGroupStart, nonOrderFeeGroup.FinancialEventGroup.FinancialEventGroupEnd);

			IEnumerable<CashTransaction> cashTransactions = this.ConvertToCashTransactions(nonOrderFees, statementPeriod);

			bucket.NonOrderFee.Local = new CashTransactionGroup { CashTransactions = cashTransactions.ToList() };
		}

		private IEnumerable<NonOrderFee> ConvertEventsToNonOrderFees(NonOrderFeeGroup nonOrderFeeGroup)
		{
			foreach ((NonOrderFinancialEvents events, DateTime postedDate) in nonOrderFeeGroup.FinancialEventsByDate)
			{
				foreach (NonOrderFee nonOrderFee in _converter.Convert(events))
				{
					if (nonOrderFee.Amount == decimal.Zero)
					{
						this.LogInfo(this.Operation.LogScope(), AmazonMessages.FeeIsSkippedDueToZeroAmount, nonOrderFee.EntryType, nonOrderFee.FeeDescription, nonOrderFee.PostedDate);
						continue;
					}

					nonOrderFee.PostedDate = postedDate;

					yield return nonOrderFee;
				}
			}
		}

		private IEnumerable<CashTransaction> ConvertToCashTransactions(IEnumerable<NonOrderFee> nonOrderFees, string statementPeriod)
		{
			foreach (NonOrderFee nonOrderFee in nonOrderFees)
				nonOrderFee.EntryType = _feeTypeHandler.GetStoredFeeMapping(nonOrderFee.FeeDescription).EntryTypeID;

			CashAccount cashAccount = CashAccount.PK.Find(this, this.PaymentMethod.CashAccountID);

			string ConstructKey(string description, string externalReference, decimal amount, DateTime transactionDate)
				=> $"{description.Trim()}|{externalReference.Trim()}|{amount.ToInvariantString().TrimEnd('0')}|{transactionDate.ToShortDateString()}";

			var storedTransactions = SelectFrom<CAAdj>.Where<CAAdj.extRefNbr.IsEqual<@P.AsString>>.View.Select(this, statementPeriod)
				.ToDictionary(result =>
				{
					CAAdj cashTransaction = (CAAdj)result;
					return ConstructKey(cashTransaction.TranDesc, cashTransaction.ExtRefNbr, cashTransaction.TranAmt.Value, cashTransaction.TranDate.Value);
				});

			var grouppedNonOrderFees = nonOrderFees.GroupBy(nonOrderFee => nonOrderFee.EntryType);

			foreach (var nonOrderFeesByType in grouppedNonOrderFees)
			{
				NonOrderFee firstFeeInGroup = nonOrderFeesByType.First();
				var feeMapping = _feeTypeHandler.GetStoredFeeMapping(firstFeeInGroup.FeeDescription);

				var feeMappingAmazon = feeMapping.GetExtension<BCFeeMappingExtAmazon>();

				if (feeMappingAmazon.Active == false)
				{
					this.LogInfo(this.Operation.LogScope(), AmazonMessages.FeeMappingIsInactive, feeMappingAmazon.FeeDescription, firstFeeInGroup.Amount);
					continue;
				}

				string description = string.Format(AmazonMessages.CashTransactionDescription, feeMapping.EntryTypeID);

				DateTime lastPostedDate;
				decimal totalAmount;

				if (nonOrderFeesByType.Count() == 1)
				{
					lastPostedDate = firstFeeInGroup.PostedDate.Value;
					totalAmount = firstFeeInGroup.Amount;
				}
				else
				{
					lastPostedDate = nonOrderFeesByType
						.Select(nonOrderFee => nonOrderFee.PostedDate.Value)
						.OrderBy(date => date)
						.Last();
					totalAmount = nonOrderFeesByType.Sum(nonOrderFee => nonOrderFee.Amount);
				}

				var transaction = this.CreateCashTransaction(description, lastPostedDate, statementPeriod, cashAccount.CashAccountCD, feeMapping.EntryTypeID, nonOrderFeesByType);

				if (storedTransactions.ContainsKey(ConstructKey(transaction.Description.Value, statementPeriod, totalAmount, transaction.PostedDate.Value.Value)))
					continue;

				yield return transaction;
			}
		}

		private CashTransaction CreateCashTransaction(string description, DateTime postedDate, string statementPeriod, string cashAccountCode, string entryTypeCode, IEnumerable<NonOrderFee> feeDetails)
		{
			return new CashTransaction
			{
				Description = description.ValueField(),
				PostedDate = postedDate.ValueField(),
				ExternalReferenceNumber = statementPeriod.ValueField(),
				CashAccountCD = cashAccountCode.ValueField(),
				EntryTypeCD = entryTypeCode.ValueField(),
				Hold = _cashPreferences.HoldEntry.ValueField(),
				Details = feeDetails.Select(fee =>
				{
					return new CashTransactionDetail
					{
						Amount = fee.Amount.ValueField(),
						AmountDescription = fee.FeeDescription.ValueField()
					};
				}).ToList()
			};
		}

		public override async Task<EntityStatus> GetBucketForImport(AmazonNonOrderFeeEntityBucket bucket, BCSyncStatus syncStatus, CancellationToken cancellationToken = default)
		{
			NonOrderFeeGroup nonOrderFeeGroup = await _nonOrderFeeGroupHandler.PrepareNonOrderFeeGroupAsync(syncStatus.ExternDescription, this.StoreCurrency, cancellationToken);
			MappedNonOrderFee mappedNonOrderFee = this.CreateMappedEntity(nonOrderFeeGroup);
			bucket.NonOrderFee = mappedNonOrderFee;

			return base.EnsureStatus(mappedNonOrderFee, SyncDirection.Import);
		}

		private bool IsAvailableForRelease => _cashPreferences.HoldEntry == false && this.PaymentMethod.ReleasePayments == true;

		public async override Task SaveBucketImport(AmazonNonOrderFeeEntityBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default)
		{
			MappedNonOrderFee nonOrderFee = bucket.NonOrderFee;
			IEnumerable<CashTransaction> cashTransactionsToStore = nonOrderFee.Local.CashTransactions;

			var message = string.Empty;
			if (!nonOrderFee.Local.CashTransactions.Any() && !nonOrderFee.Details.Any())
				message = AmazonMessages.InactiveAllFeeTypes;

			var earlierStoredTransactions = SelectFrom<CAAdj>.Where<CAAdj.noteID.IsIn<@P.AsGuid>>.View
				.Select(this, nonOrderFee.Details.Select(detail => detail.LocalID))
				.ToList();

			nonOrderFee.ClearDetails();

			foreach (CashTransaction cashTransaction in cashTransactionsToStore)
			{
				var createdCashTransaction = this.cbapi.Put<CashTransaction>(cashTransaction, nonOrderFee.LocalID);
				nonOrderFee.AddDetail(BCEntitiesAttribute.CashTransaction, createdCashTransaction.Id, nonOrderFee.Extern.FinancialEventGroup.FinancialEventGroupId);

				if (this.IsAvailableForRelease && createdCashTransaction.Approved.Value == true)
					this.cbapi.Invoke<CashTransaction, ReleaseCashTransaction>(null, createdCashTransaction.Id);
			}

			foreach (CAAdj cashTransaction in earlierStoredTransactions)
				nonOrderFee.AddDetail(BCEntitiesAttribute.CashTransaction, cashTransaction.NoteID, nonOrderFee.Extern.FinancialEventGroup.FinancialEventGroupId);

			base.UpdateStatus(nonOrderFee, operation, message);
		}
		#endregion

		#region Export
		public override Task FetchBucketsForExport(DateTime? minDateTime, DateTime? maxDateTime, PXFilterRow[] filters, CancellationToken cancellationToken = default) => throw new NotImplementedException();

		public override Task<EntityStatus> GetBucketForExport(AmazonNonOrderFeeEntityBucket bucket, BCSyncStatus status, CancellationToken cancellationToken = default) => throw new NotImplementedException();

		public override Task SaveBucketExport(AmazonNonOrderFeeEntityBucket bucket, IMappedEntity existing, string operation, CancellationToken cancellationToken = default) => throw new NotImplementedException();
		#endregion
	}

	public class AmazonNonOrderFeeRestrictor : BCBaseRestrictor, IRestrictor
	{
		public virtual FilterResult RestrictExport(IProcessor processor, IMappedEntity mapped, FilterMode mode) => null;

		public virtual FilterResult RestrictImport(IProcessor processor, IMappedEntity mapped, FilterMode mode)
		{
			#region Payments
			return base.Restrict<MappedNonOrderFee>(mapped, delegate (MappedNonOrderFee obj)
			{
				if (obj.Extern?.FinancialEventGroup is null)
					return null;

				FinancialEventGroup financialGroup = obj.Extern.FinancialEventGroup;

				if (financialGroup.ProcessingStatus != ProcessingStatus.Closed)
					return new FilterResult(FilterStatus.Invalid, PXMessages.LocalizeFormatNoPrefixNLA(AmazonMessages.FinancialGroupIsOpened, financialGroup.FinancialEventGroupId));

				return null;
			});
			#endregion
		}
	}

	public class AmazonNonOrderFeeEntityBucket : EntityBucketBase, IEntityBucket
	{
		public IMappedEntity Primary => this.NonOrderFee;

		public IMappedEntity[] Entities => new IMappedEntity[] { this.Primary };

		public MappedNonOrderFee NonOrderFee { get; set; }
	}
}
