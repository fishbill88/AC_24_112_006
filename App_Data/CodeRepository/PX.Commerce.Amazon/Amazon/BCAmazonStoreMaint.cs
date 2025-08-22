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

using IdentityModel.Client;
using PX.Commerce.Amazon.Amazon.DAC;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.Constants;
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Common;
using PX.Concurrency;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	public class BCAmazonStoreMaint : BCStoreMaint
	{
		public PXSelect<BCBindingAmazon, Where<BCBindingAmazon.bindingID, Equal<Current<BCBinding.bindingID>>>> CurrentBindingAmazon;
		[InjectDependency]
		private IAmazonCloudServiceClient _cloudServiceClient { get; set; }
		[InjectDependency]
		public Serilog.ILogger Logger { get; set; }

		public BCAmazonStoreMaint()
		{
			base.Bindings.WhereAnd<Where<BCBinding.connectorType, Equal<BCAmazonConnector.azConnectorType>>>();
		}

		#region Actions
		public PXAction<BCBinding> TestConnection;
		[PXButton(IsLockedOnToolbar = true)]
		[PXUIField(DisplayName = "Test Connection")]
		protected virtual IEnumerable testConnection(PXAdapter adapter)
		{
			Actions.PressSave();
			BCBindingAmazon bCBindingAmazon = CurrentBindingAmazon.Current ?? CurrentBindingAmazon.Select();
			BCBinding bCBinding = CurrentBinding.Current ?? CurrentBinding.Select();

			// TODO: double-check if it needs a concelation token here
			LongOperationManager.StartAsyncOperation(this, async delegate
			{
				BCAmazonConnector bCAmazonConnector = PXGraph.CreateInstance<BCAmazonConnector>();
				IAmazonRestClient _client = bCAmazonConnector.GetRestClient(bCBindingAmazon, bCBinding);
				try
				{
					var provider = new SellerDataProvider(_client);
					string serverTime = await provider.GetDateTime();
					if (!string.IsNullOrEmpty(serverTime))
					{
						Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information(AmazonMessages.ServerTimeRetrieved);
					}
					throw new PXOperationCompletedException(AmazonMessages.ConnectionEstablished);
				}
				catch (PXOperationCompletedException ex)
				{
					throw new PXOperationCompletedException(ex.Message);
				}
				catch (Exception ex)
				{
					throw new PXException(AmazonMessages.TestConnectionFailed);
				}
			});

			return adapter.Get();
		}


		public PXAction<BCBinding> Authorize;
		[PXButton(IsLockedOnToolbar = true)]
		[PXUIField(DisplayName = "Authorize")]
		protected virtual IEnumerable authorize(PXAdapter adapter)
		{
			this.Persist();

			BCBindingAmazon bindingAmazon = CurrentBindingAmazon.Current ?? CurrentBindingAmazon.Select();
			VerifyStoreSettings(bindingAmazon);

			string bindingID = Bindings.Current.BindingID.ToString();
			AmazonStateParameters parameters = new AmazonStateParameters(bindingID, GetMarketplaceURL(bindingAmazon), Accessinfo.CompanyName);
			Logger.ForContext("Scope", new BCLogTypeScope(GetType())).Information("{CommerceLogCaption}: BCAmazonStoreMaint.Authorize: {BindingID}, {MarketplaceURL}, {RedirectURI}",
				BCCaptions.CommerceLogCaption, parameters.State, parameters.MarketplaceURL, parameters.RedirectURI);

			LongOperationManager.StartAsyncOperation(this, async delegate
			{
				var url = await _cloudServiceClient.Authorize(parameters);
				if (!string.IsNullOrEmpty(url))
				{
					throw new PXRedirectToUrlException(url, PXWindowModeAttribute.Base, "Authenticate");
				}
			});

			return adapter.Get();
		}

		protected virtual void VerifyStoreSettings(BCBindingAmazon bindingAmazon)
		{
			string marketplaceURL = GetMarketplaceURL(bindingAmazon);

			if (string.IsNullOrEmpty(bindingAmazon.Region) && string.IsNullOrEmpty(marketplaceURL))
				throw new PXException(AmazonMessages.RegionMarketplaceRequired);
			else if (string.IsNullOrEmpty(bindingAmazon.Region))
				throw new PXException(AmazonMessages.RegionRequired);
			else if (string.IsNullOrEmpty(marketplaceURL))
				throw new PXException(AmazonMessages.MarketplaceRequired);

			if (string.IsNullOrEmpty(bindingAmazon.Region) && string.IsNullOrEmpty(bindingAmazon.Marketplace))
				throw new PXException(AmazonMessages.RegionMarketplaceRequired);
			else if (string.IsNullOrEmpty(bindingAmazon.Region))
				throw new PXException(AmazonMessages.RegionRequired);
			else if (string.IsNullOrEmpty(bindingAmazon.Marketplace))
				throw new PXException(AmazonMessages.MarketplaceRequired);
		}

		protected virtual string GetMarketplaceURL(BCBindingAmazon bindingAmazon) =>
			 AmazonUrlProvider.TryGetRegion(bindingAmazon.Region, out Region region)
				 ? region.Marketplaces.Where(x => x.MarketplaceValue == bindingAmazon.Marketplace).FirstOrDefault()?.MarketplaceUrl?.ToString()
				 : string.Empty;

		#endregion

		#region
		public PXSelect<BCFeeMapping, Where<BCFeeMapping.bindingID, Equal<Current<BCBinding.bindingID>>,
			And<BCFeeMapping.paymentMappingID, Equal<Current<BCPaymentMethods.paymentMappingID>>>>> FeeMappings;
		#endregion

		#region Events

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(BCConnectorsAttribute), nameof(BCConnectorsAttribute.DefaultConnector), BCAmazonConnector.TYPE)]
		public virtual void _(Events.CacheAttached<BCBinding.connectorType> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Generic Customer")]
		[BCSettingsChecker(new string[] { BCEntitiesAttribute.Order, BCEntitiesAttribute.SOInvoice, BCEntitiesAttribute.OrderOfTypeInvoice })]
		public virtual void _(Events.CacheAttached<BCBindingExt.guestCustomerID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIRequired(typeof(Where<Current<BCBindingExt.taxSynchronization>, Equal<True>>))]
		[PXUIField(DisplayName = "Primary Tax Zone")]
		public virtual void _(Events.CacheAttached<BCBindingExt.defaultTaxZoneID> e) { }

		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		public virtual void _(Events.CacheAttached<BCBindingExt.orderType> e) { }

		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		public virtual void _(Events.CacheAttached<BCBindingExt.customerNumberingID> e) { }

		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		public virtual void _(Events.CacheAttached<BCBindingExt.refundAmountItemID> e) { }

		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		public virtual void _(Events.CacheAttached<BCBindingExt.customerClassID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[BCSettingsChecker(new string[] { BCEntitiesAttribute.Order, BCEntitiesAttribute.SOInvoice, BCEntitiesAttribute.OrderOfTypeInvoice })]
		public virtual void _(Events.CacheAttached<BCBindingExt.syncOrdersFrom> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Show Discounts As", Enabled = false)]
		public virtual void _(Events.CacheAttached<BCBindingExt.postDiscounts> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Release Transactions")]
		public virtual void _(Events.CacheAttached<BCPaymentMethods.releasePayments> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRestrictor(typeof(Where<PaymentMethod.paymentType, Equal<PaymentMethodType.cashOrCheck>,
			And<Current<BCBinding.connectorType>, Equal<BCAmazonConnector.azConnectorType>,
				Or<Current<BCBinding.connectorType>, NotEqual<BCAmazonConnector.azConnectorType>>>>), AmazonMessages.ThePaymentMethodMustBeCashCheck)]
		public virtual void _(Events.CacheAttached<BCPaymentMethods.paymentMethodID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName ="Fee Type", Enabled = false)]
		[FeeTypesList]
		public virtual void _(Events.CacheAttached<BCFeeMapping.feeType> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXSelector(typeof(Search2<CAEntryType.entryTypeId,
			InnerJoin<CashAccountETDetail, On<CAEntryType.entryTypeId, Equal<CashAccountETDetail.entryTypeID>>>,
			Where<Current<BCFeeMapping.feeType>, Equal<FeeType.nonOrderFeeType>,
					And<CashAccountETDetail.cashAccountID, Equal<Current<BCPaymentMethods.cashAccountID>>,
					And<CAEntryType.accountID, IsNotNull,
				Or<Current<BCFeeMapping.feeType>, Equal<FeeType.orderFeeType>,
					And<CAEntryType.accountID, IsNotNull,
					And<CAEntryType.drCr, Equal<CADrCr.cACredit>,
					And<CAEntryType.consolidate, Equal<True>,
					And<CashAccountETDetail.cashAccountID, Equal<Current<BCPaymentMethods.cashAccountID>>>>>>>>>>>),
			SubstituteKey = typeof(CAEntryType.entryTypeId),
			DescriptionField = typeof(CAEntryType.descr))]
		public virtual void _(Events.CacheAttached<BCFeeMapping.entryTypeID> e) { }

		public override void _(Events.RowInserted<BCBinding> e)
		{
			base._(e);
			bool dirty = CurrentBindingAmazon.Cache.IsDirty;
			CurrentBindingAmazon.Insert();
			CurrentBindingAmazon.Cache.IsDirty = dirty;
		}

		// TODO: consider if removing this and using that in BCStoreMaint in the PX.Commerce.Objects is feasible or appropriate
		public virtual void _(Events.RowSelected<BCBindingAmazon> e)
		{
			BCBindingAmazon row = e.Row;
			if (row == null) return;

			bool? allowEditPaymentMethods = null;
			bool? allowEditFeeTypes = null;

			foreach (BCEntity entity in Entities.Select())
			{
				if (entity.EntityType.IsIn(BCEntitiesAttribute.Payment, BCEntitiesAttribute.NonOrderFee))
				{
					if (!allowEditPaymentMethods.HasValue || allowEditPaymentMethods == false)
						allowEditPaymentMethods = entity.IsActive == true;

					if (!allowEditFeeTypes.HasValue || allowEditFeeTypes == false)
						allowEditFeeTypes = entity.IsActive == true;
				}
			}

			PaymentMethods.AllowDelete = PaymentMethods.AllowInsert = PaymentMethods.AllowUpdate = allowEditPaymentMethods.Value;
			FeeMappings.AllowDelete = FeeMappings.AllowInsert = FeeMappings.AllowUpdate = allowEditFeeTypes.Value;
		}

		public virtual void _(Events.FieldUpdated<BCBindingExt.taxSynchronization> e)
		{
			if (e.Row == null) return;

			bool iSTaxSyncDisabled = (bool?)e.NewValue != true;

			if (iSTaxSyncDisabled)
				CurrentBindingAmazon.Cache.SetValueExt<BCBindingAmazon.defaultTaxID>(CurrentBindingAmazon.Current, null);

			CurrentBindingAmazon.Cache.Update(CurrentBindingAmazon.Current);
		}

		public virtual void _(Events.FieldUpdated<BCBindingAmazon.warehouse> e)
		{
			if (e.Row == null) return;

			if (e.NewValue != e.OldValue)
			{
				e.Cache.SetValueExt<BCBindingAmazon.locationID>(e.Row, null);
			}
		}

		public override void _(Events.FieldUpdated<BCEntity, BCEntity.isActive> e)
		{
			base._(e);

			// Cache should be marked as updated to raise persisting errors
			if (e.OldValue != e.NewValue)
			{
				CurrentBindingAmazon.Cache.MarkUpdated(CurrentBindingAmazon.Cache.Current);
			}
		}

		public virtual void _(Events.RowPersisting<BCEntity> e)
		{
			BCBindingAmazon amazonBinding = CurrentBindingAmazon.Current ?? CurrentBindingAmazon.Select();
			if (amazonBinding == null) return;

			if (e.Row is BCEntity entity)
			{
				bool anyError = false;
				if (entity.EntityType == BCEntitiesAttribute.SOInvoice && entity?.IsActive == true && PXAccess.FeatureInstalled<FeaturesSet.advancedSOInvoices>() == false)
				{
					Entities.Cache.RaiseExceptionHandling<BCEntity.isActive>(entity, entity.IsActive, new PXException(AmazonMessages.CannotActivateBecauseofAdvancedSOInvoices, PXErrorLevel.RowError));
					anyError = true;
				}

				if (anyError) throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
			}
		}

		public virtual void _(Events.RowPersisting<BCBinding> e)
		{
			BCBinding row = e.Row as BCBinding;
			if (row == null) return;
			Boolean anyError = false;

			var paymentEntity = Entities.Select().Select(x => x.GetItem<BCEntity>()).FirstOrDefault(x => x.EntityType == BCEntitiesAttribute.Payment);

			if (paymentEntity != null)
			{
				var paymentMethods = PaymentMethods.Select().Select(x => x.GetItem<BCPaymentMethods>()).ToList();
				var feeMappingKeys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

				foreach (BCFeeMapping item in FeeMappings.Select())
				{
					var mappingExt = item.GetExtension<BCFeeMappingExtAmazon>();

					var key = $"{item.PaymentMappingID.ToString() ?? string.Empty}#{mappingExt.FeeDescription}";
					if (feeMappingKeys.Contains(key))
					{
						anyError = true;

						FeeMappings.Cache.RaiseExceptionHandling<BCFeeMapping.feeType>(item, mappingExt.FeeDescription,
							new PXSetPropertyException<BCFeeMapping.feeType>(PXMessages.LocalizeFormat(BCMessages.FeeTypeAlreadyExistsInPaymentMethod, mappingExt.FeeDescription,
							paymentMethods.FirstOrDefault(x => x.PaymentMappingID == item.PaymentMappingID).StorePaymentMethod), PXErrorLevel.RowError));
					}
					else
						feeMappingKeys.Add(key);
				}

				if (anyError) throw new PXException(PX.Objects.Common.Messages.RecordCanNotBeSaved);
			}
		}
		#endregion

		public async Task ProcessAuthorizationCode(string code, string seller, int bindingId)
		{
			TokenResponse tokenResponse = await _cloudServiceClient.ProcessAuthorizationCode(code, seller, bindingId);
			if (tokenResponse.IsError)
				// Acuminator disable once PX1051 NonLocalizableString [Rethrowing an external error]
				// Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [Rethrowing an external error]
				throw new PXException($"{tokenResponse?.Error}: {tokenResponse?.ErrorDescription}");

			Bindings.Current = BCBinding.PK.Find(this, BCAmazonConnector.TYPE, bindingId);
			CurrentBindingAmazon.Current = PXSelect<BCBindingAmazon, Where<BCBindingAmazon.bindingID, Equal<Required<BCBindingAmazon.bindingID>>>>.Select(this, bindingId);
			CurrentBindingAmazon.Cache.SetValueExt(CurrentBindingAmazon.Current, nameof(BCBindingAmazon.refreshToken), tokenResponse.RefreshToken);
			CurrentBindingAmazon.Cache.SetValueExt(CurrentBindingAmazon.Current, nameof(BCBindingAmazon.sellerPartnerId), seller);
			CurrentBindingAmazon.Cache.Update(CurrentBindingAmazon.Current);
			Persist();
		}

		/// <summary>
		/// Check for duplicate fee mappings in Fees mapping form.
		/// </summary>
		public override void CheckForDuplicatedFees(PXCache cache, BCFeeMapping row)
		{
			//
			// There is nothing to be done. The Fee type validation is handled in Events.RowPersisting<BCBinding>.
			//
		}
	}
}
