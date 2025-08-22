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

namespace PX.Objects.AM.Attributes
{
    /// <summary>
    /// Production costing methods. Indicates the different costing methods available for production completion unit cost calculations.
    /// </summary>
    public class CostMethod
    {
        /// <summary>
        /// Estimated costing method.
        /// Only/Default option until "CostMethod" was implemented
        /// </summary>
        public const int Estimated = 1;
        /// <summary>
        /// Actual costing
        /// </summary>
        public const int Actual = 2;
        /// <summary>
        /// Standard costing (for standard cost items only)
        /// </summary>
        public const int Standard = 3;

        /// <summary>
        /// Description/labels for identifiers
        /// </summary>
        public class Desc
        {
            /// <summary>
            /// Standard costing description (for standard cost items only)
            /// </summary>
            public static string Standard => Messages.GetLocal(Messages.Standard);

            /// <summary>
            /// Actual costing description
            /// </summary>
            public static string Actual => Messages.GetLocal(PX.Objects.GL.Messages.Actual);

            /// <summary>
            /// Estimated costing method description
            /// </summary>
            public static string Estimated => Messages.GetLocal(Messages.Estimated);
        }

        /// <summary>
        /// Standard costing (for standard cost items only)
        /// </summary>
        public class standard : PX.Data.BQL.BqlInt.Constant<standard>
        {
            public standard() : base(Standard) {; }
        }
        /// <summary>
        /// Estimated costing method.
        /// Only/Default option until "CostMethod" was implemented
        /// </summary>
        public class estimated : PX.Data.BQL.BqlInt.Constant<estimated>
        {
            public estimated() : base(Estimated) {; }
        }
        /// <summary>
        /// Actual costing
        /// </summary>
        public class actual : PX.Data.BQL.BqlInt.Constant<actual>
        {
            public actual() : base(Actual) {; }
        }

        /// <summary>
        /// List attribute used for DAC fields
        /// </summary>
        public class ListAllAttribute : PXIntListAttribute
        {
            public ListAllAttribute()
                : base(
                new int[] { Estimated, Standard, Actual },
                new string[] { Messages.Estimated, Messages.Standard, GL.Messages.Actual })
            { }
        }

        public class ListDefaultsAttribute : PXIntListAttribute
        {
            public ListDefaultsAttribute()
                : base(
                new int[] { Estimated, Actual },
                new string[] { Messages.Estimated, GL.Messages.Actual })
            { }
        }

        public static string GetDescription(int? costMethod)
        {
            if (costMethod == null)
            {
                return string.Empty;
            }

            switch (costMethod)
            {
                case Actual:
                    return Desc.Actual;
                case Estimated:
                    return Desc.Estimated;
                case Standard:
                    return Desc.Standard;
            }
            return string.Empty;
        }
    }
}