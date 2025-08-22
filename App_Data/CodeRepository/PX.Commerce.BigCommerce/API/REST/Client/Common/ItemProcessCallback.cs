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

using System;
using System.Collections.Generic;

namespace PX.Commerce.BigCommerce.API.REST
{
	public class ItemProcessCallback<T>
	{
		public readonly Int32 Index;
		public readonly Boolean IsSuccess;
		public readonly T Result;
		public readonly List<T> OriginalBatch;
		public readonly RestException Error;

		public ItemProcessCallback(Int32 index, T result)
		{
			Index = index;
			Result = result;
			IsSuccess = true;
		}
		public ItemProcessCallback(Int32 index, RestException error, List<T> originalBatch = null)
		{
			Index = index;
			Error = error;
			IsSuccess = false;
			OriginalBatch = originalBatch;
		}
	}
}
