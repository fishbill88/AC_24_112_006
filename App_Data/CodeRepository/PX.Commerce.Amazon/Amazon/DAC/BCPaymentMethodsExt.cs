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
using PX.Commerce.Objects;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CA;
using PX.Objects.CS;
using System;

namespace PX.Commerce.Amazon
{
	/// <inheritdoc cref="BCPaymentMethodsExt"/>
	[Obsolete(PX.Objects.Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
	public sealed class BCPaymentMethodsExt : PXCacheExtension<BCPaymentMethods>
	{
		public static bool IsActive() { return PXAccess.FeatureInstalled<FeaturesSet.amazonIntegration>(); }

		public class PK : PrimaryKeyOf<BCPaymentMethods>.By<BCPaymentMethods.paymentMappingID>
		{
			public static BCPaymentMethods Find(PXGraph graph, int? paymentMappingID) => FindBy(graph, paymentMappingID);
		}
	}
}
