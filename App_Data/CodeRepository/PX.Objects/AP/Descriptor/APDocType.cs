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
using System.Linq;
using PX.Objects.Common;
using PX.Objects.GL;

namespace PX.Objects.AP
{
	public class APDocType : ILabelProvider
	{
		public const string Invoice = "INV";
		public const string CreditAdj = "ACR";
		public const string DebitAdj = "ADR";
		public const string Check = "CHK";
		public const string VoidCheck = "VCK";
		public const string Prepayment = "PPM";
		public const string Refund = "REF";
        public const string VoidRefund = "VRF";
        public const string QuickCheck = "QCK";
		public const string VoidQuickCheck = "VQC";
		public const string PrepaymentRequest = "PPR";
		public const string CashReturn = "RQC";

		protected static readonly IEnumerable<ValueLabelPair> _valueLabelPairs = new ValueLabelList
		{
			{ Invoice, Messages.Invoice },
			{ CreditAdj, Messages.CreditAdj },
			{ DebitAdj, Messages.DebitAdj },
			{ Check, Messages.Check },
			{ VoidCheck, Messages.VoidCheck },
			{ Prepayment, Messages.Prepayment },
			{ Refund, Messages.Refund },
            { VoidRefund, Messages.VoidRefund },
            { QuickCheck, Messages.QuickCheck },
			{ VoidQuickCheck, Messages.VoidQuickCheck },
			{ CashReturn, Messages.CashReturn },
			{ PrepaymentRequest, Messages.PrepaymentRequest},
		};

		protected static readonly IEnumerable<ValueLabelPair> _valueLabelPairsUI = new ValueLabelList
		{
			{ Invoice, Messages.Invoice },
			{ CreditAdj, Messages.CreditAdj },
			{ DebitAdj, Messages.DebitAdj },
			{ Prepayment, Messages.Prepayment },
			{ Refund, Messages.Refund },
			{ VoidRefund, Messages.VoidRefund },
			{ QuickCheck, Messages.QuickCheck },
			{ VoidQuickCheck, Messages.VoidQuickCheck },
			{ CashReturn, Messages.CashReturn },
			{ PrepaymentRequest, Messages.PrepaymentRequest},
		};

		protected static readonly IEnumerable<ValueLabelPair> _valueDocumentReleasableLabelPairs = new ValueLabelList
		{
			{ Invoice, Messages.Invoice },
			{ CreditAdj, Messages.CreditAdj },
			{ DebitAdj, Messages.DebitAdj },
			{ VoidCheck, Messages.VoidCheck },
			{ Prepayment, Messages.Prepayment },
			{ Refund, Messages.Refund },
			{ VoidRefund, Messages.VoidRefund },
			{ VoidQuickCheck, Messages.VoidQuickCheck },
			{ CashReturn, Messages.CashReturn },
			{ PrepaymentRequest, Messages.PrepaymentRequest},
		};

		protected static readonly IEnumerable<ValueLabelPair> _valueCheckReleasableLabelPairs = new ValueLabelList
		{
			{ Check, Messages.Check },
			{ VoidCheck, Messages.VoidCheck },
			{ QuickCheck, Messages.QuickCheck },
			{ VoidQuickCheck, Messages.VoidQuickCheck },
			{ Prepayment, Messages.Prepayment },
		};

		protected static readonly IEnumerable<ValueLabelPair> _valuePrintableLabelPairs = new ValueLabelList
		{
			{ Invoice, Messages.PrintInvoice },
			{ CreditAdj, Messages.PrintCreditAdj },
			{ DebitAdj, Messages.PrintDebitAdj },
			{ Check, Messages.PrintCheck },
			{ VoidCheck, Messages.PrintVoidCheck },
			{ Prepayment, Messages.PrintPrepayment },
			{ Refund,   Messages.PrintRefund },
            { VoidRefund,   Messages.PrintVoidRefund },
            { QuickCheck,   Messages.PrintQuickCheck },
			{ VoidQuickCheck, Messages.PrintVoidQuickCheck },
			{ CashReturn, Messages.PrintCashReturn },
		};

		public static readonly string[] Values = 
		{
			Invoice,
			CreditAdj,
			DebitAdj,
			Check,
			VoidCheck,
			Prepayment,
			Refund,
            VoidRefund,
            QuickCheck,
			VoidQuickCheck,
			CashReturn,
			PrepaymentRequest };
		public static readonly string[] Labels =
		{
			Messages.Invoice,
			Messages.CreditAdj,
			Messages.DebitAdj,
			Messages.Check,
			Messages.VoidCheck,
			Messages.Prepayment,
			Messages.Refund,
            Messages.VoidRefund,
            Messages.QuickCheck,
			Messages.VoidQuickCheck,
			Messages.CashReturn,
			Messages.PrepaymentRequest
		};

		public IEnumerable<ValueLabelPair> ValueLabelPairs => _valueLabelPairs;

		public IEnumerable<ValueLabelPair> ValueLabelPairsUI => _valueLabelPairsUI;

		public class ListAttribute : LabelListAttribute
		{
			public ListAttribute() : base(_valueLabelPairs)
			{ }
		}
		public class DocumentReleaseListAttribute : LabelListAttribute
		{
			public DocumentReleaseListAttribute() : base(_valueDocumentReleasableLabelPairs)
			{ }
		}

		public class ReleaseChecksListAttribute : LabelListAttribute
		{
			public ReleaseChecksListAttribute() : base(_valueCheckReleasableLabelPairs)
			{ }
		}

		/// <summary>
		/// Defines a Selector of the AP Document types with shorter description.<br/>
		/// In the screens displayed as combo-box.<br/>
		/// Mostly used in the reports.<br/>
		/// </summary>
		public class PrintListAttribute : LabelListAttribute
		{
			public PrintListAttribute() : base(_valuePrintableLabelPairs)
			{ }
		}

		/// <summary>
		/// Defines a list of the AP Document types, which are used for approval.
		/// </summary>
		public class APApprovalDocTypeListAttribute : LabelListAttribute
		{
			private static readonly IEnumerable<ValueLabelPair> _approvalValueLabelPairs = new ValueLabelList
			{
				{ Invoice, Messages.Invoice },
				{ CreditAdj, Messages.CreditAdj},
				{ DebitAdj, Messages.DebitAdj},
				{ PrepaymentRequest, Messages.PrepaymentRequest},
				{ Check, Messages.Check },
				{ QuickCheck, Messages.QuickCheck },
				{ Prepayment, Messages.Prepayment }
			};


			public APApprovalDocTypeListAttribute() : base(_approvalValueLabelPairs)
			{ }
		}


		public class invoice : PX.Data.BQL.BqlString.Constant<invoice>
		{
			public invoice() : base(Invoice) {; }
		}

		public class creditAdj : PX.Data.BQL.BqlString.Constant<creditAdj>
		{
			public creditAdj() : base(CreditAdj) {; }
		}

		public class debitAdj : PX.Data.BQL.BqlString.Constant<debitAdj>
		{
			public debitAdj() : base(DebitAdj) {; }
		}

		public class check : PX.Data.BQL.BqlString.Constant<check>
		{
			public check() : base(Check) {; }
		}

		public class voidCheck : PX.Data.BQL.BqlString.Constant<voidCheck>
		{
			public voidCheck() : base(VoidCheck) {; }
		}

		public class prepayment : PX.Data.BQL.BqlString.Constant<prepayment>
		{
			public prepayment() : base(Prepayment) {; }
		}

		public class refund : PX.Data.BQL.BqlString.Constant<refund>
		{
			public refund() : base(Refund) {; }
		}

        public class voidRefund : PX.Data.BQL.BqlString.Constant<voidRefund>
		{
            public voidRefund() : base(VoidRefund) { }
        }

        public class quickCheck : PX.Data.BQL.BqlString.Constant<quickCheck>
		{
			public quickCheck() : base(QuickCheck) {; }
		}

		public class voidQuickCheck : PX.Data.BQL.BqlString.Constant<voidQuickCheck>
		{
			public voidQuickCheck() : base(VoidQuickCheck) {; }
		}
		public class prepaymentRequest : PX.Data.BQL.BqlString.Constant<prepaymentRequest>
		{
			public prepaymentRequest() : base(PrepaymentRequest) {; }
		}

		public class cashReturn : PX.Data.BQL.BqlString.Constant<cashReturn>
		{
			public cashReturn() : base(CashReturn) {; }
		}

		public static string DocClass(string DocType)
		{
			switch (DocType)
			{
				case Invoice:
				case CreditAdj:
				case DebitAdj:
				case QuickCheck:
				case VoidQuickCheck:
				case CashReturn:
					return GLTran.tranClass.Normal;
				case Check:
				case VoidCheck:
				case Refund:
                case VoidRefund:
                    return GLTran.tranClass.Payment;
				case Prepayment:
					return GLTran.tranClass.Charge;
				default:
					return null;
			}
		}

		public static bool? Payable(string DocType)
		{
			switch (DocType)
			{
				case Invoice:
				case CreditAdj:
					return true;
				case Check:
				case DebitAdj:
				case VoidCheck:
				case Prepayment:
				case Refund:
                case VoidRefund:
                case QuickCheck:
				case VoidQuickCheck:
				case CashReturn:
					return false;
				default:
					return null;
			}
		}

		public static Int16? SortOrder(string DocType)
		{
			switch (DocType)
			{
				case Invoice:
				case CreditAdj:
				case QuickCheck:
					return 0;
				case Prepayment:
					return 1;
				case DebitAdj:
					return 2;
				case Check:
					return 3;
				case VoidCheck:
				case VoidQuickCheck:
				case CashReturn:
					return 4;
				case Refund:
                    return 5;
				case VoidRefund:
					return 6;
				default:
					return null;
			}
		}

		public static Decimal? SignBalance(string DocType)
		{
			switch (DocType)
			{
				case Refund:
                case VoidRefund:
                case Invoice:
				case CreditAdj:
					return 1m;
				case DebitAdj:
				case Check:
				case VoidCheck:
					return -1m;
				case Prepayment:
					return -1m;
				case QuickCheck:
				case VoidQuickCheck:
				case CashReturn:
					return 0m;
				default:
					return null;
			}
		}

		public static Decimal? SignAmount(string DocType)
		{
			switch (DocType)
			{
				case Refund:
                case VoidRefund:
                case Invoice:
				case CreditAdj:
				case QuickCheck:
					return 1m;
				case DebitAdj:
				case Check:
				case VoidCheck:
				case VoidQuickCheck:
				case CashReturn:
					return -1m;
				case Prepayment:
					return -1m;
				default:
					return null;
			}
		}

		public static string TaxDrCr(string DocType)
		{
			switch (DocType)
			{
				//Invoice Types
				case Invoice:
				case CreditAdj:
				case QuickCheck:
					return DrCr.Debit;
				case DebitAdj:
				case VoidQuickCheck:
				case CashReturn:
					return DrCr.Credit;
				//Payment Types
				case Check:
				case Prepayment:
				case VoidCheck:
					return DrCr.Debit;
				case Refund:
                case VoidRefund:
                    return DrCr.Credit;
				default:
					return DrCr.Debit;
			}
		}
		public static bool? HasNegativeAmount(string docType)
		{
			switch (docType)
			{
				case Refund:
				case Invoice:
				case CreditAdj:
				case QuickCheck:
				case DebitAdj:
				case Check:
				case Prepayment:
				case VoidQuickCheck:
				case CashReturn:
					return false;
				case VoidCheck:
                case VoidRefund:
                    return true;
				default:
					return null;
			}
		}

		/// <summary>
		/// Returns <c>true</c> if the specified document type
		/// has both Invoice and Payment parts related to one
		/// <see cref="ARRegister"/> record.
		/// </summary>
		public static bool HasBothInvoiceAndPaymentParts(string docType)
		{
			switch (docType)
			{
				case DebitAdj:
				case QuickCheck:
				case VoidQuickCheck:
				case CashReturn:
				case Prepayment:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Query if "Pre-Release" process allowed for the AP document type.
		/// </summary>
		/// <param name="docType">Type of the AP document.</param>
		/// <returns/>
		public static bool IsPrebookingAllowedForType(string docType)
		{
			switch (docType)
			{
				case Invoice:
				case CreditAdj:
				case DebitAdj:
				case QuickCheck:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Get display name of the AP document type.
		/// </summary>
		/// <param name="docType">Type of the AP document.</param>
		/// <returns/>
		public static string GetDisplayName(string docType)
		{
			return _valueLabelPairs.FirstOrDefault(a => a.Value == docType).Label;
		}
	}
}

