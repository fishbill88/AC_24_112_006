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
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.FS.DAC;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using static PX.Objects.FS.ListField;
using static PX.Objects.FS.MessageHelper;
using System.Collections;
using static PX.Objects.FS.FSBillHistory;
using PX.Data.BQL.Fluent;

namespace PX.Objects.FS
{
    public class SM_ARInvoiceEntry : FSPostingBase<ARInvoiceEntry>, IInvoiceContractGraph
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
        }

		public static bool IsFSIntegrationEnabled(ARInvoice arInvoiceRow, FSxARInvoice fsxARInvoiceRow)
		{
			if (arInvoiceRow == null)
			{
				return false;
			}

			if (arInvoiceRow.CreatedByScreenID.Substring(0, 2) == "FS")
			{
				return true;
			}

			if (fsxARInvoiceRow.HasFSEquipmentInfo == true)
			{
				return true;
			}

			return false;
		}

		[PXOverride]
		public virtual IEnumerable ReverseInvoiceAndApplyToMemo(PXAdapter adapter, Func<PXAdapter, IEnumerable> baseMethod)
		{
			var hasBillHistory = SelectFrom<FSBillHistory>
				.Where<FSBillHistory.childEntityType.IsEqual<relatedDocumentType.Values.soInvoice>
				.And<FSBillHistory.childDocType.IsEqual<ARInvoice.docType.FromCurrent>>
				.And<FSBillHistory.childRefNbr.IsEqual<ARInvoice.refNbr.FromCurrent>>
				.And<FSBillHistory.srvOrdType.IsNotNull>>.View.Select(Base).Any();

			if (hasBillHistory)
				throw new PXException(TX.Error.CannotReverseARInvoiceFromServiceOrder);

			return baseMethod(adapter);
		}

		public static void SetUnpersistedFSInfo(PXCache cache, PXRowSelectingEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

			ARInvoice arInvoiceRow = (ARInvoice)e.Row;

			if (arInvoiceRow.CreatedByScreenID == null || arInvoiceRow.CreatedByScreenID.Substring(0, 2) == "FS")
			{
				// If the document was created by FS then the FS fields will be visible
				// regardless of whether there is equipment information
				return;
			}

			FSxARInvoice fsxARInvoiceRow = cache.GetExtension<FSxARInvoice>(arInvoiceRow);

			if (PXAccess.FeatureInstalled<FeaturesSet.equipmentManagementModule>() == false)
			{
				fsxARInvoiceRow.HasFSEquipmentInfo = false;
				return;
			}

			if (fsxARInvoiceRow.HasFSEquipmentInfo == null)
			{
				using (new PXConnectionScope())
				{
					PXResultset<InventoryItem> inventoryItemSet = PXSelectJoin<InventoryItem,
																  InnerJoin<ARTran,
																  On<
																	  ARTran.inventoryID, Equal<InventoryItem.inventoryID>,
																	  And<ARTran.tranType, Equal<ARDocType.invoice>>>,
																  InnerJoin<SOLineSplit,
																  On<
																	  SOLineSplit.orderType, Equal<ARTran.sOOrderType>,
																	  And<SOLineSplit.orderNbr, Equal<ARTran.sOOrderNbr>,
																	  And<SOLineSplit.lineNbr, Equal<ARTran.sOOrderLineNbr>>>>,
																  InnerJoin<SOLine,
																  On<
																	  SOLine.orderType, Equal<SOLineSplit.orderType>,
																	  And<SOLine.orderNbr, Equal<SOLineSplit.orderNbr>,
																	  And<SOLine.lineNbr, Equal<SOLineSplit.lineNbr>>>>,
																  LeftJoin<SOShipLineSplit,
																  On<
																	  SOShipLineSplit.origOrderType, Equal<SOLineSplit.orderType>,
																	  And<SOShipLineSplit.origOrderNbr, Equal<SOLineSplit.orderNbr>,
																	  And<SOShipLineSplit.origLineNbr, Equal<SOLineSplit.lineNbr>,
																	  And<SOShipLineSplit.origSplitLineNbr, Equal<SOLineSplit.splitLineNbr>>>>>>>>>,
																  Where<
																	  ARTran.tranType, Equal<Required<ARInvoice.docType>>,
																	  And<ARTran.refNbr, Equal<Required<ARInvoice.refNbr>>,
																	  And<FSxSOLine.equipmentAction, NotEqual<ListField_EquipmentAction.None>>>>>
																  .Select(cache.Graph, arInvoiceRow.DocType, arInvoiceRow.RefNbr);

					fsxARInvoiceRow.HasFSEquipmentInfo = inventoryItemSet.Count > 0 ? true : false;
				}
			}
		}

		#region Cache Attached

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[FSSelectorExtensionMaintenanceEquipment(typeof(ARTran.customerID), typeof(True))]
		protected virtual void _(Events.CacheAttached<FSxARTran.sMEquipmentID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[FSSelectorEquipmentLineRefSOInvoice(typeof(True))]
		protected virtual void _(Events.CacheAttached<FSxARTran.equipmentComponentLineNbr> e) { }
		#endregion

		#region Event Handlers

		#region ARInvoice

		#region FieldSelecting
		#endregion
		#region FieldDefaulting
		#endregion
		#region FieldUpdating
		#endregion
		#region FieldVerifying
		#endregion
		#region FieldUpdated
		#endregion

		protected virtual void _(Events.RowSelecting<ARInvoice> e)
		{
			SetUnpersistedFSInfo(e.Cache, e.Args);
		}

		protected virtual void _(Events.RowSelected<ARInvoice> e)
		{
		}

		protected virtual void _(Events.RowInserting<ARInvoice> e)
		{
		}

		protected virtual void _(Events.RowInserted<ARInvoice> e)
		{
		}

		protected virtual void _(Events.RowUpdating<ARInvoice> e)
		{
		}

		protected virtual void _(Events.RowUpdated<ARInvoice> e)
		{
		}

		protected virtual void _(Events.RowDeleting<ARInvoice> e)
		{
		}

		protected virtual void _(Events.RowDeleted<ARInvoice> e)
		{
		}

		protected virtual void _(Events.RowPersisting<ARInvoice> e)
		{
			if (e.Row == null || SharedFunctions.isFSSetupSet(Base) == false)
			{
				return;
			}

			ARInvoice arInvoiceRow = (ARInvoice)e.Row;

			ValidatePostBatchStatus(e.Operation, ID.Batch_PostTo.AR, arInvoiceRow.DocType, arInvoiceRow.RefNbr);
		}

		protected virtual void _(Events.RowPersisted<ARInvoice> e)
		{
			if (e.Row == null || SharedFunctions.isFSSetupSet(Base) == false)
			{
				return;
			}

			ARInvoice arInvoiceRow = (ARInvoice)e.Row;
			if (e.Operation == PXDBOperation.Delete
					&& e.TranStatus == PXTranStatus.Open)
			{
				CleanPostingInfoLinkedToDoc(arInvoiceRow);
				CleanContractPostingInfoLinkedToDoc(arInvoiceRow);
			}

			if (e.Operation == PXDBOperation.Update
					&& e.TranStatus == PXTranStatus.Open)
				EvaluateComponentLineNumbers(e.Cache, arInvoiceRow);
		}

		#endregion
		#region ARTran

		#region FieldSelecting
		#endregion
		#region FieldDefaulting
		#endregion
		#region FieldUpdating

		protected virtual void _(Events.FieldUpdating<ARTran, ARTran.qty> e)
		{
			if (e.Row == null || SharedFunctions.isFSSetupSet(Base) == false)
			{
				return;
			}

			ARTran arTranRow = (ARTran)e.Row;

			if (IsLineCreatedFromAppSO(Base, Base.Document.Current, arTranRow, typeof(ARTran.qty).Name) == true)
			{
				throw new PXSetPropertyException(TX.Error.NO_UPDATE_ALLOWED_DOCLINE_LINKED_TO_APP_SO);
			}
		}

		protected virtual void _(Events.FieldUpdating<ARTran, ARTran.uOM> e)
		{
			if (e.Row == null || SharedFunctions.isFSSetupSet(Base) == false)
			{
				return;
			}

			ARTran arTranRow = (ARTran)e.Row;

			if (IsLineCreatedFromAppSO(Base, Base.Document.Current, arTranRow, typeof(ARTran.uOM).Name) == true)
			{
				throw new PXSetPropertyException(TX.Error.NO_UPDATE_ALLOWED_DOCLINE_LINKED_TO_APP_SO);
			}
		}

		protected virtual void _(Events.FieldUpdating<ARTran, ARTran.inventoryID> e)
		{
			if (e.Row == null || SharedFunctions.isFSSetupSet(Base) == false)
			{
				return;
			}

			ARTran arTranRow = (ARTran)e.Row;

			if (IsLineCreatedFromAppSO(Base, Base.Document.Current, arTranRow, typeof(ARTran.inventoryID).Name) == true)
			{
				throw new PXSetPropertyException(TX.Error.NO_UPDATE_ALLOWED_DOCLINE_LINKED_TO_APP_SO);
			}
		}
		#endregion
		#region FieldVerifying
		#endregion
		#region FieldUpdated
		#endregion

		protected virtual void _(Events.RowSelecting<ARTran> e)
		{
		}

		protected virtual void _(Events.RowSelected<ARTran> e)
		{

		}

		protected virtual void _(Events.RowInserting<ARTran> e)
		{
		}

		protected virtual void _(Events.RowInserted<ARTran> e)
		{
			UpdateARTran(e.Cache, e.Row);
		}

		protected virtual void _(Events.RowUpdating<ARTran> e)
		{
		}

		protected virtual void _(Events.RowUpdated<ARTran> e)
		{
		}

		protected virtual void _(Events.RowDeleting<ARTran> e)
		{
			if (e.Row == null || SharedFunctions.isFSSetupSet(Base) == false)
			{
				return;
			}

			ARTran arTranRow = (ARTran)e.Row;

			if (e.ExternalCall == true)
			{
				if (IsLineCreatedFromAppSO(Base, Base.Document.Current, arTranRow, null) == true)
				{
					throw new PXException(TX.Error.NO_DELETION_ALLOWED_DOCLINE_LINKED_TO_APP_SO);
				}
			}
		}

		protected virtual void _(Events.RowDeleted<ARTran> e)
		{
		}

		protected virtual void _(Events.RowPersisting<ARTran> e)
		{
		}

		protected virtual void _(Events.RowPersisted<ARTran> e)
		{
			if (e.Row == null)
			{
				return;
			}

			// We call here cache.GetStateExt for every field when the transaction is aborted
			// to set the errors in the fields and then the Generate Invoice screen can read them
			if (e.TranStatus == PXTranStatus.Aborted && IsInvoiceProcessRunning == true)
			{
				MessageHelper.GetRowMessage(e.Cache, e.Row, false, false);
			}
		}
		#endregion

		#endregion

		#region Invoicing Methods
		public override List<ErrorInfo> GetErrorInfo()
		{
			return MessageHelper.GetErrorInfo<ARTran>(Base.Document.Cache, Base.Document.Current, Base.Transactions, typeof(ARTran));
		}

		public virtual string GetLineDisplayHint(PXGraph graph, string lineRefNbr, string lineDescr, int? inventoryID)
		{
			return MessageHelper.GetLineDisplayHint(graph, lineRefNbr, lineDescr, inventoryID);
		}

		public override void CreateInvoice(PXGraph graphProcess, List<DocLineExt> docLines, short invtMult, DateTime? invoiceDate, string invoiceFinPeriodID, OnDocumentHeaderInsertedDelegate onDocumentHeaderInserted, OnTransactionInsertedDelegate onTransactionInserted, PXQuickProcess.ActionFlow quickProcessFlow)
		{
			if (docLines.Count == 0)
			{
				return;
			}

			bool? initialHold = false;

			FSServiceOrder fsServiceOrderRow = docLines[0].fsServiceOrder;
			FSSrvOrdType fsSrvOrdTypeRow = docLines[0].fsSrvOrdType;
			FSPostDoc fsPostDocRow = docLines[0].fsPostDoc;
			FSAppointment fsAppointmentRow = docLines[0].fsAppointment;

			var cancel_defaulting = new PXFieldDefaulting((sender, e) =>
			{
				e.NewValue = fsServiceOrderRow.BranchID;
				e.Cancel = true;
			});

			try
			{
				Base.FieldDefaulting.AddHandler<ARInvoice.branchID>(cancel_defaulting);

				ARInvoice arInvoiceRow = new ARInvoice();

				if (invtMult >= 0)
				{
					arInvoiceRow.DocType = ARInvoiceType.Invoice;
					CheckAutoNumbering(Base.ARSetup.SelectSingle().InvoiceNumberingID);
				}
				else
				{
					arInvoiceRow.DocType = ARInvoiceType.CreditMemo;
					CheckAutoNumbering(Base.ARSetup.SelectSingle().CreditAdjNumberingID);
				}

				arInvoiceRow.DocDate = invoiceDate;
				arInvoiceRow.FinPeriodID = invoiceFinPeriodID;
				arInvoiceRow.InvoiceNbr = fsServiceOrderRow.CustPORefNbr;
				arInvoiceRow = Base.Document.Insert(arInvoiceRow);
				initialHold = Base.ARSetup.Current.HoldEntry;

				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.hold>(arInvoiceRow, true);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.customerID>(arInvoiceRow, fsServiceOrderRow.BillCustomerID);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.customerLocationID>(arInvoiceRow, fsServiceOrderRow.BillLocationID);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.curyID>(arInvoiceRow, fsServiceOrderRow.CuryID);

				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.taxZoneID>(arInvoiceRow, fsAppointmentRow != null ? fsAppointmentRow.TaxZoneID : fsServiceOrderRow.TaxZoneID);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.taxCalcMode>(arInvoiceRow, fsAppointmentRow != null ? fsAppointmentRow.TaxCalcMode : fsServiceOrderRow.TaxCalcMode);

				string termsID = GetTermsIDFromCustomerOrVendor(graphProcess, fsServiceOrderRow.BillCustomerID, null);
				bool isTermsApplicable = !(arInvoiceRow.DocType == ARInvoiceType.CreditMemo && Base.ARSetup.Current.TermsInCreditMemos != true);
				if (isTermsApplicable)
				{
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.termsID>(arInvoiceRow, termsID ?? fsSrvOrdTypeRow.DfltTermIDARSO);
				}

				if (fsServiceOrderRow.ProjectID != null)
				{
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.projectID>(arInvoiceRow, fsServiceOrderRow.ProjectID);
				}

				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.docDesc>(arInvoiceRow, fsServiceOrderRow.DocDesc);
				arInvoiceRow.FinPeriodID = invoiceFinPeriodID;
				arInvoiceRow = Base.Document.Update(arInvoiceRow);

				SetContactAndAddress(Base, fsServiceOrderRow);

				if (onDocumentHeaderInserted != null)
				{
					onDocumentHeaderInserted(Base, arInvoiceRow);
				}

				IDocLine docLine = null;
				ARTran arTranRow = null;
				FSxARTran fsxARTranRow = null;
				PMTask pmTaskRow = null;
				FSAppointmentDet fsAppointmentDetRow = null;
				FSSODet fsSODetRow = null;
				int? acctID;

				foreach (DocLineExt docLineExt in docLines)
				{
					// Marker of a document (Appointment or Service Order) beginning
					// Supposed that records in docLines are sorted by document
					bool nextDoc = fsAppointmentRow == null
									? (docLineExt.fsSODet == docLines[0].fsSODet || fsServiceOrderRow.RefNbr != docLineExt.fsServiceOrder.RefNbr)
									: (docLineExt.fsAppointmentDet == docLines[0].fsAppointmentDet || fsAppointmentRow.RefNbr != docLineExt.fsAppointment.RefNbr);

					docLine = docLineExt.docLine;
					fsPostDocRow = docLineExt.fsPostDoc;
					fsServiceOrderRow = docLineExt.fsServiceOrder;
					fsSrvOrdTypeRow = docLineExt.fsSrvOrdType;
					fsAppointmentRow = docLineExt.fsAppointment;
					fsAppointmentDetRow = docLineExt.fsAppointmentDet;
					fsSODetRow = docLineExt.fsSODet;

					if (docLine.LineType == ID.LineType_ALL.INVENTORY_ITEM)
					{
						string billSource = fsAppointmentRow != null ? TX.TableName.APPOINTMENT : TX.TableName.SERVICE_ORDER;
						throw new PXException(TX.Error.ErrorRunBillingInventoryItemsToAR, billSource);
					}

					arTranRow = new ARTran();
					arTranRow = Base.Transactions.Insert(arTranRow);

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.branchID>(arTranRow, docLine.BranchID);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.inventoryID>(arTranRow, docLine.InventoryID);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.uOM>(arTranRow, docLine.UOM);

					pmTaskRow = docLineExt.pmTask;

					if (pmTaskRow != null && pmTaskRow.Status == ProjectTaskStatus.Completed)
					{
						throw new PXException(TX.Error.POSTING_PMTASK_ALREADY_COMPLETED, fsServiceOrderRow.RefNbr, GetLineDisplayHint(Base, docLine.LineRef, docLine.TranDesc, docLine.InventoryID), pmTaskRow.TaskCD);
					}

					if (docLine.ProjectID != null && docLine.ProjectTaskID != null)
					{
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.taskID>(arTranRow, docLine.ProjectTaskID);
					}

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.qty>(arTranRow, docLine.GetQty(FieldType.BillableField));
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.tranDesc>(arTranRow, docLine.TranDesc);

					fsPostDocRow.DocLineRef = arTranRow = Base.Transactions.Update(arTranRow);

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.salesPersonID>(arTranRow, fsServiceOrderRow?.SalesPersonID);

					if (docLine.AcctID != null)
					{
						acctID = docLine.AcctID;
					}
					else
					{
						acctID = (int?)Get_TranAcctID_DefaultValue(
							graphProcess,
							fsSrvOrdTypeRow.SalesAcctSource,
							docLine.InventoryID,
							docLine.SiteID,
							fsServiceOrderRow);
					}

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.accountID>(arTranRow, acctID);

					if (docLine.SubID != null)
					{
						try
						{
							Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.subID>(arTranRow, docLine.SubID);
						}
						catch (PXException)
						{
							arTranRow.SubID = null;
						}
					}
					else
					{
						SetCombinedSubID(graphProcess,
										Base.Transactions.Cache,
										arTranRow,
										null,
										null,
										fsSrvOrdTypeRow,
										arTranRow.BranchID,
										arTranRow.InventoryID,
										arInvoiceRow.CustomerLocationID,
										fsServiceOrderRow.BranchLocationID,
										fsServiceOrderRow.SalesPersonID,
										docLine.IsService);
					}


					if (docLine.IsFree == true)
					{
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualPrice>(arTranRow, true);
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.curyUnitPrice>(arTranRow, 0m);

						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualDisc>(arTranRow, true);
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.discPct>(arTranRow, 0m);
					}
					else
					{
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualPrice>(arTranRow, docLine.ManualPrice);
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.curyUnitPrice>(arTranRow, docLine.CuryUnitPrice * invtMult);

						bool manualDisc = false;
						if (docLineExt.fsAppointmentDet != null)
						{
							manualDisc = (bool)docLineExt.fsAppointmentDet.ManualDisc;
						}
						else
						{
							if (docLineExt.fsSODet != null)
							{
								manualDisc = (bool)docLineExt.fsSODet.ManualDisc;
							}
						}

						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualDisc>(arTranRow, manualDisc);
						if (manualDisc == true)
						{
							Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.discPct>(arTranRow, docLine.DiscPct);
						}
					}

					decimal? curyExtPrice = docLine.CuryBillableExtPrice * invtMult;
					curyExtPrice = CM.PXCurrencyAttribute.Round(Base.Transactions.Cache, arTranRow, curyExtPrice ?? 0m, CM.CMPrecision.TRANCURY);

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.curyExtPrice>(arTranRow, curyExtPrice);

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.taxCategoryID>(arTranRow, docLine.TaxCategoryID);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.commissionable>(arTranRow, fsServiceOrderRow.Commissionable ?? false);

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.costCodeID>(arTranRow, docLine.CostCodeID);

					fsxARTranRow = Base.Transactions.Cache.GetExtension<FSxARTran>(arTranRow);

					fsxARTranRow.SrvOrdType = fsServiceOrderRow.SrvOrdType;
					fsxARTranRow.ServiceOrderRefNbr = fsServiceOrderRow.RefNbr;
					fsxARTranRow.AppointmentRefNbr = fsAppointmentRow?.RefNbr;
					fsxARTranRow.ServiceOrderLineNbr = fsSODetRow?.LineNbr;
					fsxARTranRow.AppointmentLineNbr = fsAppointmentDetRow?.LineNbr;

					arTranRow = Base.Transactions.Update(arTranRow);

					if (nextDoc)
					{
						if (fsAppointmentRow == null)
							SharedFunctions.CopyNotesAndFiles(
								Base.Caches[typeof(FSServiceOrder)],
								Base.Document.Cache,
								fsServiceOrderRow,
								arInvoiceRow,
								fsSrvOrdTypeRow.CopyNotesToInvoice,
								fsSrvOrdTypeRow.CopyAttachmentsToInvoice);
						else
							SharedFunctions.CopyNotesAndFiles(
								Base.Caches[typeof(FSAppointment)],
								Base.Document.Cache,
								fsAppointmentRow,
								arInvoiceRow,
								fsSrvOrdTypeRow.CopyNotesToInvoice,
								fsSrvOrdTypeRow.CopyAttachmentsToInvoice);
					}

					SharedFunctions.CopyNotesAndFiles(Base.Transactions.Cache, arTranRow, docLine, fsSrvOrdTypeRow);
					fsPostDocRow.DocLineRef = arTranRow = Base.Transactions.Update(arTranRow);

					if (onTransactionInserted != null)
					{
						onTransactionInserted(Base, arTranRow);
					}
				}

				arInvoiceRow = Base.Document.Update(arInvoiceRow);

				if (Base.ARSetup.Current.RequireControlTotal == true)
				{
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.curyOrigDocAmt>(arInvoiceRow, arInvoiceRow.CuryDocBal);
				}

				if (initialHold != true)
				{
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.hold>(arInvoiceRow, false);
				}

				arInvoiceRow = Base.Document.Update(arInvoiceRow);
			}
			finally
			{
				Base.FieldDefaulting.RemoveHandler<ARInvoice.branchID>(cancel_defaulting);
			}
		}

		public override FSCreatedDoc PressSave(int batchID, List<DocLineExt> docLines, BeforeSaveDelegate beforeSave)
		{
			if (Base.Document.Current == null)
			{
				throw new SharedClasses.TransactionScopeException();
			}

			if (beforeSave != null)
			{
				beforeSave(Base);
			}

			ARInvoiceEntryExternalTax TaxGraphExt = Base.GetExtension<ARInvoiceEntryExternalTax>();

			if (TaxGraphExt != null)
			{
				TaxGraphExt.SkipTaxCalcAndSave();
			}
			else
			{
				Base.Save.Press();
			}

			string docType = Base.Document.Current.DocType;
			string refNbr = Base.Document.Current.RefNbr;

			// Reload ARInvoice to get the current value of IsTaxValid
			Base.Clear();
			Base.Document.Current = PXSelect<ARInvoice, Where<ARInvoice.docType, Equal<Required<ARInvoice.docType>>, And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>.Select(Base, docType, refNbr);
			ARInvoice arInvoiceRow = Base.Document.Current;

			var fsCreatedDocRow = new FSCreatedDoc()
			{
				BatchID = batchID,
				PostTo = ID.Batch_PostTo.AR,
				CreatedDocType = arInvoiceRow.DocType,
				CreatedRefNbr = arInvoiceRow.RefNbr
			};

			return fsCreatedDocRow;
		}

		public override void Clear()
		{
			Base.Clear(PXClearOption.ClearAll);
		}

		public override PXGraph GetGraph()
		{
			return Base;
		}

		public override void DeleteDocument(FSCreatedDoc fsCreatedDocRow)
		{
			Base.Document.Current = Base.Document.Search<ARInvoice.refNbr>(fsCreatedDocRow.CreatedRefNbr, fsCreatedDocRow.CreatedDocType);

			if (Base.Document.Current != null)
			{
				if (Base.Document.Current.RefNbr == fsCreatedDocRow.CreatedRefNbr
						&& Base.Document.Current.DocType == fsCreatedDocRow.CreatedDocType)
				{
					Base.Delete.Press();
				}
			}
		}

		public override void CleanPostInfo(PXGraph cleanerGraph, FSPostDet fsPostDetRow)
		{
			PXUpdate<
				Set<FSPostInfo.aRLineNbr, Null,
				Set<FSPostInfo.arRefNbr, Null,
				Set<FSPostInfo.arDocType, Null,
				Set<FSPostInfo.aRPosted, False>>>>,
			FSPostInfo,
			Where<
				FSPostInfo.postID, Equal<Required<FSPostInfo.postID>>,
				And<FSPostInfo.aRPosted, Equal<True>>>>
			.Update(cleanerGraph, fsPostDetRow.PostID);
		}

        public virtual bool IsLineCreatedFromAppSO(PXGraph cleanerGraph, object document, object lineDoc, string fieldName)
        {
            if (document == null || lineDoc == null
                || Base.Accessinfo.ScreenID.Replace(".", "") == ID.ScreenID.SERVICE_ORDER
                || Base.Accessinfo.ScreenID.Replace(".", "") == ID.ScreenID.APPOINTMENT
                || Base.Accessinfo.ScreenID.Replace(".", "") == ID.ScreenID.INVOICE_BY_SERVICE_ORDER
                || Base.Accessinfo.ScreenID.Replace(".", "") == ID.ScreenID.INVOICE_BY_APPOINTMENT)
            {
                return false;
            }

			string refNbr = ((ARInvoice)document).RefNbr;
			string docType = ((ARInvoice)document).DocType;
			int? lineNbr = ((ARTran)lineDoc).LineNbr;

			return PXSelect<FSPostInfo,
				   Where<
					   FSPostInfo.arRefNbr, Equal<Required<FSPostInfo.arRefNbr>>,
					   And<FSPostInfo.arDocType, Equal<Required<FSPostInfo.arDocType>>,
					   And<FSPostInfo.aRLineNbr, Equal<Required<FSPostInfo.aRLineNbr>>,
					   And<FSPostInfo.aRPosted, Equal<True>>>>>>
				   .Select(cleanerGraph, refNbr, docType, lineNbr).Count() > 0;
		}

		public virtual void UpdateARTran(PXCache cache, ARTran arTranRow)
		{
			var fsxARTranRow = cache.GetExtension<FSxARTran>(arTranRow);

			if (Base.Document.Current.DocType != ARInvoiceType.CreditMemo)
			{
				if (arTranRow.SOOrderType == null
						|| arTranRow.SOOrderNbr == null
						|| arTranRow.SOOrderLineNbr == null)
				{
					return;
				}

				SOLine soLineRow = SOLine.PK.Find(cache.Graph, arTranRow.SOOrderType, arTranRow.SOOrderNbr, arTranRow.SOOrderLineNbr);

				if (soLineRow == null)
				{
					return;
				}

				FSxSOLine fsxSOLineRow = PXCache<SOLine>.GetExtension<FSxSOLine>(soLineRow);

				fsxARTranRow.SrvOrdType = fsxSOLineRow.SrvOrdType;
				fsxARTranRow.AppointmentRefNbr = fsxSOLineRow.AppointmentRefNbr;
				fsxARTranRow.AppointmentLineNbr = fsxSOLineRow.AppointmentLineNbr;
				fsxARTranRow.ServiceOrderRefNbr = fsxSOLineRow.ServiceOrderRefNbr;
				fsxARTranRow.ServiceOrderLineNbr = fsxSOLineRow.ServiceOrderLineNbr;

				if (string.IsNullOrEmpty(fsxSOLineRow?.ServiceContractRefNbr) == false)
				{
					fsxARTranRow.ServiceContractRefNbr = fsxSOLineRow.ServiceContractRefNbr;
					fsxARTranRow.ServiceContractPeriodID = fsxSOLineRow.ServiceContractPeriodID;
				}

				fsxARTranRow.SMEquipmentID = fsxSOLineRow.SMEquipmentID;
				fsxARTranRow.ComponentID = fsxSOLineRow.ComponentID;
				fsxARTranRow.EquipmentComponentLineNbr = fsxSOLineRow.EquipmentComponentLineNbr;
				fsxARTranRow.EquipmentAction = fsxSOLineRow.EquipmentAction;
				fsxARTranRow.Comment = fsxSOLineRow.Comment;

				SOLine soLineRow2 = SOLine.PK.Find(cache.Graph, arTranRow.SOOrderType, arTranRow.SOOrderNbr, fsxSOLineRow.NewEquipmentLineNbr);

                if (soLineRow2 != null)
                {
                    ARTran arTranRow2 = cache.Cached.RowCast<ARTran>()
						.Where(x => x.SOOrderType == arTranRow.SOOrderType
                            && x.SOOrderNbr == arTranRow.SOOrderNbr
                            && x.SOOrderLineNbr == soLineRow2.LineNbr)
                        .FirstOrDefault();

					fsxARTranRow.NewEquipmentLineNbr = arTranRow2?.LineNbr;
				}
			}
			else
			{
				ARTran relatedARTran = null;

				var arTranView = new PXSelect<ARTran>(Base);
				arTranView.WhereAnd<Where<ARTran.tranType, Equal<Required<ARTran.tranType>>,
					And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
					And<ARTran.lineNbr, Equal<Required<ARTran.lineNbr>>>>>>();

				if (string.IsNullOrEmpty(arTranRow.OrigInvoiceType) == false
						&& string.IsNullOrEmpty(arTranRow.OrigInvoiceNbr) == false
							&& arTranRow.OrigInvoiceLineNbr != null)
				{
					relatedARTran = arTranView.Select(arTranRow.OrigInvoiceType, arTranRow.OrigInvoiceNbr, arTranRow.OrigInvoiceLineNbr);
				}
				else if (Base.CurrentDocument.Current.OrigRefNbr != null)
				{
					relatedARTran = arTranView.Select(ARInvoiceType.Invoice, Base.CurrentDocument.Current.OrigRefNbr, arTranRow.OrigLineNbr);
				}

				if (relatedARTran != null)
				{
					var relatedFSxARTran = cache.GetExtension<FSxARTran>(relatedARTran);
					fsxARTranRow.SrvOrdType = relatedFSxARTran.SrvOrdType;
					fsxARTranRow.ServiceOrderRefNbr = relatedFSxARTran.ServiceOrderRefNbr;
					fsxARTranRow.ServiceOrderLineNbr = relatedFSxARTran.ServiceOrderLineNbr;
					fsxARTranRow.AppointmentRefNbr = relatedFSxARTran.AppointmentRefNbr;
					fsxARTranRow.AppointmentLineNbr = relatedFSxARTran.AppointmentLineNbr;
					fsxARTranRow.ServiceContractRefNbr = relatedFSxARTran.ServiceContractRefNbr;
					fsxARTranRow.ServiceContractPeriodID = relatedFSxARTran.ServiceContractPeriodID;
					fsxARTranRow.Comment = relatedFSxARTran.Comment;
					fsxARTranRow.EquipmentAction = relatedFSxARTran.EquipmentAction;
					fsxARTranRow.SMEquipmentID = relatedFSxARTran.SMEquipmentID;
					fsxARTranRow.NewEquipmentLineNbr = relatedFSxARTran.NewEquipmentLineNbr;
					fsxARTranRow.ComponentID = relatedFSxARTran.ComponentID;
					fsxARTranRow.EquipmentComponentLineNbr = relatedFSxARTran.EquipmentComponentLineNbr;
					fsxARTranRow.ReplaceSMEquipmentID = relatedFSxARTran.ReplaceSMEquipmentID;
				}
			}
		}

		#region Invoice By Contract Period Methods 
		public virtual FSContractPostDoc CreateInvoiceByContract(PXGraph graphProcess, DateTime? invoiceDate, string invoiceFinPeriodID, FSContractPostBatch fsContractPostBatchRow, FSServiceContract fsServiceContractRow, FSContractPeriod fsContractPeriodRow, List<ContractInvoiceLine> docLines)
		{
			FSContractPostDoc fsContractCreatedDocRow = null;

			try
			{
				if (docLines.Count == 0)
				{
					return null;
				}

				bool? initialHold = false;

				FSSetup fsSetupRow = ServiceManagementSetup.GetServiceManagementSetup(graphProcess);

				if (!Base.Views.Caches.Contains(typeof(ARTran)))
					Base.Views.Caches.Add(typeof(ARTran));

				ARInvoice arInvoiceRow = new ARInvoice();

				arInvoiceRow.DocType = ARInvoiceType.Invoice;
				CheckAutoNumbering(Base.ARSetup.SelectSingle().InvoiceNumberingID);

				arInvoiceRow.DocDate = invoiceDate;
				arInvoiceRow.FinPeriodID = invoiceFinPeriodID;
				arInvoiceRow = Base.Document.Insert(arInvoiceRow);
				initialHold = arInvoiceRow.Hold;

				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.hold>(arInvoiceRow, true);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.branchID>(arInvoiceRow, fsServiceContractRow.BranchID);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.customerID>(arInvoiceRow, fsServiceContractRow.BillCustomerID);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.customerLocationID>(arInvoiceRow, fsServiceContractRow.BillLocationID);

				string prefixDesc = ServiceContractBillingType.ListAttribute.GetLocalizedLabel<FSServiceContract.billingType>(graphProcess.Caches[typeof(FSServiceContract)], fsServiceContractRow);
				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.docDesc>(arInvoiceRow, (PXMessages.LocalizeFormatNoPrefix(TX.Messages.ContractDesctInvoicePrefix, prefixDesc, fsServiceContractRow.RefNbr, (string.IsNullOrEmpty(fsServiceContractRow.DocDesc) ? string.Empty : fsServiceContractRow.DocDesc))));

				string termsID = GetTermsIDFromCustomerOrVendor(graphProcess, fsServiceContractRow.BillCustomerID, null);

				if (termsID != null)
				{
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.termsID>(arInvoiceRow, termsID);
				}
				else
				{
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.termsID>(arInvoiceRow, fsSetupRow.DfltContractTermIDARSO);
				}

				Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.projectID>(arInvoiceRow, fsServiceContractRow.ProjectID);

				int? acctID;

				foreach (ContractInvoiceLine docLine in docLines)
				{
					ARTran arTranRow = new ARTran();
					arTranRow = Base.Transactions.Insert(arTranRow);

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.inventoryID>(arTranRow, docLine.InventoryID);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.uOM>(arTranRow, docLine.UOM);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.salesPersonID>(arTranRow, docLine.SalesPersonID);

					arTranRow = Base.Transactions.Update(arTranRow);

					if (docLine.AcctID != null)
					{
						acctID = docLine.AcctID;
					}
					else
					{
						acctID = (int?)Get_INItemAcctID_DefaultValue(graphProcess,
																		fsSetupRow.ContractSalesAcctSource,
																		docLine.InventoryID,
																		fsServiceContractRow?.CustomerID,
																		fsServiceContractRow?.CustomerLocationID);
					}

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.accountID>(arTranRow, acctID);

					if (docLine.SubID != null)
					{
						try
						{
							Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.subID>(arTranRow, docLine.SubID);
						}
						catch (PXException)
						{
							arTranRow.SubID = null;
						}
					}
					else
					{
						SetCombinedSubID(graphProcess,
										Base.Transactions.Cache,
										arTranRow,
										null,
										null,
										fsSetupRow,
										arTranRow.BranchID,
										arTranRow.InventoryID,
										arInvoiceRow.CustomerLocationID,
										fsServiceContractRow.BranchLocationID);
					}

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.qty>(arTranRow, docLine.Qty);

					if (docLine.IsFree == true)
					{
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualPrice>(arTranRow, true);
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.curyUnitPrice>(arTranRow, 0m);

						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualDisc>(arTranRow, true);
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.discPct>(arTranRow, 0m);
					}
					else
					{
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualPrice>(arTranRow, docLine.ManualPrice);
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.curyUnitPrice>(arTranRow, docLine.CuryUnitPrice);

						bool manualDisc = false;

						if (docLine.AppDetID != null)
						{
							FSAppointmentDet apptDet = PXSelect<FSAppointmentDet,
										Where<FSAppointmentDet.appDetID, Equal<Required<FSAppointmentDet.appDetID>>>>.
									Select(Base, docLine.AppDetID);
							manualDisc = (bool)apptDet?.ManualDisc;
						}
						else
						{
							if (docLine.SODetID != null)
							{
								FSSODet soDet = PXSelect<FSSODet,
											Where<FSSODet.sODetID, Equal<Required<FSSODet.sODetID>>>>.
										Select(Base, docLine.SODetID);
								manualDisc = (bool)soDet?.ManualDisc;
							}
						}

						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.manualDisc>(arTranRow, manualDisc);
						if (manualDisc == true)
						{
							Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.discPct>(arTranRow, docLine.DiscPct);
						}
					}

					if (docLine.ServiceContractID != null
							&& docLine.ContractRelated == false
							&& (docLine.SODetID != null || docLine.AppDetID != null))
					{
						Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.curyExtPrice>(arTranRow, docLine.CuryBillableExtPrice);
					}

					string detailDesc = (fsServiceContractRow.BillingType == FSServiceContract.billingType.Values.StandardizedBillings) ? docLine.TranDescPrefix + arTranRow.TranDesc : arTranRow.TranDesc;
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.tranDesc>(arTranRow, detailDesc);

					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.taskID>(arTranRow, docLine.ProjectTaskID);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.costCodeID>(arTranRow, docLine.CostCodeID);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.deferredCode>(arTranRow, docLine.DeferredCode);

					// TODO: Add TaxCategoryID in Service Contract line definition
					//Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.taxCategoryID>(arTranRow, docLine.TaxCategoryID);
					Base.Transactions.Cache.SetValueExtIfDifferent<ARTran.commissionable>(arTranRow, docLine.Commissionable ?? false);

					FSxARTran fsxARTranRow = Base.Transactions.Cache.GetExtension<FSxARTran>(arTranRow);

					fsxARTranRow.ServiceContractRefNbr = fsServiceContractRow.RefNbr;
					fsxARTranRow.ServiceContractPeriodID = fsContractPeriodRow.ContractPeriodID;

					if (docLine.ContractRelated == false)
					{
						fsxARTranRow.SrvOrdType = docLine.SrvOrdType;
						fsxARTranRow.ServiceOrderRefNbr = docLine.fsSODet.RefNbr;
						fsxARTranRow.ServiceOrderLineNbr = docLine.fsSODet.LineNbr;
						fsxARTranRow.AppointmentRefNbr = docLine.fsAppointmentDet?.RefNbr;
						fsxARTranRow.AppointmentLineNbr = docLine.fsAppointmentDet?.LineNbr;
					}

					arTranRow = Base.Transactions.Update(arTranRow);
				}

				SetChildCustomerShipToInfo(fsServiceContractRow.ServiceContractID);

				if (Base.ARSetup.Current.RequireControlTotal == true)
				{
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.curyOrigDocAmt>(arInvoiceRow, arInvoiceRow.CuryDocBal);
				}

				if (initialHold != true)
				{
					bool? oldValue = Base.Document.Cache.GetValue<ARInvoice.hold>(arInvoiceRow) as bool?;
					Base.Document.Cache.SetValueExtIfDifferent<ARInvoice.hold>(arInvoiceRow, false);
					bool? newValue = Base.Document.Cache.GetValue<ARInvoice.hold>(arInvoiceRow) as bool?;

					if (oldValue != newValue)
					{
						Base.Document.Update(arInvoiceRow);
					}
				}

				Exception newException = null;

				try
				{
					Base.Save.Press();
				}
				catch (Exception e)
				{
					List<ErrorInfo> errorList = this.GetErrorInfo();
					newException = GetErrorInfoInLines(errorList, e);
				}

				if (newException != null)
				{
					throw newException;
				}

				arInvoiceRow = Base.Document.Current;

				fsContractCreatedDocRow = new FSContractPostDoc()
				{
					ContractPeriodID = fsContractPeriodRow.ContractPeriodID,
					ContractPostBatchID = fsContractPostBatchRow.ContractPostBatchID,
					PostDocType = arInvoiceRow.DocType,
					PostedTO = ID.Batch_PostTo.AR,
					PostRefNbr = arInvoiceRow.RefNbr,
					ServiceContractID = fsServiceContractRow.ServiceContractID
				};
			}
			finally
			{
				Clear();
			}

			return fsContractCreatedDocRow;
		}
		#endregion
		#endregion

		#region Validations
		public virtual void ValidatePostBatchStatus(PXDBOperation dbOperation, string postTo, string createdDocType, string createdRefNbr)
		{
			DocGenerationHelper.ValidatePostBatchStatus<ARInvoice>(Base, dbOperation, postTo, createdDocType, createdRefNbr);
		}
		#endregion

		#region Virtual Methods
		public virtual int? Get_INItemAcctID_DefaultValue(PXGraph graph, string salesAcctSource, int? inventoryID, int? customerID, int? locationID)
		{
			return ServiceOrderEntry.Get_INItemAcctID_DefaultValueInt(graph, salesAcctSource, inventoryID, customerID, locationID);
		}

		public virtual int? Get_TranAcctID_DefaultValue(PXGraph graph, string salesAcctSource, int? inventoryID, int? siteID, FSServiceOrder fsServiceOrderRow)
		{
			return ServiceOrderEntry.Get_TranAcctID_DefaultValueInt(graph, salesAcctSource, inventoryID, siteID, fsServiceOrderRow);
		}

		protected virtual bool IsRunningServiceContractBilling(PXGraph graph)
		{
			return InvoiceHelper.IsRunningServiceContractBilling(graph);
		}

		protected virtual void GetChildCustomerShippingContactAndAddress(PXGraph graph, int? serviceContractID, out Contact shippingContact, out Address shippingAddress)
		{
			InvoiceHelper.GetChildCustomerShippingContactAndAddress(graph, serviceContractID, out shippingContact, out shippingAddress);
		}

		#endregion

		#region Private methods

		public virtual void SetChildCustomerShipToInfo(int? serviceContractID)
		{
			if (Base.Document.Current == null)
			{
				return;
			}

			if (IsRunningServiceContractBilling(Base) == false)
			{
				return;
			}

			if (Base.Document.Cache.GetStatus(Base.Document.Current) != PXEntryStatus.Inserted)
			{
				return;
			}

			Contact childCustomerShippingContact = null;
			Address childCustomerShippingAddress = null;

			GetChildCustomerShippingContactAndAddress(Base, serviceContractID, out childCustomerShippingContact, out childCustomerShippingAddress);

			if (childCustomerShippingContact != null)
			{
				ARShippingContact docShippingContact = null;

				Base.Shipping_Contact.Current = docShippingContact = Base.Shipping_Contact.SelectSingle();

				if (docShippingContact != null)
				{
					Base.Shipping_Contact.Cache.SetValueExt<ARShippingContact.overrideContact>(docShippingContact, true);
					docShippingContact = Base.Shipping_Contact.SelectSingle();

					docShippingContact = (ARShippingContact)Base.Shipping_Contact.Cache.CreateCopy(docShippingContact);

					int? customerID = docShippingContact.CustomerID;
					int? customerContactID = docShippingContact.CustomerContactID;
					InvoiceHelper.CopyContact(docShippingContact, childCustomerShippingContact);
					docShippingContact.CustomerID = customerID;
					docShippingContact.CustomerContactID = customerContactID;
					docShippingContact.RevisionID = 0;

					docShippingContact.IsDefaultContact = false;

					Base.Shipping_Contact.Update(docShippingContact);
				}
			}

			if (childCustomerShippingAddress != null)
			{
				ARShippingAddress docShippingAddress = null;

				Base.Shipping_Address.Current = docShippingAddress = Base.Shipping_Address.SelectSingle();

				if (docShippingAddress != null)
				{
					Base.Shipping_Address.Cache.SetValueExt<ARShippingAddress.overrideAddress>(docShippingAddress, true);
					docShippingAddress = Base.Shipping_Address.SelectSingle();

					docShippingAddress = (ARShippingAddress)Base.Shipping_Address.Cache.CreateCopy(docShippingAddress);

					int? customerID = docShippingAddress.CustomerID;
					int? customerAddressID = docShippingAddress.CustomerAddressID;
					InvoiceHelper.CopyAddress(docShippingAddress, childCustomerShippingAddress);
					docShippingAddress.CustomerID = customerID;
					docShippingAddress.CustomerAddressID = customerAddressID;
					docShippingAddress.RevisionID = 0;

					docShippingAddress.IsDefaultAddress = false;

					Base.Shipping_Address.Update(docShippingAddress);
				}
			}
		}

		private void EvaluateComponentLineNumbers(PXCache cache, ARInvoice arInvoiceRow)
		{
			PXCache arTranCache = cache.Graph.Caches[typeof(ARTran)];

			var tranRows = Base.Transactions
				.Select(arInvoiceRow.DocType, arInvoiceRow.RefNbr)
				.RowCast<ARTran>();

			var componentTransactions = tranRows.Where(t =>
				arTranCache.GetExtension<FSxARTran>(t).EquipmentAction == ID.Equipment_Action.CREATING_COMPONENT &&
				arTranCache.GetExtension<FSxARTran>(t).NewEquipmentLineNbr == null);

			foreach (var componentTransaction in componentTransactions)
			{
				SOLine compSOLine = SOLine.PK.Find(cache.Graph, componentTransaction.SOOrderType, componentTransaction.SOOrderNbr, componentTransaction.SOOrderLineNbr);
				FSxSOLine fsCompSOLine = PXCache<SOLine>.GetExtension<FSxSOLine>(compSOLine);

				var linkedARTran = tranRows.FirstOrDefault(t => t.SOOrderLineNbr == fsCompSOLine.NewEquipmentLineNbr);

				if (linkedARTran != null)
				{
					var fsxARTran = arTranCache.GetExtension<FSxARTran>(componentTransaction);
					fsxARTran.NewEquipmentLineNbr = linkedARTran.LineNbr;
					arTranCache.Persist(componentTransaction, PXDBOperation.Update);
				}
			}
		}

		#endregion
	}
}
