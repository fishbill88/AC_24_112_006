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
    /// Subcontract Source Types
    /// </summary>
    public class AMSubcontractSource
    {
        /// <summary>
        /// None
        /// </summary>
        public const int None = 0;
        /// <summary>
        /// Purchase Standard Purchase process
        /// </summary>
        public const int Purchase = 1;
        /// <summary>
        /// DropShip Material goes straight to Vendor
        /// </summary>
        public const int DropShip = 2;
        /// <summary>
        /// VendorSupplied Vendor supplies the material
        /// </summary>
        public const int VendorSupplied = 3;
        /// <summary>
        /// ShipToVendor Material issued to Production Order to be shipped to Vendor
        /// </summary>
        public const int ShipToVendor = 4;

        /// <summary>
        /// Description/labels for identifiers
        /// </summary>
        public class Desc
        {
            /// <summary>
            /// None
            /// </summary>
            public static string None => Messages.GetLocal(Messages.None);

            /// <summary>
            /// Purchase Standard Purchase process
            /// </summary>
            public static string Purchase => Messages.GetLocal(Messages.Purchase);

            /// <summary>
            /// DropShip Material goes straight to Vendor
            /// </summary>
            public static string DropShip => Messages.GetLocal(Messages.DropShip);

            /// <summary>
            /// VendorSupplied Vendor supplies the material
            /// </summary>
            public static string VendorSupplied => Messages.GetLocal(Messages.VendorSupplied);

            /// <summary>
            /// ShipToVendor Material issued to Production Order to be shipped to Vendor
            /// </summary>
            public static string ShipToVendor => Messages.GetLocal(Messages.ShipToVendor);
        }


        /// <summary>
        /// None
        /// </summary>
        public class none : PX.Data.BQL.BqlInt.Constant<purchase>
        {
            public none() : base(None) { }
        }
        /// <summary>
        /// Purchase Standard Purchase process
        /// </summary>
        public class purchase : PX.Data.BQL.BqlInt.Constant<purchase>
        {
            public purchase() : base(Purchase) { }
        }
        /// <summary>
        /// DropShip Material goes straight to Vendor
        /// </summary>
        public class dropShip : PX.Data.BQL.BqlInt.Constant<dropShip>
        {
            public dropShip() : base(DropShip) { }
        }

        /// <summary>
        /// VendorSupplied Vendor supplies the material
        /// </summary>
        public class vendorSupplied : PX.Data.BQL.BqlInt.Constant<vendorSupplied>
        {
            public vendorSupplied() : base(VendorSupplied) { }
        }

        /// <summary>
        /// ShipToVendor Material issued to Production Order to be shipped to Vendor
        /// </summary>
        public class shipToVendor : PX.Data.BQL.BqlInt.Constant<shipToVendor>
        {
            public shipToVendor() : base(ShipToVendor) { }
        }

        /// <summary>
        /// List for Production Material Subcontract Source
        /// </summary>
        public class ListAttribute : PXIntListAttribute
        {
            public ListAttribute()
                : base(
                new int[] { None, Purchase, DropShip, VendorSupplied, ShipToVendor },
                new string[] { Messages.None, Messages.Purchase, Messages.DropShip, Messages.VendorSupplied, Messages.ShipToVendor })
            { }
        }
    }
}