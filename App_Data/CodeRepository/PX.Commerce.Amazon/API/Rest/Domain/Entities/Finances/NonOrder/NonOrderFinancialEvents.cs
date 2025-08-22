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

using Newtonsoft.Json;
using PX.Commerce.Core;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon.API.Rest
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class NonOrderFinancialEvents : BCAPIEntity
	{
		/// <summary>
		/// Gets or sets a list of shipment settle events.
		/// </summary>
		/// <value>A list of shipment settle events.</value>
		[JsonProperty("ShipmentSettleEventList")]
		public List<ShipmentEvent> ShipmentSettleEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of guarantee claim events.
		/// </summary>
		/// <value>A list of guarantee claim events.</value>
		[JsonProperty("GuaranteeClaimEventList")]
		public List<ShipmentEvent> GuaranteeClaimEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of chargeback events.
		/// </summary>
		/// <value>A list of chargeback events.</value>
		[JsonProperty("ChargebackEventList")]
		public List<ShipmentEvent> ChargebackEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of events related to the seller's Pay with Amazon account.
		/// </summary>
		/// <value>A list of events related to the seller's Pay with Amazon account.</value>
		[JsonProperty("PayWithAmazonEventList")]
		public List<PayWithAmazonEvent> PayWithAmazonEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of information about solution provider credits.
		/// </summary>
		/// <value>A list of information about solution provider credits.</value>
		[JsonProperty("ServiceProviderCreditEventList")]
		public List<SolutionProviderCreditEvent> ServiceProviderCreditEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of information about Retrocharge or RetrochargeReversal events.
		/// </summary>
		/// <value>A list of information about Retrocharge or RetrochargeReversal events.</value>
		[JsonProperty("RetrochargeEventList")]
		public List<RetrochargeEvent> RetrochargeEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of rental transaction event information.
		/// </summary>
		/// <value>A list of rental transaction event information.</value>
		[JsonProperty("RentalTransactionEventList")]
		public List<RentalTransactionEvent> RentalTransactionEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of sponsored products payment events.
		/// </summary>
		/// <value>A list of sponsored products payment events.</value>
		[JsonProperty("ProductAdsPaymentEventList")]
		public List<ProductAdsPaymentEvent> ProductAdsPaymentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of information about service fee events.
		/// </summary>
		/// <value>A list of information about service fee events.</value>
		[JsonProperty("ServiceFeeEventList")]
		public List<ServiceFeeEvent> ServiceFeeEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of payment events for deal-related fees.
		/// </summary>
		/// <value>A list of payment events for deal-related fees.</value>
		[JsonProperty("SellerDealPaymentEventList")]
		public List<SellerDealPaymentEvent> SellerDealPaymentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of debt recovery event information.
		/// </summary>
		/// <value>A list of debt recovery event information.</value>
		[JsonProperty("DebtRecoveryEventList")]
		public List<DebtRecoveryEvent> DebtRecoveryEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of loan servicing events.
		/// </summary>
		/// <value>A list of loan servicing events.</value>
		[JsonProperty("LoanServicingEventList")]
		public List<LoanServicingEvent> LoanServicingEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of adjustment event information for the seller's account.
		/// </summary>
		/// <value>A list of adjustment event information for the seller's account.</value>
		[JsonProperty("AdjustmentEventList")]
		public List<AdjustmentEvent> AdjustmentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of the SAFE-T claim reimbursement on the seller's account.
		/// </summary>
		/// <value>A list of the SAFE-T claim reimbursement on the seller's account.</value>
		[JsonProperty("SAFETReimbursementEventList")]
		public List<SAFETReimbursementEvent> SAFETReimbursementEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of information about fee events for the Early Reviewer Program.
		/// </summary>
		/// <value>A list of information about fee events for the Early Reviewer Program.</value>
		[JsonProperty("SellerReviewEnrollmentPaymentEventList")]
		public List<SellerReviewEnrollmentPaymentEvent> SellerReviewEnrollmentPaymentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of FBA inventory liquidation payment events.
		/// </summary>
		/// <value>A list of FBA inventory liquidation payment events.</value>
		[JsonProperty("FBALiquidationEventList")]
		public List<FBALiquidationEvent> FBALiquidationEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of coupon payment event information.
		/// </summary>
		/// <value>A list of coupon payment event information.</value>
		[JsonProperty("CouponPaymentEventList")]
		public List<CouponPaymentEvent> CouponPaymentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of fee events related to Amazon Imaging services.
		/// </summary>
		/// <value>A list of fee events related to Amazon Imaging services.</value>
		[JsonProperty("ImagingServicesFeeEventList")]
		public List<ImagingServicesFeeEvent> ImagingServicesFeeEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of network commingling transaction events.
		/// </summary>
		/// <value>A list of network commingling transaction events.</value>
		[JsonProperty("NetworkComminglingTransactionEventList")]
		public List<NetworkComminglingTransactionEvent> NetworkComminglingTransactionEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of expense information related to an affordability promotion.
		/// </summary>
		/// <value>A list of expense information related to an affordability promotion.</value>
		[JsonProperty("AffordabilityExpenseEventList")]
		public List<AffordabilityExpenseEvent> AffordabilityExpenseEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of expense information related to an affordability promotion.
		/// </summary>
		/// <value>A list of expense information related to an affordability promotion.</value>
		[JsonProperty("AffordabilityExpenseReversalEventList")]
		public List<AffordabilityExpenseEvent> AffordabilityExpenseReversalEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of removal shipment event information.
		/// </summary>
		/// <value>A list of removal shipment event information.</value>
		[JsonProperty("RemovalShipmentEventList")]
		public List<RemovalShipmentEvent> RemovalShipmentEventList { get; set; }

		/// <summary>
		/// Gets or sets a comma-delimited list of Removal shipmentAdjustment details for FBA inventory.
		/// </summary>
		/// <value>A comma-delimited list of Removal shipmentAdjustment details for FBA inventory.</value>
		[JsonProperty("RemovalShipmentAdjustmentEventList")]
		public List<RemovalShipmentAdjustmentEvent> RemovalShipmentAdjustmentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of information about trial shipment financial events.
		/// </summary>
		/// <value>A list of information about trial shipment financial events.</value>
		[JsonProperty("TrialShipmentEventList")]
		public List<TrialShipmentEvent> TrialShipmentEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of events related to a Tax-Deducted-at-Source (TDS) reimbursement.
		/// </summary>
		/// <value>A list of events related to a Tax-Deducted-at-Source (TDS) reimbursement.</value>
		[JsonProperty("TDSReimbursementEventList")]
		public List<TDSReimbursementEvent> TDSReimbursementEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of events related to an Adhoc Disbursement.
		/// </summary>
		/// <value>A list of events related to an Adhoc Disbursement.</value>
		[JsonProperty("AdhocDisbursementEventList")]
		public List<AdhocDisbursementEvent> AdhocDisbursementEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of TaxWithholding events on seller's account.
		/// </summary>
		/// <value>A list of TaxWithholding events on seller's account.</value>
		[JsonProperty("TaxWithholdingEventList")]
		public List<TaxWithholdingEvent> TaxWithholdingEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of charge refund events.
		/// </summary>
		/// <value>A list of charge refund events.</value>
		[JsonProperty("ChargeRefundEventList")]
		public List<ChargeRefundEvent> ChargeRefundEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of failed ad hoc disbursement events.
		/// </summary>
		/// <value>A list of failed ad hoc disbursement events.</value>
		[JsonProperty("FailedAdhocDisbursementEventList")]
		public List<FailedAdhocDisbursementEvent> FailedAdhocDisbursementEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of events related to a value added service charge.
		/// </summary>
		/// <value>A list of events related to a value added service charge.</value>
		[JsonProperty("ValueAddedServiceChargeEventList")]
		public List<ValueAddedServiceChargeEvent> ValueAddedServiceChargeEventList { get; set; }

		/// <summary>
		/// Gets or sets a list of events related to a capacity reservation billing charge.
		/// </summary>
		/// <value>A list of events related to a capacity reservation billing charge.</value>
		[JsonProperty("CapacityReservationBillingEventList")]
		public List<CapacityReservationBillingEvent> CapacityReservationBillingEventList { get; set; }

		/// <summary>
		/// Defines if events of the instance are not empty.
		/// </summary>
		/// <returns>True if events of the instance are not empty; otherwise - false.</returns>
		public bool IsValid
		{
			get => this.ShipmentSettleEventList?.Any() == true
				|| this.GuaranteeClaimEventList?.Any() == true
				|| this.ChargebackEventList?.Any() == true
				|| this.PayWithAmazonEventList?.Any() == true
				|| this.ServiceProviderCreditEventList?.Any() == true
				|| this.RetrochargeEventList?.Any() == true
				|| this.RentalTransactionEventList?.Any() == true
				|| this.ProductAdsPaymentEventList?.Any() == true
				|| this.ServiceFeeEventList?.Any() == true
				|| this.SellerDealPaymentEventList?.Any() == true
				|| this.DebtRecoveryEventList?.Any() == true
				|| this.LoanServicingEventList?.Any() == true
				|| this.AdjustmentEventList?.Any() == true
				|| this.SAFETReimbursementEventList?.Any() == true
				|| this.SellerReviewEnrollmentPaymentEventList?.Any() == true
				|| this.FBALiquidationEventList?.Any() == true
				|| this.CouponPaymentEventList?.Any() == true
				|| this.ImagingServicesFeeEventList?.Any() == true
				|| this.NetworkComminglingTransactionEventList?.Any() == true
				|| this.AffordabilityExpenseEventList?.Any() == true
				|| this.AffordabilityExpenseReversalEventList?.Any() == true
				|| this.RemovalShipmentEventList?.Any() == true
				|| this.RemovalShipmentAdjustmentEventList?.Any() == true
				|| this.TrialShipmentEventList?.Any() == true
				|| this.TDSReimbursementEventList?.Any() == true
				|| this.AdhocDisbursementEventList?.Any() == true
				|| this.TaxWithholdingEventList?.Any() == true
				|| this.ChargeRefundEventList?.Any() == true
				|| this.FailedAdhocDisbursementEventList?.Any() == true
				|| this.ValueAddedServiceChargeEventList?.Any() == true
				|| this.CapacityReservationBillingEventList?.Any() == true;
		}
	}
}
