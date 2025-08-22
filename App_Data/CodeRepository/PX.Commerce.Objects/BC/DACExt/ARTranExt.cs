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

using PX.Commerce.Core;
using PX.Data;
using PX.Objects.AR;
using System;
using PX.Data.EP;

namespace PX.Commerce.Objects
{
	/// <summary>
	/// DAC Extension of ARSalesPrice to add additional properties.
	/// </summary>
	[Serializable]
	public sealed class BCARTranExt : PXCacheExtension<ARTran>
	{
		public static bool IsActive() { return CommerceFeaturesHelper.CommerceEdition; }

		#region TranType
		public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
		/// <inheritdoc cref="ARTran.TranType"/>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFieldDescription]
		public string TranType { get; set; }

		#endregion
		#region RefNbr

		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		/// <inheritdoc cref="ARTran.RefNbr"/>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFieldDescription]
		public string RefNbr { get; set; }
		#endregion

		#region AssociatedOrderLineNbr
		/// <summary>
		/// The order line associated with the transaction.
		/// </summary>
		[PXDBInt()]
		[PXUIField(DisplayName = "Associated Order Line Nbr.", Visible = false,  Enabled = false)]
		public int? AssociatedOrderLineNbr { get; set; }
		/// <inheritdoc cref="AssociatedOrderLineNbr"/>
		public abstract class associatedOrderLineNbr : PX.Data.BQL.BqlInt.Field<associatedOrderLineNbr> { }
		#endregion

		#region GiftMessage
		/// <summary>
		/// The gift message for the order.
		/// </summary>
		[PXDBString(200, IsUnicode = true)]
		[PXUIField(DisplayName = "Gift Message", Visible = false,  Enabled = false)]
		/// <inheritdoc cref="PriceType"/>
		public string GiftMessage { get; set; }
		/// <inheritdoc cref="GiftMessage"/>
		public abstract class giftMessage : PX.Data.BQL.BqlString.Field<giftMessage> { }
		#endregion

		/// <summary>
		/// External document ID that is needed to identify document for the integrations with 
		/// external systems such as Commerce Edition
		/// </summary>
		[PXDBString]
		public string ExternalRef { get; set; }
		public abstract class externalRef : PX.Data.BQL.BqlString.Field<externalRef> { }
	}
}
