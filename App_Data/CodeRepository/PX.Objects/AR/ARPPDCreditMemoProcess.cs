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
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

using PX.Common;
using PX.Data;
using PX.Data.EP;

using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM.Extensions;
using PX.Objects.TX;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common.Abstractions;
using PX.Objects.Extensions.MultiCurrency;

namespace PX.Objects.AR
{
	/// <summary>
	/// Represents the processing parameters for the Generate AR Tax 
	/// Adjustments (AR504500) process, which corresponds to the <see 
	/// cref="ARPPDCreditMemoProcess"/> graph.
	/// </summary>
	[Serializable]
	[PXCacheName("AR Tax Adjustment Parameters")]
	public partial class ARPPDTaxAdjustmentParameters : PXBqlTable, IBqlTable
	{
		#region ApplicationDate
		public abstract class applicationDate : PX.Data.BQL.BqlDateTime.Field<applicationDate> { }
		/// <summary>
		/// Application Date
		/// </summary>
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.Visible, Required = true)]
		public virtual DateTime? ApplicationDate
		{
			get;
			set;
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
		/// <summary>
		/// Branch ID
		/// </summary>
		[Branch]
		public virtual int? BranchID
		{
			get;
			set;
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
		/// <summary>
		/// Customer ID
		/// </summary>
		[Customer]
		public virtual int? CustomerID
		{
			get;
			set;
		}
		#endregion
		#region GenerateOnePerCustomer
		public abstract class generateOnePerCustomer : PX.Data.BQL.BqlBool.Field<generateOnePerCustomer> { }
		/// <summary>
		/// Generate one document per customer
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Consolidate Tax Adjustments by Customer", Visibility = PXUIVisibility.Visible)]
		public virtual bool? GenerateOnePerCustomer
		{
			get;
			set;
		}
		#endregion
		#region TaxAdjustmentDate
		public abstract class taxAdjustmentDate : PX.Data.BQL.BqlDateTime.Field<taxAdjustmentDate> { }
		/// <summary>
		/// Tax adjustment date
		/// </summary>
		[PXDBDate]
		[PXFormula(typeof(Switch<Case<Where<ARPPDTaxAdjustmentParameters.generateOnePerCustomer, Equal<True>>, Current<AccessInfo.businessDate>>, Null>))]
		[PXUIField(DisplayName = "Tax Adjustment Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? TaxAdjustmentDate
		{
			get;
			set;
		}
		#endregion
		#region FinPeriod
		public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
		/// <summary>
		/// Fin Period ID
		/// </summary>
		[AROpenPeriod(typeof(ARPPDTaxAdjustmentParameters.taxAdjustmentDate), typeof(ARPPDTaxAdjustmentParameters.branchID))]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.Visible)]
		public virtual string FinPeriodID
		{
			get;
			set;
		}
		#endregion
	}

	/// <summary>
	/// A projection over <see cref="ARAdjust"/>, which selects
	/// applications of payments to invoices that have been paid
	/// in full and await processing of the cash discount on the 
	/// Generate AR Tax Adjustments (AR504500) process.
	/// </summary>
	[PXProjection(typeof(Select2<ARAdjust,
		InnerJoin<AR.ARInvoice, On<AR.ARInvoice.docType, Equal<ARAdjust.adjdDocType>, And<AR.ARInvoice.refNbr, Equal<ARAdjust.adjdRefNbr>>>>,
		Where<AR.ARInvoice.released, Equal<True>,
			And<AR.ARInvoice.pendingPPD, Equal<True>,
			And<AR.ARInvoice.openDoc, Equal<True>,
			And<ARAdjust.released, Equal<True>,
			And<ARAdjust.voided, NotEqual<True>,
			And<ARAdjust.pendingPPD, Equal<True>,
			And<ARAdjust.pPDVATAdjRefNbr, IsNull,
			And<Where<ARAdjust.adjgDocType, Equal<ARDocType.payment>,
				Or<ARAdjust.adjgDocType, Equal<ARDocType.prepayment>,
				Or<ARAdjust.adjgDocType, Equal<ARDocType.prepaymentInvoice>,
				Or<ARAdjust.adjgDocType, Equal<ARDocType.refund>>>>>>>>>>>>>>))]
	[Serializable]
	[PXCacheName("Pending PPD AR Tax Adj App")]
	public partial class PendingPPDARTaxAdjApp : ARAdjust
	{
		#region Selected
		public new abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
		#endregion
		#region Index
		public abstract class index : PX.Data.BQL.BqlInt.Field<index> { }

		/// <summary>
		/// Application index. 
		/// </summary>
		[PXInt]
		public virtual int? Index
		{
			get;
			set;
		}
		#endregion
		#region PayDocType
		public abstract class payDocType : PX.Data.BQL.BqlString.Field<payDocType> { }

		/// <inheritdoc cref="ARAdjust.AdjgDocType"/>
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "", BqlField = typeof(ARAdjust.adjgDocType))]
		public virtual string PayDocType
		{
			get;
			set;
		}
		#endregion
		#region PayRefNbr
		public abstract class payRefNbr : PX.Data.BQL.BqlString.Field<payRefNbr> { }

		/// <inheritdoc cref="ARAdjust.AdjgRefNbr"/>
		[PXDBString(15, IsUnicode = true, IsKey = true, BqlField = typeof(ARAdjust.adjgRefNbr))]
		public virtual string PayRefNbr
		{
			get;
			set;
		}
		#endregion
		#region InvDocType
		public abstract class invDocType : PX.Data.BQL.BqlString.Field<invDocType> { }

		/// <inheritdoc cref="ARAdjust.AdjdDocType"/>
		[PXDBString(3, IsKey = true, IsFixed = true, InputMask = "", BqlField = typeof(ARAdjust.adjdDocType))]
		[ARInvoiceType.TaxAdjdList]
		public virtual string InvDocType
		{
			get;
			set;
		}
		#endregion
		#region InvRefNbr
		public abstract class invRefNbr : PX.Data.BQL.BqlString.Field<invRefNbr> { }

		/// <inheritdoc cref="ARAdjust.AdjdRefNbr"/>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(ARAdjust.adjdRefNbr))]
		public virtual string InvRefNbr
		{
			get;
			set;
		}
		#endregion
		#region InvCuryID
		public abstract class invCuryID : PX.Data.BQL.BqlString.Field<invCuryID> { }

		/// <inheritdoc cref="ARRegister.CuryID"/>
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL", BqlField = typeof(AR.ARInvoice.curyID))]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual string InvCuryID
		{
			get;
			set;
		}
		#endregion
		#region InvCuryInfoID
		public abstract class invCuryInfoID : PX.Data.BQL.BqlLong.Field<invCuryInfoID> { }

		/// <inheritdoc cref="ARRegister.CuryInfoID"/>
		[PXDBLong(BqlField = typeof(AR.ARInvoice.curyInfoID))]
		[CurrencyInfo]
		public virtual long? InvCuryInfoID
		{
			get;
			set;
		}
		#endregion
		#region InvCustomerLocationID
		public abstract class invCustomerLocationID : PX.Data.BQL.BqlInt.Field<invCustomerLocationID> { }

		/// <inheritdoc cref="ARRegister.CustomerLocationID"/>
		[PXDBInt(BqlField = typeof(AR.ARInvoice.customerLocationID))]
		public virtual int? InvCustomerLocationID
		{
			get;
			set;
		}
		#endregion
		#region InvTaxZoneID
		public abstract class invTaxZoneID : PX.Data.BQL.BqlString.Field<invTaxZoneID> { }

		/// <inheritdoc cref="ARInvoice.TaxZoneID"/>
		[PXDBString(10, IsUnicode = true, BqlField = typeof(AR.ARInvoice.taxZoneID))]
		public virtual string InvTaxZoneID
		{
			get;
			set;
		}
		#endregion
		#region InvTaxCalcMode
		public abstract class invTaxCalcMode : PX.Data.BQL.BqlString.Field<invTaxCalcMode> { }

		/// <inheritdoc cref="ARRegister.TaxCalcMode"/>
		[PXDBString(1, IsFixed = true, BqlField = typeof(AR.ARInvoice.taxCalcMode))]
		public virtual string InvTaxCalcMode
		{
			get;
			set;
		}
		#endregion
		#region InvTermsID
		public abstract class invTermsID : PX.Data.BQL.BqlString.Field<invTermsID> { }

		/// <inheritdoc cref="ARInvoice.TermsID"/>
		[PXDBString(10, IsUnicode = true, BqlField = typeof(AR.ARInvoice.termsID))]
		[PXUIField(DisplayName = "Credit Terms", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<Terms.termsID, Where<Terms.visibleTo, Equal<TermsVisibleTo.all>, Or<Terms.visibleTo, Equal<TermsVisibleTo.customer>>>>), DescriptionField = typeof(Terms.descr), Filterable = true)]
		[Terms(typeof(AR.ARInvoice.docDate), typeof(AR.ARInvoice.dueDate), typeof(AR.ARInvoice.discDate), typeof(AR.ARInvoice.curyOrigDocAmt), typeof(AR.ARInvoice.curyOrigDiscAmt), typeof(AR.ARInvoice.curyTaxTotal), typeof(AR.ARInvoice.branchID))]
		public virtual string InvTermsID
		{
			get;
			set;
		}
		#endregion
		#region InvCuryOrigDocAmt
		public abstract class invCuryOrigDocAmt : PX.Data.BQL.BqlDecimal.Field<invCuryOrigDocAmt> { }

		/// <inheritdoc cref="ARRegister.CuryOrigDocAmt"/>
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.origDocAmt), BqlField = typeof(AR.ARInvoice.curyOrigDocAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? InvCuryOrigDocAmt
		{
			get;
			set;
		}
		#endregion
		#region InvCuryOrigDiscAmt
		public abstract class invCuryOrigDiscAmt : PX.Data.BQL.BqlDecimal.Field<invCuryOrigDiscAmt> { }

		/// <inheritdoc cref="ARRegister.CuryOrigDiscAmt"/>
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.origDiscAmt), BqlField = typeof(AR.ARInvoice.curyOrigDiscAmt))]
		[PXUIField(DisplayName = "Cash Discount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual decimal? InvCuryOrigDiscAmt
		{
			get;
			set;
		}
		#endregion
		#region InvCuryVatTaxableTotal
		public abstract class invCuryVatTaxableTotal : PX.Data.BQL.BqlDecimal.Field<invCuryVatTaxableTotal> { }

		/// <inheritdoc cref="ARInvoice.CuryVatTaxableTotal"/>
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.vatTaxableTotal), BqlField = typeof(AR.ARInvoice.curyVatTaxableTotal))]
		[PXUIField(DisplayName = "VAT Taxable Total", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? InvCuryVatTaxableTotal
		{
			get;
			set;
		}
		#endregion
		#region InvCuryTaxTotal
		public abstract class invCuryTaxTotal : PX.Data.BQL.BqlDecimal.Field<invCuryTaxTotal> { }

		/// <inheritdoc cref="ARInvoice.CuryTaxTotal"/>
		[PXDBCurrency(typeof(ARRegister.curyInfoID), typeof(AR.ARInvoice.taxTotal), BqlField = typeof(AR.ARInvoice.curyTaxTotal))]
		[PXUIField(DisplayName = "Tax Total", Visibility = PXUIVisibility.Visible)]
		public virtual decimal? InvCuryTaxTotal
		{
			get;
			set;
		}
		#endregion
		#region InvCuryDocBal
		public abstract class invCuryDocBal : PX.Data.BQL.BqlDecimal.Field<invCuryDocBal> { }

		/// <inheritdoc cref="ARRegister.CuryDocBal"/>
		[PXDBCurrency(typeof(AR.ARInvoice.curyInfoID), typeof(AR.ARInvoice.docBal), BaseCalc = false, BqlField = typeof(AR.ARInvoice.curyDocBal))]
		public virtual decimal? InvCuryDocBal
		{
			get;
			set;
		}
		#endregion
	}

	[Obsolete(nameof(PPDApplicationKey) + "is used instead of this class now")]
	public class PPDCreditMemoKey
	{
		private readonly FieldInfo[] _fields;
		
		public int? BranchID;
		public int? CustomerID;
		public int? CustomerLocationID;
		public string CuryID;
		public decimal? CuryRate;
		public int? ARAccountID;
		public int? ARSubID;
		public string TaxZoneID;

		public PPDCreditMemoKey()
		{
			_fields = GetType().GetFields();
		}

		public override bool Equals(object obj)
		{
			FieldInfo info = _fields.FirstOrDefault(field => !Equals(field.GetValue(this), field.GetValue(obj)));
			return info == null;
		}
		public override int GetHashCode()
		{
			int hashCode = 17;
			_fields.ForEach(field => hashCode = hashCode * 23 + field.GetValue(this).GetHashCode());
			return hashCode;
		}
	}

	[TableAndChartDashboardType]
	public class ARPPDCreditMemoProcess : PXGraph<ARPPDCreditMemoProcess>
	{
		public PXCancel<ARPPDTaxAdjustmentParameters> Cancel;
		public PXFilter<ARPPDTaxAdjustmentParameters> Filter;
		
		[PXFilterable]
		public PXFilteredProcessing<PendingPPDARTaxAdjApp, ARPPDTaxAdjustmentParameters> Applications;
		public ARSetupNoMigrationMode arsetup;
        
        public override bool IsDirty
		{
			get { return false; }
        }

		#region Cache Attached

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[ARInvoiceType.TaxAdjdList]
		protected virtual void PendingPPDARTaxAdjApp_AdjdDocType_CacheAttached(PXCache sender) { }

		[Customer]
		protected virtual void PendingPPDARTaxAdjApp_AdjdCustomerID_CacheAttached(PXCache sender) { }

		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARInvoiceType.RefNbr(typeof(Search2<
			Standalone.ARRegisterAlias.refNbr,
				InnerJoinSingleTable<ARInvoice, 
					On<ARInvoice.docType, Equal<Standalone.ARRegisterAlias.docType>,
					And<ARInvoice.refNbr, Equal<Standalone.ARRegisterAlias.refNbr>>>,
				InnerJoinSingleTable<Customer, 
					On<Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
			Where<
				Standalone.ARRegisterAlias.docType, Equal<Optional<PendingPPDARTaxAdjApp.invDocType>>,
				And2<Where<
					Standalone.ARRegisterAlias.origModule, Equal<BatchModule.moduleAR>, 
					Or<Standalone.ARRegisterAlias.released, Equal<True>>>,
				And<Match<Customer, Current<AccessInfo.userName>>>>>, 
			OrderBy<Desc<Standalone.ARRegisterAlias.refNbr>>>))]
		[ARInvoiceType.Numbering]
		[ARInvoiceNbr]
		[PXFieldDescription]
		protected virtual void PendingPPDARTaxAdjApp_AdjdRefNbr_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIField(DisplayName = "Doc. Date")]
		protected virtual void PendingPPDARTaxAdjApp_AdjdDocDate_CacheAttached(PXCache sender) { }

		[PXDBCurrency(typeof(ARAdjust.adjdCuryInfoID), typeof(ARAdjust.adjPPDAmt))]
		[PXUIField(DisplayName = "Cash Discount")]
		protected virtual void PendingPPDARTaxAdjApp_CuryAdjdPPDAmt_CacheAttached(PXCache sender) { }

		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Payment Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
		[ARPaymentType.RefNbr(typeof(Search2<
			Standalone.ARRegisterAlias.refNbr,
				InnerJoinSingleTable<ARPayment, 
					On<ARPayment.docType, Equal<Standalone.ARRegisterAlias.docType>,
					And<ARPayment.refNbr, Equal<Standalone.ARRegisterAlias.refNbr>>>,
				InnerJoinSingleTable<Customer, 
					On<Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
			Where<
				Standalone.ARRegisterAlias.docType, Equal<Current<PendingPPDARTaxAdjApp.payDocType>>,
				And<Match<Customer, Current<AccessInfo.userName>>>>,
			OrderBy<Desc<Standalone.ARRegisterAlias.refNbr>>>))]
		[ARPaymentType.Numbering]
		[PXFieldDescription]
		protected virtual void PendingPPDARTaxAdjApp_AdjgRefNbr_CacheAttached(PXCache sender) { }

		#endregion

		public ARPPDCreditMemoProcess()
		{
			Applications.AllowDelete = true;
			Applications.AllowInsert = false;
			Applications.SetSelected<PendingPPDARTaxAdjApp.selected>();
		}

		public virtual IEnumerable applications(PXAdapter adapter)
		{
			ARPPDTaxAdjustmentParameters filter = Filter.Current;
			if (filter == null || filter.ApplicationDate == null || filter.BranchID == null) yield break;

			PXSelectBase<PendingPPDARTaxAdjApp> select = new PXSelect<PendingPPDARTaxAdjApp,
				Where<PendingPPDARTaxAdjApp.adjgDocDate, LessEqual<Current<ARPPDTaxAdjustmentParameters.applicationDate>>,
					And<PendingPPDARTaxAdjApp.adjdBranchID, Equal<Current<ARPPDTaxAdjustmentParameters.branchID>>>>>(this);

			if (filter.CustomerID != null)
			{
				select.WhereAnd<Where<PendingPPDARTaxAdjApp.customerID, Equal<Current<ARPPDTaxAdjustmentParameters.customerID>>>>();
			}

			foreach (PendingPPDARTaxAdjApp res in select.Select())
			{
				yield return res;
			}

			Filter.Cache.IsDirty = false;
		}

		protected virtual void ARPPDTaxAdjustmentParameters_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ARPPDTaxAdjustmentParameters filter = (ARPPDTaxAdjustmentParameters)e.Row;
			if (filter == null) return;

			ARSetup setup = arsetup.Current;
			Applications.SetProcessDelegate(list => CreatePPDTaxAdjustments(sender, filter, setup, list));

			bool generateOnePerCustomer = filter.GenerateOnePerCustomer == true;
			PXUIFieldAttribute.SetEnabled<ARPPDTaxAdjustmentParameters.taxAdjustmentDate>(sender, filter, generateOnePerCustomer);
			PXUIFieldAttribute.SetEnabled<ARPPDTaxAdjustmentParameters.finPeriodID>(sender, filter, generateOnePerCustomer);
			PXUIFieldAttribute.SetRequired<ARPPDTaxAdjustmentParameters.taxAdjustmentDate>(sender, generateOnePerCustomer);
			PXUIFieldAttribute.SetRequired<ARPPDTaxAdjustmentParameters.finPeriodID>(sender, generateOnePerCustomer);
		}

		public virtual void ARPPDTaxAdjustmentParameters_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			ARPPDTaxAdjustmentParameters row = (ARPPDTaxAdjustmentParameters)e.Row;
			ARPPDTaxAdjustmentParameters oldRow = (ARPPDTaxAdjustmentParameters)e.OldRow;
			if (row == null || oldRow == null) return;

			if (!sender.ObjectsEqual<ARPPDTaxAdjustmentParameters.applicationDate, ARPPDTaxAdjustmentParameters.branchID, ARPPDTaxAdjustmentParameters.customerID>(oldRow, row))
			{
				Applications.Cache.Clear();
				Applications.Cache.ClearQueryCacheObsolete();
			}
		}

		protected virtual void ARPPDTaxAdjustmentParameters_GenerateOnePerCustomer_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			ARPPDTaxAdjustmentParameters filter = (ARPPDTaxAdjustmentParameters)e.Row;
			if (filter == null) return;

			if (filter.GenerateOnePerCustomer != true && (bool?)e.OldValue == true)
			{
				filter.TaxAdjustmentDate = null;
				filter.FinPeriodID = null;
				
				sender.SetValuePending<ARPPDTaxAdjustmentParameters.taxAdjustmentDate>(filter, null);
				sender.SetValuePending<ARPPDTaxAdjustmentParameters.finPeriodID>(filter, null);
			}
		}

		public static void CreatePPDTaxAdjustments(PXCache cache, ARPPDTaxAdjustmentParameters filter, ARSetup setup, List<PendingPPDARTaxAdjApp> docs)
		{
			CreatePPDCreditMemos(cache, filter, setup, docs);
			CreatePPDDebitMemos(cache, filter, setup, docs);
		}

		public static void CreatePPDCreditMemos(PXCache cache, ARPPDTaxAdjustmentParameters filter, ARSetup setup, List<PendingPPDARTaxAdjApp> docs)
		{
			docs = docs.Where(doc => doc.InvDocType != ARDocType.CreditMemo).ToList();
			ProcessAdjustmentDocuments(cache, filter, setup, docs, ARDocType.CreditMemo);
		}

		public static void CreatePPDDebitMemos(PXCache cache, ARPPDTaxAdjustmentParameters filter, ARSetup setup, List<PendingPPDARTaxAdjApp> docs)
		{
			docs = docs.Where(doc => doc.InvDocType == ARDocType.CreditMemo).ToList();
			ProcessAdjustmentDocuments(cache, filter, setup, docs, ARDocType.DebitMemo);
		}

		private static void ProcessAdjustmentDocuments(PXCache cache, ARPPDTaxAdjustmentParameters filter, ARSetup setup, List<PendingPPDARTaxAdjApp> docs, string adjustingDocType)
		{
			int i = 0;
			bool failed = false;

			List<ARRegister> toRelease = new List<ARRegister>();
			ARInvoiceEntry ie = PXGraph.CreateInstance<ARInvoiceEntry>();
			ie.ARSetup.Current = setup;
			if (filter.GenerateOnePerCustomer == true)
			{
				if (filter.TaxAdjustmentDate == null)
					throw new PXSetPropertyException(CR.Messages.EmptyValueErrorFormat,
						PXUIFieldAttribute.GetDisplayName<ARPPDTaxAdjustmentParameters.taxAdjustmentDate>(cache));

				if (filter.FinPeriodID == null)
					throw new PXSetPropertyException(CR.Messages.EmptyValueErrorFormat,
						PXUIFieldAttribute.GetDisplayName<ARPPDTaxAdjustmentParameters.finPeriodID>(cache));

				Dictionary<PPDApplicationKey, List<PendingPPDARTaxAdjApp>> dict = new Dictionary<PPDApplicationKey, List<PendingPPDARTaxAdjApp>>();
				foreach (PendingPPDARTaxAdjApp doc in docs)
				{
					CurrencyInfo info = ie.FindImplementation<IPXCurrencyHelper>().GetCurrencyInfo(doc.InvCuryInfoID);

					PPDApplicationKey key = new PPDApplicationKey();
					doc.Index = i++;
					key.BranchID = doc.AdjdBranchID;
					key.BAccountID = doc.AdjdCustomerID;
					key.LocationID = doc.InvCustomerLocationID;
					key.CuryID = info.CuryID;
					key.CuryRate = info.CuryRate;
					key.AccountID = doc.AdjdARAcct;
					key.SubID = doc.AdjdARSub;
					key.TaxZoneID = doc.InvTaxZoneID;

					List<PendingPPDARTaxAdjApp> list;
					if (!dict.TryGetValue(key, out list))
					{
						dict[key] = list = new List<PendingPPDARTaxAdjApp>();
					}

					list.Add(doc);
				}

				foreach (List<PendingPPDARTaxAdjApp> list in dict.Values)
				{
					ARInvoice invoice = CreatePPDTaxAdjustmentDocument(ie, filter, list, adjustingDocType);
					if (invoice != null) { toRelease.Add(invoice); }
					else failed = true;
				}
			}
			else foreach (PendingPPDARTaxAdjApp doc in docs)
				{
					List<PendingPPDARTaxAdjApp> list = new List<PendingPPDARTaxAdjApp>(1);
					doc.Index = i++;
					list.Add(doc);

					ARInvoice invoice = CreatePPDTaxAdjustmentDocument(ie, filter, list, adjustingDocType);
					if (invoice != null) { toRelease.Add(invoice); }
					else failed = true;
				}

			if (setup.AutoReleasePPDCreditMemo == true && toRelease.Count > 0)
			{
				using (new PXTimeStampScope(null))
				{
					ARDocumentRelease.ReleaseDoc(toRelease, true);
				}
			}

			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
		}

		private static ARInvoice CreatePPDTaxAdjustmentDocument(ARInvoiceEntry ie, ARPPDTaxAdjustmentParameters filter, List<PendingPPDARTaxAdjApp> list, string docType)
		{
			int index = 0;
			ARInvoice invoice;

			try
			{
				ie.Clear(PXClearOption.ClearAll);
				PXUIFieldAttribute.SetError(ie.Document.Cache, null, null, null);

				if (docType == ARDocType.CreditMemo)
				{
					invoice = ie.CreatePPDCreditMemo(filter, list, ref index);
				}
				else
				{
					invoice = ie.CreatePPDDebitMemo(filter, list, ref index);
				}

				PXProcessing<PendingPPDARTaxAdjApp>.SetInfo(index, ActionsMessages.RecordProcessed);
			}
			catch (Exception e)
			{
				PXProcessing<PendingPPDARTaxAdjApp>.SetError(index, e);
				invoice = null;
			}

			return invoice;
		}

		public static bool CalculateDiscountedTaxes(PXCache cache, ARTaxTran artax, decimal cashDiscPercent)
		{
			bool? result = null;
			object value = null;

			IBqlCreator whereTaxable = (IBqlCreator)Activator.CreateInstance(typeof(WhereTaxable<Required<ARTaxTran.taxID>>));
			whereTaxable.Verify(cache, artax, new List<object> { artax.TaxID }, ref result, ref value);
			
			if(cashDiscPercent == 0m)
			{
				artax.CuryDiscountedTaxableAmt = artax.CuryTaxableAmt;
				artax.CuryDiscountedPrice = artax.CuryTaxAmt;
			}
			else
			{
				IPXCurrencyHelper helper = cache.Graph.FindImplementation<IPXCurrencyHelper>();
				artax.CuryDiscountedTaxableAmt = helper.RoundCury((decimal)(artax.CuryTaxableAmt * (1m - cashDiscPercent)));
				artax.CuryDiscountedPrice = helper.RoundCury((decimal)(artax.TaxRate / 100m * artax.CuryDiscountedTaxableAmt));
			}

			return result == true;
		}
	}
}
