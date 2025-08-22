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
using PX.Data.BQL;
using PX.Objects.Common.GraphExtensions.Abstract.DAC;
using PX.Objects.Common.GraphExtensions.Abstract.Mapping;
using System;

namespace PX.Objects.Common.GraphExtensions.Abstract
{
	public abstract class TransactionZeroBaseQtyValidationExtension<TGraph, TDocument, TDocumentHoldField> : PXGraphExtension<TGraph>
		where TGraph : PXGraph
		where TDocument: class, IBqlTable, new()
		where TDocumentHoldField : BqlBool.Field<TDocumentHoldField>
	{
		[PXHidden]
		public PXSelectExtension<TransactionLineQty> Transactions;

		protected abstract TransactionLineQtyMapping GetTranLineMapping();

		#region Event Handlers

		[PXOverride]
		public virtual void Persist(Action basePersist)
		{
			var hold = (bool?)Base.Caches<TDocument>().GetValue<TDocumentHoldField>(Base.Caches<TDocument>().Current);

			// you can't save a document in Balanced status when Base Qty is 0 for at least one Details line
			if (hold != true)
			{
				foreach (TransactionLineQty tran in Transactions.Select())
				{
					if (tran.Qty != decimal.Zero && tran.BaseQty == decimal.Zero)
					{
						var item = IN.InventoryItem.PK.Find(Base, tran.InventoryID);
						var fieldName = typeof(TransactionLineQty.qty).Name;

						Transactions.Cache.MarkUpdated(tran);
						if (Transactions.Cache.RaiseExceptionHandling(fieldName, tran, tran.Qty,
							new PXSetPropertyException(Messages.CannotSaveLineWithZeroBaseQty, PXErrorLevel.Error, item.BaseUnit)))
						{
							throw new PXRowPersistingException(fieldName, tran.Qty, Messages.CannotSaveLineWithZeroBaseQty, item.BaseUnit);
						}
					}
				}
			}
			
			basePersist();
		}
		#endregion
	}
}
