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
    /// Manufacturing material types
    /// </summary>
    public class AMMaterialType
    {
        /// <summary>
        /// Standard/regular material
        /// </summary>
        public const int Regular = 1;
        /// <summary>
        /// Phantom material item
        /// </summary>
        public const int Phantom = 2;
        /// <summary>
        /// Supplemental items are supporting items included with a manufactured/configured item
        /// </summary>
        public const int Supplemental = 3;
        /// <summary>
        /// Subcontract for Outside Processing
        /// </summary>
        public const int Subcontract = 4;

        /// <summary>
        /// Description/labels for identifiers
        /// </summary>
        public class Desc
        {
            /// <summary>
            /// Standard/regular material
            /// </summary>
            public static string Regular => Messages.GetLocal(Messages.Regular);

            /// <summary>
            /// Phantom material item
            /// </summary>
            public static string Phantom => Messages.GetLocal(Messages.Phantom);

            /// <summary>
            /// Supplemental items are supporting items included with a manufactured/configured item
            /// </summary>
            public static string Supplemental => Messages.GetLocal(Messages.Supplemental);

            /// <summary>
            /// Subcontract for Outside Processing
            /// </summary>
            public static string Subcontract => Messages.GetLocal(Messages.Subcontract);
        }


        /// <summary>
        /// Standard/regular material
        /// </summary>
        public class regular : PX.Data.BQL.BqlInt.Constant<regular>
        {
            public regular() : base(Regular) { }
        }
        /// <summary>
        /// Phantom material item
        /// </summary>
        public class phantom : PX.Data.BQL.BqlInt.Constant<phantom>
        {
            public phantom() : base(Phantom) { }
        }

        /// <summary>
        /// Supplemental items are supporting items included with a manufactured/configured item
        /// </summary>
        public class supplemental : PX.Data.BQL.BqlInt.Constant<supplemental>
        {
            public supplemental() : base(Supplemental) { }
        }

        /// <summary>
        /// Supplemental items are supporting items included with a manufactured/configured item
        /// </summary>
        public class subcontract : PX.Data.BQL.BqlInt.Constant<subcontract>
        {
            public subcontract() : base(Subcontract) { }
        }

        /// <summary>
        /// List for BOM 
        /// </summary>
        public class ListAttribute : PXIntListAttribute
        {
            public ListAttribute()
                : base(
                new int[] { Regular, Phantom, Subcontract },
                new string[] { Messages.Regular, Messages.Phantom, Messages.Subcontract}) { }
        }

        /// <summary>
        /// List for configurations
        /// </summary>
        public class ConfigListAttribute : PXIntListAttribute
        {
            public ConfigListAttribute()
                : base(
                new int[] { Regular, Phantom, Supplemental, Subcontract },
                new string[] { Messages.Regular, Messages.Phantom, Messages.Supplemental, Messages.Subcontract })
            { }
        }

        /// <summary>
        /// List for Production Material
        /// </summary>
        public class ProductionListAttribute : PXIntListAttribute
        {
            public ProductionListAttribute()
                : base(
                new int[] { Regular, Subcontract },
                new string[] { Messages.Regular, Messages.Subcontract })
            { }
        }
    }
}