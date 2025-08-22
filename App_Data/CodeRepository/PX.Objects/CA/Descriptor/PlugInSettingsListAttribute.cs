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

using PX.ACHPlugInBase;
using PX.Common;
using PX.Data;
using System;
using System.Collections.Generic;

namespace PX.Objects.CA
{
	public class PlugInSettingsListAttribute : PXAggregateAttribute
	{
		protected class PXFieldValuesListInternal : PXStringListAttribute, IPXFieldUpdatingSubscriber
		{
			private readonly PXGraph _Graph;
			private readonly Type _CacheItemType;
			private readonly string _CacheFieldName;
			///<param name="graphType">Graph that contains cache with the field.</param>
			///<param name="fieldType">Field that need to be provided with value.</param>
			public PXFieldValuesListInternal(Type graphType, Type fieldType) : base()
			{
				if (fieldType == null)
					throw new PXArgumentException(nameof(fieldType), ErrorMessages.ArgumentNullException);

				if (!fieldType.IsNested || !typeof(IBqlField).IsAssignableFrom(fieldType))
					throw new PXArgumentException(nameof(fieldType), ErrorMessages.CantCreateForeignKeyReference, fieldType);

				_Graph = PXGraph.CreateInstance(graphType);
				_CacheItemType = BqlCommand.GetItemType(fieldType);
				_CacheFieldName = fieldType.Name;
			}

			internal void SetList(PXCache cache, string[] values, string[] labels)
			{
				cache.SetAltered(FieldName, true);
				SetListInternal(this.AsSingleEnumerable(), values, labels, cache);
			}

			public void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
			{
				if (e.NewValue != null)
				{
					var row = (ACHPlugInParameter)e.Row;

					if (row.Type == (int)SelectorType.RemittancePaymentMethodDetail
							|| row.Type == (int)SelectorType.VendorPaymentMethodDetail)
					{
						e.NewValue = GetPaymentDetailID(sender.Graph, (SelectorType)row.Type, e.NewValue.ToString());
					}
					else
					{
						e.NewValue = e.NewValue.ToString();
					}
				}
			}

			private Dictionary<string, string> _RemittanceDetailsDesc;
			private Dictionary<string, string> _VendorDetailsDesc;

			private string GetPaymentDetailID(PXGraph graph, SelectorType type, string value)
			{
				if (string.IsNullOrEmpty(value))
				{
					return value;
				}

				string id;

				switch(type)
				{
					case SelectorType.RemittancePaymentMethodDetail:
						if(_RemittanceDetailsDesc == null)
						{
							_RemittanceDetailsDesc = new Dictionary<string, string>();
							foreach(PaymentMethodDetail detail in (graph as PaymentMethodMaint).DetailsForCashAccount.Select())
							{
								_RemittanceDetailsDesc.Add(detail.Descr, detail.DetailID);
							}
						}

						return _RemittanceDetailsDesc.TryGetValue(value, out id) ? id : value;
					case SelectorType.VendorPaymentMethodDetail:
						if (_VendorDetailsDesc == null)
						{
							_VendorDetailsDesc = new Dictionary<string, string>();
							foreach (PaymentMethodDetail detail in (graph as PaymentMethodMaint).DetailsForVendor.Select())
							{
								_VendorDetailsDesc.Add(detail.Descr, detail.DetailID);
							}
						}

						return _VendorDetailsDesc.TryGetValue(value, out id) ? id : value;
					default:
						throw new NotImplementedException();
				}
			}


			private Dictionary<string, string> _RemittanceDetailsID;
			private Dictionary<string, string> _VendorDetailsID;

			private string GetPaymentDetailDesc(PXGraph graph, SelectorType type, string value)
			{
				if(string.IsNullOrEmpty(value))
				{
					return value;
				}

				string desc;

				switch (type)
				{
					case SelectorType.RemittancePaymentMethodDetail:
						if (_RemittanceDetailsID == null)
						{
							_RemittanceDetailsID = new Dictionary<string, string>();
							foreach (PaymentMethodDetail detail in (graph as PaymentMethodMaint).DetailsForCashAccount.Select())
							{
								_RemittanceDetailsID.Add(detail.DetailID, detail.Descr);
							}
						}

						return _RemittanceDetailsID.TryGetValue(value, out desc) ? desc : value;
					case SelectorType.VendorPaymentMethodDetail:
						if (_VendorDetailsID == null)
						{
							_VendorDetailsID = new Dictionary<string, string>();
							foreach (PaymentMethodDetail detail in (graph as PaymentMethodMaint).DetailsForVendor.Select())
							{
								_VendorDetailsID.Add(detail.DetailID, detail.Descr);
							}
						}

						return _VendorDetailsID.TryGetValue(value, out desc) ? desc : value;
					default:
						throw new NotImplementedException();
				}
			}

			public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
			{
				var row = (ACHPlugInParameter)e.Row;

				if (string.IsNullOrEmpty(row?.ParameterID))
				{
					return;
				}

				if (row?.IsGroupHeader == true)
				{
					e.ReturnState = PXStringState.CreateInstance(e.ReturnState, typeof(string), null, null, null, null, null, null, "Value", null, null, null, PXErrorLevel.Undefined, enabled: false);

					return;
				}

				var errors = PXUIFieldAttribute.GetErrorWithLevel<ACHPlugInParameter.value>(sender, e.Row);

				var settings = (sender.Graph as PaymentMethodMaint).GetPlugInSettings();
				var selectorTypes = (sender.Graph as PaymentMethodMaint).GetPlugInSelectorTypes();

				if (!settings.TryGetValue(row?.ParameterID, out var cacheFieldName) && row?.Type != null)
				{
					selectorTypes.TryGetValue((SelectorType?)row?.Type, out cacheFieldName);
				}

				if (string.IsNullOrEmpty(cacheFieldName))
				{
					base.FieldSelecting(sender, e);
				}
				else if (!string.IsNullOrEmpty(row?.ParameterID))
				{
					Type cacheItemType = _CacheItemType;
					object erow = e.Row;
					cacheItemType = typeof(ACHPlugInSettings);

					if (!sender.Graph.Views.Caches.Contains(typeof(ACHPlugInSettings)))
						sender.Graph.Views.Caches.Add(typeof(ACHPlugInSettings));

					if (sender.Graph.Caches[cacheItemType]?.GetStateExt(null, cacheFieldName) is PXFieldState state)
					{
						state.Enabled = true;
						state.Visible = true;
						state.DescriptionName = null;
						state.Value = sender.GetValue(e.Row, FieldName);

						if (row.Type == (int)SelectorType.RemittancePaymentMethodDetail
							|| row.Type == (int)SelectorType.VendorPaymentMethodDetail)
						{
							state.Value = GetPaymentDetailDesc(sender.Graph, (SelectorType)row.Type, state.Value?.ToString());
						}

						if (state.Value == null && state.DataType == typeof(bool))
						{
							sender.SetValue(e.Row, FieldName, Boolean.FalseString);
						}

						if (!string.IsNullOrEmpty(errors.errorMessage))
						{
							state.Error = errors.errorMessage;
							state.ErrorLevel = errors.errorLevel;
						}
						else
						{
							state.Error = null;
							state.ErrorLevel = PXErrorLevel.Undefined;
						}
						e.Cancel = true;
						e.ReturnState = state;
					}
				}
			}
		}

		protected PXFieldValuesListInternal ListAttr => (PXFieldValuesListInternal)_Attributes[0];
		protected PXDBStringAttribute DBStringAttr => (PXDBStringAttribute)_Attributes[1];

		public bool ExclusiveValues
		{
			get { return ListAttr.ExclusiveValues; }
			set { ListAttr.ExclusiveValues = value; }
		}

		public bool IsKey
		{
			get { return DBStringAttr.IsKey; }
			set { DBStringAttr.IsKey = value; }
		}

		///<param name="length">The maximum length of a field value.</param>
		///<param name="graphType">Graph that contains cache with the field.</param>
		///<param name="fieldType">Field that need to be provided with value.</param>
		public PlugInSettingsListAttribute(int length, Type graphType, Type fieldType) : base()
		{
			// Order is important (!)
			_Attributes.Add(new PXFieldValuesListInternal(graphType, fieldType));
			_Attributes.Add(new PXDBStringAttribute(length) { InputMask = "", IsUnicode = true });
		}

		internal void SetList(PXCache cache, string[] values, string[] labels)
		{
			ListAttr.SetList(cache, values, labels);
		}
	}
}
