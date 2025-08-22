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

using PX.ACHPlugInBase;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.Common;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Data.PXGenericInqGrph;


namespace PX.Objects.PM
{
	public class ProformaLinkMaint : PXGraph<ProformaLinkMaint>
	{
		#region DAC Attributes Override
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Pro Forma Invoice Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void _(Events.CacheAttached<PMProforma.refNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Pro Forma Invoice Line Nbr.")]
		protected virtual void _(Events.CacheAttached<PMProformaLine.lineNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Pro Forma Invoice Line Nbr.")]
		protected virtual void _(Events.CacheAttached<PMTran.proformaLineNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "AP Doc. Type")]
		[APDocType.List()]
		protected virtual void _(Events.CacheAttached<PMTran.origTranType> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "AP Doc. Nbr.")]
		protected virtual void _(Events.CacheAttached<PMTran.origRefNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "AP Doc. Line Nbr.")]
		protected virtual void _(Events.CacheAttached<PMTran.origLineNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Vendor")]
		protected virtual void _(Events.CacheAttached<PMTran.bAccountID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Financial Period")]
		protected virtual void _(Events.CacheAttached<PMTran.finPeriodID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXSelector(typeof(Search<PMRegister.refNbr>))]
		protected virtual void _(Events.CacheAttached<PMTran.refNbr> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Account Group", Visible = false)]
		protected virtual void _(Events.CacheAttached<PMTran.accountGroupID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Quantity", Visible = false)]
		protected virtual void _(Events.CacheAttached<PMTran.qty> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Billable Quantity", Visible = false)]
		protected virtual void _(Events.CacheAttached<PMTran.billableQty> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Unit Rate", Visible = false)]
		protected virtual void _(Events.CacheAttached<PMTran.tranCuryUnitRate> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Billable", Visible = false)]
		protected virtual void _(Events.CacheAttached<PMTran.billable> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "Branch", Visible = false)]
		protected virtual void _(Events.CacheAttached<PMTran.branchID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = "UOM", Visible = false)]
		protected virtual void _(Events.CacheAttached<PMTran.uOM> e) { }

		[PXBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		protected virtual void _(Events.CacheAttached<PMCostCode.isProjectOverride> e)
		{
		}

		#endregion

		public PXFilter<ProformaLinkFilter> Filter;
		public PXCancel<ProformaLinkFilter> Cancel;
		public PXSave<ProformaLinkFilter> Save;

		[PXViewName(Messages.Transaction)]
		public PXSelectJoin<PMTran,
			InnerJoin<APRegister, On<APRegister.docType, Equal<PMTran.origTranType>,
				And<APRegister.refNbr, Equal<PMTran.origRefNbr>,
				And<APRegister.batchNbr, Equal<PMTran.batchNbr>>>>>,
			Where<PMTran.proformaRefNbr, Equal<Current<ProformaLinkFilter.refNbr>>,
			And<Where<Current<ProformaLinkFilter.lineNbr>, IsNull, Or<PMTran.proformaLineNbr, Equal<Current<ProformaLinkFilter.lineNbr>>>>>>,
			OrderBy<Asc<PMTran.proformaLineNbr>>> Transactions;

		public PXFilter<TranFilter> AvailableTransactionsFilter;

		public PXSetupOptional<PMProforma, Where<PMProforma.refNbr, Equal<Current<ProformaLinkFilter.refNbr>>,
			And<PMProforma.corrected, NotEqual<True>>>> Proforma;

		[PXHidden]
		[PXFilterable]
		[PXCopyPasteHiddenView]
		public PXSelectJoin<PMTran,
			InnerJoin<APRegister, On<APRegister.docType, Equal<PMTran.origTranType>,
				And<APRegister.refNbr, Equal<PMTran.origRefNbr>,
				And<APRegister.batchNbr, Equal<PMTran.batchNbr>,
				And<APRegister.docType, NotEqual<APDocType.prepayment>,
				And<APRegister.docType, NotEqual<APDocType.refund>,
				And<APRegister.docType, NotEqual<APDocType.voidRefund>,
				And<APRegister.docType, NotEqual<APDocType.prepaymentRequest>,
				And<APRegister.docType, NotEqual<APDocType.check>,
				And<APRegister.docType, NotEqual<APDocType.voidCheck>>>>>>>>>>>,
			Where<PMTran.projectID, Equal<Current<ProformaLinkFilter.projectID>>,
			And<PMTran.proformaRefNbr, IsNull,
			And<PMTran.origModule, Equal<BatchModule.moduleAP>,
			And2<Where<Current<TranFilter.vendorID>, IsNull, Or<PMTran.bAccountID, Equal<Current<TranFilter.vendorID>>>>,
			And2<Where<Current<TranFilter.projectTaskID>, IsNull, Or<PMTran.taskID, Equal<Current<TranFilter.projectTaskID>>>>,
			And2<Where<Current<TranFilter.costCodeID>, IsNull, Or<PMTran.costCodeID, Equal<Current<TranFilter.costCodeID>>>>,
			And2<Where<Current<TranFilter.aPDocType>, IsNull, Or<PMTran.origTranType, Equal<Current<TranFilter.aPDocType>>>>,
			And2<Where<Current<TranFilter.aPRefNbr>, IsNull, Or<PMTran.origRefNbr, Equal<Current<TranFilter.aPRefNbr>>>>,
			And<PMTran.date, Between<Current<TranFilter.dateFrom>, Current<TranFilter.dateTo>>>>>>>>>>>,
			OrderBy<Asc<PMTran.date>>> AvailableTransactions;

		
		public PXAction<ProformaLinkFilter> addTransactions;
		[PXUIField(DisplayName = "Add Transactions", MapEnableRights = PXCacheRights.Insert)]
		[PXButton]
		public IEnumerable AddTransactions(PXAdapter adapter)
		{
			if (AvailableTransactions.View.AskExt() == WebDialogResult.OK)
			{
				AddSelectedLines();
			}

			return adapter.Get();
		}

		public PXAction<ProformaLinkFilter> appendTransactions;
		[PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Insert)]
		[PXButton()]
		public IEnumerable AppendTransactions(PXAdapter adapter)
		{
			AddSelectedLines();

			return adapter.Get();
		}

		public PXAction<ProformaLinkFilter> removeTransaction;
		[PXUIField(DisplayName = "Remove Transactions", MapEnableRights = PXCacheRights.Delete)]
		[PXButton()]
		public IEnumerable RemoveTransaction(PXAdapter adapter)
		{			
			if (Transactions.Current != null && Transactions.Current.Billed != true)
			{
				Transactions.Current.ProformaRefNbr = null;
				Transactions.Current.ProformaLineNbr = null;
				Transactions.Current.Selected = false;
				Transactions.UpdateCurrent();
			}

			return adapter.Get();
		}


		public PXAction<ProformaLinkFilter> viewDocument;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = false)]
		[PXButton]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			RegisterEntry graph = CreateInstance<RegisterEntry>();
			graph.Document.Current = graph.Document.Search<PMRegister.refNbr>(Transactions.Current.RefNbr, Transactions.Current.TranType);
			throw new PXRedirectRequiredException(graph, "") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
		}

		public PXAction<ProformaLinkFilter> viewBill;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = false)]
		[PXButton]
		public virtual IEnumerable ViewBill(PXAdapter adapter)
		{
			APInvoiceEntry graph = CreateInstance<APInvoiceEntry>();
			graph.Document.Current = graph.Document.Search<APInvoice.refNbr>(Transactions.Current.OrigRefNbr, Transactions.Current.OrigTranType);
			throw new PXRedirectRequiredException(graph, "") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
		}

		public PXAction<TranFilter> viewVendor;
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ViewVendor(PXAdapter adapter)
		{
			VendorMaint graph = CreateInstance<VendorMaint>();
			graph.BAccount.Current = PXSelect<VendorR, Where<VendorR.bAccountID, Equal<Current<PMTran.bAccountID>>>>.Select(this);
			throw new PXRedirectRequiredException(graph, true, "") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
		}


		protected virtual void _(Events.RowSelected<ProformaLinkFilter> e)
		{
			if (e.Row != null)
			{
				bool isEditable = e.Row.IsEditable == true;
				bool isTimeMaterial = e.Row.LineType == "T";

				addTransactions.SetEnabled(isEditable && !isTimeMaterial);
				removeTransaction.SetEnabled(isEditable && !isTimeMaterial);

				PXUIFieldAttribute.SetEnabled<ProformaLinkFilter.refNbr>(e.Cache, e.Row, e.Row.ProjectID != null);
				PXUIFieldAttribute.SetEnabled<ProformaLinkFilter.lineNbr>(e.Cache, e.Row, e.Row.RefNbr != null);
			}
		}

		protected virtual void _(Events.FieldUpdated<ProformaLinkFilter, ProformaLinkFilter.projectID> e)
		{
			e.Row.RefNbr = null;
			e.Row.LineNbr = null;
		}

		protected virtual void _(Events.FieldUpdated<ProformaLinkFilter, ProformaLinkFilter.refNbr> e)
		{
			e.Row.LineNbr = null;
		}

		protected virtual void _(Events.FieldVerifying<TranFilter, TranFilter.dateTo> e)
		{
			DateTime? date = e.NewValue as DateTime?;
			//if ( date != null && Proforma.Current != null && date.Value.Date > Proforma.Current.InvoiceDate )
			//{
			//	throw new PXSetPropertyException<TranFilter.dateTo>(Messages.ToDateGreaterThanProforma, Proforma.Current.InvoiceDate);
			//}
			if (date != null && e.Row.DateFrom != null && date.Value.Date < e.Row.DateFrom)
			{
				throw new PXSetPropertyException<TranFilter.dateTo>(Messages.ToDateLessThanFrom, e.Row.DateFrom);
			}
		}

		protected virtual void _(Events.FieldVerifying<TranFilter, TranFilter.dateFrom> e)
		{
			DateTime? date = e.NewValue as DateTime?;
			if (date != null && Proforma.Current != null && date.Value.Date > Proforma.Current.InvoiceDate)
			{
				throw new PXSetPropertyException<TranFilter.dateFrom>(Messages.ToDateGreaterThanProforma, Proforma.Current.InvoiceDate);
			}
		}

		protected virtual void _(Events.FieldUpdated<TranFilter, TranFilter.dateFrom> e)
		{
			DateTime? date = e.NewValue as DateTime?;
			if (e.Row.DateFrom != null && e.Row.DateTo != null &&
				AvailableTransactionsFilter.Current.DateTo.Value.Date < e.Row.DateFrom.Value.Date)
			{
				AvailableTransactionsFilter.Current.DateTo = e.Row.DateFrom;
			}
		}

		protected virtual void AddSelectedLines()
		{
			foreach (PMTran tran in AvailableTransactions.Select())
			{
				if (tran.Selected == true)
				{
					tran.ProformaRefNbr = Filter.Current.RefNbr;
					if (Filter.Current.LineNbr != null)
					{
						tran.ProformaLineNbr = Filter.Current.LineNbr;
					}
					Transactions.UpdateCurrent();
				}
			}
		}

		/// <summary>
		/// Filter used in the header of the form to filter out the proforma and any line of the proforma for the selected Project.
		/// </summary>
		[PXCacheName(Messages.ProformaLink)]
		[Serializable]
		public class ProformaLinkFilter : PXBqlTable, IBqlTable
		{
			#region ProjectID
			public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

			/// <summary>The project ID.</summary>
			[PXDefault]
			[Project(typeof(Where<PMProject.baseType, Equal<CT.CTPRType.project>, And<PMProject.nonProject, Equal<False>>>), WarnIfCompleted = false)]
			public virtual Int32? ProjectID
			{
				get;
				set;
			}
			#endregion
			#region RefNbr
			public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr>
			{
			}

			/// <summary>
			/// The reference number of the pro forma invoice.
			/// </summary>
			[PXDBString(PMProforma.refNbr.Length, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
			[PXDefault]
			[PXSelector(typeof(Search<PMProforma.refNbr, Where<PMProforma.projectID, Equal<Current<projectID>>,
				And<PMProforma.corrected, NotEqual<True>>>,
				OrderBy<Desc<PMProforma.refNbr>>>), DescriptionField = typeof(PMProforma.description))]
			[PXUIField(DisplayName = "Pro Forma Invoice Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual string RefNbr
			{
				get;
				set;
			}
			#endregion
			#region LineNbr
			public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

			/// <summary>
			/// The original sequence number of the line among all the pro forma invoice lines.
			/// </summary>
			/// <remarks>The sequence of line numbers of the pro forma invoice lines belonging to a single document can include gaps.</remarks>
			[PXSelector(typeof(Search<PMProformaLine.lineNbr, Where<PMProformaLine.refNbr, Equal<Current<refNbr>>,
				And<PMProformaLine.corrected, NotEqual<True>>>>), typeof(PMProformaLine.lineNbr)
				, typeof(PMProformaLine.description)
				, typeof(PMProformaLine.taskID)
				, typeof(PMProformaLine.costCodeID)
				, typeof(PMProformaLine.inventoryID)
				, typeof(PMProformaLine.uOM)
				, typeof(PMProformaLine.qty)
				, typeof(PMProformaLine.curyLineTotal), DescriptionField = typeof(PMProformaLine.description)
				)]
			[PXUIField(DisplayName = "Pro Forma Invoice Line Nbr.")]
			[PXDBInt()]
			public virtual Int32? LineNbr
			{
				get; set;
			}
			#endregion

			#region IsEditable
			public abstract class isEditable : PX.Data.BQL.BqlBool.Field<isEditable>
			{
			}

			/// <summary>
			/// Returns true if Proforma is on hold. Transactions can be linked or unlinked only for a Proforma with an 'On Hold' status.
			/// </summary>
			[PXFormula(typeof(Selector<ProformaLinkFilter.refNbr, PMProforma.hold>))]
			[PXBool]
			public virtual bool? IsEditable
			{
				get;
				set;
			}
			#endregion
			#region LineType
			public abstract class lineType : PX.Data.BQL.BqlString.Field<lineType>
			{
			}

			/// <summary>
			/// Returns the line type of the selected line. Can be either 'P' for Progressive or 'T' for Transactional.
			/// </summary>
			[PXFormula(typeof(Selector<ProformaLinkFilter.lineNbr, PMProformaLine.type>))]
			[PXString]
			public virtual string LineType
			{
				get;
				set;
			}
			#endregion
		}

		[PXHidden]
		[Serializable]
		public class TranFilter : PXBqlTable, IBqlTable
		{
			#region ProjectID
			public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
			[PXDefault(typeof(ProformaLinkFilter.projectID))]
			[Project(typeof(Where<PMProject.baseType, Equal<CT.CTPRType.project>, And<PMProject.nonProject, Equal<False>>>), WarnIfCompleted = false, Enabled = false)]
			public virtual Int32? ProjectID
			{
				get;
				set;
			}
			#endregion
			#region ProjectTaskID
			public abstract class projectTaskID : PX.Data.BQL.BqlInt.Field<projectTaskID> { }
			[ProjectTask(typeof(TranFilter.projectID))]
			public virtual Int32? ProjectTaskID
			{
				get;
				set;
			}
			#endregion
			#region AccountGroupID
			public abstract class accountGroupID : PX.Data.BQL.BqlInt.Field<accountGroupID> { }
			[AccountGroup(typeof(Where<Match<PMAccountGroup, Current<AccessInfo.userName>>>))]
			public virtual int? AccountGroupID
			{
				get;
				set;
			}
			#endregion
			#region CostCodeID
			public abstract class costCodeID : PX.Data.BQL.BqlInt.Field<costCodeID> { }
			[CostCode(Filterable = false, SkipVerification = true)]
			public virtual Int32? CostCodeID
			{
				get;
				set;
			}
			#endregion
			
			#region DateFrom
			public abstract class dateFrom : PX.Data.BQL.BqlDateTime.Field<dateFrom> { }
			[PXDefault(typeof(Search<PMProject.startDate, Where<PMProject.contractID, Equal<Current<projectID>>>>))]
			[PXDBDate()]
			[PXUIField(DisplayName = "From Date", Required = false)]
			public virtual DateTime? DateFrom
			{
				get;
				set;
			}
			#endregion
			#region DateTo
			public abstract class dateTo : PX.Data.BQL.BqlDateTime.Field<dateTo> { }
			[PXDefault(typeof(Search<PMProforma.invoiceDate, Where<PMProforma.refNbr, Equal<Current<ProformaLinkFilter.refNbr>>>>))]
			[PXDBDate()]
			[PXUIField(DisplayName = "To Date", Required = false)]
			public virtual DateTime? DateTo
			{
				get;
				set;
			}
			#endregion
			#region VendorID
			public abstract class vendorID : BqlInt.Field<vendorID> { }
			[POVendor(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName))]
			public virtual int? VendorID { get; set; }
			#endregion
			#region APDocType
			public abstract class aPDocType : PX.Data.BQL.BqlString.Field<aPDocType> { }

			[PXDBString(3, IsFixed = true)]
			[PXUIField(DisplayName = "AP Doc. Type")]
			[DocTypeList]
			public virtual String APDocType
			{
				get;
				set;
			}
			#endregion
			#region APRefNbr
			public abstract class aPRefNbr : PX.Data.BQL.BqlString.Field<aPRefNbr> { }

			[PXDBString(15, IsUnicode = true)]
			[PXUIField(DisplayName = "AP Doc. Nbr.")]
			[PXSelector(typeof(Search<APInvoice.refNbr,
				Where2<Where<APInvoice.docType, Equal<Current<aPDocType>>, Or<Current<aPDocType>, IsNull>>,
				And<Where<Current<vendorID>, IsNull, Or<APInvoice.vendorID, Equal<Current<vendorID>>>>>>>))]
			public virtual String APRefNbr
			{
				get;
				set;
			}
			#endregion
			
		}
				
		public class DocTypeListAttribute : LabelListAttribute
		{
			private static readonly IEnumerable<ValueLabelPair> proformaSupported = new ValueLabelList
			{
				{ APDocType.Invoice, AP.Messages.Invoice },
				{ APDocType.CreditAdj, AP.Messages.CreditAdj},
				{ APDocType.DebitAdj, AP.Messages.DebitAdj},
				{ APDocType.QuickCheck, AP.Messages.QuickCheck },
				{ APDocType.VoidQuickCheck, AP.Messages.VoidQuickCheck },
				{ APDocType.CashReturn, AP.Messages.CashReturn },
			};


			public DocTypeListAttribute() : base(proformaSupported)
			{ }
		}
	}

	
}
