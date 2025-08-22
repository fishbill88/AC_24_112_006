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

namespace PX.Commerce.Amazon.API.Rest.Constants
{
	public static class FeedErrorMessageCode
	{
		// List of common error codes returned by Amazon can be found in this link
		// https://sellercentral.amazon.ca/gp/help/external/17781?language=en_CA&ref=efph_17781_cont_G17811
		public const string XmlDocumentIllformed = "50001";
		public const string XmlDocumentLevelError = "E19999"; // in-house random error code that should not exist in Amazon
	}
}
