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
using PX.Commerce.Objects;
using PX.Data;
using PX.SM;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon.Amazon.DAC
{
	public class BCFeeMappingSlotAmazon : IPrefetchable<BCFeeMappingSlotAmazonFilter>
	{
		private readonly List<BCFeeMapping> _fees;

		public BCFeeMappingSlotAmazon()
		{
			_fees = new List<BCFeeMapping>();
		}

		public static List<BCFeeMapping> Get(BCFeeMappingSlotAmazonFilter filter)
		{
			if (filter is null)
				return new List<BCFeeMapping>();

			var slot = PXDatabase.GetSlot<BCFeeMappingSlotAmazon, BCFeeMappingSlotAmazonFilter>(nameof(BCFeeMappingSlotAmazon) + filter.BindingId.ToString(), filter, typeof(BCFeeMapping));

			if (!filter.PaymentMappingID.HasValue)
				return slot._fees;

			return slot._fees.Where(x => x.PaymentMappingID == filter.PaymentMappingID).ToList();
		}

		public void Prefetch(BCFeeMappingSlotAmazonFilter filter)
		{
			_fees.Clear();

			foreach (PXDataRecord row in PXDatabase.SelectMulti<BCFeeMapping>(
					new PXAliasedDataField<BCFeeMapping.bindingID>(),
					new PXAliasedDataField<BCFeeMapping.entryTypeID>(),
					new PXAliasedDataField<BCFeeMapping.paymentMappingID>(),
					new PXAliasedDataField<BCFeeMapping.feeType>(),
					new PXDataField(nameof(BCFeeMappingExtAmazon.FeeDescription), nameof(BCFeeMapping)),
					new PXDataField(nameof(BCFeeMappingExtAmazon.Active), nameof(BCFeeMapping)),
					new PXDataFieldValue<BCFeeMapping.bindingID>(PXDbType.Int, filter.BindingId)))
			{
				var feeMapping = filter.Cache.CreateInstance() as BCFeeMapping;

				feeMapping.BindingID = row.GetInt32(0);
				feeMapping.EntryTypeID = row.GetString(1);
				feeMapping.PaymentMappingID = row.GetInt32(2);
				feeMapping.FeeType = row.GetString(3);

				var feeMappingExt = feeMapping.GetExtension<BCFeeMappingExtAmazon>();
				feeMappingExt.FeeDescription = row.GetString(4);
				feeMappingExt.Active = row.GetBoolean(5);

				_fees.Add(feeMapping);
			}
		}
	}

	public class BCFeeMappingSlotAmazonFilter
	{
		public PXCache<BCFeeMapping> Cache { get; set; }

		public int BindingId { get; set; }

		public int? PaymentMappingID { get; set; }
	}
}
