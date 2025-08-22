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

namespace PX.Objects.GL
{
    public class ReclassType
    {
        public class List : PXStringListAttribute
        {
            public List()
                : base(
                new string[] { Common, Split },
                new string[] { Messages.CommonReclassType, Messages.Split })
            { }
        }

        public const string Common = "C";
        public const string Split = "S";
        
        
        public class common : PX.Data.BQL.BqlString.Constant<common>
		{
            public common() : base(Common) { }
        }

        public class split : PX.Data.BQL.BqlString.Constant<split>
		{
            public split() : base(Split) { }
        }
    }
}
