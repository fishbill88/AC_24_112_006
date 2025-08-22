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
    /// Manufacturing Attribute Source
    /// </summary>
    public class AMAttributeSource
    {
        /// <summary>
        /// BOM Sourced Attribute
        /// </summary>
        public const int BOM = 1;
        /// <summary>
        /// Production Sourced Attribute
        /// </summary>
        public const int Production = 2;
        /// <summary>
        /// Configuration Sourced Attribute
        /// </summary>
        public const int Configuration = 3;
        /// <summary>
        /// Order Type Sourced Attribute
        /// </summary>
        public const int OrderType = 4;
        
        /// <summary>
        /// Description/labels for identifiers
        /// </summary>
        public class Desc
        {
            /// <summary>
            /// BOM Sourced Attribute
            /// </summary>
            public static string BOM => Messages.GetLocal(Messages.BOM);

            /// <summary>
            /// Production Sourced Attribute
            /// </summary>
            public static string Production => Messages.GetLocal(Messages.Production);

            /// <summary>
            /// Configuration Sourced Attribute
            /// </summary>
            public static string Configuration => Messages.GetLocal(Messages.Configuration);

            /// <summary>
            /// Configuration Sourced Attribute
            /// </summary>
            public static string OrderType => Messages.GetLocal(Messages.OrderType);
        }

        /// <summary>
        /// Get the list description of the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetDescription(int? source)
        {
            if (source.GetValueOrDefault() == 0)
            {
                return string.Empty;
            }

            try
            {
                var x = new ProductionListAttribute();
                return x.ValueLabelDic[source.GetValueOrDefault()];
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Is the given attribute source an order level attribute
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsOrderLevelAttributes(int? source)
        {
            return source != null && (source == Production || source == OrderType);
        }

        /// <summary>
        /// BOM Sourced Attribute
        /// </summary>
        public class bOM : PX.Data.BQL.BqlInt.Constant<bOM>
        {
            public bOM() : base(BOM) {; }
        }
        /// <summary>
        /// Production Sourced Attribute
        /// </summary>
        public class production : PX.Data.BQL.BqlInt.Constant<production>
        {
            public production() : base(Production) {; }
        }

        /// <summary>
        /// Configuration Sourced Attribute
        /// </summary>
        public class configuration : PX.Data.BQL.BqlInt.Constant<configuration>
        {
            public configuration() : base(Configuration) {; }
        }

        /// <summary>
        /// Order Type Sourced Attribute
        /// </summary>
        public class orderType : PX.Data.BQL.BqlInt.Constant<orderType>
        {
            public orderType() : base(OrderType) {; }
        }

        /// <summary>
        /// List for Manufacturing Attribute Source
        /// </summary>
        public class ProductionListAttribute : PXIntListAttribute
        {
            public ProductionListAttribute()
                : base(
                new int[] { BOM, Production, Configuration, OrderType },
                new string[] { Messages.BOM, Messages.Production, Messages.Configuration, Messages.OrderType })
            { }
        }
    }
}