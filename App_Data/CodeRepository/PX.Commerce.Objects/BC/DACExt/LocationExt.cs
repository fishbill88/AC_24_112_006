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
using System;
using PX.Objects.CR;
using PX.Data.EP;
using PX.Commerce.Core;

namespace PX.Commerce.Objects
{
	#region LocationExt
	/// <summary>
	/// DAC extension of Location to add additional attributes.
	/// </summary>
	public sealed class BCLocationExt : PXCacheExtension<Location>
	{
		public static bool IsActive() { return CommerceFeaturesHelper.CommerceEdition; }

		#region LocationCD
		/// <summary>
		/// <inheritdoc cref="Location.LocationCD"/>
		/// </summary>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFieldDescription]
		public String LocationCD { get; set; }
		/// <inheritdoc cref="LocationCD"/>
		public abstract class locationCD : PX.Data.BQL.BqlString.Field<locationCD> { }
		#endregion
		#region Descr
		/// <summary>
		/// <inheritdoc cref="Location.Descr"/>
		/// </summary>
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFieldDescription]
		public String Descr { get; set; }
		/// <inheritdoc cref="Descr"/>
		public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
		#endregion
	}
	#endregion
}
