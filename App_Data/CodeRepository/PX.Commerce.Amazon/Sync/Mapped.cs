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

using PX.Commerce.Amazon.API;
using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon
{
	#region BCMappedEntity
	public abstract class BCMappedEntity<ExternType, LocalType> : MappedEntity<ExternType, LocalType>
		where ExternType : BCAPIEntity, IExternEntity
		where LocalType : CBAPIEntity, ILocalEntity
	{
		public BCMappedEntity(String entType)
			: base(BCAmazonConnector.TYPE, entType)
		{ }
		public BCMappedEntity(BCSyncStatus status)
			: base(status)
		{
		}
		public BCMappedEntity(String entType, LocalType entity, Guid? id, DateTime? timestamp)
			: base(BCAmazonConnector.TYPE, entType, entity, id, timestamp)
		{
		}
		public BCMappedEntity(String entType, ExternType entity, String id, String description, DateTime? timestamp)
			: base(BCAmazonConnector.TYPE, entType, entity, id, description, timestamp)
		{
		}
		public BCMappedEntity(String entType, ExternType entity, String id, String description, String hash)
			: base(BCAmazonConnector.TYPE, entType, entity, id, description, hash)
		{
		}
	}
	#endregion

	#region SalesOrders

	public abstract class BCMappedOrderEntity : BCMappedEntity<Order, SalesOrder>
	{
		public BCMappedOrderEntity(String entType)
			: base(entType)
		{ }
		public BCMappedOrderEntity(BCSyncStatus status)
			: base(status)
		{ }
		public BCMappedOrderEntity(String entType, SalesOrder entity, Guid? id, DateTime? timestamp)
			: base(entType, entity, id, timestamp)
		{ }
		public BCMappedOrderEntity(String entType, Order entity, String id, String description, DateTime? timestamp)
			: base(entType, entity, id, description, timestamp)
		{ }
		public BCMappedOrderEntity(String entType, Order entity, String id, String description, String hash)
			: base(entType, entity, id, description, hash)
		{ }
	}

	public class MappedFBMOrder : BCMappedOrderEntity
	{
		public const String TYPE = BCEntitiesAttribute.Order;

		public MappedFBMOrder()
			: base(TYPE)
		{ }
		public MappedFBMOrder(SalesOrder entity, Guid? id, DateTime? timestamp)
			: base(TYPE, entity, id, timestamp)
		{ }
		public MappedFBMOrder(Order entity, String id, String description, DateTime? timestamp)
			: base(TYPE, entity, id, description, timestamp)
		{ }
	}

	public class MappedFBAOrder : BCMappedOrderEntity
	{
		public const String TYPE = BCEntitiesAttribute.OrderOfTypeInvoice;
		public MappedFBAOrder()
			: base(TYPE)
		{ }
		public MappedFBAOrder(SalesOrder entity, Guid? id, DateTime? timestamp)
		: base(TYPE, entity, id, timestamp)
		{ }
		public MappedFBAOrder(Order entity, String id, String description, DateTime? timestamp)
			: base(TYPE, entity, id, description, timestamp)
		{ }
	}
	#endregion

	#region MappedInvoice
	public class MappedInvoice : BCMappedEntity<Order, SalesInvoice>
	{
		public const String TYPE = BCEntitiesAttribute.SOInvoice;

		public MappedInvoice()
			: base(TYPE)
		{ }
		public MappedInvoice(SalesInvoice entity, Guid? id, DateTime? timestamp)
			: base(TYPE, entity, id, timestamp) { }
		public MappedInvoice(Order entity, String id, String description, DateTime? timestamp)
			: base(TYPE, entity, id, description, timestamp) { }
	}
    #endregion

    #region MappedPayment
    public class MappedPayment : BCMappedEntity<ExternalPaymentData, Payment>
    {
		public const String TYPE = BCEntitiesAttribute.Payment;

		public MappedPayment()
			: base(TYPE)
		{ }
		public MappedPayment(Payment entity, Guid? id, DateTime? timestamp)
			: base(TYPE, entity, id, timestamp) { }
		public MappedPayment(ExternalPaymentData entity, String id, String description, DateTime? timestamp)
			: base(TYPE, entity, id, description, timestamp) { }
	}
	#endregion

	#region MappedShipment
	public class MappedShipment : BCMappedEntity<AmazonFulfillmentData, BCShipments>
	{
		public const String TYPE = BCEntitiesAttribute.Shipment;

		public MappedShipment()
			: base(TYPE)
		{ }
		public MappedShipment(BCShipments entity, Guid? id, DateTime? timestamp)
			: base(TYPE, entity, id, timestamp) { }
		public MappedShipment(AmazonFulfillmentData entity, String id, String description, DateTime? timestamp, String hashcode)
			: base(TYPE, entity, id, description, timestamp) { ExternHash = hashcode; }
	}
    #endregion

    #region MappedStockItem
    public class MappedProductLinkingItem : BCMappedEntity<MerchantListingData, StockItem>
    {
        public const String TYPE = BCEntitiesAttribute.ProductLinkingOnly;

        public MappedProductLinkingItem()
            : base(TYPE)
        { }
        public MappedProductLinkingItem(StockItem entity, Guid? id, DateTime? timestamp)
            : base(TYPE, entity, id, timestamp) { }
        public MappedProductLinkingItem(MerchantListingData entity, String id, String description, DateTime? timestamp)
            : base(TYPE, entity, id, description, timestamp) { }
    }
    #endregion

    #region MappedAvailability
    public class MappedAvailability : BCMappedEntity<InventoryMessageData, StorageDetailsResult>
    {
        public const String TYPE = BCEntitiesAttribute.ProductAvailability;

        public MappedAvailability()
            : base(TYPE)
        { }
        public MappedAvailability(StorageDetailsResult entity, Guid? id, DateTime? timestamp, Int32? parent)
            : base(TYPE, entity, id, timestamp)
        {
            UpdateParentExternTS = true;
        }
        public MappedAvailability(InventoryMessageData entity, String id, String description, DateTime? timestamp, Int32? parent)
            : base(TYPE, entity, id, description, timestamp)
        {
            UpdateParentExternTS = true;
        }
    }

	#endregion

	#region MappedNonOrderFee
	public class MappedNonOrderFee : BCMappedEntity<NonOrderFeeGroup, CashTransactionGroup>
	{
		public MappedNonOrderFee()
			: base(BCEntitiesAttribute.NonOrderFee) { }

		public MappedNonOrderFee(NonOrderFeeGroup entity, String id, String description, DateTime? timestamp)
			: base(BCEntitiesAttribute.NonOrderFee, entity, id, description, timestamp) { }
	}
	#endregion
}
