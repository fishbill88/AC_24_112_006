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

using PX.Commerce.Amazon.Amazon.DAC;
using PX.Commerce.Amazon.Sync.Interfaces;
using PX.Commerce.Objects;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon.Sync.Helpers
{
	public abstract class PaymentMethodFeeTypeHandlerBase : IPaymentMethodFeeTypeHandler
	{
		private ConcurrentDictionary<string, BCFeeMapping> _existingFeeMappings;

		private BCPaymentMethods _paymentMethod;

		private readonly int _bindingId;

		private readonly PXCache<BCFeeMapping> _cache;

		private readonly PXGraph _graph;

		private object _lock = new object();

		public void AddMissedFeeTypes(IEnumerable<string> feeDescriptions)
		{
			if (feeDescriptions is null)
				throw new PXArgumentException(nameof(feeDescriptions));

			if (!feeDescriptions.Any())
				return;

			var notLinkedFeeTypes = new HashSet<string>();
			var missedFeeTypes = new HashSet<string>();

			foreach (string feeDescription in feeDescriptions)
			{
				if (!this.ExistingFeeMappings.TryGetValue(feeDescription, out BCFeeMapping feeMapping))
				{
					missedFeeTypes.Add(feeDescription);
					continue;
				}

				var feeMappingExt = feeMapping.GetExtension<BCFeeMappingExtAmazon>();

				if (feeMappingExt.Active == true && string.IsNullOrWhiteSpace(feeMapping.EntryTypeID))
					notLinkedFeeTypes.Add(feeMappingExt.FeeDescription);
			}

			foreach (string missedFeeTypeDescription in missedFeeTypes)
			{
				notLinkedFeeTypes.Add(missedFeeTypeDescription);

				BCFeeMapping newFeeMapping = this.CreateFeeMapping(missedFeeTypeDescription);
				_cache.Insert(newFeeMapping);
			}

			if (missedFeeTypes.Any())
				_cache.Persist(PXDBOperation.Insert);

			this.HandleNotLinkedFeeMappings(notLinkedFeeTypes);
			this.ExistingFeeMappings.Clear();
		}

		private BCFeeMapping CreateFeeMapping(string feeTypeDescription)
		{
			var feeMapping = _cache.CreateInstance() as BCFeeMapping;
			feeMapping.BindingID = _bindingId;
			feeMapping.PaymentMappingID = this.PaymentMethod.PaymentMappingID;
			feeMapping.FeeType = this.FeeType;

			feeMapping.GetExtension<BCFeeMappingExtAmazon>().FeeDescription = feeTypeDescription;

			return feeMapping;
		}

		private ConcurrentDictionary<string, BCFeeMapping> ExistingFeeMappings
		{
			get
			{
				lock (_lock)
				{
					if (_existingFeeMappings is null)
						return _existingFeeMappings = this.GetMappings();

					int feeMappingsCount = SelectFrom<BCFeeMapping>
						.AggregateTo<Count<BCFeeMapping.feeMappingID>>
						.View.Select(_graph).RowCount.Value;

					if (feeMappingsCount == _existingFeeMappings.Count)
						return _existingFeeMappings;
					else
						return _existingFeeMappings = this.GetMappings();
				}
			}
		}

		protected abstract string FeeType { get; }

		private ConcurrentDictionary<string, BCFeeMapping> GetMappings()
		{
			var filter = new BCFeeMappingSlotAmazonFilter
			{
				BindingId = _bindingId,
				PaymentMappingID = this.PaymentMethod.PaymentMappingID,
				Cache = _cache
			};

			var feeMappings = BCFeeMappingSlotAmazon
				.Get(filter)
				.Where(feeMapping => feeMapping.FeeType == this.FeeType)
				.ToDictionary(feeMapping => feeMapping.GetExtension<BCFeeMappingExtAmazon>().FeeDescription);

			return new ConcurrentDictionary<string, BCFeeMapping>(feeMappings);
		}

		private BCPaymentMethods GetPaymentMethod()
		{
			return BCPaymentMethodsMappingSlot
				.Get(_bindingId)
				.FirstOrDefault(paymentMethod => paymentMethod.Active.Value);
		}

		public BCFeeMapping GetStoredFeeMapping(string feeDescription)
		{
			if (string.IsNullOrWhiteSpace(feeDescription))
				throw new PXArgumentException(nameof(feeDescription), ErrorMessages.ArgumentNullException, nameof(feeDescription));

			if (this.ExistingFeeMappings.TryGetValue(feeDescription, out BCFeeMapping feeMapping))
				return feeMapping;

			throw new PXArgumentException(nameof(feeDescription), AmazonMessages.FeeMappingIsAbsent, feeDescription);
		}

		protected abstract void HandleNotLinkedFeeMappings(IEnumerable<string> notLinkedFeeTypes);

		protected BCPaymentMethods PaymentMethod => _paymentMethod ?? (_paymentMethod = this.GetPaymentMethod());

		public PaymentMethodFeeTypeHandlerBase(int bindingId, PXGraph graph)
		{
			if (graph is null)
				throw new PXArgumentException(nameof(graph));

			_bindingId = bindingId;
			_graph = graph;
			_cache = graph.Caches[typeof(BCFeeMapping)] as PXCache<BCFeeMapping>;
		}
	}
}
