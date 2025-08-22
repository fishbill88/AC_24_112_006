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
using PX.Data;

namespace PX.Objects.AM.CacheExtensions
{
	/// <summary>
	/// The default mapping of the <see cref="OpportunityDocument" /> mapped cache extension to the specified table.
	/// </summary>
	public class OpportunityDocument : PXMappedCacheExtension
    {
        public abstract class opportunityID : PX.Data.BQL.BqlString.Field<opportunityID> { }
        public virtual string OpportunityID { get; set; }

		/// <inheritdoc cref="QuoteID"/>
		public abstract class quoteID : PX.Data.BQL.BqlString.Field<quoteID> { }
		/// <summary>
		/// The opportunity quote ID, which is a GUID.
		/// </summary>
		public virtual Guid? QuoteID { get; set; }

        public abstract class quoteNbr : PX.Data.BQL.BqlString.Field<quoteNbr> { }
        public virtual string QuoteNbr { get; set; }

        public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
        public virtual int? BAccountID { get; set; }

        public abstract class approved : PX.Data.BQL.BqlBool.Field<approved> { }
        public virtual bool? Approved { get; set; }

        public abstract class isDisabled : PX.Data.BQL.BqlBool.Field<isDisabled> { }
        public virtual bool? IsDisabled { get; set; }
    }
}
