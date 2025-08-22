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

using System.Collections.Generic;
using PX.Objects.Common;


namespace PX.Objects.GL
{
	public class GLTranType : ILabelProvider
	{
		protected static readonly IEnumerable<ValueLabelPair> ValueLabelList = new ValueLabelList
		{
			{ GLEntry, CA.Messages.GLEntry}
		};

		public IEnumerable<ValueLabelPair> ValueLabelPairs => ValueLabelList;

		public const string GLEntry = "GLE";

		public class gLEntry : PX.Data.BQL.BqlString.Constant<gLEntry>
		{
			public gLEntry() : base(GLEntry) { }
		}
	}
}