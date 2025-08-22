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
using PX.Objects.GL;
using PX.Objects.CR;

namespace PX.Objects.PM.CacheExtensions
{
	/// <summary>
	/// Extension that activates Project-related functionality for the <see cref="ARInvoice"/> when Project accounting module is enabled.
	/// </summary>
	public sealed class ARInvoiceExtProject : PXCacheExtension<ARInvoice>
	{
		/// <summary>
		/// Specifies (if set to <see langword="true" />) that the extension is active.
		/// </summary>
		public static bool IsActive() => !PXAccess.FeatureInstalled<FeaturesSet.contractManagement>() && PXAccess.FeatureInstalled<FeaturesSet.projectModule>();

		#region ProjectID
		/// <inheritdoc cref="ARInvoice.ProjectID"/>
		public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
		/// <inheritdoc cref="ARInvoice.ProjectID"/>
		// Acuminator disable once PX1030 PXDefaultIncorrectUse as smart defaulting attribute is used.
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(NonProjectBaseAttribute))]
		[PM.ActiveProjectBaseAttribute(typeof(ARInvoice.customerID))]
		[PXRestrictor(typeof(Where<PMProject.visibleInAR, Equal<True>,
			Or<PMProject.nonProject, Equal<True>>>), PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
		[PXRemoveBaseAttribute(typeof(ProjectDefaultAttribute))]
		[ProjectDefault(BatchModule.AR, typeof(Search<Location.cDefProjectID,
			Where<Location.bAccountID, Equal<Current<ARInvoice.customerID>>,
				And<Location.locationID, Equal<Current<ARInvoice.customerLocationID>>>>>))]
		public Int32? ProjectID { get; set; }
		#endregion
	}

	/// <summary>
	/// Extension that activates Project-related functionality for the <see cref="ARInvoice"/> when Project accounting module is enabled.
	/// </summary>
	public sealed class ARInvoiceExtProjectContract : PXCacheExtension<ARInvoice>
	{
		/// <summary>
		/// Specifies (if set to <see langword="true" />) that the extension is active.
		/// </summary>
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.contractManagement>() && PXAccess.FeatureInstalled<FeaturesSet.projectModule>();

		#region ProjectID
		/// <inheritdoc cref="ARInvoice.ProjectID"/>
		public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
		/// <inheritdoc cref="ARInvoice.ProjectID"/>
		// Acuminator disable once PX1030 PXDefaultIncorrectUse as smart defaulting attribute is used.
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(NonProjectBaseAttribute))]
		[PM.ActiveProjectOrContractBaseAttribute(typeof(ARInvoice.customerID))]
		[PXRestrictor(typeof(Where<PMProject.visibleInAR, Equal<True>,
			Or<PMProject.nonProject, Equal<True>>>), PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
		[PXRemoveBaseAttribute(typeof(ProjectDefaultAttribute))]
		[ProjectDefault(BatchModule.AR, typeof(Search<Location.cDefProjectID,
			Where<Location.bAccountID, Equal<Current<ARInvoice.customerID>>,
				And<Location.locationID, Equal<Current<ARInvoice.customerLocationID>>>>>))]
		public Int32? ProjectID { get; set; }
		#endregion
	}
}
