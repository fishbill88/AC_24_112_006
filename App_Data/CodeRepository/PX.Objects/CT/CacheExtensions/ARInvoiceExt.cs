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
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.PM;

namespace PX.Objects.CT.CacheExtensions
{
	/// <summary>
	/// Extension that activates Project-related functionality for the <see cref="ARInvoice"/> when Project accounting module is enabled.
	/// </summary>
	public sealed class ARInvoiceExt : PXCacheExtension<ARInvoice>
	{
		/// <summary>
		/// Specifies (if set to <see langword="true" />) that the extension is active.
		/// </summary>
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.contractManagement>() && !PXAccess.FeatureInstalled<FeaturesSet.projectModule>();

		#region ProjectID
		/// <inheritdoc cref="ARInvoice.ProjectID"/>
		public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
		/// <inheritdoc cref="ARInvoice.ProjectID"/>
		// Acuminator disable once PX1030 PXDefaultIncorrectUse as smart defaulting attribute is used.
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(NonProjectBaseAttribute))]
		[CT.ActiveContractBase]
		public Int32? ProjectID { get; set; }
		#endregion
	}
}
