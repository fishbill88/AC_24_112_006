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
    /// Indicates the source of production detail
    /// </summary>
    public class ProductionDetailSource
    {
        /// <summary>
        /// Detail was not sourced from any specific area. No Source.
        /// Users have the ability to build the details from scratch.
        /// </summary>
        public const int NoSource = 0;
        /// <summary>
        /// Source of Bill of Material details
        /// </summary>
        public const int BOM = 1;
        /// <summary>
        /// Source of Estimate order details
        /// </summary>
        public const int Estimate = 2;
        /// <summary>
        /// Source of configuration result details (BOM + Config results)
        /// </summary>
        public const int Configuration = 3;
        /// <summary>
        /// Source of Production Ref Details
        /// </summary>
        public const int ProductionRef = 4;

        /// <summary>
        /// Description/labels for identifiers
        /// </summary>
        public class Desc
        {
            public static string NoSource => Messages.GetLocal(Messages.NoSource);
            public static string BOM => Messages.GetLocal(Messages.BOM);
            public static string Estimate => Messages.GetLocal(Messages.Estimate);
            public static string Configuration => Messages.GetLocal(Messages.Configuration);
            public static string ProductionRef => Messages.GetLocal(Messages.ProductionRef);
        }

        /// <summary>
        /// Get the list description of the given source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetDescription(int? source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            try
            {
                var x = new List();
                return x.ValueLabelDic[source.GetValueOrDefault()];
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// UI List attribute
        /// </summary>
        public class List : PXIntListAttribute
        {
            public List() : base(
                new int[] { NoSource, BOM, Estimate, Configuration, ProductionRef },
                new string[] { Messages.NoSource, Messages.BOM, Messages.Estimate, Messages.Configuration, Messages.ProductionRef })
            { }
        }

        /// <summary>
        /// UI List for Quick Disassemble Source
        /// </summary>
        public class DisassembleList : PXIntListAttribute
        {
            public DisassembleList() : base(
                new int[] {BOM, ProductionRef },
                new string[] { Messages.BOM, Messages.ProductionRef })
            { }
        }

        /// <summary>
        /// Detail was not sourced from any specific area. No Source.
        /// Users have the ability to build the details from scratch.
        /// </summary>
        public class noSource : PX.Data.BQL.BqlInt.Constant<noSource>
        {
            public noSource() : base(NoSource) { }
        }

        /// <summary>
        /// Source of Bill of Material details
        /// </summary>
        public class bom : PX.Data.BQL.BqlInt.Constant<bom>
        {
            public bom() : base(BOM) {}
        }

        /// <summary>
        /// Source of Estimate order details
        /// </summary>
        public class estimate : PX.Data.BQL.BqlInt.Constant<estimate>
        {
            public estimate() : base(Estimate) {}
        }

        /// <summary>
        /// Source of configuration result details (BOM + Config results)
        /// </summary>
        public class configuration : PX.Data.BQL.BqlInt.Constant<configuration>
        {
            public configuration() : base(Configuration) {}
        }

        /// <summary>
        /// Source of Production Order details
        /// </summary>
        public class productionRef : PX.Data.BQL.BqlInt.Constant<productionRef>
        {
            public productionRef() : base(ProductionRef) {; }
        }
    }
}