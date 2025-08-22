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
using PX.Data.EP;
using PX.Objects.CA;
using PX.Objects.CS;
using System;

namespace PX.Commerce.Amazon.Amazon.DACExt
{
	/// <summary>
	/// DAC extension of CAAdj to add additional attributes and properties.
	/// It is used within the NonOrderFee import to display human-readable form of the <see cref="BCSyncDetail.LocalID"/> property for Cash transactions on View Details.
	/// </summary>
	[Serializable]
	public sealed class CAAdjExt : PXCacheExtension<CAAdj>
	{
		/// <summary>
		/// Defines if the extension is active.
		/// </summary>
		/// <returns>True if the extension is active; otherwise - false.</returns>
		public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.amazonIntegration>(); }

		#region AdjRefNbr
		/// <summary>
		/// <inheritdoc cref="CAAdj.AdjRefNbr"/>
		/// </summary>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFieldDescription]
		public string AdjRefNbr { get; set; }
		public abstract class adjRefNbr : BqlString.Field<adjRefNbr> { }
		#endregion

		#region EntryTypeID
		/// <summary>
		/// <inheritdoc cref="CAAdj.EntryTypeID"/>
		/// </summary>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFieldDescription]
		public string EntryTypeID { get; set; }
		public abstract class entryTypeID : BqlString.Field<entryTypeID> { }
		#endregion

		#region TranDesc
		/// <summary>
		/// <inheritdoc cref="CAAdj.TranDesc"/>
		/// </summary>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(PXFieldDescriptionAttribute))]
		public string TranDesc { get; set; }
		public abstract class tranDesc : BqlString.Field<tranDesc> { }
		#endregion
	}
}
