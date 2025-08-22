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
using PX.Commerce.Amazon.API.Rest.Interfaces;
using PX.Commerce.Core;
using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PX.Commerce.Amazon.API.Rest.Converters
{
	public class NonOrderFinancialEventsToNonOrderFeeConverter : IConverter<NonOrderFinancialEvents, IEnumerable<NonOrderFee>>
	{
		private readonly Dictionary<Type, Func<IExternEntity, IEnumerable<NonOrderFee>>> converters;

		public NonOrderFinancialEventsToNonOrderFeeConverter(IConverter<FeeComponent, NonOrderFee> feeConverter, IConverter<ChargeComponent, NonOrderFee> chargeConverter)
		{
			this.converters = new Dictionary<Type, Func<IExternEntity, IEnumerable<NonOrderFee>>>
			{
				{ typeof(ShipmentEvent), (entity) => new ShipmentEventToNonOrderFeeConverter(feeConverter).Convert(entity as ShipmentEvent) },
				{ typeof(AdhocDisbursementEvent), (entity) => new AdhocDisbursementEventToNonOrderFeeConverter().Convert(entity as AdhocDisbursementEvent) },
				{ typeof(AdjustmentEvent), (entity) => new AdjustmentEventToNonOrderFeeConverter().Convert(entity as AdjustmentEvent) },
				{ typeof(AffordabilityExpenseEvent), (entity) => new AffordabilityExpenseEventToNonOrderFeeConverter().Convert(entity as AffordabilityExpenseEvent) },
				{ typeof(CapacityReservationBillingEvent), (entity) => new CapacityReservationBillingEventToNonOrderFeeConverter().Convert(entity as CapacityReservationBillingEvent) },
				{ typeof(ChargeRefundEvent), (entity) => new ChargeRefundEventToNonOrderFeeConverter().Convert(entity as ChargeRefundEvent) },
				{ typeof(CouponPaymentEvent), (entity) => new CouponPaymentEventToNonOrderFeeConverter(feeConverter, chargeConverter).Convert(entity as CouponPaymentEvent) },
				{ typeof(DebtRecoveryEvent), (entity) => new DebtRecoveryEventToNonOrderFeeConverter().Convert(entity as DebtRecoveryEvent) },
				{ typeof(FailedAdhocDisbursementEvent), (entity) => new FailedAdhocDisbursementEventToNonOrderFeeConverter().Convert(entity as FailedAdhocDisbursementEvent) },
				{ typeof(FBALiquidationEvent), (entity) => new FBALiquidationEventToNonOrderFeeConverter().Convert(entity as FBALiquidationEvent) },
				{ typeof(ImagingServicesFeeEvent), (entity) => new ImagingServicesFeeEventToNonOrderFeeConverter(feeConverter).Convert(entity as ImagingServicesFeeEvent) },
				{ typeof(LoanServicingEvent), (entity) => new LoanServicingEventToNonOrderFeeConverter().Convert(entity as LoanServicingEvent) },
				{ typeof(NetworkComminglingTransactionEvent), (entity) => new NetworkComminglingTransactionEventToNonOrderFeeConverter().Convert(entity as NetworkComminglingTransactionEvent) },
				{ typeof(PayWithAmazonEvent), (entity) => new PayWithAmazonEventToNonOrderFeeConverter(feeConverter).Convert(entity as PayWithAmazonEvent) },
				{ typeof(ProductAdsPaymentEvent), (entity) => new ProductAdsPaymentEventToNonOrderFeeConverter().Convert(entity as ProductAdsPaymentEvent) },
				{ typeof(RemovalShipmentAdjustmentEvent), (entity) => new RemovalShipmentAdjustmentEventToNonOrderFeeConverter().Convert(entity as RemovalShipmentAdjustmentEvent) },
				{ typeof(RemovalShipmentEvent), (entity) => new RemovalShipmentEventToNonOrderFeeConverter().Convert(entity as RemovalShipmentEvent) },
				{ typeof(RentalTransactionEvent), (entity) => new RentalTransactionEventToNonOrderFeeConverter(feeConverter, chargeConverter).Convert(entity as RentalTransactionEvent) },
				{ typeof(RetrochargeEvent), (entity) => new RetrochargeEventToNonOrderFeeConverter(chargeConverter).Convert(entity as RetrochargeEvent) },
				{ typeof(SAFETReimbursementEvent), (entity) => new SAFETReimbursementEventToNonOrderFeeConverter().Convert(entity as SAFETReimbursementEvent) },
				{ typeof(SellerDealPaymentEvent), (entity) => new SellerDealPaymentEventToNonOrderFeeConverter().Convert(entity as SellerDealPaymentEvent) },
				{ typeof(SellerReviewEnrollmentPaymentEvent), (entity) => new SellerReviewEnrollmentPaymentEventToNonOrderFeeConverter(feeConverter, chargeConverter).Convert(entity as SellerReviewEnrollmentPaymentEvent) },
				{ typeof(ServiceFeeEvent), (entity) => new ServiceFeeEventToNonOrderFeeConverter(feeConverter).Convert(entity as ServiceFeeEvent) },
				{ typeof(SolutionProviderCreditEvent), (entity) => new SolutionProviderCreditEventToNonOrderFeeConverter().Convert(entity as SolutionProviderCreditEvent) },
				{ typeof(TaxWithholdingEvent), (entity) => new TaxWithholdingEventToNonOrderFeeConverter().Convert(entity as TaxWithholdingEvent) },
				{ typeof(TDSReimbursementEvent), (entity) => new TDSReimbursementEventToNonOrderFeeConverter().Convert(entity as TDSReimbursementEvent) },
				{ typeof(TrialShipmentEvent), (entity) => new TrialShipmentEventToNonOrderFeeConverter(feeConverter).Convert(entity as TrialShipmentEvent) },
				{ typeof(ValueAddedServiceChargeEvent), (entity) => new ValueAddedServiceChargeEventToNonOrderFeeConverter().Convert(entity as ValueAddedServiceChargeEvent) },
			};
		}

		public IEnumerable<NonOrderFee> Convert(NonOrderFinancialEvents source)
		{
			if (source is null)
				throw new PXArgumentException(nameof(source), ErrorMessages.ArgumentNullException);


			foreach (var property in source.GetType().GetProperties())
			{
				//
				// Only those property have JsonPropertyAttribute should be considered for converting.
				//
				if (property.GetCustomAttribute<JsonPropertyAttribute>() is null)
					continue;

				Type propertyType = property.PropertyType;

				if (!propertyType.IsGenericType
					|| propertyType.GetGenericTypeDefinition() != typeof(List<>))
					continue;

				Type listItemType = propertyType.GetGenericArguments()[0];

				if (!this.converters.TryGetValue(listItemType, out Func<IExternEntity, IEnumerable<NonOrderFee>> converterFunc) || converterFunc is null)
				{
					//
					// Log "There is no appropriate converter for the type {sourceType}. It should be implemented.", listItemType.ToString()
					//
					continue;
				}

				foreach (IExternEntity financialEvent in property.GetValue(source) as IEnumerable<IExternEntity>)
				{
					var nonOrderFees = converterFunc(financialEvent);

					foreach (var nonOrderFee in nonOrderFees)
					{
						nonOrderFee.FeeDescription = $"{financialEvent.GetType().Name} - {nonOrderFee.FeeDescription}";

						yield return nonOrderFee;
					}						
				}
			}
		}
	}
}
