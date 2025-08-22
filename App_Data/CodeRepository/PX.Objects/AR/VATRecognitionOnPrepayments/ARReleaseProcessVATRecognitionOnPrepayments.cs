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
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.Common.Exceptions;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.SO;
using PX.Objects.TX;
using System;
using System.Collections.Generic;
using System.Linq;
using static PX.Objects.AR.ARReleaseProcess;

namespace PX.Objects.AR
{
	public class ARReleaseProcessVATRecognitionOnPrepayments : PXGraphExtension<ARReleaseProcess>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>();
		}

		public delegate void PerformBasicReleaseChecksDelegate(PXGraph selectGraph, ARRegister document);
		[PXOverride]
		public void PerformBasicReleaseChecks(PXGraph selectGraph, ARRegister document, PerformBasicReleaseChecksDelegate baseMethod)
		{
			baseMethod(selectGraph, document);

			ARInvoice documentAsInvoice = document as ARInvoice;
			if (documentAsInvoice?.DocType == ARInvoiceType.PrepaymentInvoice)
			{
				bool isMultipleInstallment = PXSelect<Terms,
					Where<Terms.termsID, Equal<Required<ARInvoice.termsID>>,
						And<Terms.installmentType, Equal<TermsInstallmentType.multiple>>>>
					.SelectSingleBound(selectGraph, null, new object[] { documentAsInvoice.TermsID })
					.Any();

				if (isMultipleInstallment)
				{
					throw new ReleaseException(Messages.CannotReleasePrepaymentInvoiceWithMultipleInstallmentCreditTerms);
				}

				if (documentAsInvoice.PrepaymentAccountID == null)
				{
					throw new ReleaseException(AR.Messages.CannotReleasePrepaymentInvoiceWithEmptyPrepaymentAccount);
				}

				if (PXAccess.FeatureInstalled<FeaturesSet.subAccount>() && documentAsInvoice.PrepaymentSubID == null)
				{
					throw new ReleaseException(AR.Messages.CannotReleasePrepaymentInvoiceWithEmptyPrepaymentSubaccount);
				}
			}
		}

		public delegate void SetClosedPeriodsFromLatestApplicationDelegate(ARRegister doc);
		[PXOverride]
		public void SetClosedPeriodsFromLatestApplication(ARRegister doc, SetClosedPeriodsFromLatestApplicationDelegate baseMethod)
		{
			if (doc.IsPrepaymentInvoiceDocument() != true)
				baseMethod.Invoke(doc);
			else
			{
				ARTranPost lastPeriod = PXSelect<ARTranPost,
						Where<ARTranPost.docType, Equal<Required<ARTranPost.docType>>,
							And<ARTranPost.refNbr, Equal<Required<ARTranPost.refNbr>>,
							And<ARTranPost.voidAdjNbr, IsNull,
							And<NotExists<Select<ARTranPostAlias,
											Where<ARTranPostAlias.voidAdjNbr, Equal<ARTranPost.adjNbr>,
												And<ARTranPostAlias.refNbr, Equal<ARTranPost.refNbr>,
												And<ARTranPostAlias.docType, Equal<ARTranPost.docType>,
												And<ARTranPostAlias.lineNbr, Equal<ARTranPost.lineNbr>,
												And<ARTranPostAlias.sourceRefNbr, Equal<ARTranPost.sourceRefNbr>,
												And<ARTranPostAlias.sourceDocType, Equal<ARTranPost.sourceDocType>>>>>>>>>>>>>,
						OrderBy<Desc<ARTranPost.tranPeriodID,
							Desc<ARTranPost.iD>>>>
					.SelectSingleBound(Base, new object[] { }, doc.DocType, doc.RefNbr);

				ARTranPost lastDate = PXSelect<ARTranPost,
						Where<ARTranPost.docType, Equal<Required<ARTranPost.docType>>,
							And<ARTranPost.refNbr, Equal<Required<ARTranPost.refNbr>>,
							And<ARTranPost.voidAdjNbr, IsNull,
							And<NotExists<Select<ARTranPostAlias,
											Where<ARTranPostAlias.voidAdjNbr, Equal<ARTranPost.adjNbr>,
												And<ARTranPostAlias.refNbr, Equal<ARTranPost.refNbr>,
												And<ARTranPostAlias.docType, Equal<ARTranPost.docType>,
												And<ARTranPostAlias.lineNbr, Equal<ARTranPost.lineNbr>,
												And<ARTranPostAlias.sourceRefNbr, Equal<ARTranPost.sourceRefNbr>,
												And<ARTranPostAlias.sourceDocType, Equal<ARTranPost.sourceDocType>>>>>>>>>>>>>,
						OrderBy<Desc<ARTranPost.docDate,
							Desc<ARTranPost.iD>>>>
					.SelectSingleBound(Base, new object[] { }, doc.DocType, doc.RefNbr);
				doc.ClosedTranPeriodID = GL.FinPeriods.FinPeriodUtils.Max(lastPeriod?.TranPeriodID, doc.TranPeriodID);
				FinPeriodIDAttribute.SetPeriodsByMaster<ARRegister.closedFinPeriodID>(
					Base.ARDocument.Cache,
					doc,
					doc.ClosedTranPeriodID);

				doc.ClosedDate = GL.FinPeriods.FinPeriodUtils.Max(lastDate?.DocDate, doc.DocDate);
			}
		}

		public delegate void AfterSVATConversionHistoryInsertedDelegate(JournalEntry je, ARAdjust adj, ARInvoice adjddoc, ARRegister adjgdoc,
																		SVATConversionHist docSVAT, SVATConversionHist adjSVAT);
		[PXOverride]
		public virtual void AfterSVATConversionHistoryInserted(JournalEntry je, ARAdjust adj, ARInvoice adjddoc, ARRegister adjgdoc,
																	SVATConversionHist docSVAT, SVATConversionHist adjSVAT,
																	AfterSVATConversionHistoryInsertedDelegate baseMethod)
		{
			if (adjddoc.IsPrepaymentInvoiceDocument() == true) //PPI to Payment application
			{
				bool isDebit = (adjddoc.DrCr == DrCr.Debit);
				Tax tax = Tax.PK.Find(Base, adjSVAT.TaxID);

				if (!adjgdoc.IsPrepaymentInvoiceDocumentReverse())
				{
					if (tax.OnARPrepaymentTaxAcctID == null)
					{
						throw new ReleaseException(AR.Messages.CannotReleasePrepaymentInvoiceWithEmptyPendingTaxPayableAccount, tax.TaxID);
					}

					if (PXAccess.FeatureInstalled<FeaturesSet.subAccount>() && tax.OnARPrepaymentTaxSubID == null)
					{
						throw new ReleaseException(AR.Messages.CannotReleasePrepaymentInvoiceWithEmptyPendingTaxPayableSubaccount, tax.TaxID);
					}
				}

				PXCache dummycache = je.Caches[typeof(TaxTran)];
				dummycache = je.Caches[typeof(SVATTaxTran)];
				je.Views.Caches.Add(typeof(SVATTaxTran));

				SVATConversionHistExt histSVAT = Common.Utilities.Clone<SVATConversionHist, SVATConversionHistExt>(Base, docSVAT);
				IEnumerable<PXResult<SVATTaxTran>> taxTranSelection = ProcessOutputSVAT.GetSVATTaxTrans(je, histSVAT, adj.AdjdRefNbr);
				SVATTaxTran svatTaxTran = taxTranSelection.First();

				if (adjgdoc.IsPrepaymentInvoiceDocumentReverse())
				{
					adjSVAT.Processed = true;
					adjSVAT = (SVATConversionHist)Base.SVATConversionHistory.Cache.Update(adjSVAT);
					decimal tranSign = ReportTaxProcess.GetMultByTranType(BatchModule.AR, adjgdoc.DocType);
					svatTaxTran.CuryAdjustedTaxableAmt += tranSign * adjSVAT.CuryTaxableAmt;
					svatTaxTran.AdjustedTaxableAmt += tranSign * adjSVAT.TaxableAmt;
					svatTaxTran.CuryAdjustedTaxAmt += tranSign * adjSVAT.CuryTaxAmt;
					svatTaxTran.AdjustedTaxAmt += tranSign * adjSVAT.TaxAmt;
					je.Caches[typeof(SVATTaxTran)].Update(svatTaxTran);
					je.Save.Press();
				}
				else
				{
					GLTran taxTran = new GLTran();
					taxTran.SummPost = Base.SummPost;
					taxTran.BranchID = adjSVAT.AdjdBranchID;
					taxTran.CuryInfoID = adjSVAT.CuryInfoID;
					taxTran.TranType = adjSVAT.AdjdDocType;
					taxTran.TranClass = GLTran.tranClass.Tax;
					taxTran.RefNbr = adjSVAT.AdjdRefNbr;
					taxTran.TranDate = adjSVAT.AdjdDocDate;
					taxTran.AccountID = isDebit ? tax.OnARPrepaymentTaxAcctID : tax.SalesTaxAcctID;
					taxTran.SubID = isDebit ? tax.OnARPrepaymentTaxSubID : tax.SalesTaxSubID;
					taxTran.TranDesc = tax.TaxID;
					taxTran.CuryDebitAmt = isDebit ? adjSVAT.CuryTaxAmt : 0m;
					taxTran.DebitAmt = isDebit ? adjSVAT.TaxAmt : 0m;
					taxTran.CuryCreditAmt = isDebit ? 0m : adjSVAT.CuryTaxAmt;
					taxTran.CreditAmt = isDebit ? 0m : adjSVAT.TaxAmt;
					taxTran.Released = true;
					taxTran.ReferenceID = adjddoc.CustomerID;

					Account taxAccount = Account.PK.Find(Base, tax.OnARPrepaymentTaxAcctID);
					Base.SetProjectAndTaxID(taxTran, taxAccount, adjddoc);

					Base.InsertInvoiceTaxTransaction(je, taxTran,
						new GLTranInsertionContext { ARRegisterRecord = adjddoc });

					GLTran pendingTaxTran = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(taxTran);
					pendingTaxTran.AccountID = isDebit ? tax.SalesTaxAcctID : tax.OnARPrepaymentTaxAcctID;
					pendingTaxTran.SubID = isDebit ? tax.SalesTaxSubID : tax.OnARPrepaymentTaxSubID;
					pendingTaxTran.CuryDebitAmt = isDebit ? 0m : adjSVAT.CuryTaxAmt;
					pendingTaxTran.DebitAmt = isDebit ? 0m : adjSVAT.TaxAmt;
					pendingTaxTran.CuryCreditAmt = isDebit ? adjSVAT.CuryTaxAmt : 0m;
					pendingTaxTran.CreditAmt = isDebit ? adjSVAT.TaxAmt : 0m;

					Base.InsertInvoiceTaxTransaction(je, pendingTaxTran,
						new GLTranInsertionContext { ARRegisterRecord = adjddoc });

					SVATTaxTran newtaxtran = PXCache<SVATTaxTran>.CreateCopy(svatTaxTran);
					newtaxtran.RecordID = null;
					newtaxtran.Module = BatchModule.AR;
					newtaxtran.TaxType = TaxType.Sales;
					newtaxtran.AccountID = tax.SalesTaxAcctID;
					newtaxtran.SubID = tax.SalesTaxSubID;
					newtaxtran.TaxInvoiceNbr = adjgdoc.RefNbr;
					newtaxtran.TaxPeriodID = null;
					newtaxtran.TranDate = adj.AdjgDocDate;
					newtaxtran.FinDate = null;
					newtaxtran.FinPeriodID = adj.AdjgFinPeriodID;

					decimal tranSign = ReportTaxProcess.GetMult(svatTaxTran) * ReportTaxProcess.GetMult(newtaxtran);
					newtaxtran.CuryTaxableAmt = tranSign * adjSVAT.CuryTaxableAmt;
					newtaxtran.TaxableAmt = tranSign * adjSVAT.TaxableAmt;
					newtaxtran.CuryTaxAmt = tranSign * adjSVAT.CuryTaxAmt;
					newtaxtran.TaxAmt = tranSign * adjSVAT.TaxAmt;

					PXCache taxtranCache = je.Caches[typeof(SVATTaxTran)];
					taxtranCache.Insert(newtaxtran);
				}
			}
			else if (adjgdoc.IsPrepaymentInvoiceDocument()) //PPI to Invoice application
			{
				adjSVAT.Processed = true;
				adjSVAT = (SVATConversionHist)Base.SVATConversionHistory.Cache.Update(adjSVAT);
				SVATConversionHistExt histSVAT = Common.Utilities.Clone<SVATConversionHist, SVATConversionHistExt>(Base, adjSVAT);
				PXResult<SVATTaxTran, CM.CurrencyInfo, Tax> taxTran = GetSVATTaxTransSales(je, histSVAT, adj.AdjdRefNbr);

				SVATTaxTran taxtran = taxTran;
				CM.CurrencyInfo info = taxTran;
				Tax tax = taxTran;

				CM.CurrencyInfo new_info_tax = PXCache<CM.CurrencyInfo>.CreateCopy(info);
				new_info_tax.CuryInfoID = null;
				new_info_tax.ModuleCode = BatchModule.AR;
				new_info_tax.BaseCalc = false;
				new_info_tax = je.currencyinfo.Insert(new_info_tax) ?? new_info_tax;

				bool drCr = (ReportTaxProcess.GetMult(taxtran) == 1m);

				#region reverse original transaction
				{
					GLTran tran = new GLTran();
					tran.Module = BatchModule.AR;
					tran.BranchID = taxtran.BranchID;
					tran.AccountID = taxtran.AccountID;
					tran.SubID = taxtran.SubID;
					tran.CuryDebitAmt = drCr ? adjSVAT.CuryTaxAmt : 0m;
					tran.DebitAmt = drCr ? adjSVAT.TaxAmt : 0m;
					tran.CuryCreditAmt = drCr ? 0m : adjSVAT.CuryTaxAmt;
					tran.CreditAmt = drCr ? 0m : adjSVAT.TaxAmt;
					tran.TranType = taxtran.TranType;
					tran.TranClass = GLTran.tranClass.Normal;
					tran.RefNbr = taxtran.RefNbr;
					tran.TranDesc = tax.TaxID;
					tran.TranPeriodID = adj.AdjgTranPeriodID;
					tran.FinPeriodID = adj.AdjgFinPeriodID;
					tran.TranDate = taxtran.TaxInvoiceDate;
					tran.CuryInfoID = new_info_tax.CuryInfoID;
					tran.ReferenceID = adj.CustomerID;
					tran.Released = true;

					je.GLTranModuleBatNbr.Insert(tran);
				}
				#endregion

				#region reclassify to VAT account
				{
					GLTran tran = new GLTran();
					tran.Module = BatchModule.AR;
					tran.BranchID = taxtran.BranchID;
					tran.AccountID = tax.OnARPrepaymentTaxAcctID;
					tran.SubID = tax.OnARPrepaymentTaxSubID;
					tran.CuryDebitAmt = drCr ? 0m : adjSVAT.CuryTaxAmt;
					tran.DebitAmt = drCr ? 0m : adjSVAT.TaxAmt;
					tran.CuryCreditAmt = drCr ? adjSVAT.CuryTaxAmt : 0m;
					tran.CreditAmt = drCr ? adjSVAT.TaxAmt : 0m;
					tran.TranType = taxtran.TranType;
					tran.TranClass = GLTran.tranClass.Normal;
					tran.RefNbr = taxtran.RefNbr;
					tran.TranDesc = tax.TaxID;
					tran.TranPeriodID = adj.AdjgTranPeriodID;
					tran.FinPeriodID = adj.AdjgFinPeriodID;
					tran.TranDate = taxtran.TaxInvoiceDate;
					tran.CuryInfoID = new_info_tax.CuryInfoID;
					tran.ReferenceID = adj.CustomerID;
					tran.Released = true;

					je.GLTranModuleBatNbr.Insert(tran);

					PXCache dummycache = je.Caches[typeof(TaxTran)];
					dummycache = je.Caches[typeof(SVATTaxTran)];
					je.Views.Caches.Add(typeof(SVATTaxTran));

					SVATTaxTran newtaxtran = PXCache<SVATTaxTran>.CreateCopy(taxtran);
					newtaxtran.RecordID = null;
					newtaxtran.Module = BatchModule.AR;
					newtaxtran.TaxType = TaxType.Sales;
					newtaxtran.AccountID = tax.SalesTaxAcctID;
					newtaxtran.SubID = tax.SalesTaxSubID;
					newtaxtran.TaxInvoiceNbr = adjgdoc.RefNbr;
					newtaxtran.TaxPeriodID = null;
					newtaxtran.TranDate = adj.AdjgDocDate;
					newtaxtran.FinDate = null;
					newtaxtran.FinPeriodID = adj.AdjgFinPeriodID;

					decimal tranSign = (-1m) * ReportTaxProcess.GetMult(taxtran) * ReportTaxProcess.GetMult(newtaxtran);
					newtaxtran.CuryTaxableAmt = tranSign * adjSVAT.CuryTaxableAmt;
					newtaxtran.TaxableAmt = tranSign * adjSVAT.TaxableAmt;
					newtaxtran.CuryTaxAmt = tranSign * adjSVAT.CuryTaxAmt;
					newtaxtran.TaxAmt = tranSign * adjSVAT.TaxAmt;

					PXCache taxtranCache = je.Caches[typeof(SVATTaxTran)];
					taxtranCache.Insert(newtaxtran);
				}
				#endregion

			}
		}

		public static PXResult<SVATTaxTran, CM.CurrencyInfo, Tax> GetSVATTaxTransSales(JournalEntry je, SVATConversionHistExt histSVAT, string masterInvoiceNbr)
		{
			PXSelectBase<SVATTaxTran> select = ProcessOutputSVAT.GetCommandForSVATTaxTrans(je);
			select.WhereAnd<Where<SVATTaxTran.taxType, Equal<TX.TaxType.sales>>>();

			return select.Select(histSVAT.Module,
								histSVAT.VendorID,
								histSVAT.VendorID,
								histSVAT.AdjdDocType,
								histSVAT.AdjdRefNbr,
								masterInvoiceNbr,
								histSVAT.TaxID)
						.AsEnumerable()
						.Cast<PXResult<SVATTaxTran, CM.CurrencyInfo, Tax>>()
						.First();
		}

		public delegate SVATConversionHist ProcessLastSVATRecordDelegate(ARRegister adjddoc, SVATConversionHist docSVAT, SVATConversionHist adjSVAT, decimal percent);
		[PXOverride]
		public virtual SVATConversionHist ProcessLastSVATRecord(ARRegister adjddoc, SVATConversionHist docSVAT, SVATConversionHist adjSVAT, decimal percent,
															ProcessLastSVATRecordDelegate baseMethod)
		{
			bool isPartialApplication = percent != 1m;

			adjSVAT.CuryTaxableAmt = docSVAT.CuryTaxableAmt;
			adjSVAT.TaxableAmt = docSVAT.TaxableAmt;
			adjSVAT.CuryTaxAmt = docSVAT.CuryTaxAmt;
			adjSVAT.TaxAmt = docSVAT.TaxAmt;

			if (isPartialApplication)
			{
				List<object> parameters = new List<object> { docSVAT.AdjdDocType, docSVAT.AdjdRefNbr, docSVAT.TaxID };
				BqlCommand select = Base.SVATRecognitionRecords.View.BqlSelect;

				bool? isPPItoInvoiceApplication = adjddoc.IsPrepaymentInvoiceDocument() == true && adjddoc.PendingPayment == false;
				if (isPPItoInvoiceApplication == true)
				{
					select = select.WhereAnd<Where<SVATConversionHist.processed.IsEqual<@P.AsBool>>>();
					parameters.Add(true);
				}
				foreach (SVATConversionHist row in select.Select<SVATConversionHist>(Base, false, parameters.ToArray()))
				{
					adjSVAT.CuryTaxableAmt -= (row.CuryTaxableAmt ?? 0m);
					adjSVAT.TaxableAmt -= (row.TaxableAmt ?? 0m);
					adjSVAT.CuryTaxAmt -= (row.CuryTaxAmt ?? 0m);
					adjSVAT.TaxAmt -= (row.TaxAmt ?? 0m);
				}
			}

			adjSVAT.CuryUnrecognizedTaxAmt = adjSVAT.CuryTaxAmt;
			adjSVAT.UnrecognizedTaxAmt = adjSVAT.TaxAmt;

			return adjSVAT;
		}

		[PXOverride]
		public virtual void ProcessPostponedFlags(Action baseProcessPostponedFlags)
		{
			bool isNeedSavePayment = false;
			bool isNeedSaveInvoice = false;
			foreach (ARAdjust item in Base.ARAdjust_AdjgDocType_RefNbr_CustomerID.Cache.Cached.Cast<ARAdjust>())
			{
				if (item.AdjdDocType != ARDocType.PrepaymentInvoice)
					continue;

				//PendingPayment from database
				ARPayment origDoc = ARPayment.PK.Find(Base, item.AdjdDocType, item.AdjdRefNbr);

				//PendingPayment from cache
				ARRegister adjdDoc = new ARRegister { DocType = item.AdjdDocType, RefNbr = item.AdjdRefNbr };
				ARRegister register = Base.ARDocument.Locate(adjdDoc);

				if (origDoc == null || register == null)
					continue;

				Base.pe.Document.Cache.RaiseEventsOnFieldChanging<ARPayment.pendingPayment>(origDoc, register.PendingPayment);
				isNeedSavePayment = true;
			}
			foreach (ARRegister item in Base.Caches[typeof(ARRegister)].Cached.Cast<ARRegister>()
										.Where(p => p.PostponePendingPaymentFlag == true))
			{
				// we need the child caches to be initialized
				PXCache socache = Base.soAdjust.Cache;
				item.PendingPayment = false;
				Base.InvoiceEntryGraph.Document.Cache.RaiseEventsOnFieldChanging<ARRegister.pendingPayment>(item, true);
				item.PostponePendingPaymentFlag = false;
				isNeedSaveInvoice = true;
			}

			if (isNeedSavePayment)
			{
				Base.pe.Save.Press();
			}
			if (isNeedSaveInvoice)
			{
				Base.InvoiceEntryGraph.Save.Press();
			}

			baseProcessPostponedFlags();
		}

		[PXOverride]
		public virtual void CreditMemoProcessingBeforeSave(ARRegister ardoc)
		{
			ARInvoice reversingDoc = ardoc as ARInvoice;
			if (reversingDoc == null)
				return;

			if (reversingDoc.IsPrepaymentInvoiceDocumentReverse())
			{
				// Correct Applied To Order Amount (SOAdjust.AdjdAmt) on the Adjustments
				// Start correction from the latest application
				// Bite in the next order:
				// 1. Balance (SOAdjust.DocBal)
				// 2. Applied To Order (SOAdjust.AdjdAmt)
				// 3. Next application
				ARInvoiceEntryVATRecognitionOnPrepayments Base1 = Base.InvoiceEntryGraph.GetExtension<ARInvoiceEntryVATRecognitionOnPrepayments>();

				decimal reverseAmt = reversingDoc.CuryDocBal.Value;
				ARPayment origDoc = ARPayment.PK.Find(Base, reversingDoc.OrigDocType, reversingDoc.OrigRefNbr);

				SOOrderEntry sOOrderEntry = PXGraph.CreateInstance<SOOrderEntry>();

				foreach (PXResult<SOAdjust, CurrencyInfo> res in SelectFrom<SOAdjust>
										.LeftJoin<CurrencyInfo>
											.On<CurrencyInfo.curyInfoID.IsEqual<SOAdjust.adjdCuryInfoID>>
										.Where<SOAdjust.adjgDocType.IsEqual<@P.AsString.ASCII>
											.And<SOAdjust.adjgRefNbr.IsEqual<@P.AsString>>>
										.OrderBy<SOAdjust.recordID.Desc>
										.View
										.Select(Base, reversingDoc.OrigDocType, reversingDoc.OrigRefNbr))
				{
					SOAdjust soAdjust = res;
					CurrencyInfo inv_info = res;

					var paymentCopy = PXCache<ARPayment>.CreateCopy(origDoc);
					paymentCopy.CuryDocBal = origDoc.CuryOrigDocAmt;
					paymentCopy.DocBal = origDoc.OrigDocAmt;

					decimal CuryDocBal = sOOrderEntry.GetPaymentBalance(inv_info, paymentCopy, soAdjust);

					if (reverseAmt < (CuryDocBal - soAdjust.CuryAdjdAmt.Value))
						// do nothing because Balance will be calculated on the SOOrder screen
						break;
					else if (reverseAmt < soAdjust.CuryAdjdAmt)
					{
						// correct Applied To Order
						soAdjust.AdjAmt = CuryDocBal - reverseAmt;
						soAdjust.CuryAdjdAmt = soAdjust.AdjAmt;
						soAdjust.CuryAdjgAmt = soAdjust.AdjAmt;
						Base1.SOAdjustments.Cache.Update(soAdjust);
						break;
					}
					else
					{
						// move remaining reverse amount to the next application
						reverseAmt -= (CuryDocBal);
						soAdjust.CuryAdjdAmt = 0m;
						soAdjust.AdjAmt = 0m;
						Base1.SOAdjustments.Cache.SetValueExt<SOAdjust.curyAdjgAmt>(soAdjust, 0m);
						Base1.SOAdjustments.Cache.Update(soAdjust);
					}
				}
				Base1.SOAdjustments.Cache.Persist(PXDBOperation.Update);
				Base1.SOOrder_CustomerID_OrderType_RefNbr.Cache.Persist(PXDBOperation.Update);
			}
		}
	}
}
