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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Concurrency;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.SO;

using SOOrder = PX.Objects.SO.SOOrder;
using SOLine = PX.Objects.SO.SOLine;
using PX.Data.DependencyInjection;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.LicensePolicy;
using PX.Objects.PM;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AP.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.Common.Bql;
using PX.Objects.Extensions.CostAccrual;
using PX.Objects.DR;
using PX.Data.WorkflowAPI;
using PX.Objects.Common.Scopes;
using PX.Objects.IN.Services;
using PX.Objects.Extensions.MultiCurrency;
using PX.Data.Description;
using PX.Objects.PO.GraphExtensions.POOrderEntryExt;
using PX.Objects.IN.InventoryRelease;
using PX.Objects.Common.Interfaces;
using PX.Objects.PO.DAC.Projections;

namespace PX.Objects.PO
{

	#region Dac Types Overrides
	/// <summary>
	/// Represents a shipment destination address that contains the information related to the shipping of the ordered items.
	/// </summary>
	/// <remarks>
	/// The records of this type are created and edited on the <i>Purchase Orders (PO301000)</i> form
	/// (which corresponds to the <see cref="POOrderEntry"/> graph).
	/// </remarks>
	[PXCacheName(Messages.POShipAddressFull)]
    [Serializable]
	public partial class POShipAddress : POAddress
	{
		#region Keys
		public new class PK : PrimaryKeyOf<POShipAddress>.By<addressID>
		{
			public static POShipAddress Find(PXGraph graph, int? addressID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, addressID, options);
		}
		#endregion

		#region AddressID

		/// <inheritdoc cref="POAddress.addressID"/>
		public new abstract class addressID : PX.Data.BQL.BqlInt.Field<addressID> { }

		#endregion

		#region BAccountID

		/// <inheritdoc cref="BAccountID"/>
		public new abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }

		/// <inheritdoc cref="POAddress.bAccountID"/>
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public override Int32? BAccountID
		{
			get { return this._BAccountID; }
			set { this._BAccountID = value; }
		}

		#endregion

		#region BAccountAddressID

		/// <inheritdoc cref="POAddress.bAccountAddressID"/>
		public new abstract class bAccountAddressID : PX.Data.BQL.BqlInt.Field<bAccountAddressID> { }

		#endregion

		#region IsDefaultAddress

		/// <inheritdoc cref="POAddress.isDefaultAddress"/>
		public new abstract class isDefaultAddress : PX.Data.BQL.BqlBool.Field<isDefaultAddress> { }

		#endregion

		#region OverrideAddress

		/// <inheritdoc cref="POAddress.overrideAddress"/>
		public new abstract class overrideAddress : PX.Data.BQL.BqlBool.Field<overrideAddress> { }

		#endregion

		#region RevisionID

		/// <inheritdoc cref="POAddress.revisionID"/>
		public new abstract class revisionID : PX.Data.BQL.BqlInt.Field<revisionID> { }

		#endregion

		#region CountryID

		/// <inheritdoc cref="CountryID"/>
		public new abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }

		/// <inheritdoc cref="POAddress.countryID"/>
		[PXDefault(typeof(Search<GL.Branch.countryID, Where<GL.Branch.branchID, Equal<Current<AccessInfo.branchID>>>>))]
		[PXDBString(100)]
		[PXUIField(DisplayName = "Country")]
		[Country]
		public override String CountryID
		{
			get { return this._CountryID; }
			set { this._CountryID = value; }
		}

		#endregion

		#region StateID

		/// <inheritdoc cref="State"/>
		public new abstract class state : PX.Data.BQL.BqlString.Field<state> { }

		/// <inheritdoc cref="POAddress.state"/>
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State")]
		[State(typeof(POShipAddress.countryID))]
		public override String State
		{
			get { return this._State; }
			set { this._State = value; }
		}

		#endregion

		#region PostalCode

		/// <inheritdoc cref="PostalCode"/>
		public new abstract class postalCode : PX.Data.BQL.BqlString.Field<postalCode> { }

		/// <inheritdoc cref="POAddress.postalCode"/>
		[PXDBString(20)]
		[PXUIField(DisplayName = "Postal Code")]
		[PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask),
			countryIdField: typeof(POShipAddress.countryID))]
		[PXPersonalDataField]
		public override String PostalCode
		{
			get { return this._PostalCode; }
			set { this._PostalCode = value; }
		}

		#endregion

		#region IsValidated

		/// <inheritdoc cref="POAddress.isValidated"/>
		public new abstract class isValidated : PX.Data.BQL.BqlBool.Field<isValidated> { }

		#endregion
	}

	/// <summary>
	/// Represents an address of the vendor location that contains the information related to the vendor to supply the ordered goods.
	/// </summary>
	/// <remarks>
	/// The records of this type are created and edited on the <i>Purchase Orders (PO301000)</i> form
	/// (which corresponds to the <see cref="POOrderEntry"/> graph).
	/// </remarks>
	[PXCacheName(Messages.PORemitAddressFull)]
    [Serializable]
	public partial class PORemitAddress : POAddress
	{
		#region Keys
		public new class PK : PrimaryKeyOf<PORemitAddress>.By<addressID>
		{
			public static PORemitAddress Find(PXGraph graph, int? addressID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, addressID, options);
		}
		#endregion

		#region AddressID

		/// <inheritdoc cref="POAddress.addressID"/>
		public new abstract class addressID : PX.Data.BQL.BqlInt.Field<addressID> { }

		#endregion

		#region BAccountID

		/// <inheritdoc cref="BAccountID"/>
		public new abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }

		/// <inheritdoc cref="POAddress.bAccountID"/>
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public override Int32? BAccountID
		{
			get { return this._BAccountID; }
			set { this._BAccountID = value; }
		}

		#endregion

		#region BAccountAddressID

		/// <inheritdoc cref="POAddress.bAccountAddressID"/>
		public new abstract class bAccountAddressID : PX.Data.BQL.BqlInt.Field<bAccountAddressID> { }

		#endregion

		#region IsDefaultAddress

		/// <inheritdoc cref="POAddress.isDefaultAddress"/>
		public new abstract class isDefaultAddress : PX.Data.BQL.BqlBool.Field<isDefaultAddress> { }

		#endregion

		#region OverrideAddress

		/// <inheritdoc cref="POAddress.overrideAddress"/>
		public new abstract class overrideAddress : PX.Data.BQL.BqlBool.Field<overrideAddress> { }

		#endregion

		#region RevisionID

		/// <inheritdoc cref="POAddress.revisionID"/>
		public new abstract class revisionID : PX.Data.BQL.BqlInt.Field<revisionID> { }

		#endregion

		#region CountryID

		/// <inheritdoc cref="CountryID"/>
		public new abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }

		/// <inheritdoc cref="POAddress.countryID"/>
		[PXDefault(typeof(Search<GL.Branch.countryID, Where<GL.Branch.branchID, Equal<Current<AccessInfo.branchID>>>>))]
		[PXDBString(100)]
		[PXUIField(DisplayName = "Country")]
		[Country]
		public override String CountryID
		{
			get { return this._CountryID; }
			set { this._CountryID = value; }
		}

		#endregion

		#region StateID

		/// <inheritdoc cref="State"/>
		public new abstract class state : PX.Data.BQL.BqlString.Field<state> { }

		/// <inheritdoc cref="POAddress.state"/>
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "State")]
		[State(typeof(PORemitAddress.countryID))]
		public override String State
		{
			get { return this._State; }
			set { this._State = value; }
		}

		#endregion

		#region PostalCode

		/// <inheritdoc cref="PostalCode"/>
		public new abstract class postalCode : PX.Data.BQL.BqlString.Field<postalCode> { }

		/// <inheritdoc cref="POAddress.postalCode"/>
		[PXDBString(20)]
		[PXUIField(DisplayName = "Postal Code")]
		[PXZipValidation(typeof(Country.zipCodeRegexp), typeof(Country.zipCodeMask),
			countryIdField: typeof(PORemitAddress.countryID))]
		[PXPersonalDataField]
		public override String PostalCode
		{
			get { return this._PostalCode; }
			set { this._PostalCode = value; }
		}

		#endregion

		#region IsValidated

		/// <inheritdoc cref="POAddress.isValidated"/>
		public new abstract class isValidated : PX.Data.BQL.BqlBool.Field<isValidated> { }

		#endregion
	}

	/// <summary>
	/// Represents a shipping location contact that contains the information related to the shipping of the ordered items.
	/// </summary>
	/// <remarks>
	/// The records of this type are created and edited on the <i>Purchase Orders (PO301000)</i> form
	/// (which corresponds to the <see cref="POOrderEntry"/> graph).
	/// </remarks>
	[PXCacheName(Messages.POShipContactFull)]
    [Serializable]
	public partial class POShipContact : POContact
	{
		#region Keys
		public new class PK : PrimaryKeyOf<POContact>.By<contactID>
		{
			public static POContact Find(PXGraph graph, int? contactID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, contactID, options);
		}
		#endregion

		#region ContactID

		/// <inheritdoc cref="POContact.contactID"/>
		public new abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }

		#endregion

		#region BAccountID

		/// <inheritdoc cref="POContact.bAccountID"/>
		public new abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }

		#endregion

		#region BAccountContactID

		/// <inheritdoc cref="POContact.bAccountContactID"/>
		public new abstract class bAccountContactID : PX.Data.BQL.BqlInt.Field<bAccountContactID> { }

		#endregion

		#region RevisionID

		/// <inheritdoc cref="POContact.revisionID"/>
		public new abstract class revisionID : PX.Data.BQL.BqlInt.Field<revisionID> { }

		#endregion

		#region IsDefaultContact

		/// <inheritdoc cref="POContact.isDefaultContact"/>
		public new abstract class isDefaultContact : PX.Data.BQL.BqlBool.Field<isDefaultContact> { }

		#endregion

		#region OverrideContact

		/// <inheritdoc cref="POContact.overrideContact"/>
		public new abstract class overrideContact : PX.Data.BQL.BqlBool.Field<overrideContact> { }

		#endregion
	}

	/// <summary>
	/// Represents a vendor location contact that contains the information related to the vendor to supply the ordered goods.
	/// </summary>
	/// <remarks>
	/// The records of this type are created and edited on the <i>Purchase Orders (PO301000)</i> form
	/// (which corresponds to the <see cref="POOrderEntry"/> graph).
	/// </remarks>
	[PXCacheName(Messages.PORemitContactFull)]
    [Serializable]
	public partial class PORemitContact : POContact
	{
		#region Keys
		public new class PK : PrimaryKeyOf<PORemitContact>.By<contactID>
		{
			public static PORemitContact Find(PXGraph graph, int? contactID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, contactID, options);
		}
		#endregion

		#region ContactID

		/// <inheritdoc cref="POContact.contactID"/>
		public new abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }

		#endregion

		#region BAccountID

		/// <inheritdoc cref="POContact.bAccountID"/>
		public new abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }

		#endregion

		#region BAccountContactID

		/// <inheritdoc cref="POContact.bAccountContactID"/>
		public new abstract class bAccountContactID : PX.Data.BQL.BqlInt.Field<bAccountContactID> { }

		#endregion

		#region RevisionID

		/// <inheritdoc cref="POContact.revisionID"/>
		public new abstract class revisionID : PX.Data.BQL.BqlInt.Field<revisionID> { }

		#endregion

		#region IsDefaultContact

		/// <inheritdoc cref="POContact.isDefaultContact"/>
		public new abstract class isDefaultContact : PX.Data.BQL.BqlBool.Field<isDefaultContact> { }

		#endregion

		#region OverrideContact

		/// <inheritdoc cref="POContact.overrideContact"/>
		public new abstract class overrideContact : PX.Data.BQL.BqlBool.Field<overrideContact> { }

		#endregion
	}

	#endregion

	[Serializable]
	public class POOrderEntry : PXGraph<POOrderEntry, POOrder>, PXImportAttribute.IPXPrepareItems, IGraphWithInitialization
	{
		#region Extensions
		public class CostAccrual : NonStockAccrualGraph<POOrderEntry, POOrder>
		{
			[PXOverride]
			public virtual void SetExpenseAccount(PXCache sender, PXFieldDefaultingEventArgs e, InventoryItem item, Action<PXCache, PXFieldDefaultingEventArgs, InventoryItem> baseMethod)
			{
				POLine row = (POLine)e.Row;

				if (row != null && row.AccrueCost == true)
				{
					SetExpenseAccountSub(sender, e, item, row.SiteID,
					GetAccountSubUsingPostingClass: (InventoryItem inItem, INSite inSite, INPostClass inPostClass) =>
					{
						return INReleaseProcess.GetAcctID<INPostClass.invtAcctID>(Base, inPostClass.InvtAcctDefault, inItem, inSite, inPostClass);
					},
					GetAccountSubFromItem: (InventoryItem inItem) =>
					{
						return inItem.InvtAcctID;
					});
				}
			}

			[PXOverride]
			public virtual object GetExpenseSub(PXCache sender, PXFieldDefaultingEventArgs e, InventoryItem item, Func<PXCache, PXFieldDefaultingEventArgs, InventoryItem, object> baseMethod)
			{
				POLine row = (POLine)e.Row;

				object expenseAccountSub = null;

				if (row != null && row.AccrueCost == true)
				{
					expenseAccountSub = GetExpenseAccountSub(sender, e, item, row.SiteID,
					GetAccountSubUsingPostingClass: (InventoryItem inItem, INSite inSite, INPostClass inPostClass) =>
					{
						return INReleaseProcess.GetSubID<INPostClass.invtSubID>(Base, inPostClass.InvtAcctDefault, inPostClass.InvtSubMask, inItem, inSite, inPostClass);
					},
					GetAccountSubFromItem: (InventoryItem inItem) =>
					{
						return inItem.InvtSubID;
					});
				}

				return expenseAccountSub;
			}
		}

		public class MultiCurrency : MultiCurrencyGraph<POOrderEntry, POOrder>
		{
			protected override string Module => BatchModule.PO;

			protected override CurySourceMapping GetCurySourceMapping()
			{
				return new CurySourceMapping(typeof(Vendor));
			}

			protected override DocumentMapping GetDocumentMapping()
			{
				return new DocumentMapping(typeof(POOrder))
				{
					DocumentDate = typeof(POOrder.orderDate),
					BAccountID = typeof(POOrder.vendorID)
				};
			}

			protected override PXSelectBase[] GetChildren()
			{
				return new PXSelectBase[]
				{
					Base.Document,
					Base.Transactions,
					Base.Tax_Rows,
					Base.Taxes,
					Base.DiscountDetails
				};
			}

			protected override PXSelectBase[] GetTrackedExceptChildren()
			{
				return new PXSelectBase[]
				{
					Base.poLiner
				};
			}

			protected override bool AllowOverrideRate(PXCache sender, CurrencyInfo info, CurySource source)
			{
				return base.AllowOverrideRate(sender, info, source) && Base.Document.Current?.Hold == true;
			}

			protected override bool AllowOverrideCury()
			{
				return !(!base.AllowOverrideCury() && Base.IsCopyPasteContext == false &&
						  PXUIFieldAttribute.GetErrorWithLevel<POOrder.curyID>(Base.Document.Cache, Base.Document.Current).errorLevel != PXErrorLevel.Error);
			}

			[PXOverride]
			public  virtual void POOrder_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated baseMethod)
			{
				POOrder order = (POOrder)e.Row;
				if (e.ExternalCall || order?.CuryID == null || order.OverrideCurrency == true)
				{
					SourceFieldUpdated<POOrder.curyInfoID, POOrder.curyID, POOrder.orderDate>(sender, order);
				}

				baseMethod(sender, e);
			}

			protected override void _(Events.FieldUpdated<Document, Document.bAccountID> e)
			{
				// Refer to MultiCurrency.POOrder_VendorID_FieldUpdated.
				// We need update document currency prior to POOrderEntry.POOrder_VendorID_FieldUpdated execution.
			}

			protected override void _(Events.FieldUpdated<Document, Document.branchID> e)
			{
				bool resetCuryID = e.Row?.BAccountID == null && !Base.IsCopyPasteContext && (e.ExternalCall || e.Row?.CuryID == null);
				SourceFieldUpdated<Document.curyInfoID, Document.curyID, Document.documentDate>(e.Cache, e.Row, resetCuryID);
			}

			protected override void _(Events.FieldVerifying<Document, Document.curyID> e)
			{
				string newCuryID = e.NewValue as string;

				if (Base.vendor.Current != null && (bool)Base.vendor.Current.AllowOverrideCury == false && newCuryID != Base.vendor.Current.CuryID)
				{
					throw new PXSetPropertyException(Messages.VendorHasDifferentCury, Base.vendor.Current.AcctCD, Base.vendor.Current.CuryID, newCuryID);
				}

				// Acuminator disable once PX1044 ChangesInPXCacheInEventHandlers [Insert a new CuryInfo]
				base._(e);
			}
		}

		#endregion

		public delegate void OnCopyPOLineFields(POFixedDemand demand, POLine line);
		public OnCopyPOLineFields onCopyPOLineFields;

		private DiscountEngine<POLine, POOrderDiscountDetail> _discountEngine => DiscountEngineProvider.GetEngineFor<POLine, POOrderDiscountDetail>();
		[InjectDependency]
		public IInventoryAccountService InventoryAccountService { get; set; }

		#region Ctor + Public Selects

		public PXSetup<Company> company;

		[PXViewName(Messages.POOrder)]
		[PXCopyPasteHiddenFields(typeof(POOrder.sOOrderType), typeof(POOrder.sOOrderNbr), typeof(POOrder.isLegacyDropShip))]
		public PXSelectJoin<POOrder,
			LeftJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<POOrder.vendorID>>>,
			Where<POOrder.orderType, Equal<Optional<POOrder.orderType>>,
			And<Where<Vendor.bAccountID, IsNull,
			Or<Match<Vendor, Current<AccessInfo.userName>>>>>>> Document;

		public PXSelect<POOrder, Where<POOrder.orderType, Equal<Current<POOrder.orderType>>,
				And<POOrder.orderNbr, Equal<Current<POOrder.orderNbr>>>>> CurrentDocument;

		[PXViewName(Messages.POLine)]
		[PXImport(typeof(POOrder))]
		[PXCopyPasteHiddenFields(typeof(POLine.closed), typeof(POLine.cancelled), typeof(POLine.completed))]
		public PXOrderedSelect<POOrder, POLine,
			Where<POLine.orderType, Equal<Current<POOrder.orderType>>,
				And<POLine.orderNbr, Equal<Optional<POOrder.orderNbr>>>>,
			OrderBy<Asc<POLine.orderType, Asc<POLine.orderNbr, Asc<POLine.sortOrder, Asc<POLine.lineNbr>>>>>> Transactions;

		public PXSelect<POTax, Where<POTax.orderType, Equal<Current<POOrder.orderType>>, And<POTax.orderNbr, Equal<Current<POOrder.orderNbr>>>>, OrderBy<Asc<POTax.orderType, Asc<POTax.orderNbr, Asc<POTax.taxID>>>>> Tax_Rows;
		public PXSelectJoin<POTaxTran, LeftJoin<Tax, On<Tax.taxID, Equal<POTaxTran.taxID>>>,
			Where<POTaxTran.orderType, Equal<Current<POOrder.orderType>>,
				And<POTaxTran.orderNbr, Equal<Current<POOrder.orderNbr>>>>> Taxes;

		public PXSelect
			<POOrderDiscountDetail,
				Where
				<POOrderDiscountDetail.orderType, Equal<Current<POOrder.orderType>>,
					And<POOrderDiscountDetail.orderNbr, Equal<Current<POOrder.orderNbr>>>>,
				OrderBy<Asc<POOrderDiscountDetail.lineNbr>>> DiscountDetails;

		[PXViewName(Messages.PORemitAddress)]
		public PXSelect<PORemitAddress, Where<PORemitAddress.addressID, Equal<Current<POOrder.remitAddressID>>>> Remit_Address;

		[PXViewName(Messages.PORemitContact)]
		public PXSelect<PORemitContact, Where<PORemitContact.contactID, Equal<Current<POOrder.remitContactID>>>> Remit_Contact;

		[PXViewName(Messages.POShipAddress)]
		public PXSelect<POShipAddress, Where<POShipAddress.addressID, Equal<Current<POOrder.shipAddressID>>>> Shipping_Address;

		[PXViewName(Messages.POShipContact)]
		public PXSelect<POShipContact, Where<POShipContact.contactID, Equal<Current<POOrder.shipContactID>>>> Shipping_Contact;

		public PXSelect<POSetupApproval, Where<POSetupApproval.orderType, Equal<Optional<POOrder.orderType>>>> SetupApproval;

		[PXViewName(Messages.Approval)]
		public EPApprovalAutomation<POOrder, POOrder.approved, POOrder.rejected, POOrder.hold, POSetupApproval> Approval;

		[PXCopyPasteHiddenView]
		public PXSelect<INReplenishmentOrder> Replenihment;

		[PXCopyPasteHiddenView]
		public PXSelect<INReplenishmentLine,
			Where<INReplenishmentLine.pOType, Equal<Current<POLine.orderType>>,
				And<INReplenishmentLine.pONbr, Equal<Current<POLine.orderNbr>>,
				And<INReplenishmentLine.pOLineNbr, Equal<Current<POLine.lineNbr>>>>>> ReplenishmentLines;

		public PXSelect<POItemCostManager.POVendorInventoryPriceUpdate> priceStatus;
		public PXSetup<POSetup> POSetup;
		public APSetupNoMigrationMode apsetup;
		public PXSetup<INSetup> INSetup;
		public PXSetup<Branch>.Where<Branch.branchID.IsEqual<AccessInfo.branchID.FromCurrent>> Company;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<POOrder.curyInfoID>>>> Current_currencyinfo;

		public PXSelect<RQ.RQRequisitionOrder> rqrequisitionorder;

		[PXCopyPasteHiddenView]
		public PXSelect<POOrderPOReceipt,
			Where<POOrderPOReceipt.pOType, Equal<Current<POOrder.orderType>>,
				And<POOrderPOReceipt.pONbr, Equal<Current<POOrder.orderNbr>>>>> Receipts;

		[PXCopyPasteHiddenView]
		public PXSelect<POOrderAPDoc,
			Where<POOrderAPDoc.pOOrderType, Equal<Current<POOrder.orderType>>,
				And<POOrderAPDoc.pONbr, Equal<Current<POOrder.orderNbr>>>>> APDocs;

		[PXCopyPasteHiddenView]
		public PXSelectJoin<POBlanketOrderPOOrder,
			LeftJoin<POBlanketOrderPOReceipt,
				On<POBlanketOrderPOOrder.pOType, Equal<POBlanketOrderPOReceipt.pOType>,
					And<POBlanketOrderPOOrder.pONbr, Equal<POBlanketOrderPOReceipt.pONbr>,
					And<POBlanketOrderPOOrder.orderType, Equal<POBlanketOrderPOReceipt.orderType>,
					And<POBlanketOrderPOOrder.orderNbr, Equal<POBlanketOrderPOReceipt.orderNbr>>>>>>,
			Where<POBlanketOrderPOOrder.pOType, Equal<Current<POOrder.orderType>>,
				And<POBlanketOrderPOOrder.pONbr, Equal<Current<POOrder.orderNbr>>>>,
			OrderBy<Asc<POBlanketOrderPOOrder.orderNbr, Asc<POBlanketOrderPOReceipt.receiptNbr>>>> ChildOrdersReceipts;

		[PXCopyPasteHiddenView]
		public PXSelect<POBlanketOrderAPDoc,
			Where<POBlanketOrderAPDoc.pOType, Equal<Current<POOrder.orderType>>,
				And<POBlanketOrderAPDoc.pONbr, Equal<Current<POOrder.orderNbr>>>>> ChildOrdersAPDocs;


		protected virtual void _(Events.FieldSelecting<POOrderAPDoc.statusText> e)
		{
			if (Document.Current == null || e.Row == null)
				return;

			var query =
				new PXSelectGroupBy<APTranSigned,
					Where<APTranSigned.pOOrderType, Equal<Current<POOrder.orderType>>, And<APTranSigned.pONbr, Equal<Current<POOrder.orderNbr>>,
						And<APTranSigned.tranType, NotEqual<APDocType.prepayment>>>>,
					Aggregate<Sum<APTranSigned.signedBaseQty, Sum<APTranSigned.signedCuryTranAmt, Sum<APTranSigned.signedCuryRetainageAmt, Sum<APTranSigned.pOPPVAmt>>>>>>(this);

			var apTranTotal = query.SelectSingle();

			e.ReturnValue = GetAPDocStatusText(apTranTotal);
		}

		protected virtual void _(Events.FieldSelecting<POBlanketOrderAPDoc.statusText> e)
		{
			if (Document.Current == null || e.Row == null)
				return;

			var query =
				new PXSelectJoinGroupBy<APTranSigned,
					InnerJoin<POLine,
						On<APTranSigned.pOOrderType, Equal<POLine.orderType>,
							And<APTranSigned.pONbr, Equal<POLine.orderNbr>,
							And<APTranSigned.pOLineNbr, Equal<POLine.lineNbr>>>>>,
					Where<POLine.pOType, Equal<Current<POOrder.orderType>>, And<POLine.pONbr, Equal<Current<POOrder.orderNbr>>,
						And<APTranSigned.tranType, NotEqual<APDocType.prepayment>>>>,
					Aggregate<Sum<APTranSigned.signedBaseQty, Sum<APTranSigned.signedCuryTranAmt, Sum<APTranSigned.signedCuryRetainageAmt, Sum<APTranSigned.pOPPVAmt>>>>>>(this);

			var apTranTotal = query.SelectSingle();

			e.ReturnValue = GetAPDocStatusText(apTranTotal);
		}

		protected virtual string GetAPDocStatusText(APTranSigned apTranTotal)
		{
			object returnValue = null;
			Caches[typeof(APTranSigned)].RaiseFieldSelecting<APTranSigned.signedCuryTranAmt>(apTranTotal, ref returnValue, true);
			int documentCuryPrecision = returnValue is PXDecimalState signedCuryTranAmtState ? signedCuryTranAmtState.Precision : 0;

			returnValue = null;
			Caches[typeof(APTranSigned)].RaiseFieldSelecting<APTranSigned.pOPPVAmt>(apTranTotal, ref returnValue, true);
			int baseCuryPrecision = returnValue is PXDecimalState pOPPVAmtState ? pOPPVAmtState.Precision : 0;

			if (PXAccess.FeatureInstalled<FeaturesSet.inventory>())
			{
				return PXMessages.LocalizeFormatNoPrefix(Messages.StatusTotalBilledTotalPPV,
					FormatQty(apTranTotal?.SignedBaseQty ?? 0),
					FormatAmt((apTranTotal?.SignedCuryTranAmt ?? 0) + (apTranTotal?.SignedCuryRetainageAmt ?? 0), documentCuryPrecision),
					FormatAmt(apTranTotal?.POPPVAmt ?? 0, baseCuryPrecision));
			}
			else
			{
				return PXMessages.LocalizeFormatNoPrefix(Messages.StatusTotalBilled,
						FormatQty(apTranTotal?.SignedBaseQty ?? 0),
						FormatAmt((apTranTotal?.SignedCuryTranAmt ?? 0) + (apTranTotal?.SignedCuryRetainageAmt ?? 0), documentCuryPrecision));
			}
		}

		public virtual string FormatQty(decimal? value)
		{
			return (value == null) ? string.Empty : ((decimal)value).ToString("N" + CommonSetupDecPl.Qty.ToString(), System.Globalization.NumberFormatInfo.CurrentInfo);
		}

		public virtual string FormatAmt(decimal? value, int precision)
		{
			return (value == null) ? string.Empty : ((decimal)value).ToString("N" + precision.ToString(), System.Globalization.NumberFormatInfo.CurrentInfo);
		}

		protected virtual void _(Events.FieldSelecting<POOrderPOReceipt.statusText> e)
		{
			if (Document.Current == null || e.Row == null)
				return;

			var query =
				new PXSelectGroupBy<POReceiptLineSigned,
					Where<POReceiptLineSigned.pOType, Equal<Current<POOrder.orderType>>, And<POReceiptLineSigned.pONbr, Equal<Current<POOrder.orderNbr>>>>,
					Aggregate<Sum<POReceiptLineSigned.signedBaseReceiptQty>>>(this);

			var receiptLineTotal = query.SelectSingle();

			e.ReturnValue = PXMessages.LocalizeFormatNoPrefix(Messages.StatusTotalReceived, FormatQty(receiptLineTotal?.SignedBaseReceiptQty ?? 0));
		}

		protected virtual void _(Events.FieldSelecting<POBlanketOrderPOOrder.statusText> e)
		{
			if (Document.Current == null || e.Row == null)
				return;

			var orderLineQuery =
				new PXSelectGroupBy<POLine,
					Where<POLine.pOType, Equal<Current<POOrder.orderType>>, And<POLine.pONbr, Equal<Current<POOrder.orderNbr>>>>,
					Aggregate<Sum<POLine.baseOrderQty>>>(this);

			var orderLineTotal = orderLineQuery.SelectSingle();

			var receiptLineQuery =
				new PXSelectJoinGroupBy<POReceiptLineSigned,
					InnerJoin<POLine,
						On<POReceiptLineSigned.pOType, Equal<POLine.orderType>,
							And<POReceiptLineSigned.pONbr, Equal<POLine.orderNbr>,
							And<POReceiptLineSigned.pOLineNbr, Equal<POLine.lineNbr>>>>>,
					Where<POLine.pOType, Equal<Current<POOrder.orderType>>, And<POLine.pONbr, Equal<Current<POOrder.orderNbr>>>>,
					Aggregate<Sum<POReceiptLineSigned.signedBaseReceiptQty>>>(this);

			var receiptLineTotal = receiptLineQuery.SelectSingle();

			e.ReturnValue = PXMessages.LocalizeFormatNoPrefix(Messages.StatusTotalOrderedReceived, FormatQty(orderLineTotal?.BaseOrderQty ?? 0), FormatQty(receiptLineTotal?.SignedBaseReceiptQty ?? 0));
		}

		[PXViewName(CR.Messages.MainContact)]
		public PXSelect<Contact> DefaultCompanyContact;

		protected virtual IEnumerable defaultCompanyContact()
		{
			return OrganizationMaint.GetDefaultContactForCurrentOrganization(this);
		}

		[Api.Export.PXOptimizationBehavior(IgnoreBqlDelegate = true)]
		protected virtual IEnumerable transactions()
		{
			PrefetchWithDetails();
			return null;
		}

		public virtual void PrefetchWithDetails()
		{
		}

		[PXViewName(AP.Messages.Vendor)] public PXSetup<Vendor, Where<Vendor.bAccountID, Equal<Optional<POOrder.vendorID>>>>
			vendor;

		public PXSetup<VendorClass, Where<VendorClass.vendorClassID, Equal<Current<Vendor.vendorClassID>>>> vendorclass;

		[PXViewName(CR.Messages.Employee)] public
			PXSetup<EPEmployee, Where<EPEmployee.defContactID, Equal<Current<POOrder.ownerID>>>> Employee;

		public PXSetup<TaxZone, Where<TaxZone.taxZoneID, Equal<Current<POOrder.taxZoneID>>>> taxzone;

		public
			PXSetup
			<Location,
				Where
				<Location.bAccountID, Equal<Current<POOrder.vendorID>>,
					And<Location.locationID, Equal<Optional<POOrder.vendorLocationID>>>>> location;

		#region Add PO Order sub-form

		public PXFilter<POOrderFilter> filter;

		public PXSelect<POLineS,
					Where<POLineS.orderType, Equal<Current<POOrderFilter.orderType>>,
								And<POLineS.orderNbr, Equal<Current<POOrderFilter.orderNbr>>,
								And<POLineS.lineType, NotEqual<POLineType.description>,
								And<POLineS.completed, Equal<boolFalse>,
								And<Where<POLineS.orderQty, Equal<decimal0>, Or<POLineS.openQty, Greater<decimal0>>>>>>>>,
					OrderBy<Asc<POLineS.sortOrder>>> poLinesSelection;

		public PXSelectJoin<POOrderS,
			CrossJoin<APSetup>,
			Where<POOrderS.vendorID, Equal<Current<POOrder.vendorID>>,
				And<POOrderS.vendorLocationID, Equal<Current<POOrder.vendorLocationID>>,
				And<POOrderS.curyID, Equal<Current<POOrder.curyID>>,
				And<POOrderS.hold, Equal<boolFalse>,
				And<POOrderS.cancelled, Equal<boolFalse>,
				And<POOrderS.approved, Equal<boolTrue>,
				And2<Where<POOrderS.payToVendorID, Equal<Current<POOrder.payToVendorID>>, Or<Not<FeatureInstalled<FeaturesSet.vendorRelations>>>>,
				And<Where<APSetup.requireSingleProjectPerDocument, Equal<boolFalse>, Or<POOrderS.projectID, Equal<Current<POOrder.projectID>>>>>>>>>>>>,
			OrderBy<Asc<POOrderS.orderNbr>>> openOrders;

		public PXSelect<POLineR> poLiner;

		public PXSelect<POOrderR,
			Where<POOrderR.orderType, Equal<Required<POOrderR.orderType>>,
			And<POOrderR.orderNbr, Equal<Required<POOrderR.orderNbr>>,
			And<POOrderR.status, Equal<Required<POOrderR.status>>>>>> poOrder;

		#endregion

		#region SO Demand sub-form

		[PXCopyPasteHiddenView] public PXSelect<SOLineSplit3,
			Where<SOLineSplit3.pOType, Equal<Optional<POLine.orderType>>,
				And<SOLineSplit3.pONbr, Equal<Optional<POLine.orderNbr>>,
				And<SOLineSplit3.pOLineNbr, Equal<Optional<POLine.lineNbr>>>>>> FixedDemand;

		[PXCopyPasteHiddenView]
		public PXSelectReadonly<SOLineSplit,
			Where<SOLineSplit.pOType, Equal<Optional<POLine.orderType>>,
				And<SOLineSplit.pONbr, Equal<Optional<POLine.orderNbr>>,
				And<SOLineSplit.pOLineNbr, Equal<Optional<POLine.lineNbr>>>>>> RelatedSOLineSplit;

		[PXCopyPasteHiddenView] public PXSelect<SOLine5, Where<SOLine5.orderType, Equal<Optional<SOLineSplit3.orderType>>,
				And<SOLine5.orderNbr, Equal<Optional<SOLineSplit3.orderNbr>>,
				And<SOLine5.lineNbr, Equal<Optional<SOLineSplit3.lineNbr>>>>>> FixedDemandOrigSOLine;

		#endregion

		public PXFilter<RecalcDiscountsParamFilter> recalcdiscountsfilter;
		[PXCopyPasteHiddenView()]
		public PXSelect<SOLineSplit> solinesplit;

		public POOrderEntry()
		{
			POSetup setup = POSetup.Current;
			APSetup apSetup = apsetup.Current;
			if (PXAccess.FeatureInstalled<FeaturesSet.inventory>())
			{
				INSetup inSetup = INSetup.Current;
			}

			RowUpdated.AddHandler<POOrder>(ParentFieldUpdated);
			POOrderFilter currentFilter = filter.Current;
			this.poLiner.Cache.AllowInsert = false;
			this.poLiner.Cache.AllowDelete = false;

			this.poLinesSelection.Cache.AllowInsert = false;
			this.poLinesSelection.Cache.AllowDelete = false;
			this.poLinesSelection.Cache.AllowUpdate = true;

			this.openOrders.Cache.AllowInsert = false;
			this.openOrders.Cache.AllowDelete = false;
			this.openOrders.Cache.AllowUpdate = true;

			FixedDemand.AllowDelete = false;
			FixedDemand.AllowInsert = false;

			PXFieldState state = (PXFieldState)this.Transactions.Cache.GetStateExt<POLine.inventoryID>(null);
			viewInventoryID = state != null ? state.ViewName : null;

			bool isPMVisible = ProjectAttribute.IsPMVisible(BatchModule.PO);
			PXUIFieldAttribute.SetVisible<POLine.projectID>(Transactions.Cache, null, isPMVisible);
			PXUIFieldAttribute.SetVisible<POLine.taskID>(Transactions.Cache, null, isPMVisible);

			TaxAttribute.SetTaxCalc<POLine.taxCategoryID>(Transactions.Cache, null, TaxCalc.ManualLineCalc);

			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) =>
			{
				if (e.Row != null) e.NewValue = BAccountType.VendorType;
			});
			FieldDefaulting.AddHandler<InventoryItem.stkItem>((sender, e) =>
			{
				if (e.Row != null && InventoryHelper.CanCreateStockItem(sender.Graph) == false) e.NewValue = false;
			});
		}

		[InjectDependency]
		protected Func<string, ReportNotificationGenerator> ReportNotificationGeneratorFactory { get; private set; }

		[InjectDependency]
		protected ILicenseLimitsService _licenseLimits { get; set; }

		void IGraphWithInitialization.Initialize()
		{
			if (_licenseLimits != null)
			{
				OnBeforeCommit += _licenseLimits.GetCheckerDelegate<POOrder>(
					new TableQuery(TransactionTypes.LinesPerMasterRecord, typeof(POLine), (graph) =>
					{
						return new PXDataFieldValue[]
						{
							new PXDataFieldValue<POLine.orderType>(PXDbType.Char, ((POOrderEntry)graph).Document.Current?.OrderType),
							new PXDataFieldValue<POLine.orderNbr>(((POOrderEntry)graph).Document.Current?.OrderNbr)
						};
					}));
			}
		}

		protected readonly string viewInventoryID;

		#region Select Overloads

#if false
		private bool skipCheck;
		
		public virtual IEnumerable openorders()
		{
			POOrder row = this.Document.Current;
			if (row == null) yield break;

			PXSelectBase<POOrderS> select = new PXSelect<POOrderS,
								Where<POOrderS.vendorID, Equal<Current<POOrder.vendorID>>,
								And<POOrderS.vendorLocationID, Equal<Current<POOrder.vendorLocationID>>,
								And<POOrderS.curyID, Equal<Current<POOrder.curyID>>,
								And<POOrderS.hold, Equal<boolFalse>,
								And<POOrderS.cancelled, Equal<boolFalse>,
								And<POOrderS.approved, Equal<boolTrue>>>>>>>,
								OrderBy<Asc<POOrderS.orderDate>>>(this);
			foreach (POOrderS it in select.Select())
			{
				if (!this.skipCheck)
				{
					if (it.ExpirationDate.HasValue && it.ExpirationDate < row.OrderDate)
					{
						this.openOrders.Cache.RaiseExceptionHandling<POOrderS.expirationDate>(it, it.ExpirationDate, new PXSetPropertyException(Messages.POBlanketOrderExpiresBeforeTheDateOfDocument, PXErrorLevel.RowWarning, it.ExpirationDate.Value));
					}
				}
				yield return it;
			}
		}  
#endif

		#endregion

		#endregion

		#region Buttons

		public PXInitializeState<POOrder> initializeState;

		public PXAction<POOrder> hold;

		[PXUIField(DisplayName = "Hold", Visible = false,
			MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual IEnumerable Hold(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<POOrder> putOnHold;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Hold")]
		protected virtual IEnumerable PutOnHold(PXAdapter adapter) => adapter.Get<POOrder>();

		public PXAction<POOrder> releaseFromHold;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Remove Hold")]
		protected virtual IEnumerable ReleaseFromHold(PXAdapter adapter) => adapter.Get<POOrder>();

		public PXAction<POOrder> cancelOrder;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Cancel Order", MapEnableRights = PXCacheRights.Select)]
		protected virtual IEnumerable CancelOrder(PXAdapter adapter) => adapter.Get();

		public PXAction<POOrder> reopenOrder;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Reopen Order", MapEnableRights = PXCacheRights.Select)]
		public virtual IEnumerable ReopenOrder(PXAdapter adapter) => adapter.Get();

		public PXAction<POOrder> markAsDontEmail;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Do Not Email", MapEnableRights = PXCacheRights.Select)]
		protected virtual IEnumerable MarkAsDontEmail(PXAdapter adapter) => adapter.Get();

		public PXAction<POOrder> markAsDontPrint;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Do Not Print", MapEnableRights = PXCacheRights.Select)]
		protected virtual IEnumerable MarkAsDontPrint(PXAdapter adapter) => adapter.Get();

		public override int ExecuteUpdate(string viewName, IDictionary keys, IDictionary values, params object[] parameters)
		{
			if (viewName.ToLower() == "document" && values != null)
			{
				if (IsImport || IsExport || IsMobile || IsContractBasedAPI)
				{
					Document.Cache.Locate(keys);
					if (values.Contains("Hold") && values["Hold"] != PXCache.NotSetValue && values["Hold"] != null)
					{
						var hold = Document.Current.Hold ?? false;
						if (Convert.ToBoolean(values["Hold"]) != hold)
						{
							((PXAction<POOrder>)this.Actions["hold"]).PressImpl(false);
							values["Hold"] = PXCache.NotSetValue;
						}
					}
				}
			}
			return base.ExecuteUpdate(viewName, keys, values, parameters);
		}

		public PXAction<POOrder> action;
		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.ActionsFolder, CommitChanges = true, MenuAutoOpen = true)]
		protected virtual IEnumerable Action(PXAdapter adapter,
			[PXInt] [PXIntList(new int[] { 1, 2 }, new string[] { "Persist", "Update" })] int? actionID,
			[PXBool] bool refresh,
			[PXString] string actionName
		)
		{
			List<POOrder> result = new List<POOrder>();
			if (actionName != null)
			{
				PXAction a = this.Actions[actionName];
				if (a != null)
					foreach (PXResult<POOrder> e in a.Press(adapter))
						result.Add(e);
			}
			else
				foreach (POOrder e in adapter.Get<POOrder>())
					result.Add(e);

			if (refresh)
			{
				foreach (POOrder order in result)
					Document.Search<POOrder.orderNbr>(order.OrderNbr, order.OrderType);
			}
			switch (actionID)
			{
				case 1:
					Save.Press();
					break;
				case 2:
					break;
			}
			return result;
		}

		public PXAction<POOrder> complete;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Complete Order")]
		protected virtual IEnumerable Complete(PXAdapter adapter)
		{
			foreach (var row in adapter.Get())
			{
				POOrder doc = (POOrder)(row is PXResult pxResult ? pxResult[0] : row);
				Document.Current = doc;

				POReceipt receipt = PXSelectJoin<POReceipt,
				InnerJoin<POOrderReceipt, On<POOrderReceipt.FK.Receipt>>,
				Where<POOrderReceipt.pOType, Equal<Required<POOrder.orderType>>,
					And<POOrderReceipt.pONbr, Equal<Required<POOrder.orderNbr>>,
					And<POReceipt.released, Equal<False>>>>>.Select(this, doc.OrderType, doc.OrderNbr);
				if (receipt != null)
				{
					throw new PXException(Messages.POOrderHasUnreleaseReceiptsAndCantBeCompleted, doc.OrderNbr);
				}

				foreach (POLine line in this.Transactions.Select())
				{
					if (line.Completed == true) continue;

					POLine upd = (POLine)this.Transactions.Cache.CreateCopy(line);
					upd.Completed = true;
					using (new SuppressOrderEventsScope(this))
						this.Transactions.Update(upd);
				}

				yield return row;
			}
		}

		public PXAction<POOrder> notification;

		[PXUIField(DisplayName = "Notifications", Visible = false)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
		protected virtual IEnumerable Notification(PXAdapter adapter,
			[PXString] string notificationCD)
		{
			bool massProcess = adapter.MassProcess;
			var orders = adapter.Get<POOrder>().ToArray();

			PXLongOperation.StartOperation(this, () =>
			{
				var orderEntry = Lazy.By(() => PXGraph.CreateInstance<POOrderEntry>());
				bool anyfailed = false;

				foreach (POOrder order in orders)
				{
					if (massProcess) PXProcessing<POOrder>.SetCurrentItem(order);

					try
					{
						var parameters = new Dictionary<string, string>();
						parameters["POOrder.OrderType"] = order.OrderType;
						parameters["POOrder.OrderNbr"] = order.OrderNbr;

						using (var ts = new PXTransactionScope())
						{
							orderEntry.Value.Document.Current = order;

							orderEntry.Value.GetExtension<POOrderEntry_ActivityDetailsExt>().SendNotification(APNotificationSource.Vendor, notificationCD, order.BranchID, parameters);
							orderEntry.Value.Document.Update(order);
							orderEntry.Value.Save.Press();

							ts.Complete();
						}

						if (massProcess) PXProcessing<POOrder>.SetProcessed();
					}
					catch (Exception exception) when (massProcess)
					{
						PXProcessing<POOrder>.SetError(exception);
						anyfailed = true;
					}
				}

				if (anyfailed)
				{
					throw new PXOperationCompletedWithErrorException(ErrorMessages.SeveralItemsFailed);
				}
			});

			return orders;
		}

		public PXAction<POOrder> emailPurchaseOrder;
		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = "Email Purchase Order", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable EmailPurchaseOrder(
			PXAdapter adapter, 
			[PXString] 
			string notificationCD = null) => Notification(adapter, notificationCD ?? "PURCHASE ORDER");

		#region Reports

		public PXAction<POOrder> report;

		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.ReportsFolder, CommitChanges = true, MenuAutoOpen = true)]
		protected virtual IEnumerable Report(PXAdapter adapter,
			[PXInt] [PXIntList(new int[] { 1, 2 }, new string[] { "Vendor Details", "Activities" })] int? inquiryID,
			[PXString(8, InputMask = "CC.CC.CC.CC")] string reportID,
			[PXBool] bool sendByEmail,
			[PXBool] bool refresh
			)
		{
			List<POOrder> list = adapter.Get<POOrder>().ToList();
			if (!String.IsNullOrEmpty(reportID))
			{
				int i = 0;
				string actualReportID = null;
				string mailingReportID = null;
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				Dictionary<string, string> mailingParameters = new Dictionary<string, string>();
				PXReportRequiredException ex = null;
				Dictionary<PX.SM.PrintSettings, PXReportRequiredException> reportsToPrint = new Dictionary<PX.SM.PrintSettings, PXReportRequiredException>();

				foreach (POOrder order in list)
				{
					parameters = new Dictionary<string, string>();
					parameters["POOrder.OrderType"] = order.OrderType;
					parameters["POOrder.OrderNbr"] = order.OrderNbr;

					actualReportID = new NotificationUtility(this).SearchVendorReport(reportID, order.VendorID, order.BranchID);
					ex = PXReportRequiredException.CombineReport(ex, actualReportID, parameters, OrganizationLocalizationHelper.GetCurrentLocalization(this));
					ex.Mode = PXBaseRedirectException.WindowMode.New;

					reportsToPrint = PX.SM.SMPrintJobMaint.AssignPrintJobToPrinter(reportsToPrint, parameters, adapter,
						new NotificationUtility(this).SearchPrinter, APNotificationSource.Vendor, reportID, actualReportID,
						order.BranchID, OrganizationLocalizationHelper.GetCurrentLocalization(this));

					Location loc =
						PXSelect<Location,
						Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
						And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(this, order.VendorID,
					order.VendorLocationID);

					mailingParameters["POOrder.OrderType" + i.ToString()] = order.OrderType;
					mailingParameters["POOrder.OrderNbr" + i.ToString()] = order.OrderNbr;

					i++;
					if (mailingReportID == null)
						mailingReportID = new NotificationUtility(this).SearchVendorReport(reportID, order.VendorID, order.BranchID);

					if (refresh)
					{
						Document.Search<POOrder.orderNbr>(order.OrderNbr, order.OrderType);

						POOrder.Events
							.Select(ev => ev.Printed)
							.FireOn(this, Document.Current);
					}
				}

				if (ex != null)
				{
					if (sendByEmail)
					{
						try
						{
							ReportNotificationGenerator reportNotificationGenerator = ReportNotificationGeneratorFactory(actualReportID);
							reportNotificationGenerator.Parameters = mailingParameters;

							if (reportNotificationGenerator.Send().Any())
							{
								this.SelectTimeStamp();
								Save.Press();
							}
							else
							{
								throw new PXException(ErrorMessages.MailSendFailed);
							}
						}
						finally
						{
							Clear();
						}
					}
					else
					{
						Save.Press();

						LongOperationManager.StartAsyncOperation(ct=>PX.SM.SMPrintJobMaint.CreatePrintJobGroups(reportsToPrint, ct));

						throw ex;
					}
				}
			}
			else if (inquiryID.HasValue)
			{
				switch (inquiryID)
				{
					case 1:
						if (vendor.Current != null)
						{
							APDocumentEnq graph = PXGraph.CreateInstance<APDocumentEnq>();
							graph.Filter.Current.VendorID = vendor.Current.BAccountID;
							graph.Filter.Select();
							throw new PXRedirectRequiredException(graph, "Vendor Details")
							{
								Mode = PXBaseRedirectException.WindowMode.New
							};
						}
						break;
				}
			}
			return list;
		}

		public PXAction<POOrder> vendorDetails;
		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = "Vendor Details", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable VendorDetails(PXAdapter adapter)
			=> Report(adapter.Apply(a => a.Menu = "Vendor Details"), 1, null, false, false);

		public PXAction<POOrder> printPurchaseOrder;
		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = "Print Purchase Order", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable PrintPurchaseOrder(PXAdapter adapter)
			=> Report(adapter.Apply(a => a.Menu = "Print Purchase Order"), null, "PO641000", false, true);

		public PXAction<POOrder> viewPurchaseOrderReceipt;
		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = "Purchase Order Receipt and Billing History", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable ViewPurchaseOrderReceipt(PXAdapter adapter)
			=> Report(adapter.Apply(a => a.Menu = "Purchase Order Receipt and Billing History"), null, "PO643000", false, false);
		
		#endregion

		public PXAction<POOrder> addPOOrder;

		[PXUIField(DisplayName = Messages.AddBlanketOrder, MapEnableRights = PXCacheRights.Select,
			MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable AddPOOrder(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				 this.Document.Current.Hold == true &&
				 POOrderType.IsUseBlanket(this.Document.Current.OrderType))
			{
				if (this.openOrders.AskExt() == WebDialogResult.OK)
				{
					//this.skipCheck = true;		
					foreach (POOrderS it in this.openOrders.Cache.Updated)
					{
						if ((bool)it.Selected)
							this.AddPurchaseOrder(it);
						it.Selected = false;
					}
					//this.skipCheck = false;
				}
				else
				{
					//this.skipCheck = true;		
					foreach (POOrderS it in this.openOrders.Cache.Updated)
						it.Selected = false;
					//this.skipCheck = false;	
				}
			}
			return adapter.Get();
		}

		public PXAction<POOrder> addPOOrderLine;

		[PXUIField(DisplayName = Messages.AddBlanketLine, MapEnableRights = PXCacheRights.Select,
			MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable AddPOOrderLine(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
					this.Document.Current.Hold == true &&
					POOrderType.IsUseBlanket(this.Document.Current.OrderType))
			{
				bool containsRetainage = false;
				if (this.poLinesSelection.AskExt() == WebDialogResult.OK)
				{
					foreach (POLineS it in poLinesSelection.Cache.Updated)
					{
						if ((bool)it.Selected)
						{
							this.AddPOLine(it, this.filter.Current.OrderType == POOrderType.Blanket);
							containsRetainage |= (it.RetainagePct != 0m);
						}
						it.Selected = false;
					}
				}
				else
				{
					foreach (POLineS it in poLinesSelection.Cache.Updated)
						it.Selected = false;
				}

				if (containsRetainage)
				{
					bool updateDoc = EnableRetainage();
					if (updateDoc)
					{
						this.Document.Update(this.Document.Current);
					}
				}
				filter.Cache.Clear();
				poLinesSelection.Cache.Clear();
				poLinesSelection.Cache.ClearQueryCache();
			}

			return adapter.Get();
		}

		public PXAction<POOrder> createPOReceipt;

		[PXUIField(DisplayName = Messages.CreatePOReceipt, MapEnableRights = PXCacheRights.Select,
			MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXProcessButton]
		public virtual IEnumerable CreatePOReceipt(PXAdapter adapter)
		{
			if (this.Document.Current != null && 
				this.Document.Current.OrderType.IsIn(POOrderType.RegularOrder, POOrderType.DropShip, POOrderType.ProjectDropShip))
			{
				POOrder order = (POOrder)this.Document.Current;
				if (order.Status == POOrderStatus.Open)
				{
					ValidateLines();
					bool needsPOReceipt = false;
					foreach (POLine iLn in this.Transactions.Select())
					{
						if (NeedsPOReceipt(iLn, POSetup.Current))
						{
							needsPOReceipt = true;
							break;
						}
					}
					if (needsPOReceipt)
					{
						Save.Press();

						PXLongOperation.StartOperation(this, () => CreateInstance<POReceiptEntry>().CreateReceiptFrom(order, redirect: true));
					}
					else
					{
						throw new PXException(Messages.POHasNoItemsToReceive, order.OrderNbr);
					}
				}
			}
			return adapter.Get();
		}

		public PXAction<POOrder> createAPInvoice;

		[PXUIField(DisplayName = Messages.CreateAPInvoice, MapEnableRights = PXCacheRights.Select, 
			MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
        [APMigrationModeDependentActionRestriction(
            restrictInMigrationMode: true,
            restrictForRegularDocumentInMigrationMode: true,
            restrictForUnreleasedMigratedDocumentInNormalMode: true)]
        public virtual IEnumerable CreateAPInvoice(PXAdapter adapter)
		{
			if (this.Document.Current != null &&
				 (POOrderType.IsNormalType(this.Document.Current.OrderType) || this.Document.Current.OrderType.IsIn(POOrderType.DropShip, POOrderType.ProjectDropShip)))
			{
				POOrder order = (POOrder)this.Document.Current;
				if (order.Status.IsIn(POOrderStatus.AwaitingLink, POOrderStatus.Open, POOrderStatus.Completed))
				{
					ValidateLines();
					if (this.NeedsAPInvoice())
					{
						this.Save.Press();
						APInvoiceEntry invoiceGraph = PXGraph.CreateInstance<APInvoiceEntry>();
						bool hasRetainedTaxes = apsetup.Current.RetainTaxes == true && order.RetainageApply == true;
						invoiceGraph.InvoicePOOrder(order, true, keepOrderTaxes: !hasRetainedTaxes);
						invoiceGraph.AttachPrepayment();
						throw new PXRedirectRequiredException(invoiceGraph, Messages.POReceiptRedirection);
					}
					else
					{
						throw new PXException(Messages.APInvoicePOOrderCreation_NoApplicableLinesFound);
					}
				}
			}
			return adapter.Get();
		}

		public PXAction<POOrder> validateAddresses;

		[PXUIField(DisplayName = CS.Messages.ValidateAddresses, MapEnableRights = PXCacheRights.Select,
			MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		[PXButton]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			foreach (POOrder current in adapter.Get<POOrder>())
			{
				if (current != null)
				{
					FindAllImplementations<IAddressValidationHelper>().ValidateAddresses();
				}
				yield return current;
			}
		}

		protected virtual void AddPurchaseOrder(POOrderS aOrder)
		{

			PXSelectBase<POLineS> sel = new PXSelect<POLineS,
																Where<POLineS.orderType, Equal<Required<POLine.orderType>>,
																	And<POLineS.orderNbr, Equal<Required<POLine.orderNbr>>,
																	And<POLineS.cancelled, NotEqual<boolTrue>,
																	And<POLineS.completed, NotEqual<boolTrue>,
																	And<Where<POLineS.orderQty, Equal<decimal0>, Or<POLineS.openQty, Greater<decimal0>>>>>>>>>(this);
			bool containsRetainage = false;
			foreach (POLineS iLn in sel.Select(aOrder.OrderType, aOrder.OrderNbr))
			{
				AddPOLine(iLn, aOrder.OrderType == POOrderType.Blanket);
				containsRetainage |= (iLn.RetainagePct != 0m);
			}

			bool updateDoc = false;
			POOrder order = this.Document.Current;
			if (string.IsNullOrEmpty(order.VendorRefNbr))
			{
				order.VendorRefNbr = aOrder.VendorRefNbr;
				updateDoc = true;
			}
			if (containsRetainage)
			{
				updateDoc |= EnableRetainage();
			}

			if (updateDoc)
			{
				order = this.Document.Update(order);
			}
		}

		protected virtual bool EnableRetainage()
		{
			if (Document.Current.RetainageApply == true || POOrderType.IsNormalType(Document.Current.OrderType))
				return false;

			Document.Current.RetainageApply = true;
			Document.Cache.SetDefaultExt<POOrder.defRetainagePct>(Document.Current);
			Document.Cache.RaiseExceptionHandling<POOrder.retainageApply>(Document.Current, true,
				new PXSetPropertyException(Messages.AutoAppliedRetainageFromOrder, PXErrorLevel.Warning));

			return true;
		}

		protected virtual void AddPOLine(POLineS aLine, bool blanked)
		{
			POLine line = null;
			if (blanked)
				foreach (POLine iLn in this.Transactions.Select())
					if (iLn.POType == aLine.OrderType &
							iLn.PONbr == aLine.OrderNbr &&
							iLn.POLineNbr == aLine.LineNbr)
					{
						line = iLn;
						break;
					}

			if (line == null)
			{
				line = new POLine();

				line.BranchID = aLine.BranchID;
				line.InventoryID = aLine.InventoryID;
				line.SubItemID = aLine.SubItemID;
				line.SiteID = aLine.SiteID;
				line.TaxCategoryID = aLine.TaxCategoryID;
				line.TranDesc = aLine.TranDesc;
				line.UnitCost = aLine.UnitCost;
				line.UnitVolume = aLine.UnitVolume;
				line.UnitWeight = aLine.UnitWeight;
				line.UOM = aLine.UOM;
				line.AlternateID = aLine.AlternateID;
				line.CuryUnitCost = aLine.CuryUnitCost;
				line.ManualPrice = aLine.ManualPrice;
				line.ExpenseAcctID = aLine.ExpenseAcctID;
				line.ExpenseSubID = aLine.ExpenseSubID;
				line.RcptQtyMin = aLine.RcptQtyMin;
				line.RcptQtyMax = aLine.RcptQtyMax;
				line.RcptQtyThreshold = aLine.RcptQtyThreshold;
				line.RcptQtyAction = aLine.RcptQtyAction;
				line.POType = aLine.OrderType;
				line.PONbr = aLine.OrderNbr;
				line.POLineNbr = aLine.LineNbr;
				line.ProjectID = aLine.ProjectID;
				line.TaskID = aLine.TaskID;
				line.CostCodeID = aLine.CostCodeID;

				if (blanked)
				{
					line.OrderQty = aLine.OpenQty;
				}
				else
				{
					line.OrderQty = aLine.OrderQty;
				}
				if(aLine.LineType == POLineType.Freight || aLine.OrderQty == 0)
					line.CuryLineAmt = aLine.CuryLineAmt;

				line.OpenQty = line.OrderQty;

				line.RetainagePct = aLine.RetainagePct;
				bool partiallyOrdered = (aLine.OpenQty != aLine.OrderQty);
				if (blanked && !partiallyOrdered)
				{
					line.CuryRetainageAmt = aLine.CuryRetainageAmt;
					line.RetainageAmt = aLine.RetainageAmt;
				}

				line = this.Transactions.Insert(line);
				if (line.LineType != aLine.LineType && POLineType.IsDefault(line.LineType))
				{
					//field-level events should not be fired, it will lead to OrderQty being reset
					var copy = PXCache<POLine>.CreateCopy(line);
					line.LineType = aLine.LineType;
					this.Transactions.Cache.RaiseRowUpdated(line, copy);
				}
			}
		}

		public PXAction<POOrder> recalculateDiscountsAction;

		[PXUIField(DisplayName = "Recalculate Prices", MapEnableRights = PXCacheRights.Select,
			MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXButton]
		public virtual IEnumerable RecalculateDiscountsAction(PXAdapter adapter)
		{
			if (adapter.MassProcess)
			{
				PXLongOperation.StartOperation(this, () => this.RecalculateDiscountsProc(false));
			}
			else if (adapter.ExternalCall == false || this.IsImport == true)
				{
				this.RecalculateDiscountsProc(true);
			}
			else if (recalcdiscountsfilter.AskExt() == WebDialogResult.OK)
			{
				POOrderEntry clone = this.Clone();
				PXLongOperation.StartOperation(this, () => clone.RecalculateDiscountsProc(true));
			}
			return adapter.Get();
		}

		protected virtual void RecalculateDiscountsProc(bool redirect)
		{
			_discountEngine.RecalculatePricesAndDiscounts(
				Transactions.Cache,
				Transactions,
				Transactions.Current,
				DiscountDetails,
				Document.Current.VendorLocationID,
				Document.Current.OrderDate,
				recalcdiscountsfilter.Current,
				DiscountEngine.DefaultAPDiscountCalculationParameters);
			if (redirect)
			{
				PXLongOperation.SetCustomInfo(this);
			}
			else
			{
				this.Save.Press();
			}
		}

		public PXAction<POOrder> recalcOk;

		[PXUIField(DisplayName = "OK", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable RecalcOk(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<POOrder> viewBlanketOrder;
		[PXUIField(DisplayName = Messages.ViewParentOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewBlanketOrder(PXAdapter adapter)
		{
			PXRedirectHelper.TryRedirect(
				Document.Cache,
				POLine.FK.BlanketOrder.FindParent(this, Transactions.Current),
				Messages.ViewParentOrder,
				PXRedirectHelper.WindowMode.NewWindow);
		
			return adapter.Get();
		}

		#endregion

		#region Internal Functions

		protected virtual void ParentFieldUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<POOrder.orderDate, POOrder.curyID>(e.Row, e.OldRow))
			{
				var order = e.Row as POOrder;

				if (order != null && order.LinesStatusUpdated != true)
				{
					foreach (POLine tran in Transactions.Select())
					{
						Transactions.Cache.MarkUpdated(tran, assertError: true);
					}
					order.LinesStatusUpdated = true;
				}
			}
		}

		private object GetAcctSub<Field>(PXCache cache, object data) where Field : IBqlField
		{
			object NewValue = cache.GetValueExt<Field>(data);
			if (NewValue is PXFieldState)
			{
				return ((PXFieldState)NewValue).Value;
			}
			else
			{
				return NewValue;
			}
		}

		public static bool NeedsPOReceipt(POLine aLine, bool skipCompleted, POSetup poSetup)
		{
			if (skipCompleted && (aLine.Completed == true || aLine.Cancelled == true))
			{
				return false;
			}

			if (aLine.OrderType == POOrderType.ProjectDropShip && aLine.DropshipReceiptProcessing == DropshipReceiptProcessingOption.SkipReceipt)
			{
				return false;
			}

			if (aLine.LineType.IsIn(
					POLineType.GoodsForDropShip,
					POLineType.NonStockForDropShip,
					POLineType.NonStockForSalesOrder,
					POLineType.NonStockForServiceOrder,
					POLineType.NonStockForManufacturing,
					POLineType.GoodsForInventory,
					POLineType.GoodsForSalesOrder,
					POLineType.GoodsForServiceOrder,
					POLineType.GoodsForManufacturing,
					POLineType.GoodsForReplenishment,
					POLineType.NonStock,
					POLineType.Freight,
					POLineType.GoodsForProject,
					POLineType.NonStockForProject)
				|| aLine.LineType == POLineType.Service
					&& (aLine.POAccrualType == POAccrualType.Receipt
					|| poSetup != null && (aLine.OrderType == POOrderType.DropShip && poSetup.AddServicesFromDSPOtoPR == true
					|| aLine.OrderType == POOrderType.RegularOrder && (poSetup.AddServicesFromNormalPOtoPR == true || aLine.ProcessNonStockAsServiceViaPR == true))))
			{
				return true;
			}
			return false;
		}

		public virtual bool NeedsPOReceipt(POLine aLine, POSetup poSetup)
		{
			return POOrderEntry.NeedsPOReceipt(aLine, true, poSetup);
		}

		public virtual bool NeedsAPInvoice()
		{
			foreach (POLine insertedLine in this.Transactions.Cache.Inserted)
			{
				if (NeedsAPInvoice(insertedLine, true, this.POSetup.Current))
				{
					return true;
				}
			}
			foreach (POLine persistedLine in PXSelectReadonly2<POLine,
				LeftJoin<APTran, On<APTran.pOAccrualRefNoteID, Equal<POLine.orderNoteID>, And<APTran.pOAccrualLineNbr, Equal<POLine.lineNbr>,
					And<APTran.released, Equal<False>>>>>,
				Where<POLine.orderType, Equal<Current<POOrder.orderType>>, And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>,
					And<POLine.pOAccrualType, Equal<POAccrualType.order>,
					And<APTran.refNbr, IsNull>>>>>
				.Select(this))
			{
				if (NeedsAPInvoice(persistedLine, true, this.POSetup.Current) && this.Transactions.Cache.GetStatus(persistedLine) != PXEntryStatus.Deleted)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool NeedsAPInvoice(POLine aLine, bool skipCompleted, POSetup poSetup)
		{
			if (skipCompleted)
			{
				if (aLine.Closed == true || aLine.Cancelled == true)
					return false;

				bool billed = (aLine.CompletedQty <= aLine.BilledQty);
				if (billed)
				{
					billed = (aLine.CompletePOLine == CompletePOLineTypes.Quantity)
						? (aLine.OrderQty > 0 && aLine.OrderQty * aLine.RcptQtyThreshold / 100m <= aLine.BilledQty)
						: ((aLine.CuryExtCost + aLine.CuryRetainageAmt) * aLine.RcptQtyThreshold / 100m <= aLine.CuryBilledAmt);
					if (billed)
						return false;
				}
			}
			if (aLine.POAccrualType == POAccrualType.Order
				|| aLine.LineType == POLineType.MiscCharges)
			{
				return true;
			}
			return false;
		}

		public virtual void ValidateLines()
		{
			bool validationFailed = false;
			foreach (POLine line in Transactions.Select())
			{
				if (line.TaskID != null)
				{
					PMProject project = PXSelectorAttribute.Select<POLine.projectID>(Transactions.Cache, line) as PMProject;
					if (project.IsActive != true)
					{
						PXUIFieldAttribute.SetError<POLine.projectID>(Transactions.Cache, line, Messages.ProjectIsNotActive, project.ContractCD);
						validationFailed = true;
					}
					else
					{
						PMTask task = PXSelectorAttribute.Select<POLine.taskID>(Transactions.Cache, line) as PMTask;
						if (task.IsActive != true)
						{
							PXUIFieldAttribute.SetError<POLine.taskID>(Transactions.Cache, line, Messages.ProjectTaskIsNotActive, task.TaskCD);
							validationFailed = true;
						}
					}
				}
			}

			if (validationFailed)
			{
				throw new PXException(Messages.LineIsInvalid);
			}
		}

		internal void UpdateSOLine(SOLineSplit3 split, int? vendorID, bool poCreated)
		{
			bool setVendor = split.VendorID != vendorID;
			bool setPOCreated = split.POCreated != poCreated;
			if (setVendor || setPOCreated)
			{
				SOLine5 origsoline = (SOLine5)FixedDemandOrigSOLine.Select(split.OrderType, split.OrderNbr, split.LineNbr);
				bool changed = false;
				if (setVendor)
				{
					split.VendorID = vendorID;
					if (origsoline != null && origsoline.VendorID != vendorID)
					{
						origsoline.VendorID = vendorID;
						changed = true;
					}
				}
				if (setPOCreated)
				{
					split.POCreated = poCreated;
					if (origsoline != null && origsoline.POCreated != poCreated)
					{
						origsoline.POCreated = poCreated;
						changed = true;
					}
				}
				if (changed)
					FixedDemandOrigSOLine.Cache.MarkUpdated(origsoline, assertError: true);
			}
		}

		public virtual bool GetRequireControlTotal(string aOrderType)
		{
			bool result = false;
			switch (aOrderType)
			{
				case POOrderType.Blanket:
				case POOrderType.StandardBlanket:
					result = POSetup.Current.RequireBlanketControlTotal == true; 
					break;
				case POOrderType.RegularOrder:
					result = POSetup.Current.RequireOrderControlTotal == true;
					break;
				case POOrderType.DropShip:
					result = POSetup.Current.RequireDropShipControlTotal == true;
					break;
				case POOrderType.ProjectDropShip:
					result = POSetup.Current.RequireProjectDropShipControlTotal == true;
					break;
			}
			return result;
		}

		private bool IsZeroQuantityValid(POLine row)
		{
			if (POLineType.IsStock(row.LineType))
				return false;

			if (row.InventoryID == null)
				return true;

			if (row.CompletePOLine == CompletePOLineTypes.Quantity)
				return false;

			return row.POAccrualType != POAccrualType.Receipt;
		}

		private bool IsZeroUnitCostValid(POLine row)
		{
			if (POLineType.IsStock(row.LineType))
				return true;

			return row.CuryLineAmt != Decimal.Zero;
		}

		#endregion

		#region Field order (API and Copy-Paste) overrides

		protected override List<KeyValuePair<string, List<FieldInfo>>> AdjustApiScript(List<KeyValuePair<string, List<FieldInfo>>> fieldsByView)
		{
			List<KeyValuePair<string, List<FieldInfo>>> script = base.AdjustApiScript(fieldsByView);

			List<FieldInfo> documentViewScript = script.FirstOrDefault(x => x.Key == nameof(Document)).Value;
			if (documentViewScript != null)
			{
				FieldInfo expectedDateInfo = documentViewScript.FirstOrDefault(x => x.FieldName == nameof(POOrder.ExpectedDate));
				if (expectedDateInfo != null)
				{
					documentViewScript.Remove(expectedDateInfo);
					documentViewScript.Add(expectedDateInfo);
				}
			}

			return script;
		}

		public override void CopyPasteGetScript(bool isImportSimple, List<PX.Api.Models.Command> script, List<PX.Api.Models.Container> containers)
		{
			base.CopyPasteGetScript(isImportSimple, script, containers);

			int expectedDateIndex = script.FindIndex(x => x.FieldName == nameof(POOrder.ExpectedDate));
			if (expectedDateIndex == -1)
				return;

			Api.Models.Command cmd = script[expectedDateIndex];
			Api.Models.Command lastCmd = script.LastOrDefault(x => string.Equals(x.ObjectName, cmd.ObjectName));
			int expectedDateGroupLastIndex = script.IndexOf(lastCmd);

			Api.Models.Container cnt = containers[expectedDateIndex];

			script.RemoveAt(expectedDateIndex);
			containers.RemoveAt(expectedDateIndex);

			script.Insert(expectedDateGroupLastIndex, cmd);
			containers.Insert(expectedDateGroupLastIndex, cnt);
		}

		#endregion

		#region Events

		#region Internal Variables

		protected bool _ExceptionHandling = false;
		private bool _blockUIUpdate = false;

		public bool BlockUIUpdate => _blockUIUpdate;

		#endregion

		#region POOrder

		[InterBranchRestrictor(typeof(Where2<SameOrganizationBranch<INSite.branchID, Current<POOrder.branchID>>,
			Or<Current<POOrder.orderType>, Equal<POOrderType.standardBlanket>>>))]
		[PXMergeAttributes(Method = MergeMethod.Append)]
		protected virtual void POOrder_SiteID_CacheAttached(PXCache sender)	{ }

		protected virtual void POOrder_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			POOrder doc = e.Row as POOrder;

			if (doc == null)
			{
				return;
			}
			doc.RequestApproval = POSetup.Current.OrderRequestApproval;
			bool retainageApply = (doc.RetainageApply == true);
			bool isProjectDropShip = doc.OrderType == POOrderType.ProjectDropShip;
			bool vendorDiscountsFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>();

			PXUIFieldAttribute.SetVisible<POOrder.hold>(cache, null, true);
			PXUIFieldAttribute.SetRequired<POOrder.termsID>(cache, true);
			bool isBlanket = doc.OrderType == POOrderType.Blanket;
			PXUIFieldAttribute.SetVisible<POOrder.expirationDate>(cache, null, isBlanket);
			PXUIFieldAttribute.SetVisible<POOrder.expectedDate>(cache, null, !isBlanket);
			PXUIFieldAttribute.SetVisible<POOrder.curyID>(cache, null, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

			if (isProjectDropShip)
			{
				createAPInvoice.SetDisplayOnMainToolbar(doc.DropshipReceiptProcessing == DropshipReceiptProcessingOption.SkipReceipt);
				PXStringListAttribute.SetList<POOrder.shipDestType>(Document.Cache, null, new string[] { POShippingDestination.ProjectSite }, new string[] { Messages.ShipDestProjectSite });
			}
			else
			{
				PXStringListAttribute.SetList<POOrder.shipDestType>(Document.Cache, null, new string[] 
				{ 
					POShippingDestination.CompanyLocation, POShippingDestination.Customer, POShippingDestination.Vendor, POShippingDestination.Site }, 
					new string[] { Messages.ShipDestCompanyLocation, Messages.ShipDestCustomer, Messages.ShipDestVendor, Messages.ShipDestSite
				});
			}

			PXUIFieldAttribute.SetEnabled<POOrder.defRetainagePct>(cache, null, retainageApply);
			PXUIFieldAttribute.SetVisible<POOrder.curyRetainageTotal>(cache, null, retainageApply);
			PXUIFieldAttribute.SetEnabled<POOrder.curyRetainageTotal>(cache, null, retainageApply);
			PXUIFieldAttribute.SetVisible<POLine.retainagePct>(Transactions.Cache, null, retainageApply);
			PXUIFieldAttribute.SetVisible<POLine.curyRetainageAmt>(Transactions.Cache, null, retainageApply);
			PXUIFieldAttribute.SetVisible<POTaxTran.curyRetainedTaxableAmt>(Taxes.Cache, null, retainageApply);
			PXUIFieldAttribute.SetEnabled<POTaxTran.curyRetainedTaxableAmt>(Taxes.Cache, null, retainageApply);
			PXUIFieldAttribute.SetVisible<POTaxTran.curyRetainedTaxAmt>(Taxes.Cache, null, retainageApply);
			PXUIFieldAttribute.SetEnabled<POTaxTran.curyRetainedTaxAmt>(Taxes.Cache, null, retainageApply);
			PXUIFieldAttribute.SetVisible<POOrderDiscountDetail.curyRetainedDiscountAmt>(DiscountDetails.Cache, null, retainageApply);
			PXUIFieldAttribute.SetEnabled<POOrderDiscountDetail.curyRetainedDiscountAmt>(DiscountDetails.Cache, null, retainageApply);
			PXUIFieldAttribute.SetVisible<POOrder.sOOrderType>(cache, null, !isProjectDropShip);
			PXUIFieldAttribute.SetVisible<POOrder.sOOrderNbr>(cache, null, !isProjectDropShip);
			PXUIFieldAttribute.SetVisible<POOrder.rQReqNbr>(cache, doc, !isProjectDropShip);
			PXUIFieldAttribute.SetVisible<POLine.siteID>(Transactions.Cache, null, !isProjectDropShip);
			PXUIFieldAttribute.SetEnabled(this.poLinesSelection.Cache, null, false);
			PXUIFieldAttribute.SetEnabled(this.openOrders.Cache, null, false);

			bool isPMVisible = PXAccess.FeatureInstalled<FeaturesSet.projectModule>();
			bool requireSingleProject = RequireSingleProject(doc);
			PXUIFieldAttribute.SetVisible<POOrder.projectID>(cache, null, isPMVisible && requireSingleProject);
			PXDefaultAttribute.SetPersistingCheck<POOrder.projectID>(cache, null,
				(isPMVisible && requireSingleProject) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			if (doc.Cancelled != true && doc.Status.IsNotIn(POOrderStatus.Completed, POOrderStatus.Closed))
			{
				if (!this._blockUIUpdate)
				{
					PXUIFieldAttribute.SetEnabled(cache, doc, true);
					PXUIFieldAttribute.SetEnabled<POOrder.status>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyOrderTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyDiscTot>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyDetailExtCostTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyLineTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyTaxTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.openOrderQty>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyUnbilledLineTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyUnbilledOrderTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.unbilledOrderQty>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.rQReqNbr>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyUnbilledTaxTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.shipToBAccountID>(cache, doc, doc.ShipDestType != POShippingDestination.CompanyLocation && IsShipToBAccountRequired(doc));
					PXUIFieldAttribute.SetEnabled<POOrder.shipToLocationID>(cache, doc, IsShipToBAccountRequired(doc));
					PXUIFieldAttribute.SetEnabled<POOrder.siteID>(cache, doc, (doc.ShipDestType == POShippingDestination.Site));
					PXUIFieldAttribute.SetEnabled<POOrder.curyVatExemptTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyVatTaxableTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.orderBasedAPBill>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.pOAccrualType>(cache, doc, false);

					PXUIFieldAttribute.SetRequired<POOrder.siteID>(cache, (doc.OrderType != POOrderType.RegularSubcontract));
					PXUIFieldAttribute.SetRequired<POOrder.shipToBAccountID>(cache, IsShipToBAccountRequired(doc));
					PXUIFieldAttribute.SetRequired<POOrder.shipToLocationID>(cache, IsShipToBAccountRequired(doc));

					PXUIFieldAttribute.SetEnabled<POOrder.hold>(cache, doc, true);
					PXUIFieldAttribute.SetEnabled<POOrder.termsID>(cache, doc, true);
					PXUIFieldAttribute.SetEnabled<POOrder.expectedDate>(cache, doc, true);
					PXUIFieldAttribute.SetEnabled<POOrder.cancelled>(cache, doc, doc.Status == POOrderStatus.Open);//todo move to WF
					PXUIFieldAttribute.SetEnabled<POOrder.retainageApply>(cache, doc, doc.OrderType != POOrderType.DropShip);//todo move to WF

					PXUIFieldAttribute.SetEnabled<POOrder.projectID>(cache, null, isPMVisible && requireSingleProject);

					PXUIFieldAttribute.SetEnabled<POOrderS.selected>(this.openOrders.Cache, null, true);
					PXUIFieldAttribute.SetEnabled<POLineS.selected>(this.poLinesSelection.Cache, null, true);

					PXUIFieldAttribute.SetEnabled<POOrder.curyLineDiscTotal>(cache, doc, false);
					PXUIFieldAttribute.SetEnabled<POOrder.curyDiscTot>(cache, doc, !vendorDiscountsFeatureInstalled);
					PXUIFieldAttribute.SetEnabled<POOrder.shipDestType>(Document.Cache, null, !isProjectDropShip);

					PXUIFieldAttribute.SetEnabled<POOrder.defRetainagePct>(cache, null, retainageApply);
				}
			}

			if (POSetup.Current.OrderRequestApproval != true && doc.Approved == true)
			{
				PXUIFieldAttribute.SetVisible<POOrder.approved>(cache, doc, false);
			}
			else
			{
				PXUIFieldAttribute.SetVisible<POOrder.approved>(cache, doc, true);
				PXDefaultAttribute.SetPersistingCheck<POOrder.ownerID>(cache, doc,
					POSetup.Current.OrderRequestApproval != true && doc.Approved == true
						? PXPersistingCheck.Nothing
						: PXPersistingCheck.NullOrBlank);
			}

			PXUIFieldAttribute.SetEnabled<POOrder.orderType>(cache, doc);
			PXUIFieldAttribute.SetEnabled<POOrder.orderNbr>(cache, doc);

			bool vendorSelected = doc.VendorID != null && doc.VendorLocationID != null;
			Transactions.Cache.AllowDelete = Transactions.Cache.AllowUpdate = Transactions.Cache.AllowInsert = vendorSelected;

			addPOOrder.SetEnabled(vendorSelected);
			addPOOrderLine.SetEnabled(vendorSelected);

			Taxes.Cache.AllowDelete = DiscountDetails.Cache.AllowDelete = Transactions.Cache.AllowDelete;
			Taxes.Cache.AllowUpdate = DiscountDetails.Cache.AllowUpdate = Transactions.Cache.AllowUpdate;
			Taxes.Cache.AllowInsert = DiscountDetails.Cache.AllowInsert = Transactions.Cache.AllowInsert;

			if (doc != null && vendor.Current != null && (bool)vendor.Current.TaxAgency == true)
			{
				// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Moved from suppression file]
				doc.TaxZoneID = null;
				PXUIFieldAttribute.SetEnabled<POOrder.taxZoneID>(cache, doc, false);
				PXUIFieldAttribute.SetEnabled<POLine.taxCategoryID>(Transactions.Cache, null, false);
			}

			//purchaseOrder.SetEnabled(doc != null && doc.Status == POOrderStatus.Open);

			if (doc.VendorID != null && this._blockUIUpdate == false)
			{
				if (doc.HasUsedLine == null)
				{
					POLine usedLines = PXSelect<POLine, Where<POLine.orderType, Equal<Required<POLine.orderType>>,
										And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
											And<Where<POLine.receivedQty, Greater<decimal0>,
												Or<POLine.completed, Equal<boolTrue>,
													Or<POLine.cancelled, Equal<boolTrue>>>>>>>>.Select(this, doc.OrderType, doc.OrderNbr);
					doc.HasUsedLine = (usedLines != null);
				}
				PXUIFieldAttribute.SetEnabled<POOrder.orderDate>(cache, doc, doc.HasUsedLine != true);
			}
			if (!this._blockUIUpdate)
			{
				bool showControlTotal = GetRequireControlTotal(doc.OrderType);
				PXUIFieldAttribute.SetVisible<POOrder.curyControlTotal>(cache, e.Row, showControlTotal);
				PXUIFieldAttribute.SetEnabled<POOrder.curyDiscTot>(cache, doc, !vendorDiscountsFeatureInstalled);
				PXUIFieldAttribute.SetVisible<POOrder.shipToBAccountID>(cache, doc, IsShipToBAccountRequired(doc));
				PXUIFieldAttribute.SetVisible<POOrder.shipToLocationID>(cache, doc, IsShipToBAccountRequired(doc));
				PXUIFieldAttribute.SetVisible<POOrder.siteID>(cache, doc, (doc.ShipDestType == POShippingDestination.Site));
				this.validateAddresses.SetEnabled(doc.Cancelled == false && FindAllImplementations<IAddressValidationHelper>().RequiresValidation());
			}



			if (doc != null && doc.ShipDestType == POShippingDestination.Site)
			{
				var siteIdErrorString = PXUIFieldAttribute.GetError<POOrder.siteID>(cache, e.Row);
				if (siteIdErrorString == null)
				{
					var siteIdErrorMessage = doc.SiteIdErrorMessage;
					if (!string.IsNullOrWhiteSpace(siteIdErrorMessage))
					{
						cache.RaiseExceptionHandling<POOrder.siteID>(e.Row, cache.GetValueExt<POOrder.siteID>(e.Row),
							new PXSetPropertyException(siteIdErrorMessage, PXErrorLevel.Error));
					}
				}
			}

			PXUIFieldAttribute.SetEnabled<POOrder.payToVendorID>(cache, doc, doc.Status.IsNotIn(POOrderStatus.Cancelled, POOrderStatus.Completed, POOrderStatus.Closed));

			if (doc != null)
			{
				bool blanket = (doc.OrderType == POOrderType.Blanket);

				Receipts.AllowSelect =
				APDocs.AllowSelect = !blanket;

				ChildOrdersReceipts.AllowSelect =
				ChildOrdersAPDocs.AllowSelect = blanket;
			}
		}

		public virtual bool IsShipToBAccountRequired(POOrder doc)
		{
			return doc.ShipDestType.IsNotIn(POShippingDestination.Site, POShippingDestination.ProjectSite);
		}

		protected virtual void POOrder_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			POOrder oldDoc = (POOrder)e.OldRow;
			if (doc == null) return;

			if (!sender.ObjectsEqual<POOrder.hold>(oldDoc, doc))
			{
				Transactions.Cache.ClearItemAttributes();
				//needed to reset field states to set them correctly afterwards - it helps to avoid unwanted effects in UI behavior when 
				//using PXUIFieldAttribute.SetEnabled(sender, row, false) or similar statements in RowSelected.
			}

			// Acuminator disable once PX1074 SetupNotEnteredExceptionInEventHandlers [Justification: new function around old check - RequireSingleProjectPerDocument is disabled in acuminator ignore file]
			if (!sender.ObjectsEqual<POOrder.projectID>(e.OldRow, e.Row) && RequireSingleProject(doc))
			{
				string dropshipReceiptProcessing = null;
				string dropshipExpenseRecording = null;
				if (doc.ProjectID != null && doc.OrderType == POOrderType.ProjectDropShip)
				{
					PMProject project = PMProject.PK.Find(this, doc.ProjectID);
					if (project != null)
					{
						if ((!PXAccess.FeatureInstalled<FeaturesSet.inventory>() && !PXAccess.FeatureInstalled<FeaturesSet.pOReceiptsWithoutInventory>()))
						{
							dropshipReceiptProcessing = DropshipReceiptProcessingOption.SkipReceipt;
							dropshipExpenseRecording = DropshipExpenseRecordingOption.OnBillRelease;
						}

						if (PXAccess.FeatureInstalled<FeaturesSet.inventory>())
						{
							dropshipReceiptProcessing = project.DropshipReceiptProcessing;
							dropshipExpenseRecording = project.DropshipExpenseRecording;
						}

						if (PXAccess.FeatureInstalled<FeaturesSet.pOReceiptsWithoutInventory>())
						{
							dropshipReceiptProcessing = DropshipReceiptProcessingOption.GenerateReceipt;
							dropshipExpenseRecording = DropshipExpenseRecordingOption.OnBillRelease;
						}
					}

					if (dropshipExpenseRecording == DropshipExpenseRecordingOption.OnReceiptRelease && INSetup.Current?.UpdateGL != true)
					{
						dropshipExpenseRecording = DropshipExpenseRecordingOption.OnBillRelease;
						sender.RaiseExceptionHandling<POOrder.projectID>(e.Row, null,
							new PXSetPropertyException(Messages.ProjectDropShipPostOnReceiptReleaseUpdateGLNotSet, sender.GetValueExt<POOrder.projectID>(e.Row)));
					}

					doc.DropshipReceiptProcessing = dropshipReceiptProcessing;
					doc.DropshipExpenseRecording = dropshipExpenseRecording;

					try
					{
						POShipAddressAttribute.DefaultRecord<POOrder.shipAddressID>(sender, e.Row);
					}
					catch (SharedRecordMissingException)
					{
						//consider ignoring SharedRecordMissingException exception
						sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
							new PXSetPropertyException(Messages.ShippingAddressMayNotBeEmpty, PXErrorLevel.Error));
					}
					try
					{
						POShipContactAttribute.DefaultRecord<POOrder.shipContactID>(sender, e.Row);
					}
					catch (SharedRecordMissingException)
					{
						//consider ignoring SharedRecordMissingException exception
						sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
							new PXSetPropertyException(Messages.ShippingContactMayNotBeEmpty, PXErrorLevel.Error));
					}
				}

				foreach (POLine line in Transactions.Select())
				{
					line.ProjectID = doc.ProjectID;
					if (doc.OrderType == POOrderType.ProjectDropShip)
					{
						line.DropshipReceiptProcessing = dropshipReceiptProcessing;
						line.DropshipExpenseRecording = dropshipExpenseRecording;
						line.POAccrualType = (string)PXFormulaAttribute.Evaluate<POLine.pOAccrualType>(Transactions.Cache, line);
					}
					Transactions.Update(line);
				}
				if (filter.Current?.OrderNbr != null)
				{
					filter.Current.OrderNbr = null;
				}
			}

			if (e.ExternalCall &&
				(!sender.ObjectsEqual<POOrder.orderDate>(e.OldRow, e.Row) ||
				!sender.ObjectsEqual<POOrder.vendorLocationID>(e.OldRow, e.Row)))
			{
				_discountEngine.AutoRecalculatePricesAndDiscounts(Transactions.Cache, Transactions,
					null, DiscountDetails, doc.VendorLocationID, doc.OrderDate, DiscountEngine.DefaultAPDiscountCalculationParameters);
			}

			if (!_discountEngine.IsInternalDiscountEngineCall && e.ExternalCall && sender.GetStatus(doc) != PXEntryStatus.Deleted &&
				!sender.ObjectsEqual<POOrder.curyDiscTot>(e.OldRow, e.Row))
			{
				_discountEngine.SetTotalDocDiscount(Transactions.Cache, Transactions, DiscountDetails,
					Document.Current.CuryDiscTot, DiscountEngine.DefaultAPDiscountCalculationParameters);
				RecalculateTotalDiscount();
			}

			if (doc.Cancelled != null && (bool)doc.Cancelled == false)
			{
				if (!GetRequireControlTotal(doc.OrderType))
				{
					if (doc.CuryOrderTotal != doc.CuryControlTotal)
					{
						if (doc.CuryOrderTotal != null && doc.CuryOrderTotal != 0)
							sender.SetValueExt<POOrder.curyControlTotal>(e.Row, doc.CuryOrderTotal);
						else
							sender.SetValueExt<POOrder.curyControlTotal>(e.Row, 0m);
					}
				}
			}
			if ((bool)doc.Hold == false && doc.Cancelled == false)
			{
				if (doc.CuryControlTotal != doc.CuryOrderTotal)
				{
					sender.RaiseExceptionHandling<POOrder.curyControlTotal>(e.Row, doc.CuryControlTotal,
						new PXSetPropertyException(Messages.DocumentOutOfBalance));
				}
				else
				{
					sender.RaiseExceptionHandling<POOrder.curyControlTotal>(e.Row, doc.CuryControlTotal, null);
				}

				if (doc.CuryLineTotal < Decimal.Zero && doc.Hold == false)
				{
					if (sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, doc.CuryLineTotal,
						new PXSetPropertyException(Messages.POOrderTotalAmountMustBeNonNegative, $"[{nameof(POOrder.curyLineTotal)}]")))
					{
						throw new PXRowPersistingException(nameof(POOrder.curyLineTotal), null,
							Messages.POOrderTotalAmountMustBeNonNegative, nameof(POOrder.curyLineTotal));
					}
				}
				else
				{
					sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, null, null);
				}
			}
			else
			{
				sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, null, null);
			}

			if (doc != null && IsExternalTax(doc.TaxZoneID) && !sender.ObjectsEqual<POOrder.curyDiscTot>(e.Row, e.OldRow))
			{
				doc.IsTaxValid = false;
			}

			if ((this.Document.View.Answer == WebDialogResult.Yes || (oldDoc.VendorID != null && !sender.ObjectsEqual<POOrder.vendorID>(e.Row, e.OldRow))) 
				&& !sender.ObjectsEqual<POOrder.orderDate>(e.Row, e.OldRow))
			{
				foreach (POLine iLine in this.Transactions.Select())
				{
					if ((bool)iLine.Completed || (bool)iLine.Cancelled || (iLine.ReceivedQty > 0)) continue;
					this.Transactions.Cache.SetDefaultExt<POLine.requestedDate>(iLine);
				}
			}

			if ((this.Document.View.Answer == WebDialogResult.Yes || (oldDoc.VendorID != null && !sender.ObjectsEqual<POOrder.vendorID>(e.Row, e.OldRow))) 
				&& !sender.ObjectsEqual<POOrder.expectedDate>(e.Row, e.OldRow))
			{
				foreach (POLine iLine in this.Transactions.Select())
				{
					if ((bool)iLine.Completed || (bool)iLine.Cancelled || (iLine.ReceivedQty > 0)) continue;
					POLine copy = PXCache<POLine>.CreateCopy(iLine);
					this.Transactions.Cache.SetDefaultExt<POLine.promisedDate>(copy);
					this.Transactions.Cache.Update(copy);
				}
			}

			if(!sender.ObjectsEqual<POOrder.dontPrint>(e.OldRow, e.Row) && doc.DontPrint == true)
			{
				POOrder.Events
					.Select(ev => ev.DoNotPrintChecked)
					.FireOn(this, (POOrder)e.Row);
			}

			if (!sender.ObjectsEqual<POOrder.dontEmail>(e.OldRow, e.Row) && doc.DontEmail == true)
			{
				POOrder.Events
					.Select(ev => ev.DoNotEmailChecked)
					.FireOn(this, (POOrder)e.Row);
			}
		}

		protected virtual void POOrder_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if (e.Row != null)
			{
				using (ReadOnlyScope rs = new ReadOnlyScope(Shipping_Address.Cache, Shipping_Contact.Cache))
				{
					try
					{
						POShipAddressAttribute.DefaultRecord<POOrder.shipAddressID>(sender, e.Row);
					}
					catch (SharedRecordMissingException)
					{
						sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
							new PXSetPropertyException(Messages.ShippingAddressMayNotBeEmpty, PXErrorLevel.Error));
					}
					try
					{
						POShipContactAttribute.DefaultRecord<POOrder.shipContactID>(sender, e.Row);
					}
					catch (SharedRecordMissingException)
					{
						sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
							new PXSetPropertyException(Messages.ShippingContactMayNotBeEmpty, PXErrorLevel.Error));
					}
				}
			}
		}

		protected virtual void POOrder_ShipDestType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row == null) return;

			if (row.ShipDestType == POShippingDestination.Site)
			{
				sender.SetDefaultExt<POOrder.siteID>(e.Row);
				sender.SetValueExt<POOrder.shipToBAccountID>(e.Row, null);
				sender.SetValueExt<POOrder.shipToLocationID>(e.Row, null);
			}
			else if (row.ShipDestType == POShippingDestination.ProjectSite)
			{
				sender.SetValueExt<POOrder.siteID>(e.Row, null);
				sender.SetValueExt<POOrder.shipToBAccountID>(e.Row, null);
				sender.SetValueExt<POOrder.shipToLocationID>(e.Row, null);
			}
			else
			{
				sender.SetValueExt<POOrder.siteID>(e.Row, null);
				sender.SetDefaultExt<POOrder.shipToBAccountID>(e.Row);
				sender.SetDefaultExt<POOrder.shipToLocationID>(e.Row);
			}
		}

		protected virtual void POOrder_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;

			if (row != null && row.ShipDestType == POShippingDestination.Site)
			{
				string siteIdErrorMessage = string.Empty;

				try
				{
					POShipAddressAttribute.DefaultRecord<POOrder.shipAddressID>(sender, e.Row);
				}
				catch (SharedRecordMissingException)
				{
					sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
						new PXSetPropertyException(Messages.ShippingAddressMayNotBeEmpty, PXErrorLevel.Error));
					sender.SetValueExt<POOrder.shipAddressID>(e.Row, null);
					siteIdErrorMessage = Messages.ShippingAddressMayNotBeEmpty;
				}
				try
				{
					POShipContactAttribute.DefaultRecord<POOrder.shipContactID>(sender, e.Row);
				}
				catch (SharedRecordMissingException)
				{
					sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
						new PXSetPropertyException(Messages.ShippingContactMayNotBeEmpty, PXErrorLevel.Error));
					sender.SetValueExt<POOrder.shipContactID>(e.Row, null);
					siteIdErrorMessage = Messages.ShippingContactMayNotBeEmpty;
				}

				sender.SetValueExt<POOrder.siteIdErrorMessage>(e.Row, siteIdErrorMessage);

				if (string.IsNullOrWhiteSpace(siteIdErrorMessage))
					PXUIFieldAttribute.SetError<POOrder.siteID>(sender, e.Row, null);
			}
		}

		protected virtual void POOrder_ShipToBAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				sender.SetDefaultExt<POOrder.shipToLocationID>(e.Row);
			}
		}

		protected virtual void POOrder_ShipToLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				try
				{
					POShipAddressAttribute.DefaultRecord<POOrder.shipAddressID>(sender, e.Row);
				}
				catch (SharedRecordMissingException)
				{
					sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
						new PXSetPropertyException(Messages.ShippingAddressMayNotBeEmpty, PXErrorLevel.Error));
				}
				try
				{
					POShipContactAttribute.DefaultRecord<POOrder.shipContactID>(sender, e.Row);
				}
				catch (SharedRecordMissingException)
				{
					sender.RaiseExceptionHandling<POOrder.siteID>(e.Row, sender.GetValueExt<POOrder.siteID>(e.Row),
						new PXSetPropertyException(Messages.ShippingContactMayNotBeEmpty, PXErrorLevel.Error));
				}
			}
		}

		protected virtual void POOrder_ShipToLocationID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null && !IsShipToBAccountRequired(row))
			{
				e.Cancel = true;
				e.NewValue = null;
			}
		}

		protected virtual void POOrder_ShipToBAccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row == null) return;

			if (!IsShipToBAccountRequired(row))
			{
				e.Cancel = true;
				e.NewValue = null;
			}
			else if (row.OrderType == POOrderType.DropShip)
			{
				SOLineSplit demand = PXSelectJoin<SOLineSplit,
					InnerJoin
					<SOOrder, On<SOOrder.orderType, Equal<SOLineSplit.orderType>, And<SOOrder.orderNbr, Equal<SOLineSplit.orderNbr>>>>,
					Where<SOLineSplit.pOType, Equal<Current<POOrder.orderType>>,
					And<SOLineSplit.pONbr, Equal<Current<POOrder.orderNbr>>,
					And<SOOrder.customerID, NotEqual<Required<SOOrder.customerID>>>>>>.Select(this, e.NewValue);

				if (demand != null)
				{
					Customer customer =
						PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, e.NewValue);
					var ex = new PXSetPropertyException(Messages.CustomerCannotBeChangedForOrderWithActiveDemand);
					ex.ErrorValue = customer?.AcctCD;

					throw ex;
				}
			}
		}

		protected virtual void POOrder_OwnerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null && (this.POSetup.Current.UpdateSubOnOwnerChange ?? false))
			{
				foreach (POLine iLn in this.Transactions.Select())
				{
					if (iLn.LineType == POLineType.NonStock || iLn.LineType == POLineType.Service)
					{
						this.Transactions.Cache.SetDefaultExt<POLine.expenseSubID>(iLn);
					}
				}
			}
		}

		protected virtual void POOrder_Cancelled_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if (doc == null || (bool)e.NewValue != true)
				return;

			if (doc.OrderType == POOrderType.Blanket)
			{
				POLine line = PXSelect<POLine,
					Where<POLine.receivedQty, Greater<decimal0>,
						And<POLine.orderType, Equal<Required<POOrder.orderType>>,
						And<POLine.orderNbr, Equal<Required<POOrder.orderNbr>>>>>>
					.Select(this, doc.OrderType, doc.OrderNbr);

				if (line != null)
				{
					ThrowErrorWhenPurchaseReceiptExists(null, doc);
				}
			}
			else
			{
			POReceipt receipt = PXSelectJoin<POReceipt,
						InnerJoin<POOrderReceipt,
							On<POOrderReceipt.FK.Receipt>>,
						Where<POOrderReceipt.pOType, Equal<Required<POReceiptLine.pOType>>,
					And<POOrderReceipt.pONbr, Equal<Required<POReceiptLine.pONbr>>>>,
				OrderBy<Asc<POReceipt.released>>>.Select(this, doc.OrderType, doc.OrderNbr);

			if (receipt != null)
			{
				ThrowErrorWhenPurchaseReceiptExists(receipt, doc);
			}
		}
		}

		protected virtual void ThrowErrorWhenPurchaseReceiptExists(POReceipt receipt, POOrder order)
		{
			if (receipt?.Released == false)
			{
				throw new PXSetPropertyException(Messages.POOrderHasUnreleaseReceiptsAndCantBeCancelled);
			}
			else if (order.OrderType == POOrderType.Blanket || receipt?.Released == true)
			{
				throw new PXSetPropertyException(Messages.POOrderHasReleasedReceiptAndCantBeCancelled, order.OrderNbr);
			}
		}

		protected virtual void ThrowErrorWhenPurchaseReceiptExists(POReceipt receipt, POLine line)
		{
			if (receipt?.Released == false)
			{
				throw new PXSetPropertyException(Messages.POLineHasUnreleaseReceiptsAndCantBeCompletedOrCancelled);
			}
			else if (line.OrderType == POOrderType.Blanket || receipt?.Released == true)
			{
				throw new PXSetPropertyException(Messages.POLineHasReleaseReceiptsAndCantBeCancelled, line.OrderNbr);
			}
		}

		public virtual void POOrder_Cancelled_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			var oldCancelled = e.OldValue as bool? == true;
			var cancelled = doc.Cancelled == true;
			if (oldCancelled == cancelled)
				return;

			foreach (POLine line in this.Transactions.Select())
			{
				bool? newState = null;
				if (cancelled && line.Completed != true)
				{
					newState = true;
				}
				if (!cancelled && line.Cancelled == true)
				{
					newState = CanReopenPOLine(line) ? false : (bool?)null;
				}

				InventoryItem item = InventoryItem.PK.Find(this, line.InventoryID);
				if (newState != true && item?.IsConverted == true &&
					line.IsStockItem != null && line.IsStockItem != item.StkItem)
				{
					continue;
				}

				if (newState != null)
				{
					try
					{
						POLine upd = (POLine)this.Transactions.Cache.CreateCopy(line);
						upd.Cancelled = newState;
						upd.Completed = newState;
						using (new SuppressOrderEventsScope(this))
							this.Transactions.Update(upd);
					}
					catch (PXFieldValueProcessingException)
					{
						//FieldValueProcessingException will be eaten by OnExceptionHandling of POOrder. 
						//throw generic PXException to cause session rollback 
						throw new PXException(ErrorMessages.RecordRaisedErrors, AP.Messages.Updating, Transactions.Cache.GetItemType().Name);
					}
				}
			}
			if(!cancelled)
				RaiseOrderEvents(doc);
		}

		public virtual bool CanReopenPOLine(POLine line)
		{
			POOrder document = Document.Current;
			return line.Completed != true && line.Cancelled != true || !(IsLinkedToSO(line) && document?.IsLegacyDropShip == true);
		}

		private bool IsLineWithDropShipLocation(POLine poLine)
		{
			INSite site = INSite.PK.Find(this, poLine?.SiteID);
			return site == null || site.DropShipLocationID != null;
		}

		protected virtual void POOrder_Hold_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			if ((bool?)e.NewValue == false && Document.Current?.OrderType == POOrderType.DropShip)
			{
				foreach (POLine poLine in Transactions.Select())
				{
					if (IsLineWithDropShipLocation(poLine))
						continue;

					Transactions.Cache.MarkUpdated(poLine, assertError: true);
				}
			}
			else if ((bool?)e.NewValue == true)
			{
				POOrderReceipt receiptOpen =
				PXSelectJoin<POOrderReceipt,
					InnerJoin<POReceipt,
						On2<POOrderReceipt.FK.Receipt,
							And<POReceipt.released, Equal<boolFalse>>>>,
					Where<POOrderReceipt.pOType, Equal<Current<POOrder.orderType>>,
						And<POOrderReceipt.pONbr, Equal<Current<POOrder.orderNbr>>>>>.SelectSingleBound(this, new object[] { e.Row });
				if (receiptOpen != null)
					throw new PXException(Messages.PurchaseOrderOnHoldWithReceipt, receiptOpen.POType, receiptOpen.PONbr, receiptOpen.ReceiptNbr);
			}
		}

		protected virtual void POOrder_OrderType_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = POOrderType.RegularOrder;
		}

		protected virtual void POOrder_Approved_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = POSetup.Current == null || POSetup.Current.OrderRequestApproval != true;
		}

		protected virtual void POOrder_DontPrint_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				e.NewValue = location.Current == null || location.Current.VPrintOrder != true;
			}
		}

		protected virtual void POOrder_DontEmail_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				e.NewValue = location.Current == null || location.Current.VEmailOrder != true;
			}
		}

		protected virtual void POOrder_ExpectedDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			Location vendorLocation = this.location.Current;
			if (row?.OrderDate != null)
			{
				int offset = (vendorLocation != null ? (int)(vendorLocation.VLeadTime ?? 0) : 0);
				e.NewValue = row.OrderDate.Value.AddDays(offset);
			}
		}

		protected virtual void POOrder_TaxZoneID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			if (row != null)
			{
				e.NewValue = GetDefaultTaxZone(row);
			}
		}

		public virtual string GetDefaultTaxZone(POOrder row)
		{
			string result = null;
			if (row != null)
			{
				Location vendorLocation = PXSelect<Location, Where<Location.bAccountID, Equal<Required<Location.bAccountID>>, And<Location.locationID, Equal<Required<Location.locationID>>>>>
					.Select(this, row.VendorID, row.VendorLocationID);
				if (vendorLocation != null)
				{
					if (!string.IsNullOrEmpty(vendorLocation.VTaxZoneID))
					{
						result = vendorLocation.VTaxZoneID;
					}
				}
			}

			return result;
		}

		protected virtual void POOrder_VendorLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Location current = (Location)this.location.Current;
			POOrder row = (POOrder)e.Row;

			using (new VendorLocationUpdatedContext())
			{
				if (current == null || (current.BAccountID != row.VendorID || current.LocationID != row.VendorLocationID))
				{
					current = this.location.Select();
					this.location.Current = current;
				}

				sender.SetDefaultExt<POOrder.branchID>(e.Row);
				sender.SetDefaultExt<POOrder.taxCalcMode>(e.Row);
				sender.SetDefaultExt<POOrder.shipVia>(e.Row);
				sender.SetDefaultExt<POOrder.fOBPoint>(e.Row);
				sender.SetDefaultExt<POOrder.expectedDate>(e.Row);
				sender.SetDefaultExt<POOrder.siteID>(e.Row);

				sender.SetDefaultExt<POOrder.approved>(e.Row);
				sender.SetDefaultExt<POOrder.dontPrint>(e.Row);
				sender.SetDefaultExt<POOrder.dontEmail>(e.Row);
				sender.SetDefaultExt<POOrder.printed>(e.Row);
				sender.SetDefaultExt<POOrder.emailed>(e.Row);

				if (row.OrderType != POOrderType.DropShip)
				{
					sender.SetDefaultExt<POOrder.shipDestType>(e.Row);
					sender.SetDefaultExt<POOrder.shipToLocationID>(e.Row);
				}

				PORemitAddressAttribute.DefaultRecord<POOrder.remitAddressID>(sender, e.Row);
				PORemitContactAttribute.DefaultRecord<POOrder.remitContactID>(sender, e.Row);

				//Updating VendorLocationID on linked SO entities
				if (e.OldValue != null)
				{
					foreach (PXResult<POLine, INItemPlan> demandPlan in SelectFrom<POLine>.
					InnerJoin<INItemPlan>.On<POLine.planID.IsEqual<INItemPlan.supplyPlanID>>.
					Where<POLine.orderType.IsEqual<@P.AsString.ASCII>.And<POLine.orderNbr.IsEqual<@P.AsString>>>.
					View.Select(this, row.OrderType, row.OrderNbr))
					{
						INItemPlan inItemPlan = (INItemPlan)demandPlan;

						inItemPlan.VendorLocationID = Document.Current.VendorLocationID;
						this.Caches<INItemPlan>().Update(inItemPlan);
					}
				}
			}
		}

		protected virtual void POOrder_PayToVendorID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			POOrder order = e.Row as POOrder;
			if (order == null) return;

			Vendor payToVendor = PXSelectReadonly<Vendor, Where<Vendor.bAccountID, Equal<Required<POOrder.payToVendorID>>>>.Select(this, e.NewValue);
			// No trust in PXSelectorAttribute.Select

			if (payToVendor?.CuryID != null && payToVendor.AllowOverrideCury != true && order.CuryID != payToVendor.CuryID)
			{
				e.NewValue = payToVendor.AcctCD;
				throw new PXSetPropertyException(Messages.PayToVendorHasDifferentCury, payToVendor.AcctCD, payToVendor.CuryID, order.CuryID);
			}
		}

		[PopupMessage]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void POOrder_VendorID_CacheAttached(PXCache sender)
		{
		}

		protected virtual void POOrder_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder order = e.Row as POOrder;
			if (order == null) return;

			using (new VendorUpdatedContext())
			{
				sender.SetDefaultExt<POOrder.vendorLocationID>(order);

				// Pay-to Vendor must be defaulted before terms defaulting
				if (order.VendorID != null)
				{
					int? payToVendorID;
					if (PXAccess.FeatureInstalled<FeaturesSet.vendorRelations>())
					{
						Vendor orderVendor = Vendor.PK.Find(this, order.VendorID);
						// This code does not work: PXSelectorAttribute.Select<POOrder.vendorID>(sender, order/*, order.VendorID*/) as Vendor
						// TODO: Need to investigate and redesign
						payToVendorID = orderVendor?.PayToVendorID ?? order.VendorID;
					}
					else
					{
						payToVendorID = order.VendorID;
					}

					order.PayToVendorID = payToVendorID;
				}
				else
				{
					order.PayToVendorID = null;
				}

				sender.SetDefaultExt<POOrder.termsID>(order); // Defaulting of terms depends on pay-to vendor ID

				
				if (e.OldValue != null && ((POOrder)e.Row).VendorID != (int?)e.OldValue)
				{
					sender.SetDefaultExt<POOrder.orderDate>(e.Row);
					sender.SetValue<POOrder.vendorRefNbr>(e.Row, null);
					sender.SetDefaultExt<POOrder.ownerID>(e.Row);

					foreach (POLine poLine in Transactions.Select())
					{
						poLine.VendorID = Document.Current.VendorID;
						poLine.POAccrualType = (string)PXFormulaAttribute.Evaluate<POLine.pOAccrualType>(Transactions.Cache, poLine);

						Transactions.Update(poLine);
					}

					//Updating VendorID and VendorLocationID on linked SO entities
					foreach (PXResult<POLine, INItemPlan, SOLineSplit3> fixedDemand in SelectFrom<POLine>.
							LeftJoin<INItemPlan>.On<POLine.planID.IsEqual<INItemPlan.supplyPlanID>>.
							LeftJoin<SOLineSplit3>.On<SOLineSplit3.planID.IsEqual<INItemPlan.planID>>.
							Where<POLine.orderType.IsEqual<@P.AsString.ASCII>.And<POLine.orderNbr.IsEqual<@P.AsString>>>.
							View.Select(this, order.OrderType, order.OrderNbr))
					{
						SOLineSplit3 soLineSplit = (SOLineSplit3)fixedDemand;
						INItemPlan inItemPlan = (INItemPlan)fixedDemand;

						if (soLineSplit.OrderNbr != null)
						{
							UpdateSOLine(soLineSplit, Document.Current.VendorID, true);
							FixedDemand.Cache.MarkUpdated(soLineSplit, assertError: true);
						}

						if (inItemPlan.PlanID != null)
						{
							inItemPlan.VendorID = Document.Current.VendorID;
							inItemPlan.VendorLocationID = Document.Current.VendorLocationID;
							this.Caches<INItemPlan>().Update(inItemPlan);
						}
					}
				}

				Validate.VerifyField<POOrder.payToVendorID>(sender, order);
				Validate.VerifyField<POOrder.vendorRefNbr>(sender, order);
			}
		}

		public class VendorUpdatedContext : IDisposable
		{
			private const string isVendorUpdatedSlotName = "IsVendorUpdatedContext";

			public VendorUpdatedContext()
			{
				PXContext.SetSlot<bool>(isVendorUpdatedSlotName, true);
			}

			public void Dispose()
			{
				PXContext.SetSlot<bool>(isVendorUpdatedSlotName, false);
			}

			public static bool IsScoped()
			{
				return PXContext.GetSlot<bool>(VendorUpdatedContext.isVendorUpdatedSlotName);
			}
		}

		public class VendorLocationUpdatedContext : IDisposable
		{
			private const string isVendorLocationUpdatedSlotName = "IsVendorLocationUpdatedContext";

			public VendorLocationUpdatedContext()
			{
				PXContext.SetSlot<bool>(isVendorLocationUpdatedSlotName, true);
			}

			public void Dispose()
			{
				PXContext.SetSlot<bool>(isVendorLocationUpdatedSlotName, false);
			}

			public static bool IsScoped()
			{
				return PXContext.GetSlot<bool>(VendorLocationUpdatedContext.isVendorLocationUpdatedSlotName);
			}
		}

		protected virtual void POOrder_VendorID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = e.Row as POOrder;
			if (row == null) return;

			if (Transactions.Select().Count == 0)
				return;

			Vendor newVendor =
				SelectFrom<Vendor>.Where<Vendor.bAccountID.IsEqual<@P.AsInt>>.View.Select(this, e.NewValue);
			if (newVendor != null)
			{
				if (newVendor.AllowOverrideCury != true && newVendor.CuryID != row.CuryID &&
					!string.IsNullOrEmpty(newVendor.CuryID))
				{
					RaiseVendorIDException(sender, row, e.NewValue,
						string.Format(Messages.VendorChangedOnOrderWithRestrictedCurrency, row.CuryID));
				}

				CurrencyInfo currencyinfo = FindImplementation<IPXCurrencyHelper>().GetCurrencyInfo(row.CuryInfoID);
				if (currencyinfo != null &&
					currencyinfo.CuryID != currencyinfo.BaseCuryID &&
					newVendor.AllowOverrideRate != true &&
					newVendor.CuryRateTypeID != currencyinfo.CuryRateTypeID)
				{
					RaiseVendorIDException(sender, row, e.NewValue,
						string.Format(Messages.VendorChangedOnOrderWithRestrictedRateType, currencyinfo.CuryRateTypeID));
				}
			}

			if (Receipts.Select().Count > 0)
			{
				RaiseVendorIDException(sender, row, e.NewValue, Messages.VendorChangedOnOrderWithPurchaseReceipt);
			}

			if (APDocs.Select().Count > 0)
			{
				RaiseVendorIDException(sender, row, e.NewValue, Messages.VendorChangedOnOrderWithAPDocument);
			}

			POLine lineFromBlanketOrder = Transactions.Select().AsEnumerable()
				.FirstOrDefault(res => !string.IsNullOrEmpty(((POLine)res).PONbr));
			if (lineFromBlanketOrder != null)
			{
				RaiseVendorIDException(sender, row, e.NewValue, Messages.VendorChangedOnOrderWithLinesFromBlanketOrder);
			}

			var soLineSplitsLinkedWithCurrentPO =
				SelectFrom<SOLineSplit>
					.Where<SOLineSplit.pOType.IsEqual<@P.AsString.ASCII>.And<SOLineSplit.pONbr.IsEqual<@P.AsString>>>
					.AggregateTo<GroupBy<SOLineSplit.orderType>, GroupBy<SOLineSplit.orderNbr>,
						GroupBy<SOLineSplit.lineNbr>, GroupBy<SOLineSplit.pOType>, GroupBy<SOLineSplit.pONbr>>
					.View.Select(this, row.OrderType, row.OrderNbr);

			foreach (SOLineSplit splitLinkedWithCurrentPO in soLineSplitsLinkedWithCurrentPO)
			{
				var soLineSplitLinkedWithDifferentPO = SelectFrom<SOLineSplit>
					.Where<SOLineSplit.orderType.IsEqual<@P.AsString.ASCII>.And<SOLineSplit.orderNbr.IsEqual<@P.AsString>.And<
						SOLineSplit.lineNbr.IsEqual<@P.AsInt>
						.And<Brackets<SOLineSplit.pOType.IsNotEqual<@P.AsString.ASCII>.Or<
							SOLineSplit.pONbr.IsNotEqual<@P.AsString>>>>>>>
					.View.Select(this, row.OrderType, row.OrderNbr);

				if (soLineSplitLinkedWithDifferentPO.Count != 0)
				{
					RaiseVendorIDException(sender, row, e.NewValue, Messages.VendorChangedOnOrderLinkedWithSO);
				}
			}
		}

		public virtual void RaiseVendorIDException(PXCache sender, POOrder order, object newVendorID, string error)
		{
			BAccountR newAccount = (BAccountR)PXSelectorAttribute.Select<POOrder.vendorID>(sender, order, newVendorID);
			var ex = new PXSetPropertyException(error);
			ex.ErrorValue = newAccount?.AcctCD;
			throw ex;
		}

		protected virtual void POOrder_PayToVendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder order = e.Row as POOrder;
			if (order == null) return;

			sender.SetDefaultExt<POOrder.termsID>(order);
			foreach (POLine line in Transactions
				.Select()
				.RowCast<POLine>()
				.Where(line => line.Completed != true && line.Cancelled != true && line.Closed != true))
			{
				if (IsAccrualAccountEnabled(line))
				{
					try
					{
						this.Transactions.Cache.SetDefaultExt<POLine.pOAccrualAcctID>(line);
					}
					catch (PXSetPropertyException<POLine.pOAccrualAcctID> exc)
					{
						this.Transactions.Cache.RaiseExceptionHandling<POLine.pOAccrualAcctID>(line, line.POAccrualAcctID, exc);
					}
					try
					{
						this.Transactions.Cache.SetDefaultExt<POLine.pOAccrualSubID>(line);
					}
					catch (PXSetPropertyException<POLine.pOAccrualSubID> exc)
					{
						this.Transactions.Cache.RaiseExceptionHandling<POLine.pOAccrualSubID>(line, line.POAccrualSubID, exc);
					}
				}
				if (line.LineType == POLineType.NonStock || line.LineType == POLineType.Service)
				{
					Transactions.Cache.SetDefaultExt<POLine.expenseSubID>(line);
				}
			}
		}

		protected virtual void POOrder_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if (PXSelectGroupBy<POOrderReceipt,
				Where<POOrderReceipt.pONbr, Equal<Required<POOrder.orderNbr>>,
				And<POOrderReceipt.pOType, Equal<Required<POOrder.orderType>>>>,
				Aggregate<Count>>.Select(this, doc.OrderNbr, doc.OrderType).RowCount > 0)
			{
				throw new PXException(Messages.POOrderHasReceiptsAndCannotBeDeleted);
			}

			if (PXSelectGroupBy<APTran,
			Where<APTran.pONbr, Equal<Required<POOrder.orderNbr>>,
			And<APTran.pOOrderType, Equal<Required<POOrder.orderType>>,
			And<APTran.released, Equal<True>>>>,
			Aggregate<Count>>.Select(this, doc.OrderNbr, doc.OrderType).RowCount > 0)
			{
				throw new PXException(Messages.POOrderHasBillsReleasedAndCannotBeDeleted);
			}

			PXResult<POOrderPrepayment> prepayment = PXSelect<POOrderPrepayment,
				Where<POOrderPrepayment.orderType, Equal<Current<POOrder.orderType>>,
				And<POOrderPrepayment.orderNbr, Equal<Current<POOrder.orderNbr>>>>>
				.SelectSingleBound(this, new object[] { doc });
			if (prepayment != null)
			{
				throw new PXException(Messages.POOrderHasPrepaymentAndCannotBeDeleted);
			}

			if (PXSelectGroupBy<APTran,
				Where<APTran.pONbr, Equal<Required<POOrder.orderNbr>>,
				And<APTran.pOOrderType, Equal<Required<POOrder.orderType>>>>,
				Aggregate<Count>>.Select(this, doc.OrderNbr, doc.OrderType).RowCount > 0)
			{
				throw new PXException(Messages.POOrderHasBillsGeneratedAndCannotBeDeleted);
			}

			if (doc.OrderType == POOrderType.Blanket)
			{
				POLine referencedPOLine =
						SelectFrom<POLine>.
						Where<POLine.pONbr.IsEqual<POOrder.orderNbr.FromCurrent>.
							And<POLine.pOType.IsEqual<POOrder.orderType.FromCurrent>>>
						.View.SelectSingleBound(this, new object[] { doc });
				if (referencedPOLine != null)
				{
					throw new PXException(Messages.BlanketPurchaseOrderCannotBeDeleted, doc.OrderNbr);
				}
			}
		}

		protected virtual void POOrder_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			POOrder doc = (POOrder)e.Row;
			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete) return;

			PXDefaultAttribute.SetPersistingCheck<POOrder.siteID>(sender, doc, (doc.ShipDestType == POShippingDestination.Site && doc.OrderType != POOrderType.RegularSubcontract) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POOrder.shipToLocationID>(sender, doc, IsShipToBAccountRequired(doc) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POOrder.shipToBAccountID>(sender, doc, IsShipToBAccountRequired(doc) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			if (doc.OrderType == POOrderType.ProjectDropShip)
			{
				if (doc.ShipDestType != POShippingDestination.ProjectSite)
				{
					if (sender.RaiseExceptionHandling<POOrder.shipDestType>(e.Row, doc.ShipDestType, new PXSetPropertyException(Messages.ProjectDropShipShipDestTypeInvalid, PXErrorLevel.Error)))
					{
						throw new PXRowPersistingException(typeof(POOrder.shipDestType).Name, null, Messages.ProjectDropShipShipDestTypeInvalid);
					}
				}

				if (doc.ProjectID == null)
				{
					if (sender.RaiseExceptionHandling<POOrder.projectID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, $"[{nameof(POOrder.projectID)}]")))
					{
						throw new PXRowPersistingException(typeof(POOrder.projectID).Name, null, ErrorMessages.FieldIsEmpty, nameof(POOrder.projectID));
					}
				}
			}

			if (string.IsNullOrEmpty(doc.TermsID))
			{
				if (sender.RaiseExceptionHandling<POOrder.termsID>(e.Row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, $"[{nameof(POOrder.termsID)}]")))
				{
					throw new PXRowPersistingException(typeof(POOrder.termsID).Name, null, ErrorMessages.FieldIsEmpty, nameof(POOrder.termsID));
				}
			}

			if (doc.CuryLineTotal < Decimal.Zero && doc.Hold == false)
			{
				if (sender.RaiseExceptionHandling<POOrder.curyLineTotal>(e.Row, doc.CuryLineTotal, new PXSetPropertyException(Messages.POOrderTotalAmountMustBeNonNegative, $"[{nameof(POOrder.curyLineTotal)}]")))
				{
					throw new PXRowPersistingException(typeof(POOrder.curyLineTotal).Name, null, Messages.POOrderTotalAmountMustBeNonNegative, nameof(POOrder.curyLineTotal));
				}
			}

			if (doc.CuryDiscTot > Math.Abs(doc.CuryLineTotal ?? 0m))
			{
				if (sender.RaiseExceptionHandling<POOrder.curyDiscTot>(e.Row, doc.CuryDiscTot, new PXSetPropertyException(Messages.DiscountGreaterDetailTotal, PXErrorLevel.Error)))
				{
					throw new PXRowPersistingException(typeof(POOrder.curyDiscTot).Name, null, Messages.DiscountGreaterDetailTotal);
				}
			}
		}
		protected virtual void POOrder_OrderDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;

			if (IsVendorOrLocationChanged(sender, row))
				return;

			if (this.Transactions.Select().Count > 0 && !IsMobile)
			{
				this.Document.Ask(Messages.Warning, Messages.POOrderOrderDateChangeConfirmation, MessageButtons.YesNo, MessageIcon.Question);
			}
		}

		protected virtual void POOrder_OrderDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POOrder.expectedDate>(e.Row);
		}

		protected virtual void POOrder_RetainageApply_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			var document = (POOrder)e.Row;
			if (document == null) return;

			bool? newValue = (bool?)e.NewValue;
			if (document.RetainageApply == true && newValue == false)
			{
				POLine retainageLine = PXSelect<POLine,
					Where<POLine.orderType, Equal<Current<POOrder.orderType>>, And<POLine.orderNbr, Equal<Current<POOrder.orderNbr>>,
						And<POLine.retainagePct, NotEqual<decimal0>>>>>
					.SelectWindowed(this, 0, 1);
				if (retainageLine == null) return;

				if (WebDialogResult.Yes == Document.Ask(
						Messages.Warning,
						AP.Messages.UncheckApplyRetainage,
						MessageButtons.YesNo,
						MessageIcon.Warning))
				{
					foreach (POLine line in Transactions.Select())
					{
						if (line.RetainagePct != 0m)
						{
							line.CuryRetainageAmt = 0m;
							line.RetainagePct = 0m;
							Transactions.Update(line);
						}
					}
				}
				else
				{
					e.NewValue = true;
					e.Cancel = true;
				}
			}
		}

		protected virtual void POOrder_ExpectedDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POOrder row = (POOrder)e.Row;
			DateTime newExpectedDate;

			if (IsVendorOrLocationChanged(sender, row))
			{
				if (e.NewValue != null && DateTime.TryParse(e.NewValue.ToString(), out newExpectedDate) && row.ExpectedDate != newExpectedDate)
				{
					sender.RaiseExceptionHandling<POOrder.expectedDate>(row, row.ExpectedDate,
								new PXSetPropertyException(Messages.POOrderPromisedDateChangedAutomatically, PXErrorLevel.Warning, ((DateTime)row.ExpectedDate).Date, ((DateTime)newExpectedDate).Date));
				}
				return;
			}

			if (this.Transactions.Select().Count > 0 && !IsMobile && !IsContractBasedAPI)
			{
				this.Document.Ask(Messages.Warning, Messages.POOrderPromisedDateChangeConfirmation, MessageButtons.YesNo, MessageIcon.Question);
			}
		}

		protected virtual void POOrder_ProjectID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POOrder row = e.Row as POOrder;
			if (row == null) return;

			if (row.OrderType == POOrderType.ProjectDropShip)
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2023R1)]
		protected virtual void POOrder_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POOrder row = e.Row as POOrder;
			if (row == null) return;
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2023R1)]
		protected virtual void POOrder_CuryID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
		}

		protected virtual void _(Events.RowPersisting<POShipAddress> e)
		{
			if (e.Row != null)
			{
				PXDefaultAttribute.SetPersistingCheck<POShipAddress.bAccountID>(e.Cache, e.Row, Document.Current?.OrderType == POOrderType.ProjectDropShip ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
			}
		}

		protected virtual void _(Events.RowPersisting<POShipContact> e)
		{
			if (e.Row != null)
			{
				PXDefaultAttribute.SetPersistingCheck<POShipContact.bAccountID>(e.Cache, e.Row, Document.Current?.OrderType == POOrderType.ProjectDropShip ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
			}
		}

		[PXDBString(10, IsUnicode = true)]
		[PXDBDefault(typeof(POOrder.taxZoneID))]
		[PXUIFieldAttribute(DisplayName = "Vendor Tax Zone", Enabled = false)]
		protected virtual void POTaxTran_TaxZoneID_CacheAttached(PXCache sender)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXParent(typeof(Select<POOrder, Where<POOrder.orderType, Equal<Current<RQ.RQRequisitionOrder.orderType>>, And<POOrder.orderNbr, Equal<Current<RQ.RQRequisitionOrder.orderNbr>>>>>))]
		protected virtual void RQRequisitionOrder_OrderNbr_CacheAttached(PXCache sender)
		{
		}

		protected virtual void POTaxTran_TaxZoneID_ExceptionHandling(PXCache sender, PXExceptionHandlingEventArgs e)
		{
			Exception ex = e.Exception as PXSetPropertyException;
			if (ex != null)
			{
				Document.Cache.RaiseExceptionHandling<POOrder.taxZoneID>(Document.Current, null, ex);
			}
		}

		protected virtual void POTaxTran_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (!(e.Row is POTaxTran poTaxTran))
				return;

			PXUIFieldAttribute.SetEnabled<POTaxTran.taxID>(sender, e.Row, sender.GetStatus(e.Row) == PXEntryStatus.Inserted);

			PXUIFieldAttribute.SetEnabled<POTaxTran.curyExpenseAmt>(sender, e.Row, false); //this field is disabled for now as there is no logic implemented that can transfer manually edited Expense Amounts to AP
		}

		protected virtual void _(Events.FieldVerifying<POOrder, POOrder.expirationDate> eventArgs)
		{
			if (eventArgs.Row?.OrderType == POOrderType.Blanket &&
				(DateTime?)eventArgs.NewValue < eventArgs.Row.OrderDate)
			{
				eventArgs.Cache.RaiseExceptionHandling<POOrder.expirationDate>(eventArgs.Row, eventArgs.NewValue,
					new PXSetPropertyException<POOrder.expirationDate>(Messages.POBlanketOrderExpiresEarlierThanTheOrderDate, PXErrorLevel.RowWarning));
			}
		}
		#endregion

		#region Status changing

		public virtual void UpdateDocumentState(POOrder order)
        {
			Document.Cache.ClearQueryCacheObsolete();
			Document.Search<POOrder.orderNbr>(order.OrderNbr, order.OrderType);

			if (Document.Current.Hold != true)
			{
				if(!RaiseOrderEvents(Document.Current, false))
                {
					POOrder.Events
						.Select(ev => ev.LinesReopened)
						.FireOn(this, Document.Current);
				}
			}
		}

		public class SuppressOrderEventsScope : CounterScope<SuppressOrderEventsScope>
        {
			public SuppressOrderEventsScope(PXGraph graph): base(graph) { }
		}

		protected virtual bool RaiseOrderEvents(POOrder document) => RaiseOrderEvents(document, true);

		private bool RaiseOrderEvents(POOrder order, bool validateState)
		{
			if (SuppressOrderEventsScope.Suppressed(this))
				return false;

			if (validateState
				&& (order.Status != POOrderStatus.Open || order.Hold == true))
				return false;

			if(order.LinesToCloseCntr == 0)
            {
				POOrder.Events
						.Select(ev => ev.LinesClosed)
						.FireOn(this, order);

				return true;
			}
			if(order.LinesToCompleteCntr == 0)
			{
				POOrder.Events
						.Select(ev => ev.LinesCompleted)
						.FireOn(this, order);

				return true;
			}
			return false;
		}

		#endregion

		#region POLine Events

		[POCommitment]
		[PXDBGuid]
		protected virtual void POLine_CommitmentID_CacheAttached(PXCache sender) { }

		[PXFormula(typeof(Selector<POLine.inventoryID, InventoryItem.kitItem>))]
		[PXMergeAttributes(Method = MergeMethod.Append)]
		public void POLine_IsKit_CacheAttached(PXCache sender) { }

		[PXFormula(typeof(Selector<POLine.inventoryID, InventoryItem.stkItem>))]
		[PXMergeAttributes(Method = MergeMethod.Append)]
		public void POLine_IsStockItem_CacheAttached(PXCache sender) { }

		[InterBranchRestrictor(typeof(Where2<SameOrganizationBranch<INSite.branchID, Current<POOrder.branchID>>,
			Or<Current<POOrder.orderType>, Equal<POOrderType.standardBlanket>>>))]
		[PXMergeAttributes(Method = MergeMethod.Append)]
		protected virtual void POLine_SiteID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXParent(typeof(Select<POLineR,
			Where<POLineR.orderType, Equal<Current<POLine.pOType>>,
										And<POLineR.orderType, Equal<POOrderType.blanket>,
										And<POLineR.orderNbr, Equal<Current<POLine.pONbr>>,
										And<POLineR.lineNbr, Equal<Current<POLine.pOLineNbr>>>>>>>))]
		protected virtual void _(Events.CacheAttached<POLine.pOLineNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXFormula(null, typeof(SumCalc<POLineR.curyBLOrderedCost>))]
		protected virtual void _(Events.CacheAttached<POLine.curyExtCost> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		[PXDBDefault(typeof(POOrder.orderType))]
		protected virtual void _(Events.CacheAttached<POLine.orderType> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		[PXDBDefault(typeof(POOrder.orderNbr))]
		protected virtual void _(Events.CacheAttached<POLine.orderNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		[PXDBDefault(typeof(POOrder.vendorID))]
		protected virtual void _(Events.CacheAttached<POLine.vendorID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDefaultAttribute))]
		[PXDBDefault(typeof(POOrder.orderDate))]
		protected virtual void _(Events.CacheAttached<POLine.orderDate> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[POTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran),
			//Per Unit Tax settings
			Inventory = typeof(POLine.inventoryID), UOM = typeof(POLine.uOM), LineQty = typeof(POLine.orderQty))]
		[POUnbilledTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran),
			//Per Unit Tax settings
			Inventory = typeof(POLine.inventoryID), UOM = typeof(POLine.uOM), LineQty = typeof(POLine.unbilledQty))]
		[PORetainedTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
		protected virtual void _(Events.CacheAttached<POLine.taxCategoryID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDBQuantityAttribute), nameof(PXDBQuantityAttribute.DecimalVerifyUnits), InventoryUnitType.PurchaseUnit)]
		[PXCustomizeBaseAttribute(typeof(PXDBQuantityAttribute), nameof(PXDBQuantityAttribute.ConvertToDecimalVerifyUnits), false)]
		protected virtual void _(Events.CacheAttached<POLine.orderQty> e) { }

		protected virtual void POLine_OrderQty_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row != null)
			{
				e.NewValue = row.LineType == POLineType.Freight ? Decimal.One : Decimal.Zero;
			}
		}

		protected object GetValue<Field>(object data)
			where Field : IBqlField
		{
			if (data == null) return null;
			return this.Caches[BqlCommand.GetItemType(typeof(Field))].GetValue(data, typeof(Field).Name);
		}

		protected virtual void POLine_OrderQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row != null)
			{
				if (row.OrderQty == 0)
				{
					sender.SetValueExt<POLine.curyDiscAmt>(row, decimal.Zero);
					sender.SetValueExt<POLine.discPct>(row, decimal.Zero);
				}
				else
				{
					sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
				}
			}
		}

		protected virtual void POLine_POAccrualAcctID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var row = (POLine)e.Row;
			if (row == null) return;

			if (IsAccrualAccountEnabled(row))
			{
				var item = InventoryItem.PK.Find(this, row.InventoryID);
				if (item != null && (item.StkItem == true || item.NonStockReceipt == true))
				{
					var postClass = INPostClass.PK.Find(this, item.PostClassID);
					if (postClass != null)
					{
						var site = INSite.PK.Find(this, row.SiteID);
						Vendor vendorForAccrual = PXAccess.FeatureInstalled<FeaturesSet.vendorRelations>() && Document.Current.VendorID != Document.Current.PayToVendorID
							? PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<POOrder.payToVendorID>>>>.SelectSingleBound(this, new object[] { })
							: vendor.Current;

						e.NewValue = INReleaseProcess.GetPOAccrualAcctID<INPostClass.pOAccrualAcctID>(this, postClass.POAccrualAcctDefault, item, site, postClass, vendorForAccrual);
						if (e.NewValue != null)
						{
							e.Cancel = true;
						}
					}
				}
			}
			else
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void POLine_POAccrualSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var row = (POLine)e.Row;
			if (row == null) return;

			if (IsAccrualAccountEnabled(row))
			{
				var item = InventoryItem.PK.Find(this, row.InventoryID);
				if (item != null && (item.StkItem == true || item.NonStockReceipt == true))
				{
					var postClass = INPostClass.PK.Find(this, item.PostClassID);
					if (postClass != null)
					{
						var site = INSite.PK.Find(this, row.SiteID);
						try
						{
							Vendor vendorForAccrual = PXAccess.FeatureInstalled<FeaturesSet.vendorRelations>() && Document.Current.VendorID != Document.Current.PayToVendorID
								? PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<POOrder.payToVendorID>>>>.SelectSingleBound(this, new object[] { })
								: vendor.Current;
							e.NewValue = INReleaseProcess.GetPOAccrualSubID<INPostClass.pOAccrualSubID>(this, postClass.POAccrualAcctDefault, postClass.POAccrualSubMask, item, site, postClass, vendorForAccrual);
						}
						catch (PXMaskArgumentException) { }
						if (e.NewValue != null)
						{
							e.Cancel = true;
						}
					}
				}
			}
			else
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void POLine_POAccrualAcctID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var row = (POLine)e.Row;

			if (row != null && !IsAccrualAccountEnabled(row) && sender.Graph.IsImport == true)
			{
				row.POAccrualAcctID = null;
			}
		}

		protected virtual void POLine_POAccrualSubID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var row = (POLine)e.Row;

			if (row != null && !IsAccrualAccountEnabled(row) && sender.Graph.IsImport == true)
			{
				row.POAccrualSubID = null;
			}
		}

		protected virtual bool IsExpenseAccountEnabled(POLine line)
		{
			return line.LineType != POLineType.Description && (!POLineType.IsStock(line.LineType) || line.OrderType == POOrderType.ProjectDropShip);
		}

		protected virtual bool IsAccrualAccountEnabled(POLine line)
		{
			return POLineType.UsePOAccrual(line.LineType) && (line.OrderType != POOrderType.ProjectDropShip || line.DropshipExpenseRecording != DropshipExpenseRecordingOption.OnBillRelease);
		}

		protected virtual bool IsAccrualAccountRequired(POLine line)
		{
			return line.POAccrualType == POAccrualType.Order && IsAccrualAccountEnabled(line);
		}

		/// <summary>
		/// Sets Expense Account for items with Accrue Cost = true. See implementation in CostAccrual extension.
		/// </summary>
		public virtual void SetExpenseAccount(PXCache sender, PXFieldDefaultingEventArgs e, InventoryItem item)
		{
		}

		protected virtual void POLine_ExpenseAcctID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row == null) return;

			switch (row.LineType)
			{
				case POLineType.Description:
					e.Cancel = true;
					break;
				case POLineType.Freight:
					if (Document.Current != null && Document.Current.OrderType == POOrderType.ProjectDropShip)
					{
						var project = PMProject.PK.Find(this, row.ProjectID);
						if (project != null && project.DropshipExpenseAccountSource.IsIn(DropshipExpenseAccountSourceOption.Task, DropshipExpenseAccountSourceOption.Project))
							goto default;
					}
					Carrier carrier = Carrier.PK.Find(this, location.Current.VCarrierID);
					e.NewValue = GetValue<Carrier.freightExpenseAcctID>(carrier) ?? POSetup.Current.FreightExpenseAcctID;
					e.Cancel = true;
					break;
				default:
					var item = InventoryItem.PK.Find(this, row.InventoryID);
					if (Document.Current != null && Document.Current.OrderType == POOrderType.ProjectDropShip)
					{
						var project = PMProject.PK.Find(this, row.ProjectID);
						if (project != null)
						{
							switch (project.DropshipExpenseAccountSource)
							{
								case DropshipExpenseAccountSourceOption.Task:
									{
										var task = PMTask.PK.FindDirty(this, row.ProjectID, row.TaskID);
										if (task != null)
										{
											e.NewValue = GetValue<PMTask.defaultExpenseAccountID>(task);
											if (e.NewValue == null)
												goto case DropshipExpenseAccountSourceOption.Project;
											e.Cancel = true;
										}
										break;
									}
								case DropshipExpenseAccountSourceOption.Project:
									{
										e.NewValue = GetValue<PMProject.defaultExpenseAccountID>(project);
										if (e.NewValue == null)
											goto case DropshipExpenseAccountSourceOption.PostingClassOrItem;
										e.Cancel = true;
										break;
									}
								case DropshipExpenseAccountSourceOption.PostingClassOrItem:
									{
										if (item != null)
										{
											// Acuminator disable once PX1074 SetupNotEnteredExceptionInEventHandlers [Justification: new function around old code where this check has already been disabled]
											SetExpenseAccountUsingDefaultRules(e, row, item, true);
										}
										else
										{
											e.NewValue = location.Current.VExpenseAcctID;
										}
										break;
									}
								default:
									break;
							}
						}
					}
					else
					{
						if (item != null)
						{
							if (row != null && item.StkItem != true && row.AccrueCost == true)
							{
								SetExpenseAccount(sender, e, item);
							}
							else
							{
								// Acuminator disable once PX1074 SetupNotEnteredExceptionInEventHandlers [Justification: new function around old code where this check has already been disabled]
								SetExpenseAccountUsingDefaultRules(e, row, item, false);
							}
						}
						else
						{
							e.NewValue = location.Current?.VExpenseAcctID;
						}
					}

					if (e.NewValue != null)
						e.Cancel = true;
					break;
			}
		}

		private void SetExpenseAccountUsingDefaultRules(PXFieldDefaultingEventArgs e, POLine row, InventoryItem item, bool setExpenseAccountForStockItems)
		{
			INPostClass postClass = INPostClass.PK.Find(this, item.PostClassID);
			// Acuminator disable once PX1074 SetupNotEnteredExceptionInEventHandlers [Justification]
			APSetup setup = apsetup.Current;
			bool expenseAccountAllowed = setExpenseAccountForStockItems ? 
				(POLineType.IsNonStock(row.LineType) || POLineType.IsStock(row.LineType)) : 
				POLineType.IsNonStock(row.LineType);

			if (expenseAccountAllowed
				&& !POLineType.IsService(row.LineType)
				&& postClass != null
				&& (vendor.Current?.IsBranch != true
					|| setup?.IntercompanyExpenseAccountDefault == APAcctSubDefault.MaskItem))
			{
				var site = INSite.PK.Find(this, row.SiteID);
				try
				{
					e.NewValue = INReleaseProcess.GetAcctID<INPostClass.cOGSAcctID>(this, postClass.COGSAcctDefault, item, site, postClass);
				}
				catch (PXMaskArgumentException)
				{
				}
			}
			else if (expenseAccountAllowed)
			{
				if (vendor.Current?.IsBranch == true)
				{
					switch (setup?.IntercompanyExpenseAccountDefault)
					{
						case APAcctSubDefault.MaskLocation:
							e.NewValue = location.Current?.VExpenseAcctID;
							break;
						case APAcctSubDefault.MaskItem:
							e.NewValue = item.COGSAcctID;
							break;
					}
				}
				else
				{
					e.NewValue = item.COGSAcctID ?? location.Current.VExpenseAcctID;
				}
			}
		}

		protected virtual void POLine_ExpenseAcctID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if ((row.ProjectID == null || row.ProjectID == ProjectDefaultAttribute.NonProject()) && row.OrderType != POOrderType.ProjectDropShip)
			{
				sender.SetDefaultExt<POLine.projectID>(e.Row);
			}
		}

		/// <summary>
		/// Sets Expense Subaccount for items with Accrue Cost = true. See implementation in CostAccrual extension.
		/// </summary>
		public virtual object GetExpenseSub(PXCache sender, PXFieldDefaultingEventArgs e, InventoryItem item)
		{
			return null;
		}

		protected virtual void POLine_ExpenseSubID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row == null) return;

			switch (row.LineType)
			{
				case POLineType.Description:
					break;
				case POLineType.Freight:
					Carrier carrier = Carrier.PK.Find(this, location.Current.VCarrierID);
					e.NewValue = GetValue<Carrier.freightExpenseSubID>(carrier) ?? POSetup.Current.FreightExpenseSubID;
					e.Cancel = true;
					break;
				default:
					var item = InventoryItem.PK.Find(this, row.InventoryID);
					if (item != null)
					{
						if (Document.Current != null && Document.Current.OrderType == POOrderType.ProjectDropShip)
						{
							var pmProject = PMProject.PK.Find(this, row.ProjectID);
							var pmTask = PMTask.PK.FindDirty(this, row.ProjectID, row.TaskID);
							var postClass = INPostClass.PK.Find(this, item.PostClassID);

							if (item != null && pmProject != null && (pmTask != null || !pmProject.DropshipExpenseSubMask.Contains(AcctSubDefault.Task)))
							{
								object item_SubID = GetValue<InventoryItem.cOGSSubID>(item);
								object postClass_SubID = GetValue<INPostClass.cOGSSubID>(postClass);
								object project_SubID = GetValue<PMProject.defaultExpenseSubID>(pmProject);
								object task_SubID = GetValue<PMTask.defaultExpenseSubID>(pmTask);

								object subCD = DropshipExpenseSubAccountMaskAttribute.MakeSub<PMProject.dropshipExpenseSubMask>(this, pmProject.DropshipExpenseSubMask,
												new object[] { item_SubID, postClass_SubID, project_SubID, task_SubID },
												new Type[] { typeof(InventoryItem.cOGSSubID), typeof(INPostClass.cOGSSubID), typeof(PMProject.defaultExpenseSubID), typeof(PMTask.defaultExpenseSubID) });
								sender.RaiseFieldUpdating<POReceiptLine.expenseSubID>(e.Row, ref subCD);
								e.NewValue = subCD;
							}
						}
						else
						{
							INPostClass postClass;
							if (POLineType.IsNonStock(row.LineType) && !POLineType.IsService(row.LineType) && (postClass = INPostClass.PK.Find(this, item.PostClassID)) != null)
							{
								var site = INSite.PK.Find(this, row.SiteID);
								try
								{
									e.NewValue = INReleaseProcess.GetSubID<INPostClass.cOGSSubID>(this, postClass.COGSAcctDefault, postClass.COGSSubMask, item, site, postClass);
								}
								catch (PXMaskArgumentException)
								{
								}
							}
							else
							{
								e.NewValue = null;
							}

							if (POLineType.IsNonStock(row.LineType))
							{
								POOrder order = Document.Current;
								EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.defContactID, Equal<Required<EPEmployee.defContactID>>>>.Select(this, order.OwnerID);

								CRLocation companyloc = PXSelectJoin<CRLocation,
									InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>,
										And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>,
									InnerJoin<Branch, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>>>,
									Where<Branch.branchID, Equal<Required<POLine.branchID>>>>.Select(this, row.BranchID);

								int? projectID = row.ProjectID ?? ProjectDefaultAttribute.NonProject();
								PMProject project = PMProject.PK.Find(this, projectID);
								int? projectTask_SubID = PMTask.PK.FindDirty(this, row.ProjectID, row.TaskID)?.DefaultExpenseSubID;

								Location vendorLocation = location.Current;
								if (PXAccess.FeatureInstalled<FeaturesSet.vendorRelations>() &&
									order.PayToVendorID != null &&
									order.VendorID != order.PayToVendorID)
								{
									vendorLocation = PXSelectJoin<Location,
										LeftJoin<BAccount,
											On<Location.bAccountID, Equal<BAccount.bAccountID>,
												And<Location.locationID, Equal<BAccount.defLocationID>>>>,
										Where<BAccount.bAccountID, Equal<Current<POOrder.payToVendorID>>>>.SelectSingleBound(this, new object[] { order });
								}

								object subCD;

								if (row != null && item.StkItem != true && row.AccrueCost == true)
								{
									subCD = GetExpenseSub(sender, e, item);
								}
								else
								{
									subCD = AP.SubAccountMaskAttribute.MakeSub<APSetup.expenseSubMask>(
										this,
										apsetup.Current.ExpenseSubMask,
																						new[]
																									   {
									GetValue<Location.vExpenseSubID>(vendorLocation),
																								   e.NewValue ?? GetValue<InventoryItem.cOGSSubID>((InventoryItem)item),
																								   GetValue<EPEmployee.expenseSubID>(employee),
																								   GetValue<CRLocation.cMPExpenseSubID>(companyloc),
																								project.DefaultExpenseSubID,
																								projectTask_SubID
																									   },
																						new[]
																									   {
																								   typeof (Location.vExpenseSubID),
																								   typeof (InventoryItem.cOGSSubID),
																								   typeof (EPEmployee.expenseSubID),
																								   typeof (CRLocation.cMPExpenseSubID),
									typeof(PMProject.defaultExpenseSubID),
									typeof(PMTask.defaultExpenseSubID)
										});
								}
								sender.RaiseFieldUpdating<POReceiptLine.expenseSubID>(e.Row, ref subCD);
								e.NewValue = subCD;
							}
							else
							{
								e.NewValue = null;
							}
						}
					}
					else
					{
						string expenseSubMask = apsetup.Current.ExpenseSubMask;
						expenseSubMask = expenseSubMask.Replace(APAcctSubDefault.MaskItem, APAcctSubDefault.MaskLocation);

						POOrder order = Document.Current;
						EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.defContactID, Equal<Required<EPEmployee.defContactID>>>>.Select(this, order.OwnerID);

						CRLocation companyloc = PXSelectJoin<CRLocation,
							InnerJoin<BAccountR, On<CRLocation.bAccountID, Equal<BAccountR.bAccountID>,
								And<CRLocation.locationID, Equal<BAccountR.defLocationID>>>,
							InnerJoin<Branch, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>>>,
							Where<Branch.branchID, Equal<Required<POLine.branchID>>>>.Select(this, row.BranchID);

						int? projectID = row.ProjectID ?? ProjectDefaultAttribute.NonProject();
						PMProject project = PMProject.PK.Find(this, projectID);
						int? projectTask_SubID = PMTask.PK.FindDirty(this, row.ProjectID, row.TaskID)?.DefaultExpenseSubID;

						Location vendorLocation = location.Current;
						if (PXAccess.FeatureInstalled<FeaturesSet.vendorRelations>() &&
							order.PayToVendorID != null &&
							order.VendorID != order.PayToVendorID)
						{
							vendorLocation = PXSelectJoin<Location,
								LeftJoin<BAccount,
									On<Location.bAccountID, Equal<BAccount.bAccountID>,
										And<Location.locationID, Equal<BAccount.defLocationID>>>>,
								Where<BAccount.bAccountID, Equal<Current<POOrder.payToVendorID>>>>.SelectSingleBound(this, new object[] { order });
						}

						object subCD = AP.SubAccountMaskAttribute.MakeSub<APSetup.expenseSubMask>(this, expenseSubMask, new[] {
							GetValue<Location.vExpenseSubID>(vendorLocation),
							GetValue<Location.vExpenseSubID>(vendorLocation),
							GetValue<EPEmployee.expenseSubID>(employee),
							GetValue<CRLocation.cMPExpenseSubID>(companyloc),
							project.DefaultExpenseSubID,
							projectTask_SubID }, new Type[] { typeof(Location.vExpenseSubID), typeof(Location.vExpenseSubID), typeof(EPEmployee.expenseSubID), typeof(CRLocation.cMPExpenseSubID), typeof(PMProject.defaultExpenseSubID), typeof(PMTask.defaultExpenseSubID) });

						sender.RaiseFieldUpdating<POReceiptLine.expenseSubID>(e.Row, ref subCD);
						e.NewValue = subCD;
					}
					e.Cancel = true;
					break;
			}

		}

		protected virtual void POLine_TaxCategoryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (TaxAttribute.GetTaxCalc<POLine.taxCategoryID>(sender, e.Row) == TaxCalc.Calc && taxzone.Current != null && !string.IsNullOrEmpty(taxzone.Current.DfltTaxCategoryID) && ((POLine)e.Row).InventoryID == null)
			{
				e.NewValue = taxzone.Current.DfltTaxCategoryID;
			}
			if (vendor != null && vendor.Current != null && (bool)vendor.Current.TaxAgency == true)
			{
				((POLine)e.Row).TaxCategoryID = string.Empty;
				e.Cancel = true;
			}
		}

		protected virtual void POLine_UnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (((POLine)e.Row).InventoryID == null)
			{
				e.NewValue = 0m;
			}
		}

		/*When PO line is entered the system should substitute the Inventory unit price
		based on following logic:
		   1: Look for Inventory ID/Subitem ID in vendor price list. If found:
			  a) If the PO currency and default PO units are the same as in vendor
				 price list set the the unit cost as in vendor price list. 
		  		 The search is done strictly by the Currency And UOM of the specific row;
				 If the Date provided is less then LastEffectiveDate the LastEffectivePrice is taken,
				 else -  EffectivePrice.				
				 
		   2: If no records found for the inventory item use last received cost form
		Inventory Item to receive the base currency price. ID PO currency is different
		from the base currency convert the amount to foreign currency using rate. Last
		received price in PO is always specified in Base units. Convert the price to
		the units specified in PO order. */
		protected virtual void POLine_CuryUnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (this.skipCostDefaulting)
				return;

			POLine row = e.Row as POLine;
			POOrder doc = this.Document.Current;

			if (row?.ManualPrice == true)
			{
				e.NewValue = row.CuryUnitCost ?? 0m;
				return;
			}

			if (row?.InventoryID == null || doc?.VendorID == null)
				return;

			decimal? vendorUnitCost = null;

			if (row.UOM != null)
			{
				DateTime date = Document.Current.OrderDate.Value;
				CurrencyInfo currencyinfo = FindImplementation<IPXCurrencyHelper>().GetCurrencyInfo(doc.CuryInfoID);
				vendorUnitCost = APVendorPriceMaint.CalculateUnitCost(sender, row.VendorID, doc.VendorLocationID, row.InventoryID, row.SiteID, currencyinfo.GetCM(), row.UOM, row.OrderQty, date, row.CuryUnitCost);
				e.NewValue = vendorUnitCost;
			}

			if (vendorUnitCost == null)
			{
				e.NewValue = POItemCostManager.Fetch<POLine.inventoryID, POLine.curyInfoID>(sender.Graph, row,
					doc.VendorID, doc.VendorLocationID, doc.OrderDate, doc.CuryID, row.InventoryID, row.SubItemID, row.SiteID, row.UOM);
			}

			APVendorPriceMaint.CheckNewUnitCost<POLine, POLine.curyUnitCost>(sender, row, e.NewValue);
		}

		protected virtual void POLine_ManualPrice_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row != null)
				sender.SetDefaultExt<POLine.curyUnitCost>(row);
		}

		protected virtual void POLine_CuryUnitCost_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (IsNegativeCostStockItem((POLine)e.Row, (decimal?)e.NewValue))
				throw new PXSetPropertyException(Messages.UnitCostMustBeNonNegativeForStockItem);
			}

		protected virtual bool IsNegativeCostStockItem(POLine row, decimal? value)
		{
			return value < decimal.Zero
				&& row.LineType != null//it is null when insert POLine with assigned LineType and when this method is calling from InventoryID_FieldUpdated event handlers
				&& row.LineType.IsNotIn(
						POLineType.NonStock,
						POLineType.Service,
						POLineType.MiscCharges,
						POLineType.Freight,
						POLineType.NonStockForDropShip,
						POLineType.NonStockForProject,
						POLineType.NonStockForSalesOrder,
						POLineType.NonStockForServiceOrder,
						POLineType.NonStockForManufacturing);
		}
		protected virtual void POLine_PromisedDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row != null)
			{
				if (row.InventoryID != null)
				{
					InventoryItem item = InventoryItem.PK.Find(this, row.InventoryID);

					POOrder order = this.Document.Current;

					PXResult<CRLocation, POVendorInventory> r =
						(PXResult<CRLocation, POVendorInventory>)
						PXSelectJoin<CRLocation,
						LeftJoin<POVendorInventory,
									On<POVendorInventory.inventoryID, Equal<Required<POLine.inventoryID>>,
							 And<POVendorInventory.subItemID, Equal<Required<POLine.subItemID>>,
								 And<POVendorInventory.vendorID, Equal<CRLocation.bAccountID>,
								 And<Where<POVendorInventory.vendorLocationID, Equal<CRLocation.locationID>,
										 Or<POVendorInventory.vendorLocationID, IsNull>>>>>>>,
						Where<CRLocation.bAccountID, Equal<Required<POLine.vendorID>>,
							And<CRLocation.locationID, Equal<Required<POLine.vendorLocationID>>>>,
						OrderBy<Desc<POVendorInventory.vendorLocationID,
										 Asc<CRLocation.locationID>>>>
							.SelectWindowed(this, 0, 1, row.InventoryID, row.SubItemID, order.VendorID, order.VendorLocationID);
					if (r == null) return;

					CRLocation location = r;
					POVendorInventory vendorCatalogue = r;

					if (order.ExpectedDate == null)
					{
						e.NewValue =
							order.OrderDate.Value.AddDays(
							location.VLeadTime.GetValueOrDefault() + vendorCatalogue.AddLeadTimeDays.GetValueOrDefault());
					}

					else
					{
						e.NewValue =
							order.ExpectedDate.Value.AddDays(
							vendorCatalogue.AddLeadTimeDays.GetValueOrDefault());
					}
				}
			}
		}

		protected virtual void POLine_LineType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POLine.orderQty>(e.Row);
			sender.SetDefaultExt<POLine.expenseAcctID>(e.Row);
			sender.SetDefaultExt<POLine.expenseSubID>(e.Row);
			sender.SetDefaultExt<POLine.pOAccrualAcctID>(e.Row);
			sender.SetDefaultExt<POLine.pOAccrualSubID>(e.Row);
		}

		protected virtual void POLine_UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POLine.unitCost>(e.Row);
			sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
			sender.SetDefaultExt<POLine.promisedDate>(e.Row);
			sender.SetValue<POLine.unitCost>(e.Row, null);
		}

		internal bool skipCostDefaulting = false;

		/// <summary>
		/// Gets a value indicating whether to skip the <see cref="POLine.CuryUnitCost"/>'s field defaulting event. Used by PriceUnits customization for Lexware
		/// </summary>       
		public bool SkipCostDefaulting => skipCostDefaulting;


		[PopupMessage]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void POLine_InventoryID_CacheAttached(PXCache sender)
		{
		}

		protected virtual void POLine_VendorID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;

			sender.SetDefaultExt<POLine.rcptQtyMin>(e.Row);
			sender.SetDefaultExt<POLine.rcptQtyMax>(e.Row);
			sender.SetDefaultExt<POLine.rcptQtyThreshold>(e.Row);
			sender.SetDefaultExt<POLine.rcptQtyAction>(e.Row);

			sender.SetDefaultExt<POLine.expenseAcctID>(e.Row);
			try
			{
				sender.SetDefaultExt<POLine.expenseSubID>(e.Row);
			}
			catch (PXSetPropertyException)
			{
				sender.SetValue<POLine.expenseSubID>(e.Row, null);
			}

			sender.SetDefaultExt<POLine.pOAccrualAcctID>(e.Row);
			try
			{
				sender.SetDefaultExt<POLine.pOAccrualSubID>(e.Row);
			}
			catch (PXSetPropertyException)
			{
				sender.SetValue<POLine.pOAccrualSubID>(e.Row, null);
			}
		}

		protected virtual void _(Events.FieldVerifying<POLine, POLine.inventoryID> e)
		{
			if (e.Row != null && IsItemReceivedOrAPDocCreated(e.Row))
			{
				e.Cancel = true;
				e.NewValue = InventoryItem.PK.Find(this, (int?)e.OldValue)?.InventoryCD;
				throw new PXSetPropertyException(Messages.CannotChangeItemForLineWithExistingAPDoc, PXErrorLevel.Error, e.NewValue);
			}

			if (e.Row != null && e.Row.OrderType == POOrderType.Blanket
				&& e.OldValue != e.NewValue
				&& BlanketPOLineIsReferencedInPOOrder(e.Row, out var relatedPOLine))
			{
				e.Cancel = true;
				e.NewValue = InventoryItem.PK.Find(this, e.Row.InventoryID)?.InventoryCD;
				throw new PXSetPropertyException(Messages.InventoryIDCannotBeChangedInBlanketPOLine, PXErrorLevel.Error, e.NewValue, relatedPOLine.OrderNbr);
			}
		}

		protected virtual void POLine_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			this.skipCostDefaulting = true;
			sender.SetDefaultExt<POLine.accrueCost>(e.Row);
			sender.SetDefaultExt<POLine.processNonStockAsServiceViaPR>(e.Row);
			sender.SetDefaultExt<POLine.vendorID>(e.Row);
			sender.SetDefaultExt<POLine.subItemID>(e.Row);
			sender.SetDefaultExt<POLine.siteID>(e.Row);
			sender.SetDefaultExt<POLine.expenseAcctID>(e.Row);
			sender.SetDefaultExt<POLine.expenseSubID>(e.Row);
			sender.SetDefaultExt<POLine.pOAccrualAcctID>(e.Row);
			sender.SetDefaultExt<POLine.pOAccrualSubID>(e.Row);
			sender.SetDefaultExt<POLine.taxCategoryID>(e.Row);
			sender.SetDefaultExt<POLine.uOM>(e.Row);
			sender.SetDefaultExt<POLine.unitCost>(e.Row);
			this.skipCostDefaulting = false;
			sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
			sender.SetDefaultExt<POLine.promisedDate>(e.Row);
			sender.SetValue<POLine.unitCost>(e.Row, null);
			sender.SetDefaultExt<POLine.siteID>(e.Row);
			sender.SetDefaultExt<POLine.unitWeight>(e.Row);
			sender.SetDefaultExt<POLine.unitVolume>(e.Row);

			POLine tran = e.Row as POLine;
			InventoryItem item;
			if (tran != null && (item = InventoryItem.PK.Find(this, tran.InventoryID)) != null)
			{
				tran.TranDesc = PXDBLocalizableStringAttribute.GetTranslation(Caches[typeof(InventoryItem)], item, "Descr", vendor.Current?.LocaleName);
				tran.POAccrualType = (string)PXFormulaAttribute.Evaluate<POLine.pOAccrualType>(Transactions.Cache, tran);
			}

			if (tran?.POType == POOrderType.Blanket && tran.PONbr != null
				&& e.OldValue != null && tran.InventoryID != (int?)e.OldValue)
			{
				string oldBlanketNumber = tran.PONbr;
				sender.SetValueExt<POLine.pOLineNbr>(tran, null);
				sender.SetValueExt<POLine.pONbr>(tran, null);
				sender.SetValueExt<POLine.pOType>(tran, null);
				sender.RaiseExceptionHandling<POLine.inventoryID>(tran, tran.InventoryID,
					new PXSetPropertyException(Messages.LineDelinkedFromBlanket, PXErrorLevel.Warning, oldBlanketNumber));
			}
		}

		protected virtual void POLine_ProjectID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;

			if (row.OrderType == POOrderType.ProjectDropShip)
			{
				if (row.ProjectID == null) return;
				sender.SetDefaultExt<POLine.expenseAcctID>(row);
			}

			sender.SetDefaultExt<POLine.expenseSubID>(row);
		}

		protected virtual void POLine_TaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;

			if (row.OrderType == POOrderType.ProjectDropShip)
			{
				if (row.ProjectID == null) return;
				sender.SetDefaultExt<POLine.expenseAcctID>(row);
			}

			sender.SetDefaultExt<POLine.expenseSubID>(row);
		}

		protected virtual void POLine_TaskID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row == null) return;
			if (!(e.NewValue is Int32)) return;

			PMProject project = PMProject.PK.Find(this, row.ProjectID);

			if (e.NewValue != null && POLineType.IsStock(row.LineType) && row.SiteID != null && row.OrderType != POOrderType.ProjectDropShip && project?.AccountingMode == ProjectAccountingModes.Linked)
			{
				HashSet<int> validWarehouses = new HashSet<int>();
				HashSet<int> explicitWarehouses = new HashSet<int>();

				PXResultset<INLocation> projectLocations = PXSelectReadonly<INLocation,
					Where<INLocation.projectID, Equal<Required<INLocation.projectID>>,
					And2<Where<INLocation.taskID, IsNull>, Or<INLocation.taskID, Equal<Required<INLocation.taskID>>>>>>.Select(this, row.ProjectID, e.NewValue);

				foreach (INLocation taskLocation in projectLocations)
				{
					validWarehouses.Add(taskLocation.SiteID.Value);

					if (taskLocation.TaskID == (int?)e.NewValue)
					{
						explicitWarehouses.Add(taskLocation.SiteID.Value);
					}
				}

				if (explicitWarehouses.Count > 0 && !explicitWarehouses.Contains(row.SiteID.Value) ||
					!validWarehouses.Contains(row.SiteID.Value))
				{
					string taskCD = null;
					PMTask task = PMTask.PK.FindDirty(this, row.ProjectID, row.TaskID);
					if (task != null)
					{
						taskCD = task.TaskCD; // For Error.
					}

					if (explicitWarehouses.Count > 0)
					{
						INSite warehouse = INSite.PK.Find(this, explicitWarehouses.First());
						sender.RaiseExceptionHandling<POLine.taskID>(row, taskCD, new PXSetPropertyException(Messages.ProjectTaskIsAssociatedWithAnotherWarehouse, PXErrorLevel.Warning, warehouse.SiteCD));
					}
					else
					{
						sender.RaiseExceptionHandling<POLine.taskID>(row, taskCD, new PXSetPropertyException(Messages.ProjectTaskIsNotAssociatedWithAnyInLocation, PXErrorLevel.Warning));
					}
				}

			}

		}

		protected virtual void POLine_SiteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row == null) return;

			if (POLineType.IsProjectDropShip(row.LineType))
			{
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		protected virtual void POLine_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row == null) return;

			if ((row.LineType == POLineType.GoodsForInventory ||
				row.LineType == POLineType.GoodsForSalesOrder ||
				row.LineType == POLineType.GoodsForServiceOrder ||
				row.LineType == POLineType.NonStock ||
				row.LineType == POLineType.Service ||
				row.LineType == POLineType.GoodsForReplenishment ||
				row.LineType == POLineType.GoodsForDropShip)
				&& row.SiteID != null)
			{
				sender.SetDefaultExt<POLine.expenseAcctID>(e.Row);
				sender.SetDefaultExt<POLine.expenseSubID>(e.Row);

				APTran linkedAPTran = SelectFrom<APTran>.
					Where<
					APTran.pOOrderType.IsEqual<@P.AsString.ASCII>.
					And<APTran.pONbr.IsEqual<@P.AsString>.
					And<APTran.pOLineNbr.IsEqual<@P.AsInt>.
					And<APTran.tranType.IsNotEqual<APDocType.prepayment>>>>>.View.Select(this, row.OrderType, row.OrderNbr, row.LineNbr);

				POReceiptLine linkedReceipts = SelectFrom<POReceiptLine>.
					Where<
					POReceiptLine.pOType.IsEqual<@P.AsString.ASCII>.
					And<POReceiptLine.pONbr.IsEqual<@P.AsString>.
					And<POReceiptLine.pOLineNbr.IsEqual<@P.AsInt>>>>.View.Select(this, row.OrderType, row.OrderNbr, row.LineNbr);

				if (linkedAPTran == null && linkedReceipts==null)
				{
				sender.SetDefaultExt<POLine.pOAccrualAcctID>(e.Row);
				sender.SetDefaultExt<POLine.pOAccrualSubID>(e.Row);
			}
			}

			sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
		}

		protected virtual void POLine_SubItemID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<POLine.uOM>(e.Row);
			sender.SetDefaultExt<POLine.unitCost>(e.Row);
			sender.SetDefaultExt<POLine.curyUnitCost>(e.Row);
			sender.SetDefaultExt<POLine.promisedDate>(e.Row);
		}

		protected virtual void POLine_DiscountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row != null && e.ExternalCall)
			{
				_discountEngine.UpdateManualLineDiscount(sender, Transactions, row, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, DiscountEngine.DefaultAPDiscountCalculationParameters);
			}
		}

		protected virtual void POLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			POOrder doc = this.Document.Current;
			if (doc == null || row == null) return;

			if (IsExport && !IsContractBasedAPI) return;//for performance 

			InventoryItem item = InventoryItem.PK.Find(this, row.InventoryID);
			bool isItemConverted = (item?.IsConverted == true && row.IsStockItem != null && row.IsStockItem != item.StkItem);
			bool isLegacyPOLinkedToSO = doc?.IsLegacyDropShip == true && row.Completed == true && IsLinkedToSO(row);

			if (isItemConverted)
			{
				PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
				if (row.Closed != true)
					PXUIFieldAttribute.SetEnabled<POLine.closed>(sender, e.Row, true);
			}
			else if (this.Document.Current.Hold != true || isLegacyPOLinkedToSO)
			{
				PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
				PXUIFieldAttribute.SetEnabled<POLine.closed>(sender, e.Row, doc.Status.IsIn(POOrderStatus.Hold, POOrderStatus.PendingApproval, POOrderStatus.Open));

				if (doc.Status.IsIn(POOrderStatus.PendingApproval, POOrderStatus.Open)
					&& !isLegacyPOLinkedToSO)
				{
					PXUIFieldAttribute.SetEnabled<POLine.promisedDate>(sender, e.Row, true);
					PXUIFieldAttribute.SetEnabled<POLine.cancelled>(sender, e.Row,
																			row.ReceivedQty == 0 ||
																			row.ReceivedQty < row.OrderQty * row.RcptQtyThreshold / 100);

					PXUIFieldAttribute.SetEnabled<POLine.completed>(sender, e.Row,
																			row.ReceivedQty == 0 ||
																			row.ReceivedQty >= row.OrderQty * row.RcptQtyThreshold / 100);
				}
			}
			else
			{
				if (!this._blockUIUpdate)
				{
					bool isNonStockKit = (!row.IsStockItem ?? false) && (row.IsKit ?? false);
					bool isPMVisible = ProjectAttribute.IsPMVisible(BatchModule.PO);
					bool retainageApply = (doc?.RetainageApply == true);
					bool requireSingleProject = RequireSingleProject(doc);
					PXUIFieldAttribute.SetEnabled<POLine.completePOLine>(sender, e.Row, false);
					switch (row.LineType)
					{
						case POLineType.Description:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.branchID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.tranDesc>(sender, e.Row, true);
							break;

						case POLineType.Freight:
						case POLineType.MiscCharges:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.branchID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.tranDesc>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.taxCategoryID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.curyLineAmt>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.cancelled>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.projectID>(sender, e.Row, isPMVisible && !requireSingleProject);
							PXUIFieldAttribute.SetEnabled<POLine.taskID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.retainagePct>(sender, e.Row, retainageApply);
							PXUIFieldAttribute.SetEnabled<POLine.curyRetainageAmt>(sender, e.Row, retainageApply);
							break;

						case POLineType.NonStock:
						case POLineType.Service:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.siteID>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.subItemID>(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.cancelled>(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.curyExtCost>(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.uOM>(sender, e.Row, row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.completePOLine>(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.projectID>(sender, e.Row, isPMVisible && !requireSingleProject);
							PXUIFieldAttribute.SetEnabled<POLine.retainagePct>(sender, e.Row, retainageApply);
							PXUIFieldAttribute.SetEnabled<POLine.curyRetainageAmt>(sender, e.Row, retainageApply);
							break;

						default:
							PXUIFieldAttribute.SetEnabled(sender, e.Row, true);
							PXUIFieldAttribute.SetEnabled<POLine.inventoryID>(sender, e.Row, row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.subItemID>(sender, e.Row, !isNonStockKit && row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.uOM>(sender, e.Row, row.ReceivedQty == 0);
							PXUIFieldAttribute.SetEnabled<POLine.discountSequenceID>(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.completePOLine>(sender, e.Row, false);
							PXUIFieldAttribute.SetEnabled<POLine.projectID>(sender, e.Row, isPMVisible && !requireSingleProject);
							PXUIFieldAttribute.SetEnabled<POLine.retainagePct>(sender, e.Row, retainageApply);
							PXUIFieldAttribute.SetEnabled<POLine.curyRetainageAmt>(sender, e.Row, retainageApply);
							PXUIFieldAttribute.SetEnabled<POLine.isSpecialOrder>(sender, e.Row, false);
							break;
					}

					PXUIFieldAttribute.SetEnabled<POLine.lineType>(sender, e.Row, POLineType.IsDefault(row.LineType) && row.ReceivedQty == 0);
					PXUIFieldAttribute.SetEnabled<POLine.receivedQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.curyExtCost>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.baseOrderQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.pOAccrualType>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.completedQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.billedQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.curyBilledAmt>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.openQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.unbilledQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.curyUnbilledAmt>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.displayReqPrepaidQty>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.curyReqPrepaidAmt>(sender, e.Row, false);
					PXUIFieldAttribute.SetEnabled<POLine.allowEditUnitCostInPR>(sender, e.Row, false);

					PXUIFieldAttribute.SetEnabled<POLine.expenseAcctID>(sender, e.Row, IsExpenseAccountEnabled(row));
					PXUIFieldAttribute.SetEnabled<POLine.expenseSubID>(sender, e.Row, IsExpenseAccountEnabled(row));

					PXUIFieldAttribute.SetEnabled<POLine.pOAccrualAcctID>(sender, e.Row, IsAccrualAccountEnabled(row));
					PXUIFieldAttribute.SetEnabled<POLine.pOAccrualSubID>(sender, e.Row, IsAccrualAccountEnabled(row));
				}
				if (doc.Cancelled == false && doc.Status.IsNotIn(POOrderStatus.Completed, POOrderStatus.Closed))
				{
					if (row.POType == POOrderType.Blanket && !String.IsNullOrEmpty(row.PONbr))
					{
						POOrder source = PXSelectReadonly<POOrder, Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
												And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>.Select(this, row.POType, row.PONbr);
						if (source != null && source.ExpirationDate != null && source.ExpirationDate < doc.OrderDate)
						{
							this.Transactions.Cache.RaiseExceptionHandling<POLine.lineType>(row, row.LineType, new PXSetPropertyException(Messages.SourcePOOrderExpiresBeforeTheDateOfDocument, PXErrorLevel.RowWarning, source.ExpirationDate.Value, source.OrderNbr));
						}
					}
				}

				if (doc.ShipDestType == POShippingDestination.Site && doc.SiteID != null
					&& row.SiteID != null && doc.SiteID != row.SiteID)
				{
					INSite site = INSite.PK.Find(this, row.SiteID);
					sender.RaiseExceptionHandling<POLine.siteID>(e.Row, site?.SiteCD,
						new PXSetPropertyException(Messages.DetailSiteDiffersFromShipping, PXErrorLevel.Warning, site?.SiteCD));
				} // Checking for existing warning in order not to clear Inter-branch error.
				else if (PXUIFieldAttribute.GetWarning<POLine.siteID>(sender, row) != null)
				{
					INSite site = INSite.PK.Find(this, row.SiteID);
					sender.RaiseExceptionHandling<POLine.siteID>(e.Row, site?.SiteCD, null);
				}
			}

			PXUIFieldAttribute.SetVisible<POLine.pONbr>(sender, e.Row, doc.OrderType != POOrderType.ProjectDropShip);
			PXUIFieldAttribute.SetVisible<POLine.pOType>(sender, e.Row, doc.OrderType != POOrderType.ProjectDropShip);
			PXUIFieldAttribute.SetEnabled<POLine.pONbr>(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<POLine.pOType>(sender, e.Row, false);
			PXUIFieldAttribute.SetEnabled<POLine.dRTermStartDate>(sender, e.Row, row?.ItemRequiresTerms == true);
			PXUIFieldAttribute.SetEnabled<POLine.dRTermEndDate>(sender, e.Row, row?.ItemRequiresTerms == true);
		}

		protected virtual void POLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			POLine row = (POLine)e.Row;
            if (row == null || e.Operation.Command() == PXDBOperation.Delete)
                return;

			if (Document.Current?.OrderType == POOrderType.DropShip && Document.Current.Hold == false && !IsLineWithDropShipLocation(row))
			{
				INSite site = INSite.PK.Find(this, row.SiteID);
				if (sender.RaiseExceptionHandling<POLine.siteID>(row, site.SiteCD, new PXSetPropertyException(Messages.SiteWithoutDropShipLocation)))
				{
					throw new PXRowPersistingException(typeof(POLine.siteID).Name, site.SiteCD, Messages.SiteWithoutDropShipLocation);
				}
			}

			bool isStock = POLineType.IsStock(row.LineType);
			bool isNonStock = POLineType.IsNonStock(row.LineType);
			bool isNonStockKit = (!row.IsStockItem ?? false) && (row.IsKit ?? false);

			PXDefaultAttribute.SetPersistingCheck<POLine.inventoryID>(sender, e.Row, POLineType.IsStock(row.LineType) || POLineType.IsNonStock(row.LineType) && !POLineType.IsService(row.LineType) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.subItemID>(sender, e.Row, (isStock && !isNonStockKit) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.uOM>(sender, e.Row, isStock || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.orderQty>(sender, e.Row, isStock || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.baseOrderQty>(sender, e.Row, isStock || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.curyUnitCost>(sender, e.Row, isStock || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.unitCost>(sender, e.Row, isStock || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<POLine.expenseAcctID>(sender, e.Row, IsExpenseAccountEnabled(row) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.expenseSubID>(sender, e.Row, IsExpenseAccountEnabled(row) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<POLine.pOAccrualAcctID>(sender, e.Row, IsAccrualAccountRequired(row) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<POLine.pOAccrualSubID>(sender, e.Row, IsAccrualAccountRequired(row) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXDefaultAttribute.SetPersistingCheck<POLine.curyExtCost>(sender, e.Row, row.LineType == POLineType.Description ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);

			PXDefaultAttribute.SetPersistingCheck<POLine.siteID>(sender, e.Row, !POLineType.IsProjectDropShip(row.LineType) && (isStock || isNonStock && !POLineType.IsService(row.LineType)) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			
			if (row.OrderQty == Decimal.Zero && !IsZeroQuantityValid(row))
            {
				string msg = isStock ? Messages.POLineQuantityMustBeGreaterThanZero : Messages.POLineQuantityMustBeGreaterThanZeroNonStock;
				sender.RaiseExceptionHandling<POLine.orderQty>(row, row.OrderQty, new PXSetPropertyException(msg, PXErrorLevel.Error));
			}

			if (row.CuryUnitCost == Decimal.Zero && !IsZeroUnitCostValid(row))
            {
				sender.RaiseExceptionHandling<POLine.curyUnitCost>(row, row.CuryUnitCost, new PXSetPropertyException(Messages.UnitCostShouldBeNonZeroForStockItems, PXErrorLevel.Warning));
			}
						
			CheckProjectAccountRule(sender, row);
		}

		protected virtual void POLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			ClearUnused(row);

			if (row.OrderType == POOrderType.ProjectDropShip && row.ProjectID != null && row.TaskID != null && row.ExpenseAcctID == null)
			{
				sender.SetDefaultExt<POLine.expenseAcctID>(row);
			}

			RecalculateDiscounts(sender, (POLine)e.Row);

			TaxAttribute.Calculate<POLine.taxCategoryID>(sender, e);
		}

		protected virtual void POLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row.InventoryID != ((POLine)e.OldRow).InventoryID)
			{

			}
			if (row.LineType != ((POLine)e.OldRow).LineType)
			{
                ClearUnused(row);
			}

			if (row.OrderType == POOrderType.ProjectDropShip && !sender.ObjectsEqual<POLine.dropshipExpenseRecording>(e.Row, e.OldRow))
			{
				if (row.DropshipExpenseRecording == DropshipExpenseRecordingOption.OnReceiptRelease)
				{
					sender.SetDefaultExt<POLine.pOAccrualAcctID>(row);
					sender.SetDefaultExt<POLine.pOAccrualSubID>(row);
				}
				else
				{
					sender.SetValueExt<POLine.pOAccrualAcctID>(row, null);
					sender.SetValueExt<POLine.pOAccrualSubID>(row, null);
				}
			}

			if (!sender.ObjectsEqual<POLine.vendorID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.branchID>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<POLine.inventoryID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.baseOrderQty>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<POLine.curyUnitCost>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.curyExtCost>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<POLine.curyLineAmt>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.curyDiscAmt>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<POLine.discPct>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.manualDisc>(e.Row, e.OldRow) ||
					!sender.ObjectsEqual<POLine.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.siteID>(e.Row, e.OldRow))
				RecalculateDiscounts(sender, row);

			if ((e.ExternalCall || sender.Graph.IsImport) && sender.ObjectsEqual<POLine.vendorID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<POLine.inventoryID>(e.Row, e.OldRow) && sender.ObjectsEqual<POLine.uOM>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<POLine.orderQty>(e.Row, e.OldRow) && sender.ObjectsEqual<POLine.branchID>(e.Row, e.OldRow)
				&& sender.ObjectsEqual<POLine.siteID>(e.Row, e.OldRow) && sender.ObjectsEqual<POLine.manualPrice>(e.Row, e.OldRow)
				&& (!sender.ObjectsEqual<POLine.curyUnitCost>(e.Row, e.OldRow) || !sender.ObjectsEqual<POLine.curyLineAmt>(e.Row, e.OldRow)))
				row.ManualPrice = true;

			TaxAttribute.Calculate<POLine.taxCategoryID>(sender, e);

			if (!sender.ObjectsEqual<POLine.completed>(e.Row, e.OldRow) && row.Completed == true
				|| !sender.ObjectsEqual<POLine.closed>(e.Row, e.OldRow) && row.Closed == true)
			{
				if (RaiseOrderEvents(Document.Current))
				{
					Document.View.RequestRefresh();
					Transactions.View.RequestRefresh();
				}
			}
		}

		protected virtual void POLine_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row.ReceivedQty > 0)
			{
				e.Cancel = true;
				throw new PXException(Messages.POOrderLineHasReceiptsAndCannotBeDeleted);
			}

			if (PXSelectGroupBy<POReceiptLine,
				Where<POReceiptLine.pOType, Equal<Current<POLine.orderType>>,
				And<POReceiptLine.pONbr, Equal<Current<POLine.orderNbr>>,
				And<POReceiptLine.pOLineNbr, Equal<Current<POLine.lineNbr>>>>>,
				Aggregate<Count>>.SelectSingleBound(this, new object[] { row }).RowCount > 0)
			{
				e.Cancel = true;
				throw new PXException(Messages.POOrderLineHasReceiptsAndCannotBeDeleted);
			}

			if (PXSelectGroupBy<APTran,
				Where<APTran.pOOrderType, Equal<Current<POLine.orderType>>,
				And<APTran.pONbr, Equal<Current<POLine.orderNbr>>,
				And<APTran.pOLineNbr, Equal<Current<POLine.lineNbr>>,
				And<APTran.released, Equal<True>>>>>,
				Aggregate<Count>>.SelectSingleBound(this, new object[] { row }).RowCount > 0)
			{
				e.Cancel = true;
				throw new PXException(Messages.POOrderLineHasBillsReleasedAndCannotBeDeleted);
			}

			PXResult<APTran> apRecord = PXSelectJoin<APTran,
				LeftJoin<POOrderPrepayment,
					On<POOrderPrepayment.aPDocType, Equal<APTran.tranType>,
					And<POOrderPrepayment.aPRefNbr, Equal<APTran.refNbr>,
					And<POOrderPrepayment.orderType, Equal<Current<POLine.orderType>>,
					And<POOrderPrepayment.orderNbr, Equal<Current<POLine.orderNbr>>>>>>>,
				Where<APTran.pOOrderType, Equal<Current<POLine.orderType>>,
				And<APTran.pONbr, Equal<Current<POLine.orderNbr>>,
				And<APTran.pOLineNbr, Equal<Current<POLine.lineNbr>>>>>,
				OrderBy<Desc<POOrderPrepayment.aPRefNbr>>>.SelectSingleBound(this, new object[] { row });
			if (apRecord != null && apRecord.GetItem<POOrderPrepayment>().APRefNbr != null)
			{
				e.Cancel = true;
				throw new PXException(Messages.POOrderLineHasPrepaymentRequestAndCannotBeDeleted);
			}
			else if (apRecord != null)
			{
				e.Cancel = true;
				throw new PXException(Messages.POOrderLineHasBillsGeneratedAndCannotBeDeleted);
			}

			if (row.OrderType == POOrderType.Blanket && BlanketPOLineIsReferencedInPOOrder(row, out var relatedPOLine))
			{
				e.Cancel = true;
				throw new PXException(Messages.BlanketPOLineCannotBeDeleted, relatedPOLine.OrderNbr);
			}
		}

		protected virtual void POLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			if (Document.Current != null && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.Deleted && Document.Cache.GetStatus(Document.Current) != PXEntryStatus.InsertedDeleted)
			{
				_discountEngine.RecalculateGroupAndDocumentDiscounts(sender, Transactions, null, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, DiscountEngine.DefaultAPDiscountCalculationParameters);
				RecalculateTotalDiscount();
			}
		}

		protected virtual void POLine_Closed_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row.Closed == true)
			{
				sender.SetValueExt<POLine.completed>(row, true);
			}
		}

		protected virtual void POLine_Completed_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if(row.Completed == false)
			{
				sender.SetValueExt<POLine.closed>(row, false);
			}
		}

		protected virtual void POLine_Completed_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row != null && (bool)e.NewValue == true)
			{
				POReceipt receipt = PXSelectJoin<POReceipt,
					InnerJoin<POReceiptLine, On<POReceiptLine.FK.Receipt>>,
					Where<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
						And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
						And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>,
						And<POReceiptLine.released, Equal<False>>>>>>.Select(this, row?.OrderType, row?.OrderNbr, row?.LineNbr);

				if (receipt != null)
				{
					ThrowErrorWhenPurchaseReceiptExists(receipt, row);
				}
			}
		}

		protected virtual void POLine_Cancelled_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			POLine row = (POLine)e.Row;
			if (row.Cancelled == true)
			{
				sender.SetValueExt<POLine.completed>(row, true);
			}
			else if (row.OpenQty < row.OrderQty * row.RcptQtyThreshold / 100m)
			{
				sender.SetValueExt<POLine.completed>(row, false);
			}

			sender.RaiseFieldUpdated<POLine.taxCategoryID>(row, row.TaxCategoryID);
		}

		protected virtual void POLine_Cancelled_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			POLine line = (POLine)e.Row;
			if ((bool?)e.NewValue == true)
		{
				if (line.OrderType == POOrderType.Blanket && line.ReceivedQty > 0m)
				{
					ThrowErrorWhenPurchaseReceiptExists(null, line);
				}
				else
				{
			POReceipt receipt = PXSelectJoin<POReceipt,
						InnerJoin<POReceiptLine, On<POReceiptLine.FK.Receipt>>,
						Where<POReceiptLine.pOType, Equal<Required<POReceiptLine.pOType>>,
							And<POReceiptLine.pONbr, Equal<Required<POReceiptLine.pONbr>>,
						And<POReceiptLine.pOLineNbr, Equal<Required<POReceiptLine.pOLineNbr>>>>>,
				OrderBy<Asc<POReceipt.released>>>.Select(this, line?.OrderType, line?.OrderNbr, line?.LineNbr);

			if (receipt != null)
			{
					ThrowErrorWhenPurchaseReceiptExists(receipt, line);
				}
			}
		}
		}

		protected bool IsRequired(string poLineType)
		{
			switch (poLineType)
			{
				case PX.Objects.PO.POLineType.NonStock:
				case PX.Objects.PO.POLineType.Freight:
				case PX.Objects.PO.POLineType.Service:
					return true;

				default:
					return false;
			}
		}

		protected virtual void POLine_ProjectID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			POLine row = e.Row as POLine;
			if (row != null && ProjectAttribute.IsPMVisible(BatchModule.PO))
			{
				// Acuminator disable once PX1074 SetupNotEnteredExceptionInEventHandlers [Justification: new function around old check - RequireSingleProjectPerDocument is disabled in acuminator ignore file]
				if (RequireSingleProject(Document.Current) && Document.Current != null)
				{
					e.NewValue = Document.Current.ProjectID;
					e.Cancel = true;
				}
				else if (location.Current != null && location.Current.VDefProjectID != null && IsRequired(row.LineType))
				{
					PX.Objects.PM.PMProject project = PXSelect<PM.PMProject, Where<PM.PMProject.contractID, Equal<Required<PM.PMProject.contractID>>>>.Select(this, location.Current.VDefProjectID);
					if (project != null)
						e.NewValue = project.ContractCD;
				}
			}
		}

		[PXBool]
		[DRTerms.Dates(typeof(POLine.dRTermStartDate), typeof(POLine.dRTermEndDate), typeof(POLine.inventoryID), VerifyDatesPresent = false)]
		protected virtual void POLine_ItemRequiresTerms_CacheAttached(PXCache sender) { }

		#endregion

		#region SOLineSplit3 events
		protected virtual void SOLineSplit3_POUOM_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			SOLineSplit3 soline = (SOLineSplit3)e.Row;
			if (soline == null) return;
			POLine orig_line = PXSelect<POLine, Where<POLine.orderType, Equal<Current<SOLineSplit3.pOType>>,
				And<POLine.orderNbr, Equal<Current<SOLineSplit3.pONbr>>,
				And<POLine.lineNbr, Equal<Current<SOLineSplit3.pOLineNbr>>>>>>.SelectSingleBound(this, new object[] { soline });

			e.ReturnValue = (orig_line != null && orig_line.UOM != null) ? orig_line.UOM : soline.UOM;
		}
		protected virtual void SOLineSplit3_POUOMOrderQty_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			SOLineSplit3 soline = (SOLineSplit3)e.Row;
			if (soline == null) return;

			POLine orig_line = PXSelect<POLine, Where<POLine.orderType, Equal<Current<SOLineSplit3.pOType>>,
				And<POLine.orderNbr, Equal<Current<SOLineSplit3.pONbr>>,
				And<POLine.lineNbr, Equal<Current<SOLineSplit3.pOLineNbr>>>>>>.SelectSingleBound(this, new object[] { soline });
			if (orig_line != null)
			{
				string uom = orig_line.UOM ?? soline.UOM;
				if (string.Equals(soline.UOM, uom) == false)
				{
					decimal BaseOrderQty = INUnitAttribute.ConvertToBase<SOLineSplit3.inventoryID>(sender, soline, soline.UOM, (decimal)soline.OrderQty, INPrecision.QUANTITY);
					e.ReturnValue = INUnitAttribute.ConvertFromBase<SOLineSplit3.inventoryID>(sender, soline, uom, BaseOrderQty, INPrecision.QUANTITY);
				}
				else
				{
					e.ReturnValue = soline.OrderQty;
				}
			}
		}
		#endregion

		#region Vendor
		protected virtual void Vendor_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true; //Vendor should not be updated from this screen
		}
		#endregion

		#region Currency Info

		protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (e.Row != null && HasDetailRecords())
				{
					e.NewValue = ((CurrencyInfo)e.Row).CuryID ?? vendor?.Current?.CuryID;
					e.Cancel = true;
				}
				else if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryID))
				{
					e.NewValue = vendor.Current.CuryID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				CM.CMSetup cmsetup = PXSelect<CM.CMSetup>.Select(this);

				if (e.Row != null && HasDetailRecords())
				{
					e.NewValue = ((CurrencyInfo)e.Row).CuryRateTypeID ?? vendor?.Current?.CuryRateTypeID ?? cmsetup?.APRateTypeDflt ?? e.NewValue; 
					e.Cancel = true;
				}
				else
				{
					if (vendor.Current != null && !string.IsNullOrEmpty(vendor.Current.CuryRateTypeID))
					{
						e.NewValue = vendor.Current.CuryRateTypeID;
						e.Cancel = true;
					}
					else
					{
						if (cmsetup != null)
						{
							e.NewValue = cmsetup.APRateTypeDflt;
							e.Cancel = true;
						}
					}
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (Document.Cache.Current != null)
			{
				e.NewValue = ((POOrder)Document.Cache.Current).OrderDate;
				e.Cancel = true;
			}
		}

		#endregion

		#region POOrderDiscountDetail events

		protected virtual void POOrderDiscountDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (Document == null || Document.Current != null)
				return;

			//Event handler is kept to avoid breaking changes.
		}

		protected virtual void POOrderDiscountDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			POOrderDiscountDetail discountDetail = (POOrderDiscountDetail)e.Row;
			if (!_discountEngine.IsInternalDiscountEngineCall && discountDetail != null)
			{
				if (discountDetail.DiscountID != null)
				{
					_discountEngine.InsertManualDocGroupDiscount(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, DiscountEngine.DefaultAPDiscountCalculationParameters);
					RecalculateTotalDiscount();
				}

				if (_discountEngine.SetExternalManualDocDiscount(Transactions.Cache, Transactions, DiscountDetails, discountDetail, null, DiscountEngine.DefaultAPDiscountCalculationParameters))
					RecalculateTotalDiscount();
			}
		}

		protected virtual void POOrderDiscountDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			POOrderDiscountDetail discountDetail = (POOrderDiscountDetail)e.Row;
			POOrderDiscountDetail oldDiscountDetail = (POOrderDiscountDetail)e.OldRow;
			if (!_discountEngine.IsInternalDiscountEngineCall && discountDetail != null)
			{
				if (!sender.ObjectsEqual<POOrderDiscountDetail.skipDiscount>(e.Row, e.OldRow))
				{
					_discountEngine.UpdateDocumentDiscount(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, discountDetail.Type != DiscountType.Document, DiscountEngine.DefaultAPDiscountCalculationParameters);
					RecalculateTotalDiscount();
				}
				if (!sender.ObjectsEqual<POOrderDiscountDetail.discountID>(e.Row, e.OldRow) || !sender.ObjectsEqual<POOrderDiscountDetail.discountSequenceID>(e.Row, e.OldRow))
				{
					_discountEngine.UpdateManualDocGroupDiscount(Transactions.Cache, Transactions, DiscountDetails, discountDetail, discountDetail.DiscountID, sender.ObjectsEqual<POOrderDiscountDetail.discountID>(e.Row, e.OldRow) ? discountDetail.DiscountSequenceID : null, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, DiscountEngine.DefaultAPDiscountCalculationParameters);
					RecalculateTotalDiscount();
				}

				if (_discountEngine.SetExternalManualDocDiscount(Transactions.Cache, Transactions, DiscountDetails, discountDetail, oldDiscountDetail, DiscountEngine.DefaultAPDiscountCalculationParameters))
					RecalculateTotalDiscount();
			}
		}

		protected virtual void POOrderDiscountDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			POOrderDiscountDetail discountDetail = (POOrderDiscountDetail)e.Row;
			if (!_discountEngine.IsInternalDiscountEngineCall && discountDetail != null)
			{
				_discountEngine.UpdateDocumentDiscount(Transactions.Cache, Transactions, DiscountDetails, Document.Current.BranchID, Document.Current.VendorLocationID, Document.Current.OrderDate, (discountDetail.Type != null && discountDetail.Type != DiscountType.Document && discountDetail.Type != DiscountType.ExternalDocument), DiscountEngine.DefaultAPDiscountCalculationParameters);
			}
			RecalculateTotalDiscount();
		}

		protected virtual void POOrderDiscountDetail_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			POOrderDiscountDetail discountDetail = (POOrderDiscountDetail)e.Row;

			bool isExternalDiscount = discountDetail.Type == DiscountType.ExternalDocument;

			PXDefaultAttribute.SetPersistingCheck<POOrderDiscountDetail.discountID>(sender, discountDetail, isExternalDiscount ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
			PXDefaultAttribute.SetPersistingCheck<POOrderDiscountDetail.discountSequenceID>(sender, discountDetail, isExternalDiscount ? PXPersistingCheck.Nothing : PXPersistingCheck.NullOrBlank);
		}

		#endregion

		protected virtual void ClearUnused(POLine poLine)
		{
			if (poLine.LineType == POLineType.Description
				|| poLine.LineType == POLineType.Freight || poLine.LineType == POLineType.MiscCharges)
			{
				poLine.InventoryID = null;
				poLine.SubItemID = null;
				poLine.UOM = null;
				poLine.ReceivedQty = Decimal.Zero;
				poLine.BaseReceivedQty = Decimal.Zero;
				poLine.OpenQty = poLine.OrderQty;
				poLine.BaseOpenQty = poLine.BaseOrderQty;
				poLine.CuryUnitCost = decimal.Zero;
				poLine.UnitCost = decimal.Zero;
				poLine.UnitVolume = decimal.Zero;
				poLine.UnitWeight = decimal.Zero;
				poLine.RcptQtyAction = POReceiptQtyAction.Accept;
				poLine.RcptQtyMax = 100;
				poLine.RcptQtyMin = Decimal.Zero;
			}

			if (poLine.LineType == POLineType.Description)
			{
				poLine.SiteID = null;
				poLine.ExpenseAcctID = null;
				poLine.ExpenseSubID = null;
			}

			if (!IsExpenseAccountEnabled(poLine))
			{
				poLine.ExpenseAcctID = null;
				poLine.ExpenseSubID = null;
			}
			if (!IsAccrualAccountEnabled(poLine))
			{
				poLine.POAccrualAcctID = null;
				poLine.POAccrualSubID = null;
			}
		}

		public bool IsLinkedToSO(POLine row)
		{
			return (row.LineType == POLineType.GoodsForSalesOrder || row.LineType == POLineType.GoodsForDropShip ||
					row.LineType == POLineType.NonStockForSalesOrder || row.LineType == POLineType.NonStockForDropShip);
		}

		protected virtual void _(Events.FieldUpdating<POOrder, POOrder.approved> e)
		{
			if (e.Row != null
				&& !true.Equals(e.OldValue)
				&& (e.NewValue is true
					|| (e.NewValue != null
						&& bool.TryParse(e.NewValue.ToString(), out bool newValue)
						&& newValue is true)))
			{
				if (GetRequireControlTotal(e.Row.OrderType)
					&& e.Row.CuryOrderTotal != e.Row.CuryControlTotal)
				{
					e.Cancel = true;
					e.NewValue = false;
				}
			}
		}

		protected virtual void _(Events.FieldUpdating<POOrder, POOrder.rejected> e)
		{
			if (e.Row != null
				&& !true.Equals(e.OldValue)
				&& (e.NewValue is true
					|| (e.NewValue != null
						&& bool.TryParse(e.NewValue.ToString(), out bool newValue)
						&& newValue is true)))
			{
				if (GetRequireControlTotal(e.Row.OrderType)
					&& e.Row.CuryOrderTotal != e.Row.CuryControlTotal)
				{
					e.Cancel = true;
					e.NewValue = false;
				}
			}
		}

		#endregion

		private bool BlanketPOLineIsReferencedInPOOrder(POLine currentLine, out POLine relatedPOLine)
		{
			relatedPOLine =
					SelectFrom<POLine>.
					Where<POLine.pONbr.IsEqual<POLine.orderNbr.FromCurrent>.
						And<POLine.pOType.IsEqual<POLine.orderType.FromCurrent>>.
						And<POLine.pOLineNbr.IsEqual<POLine.lineNbr.FromCurrent>>>
					.View.SelectSingleBound(this, new object[] { currentLine });
			return relatedPOLine != null;
		}

		#region Internal Member Definitions
		[Serializable]
		public partial class POOrderFilter : PXBqlTable, IBqlTable
		{
			#region VendorID
			public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }

			protected Int32? _VendorID;
			[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true)]
			[PXDefault(typeof(POOrder.vendorID))]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			#region OrderType
			public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
			protected String _OrderType;
			[PXDBString(2, IsFixed = true)]
			[PXDefault(POOrderType.Blanket)]
			[POOrderType.BlanketList()]
			[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, InputMask = "")]
			[PXDefault()]
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PO.RefNbr(
				typeof(Search2<POOrderS.orderNbr,
				CrossJoin<APSetup,
				InnerJoin<Vendor, On<POOrderS.vendorID, Equal<Vendor.bAccountID>>>>,
				 Where<POOrderS.orderType, Equal<Current<POOrderEntry.POOrderFilter.orderType>>,
				And<POOrderS.vendorID, Equal<Current<POOrder.vendorID>>,
				And<POOrderS.vendorLocationID, Equal<Current<POOrder.vendorLocationID>>,
				And<POOrderS.curyID, Equal<Current<POOrder.curyID>>,
				And<POOrderS.hold, Equal<boolFalse>,
				And<POOrderS.cancelled, Equal<boolFalse>,
				And<POOrderS.approved, Equal<boolTrue>,
				And2<Where<POOrderS.orderQty, Equal<decimal0>, Or<POOrderS.openOrderQty, Greater<decimal0>>>,
						And2<Where<POOrderS.orderType, Equal<POOrderType.blanket>,
							Or<POOrderS.orderType, Equal<POOrderType.standardBlanket>>>,
						And2<Where<POOrderS.payToVendorID, Equal<Current<POOrder.payToVendorID>>,
							Or<Not<FeatureInstalled<FeaturesSet.vendorRelations>>>>,
				And<Where<APSetup.requireSingleProjectPerDocument, Equal<boolFalse>,
					Or<POOrderS.projectID, Equal<Current<POOrder.projectID>>>>>>>>>>>>>>>>),
				Filterable = true)]
			public virtual String OrderNbr
			{
				get
				{
					return this._OrderNbr;
				}
				set
				{
					this._OrderNbr = value;
				}
			}
			#endregion
		}

		[PXProjection(typeof(Select<POLine>), Persistent = false)]
		[PXCacheName(Messages.POLineS)]
		[Serializable]
		public partial class POLineS : PXBqlTable, IBqlTable, ISortOrder
		{
			#region Selected
			public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
			protected bool? _Selected = false;
			[PXBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Selected")]
			public virtual bool? Selected
			{
				get
				{
					return _Selected;
				}
				set
				{
					_Selected = value;
				}
			}
			#endregion
			#region BranchID
			public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
			protected Int32? _BranchID;
			[Branch(BqlField = typeof(POLine.branchID))]
			public virtual Int32? BranchID
			{
				get
				{
					return this._BranchID;
				}
				set
				{
					this._BranchID = value;
				}
			}
			#endregion
			#region OrderType
			public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POLine.orderType))]
			[PXDBDefault(typeof(POOrder.orderType))]
			[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.Visible, Visible = false)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
			protected String _OrderNbr;

			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POLine.orderNbr))]
			[PXDBDefault(typeof(POOrder.orderNbr))]
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
			public virtual String OrderNbr
			{
				get
				{
					return this._OrderNbr;
				}
				set
				{
					this._OrderNbr = value;
				}
			}
			#endregion
			#region LineNbr
			public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(POLine.lineNbr))]
			[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
			[PXLineNbr(typeof(POOrder.lineCntr))]
			public virtual Int32? LineNbr
			{
				get
				{
					return this._LineNbr;
				}
				set
				{
					this._LineNbr = value;
				}
			}
			#endregion
			#region SortOrder
			public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }
			protected Int32? _SortOrder;
			[PXDBInt(BqlField = typeof(POLine.sortOrder))]
			public virtual Int32? SortOrder
			{
				get
				{
					return this._SortOrder;
				}
				set
				{
					this._SortOrder = value;
				}
			}
			#endregion
			#region LineType
			public abstract class lineType : PX.Data.BQL.BqlString.Field<lineType> { }
			protected String _LineType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(POLine.lineType))]
			[PXDefault(POLineType.GoodsForInventory)]
			[POLineType.List()]
			[PXUIField(DisplayName = "Line Type")]
			public virtual String LineType
			{
				get
				{
					return this._LineType;
				}
				set
				{
					this._LineType = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
			protected Int32? _InventoryID;
			[POLineInventoryItem(Filterable = true, BqlField = typeof(POLine.inventoryID))]
			public virtual Int32? InventoryID
			{
				get
				{
					return this._InventoryID;
				}
				set
				{
					this._InventoryID = value;
				}
			}
			#endregion
			#region SubItemID
			public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
			protected Int32? _SubItemID;
			[SubItem(typeof(POLineS.inventoryID), BqlField = typeof(POLine.subItemID))]
			public virtual Int32? SubItemID
			{
				get
				{
					return this._SubItemID;
				}
				set
				{
					this._SubItemID = value;
				}
			}
			#endregion
			#region SiteID
			public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
			protected Int32? _SiteID;
			[SiteAvail(typeof(POLineS.inventoryID), typeof(POLineS.subItemID), typeof(POLineS.costCenterID), BqlField = typeof(POLine.siteID))]
			public virtual Int32? SiteID
			{
				get
				{
					return this._SiteID;
				}
				set
				{
					this._SiteID = value;
				}
			}
			#endregion

			#region UOM
			public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
			protected String _UOM;


			[INUnit(typeof(POLineS.inventoryID), DisplayName = "UOM", BqlField = typeof(POLine.uOM))]
			public virtual String UOM
			{
				get
				{
					return this._UOM;
				}
				set
				{
					this._UOM = value;
				}
			}
			#endregion
			#region OrderQty
			public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
			protected Decimal? _OrderQty;
			[PXDBQuantity(typeof(POLineS.uOM), typeof(POLineS.baseOrderQty), HandleEmptyKey = true, BqlField = typeof(POLine.orderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXFormula(null, typeof(SumCalc<POOrder.orderQty>))]
			[PXUIField(DisplayName = "Order Qty.", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? OrderQty
			{
				get
				{
					return this._OrderQty;
				}
				set
				{
					this._OrderQty = value;
				}
			}
			#endregion
			#region BaseOrderQty
			public abstract class baseOrderQty : PX.Data.BQL.BqlDecimal.Field<baseOrderQty> { }
			protected Decimal? _BaseOrderQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseOrderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? BaseOrderQty
			{
				get
				{
					return this._BaseOrderQty;
				}
				set
				{
					this._BaseOrderQty = value;
				}
			}
			#endregion
			#region ReceivedQty
			public abstract class receivedQty : PX.Data.BQL.BqlDecimal.Field<receivedQty> { }
			protected Decimal? _ReceivedQty;
			[PXDBQuantity(typeof(POLineS.uOM), typeof(POLineS.baseReceivedQty), HandleEmptyKey = true, BqlField = typeof(POLine.receivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Received Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? ReceivedQty
			{
				get
				{
					return this._ReceivedQty;
				}
				set
				{
					this._ReceivedQty = value;
				}
			}
			#endregion
			#region BaseReceivedQty
			public abstract class baseReceivedQty : PX.Data.BQL.BqlDecimal.Field<baseReceivedQty> { }
			protected Decimal? _BaseReceivedQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseReceivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Base Received Qty.", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? BaseReceivedQty
			{
				get
				{
					return this._BaseReceivedQty;
				}
				set
				{
					this._BaseReceivedQty = value;
				}
			}
			#endregion
			#region OpenQty
			[PXDBQuantity(BqlField = typeof(POLine.openQty))]
			[PXUIField(DisplayName = "Open Qty.", Enabled = false)]
			public virtual decimal? OpenQty { get; set; }
			public abstract class openQty : BqlDecimal.Field<openQty> { }
			#endregion
			#region CuryInfoID
			public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
			protected Int64? _CuryInfoID;
			[PXDBLong(BqlField = typeof(POLine.curyInfoID))]
			public virtual Int64? CuryInfoID
			{
				get
				{
					return this._CuryInfoID;
				}
				set
				{
					this._CuryInfoID = value;
				}
			}
			#endregion
			#region CuryUnitCost
			public abstract class curyUnitCost : PX.Data.BQL.BqlDecimal.Field<curyUnitCost> { }
			protected Decimal? _CuryUnitCost;

			[PXDBCurrency(typeof(POLineS.curyInfoID), typeof(POLineS.unitCost), BqlField = typeof(POLine.curyUnitCost))]
			[PXUIField(DisplayName = "Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? CuryUnitCost
			{
				get
				{
					return this._CuryUnitCost;
				}
				set
				{
					this._CuryUnitCost = value;
				}
			}
			#endregion
			#region UnitCost
			public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
			protected Decimal? _UnitCost;

			[PXDBPriceCost(BqlField = typeof(POLine.unitCost))]
			public virtual Decimal? UnitCost
			{
				get
				{
					return this._UnitCost;
				}
				set
				{
					this._UnitCost = value;
				}
			}
			#endregion
			#region ManualPrice
			public abstract class manualPrice : PX.Data.BQL.BqlBool.Field<manualPrice> { }
			protected Boolean? _ManualPrice;
			[PXDBBool(BqlField = typeof(POLine.manualPrice))]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Manual Price", Visibility = PXUIVisibility.Visible)]
			public virtual Boolean? ManualPrice
			{
				get
				{
					return this._ManualPrice;
				}
				set
				{
					this._ManualPrice = value;
				}
			}
			#endregion
			#region CuryLineAmt
			[PXDBCurrency(typeof(POLine.curyInfoID), typeof(POLine.lineAmt), BqlField = typeof(POLine.curyLineAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Ext. Cost")]
			public virtual decimal? CuryLineAmt { get; set; }
			public abstract class curyLineAmt : BqlDecimal.Field<curyLineAmt> { }
			#endregion
			#region LineAmt
			public abstract class lineAmt : BqlDecimal.Field<lineAmt> { }
			[PXDBDecimal(4, BqlField = typeof(POLine.lineAmt))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual decimal? LineAmt { get; set; }
			#endregion
			#region TaxCategoryID
			public abstract class taxCategoryID : PX.Data.BQL.BqlString.Field<taxCategoryID> { }
			protected String _TaxCategoryID;
			[PXDBString(TaxCategory.taxCategoryID.Length, IsUnicode = true, BqlField = typeof(POLine.taxCategoryID))]
			[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
			//[POTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
			//[POOpenTax(typeof(POOrder), typeof(POTax), typeof(POTaxTran))]
			[PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
			[PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
			public virtual String TaxCategoryID
			{
				get
				{
					return this._TaxCategoryID;
				}
				set
				{
					this._TaxCategoryID = value;
				}
			}
			#endregion
			#region ExpenseAcctID
			public abstract class expenseAcctID : PX.Data.BQL.BqlInt.Field<expenseAcctID> { }
			protected Int32? _ExpenseAcctID;
			[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, Filterable = false, BqlField = typeof(POLine.expenseAcctID))]
			public virtual Int32? ExpenseAcctID
			{
				get
				{
					return this._ExpenseAcctID;
				}
				set
				{
					this._ExpenseAcctID = value;
				}
			}
			#endregion
			#region ExpenseSubID
			public abstract class expenseSubID : PX.Data.BQL.BqlInt.Field<expenseSubID> { }
			protected Int32? _ExpenseSubID;

			[SubAccount(typeof(POLineS.expenseAcctID), DisplayName = "Sub.", Visibility = PXUIVisibility.Visible, Filterable = true, BqlField = typeof(POLine.expenseSubID))]
			public virtual Int32? ExpenseSubID
			{
				get
				{
					return this._ExpenseSubID;
				}
				set
				{
					this._ExpenseSubID = value;
				}
			}
			#endregion
			#region AlternateID
			public abstract class alternateID : PX.Data.BQL.BqlString.Field<alternateID> { }
			protected String _AlternateID;
			[PXDBString(50, IsUnicode = true, BqlField = typeof(POLine.alternateID), InputMask = "")]
			[PXUIField(DisplayName = "Alternate ID")]
			public virtual String AlternateID
			{
				get
				{
					return this._AlternateID;
				}
				set
				{
					this._AlternateID = value;
				}
			}
			#endregion
			#region TranDesc
			public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }
			protected String _TranDesc;
			[PXDBString(256, IsUnicode = true, BqlField = typeof(POLine.tranDesc))]
			[PXUIField(DisplayName = "Line Description", Visibility = PXUIVisibility.Visible)]
			public virtual String TranDesc
			{
				get
				{
					return this._TranDesc;
				}
				set
				{
					this._TranDesc = value;
				}
			}
			#endregion
			#region UnitWeight
			public abstract class unitWeight : PX.Data.BQL.BqlDecimal.Field<unitWeight> { }
			protected Decimal? _UnitWeight;
			[PXDBDecimal(6, BqlField = typeof(POLine.unitWeight))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unit Weight")]
			public virtual Decimal? UnitWeight
			{
				get
				{
					return this._UnitWeight;
				}
				set
				{
					this._UnitWeight = value;
				}
			}
			#endregion
			#region UnitVolume
			public abstract class unitVolume : PX.Data.BQL.BqlDecimal.Field<unitVolume> { }
			protected Decimal? _UnitVolume;
			[PXDBDecimal(6, BqlField = typeof(POLine.unitVolume))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Unit Volume")]
			public virtual Decimal? UnitVolume
			{
				get
				{
					return this._UnitVolume;
				}
				set
				{
					this._UnitVolume = value;
				}
			}
			#endregion
			#region ProjectID
			public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
			protected Int32? _ProjectID;
			[POProjectDefault(typeof(POLine.lineType))]
			[PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>), PM.Messages.InactiveContract, typeof(PMProject.contractCD))]
			[PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
			[ProjectBaseAttribute(BqlField = typeof(POLine.projectID))]
			public virtual Int32? ProjectID
			{
				get
				{
					return this._ProjectID;
				}
				set
				{
					this._ProjectID = value;
				}
			}
			#endregion
			#region TaskID
			public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
			protected Int32? _TaskID;
			[ActiveProjectTask(typeof(POLine.projectID), BatchModule.PO, DisplayName = "Project Task", BqlField = typeof(POLine.taskID))]
			public virtual Int32? TaskID
			{
				get
				{
					return this._TaskID;
				}
				set
				{
					this._TaskID = value;
				}
			}
			#endregion
            #region CostCodeID
            public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }
            protected Int32? _CostCodeID;
            [CostCode(typeof(expenseAcctID), typeof(taskID), DisplayName = "Cost Code", BqlField = typeof(POLine.costCodeID))]
            public virtual Int32? CostCodeID
            {
                get;
                set;
            }
            #endregion
			#region RcptQtyMin
			public abstract class rcptQtyMin : PX.Data.BQL.BqlDecimal.Field<rcptQtyMin> { }
			protected Decimal? _RcptQtyMin;
			[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(POLine.rcptQtyMin))]
			[PXDefault(typeof(Search<Location.vRcptQtyMin,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Min. Receipt (%)")]
			public virtual Decimal? RcptQtyMin
			{
				get
				{
					return this._RcptQtyMin;
				}
				set
				{
					this._RcptQtyMin = value;
				}
			}
			#endregion
			#region RcptQtyMax
			public abstract class rcptQtyMax : PX.Data.BQL.BqlDecimal.Field<rcptQtyMax> { }
			protected Decimal? _RcptQtyMax;
			[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(POLine.rcptQtyMax))]
			[PXDefault(typeof(Search<Location.vRcptQtyMax,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Max. Receipt (%)")]
			public virtual Decimal? RcptQtyMax
			{
				get
				{
					return this._RcptQtyMax;
				}
				set
				{
					this._RcptQtyMax = value;
				}
			}
			#endregion
			#region RcptQtyThreshold
			public abstract class rcptQtyThreshold : PX.Data.BQL.BqlDecimal.Field<rcptQtyThreshold> { }
			protected Decimal? _RcptQtyThreshold;
			[PXDBDecimal(2, MinValue = 0.0, MaxValue = 999.0, BqlField = typeof(POLine.rcptQtyThreshold))]
			[PXDefault(typeof(Search<Location.vRcptQtyThreshold,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Complete On (%)")]
			public virtual Decimal? RcptQtyThreshold
			{
				get
				{
					return this._RcptQtyThreshold;
				}
				set
				{
					this._RcptQtyThreshold = value;
				}
			}
			#endregion
			#region RcptQtyAction
			public abstract class rcptQtyAction : PX.Data.BQL.BqlString.Field<rcptQtyAction> { }
			protected String _RcptQtyAction;
			[PXDBString(1, IsFixed = true, BqlField = typeof(POLine.rcptQtyAction))]
			[POReceiptQtyAction.List()]
			[PXDefault(typeof(Search<Location.vRcptQtyAction,
				Where<Location.locationID, Equal<Current<POOrder.vendorLocationID>>,
					And<Location.bAccountID, Equal<Current<POOrder.vendorID>>>>>))]
			[PXUIField(DisplayName = "Receipt Action")]
			public virtual String RcptQtyAction
			{
				get
				{
					return this._RcptQtyAction;
				}
				set
				{
					this._RcptQtyAction = value;
				}
			}
			#endregion
			#region Cancelled
			public abstract class cancelled : PX.Data.BQL.BqlBool.Field<cancelled> { }
			protected Boolean? _Cancelled;
			[PXDBBool(BqlField = typeof(POLine.cancelled))]
			[PXUIField(DisplayName = "Canceled", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public virtual Boolean? Cancelled
			{
				get
				{
					return this._Cancelled;
				}
				set
				{
					this._Cancelled = value;
				}
			}
			#endregion
			#region Completed
			public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }
			protected Boolean? _Completed;
			[PXDBBool(BqlField = typeof(POLine.completed))]
			[PXUIField(DisplayName = "Completed", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public virtual Boolean? Completed
			{
				get
				{
					return this._Completed;
				}
				set
				{
					this._Completed = value;
				}
			}
			#endregion

			#region RetainagePct
			public abstract class retainagePct : PX.Data.BQL.BqlDecimal.Field<retainagePct> { }
			[PXDBDecimal(6, MinValue = 0, MaxValue = 100, BqlField = typeof(POLine.retainagePct))]
			[PXUIField(DisplayName = "Retainage Percent", FieldClass = nameof(FeaturesSet.Retainage))]
			public virtual decimal? RetainagePct
			{
				get;
				set;
			}
			#endregion
			#region CuryRetainageAmt
			public abstract class curyRetainageAmt : PX.Data.BQL.BqlDecimal.Field<curyRetainageAmt> { }
			[PXDBCurrency(typeof(POLineS.curyInfoID), typeof(POLineS.retainageAmt), BqlField = typeof(POLine.curyRetainageAmt))]
			[PXUIField(DisplayName = "Retainage Amount", FieldClass = nameof(FeaturesSet.Retainage))]
			public virtual decimal? CuryRetainageAmt
			{
				get;
				set;
			}
			#endregion
			#region RetainageAmt
			public abstract class retainageAmt : PX.Data.BQL.BqlDecimal.Field<retainageAmt> { }
			[PXDBBaseCury(BqlField = typeof(POLine.retainageAmt))]
			public virtual decimal? RetainageAmt
			{
				get;
				set;
			}
			#endregion
			#region CostCenterID
			public abstract class costCenterID : PX.Data.BQL.BqlInt.Field<costCenterID> { }
			[PXDBInt(BqlField = typeof(POLine.costCenterID))]
			public virtual Int32? CostCenterID
			{
				get;
				set;
			}
			#endregion
		}

		[PXProjection(typeof(Select5<POOrder,
										InnerJoin<POLine, On<POLine.orderType, Equal<POOrder.orderType>,
											And<POLine.orderNbr, Equal<POOrder.orderNbr>,
											And<POLine.cancelled, NotEqual<boolTrue>,
											And<POLine.completed, NotEqual<boolTrue>,
											And2<Where<POLine.orderQty, Equal<decimal0>, Or<POLine.openQty, Greater<decimal0>>>,
											And<Where<POOrder.orderType, Equal<POOrderType.standardBlanket>,
														 Or<POLine.lineType, NotEqual<POLineType.description>>>>>>>>>>,
											Where<POOrder.orderType, Equal<POOrderType.blanket>,
												 Or<POOrder.orderType, Equal<POOrderType.standardBlanket>>>,
											Aggregate
												<GroupBy<POOrder.orderType,
												GroupBy<POOrder.orderNbr,
												GroupBy<POOrder.orderDate,
												GroupBy<POOrder.curyID,
												GroupBy<POOrder.curyOrderTotal,
												GroupBy<POOrder.hold,
												GroupBy<POOrder.cancelled,
												GroupBy<POOrder.approved,
												GroupBy<POOrder.isTaxValid,
												GroupBy<POOrder.isUnbilledTaxValid,
												Max<POOrder.openOrderQty,
												Sum<POLine.orderQty,
												Sum<POLine.baseOrderQty,
												Sum<POLine.receivedQty,
												Sum<POLine.baseReceivedQty,
												Sum<POLine.curyExtCost,
												Sum<POLine.extCost,
												Sum<POLine.curyBLOrderedCost,
												Sum<POLine.bLOrderedCost>>>>>>>>>>>>>>>>>>>>>), Persistent = false)]
		[Serializable]
		public partial class POOrderS : PXBqlTable, IBqlTable
		{
			#region Selected
			public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
			protected bool? _Selected = false;
			[PXBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Selected")]
			public virtual bool? Selected
			{
				get
				{
					return _Selected;
				}
				set
				{
					_Selected = value;
				}
			}
			#endregion

			#region ReceivedQty
			public abstract class receivedQty : PX.Data.BQL.BqlDecimal.Field<receivedQty> { }
			protected Decimal? _ReceivedQty;
			[PXDBQuantity(HandleEmptyKey = true, BqlField = typeof(POLine.receivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Received Qty.", Visibility = PXUIVisibility.Visible, Enabled = false)]
			public virtual Decimal? ReceivedQty
			{
				get
				{
					return this._ReceivedQty;
				}
				set
				{
					this._ReceivedQty = value;
				}
			}
			#endregion
			#region BaseReceivedQty
			public abstract class baseReceivedQty : PX.Data.BQL.BqlDecimal.Field<baseReceivedQty> { }
			protected Decimal? _BaseReceivedQty;
			[PXDBDecimal(6, BqlField = typeof(POLine.baseReceivedQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Base Received Qty.", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? BaseReceivedQty
			{
				get
				{
					return this._BaseReceivedQty;
				}
				set
				{
					this._BaseReceivedQty = value;
				}
			}
			#endregion
			#region LineTotalQty
			public abstract class lineTotalQty : PX.Data.BQL.BqlDecimal.Field<lineTotalQty> { }
			[PXDBQuantity(HandleEmptyKey = true, BqlField = typeof(POLine.orderQty))]
			public virtual decimal? LineTotalQty
			{
				get;
				set;
			}
			#endregion
			#region BaseLineTotalQty
			public abstract class baseLineTotalQty : PX.Data.BQL.BqlDecimal.Field<baseLineTotalQty> { }
			[PXDBDecimal(6, BqlField = typeof(POLine.baseOrderQty))]
			public virtual decimal? BaseLineTotalQty
			{
				get;
				set;
			}
			#endregion
			#region CuryLineTotalCost
			public abstract class curyLineTotalCost : PX.Data.BQL.BqlDecimal.Field<curyLineTotalCost> { }
			[PXDBDecimal(6, BqlField = typeof(POLine.curyExtCost))]
			public virtual decimal? CuryLineTotalCost
			{
				get;
				set;
			}
			#endregion
			#region LineTotalCost
			public abstract class lineTotalCost : PX.Data.BQL.BqlDecimal.Field<lineTotalCost> { }
			[PXDBDecimal(6, BqlField = typeof(POLine.extCost))]
			public virtual decimal? LineTotalCost
			{
				get;
				set;
			}
			#endregion
			#region CuryBLOrderedCost
			public abstract class curyBLOrderedCost : PX.Data.BQL.BqlDecimal.Field<curyBLOrderedCost> { }
			[PXDBDecimal(6, BqlField = typeof(POLine.curyBLOrderedCost))]
			public virtual decimal? CuryBLOrderedCost
			{
				get;
				set;
			}
			#endregion
			#region BLOrderedCost
			public abstract class bLOrderedCost : PX.Data.BQL.BqlDecimal.Field<bLOrderedCost> { }
			[PXDBDecimal(6, BqlField = typeof(POLine.bLOrderedCost))]
			public virtual decimal? BLOrderedCost
			{
				get;
				set;
			}
			#endregion

			#region CuryLeftToReceiveCost
			public abstract class curyLeftToReceiveCost : PX.Data.BQL.BqlDecimal.Field<curyLeftToReceiveCost> { }
			[PXCurrency(typeof(POOrderS.curyInfoID), typeof(POOrderS.leftToReceiveCost))]
			[PXUIField(DisplayName = "Open Amt.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual decimal? CuryLeftToReceiveCost
			{
				[PXDependsOnFields(typeof(curyLineTotalCost), typeof(curyBLOrderedCost))]
				get
				{
					return this.CuryLineTotalCost - this.CuryBLOrderedCost;
				}
			}
			#endregion
			#region LeftToReceiveCost
			public abstract class leftToReceiveCost : PX.Data.BQL.BqlDecimal.Field<leftToReceiveCost> { }
			[PXBaseCury]
			public virtual decimal? LeftToReceiveCost
			{
				[PXDependsOnFields(typeof(lineTotalCost), typeof(bLOrderedCost))]
				get
				{
					return this.LineTotalCost - this.BLOrderedCost;
				}
			}
			#endregion

			#region OrderType
			public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POOrder.orderType))]
			[POOrderType.List()]
			[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POOrder.orderNbr))]
			[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			[PO.Numbering()]
			[PO.RefNbr(typeof(Search2<POOrderS.orderNbr,
				InnerJoinSingleTable<Vendor, On<POOrderS.vendorID, Equal<Vendor.bAccountID>>>,
				Where<POOrderS.orderType, Equal<Optional<POOrderS.orderType>>,
				And<Match<Vendor, Current<AccessInfo.userName>>>>>), Filterable = true)]
			public virtual String OrderNbr
			{
				get
				{
					return this._OrderNbr;
				}
				set
				{
					this._OrderNbr = value;
				}
			}
			#endregion
			#region VendorID
			public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
			protected Int32? _VendorID;
			[VendorActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, BqlField = typeof(POOrder.vendorID))]
			[PXDefault()]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			#region VendorLocationID
			public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
			protected Int32? _VendorLocationID;

			[LocationID(typeof(Where<Location.bAccountID, Equal<Current<POOrderS.vendorID>>>), DescriptionField = typeof(Location.descr), Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(POOrder.vendorLocationID))]
			public virtual Int32? VendorLocationID
			{
				get
				{
					return this._VendorLocationID;
				}
				set
				{
					this._VendorLocationID = value;
				}
			}
			#endregion
			#region OrderDate
			public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<orderDate> { }
			protected DateTime? _OrderDate;

			[PXDBDate(BqlField = typeof(POOrder.orderDate))]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual DateTime? OrderDate
			{
				get
				{
					return this._OrderDate;
				}
				set
				{
					this._OrderDate = value;
				}
			}
			#endregion
			#region ExpectedDate
			public abstract class expectedDate : PX.Data.BQL.BqlDateTime.Field<expectedDate> { }
			protected DateTime? _ExpectedDate;

			[PXDBDate(BqlField = typeof(POOrder.expectedDate))]
			[PXDefault(typeof(POOrderS.orderDate), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Promised On")]
			public virtual DateTime? ExpectedDate
			{
				get
				{
					return this._ExpectedDate;
				}
				set
				{
					this._ExpectedDate = value;
				}
			}
			#endregion
			#region ExpectedDate
			public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
			protected DateTime? _ExpirationDate;

			[PXDBDate(BqlField = typeof(POOrder.expirationDate))]
			[PXUIField(DisplayName = "Expired On")]
			public virtual DateTime? ExpirationDate
			{
				get
				{
					return this._ExpirationDate;
				}
				set
				{
					this._ExpirationDate = value;
				}
			}
			#endregion
			#region Status
			public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
			protected String _Status;
			[PXDBString(1, IsFixed = true, BqlField = typeof(POOrder.status))]
			[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			[POOrderStatus.List()]
			public virtual String Status
			{
				get
				{
					return this._Status;
				}
				set
				{
					this._Status = value;
				}
			}
			#endregion
			#region Hold
			public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
			protected Boolean? _Hold;
			[PXDBBool(BqlField = typeof(POOrder.hold))]
			[PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
			[PXDefault(true)]
			public virtual Boolean? Hold
			{
				get
				{
					return this._Hold;
				}
				set
				{
					this._Hold = value;
				}
			}
			#endregion
			#region Approved
			public abstract class approved : PX.Data.BQL.BqlBool.Field<approved> { }
			protected Boolean? _Approved;
			[PXDBBool(BqlField = typeof(POOrder.approved))]
			[PXUIField(DisplayName = "Approved", Visibility = PXUIVisibility.Visible)]
			[PXDefault(true)]
			public virtual Boolean? Approved
			{
				get
				{
					return this._Approved;
				}
				set
				{
					this._Approved = value;
				}
			}
			#endregion
			#region Cancelled
			public abstract class cancelled : PX.Data.BQL.BqlBool.Field<cancelled> { }
			protected Boolean? _Cancelled;
			[PXDBBool(BqlField = typeof(POOrder.cancelled))]
			[PXUIField(DisplayName = "Cancel", Visibility = PXUIVisibility.Visible)]
			[PXDefault(false)]
			public virtual Boolean? Cancelled
			{
				get
				{
					return this._Cancelled;
				}
				set
				{
					this._Cancelled = value;
				}
			}
			#endregion
			#region IsTaxValid
			public abstract class isTaxValid : PX.Data.BQL.BqlBool.Field<isTaxValid> { }
			[PXDBBool(BqlField = typeof(POOrder.isTaxValid))]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Tax Is Up to Date", Enabled = false)]
			public virtual Boolean? IsTaxValid
			{
				get;
				set;
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(POOrder.curyID))]
			[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]

			[PXSelector(typeof(Currency.curyID))]
			public virtual String CuryID
			{
				get
				{
					return this._CuryID;
				}
				set
				{
					this._CuryID = value;
				}
			}
			#endregion
			#region CuryInfoID
			public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
			protected Int64? _CuryInfoID;
			[PXDBLong(BqlField = typeof(POOrder.curyInfoID))]
			public virtual Int64? CuryInfoID
			{
				get
				{
					return this._CuryInfoID;
				}
				set
				{
					this._CuryInfoID = value;
				}
			}
			#endregion
			#region VendorRefNbr
			public abstract class vendorRefNbr : PX.Data.BQL.BqlString.Field<vendorRefNbr> { }
			protected String _VendorRefNbr;
			[PXDBString(40, IsUnicode = true, BqlField = typeof(POOrder.vendorRefNbr))]
			[PXUIField(DisplayName = "Vendor Ref.", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String VendorRefNbr
			{
				get
				{
					return this._VendorRefNbr;
				}
				set
				{
					this._VendorRefNbr = value;
				}
			}
			#endregion
			#region CuryOrderTotal
			public abstract class curyOrderTotal : PX.Data.BQL.BqlDecimal.Field<curyOrderTotal> { }
			protected Decimal? _CuryOrderTotal;

			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(POOrderS.curyInfoID), typeof(POOrderS.orderTotal), BqlField = typeof(POOrder.curyOrderTotal))]
			[PXUIField(DisplayName = "Order Total", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual Decimal? CuryOrderTotal
			{
				get
				{
					return this._CuryOrderTotal;
				}
				set
				{
					this._CuryOrderTotal = value;
				}
			}
			#endregion
			#region OrderTotal
			public abstract class orderTotal : PX.Data.BQL.BqlDecimal.Field<orderTotal> { }
			protected Decimal? _OrderTotal;
			[PXDBBaseCury(BqlField = typeof(POOrder.orderTotal))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? OrderTotal
			{
				get
				{
					return this._OrderTotal;
				}
				set
				{
					this._OrderTotal = value;
				}
			}
			#endregion
			#region OpenOrderQty
			[PXDBQuantity(BqlField = typeof(POOrder.openOrderQty))]
			[PXUIField(DisplayName = "Open Qty.", Enabled = false)]
			public virtual decimal? OpenOrderQty { get; set; }
			public abstract class openOrderQty : BqlDecimal.Field<openOrderQty> { }
			#endregion
			#region OrderQty
			public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
			protected Decimal? _OrderQty;
			[PXDBQuantity(BqlField = typeof(POOrder.orderQty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? OrderQty
			{
				get
				{
					return this._OrderQty;
				}
				set
				{
					this._OrderQty = value;
				}
			}
			#endregion
			#region TermsID
			public abstract class termsID : PX.Data.BQL.BqlString.Field<termsID> { }
			protected String _TermsID;
			[PXDBString(10, IsUnicode = true, IsFixed = true, BqlField = typeof(POOrder.termsID))]
			[PXUIField(DisplayName = "Terms", Visibility = PXUIVisibility.Visible)]
			[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.vendor>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]

			public virtual String TermsID
			{
				get
				{
					return this._TermsID;
				}
				set
				{
					this._TermsID = value;
				}
			}
			#endregion
			#region OrderDesc
			public abstract class orderDesc : PX.Data.BQL.BqlString.Field<orderDesc> { }
			protected String _OrderDesc;
			[PXDBString(60, IsUnicode = true, BqlField = typeof(POOrder.orderDesc))]
			[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String OrderDesc
			{
				get
				{
					return this._OrderDesc;
				}
				set
				{
					this._OrderDesc = value;
				}
			}
			#endregion
			#region CuryLineTotal
			public abstract class curyLineTotal : PX.Data.BQL.BqlDecimal.Field<curyLineTotal> { }
			protected Decimal? _CuryLineTotal;
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(POOrderS.curyInfoID), typeof(POOrderS.lineTotal), BqlField = typeof(POOrder.curyLineTotal))]
			[PXUIField(DisplayName = "Line Total", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual Decimal? CuryLineTotal
			{
				get
				{
					return this._CuryLineTotal;
				}
				set
				{
					this._CuryLineTotal = value;
				}
			}
			#endregion
			#region LineTotal
			public abstract class lineTotal : PX.Data.BQL.BqlDecimal.Field<lineTotal> { }
			protected Decimal? _LineTotal;
			[PXDBBaseCury(BqlField = typeof(POOrder.lineTotal))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public virtual Decimal? LineTotal
			{
				get
				{
					return this._LineTotal;
				}
				set
				{
					this._LineTotal = value;
				}
			}
			#endregion
			#region PayToVendorID
			public abstract class payToVendorID : PX.Data.BQL.BqlInt.Field<payToVendorID> { }
			/// <summary>
			/// A reference to the <see cref="Vendor"/>.
			/// </summary>
			/// <value>
			/// An integer identifier of the vendor, whom the AP bill will belong to. 
			/// </value>
			[POOrderPayToVendor(CacheGlobal = true, Filterable = true, BqlField = typeof(POOrder.payToVendorID))]
			public virtual int? PayToVendorID { get; set; }
			#endregion
			#region ProjectID
			public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
			[ProjectBase(BqlField = typeof(POOrder.projectID))]
			public virtual int? ProjectID
			{
				get;
				set;
			}
			#endregion
		}

		[PXProjection(typeof(Select<POOrder>), Persistent = true)]
		[Serializable]
		public partial class POOrderR : PXBqlTable, IBqlTable
		{
			#region Keys
			public class PK : PrimaryKeyOf<POOrderR>.By<orderType, orderNbr>
			{
				public static POOrderR Find(PXGraph graph, string orderType, string orderNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, orderType, orderNbr, options);
			}
			#endregion
			#region OrderType
			public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(POOrder.orderType))]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
			protected String _OrderNbr;

			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(POOrder.orderNbr))]
			[PXDefault()]
			public virtual String OrderNbr
			{
				get
				{
					return this._OrderNbr;
				}
				set
				{
					this._OrderNbr = value;
				}
			}
			#endregion
			#region Status
			public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
			protected String _Status;
			[PXDBString(1, IsFixed = true, BqlField = typeof(POOrder.status))]
			public virtual String Status
			{
				get
				{
					return this._Status;
				}
				set
				{
					this._Status = value;
				}
			}
			#endregion
			#region Hold
			public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
			protected Boolean? _Hold;
			[PXDBBool(BqlField = typeof(POOrder.hold))]
			public virtual Boolean? Hold
			{
				get
				{
					return this._Hold;
				}
				set
				{
					this._Hold = value;
				}
			}
			#endregion
			#region LinesToCompleteCntr
			public abstract class linesToCompleteCntr : BqlInt.Field<linesToCompleteCntr> { }
			[PXDBInt(BqlField = typeof(POOrder.linesToCompleteCntr))]
			public virtual int? LinesToCompleteCntr
			{
				get;
				set;
			}
			#endregion
			#region LinesToCloseCntr
			public abstract class linesToCloseCntr : BqlInt.Field<linesToCloseCntr> { }
			[PXDBInt(BqlField = typeof(POOrder.linesToCloseCntr))]
			public virtual int? LinesToCloseCntr
			{
				get;
				set;
			}
			#endregion
			#region OpenOrderQty
			[PXDBQuantity(BqlField = typeof(POOrder.openOrderQty))]
			public virtual decimal? OpenOrderQty { get; set; }
			public abstract class openOrderQty : BqlDecimal.Field<openOrderQty> { }
			#endregion
		}

		/// <exclude/>
		[PXCacheName(SO.Messages.SOLineSplit)]
		[PXProjection(typeof(Select2<SOLineSplit,
					InnerJoin<SOLine, On<SOLine.orderType, Equal<SOLineSplit.orderType>, And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>, And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>>>), new Type[] { typeof(SOLineSplit) })]
		[Serializable]
		public partial class SOLineSplit3 : PXBqlTable, IBqlTable, ISortOrder
		{
			#region Keys
			public static class FK
			{
				public class OrderLine : SOLine5.PK.ForeignKeyOf<SOLineSplit3>.By<orderType, orderNbr, lineNbr> { }
				public class POLine : Objects.PO.POLine.PK.ForeignKeyOf<SOLineSplit3>.By<pOType, pONbr, pOLineNbr> { }
			}
			#endregion // Keys

			#region OrderType
			public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(SOLineSplit.orderType))]
			[PXDefault()]
			[PXUIField(DisplayName = "Order Type", Enabled = false)]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(SOLineSplit.orderNbr))]
			[PXDefault()]
			[PXUIField(DisplayName = "Order Nbr.", Enabled = false)]
			[PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Current<SOLineSplit3.orderType>>>>))]
			public virtual String OrderNbr
			{
				get
				{
					return this._OrderNbr;
				}
				set
				{
					this._OrderNbr = value;
				}
			}
			#endregion
			#region LineNbr
			public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(SOLineSplit.lineNbr))]
			[PXUIField(DisplayName = "Line Nbr.", Enabled = false)]
			public virtual Int32? LineNbr
			{
				get
				{
					return this._LineNbr;
				}
				set
				{
					this._LineNbr = value;
				}
			}
			#endregion
			#region SortOrder
			public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }
			protected Int32? _SortOrder;
			[PXDBInt(BqlField = typeof(SOLine.sortOrder))]
			public virtual Int32? SortOrder
			{
				get
				{
					return this._SortOrder;
				}
				set
				{
					this._SortOrder = value;
				}
			}
			#endregion
			#region SplitLineNbr
			public abstract class splitLineNbr : PX.Data.BQL.BqlInt.Field<splitLineNbr> { }
			protected Int32? _SplitLineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(SOLineSplit.splitLineNbr))]
			public virtual Int32? SplitLineNbr
			{
				get
				{
					return this._SplitLineNbr;
				}
				set
				{
					this._SplitLineNbr = value;
				}
			}
			#endregion
			#region Behavior
			public abstract class behavior : Data.BQL.BqlString.Field<behavior> { }
			[PXDBString(2, IsFixed = true, InputMask = ">aa", BqlField = typeof(SOLineSplit.behavior))]
			public virtual string Behavior
			{
				get;
				set;
			}
			#endregion
			#region Operation
			public abstract class operation : PX.Data.BQL.BqlString.Field<operation> { }
			protected String _Operation;
			[PXDBString(1, IsFixed = true, BqlField = typeof(SOLineSplit.operation))]
			public virtual String Operation
			{
				get
				{
					return this._Operation;
				}
				set
				{
					this._Operation = value;
				}
			}
			#endregion
			#region LineType
			public abstract class lineType : PX.Data.BQL.BqlString.Field<lineType> { }
			protected String _LineType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(SOLine.lineType))]
			[PXUIField(DisplayName = "Line Type", Enabled = false)]
			public virtual String LineType
			{
				get
				{
					return this._LineType;
				}
				set
				{
					this._LineType = value;
				}
			}
			#endregion
			#region VendorID
			public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
			protected Int32? _VendorID;
			[PXDBInt(BqlField = typeof(SOLineSplit.vendorID))]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			#region POCreate
			public abstract class pOCreate : PX.Data.BQL.BqlBool.Field<pOCreate> { }
			protected bool? _POCreate;
			[PXDBBool(BqlField = typeof(SOLineSplit.pOCreate))]
			public virtual bool? POCreate
			{
				get
				{
					return this._POCreate;
				}
				set
				{
					this._POCreate = value;
				}
			}
			#endregion
			#region POCreated
			public abstract class pOCreated : PX.Data.IBqlField
			{
			}
			protected bool? _POCreated;
			[PXDBBool(BqlField = typeof(SOLine.pOCreated))]
			public virtual bool? POCreated
			{
				get
				{
					return this._POCreated;
				}
				set
				{
					this._POCreated = value;
				}
			}
			#endregion
			#region POCompleted
			public abstract class pOCompleted : PX.Data.BQL.BqlBool.Field<pOCompleted> { }

			[PXDBBool(BqlField = typeof(SOLineSplit.pOCompleted))]
			public virtual Boolean? POCompleted
			{
				get;
				set;
			}
			#endregion
			#region POCancelled
			public abstract class pOCancelled : PX.Data.BQL.BqlBool.Field<pOCancelled> { }

			[PXDBBool(BqlField = typeof(SOLineSplit.pOCancelled))]
			public virtual Boolean? POCancelled
			{
				get;
				set;
			}
			#endregion
			#region POSource
			public abstract class pOSource : PX.Data.BQL.BqlString.Field<pOSource> { }

			[PXDBString(BqlField = typeof(SOLineSplit.pOSource))]
			public virtual string POSource
			{
				get;
				set;
			}
			#endregion
			#region POType
			public abstract class pOType : PX.Data.BQL.BqlString.Field<pOType> { }
			protected String _POType;
			[PXDBString(2, IsFixed = true, BqlField = typeof(SOLineSplit.pOType))]
			[PXUIField(DisplayName = "PO Type", Enabled = false)]
			public virtual String POType
			{
				get
				{
					return this._POType;
				}
				set
				{
					this._POType = value;
				}
			}
			#endregion
			#region PONbr
			public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr> { }
			protected String _PONbr;
			[PXDBString(15, IsUnicode = true, BqlField = typeof(SOLineSplit.pONbr))]
			[PXDBDefault(typeof(POOrder.orderNbr), PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "PO Nbr.", Enabled = false)]
			public virtual String PONbr
			{
				get
				{
					return this._PONbr;
				}
				set
				{
					this._PONbr = value;
				}
			}
			#endregion
			#region POLineNbr
			public abstract class pOLineNbr : PX.Data.BQL.BqlInt.Field<pOLineNbr> { }
			protected Int32? _POLineNbr;
			[PXDBInt(BqlField = typeof(SOLineSplit.pOLineNbr))]
			[PXUIField(DisplayName = "PO Line Nbr.", Enabled = false)]
			public virtual Int32? POLineNbr
			{
				get
				{
					return this._POLineNbr;
				}
				set
				{
					this._POLineNbr = value;
				}
			}
			#endregion
			#region RefNoteID
			public abstract class refNoteID : PX.Data.BQL.BqlGuid.Field<refNoteID> { }
			protected Guid? _RefNoteID;
			[PXDBGuid(BqlField = typeof(SOLineSplit.refNoteID))]
			public virtual Guid? RefNoteID
			{
				get
				{
					return this._RefNoteID;
				}
				set
				{
					this._RefNoteID = value;
				}
			}
			#endregion
			#region RequestDate
			public abstract class requestDate : PX.Data.BQL.BqlDateTime.Field<requestDate> { }
			protected DateTime? _RequestDate;
			[PXDBDate(BqlField = typeof(SOLine.requestDate))]
			[PXDefault()]
			[PXUIField(DisplayName = "Requested", Enabled = false)]
			public virtual DateTime? RequestDate
			{
				get
				{
					return this._RequestDate;
				}
				set
				{
					this._RequestDate = value;
				}
			}
			#endregion
			#region CustomerID
			public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
			protected Int32? _CustomerID;
			[AR.Customer(BqlField = typeof(SOLine.customerID), Enabled = false)]
			[PXDefault()]
			public virtual Int32? CustomerID
			{
				get
				{
					return this._CustomerID;
				}
				set
				{
					this._CustomerID = value;
				}
			}
			#endregion
			#region SiteID
			public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
			protected Int32? _SiteID;
			[SiteAvail(typeof(SOLineSplit.inventoryID), typeof(SOLineSplit.subItemID), typeof(CostCenter.freeStock), BqlField = typeof(SOLineSplit.siteID), DisplayName = "Warehouse")]
			public virtual Int32? SiteID
			{
				get
				{
					return this._SiteID;
				}
				set
				{
					this._SiteID = value;
				}
			}
			#endregion
			#region UOM
			public abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }
			protected String _UOM;
			[INUnit(typeof(SOLineSplit.inventoryID), DisplayName = "Orig. UOM", BqlField = typeof(SOLineSplit.uOM), Enabled = false)]
			[PXDefault()]
			public virtual String UOM
			{
				get
				{
					return this._UOM;
				}
				set
				{
					this._UOM = value;
				}
			}
			#endregion
			#region OrderQty
			public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
			protected Decimal? _OrderQty;
			[PXDBQuantity(BqlField = typeof(SOLineSplit.qty))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Orig. Quantity", Enabled = false)]
			public virtual Decimal? OrderQty
			{
				get
				{
					return this._OrderQty;
				}
				set
				{
					this._OrderQty = value;
				}
			}
			#endregion
			#region BaseOrderQty
			public abstract class baseOrderQty : PX.Data.BQL.BqlDecimal.Field<baseOrderQty> { }

			[PXDBDecimal(6, BqlField = typeof(SOLineSplit.baseQty))]
			public virtual Decimal? BaseOrderQty
			{
				get;
				set;
			}
			#endregion
			#region LinePOCreate
			/// <exclude />
			public abstract class linePOCreate : PX.Data.BQL.BqlBool.Field<linePOCreate> { }
			protected bool? _LinePOCreate;
			/// <exclude />
			[PXDBBool(BqlField = typeof(SOLine.pOCreate))]
			public virtual bool? LinePOCreate
			{
				get
				{
					return this._LinePOCreate;
				}
				set
				{
					this._LinePOCreate = value;
				}
			}
			#endregion
			#region IsValidForDropShip
			public abstract class isValidForDropShip : PX.Data.BQL.BqlBool.Field<isValidForDropShip> { }

			[PXBool]
			[PXDBCalced(typeof(Switch<Case<Where<SOLine.baseOpenQty, Equal<SOLine.baseOrderQty>,
				And<SOLine.baseOrderQty.Multiply<SOLine.lineSign>, Equal<SOLineSplit.baseQty>,
				And<SOLineSplit.isAllocated, NotEqual<True>>>>, True>, False>), typeof(bool))]
			public virtual bool? IsValidForDropShip
			{
				get;
				set;
			}
			#endregion

			#region POUOM
			public abstract class pOUOM : PX.Data.BQL.BqlString.Field<pOUOM> { }
			protected String _POUOM;
			[PXString(6, IsUnicode = true, InputMask = ">aaaaaa")]
			[PXUIField(DisplayName = "UOM", Enabled = false)]
			public virtual String POUOM
			{
				get
				{
					return this._POUOM;
				}
				set
				{
					this._POUOM = value;
				}
			}
			#endregion
			#region POUOMOrderQty
			public abstract class pOUOMOrderQty : PX.Data.BQL.BqlDecimal.Field<pOUOMOrderQty> { }
			protected Decimal? _POUOMOrderQty;
			[PXQuantity]
			[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
			[PXUIField(DisplayName = "Quantity", Enabled = false)]
			public virtual Decimal? POUOMOrderQty
			{
				get
				{
					return this._POUOMOrderQty;
				}
				set
				{
					this._POUOMOrderQty = value;
				}
			}
			#endregion
			#region PlanID
			public abstract class planID : PX.Data.BQL.BqlLong.Field<planID> { }
			protected Int64? _PlanID;
			[PXDBLong(BqlField = typeof(SOLineSplit.planID))]
			public virtual Int64? PlanID
			{
				get
				{
					return this._PlanID;
				}
				set
				{
					this._PlanID = value;
				}
			}
			#endregion
			#region IsStockItem
			public abstract class isStockItem : PX.Data.BQL.BqlBool.Field<isStockItem> { }
			[PXDBBool(BqlField = typeof(SOLineSplit.isStockItem))]
			public virtual bool? IsStockItem
			{
				get;
				set;
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
			protected Int32? _InventoryID;
			[CrossItem(INPrimaryAlternateType.CPN, Filterable = true, BqlField = typeof(SOLineSplit.inventoryID))]
			public virtual Int32? InventoryID
			{
				get
				{
					return this._InventoryID;
				}
				set
				{
					this._InventoryID = value;
				}
			}
			#endregion
			#region ShipDate
			public abstract class shipDate : PX.Data.BQL.BqlDateTime.Field<shipDate> { }
			protected DateTime? _ShipDate;
			[PXDBDate(BqlField = typeof(SOLineSplit.shipDate))]
			[PXUIField(DisplayName = "Ship On", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual DateTime? ShipDate
			{
				get
				{
					return this._ShipDate;
				}
				set
				{
					this._ShipDate = value;
				}
			}
			#endregion
			#region TranDesc
			public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }
			protected String _TranDesc;
			[PXDBString(256, IsUnicode = true, BqlField = typeof(SOLine.tranDesc))]
			[PXUIField(DisplayName = "Line Description")]
			public virtual String TranDesc
			{
				get
				{
					return this._TranDesc;
				}
				set
				{
					this._TranDesc = value;
				}
			}
			#endregion
			#region SalesAcctID
			public abstract class salesAcctID : PX.Data.BQL.BqlInt.Field<salesAcctID> { }
			[PXDBInt(BqlField = typeof(SOLine.salesAcctID))]
			public virtual Int32? SalesAcctID
			{
				get;
				set;
			}
			#endregion
			#region SalesSubID
			public abstract class salesSubID : PX.Data.BQL.BqlInt.Field<salesSubID> { }
			[PXDBInt(BqlField = typeof(SOLine.salesSubID))]
			public virtual Int32? SalesSubID
			{
				get;
				set;
			}
			#endregion
			#region ProjectID
			public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
			protected Int32? _ProjectID;
			[PXDBInt(BqlField = typeof(SOLine.projectID))]
			public virtual Int32? ProjectID
			{
				get
				{
					return this._ProjectID;
				}
				set
				{
					this._ProjectID = value;
				}
			}
			#endregion
			#region TaskID
			public abstract class taskID : PX.Data.BQL.BqlInt.Field<taskID> { }
			protected Int32? _TaskID;
			[PXDBInt(BqlField = typeof(SOLine.taskID))]
			public virtual Int32? TaskID
			{
				get
				{
					return this._TaskID;
				}
				set
				{
					this._TaskID = value;
				}
			}
			#endregion
			#region CostCodeID
			public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID>
			{
			}
			[PXDBInt(BqlField = typeof(SOLine.costCodeID))]
			public virtual Int32? CostCodeID
			{
				get;
				set;
			}
			#endregion
			#region IsSpecialOrder
			public abstract class isSpecialOrder : PX.Data.BQL.BqlBool.Field<isSpecialOrder> { }
			[PXDBBool(BqlField = typeof(SOLine.isSpecialOrder))]
			public virtual Boolean? IsSpecialOrder
			{
				get;
				set;
			}
			#endregion
			#region CuryUnitCost
			public abstract class curyUnitCost : PX.Data.BQL.BqlDecimal.Field<curyUnitCost> { }
			[PXDBDecimal(6, BqlField = typeof(SOLine.curyUnitCost))]
			public virtual decimal? CuryUnitCost
			{
				get;
				set;
			}
			#endregion
			#region Active
			public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }

			[PXBool]
			[PXDBCalced(typeof(Switch<Case<Where<SOLineSplit.completed.IsEqual<True>>, False>, True>), typeof(bool?))]
			[PXUIField(DisplayName = "Active", Enabled = false)]
			public virtual Boolean? Active
			{
				get;
				set;
			}
			#endregion

			#region NoteID
			public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
			protected Guid? _NoteID;
			[PXNote(BqlField = typeof(SOLine.noteID))]
			public virtual Guid? NoteID
			{
				get
				{
					return this._NoteID;
				}
				set
				{
					this._NoteID = value;
				}
			}
			#endregion
			#region tstamp
			public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

			[PXDBTimestamp(BqlField = typeof(SOLineSplit.Tstamp), VerifyTimestamp = VerifyTimestampOptions.BothFromGraphAndRecord)]
			public virtual byte[] tstamp { get; set; }
			#endregion
		}

		[PXProjection(typeof(Select<SOLine>), Persistent = true)]
		[Serializable]
		[PXHidden]
		public partial class SOLine5 : PXBqlTable, IBqlTable
		{
			#region Keys
			public class PK : PrimaryKeyOf<SOLine5>.By<orderType, orderNbr, lineNbr>
			{
				public static SOLine5 Find(PXGraph graph, string orderType, string orderNbr, int? lineNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, orderType, orderNbr, lineNbr, options);
			}
			#endregion // Keys

			#region OrderType
			public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
			protected String _OrderType;
			[PXDBString(2, IsKey = true, IsFixed = true, BqlField = typeof(SOLine.orderType))]
			[PXDefault()]
			public virtual String OrderType
			{
				get
				{
					return this._OrderType;
				}
				set
				{
					this._OrderType = value;
				}
			}
			#endregion
			#region OrderNbr
			public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
			protected String _OrderNbr;
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(SOLine.orderNbr))]
			[PXDefault()]
			public virtual String OrderNbr
			{
				get
				{
					return this._OrderNbr;
				}
				set
				{
					this._OrderNbr = value;
				}
			}
			#endregion
			#region LineNbr
			public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true, BqlField = typeof(SOLine.lineNbr))]
			public virtual Int32? LineNbr
			{
				get
				{
					return this._LineNbr;
				}
				set
				{
					this._LineNbr = value;
				}
			}
			#endregion
			#region VendorID
			public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
			protected Int32? _VendorID;
			[PXDBInt(BqlField = typeof(SOLine.vendorID))]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			#region POCreated
			public abstract class pOCreated : PX.Data.IBqlField
			{
			}
			protected bool? _POCreated;
			[PXDBBool(BqlField = typeof(SOLine.pOCreated))]
			public virtual bool? POCreated
			{
				get
				{
					return this._POCreated;
				}
				set
				{
					this._POCreated = value;
				}
			}
			#endregion
			#region POSource
			public abstract class pOSource : PX.Data.BQL.BqlString.Field<pOSource> { }

			[PXDBString(BqlField = typeof(SOLine.pOSource))]
			public virtual string POSource
			{
				get;
				set;
			}
			#endregion
			#region CuryUnitCost
			public abstract class curyUnitCost : PX.Data.BQL.BqlDecimal.Field<curyUnitCost> { }
			[PXDBDecimal(6, BqlField = typeof(SOLine.curyUnitCost))]
			public virtual decimal? CuryUnitCost
			{
				get;
				set;
			}
			#endregion
			#region IsCostUpdatedOnPO
			public abstract class isCostUpdatedOnPO : PX.Data.BQL.BqlBool.Field<isCostUpdatedOnPO> { }
			[PXDBBool(BqlField = typeof(SOLine.isCostUpdatedOnPO))]
			public virtual Boolean? IsCostUpdatedOnPO
			{
				get;
				set;
			}
			#endregion
			#region CuryUnitCostUpdated
			public abstract class curyUnitCostUpdated : BqlDecimal.Field<curyUnitCostUpdated> { }
			[PXDecimal(6)]
			public virtual decimal? CuryUnitCostUpdated
			{
				get;
				set;
			}
			#endregion
			#region Completed
			public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }
			[PXDBBool(BqlField = typeof(SOLine.completed))]
			public virtual Boolean? Completed
			{
				get;
				set;
			}
			#endregion

			#region tstamp
			public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

			[PXDBTimestamp(BqlField = typeof(SOLine.Tstamp), VerifyTimestamp = VerifyTimestampOptions.BothFromGraphAndRecord)]
			public virtual byte[] tstamp { get; set; }
			#endregion
		}
		#endregion

		#region Implementation of IPXPrepareItems

		public virtual bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			if (string.Compare(viewName, "Transactions", true) == 0)
			{
				if (values.Contains("orderType")) values["orderType"] = Document.Current.OrderType;
				else values.Add("orderType", Document.Current.OrderType);

				if (values.Contains("orderNbr")) values["orderNbr"] = Document.Current.OrderNbr;
				else values.Add("orderNbr", Document.Current.OrderNbr);
				this._blockUIUpdate = true;
			}

			return true;
		}

		public bool RowImporting(string viewName, object row)
		{
			return row == null;
		}

		public bool RowImported(string viewName, object row, object oldRow)
		{
			return oldRow == null;
		}

		public virtual void PrepareItems(string viewName, IEnumerable items)
		{
		}

		#endregion

		#region EPApproval Cahce Attache
		[PXDefault(typeof(POOrder.orderDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void EPApproval_DocDate_CacheAttached(PXCache sender)
		{
		}

		[PXDefault(typeof(POOrder.vendorID), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void EPApproval_BAccountID_CacheAttached(PXCache sender)
		{
		}

		[PXDefault(typeof(Search<CREmployee.defContactID, Where<CREmployee.defContactID.IsEqual<POOrder.ownerID.FromCurrent>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void EPApproval_DocumentOwnerID_CacheAttached(PXCache sender)
		{
		}

		[PXDefault(typeof(POOrder.orderDesc), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void EPApproval_Descr_CacheAttached(PXCache sender)
		{
		}

		[CurrencyInfo(typeof(POOrder.curyInfoID))]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void EPApproval_CuryInfoID_CacheAttached(PXCache sender)
		{
		}

		[PXDefault(typeof(POOrder.curyOrderTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void EPApproval_CuryTotalAmount_CacheAttached(PXCache sender)
		{
		}

		[PXDefault(typeof(POOrder.orderTotal), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void EPApproval_TotalAmount_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#region External Tax

		public virtual bool IsExternalTax(string taxZoneID)
		{
			return false;
		}

		public virtual POOrder CalculateExternalTax(POOrder order)
		{
			return order;
		}

		protected virtual void InsertImportedTaxes()
		{
		}

		#endregion

		#region Discounts
		protected virtual void RecalculateDiscounts(PXCache sender, POLine line)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>() && line.InventoryID != null && line.OrderQty != null && line.CuryLineAmt != null)
			{
				DiscountEngine.DiscountCalculationOptions discountCalculationOptions = DiscountEngine.DefaultAPDiscountCalculationParameters;
				if (line.CalculateDiscountsOnImport == true)
					discountCalculationOptions = discountCalculationOptions | DiscountEngine.DiscountCalculationOptions.CalculateDiscountsFromImport;

				_discountEngine.SetDiscounts(
					sender,
					Transactions,
					line,
					DiscountDetails,
					Document.Current.BranchID,
					Document.Current.VendorLocationID,
					Document.Current.CuryID,
					Document.Current.OrderDate,
					recalcdiscountsfilter.Current,
					discountCalculationOptions);

				RecalculateTotalDiscount();
			}
			else if (!PXAccess.FeatureInstalled<FeaturesSet.vendorDiscounts>() && Document.Current != null)
			{
				_discountEngine.CalculateDocumentDiscountRate(Transactions.Cache, Transactions, line, DiscountDetails);
			}
		}

		private void RecalculateTotalDiscount()
		{
			if (Document.Current != null)
			{
				POOrder old_row = PXCache<POOrder>.CreateCopy(Document.Current);
				var discountTotals = _discountEngine.GetDiscountTotals(DiscountDetails);

				//Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers 
				Document.Cache.SetValueExt<SOOrder.curyGroupDiscTotal>(Document.Current, discountTotals.groupDiscountTotal);
				//Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers 
				Document.Cache.SetValueExt<SOOrder.curyDocumentDiscTotal>(Document.Current, discountTotals.documentDiscountTotal);
				//Acuminator disable once PX1045 PXGraphCreateInstanceInEventHandlers 
				Document.Cache.SetValueExt<SOOrder.curyDiscTot>(Document.Current, discountTotals.discountTotal);

				Document.Cache.RaiseRowUpdated(Document.Current, old_row);
			}
		}
		#endregion

		public virtual bool IsVendorOrLocationChanged(PXCache sender, POOrder order)
		{
			return VendorUpdatedContext.IsScoped() || VendorLocationUpdatedContext.IsScoped();
		}

		public virtual bool HasDetailRecords()
		{
			if (Transactions.Current != null)
				return true;

			if (Document.Cache.GetStatus(Document.Current) == PXEntryStatus.Inserted)
			{
				return Transactions.Cache.IsDirty;
			}
			else
			{
				return Transactions.Select().Count > 0;
			}
		}

		public virtual void CheckProjectAccountRule(PXCache sender, POLine row)
		{
			if (!(row.LineType == POLineType.Description || POLineType.IsStock(row.LineType)) &&
				row.ProjectID != ProjectDefaultAttribute.NonProject())
			{
				var select = new PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>(this);
				Account account = null;
				if (row.ExpenseAcctID != null)
				{
					account = select.Select(row.ExpenseAcctID);
				}

				if (account != null)
				{
					if (account.AccountGroupID == null)
					{
						sender.RaiseExceptionHandling<POLine.expenseAcctID>(row, account.AccountCD,
								new PXSetPropertyException(AP.Messages.NoAccountGroup, PXErrorLevel.Error, account.AccountCD));
					}
				}
			}
		}
		public override void Persist()
		{
			_discountEngine.ValidateDiscountDetails(DiscountDetails);

			if (apsetup.Current.VendorPriceUpdate == APVendorPriceUpdateType.Purchase && this.Document.Current != null && this.Document.Current.UpdateVendorCost != false)
			{
				List<POLine> linesToUpdateItemCost = new List<POLine>();
				foreach (POLine row in Transactions.Cache.Cached)
				{
					bool canOrderUpdateCost = row.OrderType.IsIn(POOrderType.RegularOrder, POOrderType.DropShip, POOrderType.ProjectDropShip);
					if (Transactions.Cache.GetStatus(row).IsIn(PXEntryStatus.Inserted, PXEntryStatus.Updated)
						&& row.InventoryID != null && canOrderUpdateCost && row.CuryUnitCost > 0)
					{
						linesToUpdateItemCost.Add(row);
					}
				}
				linesToUpdateItemCost.Sort((x, y) => ((int)x.LineNbr).CompareTo((int)y.LineNbr));

				foreach (POLine row in linesToUpdateItemCost)
				{
					POItemCostManager.Update(this,
						this.Document.Current.VendorID,
						this.Document.Current.VendorLocationID,
						this.Document.Current.CuryID,
						row.InventoryID,
						row.SubItemID,
						row.UOM,
						row.CuryUnitCost.Value);
				}
			}

			SOOrderEntry.ProcessPOOrder(this, Document.Current);

			ClearPOLinePlanIDIfPlanIsDeleted();

			//update values of AllowEditUnitCostInPR field
			foreach (POLine row in Transactions.Cache.Cached)
			{
				if ((Transactions.Cache.GetStatus(row) == PXEntryStatus.Inserted || Transactions.Cache.GetStatus(row) == PXEntryStatus.Updated))
				{
					row.AllowEditUnitCostInPR = row.HasInclusiveTaxes != true && row.CuryRetainageAmt == 0m;
				}
			}

			//Taxes can be imported as-is, ignoring all the validation rules when certain conditions are met. See POOrderEntryExternalTaxImport extension.
			InsertImportedTaxes();

			base.Persist();
		}

		protected virtual void ClearPOLinePlanIDIfPlanIsDeleted()
		{
			var planCache = this.Caches<INItemPlan>();
			HashSet<long> deletedPlanIDs =
				planCache.Cached.RowCast<INItemPlan>()
				.Where(p => planCache.GetStatus(p).IsIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted) == true
					&& p.PlanID != null)
				.Select(p => p.PlanID.Value)
				.ToHashSet();

			if (deletedPlanIDs.Any())
			{
				foreach (POLine poline in Transactions.Select())
				{
					if (poline.PlanID != null && deletedPlanIDs.Contains(poline.PlanID.Value))
					{
						poline.PlanID = null;
						Transactions.Cache.MarkUpdated(poline, assertError: true);
					}
				}
			}
		}

		public virtual string GetPOFixDemandSorter(POFixedDemand line)
		{
			if (line.PlanType == INPlanConstants.Plan90)
			{
				var ii = InventoryItem.PK.Find(this, line.InventoryID);
				return string.Format("ZZ.{0}", ii == null ? String.Empty : ii.InventoryCD);
			}
			else
			{
				return string.Format("{0}.{1}.{2:D7}", line.OrderType, line.OrderNbr, line.SortOrder.GetValueOrDefault());
			}
		}

		public virtual List<POFixedDemand> SortPOFixDemandList(List<POFixedDemand> list)
		{
			list.Sort((a, b) =>
			{
				string aSortOrder = String.Empty;
				string bSortOrder = String.Empty;

				aSortOrder = GetPOFixDemandSorter(a);
				bSortOrder = GetPOFixDemandSorter(b);

				return aSortOrder.CompareTo(bSortOrder);
			});

			return list;
		}

		public virtual bool RequireSingleProject(POOrder order)
		{
			return apsetup.Current.RequireSingleProjectPerDocument == true || order?.OrderType == POOrderType.ProjectDropShip;
		}

		private bool IsItemReceivedOrAPDocCreated(POLine row)
		{
			if (row.ReceivedQty > 0)
			{
				return true;
			}

			POReceiptLine receiptLine = SelectFrom<POReceiptLine>.
				Where<POReceiptLine.pOType.IsEqual<POLine.orderType.FromCurrent>.
				And<POReceiptLine.pONbr.IsEqual<POLine.orderNbr.FromCurrent>>.
				And<POReceiptLine.pOLineNbr.IsEqual<POLine.lineNbr.FromCurrent>>>
				.View.SelectSingleBound(this, new object[] { row });
			if (receiptLine != null)
			{
				return true;
			}

			APTran tran = SelectFrom<APTran>.
				Where<APTran.pOOrderType.IsEqual<POLine.orderType.FromCurrent>.
				And<APTran.pONbr.IsEqual<POLine.orderNbr.FromCurrent>>.
				And<APTran.pOLineNbr.IsEqual<POLine.lineNbr.FromCurrent>>>
				.View.SelectSingleBound(this, new object[] { row });
			if (tran != null)
			{
				return true;
			}

			return false;
		}

		#region Public API Methods
		public virtual POOrder FillPOOrderFromDemand(POOrder order, POFixedDemand demand, SOOrder soorder, DateTime? PurchDate, bool extSort, int? branchID = null)
		{
			order = PXCache<POOrder>.CreateCopy(Document.Insert(order));
			order.VendorID = demand.VendorID;
			order.VendorLocationID = demand.VendorLocationID;
			order.SiteID = demand.POSiteID;
			order.OrderDate = PurchDate;

			bool setBranch = PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>() && branchID != null;
			if (setBranch)
			{
				order.BranchID = branchID;
			}

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				// Redefault the base currency and use currency from vendor.
				order.OverrideCurrency = true;
			}

			if (demand.DemandProjectID != null && apsetup.Current.RequireSingleProjectPerDocument == true)
			{
				order.ProjectID = demand.DemandProjectID;
			}

			if (order.OrderType == POOrderType.DropShip || extSort)
			{
				order.SOOrderType = soorder.OrderType;
				order.SOOrderNbr = soorder.OrderNbr;
			}

			if (!string.IsNullOrEmpty(order.BLOrderNbr))
			{
				POOrder blanket = PXSelect<POOrder,
					Where<POOrder.orderType, Equal<Current<POOrder.bLType>>,
						And<POOrder.orderNbr, Equal<Current<POOrder.bLOrderNbr>>>>>
					.SelectSingleBound(this, new object[] { order });
				if (blanket != null)
				{
					order.VendorRefNbr = blanket.VendorRefNbr;
				}
			}

			if (order.OrderType == POOrderType.DropShip)
			{
				order.ShipDestType = POShippingDestination.Customer;
				order.ShipToBAccountID = soorder.CustomerID;
				order.ShipToLocationID = soorder.CustomerLocationID;
			}
			else if (POSetup.Current.ShipDestType == POShipDestType.Site)
			{
				order.ShipDestType = POShippingDestination.Site;
				order.SiteID = demand.POSiteID;
			}

			order = Document.Update(order);

			// The branch may be replaced by default branch of the vendor
			// and we want to preserve the branch that user has selected.
			if (setBranch && order.BranchID != branchID)
			{
				order.BranchID = branchID;
				order = Document.Update(order);
			}

			if (demand.IsSpecialOrder == true && order.CuryID != demand.CuryID)
			{
				order.CuryID = demand.CuryID;
				order = Document.Update(order);
			}

			if (order.OrderType == POOrderType.DropShip)
			{
				SOAddress soAddress = PXSelect<SOAddress,
					Where<SOAddress.addressID, Equal<Required<SOOrder.shipAddressID>>>>
					.Select(this, soorder.ShipAddressID);

				if (soAddress.IsDefaultAddress == false)
				{
					AddressAttribute.CopyRecord<POOrder.shipAddressID>(Document.Cache, order, soAddress, true);
				}
				SOContact soContact = PXSelect<SOContact,
					Where<SOContact.contactID, Equal<Required<SOOrder.shipContactID>>>>
					.Select(this, soorder.ShipContactID);

				if (soContact.IsDefaultContact == false)
				{
					ContactAttribute.CopyRecord<POOrder.shipContactID>(Document.Cache, order, soContact, true);
				}

				if (order.ExpectedDate < soorder.RequestDate)
				{
					order = PXCache<POOrder>.CreateCopy(order);
					order.ExpectedDate = soorder.RequestDate;
					order = Document.Update(order);
				}
			}

			return order;
		}

		public virtual void FillPOLineFromDemand(POLine dest, POFixedDemand demand, string OrderType, SOLineSplit3 solinesplit)
		{
			if (demand.PlanType == INPlanConstants.Plan90)
			{
				dest.LineType = POLineType.GoodsForReplenishment;
			}
			else if (demand.PlanType == INPlanConstants.PlanM5 || demand.PlanType == INPlanConstants.PlanM6 || demand.PlanType == INPlanConstants.PlanM9)
			{
				dest.LineType = POLineType.GoodsForManufacturing;
			}
            else if (demand.PlanType == INPlanConstants.PlanF6)
            {
                if (dest.LineType == null)
                {
                    dest.LineType = POLineType.GoodsForServiceOrder;
                }
            }
			else if (OrderType == POOrderType.RegularOrder)
			{
				if (solinesplit != null)
					dest.LineType = (solinesplit.LineType == SO.SOLineType.Inventory
										? POLineType.GoodsForSalesOrder
										: POLineType.NonStockForSalesOrder);
				else
					dest.LineType = POLineType.GoodsForSalesOrder;
			}
			else
			{
				if (solinesplit != null)
				{
					dest.LineType = (solinesplit.LineType == SO.SOLineType.Inventory
										? POLineType.GoodsForDropShip
										: POLineType.NonStockForDropShip);
					dest.ExpenseSubID = null;
					if (solinesplit.LineType == SOLineType.NonInventory)
					{
						var item = InventoryItem.PK.Find(this, solinesplit.InventoryID);
						var postclass = INPostClass.PK.Find(this, item?.PostClassID);
						if (postclass != null && postclass.COGSSubFromSales == true)
							dest.ExpenseSubID = solinesplit.SalesSubID;
					}
				}
			}

			if (solinesplit != null && demand.PlanType.IsIn(INPlanConstants.Plan6D, INPlanConstants.Plan6E))
			{
				dest.RequestedDate = solinesplit.RequestDate;
			}
			else if (solinesplit != null && demand.PlanType != INPlanConstants.Plan90)
			{
				dest.RequestedDate = solinesplit.ShipDate;
			}
			dest.VendorLocationID = demand.VendorLocationID;
			dest.InventoryID = demand.InventoryID;
			dest.SubItemID = demand.SubItemID;
			dest.UOM = demand.DemandUOM;
			dest.SiteID = demand.POSiteID;
			dest.OrderQty = demand.OrderQty;
			if (solinesplit != null)
			{
				if (this.POSetup.Current.CopyLineDescrSO == true)
					dest.TranDesc = solinesplit.TranDesc;

				dest.ProjectID = solinesplit.ProjectID;
				dest.TaskID = solinesplit.TaskID;
				dest.CostCodeID = solinesplit.CostCodeID;
				dest.IsSpecialOrder = solinesplit.IsSpecialOrder;
				if (dest.IsSpecialOrder == true)
				{
					dest.CuryUnitCost = solinesplit.CuryUnitCost;
					dest.SOOrderType = solinesplit.OrderType;
					dest.SOOrderNbr = solinesplit.OrderNbr;
					dest.SOLineNbr = solinesplit.LineNbr;
				}

				dest.IsStockItem = solinesplit.IsStockItem;
			}

			if (solinesplit != null && demand.AlternateID != null && demand.InventoryID != null)
			{
				PXSelectBase<INItemXRef> xref = new PXSelect<INItemXRef,
					Where<INItemXRef.inventoryID, Equal<Required<INItemXRef.inventoryID>>,
					And<INItemXRef.alternateID, Equal<Required<INItemXRef.alternateID>>>>>(this);

				INItemXRef soXRef = xref.Select(demand.InventoryID, demand.AlternateID);

				if (soXRef != null && soXRef.AlternateType == INAlternateType.Global)
				{
					if (dest.AlternateID != null)
					{
						INItemXRef poXRef = xref.Select(dest.InventoryID, dest.AlternateID);

						if (poXRef != null && poXRef.AlternateType == INAlternateType.Global)
						{
							dest.AlternateID = demand.AlternateID;
						}
					}
					else
					{
						object defAlternateID;
						Transactions.Cache.RaiseFieldDefaulting<POLine.alternateID>(dest, out defAlternateID);
						INItemXRef defXRef = defAlternateID == null ? null : xref.Select(demand.InventoryID, defAlternateID);

						dest.AlternateID = defXRef?.AlternateType == INAlternateType.VPN
							? defXRef.AlternateID
							: demand.AlternateID;
					}
				}
			}
		}
		#endregion

		#region Address Lookup Extension
		/// <exclude/>
		public class POOrderEntryAddressLookupExtension : CR.Extensions.AddressLookupExtension<POOrderEntry, POOrder, POShipAddress>
		{
			protected override string AddressView => nameof(Base.Shipping_Address);
		}

		/// <exclude/>
		public class POOrderEntryRemitAddressLookupExtension : CR.Extensions.AddressLookupExtension<POOrderEntry, POOrder, PORemitAddress>
		{
			protected override string AddressView => nameof(Base.Remit_Address);
		}

		public class POOrderEntryShippingAddressCachingHelper : AddressValidationExtension<POOrderEntry, POShipAddress>
		{
			protected override IEnumerable<PXSelectBase<POShipAddress>> AddressSelects()
			{
				yield return Base.Shipping_Address;
			}
		}

		public class POOrderEntryRemitAddressCachingHelper : AddressValidationExtension<POOrderEntry, PORemitAddress>
		{
			protected override IEnumerable<PXSelectBase<PORemitAddress>> AddressSelects()
			{
				yield return Base.Remit_Address;
			}
		}
		#endregion

		#region Entity Event handlers
		public PXWorkflowEventHandler<POOrder> OnLinesCompleted;
		public PXWorkflowEventHandler<POOrder> OnLinesClosed;
		public PXWorkflowEventHandler<POOrder> OnLinesReopened;

		public PXWorkflowEventHandler<POOrder> OnLinesLinked;
		public PXWorkflowEventHandler<POOrder> OnLinesUnlinked;

		public PXWorkflowEventHandler<POOrder> OnPrinted;

		public PXWorkflowEventHandler<POOrder> OnDoNotPrintChecked;
		public PXWorkflowEventHandler<POOrder> OnDoNotEmailChecked;

		public PXWorkflowEventHandler<POOrder> OnReleaseChangeOrder;
		#endregion
	}

	[PXHidden()]
	[PXProjection(typeof(Select4<POVendorInventory, Where<POVendorInventory.vendorID, Equal<CurrentValue<POSiteStatusFilter.vendorID>>>, Aggregate<GroupBy<POVendorInventory.vendorID, GroupBy<POVendorInventory.inventoryID, GroupBy<POVendorInventory.subItemID>>>>>), Persistent = false)]
	public class POVendorInventoryOnly : PXBqlTable, IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
		protected Int32? _VendorID;
		[PXDBInt(IsKey = true, BqlField = typeof(POVendorInventory.vendorID))]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		protected Int32? _InventoryID;
		[PXDBInt(IsKey = true, BqlField = typeof(POVendorInventory.inventoryID))]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region SubItemID
		public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		protected Int32? _SubItemID;
		[PXDBInt(IsKey = true, BqlField = typeof(POVendorInventory.subItemID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion		
	}

    [Serializable]
	public partial class POSiteStatusFilter : INSiteStatusFilter
	{
		#region SiteID
		public new abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

		[PXUIField(DisplayName = "Warehouse")]
		[SiteAttribute]
		[InterBranchRestrictor(typeof(Where2<SameOrganizationBranch<INSite.branchID, Current<POOrder.branchID>>,
			Or<Current<POOrder.orderType>, Equal<POOrderType.standardBlanket>>>))]
		[PXDefault(typeof(INRegister.siteID), PersistingCheck = PXPersistingCheck.Nothing)]
		public override Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region OnlyAvailable
		public new abstract class onlyAvailable : PX.Data.BQL.BqlBool.Field<onlyAvailable> { }
		[PXBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Only Vendor's Items")]
		public override bool? OnlyAvailable
		{
			get
			{
				return base._OnlyAvailable;
			}
			set
			{
				base._OnlyAvailable = value;
			}
		}
		#endregion

		#region VendorID
		public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
		protected Int32? _VendorID;
		[PXDBInt]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
	}

	[PXProjection(typeof(Select2<InventoryItem,
		LeftJoin<INSiteStatusByCostCenter,
			On<INSiteStatusByCostCenter.inventoryID, Equal<InventoryItem.inventoryID>,
				And<INSiteStatusByCostCenter.siteID, NotEqual<SiteAttribute.transitSiteID>,
				And<INSiteStatusByCostCenter.costCenterID, Equal<CostCenter.freeStock>>>>,
		LeftJoin<INSubItem,
			On<INSiteStatusByCostCenter.FK.SubItem>,
		LeftJoin<INSite,
			On2<INSiteStatusByCostCenter.FK.Site,
				And<INSite.baseCuryID, EqualBaseCuryID<Current2<POOrder.branchID>>>>,
		LeftJoin<INItemXRef,
			On<INItemXRef.inventoryID, Equal<InventoryItem.inventoryID>,
				And2<Where<INItemXRef.subItemID, Equal<INSiteStatusByCostCenter.subItemID>,
					Or<INSiteStatusByCostCenter.subItemID, IsNull>>,
				And<Where<CurrentValue<POSiteStatusFilter.barCode>, IsNotNull,
				And<INItemXRef.alternateType, In3<INAlternateType.barcode, INAlternateType.gIN>>>>>>,
		LeftJoin<INItemPartNumber,
			On<INItemPartNumber.inventoryID, Equal<InventoryItem.inventoryID>,
				And<INItemPartNumber.alternateID, Like<CurrentValue<POSiteStatusFilter.inventory_Wildcard>>,
				And2<Where<INItemPartNumber.bAccountID, Equal<Zero>, 
					Or<INItemPartNumber.bAccountID, Equal<CurrentValue<POOrder.vendorID>>,
					Or<INItemPartNumber.alternateType, Equal<INAlternateType.cPN>>>>,
				And<Where<INItemPartNumber.subItemID, Equal<INSiteStatusByCostCenter.subItemID>,
					Or<INSiteStatusByCostCenter.subItemID, IsNull>>>>>>,
		LeftJoin<INItemClass,
			On<InventoryItem.FK.ItemClass>,
		LeftJoin<INPriceClass,
			On<INPriceClass.priceClassID, Equal<InventoryItem.priceClassID>>,
		LeftJoin<InventoryItemCurySettings,
			On<InventoryItemCurySettings.inventoryID, Equal<InventoryItem.inventoryID>,
				And<InventoryItemCurySettings.curyID, EqualBaseCuryID<Current2<POOrder.branchID>>>>,
		LeftJoin<Vendor,
			On<Vendor.bAccountID, Equal<InventoryItemCurySettings.preferredVendorID>>,
		LeftJoin<INUnit,
			On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>,
				And<INUnit.unitType, Equal<INUnitType.inventoryItem>,
				And<INUnit.fromUnit, Equal<InventoryItem.purchaseUnit>,
				And<INUnit.toUnit, Equal<InventoryItem.baseUnit>>>>>>>>>>>>>>>,
		Where2<CurrentMatch<InventoryItem, AccessInfo.userName>,
			And2<Where<INSiteStatusByCostCenter.siteID, IsNull, Or<INSite.branchID, IsNotNull,
				And2<CurrentMatch<INSite, AccessInfo.userName>,
				And<Where2<FeatureInstalled<FeaturesSet.interBranch>,
					Or2<SameOrganizationBranch<INSite.branchID, Current<POOrder.branchID>>,
					Or<CurrentValue<POOrder.orderType>, Equal<POOrderType.standardBlanket>>>>>>>>,
			And2<Where<INSiteStatusByCostCenter.subItemID, IsNull,
				Or<CurrentMatch<INSubItem, AccessInfo.userName>>>,
			And<InventoryItem.stkItem, Equal<boolTrue>,
			And<InventoryItem.isTemplate, Equal<False>,
			And<InventoryItem.itemStatus, NotIn3<
				InventoryItemStatus.unknown,
				InventoryItemStatus.inactive,
				InventoryItemStatus.markedForDeletion,
				InventoryItemStatus.noPurchases>>>>>>>>), Persistent = false)]
	public partial class POSiteStatusSelected : PXBqlTable, IQtySelectable, IAlternateSelectable, IPXSelectable, IBqlTable
	{
		#region Selected
		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion

		#region InventoryID
		public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
		protected Int32? _InventoryID;
		[Inventory(BqlField = typeof(InventoryItem.inventoryID), IsKey = true)]
		[PXDefault()]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion

		#region InventoryCD
		public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
		protected string _InventoryCD;
		[PXDefault()]
		[InventoryRaw(BqlField = typeof(InventoryItem.inventoryCD))]
		public virtual String InventoryCD
		{
			get
			{
				return this._InventoryCD;
			}
			set
			{
				this._InventoryCD = value;
			}
		}
		#endregion

		#region Descr
		public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }

		protected string _Descr;
		[PXDBLocalizableString(Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(InventoryItem.descr), IsProjection = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion

		#region ItemClassID
		public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
		protected int? _ItemClassID;
		[PXDBInt(BqlField = typeof(InventoryItem.itemClassID))]
		[PXUIField(DisplayName = "Item Class ID", Visible = false)]
		[PXDimensionSelector(INItemClass.Dimension, typeof(INItemClass.itemClassID), typeof(INItemClass.itemClassCD), ValidComboRequired = true)]
		public virtual int? ItemClassID
		{
			get
			{
				return this._ItemClassID;
			}
			set
			{
				this._ItemClassID = value;
			}
		}
		#endregion

		#region ItemClassCD
		public abstract class itemClassCD : PX.Data.BQL.BqlString.Field<itemClassCD> { }
		protected string _ItemClassCD;
		[PXDBString(30, IsUnicode = true, BqlField = typeof(INItemClass.itemClassCD))]
		public virtual string ItemClassCD
		{
			get
			{
				return this._ItemClassCD;
			}
			set
			{
				this._ItemClassCD = value;
			}
		}
		#endregion

		#region ItemClassDescription
		public abstract class itemClassDescription : PX.Data.BQL.BqlString.Field<itemClassDescription> { }
		protected String _ItemClassDescription;
		[PXDBLocalizableString(Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(INItemClass.descr), IsProjection = true)]
		[PXUIField(DisplayName = "Item Class Description", Visible = false)]
		public virtual String ItemClassDescription
		{
			get
			{
				return this._ItemClassDescription;
			}
			set
			{
				this._ItemClassDescription = value;
			}
		}
		#endregion

		#region PriceClassID
		public abstract class priceClassID : PX.Data.BQL.BqlString.Field<priceClassID> { }

		protected string _PriceClassID;
		[PXDBString(10, IsUnicode = true, BqlField = typeof(InventoryItem.priceClassID))]
		[PXUIField(DisplayName = "Price Class ID", Visible = false)]
		public virtual String PriceClassID
		{
			get
			{
				return this._PriceClassID;
			}
			set
			{
				this._PriceClassID = value;
			}
		}
		#endregion

		#region PriceClassDescription
		public abstract class priceClassDescription : PX.Data.BQL.BqlString.Field<priceClassDescription> { }
		protected String _PriceClassDescription;
		[PXDBString(Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(INPriceClass.description))]
		[PXUIField(DisplayName = "Price Class Description", Visible = false)]
		public virtual String PriceClassDescription
		{
			get
			{
				return this._PriceClassDescription;
			}
			set
			{
				this._PriceClassDescription = value;
			}
		}
		#endregion

		#region PreferredVendorID
		public abstract class preferredVendorID : PX.Data.BQL.BqlInt.Field<preferredVendorID> { }

		protected Int32? _PreferredVendorID;
		[AP.Vendor(DisplayName = "Preferred Vendor ID", Required = false, DescriptionField = typeof(AP.Vendor.acctName), BqlField = typeof(InventoryItemCurySettings.preferredVendorID), Visible = false)]
		public virtual Int32? PreferredVendorID
		{
			get
			{
				return this._PreferredVendorID;
			}
			set
			{
				this._PreferredVendorID = value;
			}
		}
		#endregion

		#region PreferredVendorDescription
		public abstract class preferredVendorDescription : PX.Data.BQL.BqlString.Field<preferredVendorDescription> { }
		protected String _PreferredVendorDescription;
		[PXDBString(250, IsUnicode = true, BqlField = typeof(Vendor.acctName))]
		[PXUIField(DisplayName = "Preferred Vendor Name", Visible = false)]
		public virtual String PreferredVendorDescription
		{
			get
			{
				return this._PreferredVendorDescription;
			}
			set
			{
				this._PreferredVendorDescription = value;
			}
		}
		#endregion

		#region BarCode
		public abstract class barCode : PX.Data.BQL.BqlString.Field<barCode> { }
		protected String _BarCode;
		[PXDBString(255, BqlField = typeof(INItemXRef.alternateID), IsUnicode = true)]
		public virtual String BarCode
		{
			get
			{
				return this._BarCode;
			}
			set
			{
				this._BarCode = value;
			}
		}
		#endregion

		#region BarCodeAlternateType
		//INAlternateType.Barcode is not only possible value
		/// <inheritdoc cref="INItemXRef.AlternateType"/>
		[PXDBString(4, BqlField = typeof(INItemXRef.alternateType))]
		public virtual String BarCodeType { get; set; }
		public abstract class barCodeType : PX.Data.BQL.BqlString.Field<barCodeType> { }
		#endregion

		#region BarCodeDescr
		/// <inheritdoc cref="INItemXRef.Descr"/>
		[PXDBString(60, IsUnicode = true, BqlField = typeof(INItemXRef.descr))]
		public virtual String BarCodeDescr { get; set; }
		public abstract class barCodeDescr : PX.Data.BQL.BqlString.Field<barCodeDescr> { }
		#endregion

		#region AlternateID
		public abstract class alternateID : PX.Data.BQL.BqlString.Field<alternateID> { }
		protected String _AlternateID;
		[PXDBString(225, IsUnicode = true, InputMask = "", BqlField = typeof(INItemPartNumber.alternateID))]
		[PXUIField(DisplayName = "Alternate ID")]
		[PXExtraKey]
		public virtual String AlternateID
		{
			get
			{
				return this._AlternateID;
			}
			set
			{
				this._AlternateID = value;
			}
		}
		#endregion

		#region AlternateType
		public abstract class alternateType : PX.Data.BQL.BqlString.Field<alternateType> { }
		protected String _AlternateType;
		[PXDBString(4, BqlField = typeof(INItemPartNumber.alternateType))]
		[INAlternateType.List()]
		[PXDefault(INAlternateType.Global)]
		[PXUIField(DisplayName = "Alternate Type")]
		public virtual String AlternateType
		{
			get
			{
				return this._AlternateType;
			}
			set
			{
				this._AlternateType = value;
			}
		}
		#endregion

		#region Descr
		public abstract class alternateDescr : PX.Data.BQL.BqlString.Field<alternateDescr> { }
		protected String _AlternateDescr;
		[PXDBString(60, IsUnicode = true, BqlField = typeof(INItemPartNumber.descr))]
		[PXUIField(DisplayName = "Alternate Description", Visible = false)]
		public virtual String AlternateDescr
		{
			get
			{
				return this._AlternateDescr;
			}
			set
			{
				this._AlternateDescr = value;
			}
		}
		#endregion

		#region InventoryAlternateID
		/// <inheritdoc cref="INItemXRef.AlternateID" />
		[PXDBString(225, IsUnicode = true, InputMask = "", BqlField = typeof(INItemPartNumber.alternateID))]
		public virtual String InventoryAlternateID { get; set; }
		public abstract class inventoryAlternateID : PX.Data.BQL.BqlString.Field<inventoryAlternateID> { }


		#endregion

		#region InventoryAlternateType
		/// <inheritdoc cref="INItemXRef.AlternateType"/>
		[PXDBString(4, BqlField = typeof(INItemPartNumber.alternateType))]
		public virtual String InventoryAlternateType { get; set; }
		public abstract class inventoryAlternateType : PX.Data.BQL.BqlString.Field<inventoryAlternateType> { }
		#endregion

		#region InventoryDescr
		/// <inheritdoc cref="INItemXRef.Descr"/>
		[PXDBString(4, BqlField = typeof(INItemPartNumber.descr))]
		public virtual String InventoryAlternateDescr { get; set; }
		public abstract class inventoryAlternateDescr : PX.Data.BQL.BqlString.Field<inventoryAlternateDescr> { }
		#endregion

		#region SiteID
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		protected int? _SiteID;
		[PXUIField(DisplayName = "Warehouse")]
		[SiteAttribute(BqlField = typeof(INSiteStatusByCostCenter.siteID))]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
        #endregion

        #region SiteCD
        public abstract class siteCD : PX.Data.BQL.BqlString.Field<siteCD> { }
        protected String _SiteCD;
		[PXString(IsUnicode = true, IsKey = true)]
		[PXDBCalced(typeof(IsNull<Data.RTrim<INSite.siteCD>, Empty>), typeof(string))]
		public virtual String SiteCD
        {
            get
            {
                return this._SiteCD;
            }
            set
            {
                this._SiteCD = value;
            }
        }
        #endregion

        #region SubItemID
        public abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }
		protected int? _SubItemID;
		[SubItem(typeof(POSiteStatusSelected.inventoryID),BqlField = typeof(INSubItem.subItemID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion

		#region SubItemCD
		public abstract class subItemCD : PX.Data.BQL.BqlString.Field<subItemCD> { }
		protected String _SubItemCD;
		[PXString(IsUnicode = true, IsKey = true)]
		[PXDBCalced(typeof(IsNull<Data.RTrim<INSubItem.subItemCD>, Empty>), typeof(string))]
		public virtual String SubItemCD
		{
			get
			{
				return this._SubItemCD;
			}
			set
			{
				this._SubItemCD = value;
			}
		}
		#endregion

		#region BaseUnit
		public abstract class baseUnit : PX.Data.BQL.BqlString.Field<baseUnit> { }

		protected string _BaseUnit;
		[PXDefault(typeof(Search<INItemClass.baseUnit, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>))]
		[INUnit(DisplayName = "Base Unit", Visibility = PXUIVisibility.Visible, BqlField = typeof(InventoryItem.baseUnit))]
		public virtual String BaseUnit
		{
			get
			{
				return this._BaseUnit;
			}
			set
			{
				this._BaseUnit = value;
			}
		}
		#endregion

		#region PurchaseUnit
		public abstract class purchaseUnit : PX.Data.BQL.BqlString.Field<purchaseUnit> { }
		protected string _PurchaseUnit;
		[INUnit(typeof(POSiteStatusSelected.inventoryID), DisplayName = "Purchase Unit", BqlField = typeof(InventoryItem.purchaseUnit))]
		public virtual String PurchaseUnit
		{
			get
			{
				return this._PurchaseUnit;
			}
			set
			{
				this._PurchaseUnit = value;
			}
		}
		#endregion

		#region QtySelected
		public abstract class qtySelected : PX.Data.BQL.BqlDecimal.Field<qtySelected> { }
		protected Decimal? _QtySelected;
		[PXQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Selected")]
		public virtual Decimal? QtySelected
		{
			get
			{
				return this._QtySelected ?? 0m;
			}
			set
			{
				if (value != null && value != 0m)
					this._Selected = true;
				this._QtySelected = value;
			}
		}
		#endregion

		#region QtyOnHand
		public abstract class qtyOnHand : PX.Data.BQL.BqlDecimal.Field<qtyOnHand> { }
		protected Decimal? _QtyOnHand;
		[PXDBQuantity(BqlField = typeof(INSiteStatusByCostCenter.qtyOnHand))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Hand")]
		public virtual Decimal? QtyOnHand
		{
			get
			{
				return this._QtyOnHand;
			}
			set
			{
				this._QtyOnHand = value;
			}
		}
		#endregion

		#region QtyOnHandExt
		public abstract class qtyOnHandExt : PX.Data.BQL.BqlDecimal.Field<qtyOnHandExt> { }
		protected Decimal? _QtyOnHandExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatusByCostCenter.qtyOnHand, INUnit.unitRate>>,
			Div<INSiteStatusByCostCenter.qtyOnHand, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. On Hand")]
		public virtual Decimal? QtyOnHandExt
		{
			get
			{
				return this._QtyOnHandExt;
			}
			set
			{
				this._QtyOnHandExt = value;
			}
		}
		#endregion

		#region QtyAvail
		public abstract class qtyAvail : PX.Data.BQL.BqlDecimal.Field<qtyAvail> { }
		protected Decimal? _QtyAvail;
		[PXDBQuantity(BqlField = typeof(INSiteStatusByCostCenter.qtyAvail))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Available")]
		public virtual Decimal? QtyAvail
		{
			get
			{
				return this._QtyAvail;
			}
			set
			{
				this._QtyAvail = value;
			}
		}
		#endregion

		#region QtyAvailExt
		public abstract class qtyAvailExt : PX.Data.BQL.BqlDecimal.Field<qtyAvailExt> { }
		protected Decimal? _QtyAvailExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatusByCostCenter.qtyAvail, INUnit.unitRate>>,
			Div<INSiteStatusByCostCenter.qtyAvail, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. Available")]
		public virtual Decimal? QtyAvailExt
		{
			get
			{
				return this._QtyAvailExt;
			}
			set
			{
				this._QtyAvailExt = value;
			}
		}
		#endregion

		#region QtyPOPrepared
		public abstract class qtyPOPrepared : PX.Data.BQL.BqlDecimal.Field<qtyPOPrepared> { }
		protected Decimal? _QtyPOPrepared;
		[PXDBQuantity(BqlField = typeof(INSiteStatusByCostCenter.qtyPOPrepared))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Prepared")]
		public virtual Decimal? QtyPOPrepared
		{
			get
			{
				return this._QtyPOPrepared;
			}
			set
			{
				this._QtyPOPrepared = value;
			}
		}
		#endregion

		#region QtyPOPreparedExt
		public abstract class qtyPOPreparedExt : PX.Data.BQL.BqlDecimal.Field<qtyPOPreparedExt> { }
		protected Decimal? _QtyPOPreparedExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
				Mult<INSiteStatusByCostCenter.qtyPOPrepared, INUnit.unitRate>>,
				Div<INSiteStatusByCostCenter.qtyPOPrepared, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Prepared")]
		public virtual Decimal? QtyPOPreparedExt
		{
			get
			{
				return this._QtyPOPreparedExt;
			}
			set
			{
				this._QtyPOPreparedExt = value;
			}
		}
		#endregion

		#region QtyPOOrders
		public abstract class qtyPOOrders : PX.Data.BQL.BqlDecimal.Field<qtyPOOrders> { }
		protected Decimal? _QtyPOOrders;
		[PXDBQuantity(BqlField = typeof(INSiteStatusByCostCenter.qtyPOOrders))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Orders")]
		public virtual Decimal? QtyPOOrders
		{
			get
			{
				return this._QtyPOOrders;
			}
			set
			{
				this._QtyPOOrders = value;
			}
		}
		#endregion

		#region QtyPOOrdersExt
		public abstract class qtyPOOrdersExt : PX.Data.BQL.BqlDecimal.Field<qtyPOOrdersExt> { }
		protected Decimal? _QtyPOOrdersExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
				Mult<INSiteStatusByCostCenter.qtyPOOrders, INUnit.unitRate>>,
				Div<INSiteStatusByCostCenter.qtyPOOrders, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Orders")]
		public virtual Decimal? QtyPOOrdersExt
		{
			get
			{
				return this._QtyPOOrdersExt;
			}
			set
			{
				this._QtyPOOrdersExt = value;
			}
		}
		#endregion

		#region QtyPOReceipts
		public abstract class qtyPOReceipts : PX.Data.BQL.BqlDecimal.Field<qtyPOReceipts> { }
		protected Decimal? _QtyPOReceipts;
		[PXDBQuantity(BqlField = typeof(INSiteStatusByCostCenter.qtyPOReceipts))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Receipts")]
		public virtual Decimal? QtyPOReceipts
		{
			get
			{
				return this._QtyPOReceipts;
			}
			set
			{
				this._QtyPOReceipts = value;
			}
		}
		#endregion

		#region QtyPOReceiptsExt
		public abstract class qtyPOReceiptsExt : PX.Data.BQL.BqlDecimal.Field<qtyPOReceiptsExt> { }
		protected Decimal? _QtyPOReceiptsExt;
		[PXDBCalced(typeof(Switch<Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
			Mult<INSiteStatusByCostCenter.qtyPOReceipts, INUnit.unitRate>>,
			Div<INSiteStatusByCostCenter.qtyPOReceipts, INUnit.unitRate>>), typeof(decimal))]
		[PXQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Qty. PO Receipts")]
		public virtual Decimal? QtyPOReceiptsExt
		{
			get
			{
				return this._QtyPOReceiptsExt;
			}
			set
			{
				this._QtyPOReceiptsExt = value;
			}
		}
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
		protected Guid? _NoteID;
		[PXNote(BqlField = typeof(InventoryItem.noteID))]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
	}
}
