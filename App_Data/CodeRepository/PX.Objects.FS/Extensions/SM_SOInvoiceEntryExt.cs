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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.FS.DAC;
using PX.Objects.SO;
using PX.Objects.SO.GraphExtensions.SOInvoiceEntryExt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.FS
{
	public class SM_SOInvoiceEntryCorrectionExt : PXGraphExtension<Correction, SOInvoiceEntry>
	{
		public delegate void SetupFunc(Events.RowSelected<ARInvoice> e, bool isVisible, bool isEnabled);
		public delegate IEnumerable ReverseDirectInvoiceFunc(PXAdapter adapter);
		public delegate ARInvoiceState GetDocumentStateFunc(PXCache cache, ARInvoice doc);
		public delegate void ReleaseInvoiceFunc(List<ARRegister> list, bool isMassProcess);

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
		}

		private class ARTranKey
		{
			public string RefNbr { get; }
			public string TranType { get; }
			public int? LineNbr { get; }

			private ARTranKey(string refNbr, string tranType, int? lineNbr)
			{
				RefNbr = refNbr;
				TranType = tranType;
				LineNbr = lineNbr;
			}

			public static ARTranKey FromTran(ARTran aRTran)
			{
				return new ARTranKey(aRTran.RefNbr, aRTran.TranType, aRTran.LineNbr);
			}

			public static ARTranKey FromOrigin(ARTran aRTran)
			{
				return new ARTranKey(aRTran.OrigInvoiceNbr, aRTran.OrigInvoiceType, aRTran.OrigInvoiceLineNbr);
			}

			public override bool Equals(object obj)
			{
				if(ReferenceEquals(this, obj))
				{
					return true;
				}

				if (obj is ARTranKey key)
				{
					return this.RefNbr == key.RefNbr && this.TranType == key.TranType && this.LineNbr == key.LineNbr;
				}

				return false;
			}

			public override int GetHashCode()
			{
				return RefNbr.GetHashCode() ^ TranType.GetHashCode() ^ LineNbr.GetHashCode();
			}
		}

		/// <summary>
		/// Overrides <see cref="SOInvoiceEntry.ReleaseInvoiceProc"/>
		/// </summary>
		[PXOverride]
		public virtual void ReleaseInvoiceProc(List<ARRegister> list, bool isMassProcess, Action<List<ARRegister>, bool> baseMethod)
		{
			foreach (var doc in list
				.OfType<ARInvoice>()
				.Where(d => d.DocType == ARDocType.CreditMemo))
			{
				var transactions = Base
					.Transactions
					.View
					.SelectMultiBound(new object[] { doc })
					.Cast<ARTran>()
					.Where(arTran =>
						arTran.OrigInvoiceLineNbr != null &&
						arTran.OrigInvoiceNbr != null &&
						arTran.OrigInvoiceType != null)
					.ToList();

				if (transactions.Any(arTran => Base.Caches[typeof(ARTran)].GetExtension<FSxARTran>(arTran).IsFSRelated == true))
				{
					bool isFullCM = true;

					var origLines = new HashSet<ARTranKey>();

					foreach (var tran in transactions)
					{
						var lines = SelectFrom<ARTran>
							.Where<ARTran.tranType.IsEqual<P.AsString.ASCII>
							.And<ARTran.refNbr.IsEqual<P.AsString>>>
							.View
							.Select(Base, tran.OrigInvoiceType, tran.OrigInvoiceNbr)
							.AsEnumerable()
							.Select(r =>
							{
								var arTran = r.GetItem<ARTran>();

								return ARTranKey.FromTran(arTran);
							});

						origLines.AddRange(lines);
					}

					foreach (var tran in transactions)
					{
						var key = ARTranKey.FromOrigin(tran);
						isFullCM &= origLines.Remove(key);
					}

					isFullCM &= origLines.Count < 1;

					if (!isFullCM)
					{
						throw new PXException(TX.Error.SOCreditMemoPartiallyReleasing, doc.RefNbr, doc.OrigRefNbr);
					}
				}

				Base.Transactions.Cache.Clear();
			}

			baseMethod(list, isMassProcess);
		}

		[PXOverride]
		public virtual void SetupCorrectActionsState(Events.RowSelected<ARInvoice> e, bool isVisible, bool isEnabled, SetupFunc baseMethod)
		{
			baseMethod(e, isVisible, isEnabled);

			bool isAdvancedSOEnabled = PXAccess.FeatureInstalled<FeaturesSet.advancedSOInvoices>();
			bool anyServiceLine = AnyFieldServiceLine();
			bool anyRelatedCM = false;

			if (anyServiceLine)
			{
				anyRelatedCM = e.Row.DocType == ARDocType.Invoice && AnyRelatedCreditMemo(e.Row);
			}

			Base.Actions[SOInvoiceEntry_Workflow.ActionCategories.CorrectionsCategoryID]?.SetVisible(nameof(Correction.ReverseDirectInvoice), isVisible && isAdvancedSOEnabled && anyServiceLine);

			Base1.cancelInvoice.SetEnabled(isEnabled && (!anyServiceLine || !isAdvancedSOEnabled));
			Base1.correctInvoice.SetEnabled(isEnabled && !anyServiceLine);
			Base1.reverseDirectInvoice.SetEnabled(isEnabled && anyServiceLine && !anyRelatedCM);
		}

		[PXOverride]
		public virtual ARInvoiceState GetDocumentState(PXCache cache, ARInvoice doc, GetDocumentStateFunc baseMethod)
		{
			var state = baseMethod(cache, doc);

			bool isReversed = doc.DocType == ARDocType.CreditMemo && doc.OrigRefNbr != null && AnyFieldServiceLine();

			state.AllowDeleteTransactions &= !isReversed;
			state.AllowInsertTransactions &= !isReversed;

			return state;
		}

		protected virtual void _(Events.RowDeleting<ARTran> e)
		{
			var arTran = e.Row;
			if (arTran != null)
			{
				var invoice = Base.Document.Current;
				if (invoice?.DocType?.IsIn(ARDocType.CreditMemo, ARDocType.Invoice) == true && Base.Document.Cache.GetStatus(invoice) != PXEntryStatus.Deleted)
				{
					var fsxARTran = Base.Caches[typeof(ARTran)].GetExtension<FSxARTran>(arTran);
					if (fsxARTran.AppointmentRefNbr != null || fsxARTran.ServiceOrderRefNbr != null || fsxARTran.ServiceContractRefNbr != null)
					{
						throw new PXException(TX.Error.SOinvoiceWithFieldServiceLinesCannotBeDeleted);
					}
				}
			}
		}

		private bool AnyFieldServiceLine()
		{
			return Base
				.Transactions
				.Select()
				.RowCast<ARTran>()
				.Any(tr => Base.Caches[typeof(ARTran)].GetExtension<FSxARTran>(tr).IsFSRelated == true);
		}

		private bool AnyRelatedCreditMemo(ARInvoice invoice)
		{
			var result = SelectFrom<ARInvoice>
				.Where<ARInvoice.docType.IsEqual<ARDocType.invoice>
					.And<ARInvoice.origDocType.IsEqual<P.AsString.ASCII>
					.And<ARInvoice.origRefNbr.IsEqual<P.AsString>>>>
				.View
				.SelectSingleBound(Base, null, invoice.DocType, invoice.RefNbr);

			return (result.RowCount ?? 0) > 0;
		}
	}
}
