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
using PX.Objects.AM.CacheExtensions;
using PX.Objects.IN;
using PX.Objects.AM.GraphExtensions;
using System;

namespace PX.Objects.AM.Attributes
{
    /// <summary>
    /// Default the mark for field value based on <see cref="InventoryItem"/> settings for <see cref="InventoryItemExt.AMDefaultMarkFor"/>
    /// (Use the attribute on the Mark for Field)
    /// </summary>
    public class DefaultMarkForAttribute : PXEventSubscriberAttribute, IPXFieldDefaultingSubscriber
    {
        protected int MarkForType;
        protected string InventoryFieldName = "InventoryID";
        protected object ValueIfMarkForMatched = true;
        protected object ValueIfMarkForNotMatched = false;
		protected IBqlCreator _Formula;

        public DefaultMarkForAttribute(int markForType)
        {
            MarkForType = markForType;
        }

		public DefaultMarkForAttribute(Type type)
        {
            Formula = type;
        }

		public virtual Type Formula
		{
			get { return _Formula?.GetType(); }
			set
			{
				_Formula = PXFormulaAttribute.InitFormula(value);
			}
		}

		public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
			if(_Formula != null)
			{
				bool? result = null;
				object value = null;
				BqlFormula.Verify(sender, e.Row, _Formula, ref result, ref value);
				MarkForType = (int?)value?? 0;
			}

            var inventoryId = GetInventoryId(sender, e.Row);
            var markForType = GetMarkForType(sender, e.Row, inventoryId);
            if (inventoryId == null)
            {
                e.NewValue = ValueIfMarkForNotMatched;
            }
			else if (markForType == null || MarkForType != markForType)
            {
                e.NewValue = ValueIfMarkForNotMatched;
            }
			else
			{
				e.NewValue = MarkForType != 0? ValueIfMarkForMatched:ValueIfMarkForNotMatched;
			}
        }

        protected virtual int? GetInventoryId(PXCache cache, object data)
        {
            return (int?)cache.GetValue(data, InventoryFieldName);
        }

        protected virtual int? GetMarkForType(PXCache cache, object data, int? inventoryId)
        {
            return GetInventoryItem(cache, data, inventoryId)?.GetExtension<InventoryItemExt>()?.AMDefaultMarkFor;
        }

        protected virtual InventoryItem GetInventoryItem(PXCache cache, object data, int? inventoryId)
        {
            return (InventoryItem)PXSelectorAttribute.Select(cache, data, InventoryFieldName)
                ?? InventoryItem.PK.Find(cache.Graph, inventoryId);
        }
    }
}
