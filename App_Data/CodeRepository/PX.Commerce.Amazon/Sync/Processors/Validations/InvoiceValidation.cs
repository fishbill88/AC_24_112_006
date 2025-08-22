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

using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.SO;

namespace PX.Commerce.Amazon
{
	public class InvoiceValidation : BCBaseValidator, ILocalValidator
	{
        public int Priority { get { return 0; } }

		public void Validate(IProcessor iproc, ILocalEntity ilocal)
		{
			Validate<AmazonInvoiceProcessor, SalesInvoice>(iproc, ilocal, (processor, entity) =>
			{
				if (PXAccess.FeatureInstalled<FeaturesSet.advancedSOInvoices>() == false)
				{
					throw new PXException(AmazonMessages.CannotProcessBecauseofAdvancedSOInvoices, entity.ExternalRef.Value);
				}
			});
			
		}


	}
}
