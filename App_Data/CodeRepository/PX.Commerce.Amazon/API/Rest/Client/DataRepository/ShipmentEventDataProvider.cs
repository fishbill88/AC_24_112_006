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

using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Amazon.API.Rest.Client.Interface;
using PX.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API
{
	public class ShipmentEventDataProvider : IShipmentEventDataProvider
	{
		private IOrderFinanceEventsDataProvider FinanceEventsDataProvider { get; set; }

		public ShipmentEventDataProvider(IOrderFinanceEventsDataProvider financeEventsDataProvider)
		{
			this.FinanceEventsDataProvider = financeEventsDataProvider;
		}

		public async IAsyncEnumerable<ShipmentEvent> GetListOfShipmentEventsBy(string orderId, CancellationToken cancellationToken)
		{
			var filterByOrderId = new FinancialEventsFilterByOrder { OrderId = orderId };

			await foreach (OrderFinancialEvents financialEvents in this.FinanceEventsDataProvider.GetFinancialEventsByOrderAsync(filterByOrderId, cancellationToken))
			{
				if (!financialEvents.ShipmentEventList.Any())
					continue;

				var shipmentEvents = financialEvents.ShipmentEventList.OrderByDescending(shipmentEvent => shipmentEvent.PostedDate);

				foreach (var shipmentEvent in shipmentEvents)
					yield return shipmentEvent;
			}
		}

		public async Task<ShipmentEvent> GetShipmentEventBy(string orderId, int shipmentEventId, CancellationToken cancellationToken)
		{
			var shipmentEvents = new List<ShipmentEvent>();

			await foreach (var shipment in this.GetListOfShipmentEventsBy(orderId, cancellationToken))
				shipmentEvents.Add(shipment);

			if (!shipmentEvents.Any())
				return null;

			await ValidateShipmentEventId(orderId, shipmentEventId, shipmentEvents);

			return shipmentEvents[shipmentEventId - 1];
		}

		public async IAsyncEnumerable<ShipmentEvent> GetListOfShipmentEvents(FinancialEventsFilter filter, CancellationToken cancellationToken)
		{
			await foreach (OrderFinancialEvents financialEvents in this.FinanceEventsDataProvider.GetFinancialEventsAsync(filter, cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (!financialEvents.ShipmentEventList.Any())
					continue;

				var shipmentEvents = financialEvents.ShipmentEventList.OrderByDescending(shipmentEvent => shipmentEvent.PostedDate);

				foreach (var shipmentEvent in shipmentEvents)
					yield return shipmentEvent;
			}
		}

		static async Task ValidateShipmentEventId(string orderId, int shipmentEventId, List<ShipmentEvent> shipmentEvents)
		{
			if (shipmentEventId > shipmentEvents.Count || shipmentEventId < 1)
			{
				throw new PXException(AmazonMessages.TheExternalPaymentIDIsNotCorrectWithNumberOfShipmentEvents, orderId, shipmentEvents.Count);
			}
		}
	}
}
