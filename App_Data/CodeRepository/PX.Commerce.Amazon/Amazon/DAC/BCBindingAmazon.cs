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
using PX.Commerce.Amazon;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.TX;
using System;


namespace PX.Commerce.Amazon
{
	/// <summary>
	/// Amazon store settings
	/// </summary>
	[Serializable]
	[PXCacheName("Amazon Settings")]
	public class BCBindingAmazon : PXBqlTable, IBqlTable
	{
		public class PK : PrimaryKeyOf<BCBindingAmazon>.By<BCBindingAmazon.bindingID>
		{
			public static BCBindingAmazon Find(PXGraph graph, int? binding) => FindBy(graph, binding);
		}

		#region BindingID
		/// <summary>
		/// The primary key and identity of the binding.
		/// </summary>
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(BCBinding.bindingID))]
		[PXUIField(DisplayName = "Store", Visible = false)]
		[PXParent(typeof(Select<BCBinding, Where<BCBinding.bindingID, Equal<Current<BCBindingAmazon.bindingID>>>>))]
		public int? BindingID { get; set; }
		public abstract class bindingID : PX.Data.BQL.BqlInt.Field<bindingID> { }
		#endregion

		/// <summary>
		/// The identifier of the selling partner who is authorizing your application.
		/// </summary>
		#region Seller PartnerID
		[PXDBString(IsUnicode = true)]
		[PXUIField(DisplayName = "Seller Partner ID", Enabled = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[BCSettingsChecker(new string[] { BCEntitiesAttribute.Shipment })]
		public virtual string SellerPartnerId { get; set; }
		public abstract class sellerPartnerId : PX.Data.BQL.BqlString.Field<sellerPartnerId> { }
		#endregion

		/// <summary>
		/// AWS region
		/// </summary>
		#region Region
		[PXDBString(100, IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Region", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[BCAmazonRegionAttribute]
		public virtual string Region { get; set; }
		public abstract class region : PX.Data.BQL.BqlString.Field<region> { }
		#endregion

		/// <summary>
		///  Marketplace where selling partners authorizes the application
		/// </summary>
		#region MarketPlace
		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Marketplace", Required = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[BCAmazonMarketPlaceAttribute(typeof(BCBindingAmazon.region))]
		public virtual string Marketplace { get; set; }
		public abstract class marketplace : PX.Data.BQL.BqlString.Field<marketplace> { }
		#endregion

		/// <summary>
		/// The tax ID that the system will use to import taxes during the order import.
		/// </summary>
		#region DefaultTaxID
		[PXDBString()]
		[PXUIField(DisplayName = "Default Tax")]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIEnabled(typeof(Where<Current<BCBindingExt.taxSynchronization>, NotEqual<False>>))]
		[PXUIRequired(typeof(Where<Current<BCBindingExt.taxSynchronization>, Equal<True>>))]
		public virtual string DefaultTaxID { get; set; }
		public abstract class defaultTaxID : PX.Data.BQL.BqlString.Field<defaultTaxID> { }
		#endregion

		/// <summary>
		/// It's longed live token to exchange Access Token
		/// </summary>
		#region Refresh Token
		[PXRSACryptString(IsUnicode = true, InputMask = "")]
		[PXUIField(DisplayName = "Refresh Token", Visible = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual string RefreshToken { get; set; }
		public abstract class refreshToken : PX.Data.BQL.BqlString.Field<refreshToken> { }
		#endregion


		#region Seller-Fulfilled Order Type
		/// <summary>
		/// The type of the document, which is used for FBM orders.
		/// </summary>
		[PXDBString(2, IsFixed = true, InputMask = "")]
		[PXUIField(DisplayName = AmazonCaptions.FBMOrderType)]
		[PXSelector(
			typeof(Search<SOOrderType.orderType,
				Where<SOOrderType.active, Equal<True>,
					And<SOOrderType.behavior, Equal<SOBehavior.sO>,
					And<SOOrderType.aRDocType, Equal<ARDocType.invoice>,
					And<SOOrderTypeExt.encryptAndPseudonymizePII, Equal<True>>>>>>),
			DescriptionField = typeof(SOOrderType.descr))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[BCSettingsCheckerAttribute(new string[] { BCEntitiesAttribute.Order, BCEntitiesAttribute.Shipment })]
		public virtual string SellerFulfilledOrderType { get; set; }
		public abstract class sellerFulfilledOrderType : PX.Data.BQL.BqlString.Field<sellerFulfilledOrderType> { }
		#endregion

		#region Amazon Fulfilled Order Type
		/// <summary>
		/// The type of the document, which is used for FBA orders.
		/// </summary>
		[PXDBString(2, IsFixed = true, InputMask = "")]
		[PXUIField(DisplayName = AmazonCaptions.FBAOrderType)]
		[PXSelector(
			typeof(Search<SOOrderType.orderType,
				Where<SOOrderType.active, Equal<True>,
					And<SOOrderType.behavior, Equal<SOBehavior.iN>,
					And<SOOrderType.aRDocType, Equal<ARDocType.invoice>,
					And<SOOrderTypeExt.encryptAndPseudonymizePII, Equal<True>>>>>>),
			DescriptionField = typeof(SOOrderType.descr))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[BCSettingsCheckerAttribute(new string[] { BCEntitiesAttribute.OrderOfTypeInvoice })]
		public virtual string AmazonFulfilledOrderType { get; set; }
		public abstract class amazonFulfilledOrderType : PX.Data.BQL.BqlString.Field<amazonFulfilledOrderType> { }
		#endregion

		#region ShipViaCodesToCarriers
		public abstract class shipViaCodesToCarriers : PX.Data.BQL.BqlString.Field<shipViaCodesToCarriers> { }

		/// <summary>
		/// The substitution list will be used by the user to define the mapping between the ERP (Ship Via) and Amazon (Carrier).
		/// </summary>
		[PXDBString(25)]
		[PXUIField(DisplayName = "Ship Via Codes to Carriers")]
		[PXSelector(typeof(SYSubstitution.substitutionID))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[BCSettingsCheckerAttribute(new string[] { BCEntitiesAttribute.Shipment })]
		public virtual String ShipViaCodesToCarriers { get; set; }
		#endregion

		#region ShipViaCodesToCarrierServices
		/// <summary>
		/// The substitution list will be used by the user to define the mapping between the ERP (Ship Via) and Amazon (Carrier Service).
		/// </summary>
		[PXDBString(25)]
		[PXUIField(DisplayName = "Ship Via Codes to Carrier Services")]
		[PXSelector(typeof(SYSubstitution.substitutionID))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[BCSettingsCheckerAttribute(new string[] { BCEntitiesAttribute.Shipment })]
		public virtual String ShipViaCodesToCarrierServices { get; set; }
		public abstract class shipViaCodesToCarrierServices : PX.Data.BQL.BqlString.Field<shipViaCodesToCarrierServices> { }
		#endregion

		/// <summary>
		/// The warehouse will be used for items in the FBA orders
		/// </summary>
		#region Warehouse
		[PXDBInt()]
		[PXUIField(DisplayName = "Marketplace Warehouse")]
		[PXSelector(typeof(INSite.siteID),
					SubstituteKey = typeof(INSite.siteCD),
					DescriptionField = typeof(INSite.descr))]
		[PXRestrictor(typeof(Where<INSite.siteID, NotEqual<SiteAttribute.transitSiteID>>),
							 PX.Objects.IN.Messages.TransitSiteIsNotAvailable)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? Warehouse { get; set; }
		public abstract class warehouse : PX.Data.BQL.BqlInt.Field<warehouse> { }
		#endregion

		/// <summary>
		/// The warehouse location that has been set from where the inventory will be reduced when 
		/// marketplace invoices are released for FBA Orders
		/// </summary>
		#region LocationID
		[PXDBInt()]
		[PXUIField(DisplayName = "Marketplace Warehouse Location")]
		[PXSelector(typeof(Search<INLocation.locationID,
			Where<INLocation.siteID, Equal<Current<BCBindingAmazon.warehouse>>,
				And<INLocation.active, Equal<True>>>>),
			SubstituteKey = typeof(INLocation.locationCD),
			DescriptionField = typeof(INLocation.descr)
			)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? LocationID { get; set; }
		public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
		#endregion

		/// <summary>
		/// The shippingAccount will be used for shipping line in FBA
		/// </summary>
		#region ShippingAccount
		[Account(DisplayName = "Shipping Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? ShippingAccount { get; set; }
		public abstract class shippingAccount : PX.Data.BQL.BqlInt.Field<shippingAccount> { }
		#endregion

		/// <summary>
		/// The Shipping Subaccount will be used for shipping line in FBA
		/// </summary>
		#region Shipping Subaccount
		[SubAccount(DisplayName = "Shipping Subaccount", Visibility = PXUIVisibility.Visible)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? ShippingSubAccount { get; set; }
		public abstract class shippingSubAccount : PX.Data.BQL.BqlInt.Field<shippingSubAccount> { }
		#endregion

		/// <summary>
		/// Release Invoices
		/// </summary>
		#region ReleaseInvoices
		[PXDBBool]
		[PXUIField(DisplayName = "Release Invoices")]
		[PXDefault(false)]
		public virtual bool? ReleaseInvoices { get; set; }
		public abstract class releaseInvoices : PX.Data.BQL.BqlBool.Field<releaseInvoices> { }
		#endregion

		#region Shipping Price Item
		/// <summary>
		/// Item that should used for shipping.
		/// </summary>
		[PXDBInt()]
		[PXUIField(DisplayName = AmazonMessages.ShippingPriceItem)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<InventoryItem.inventoryID,
			Where<InventoryItem.nonStockShip, Equal<True>,
				And<InventoryItem.kitItem, Equal<False>,
				And2<Where<InventoryItem.itemStatus, Equal<InventoryItemStatus.active>,
										Or<InventoryItem.itemStatus, Equal<InventoryItemStatus.noPurchases>,
										Or<InventoryItem.itemStatus, Equal<InventoryItemStatus.noRequest>>>>,
				And<Where<InventoryItem.itemType, Equal<INItemTypes.serviceItem>,
				Or<InventoryItem.itemType, Equal<INItemTypes.nonStockItem>>>>>>>>),
			SubstituteKey = typeof(InventoryItem.inventoryCD),
			DescriptionField = typeof(InventoryItem.descr))]
		[BCSettingsCheckerAttribute(new string[] { BCEntitiesAttribute.SOInvoice, BCEntitiesAttribute.OrderOfTypeInvoice })]
		public virtual Int32? ShippingPriceItem { get; set; }
		public abstract class shippingPriceItem : PX.Data.BQL.BqlInt.Field<shippingPriceItem> { }
		#endregion
	}
	/// <inheritdoc cref="BCBinding"/>
	[PXPrimaryGraph(new Type[] { typeof(BCAmazonStoreMaint) },
						new Type[] { typeof(Where<BCBinding.connectorType, Equal<BCAmazonConnector.azConnectorType>>), })]
	public sealed class BCBindingAmazonExtension : PXCacheExtension<BCBinding>
	{
		public static bool IsActive() { return PXAccess.FeatureInstalled<PX.Objects.CS.FeaturesSet.amazonIntegration>(); }
	}
}
