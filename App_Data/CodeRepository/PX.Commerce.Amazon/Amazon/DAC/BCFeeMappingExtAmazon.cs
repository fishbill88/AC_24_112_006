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

using PX.Commerce.Amazon.Constants;
using PX.Commerce.Core;
using PX.Commerce.Objects;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CA;
using PX.Objects.CS;

namespace PX.Commerce.Amazon.Amazon.DAC
{
	/// <summary>
	/// The Amazon extension for the <seealso cref="BCFeeMapping"/> DAC.
	/// </summary>
	public sealed class BCFeeMappingExtAmazon : PXCacheExtension<BCFeeMapping>
	{
		public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.amazonIntegration>(); }

		#region IsActive
		/// <summary>
		/// Defines if the Fee type mapping is active or not.
		/// </summary>
		// Acuminator disable once PX1030 PXDefaultIncorrectUse [The field have PXDBoolAttribute]
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public bool? Active { get; set; }
		public abstract class active : BqlString.Field<active> { }
		#endregion

		#region FeeDescription
		/// <summary>
		/// Gets or sets the Fee type description.
		/// </summary>
		// Acuminator disable once PX1030 PXDefaultIncorrectUse [The field have PXDBStringAttribute]
		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Fee Description", Enabled = false)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public string FeeDescription { get; set; }
		public abstract class feeDescription : BqlString.Field<feeDescription> { }
		#endregion
	}
}
