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
    /// Manufacturing Overhead Types
    /// </summary>
    public class OverheadType
    {
        /// <summary>
        /// Fixed overhead
        /// </summary>
        public const string FixedType = "F";
        /// <summary>
        /// Variable overhead by labor hours
        /// </summary>
        public const string VarLaborHrs = "H";
        /// <summary>
        /// Variable overhead by labor cost
        /// </summary>
        public const string VarLaborCost = "C";
        /// <summary>
        /// Variable overhead by material cost
        /// </summary>
        public const string VarMatlCost = "A";
        /// <summary>
        /// Variable overhead by machine hours
        /// </summary>
        public const string VarMachHrs = "M";
        /// <summary>
        /// Variable overhead by quantity completed
        /// </summary>
        public const string VarQtyComp = "Q";
        /// <summary>
        /// Variable overhead by total completed
        /// </summary>
        public const string VarQtyTot = "T";

        /// <summary>
        /// Description/labels for identifiers
        /// </summary>
        public static class Desc
        {
            public static string FixedType => Messages.GetLocal(Messages.FixedType);
            public static string VarLaborHrs => Messages.GetLocal(Messages.VarLaborHrs);
            public static string VarLaborCost => Messages.GetLocal(Messages.VarLaborCost);
            public static string VarMatlCost => Messages.GetLocal(Messages.VarMatlCost);
            public static string VarMachHrs => Messages.GetLocal(Messages.VarMachHrs);
            public static string VarQtyComp => Messages.GetLocal(Messages.VarQtyComp);
            public static string VarQtyTot => Messages.GetLocal(Messages.VarQtyTot);
        }

        /// <summary>
        /// Translate the overhead type identifier to the description
        /// </summary>
        /// <param name="overheadType">Overhead type id</param>
        /// <returns>Description of the overhead</returns>
        public static string GetTypeDesc(string overheadType)
        {
            switch (overheadType.Trim())
            {
                case FixedType:
                    return Desc.FixedType;

                case VarLaborHrs:
                    return Desc.VarLaborHrs;

                case VarMachHrs:
                    return Desc.VarMachHrs;

                case VarLaborCost:
                    return Desc.VarLaborCost;

                case VarMatlCost:
                    return Desc.VarMatlCost;

                case VarQtyComp:
                    return Desc.VarQtyComp;

                case VarQtyTot:
                    return Desc.VarQtyTot;
            }
            return string.Empty;
        }


        public class fixedType : PX.Data.BQL.BqlString.Constant<fixedType>
        {
            public fixedType() : base(FixedType) { }
        }
        public class varLaborHrs : PX.Data.BQL.BqlString.Constant<varLaborHrs>
        {
            public varLaborHrs() : base(VarLaborHrs) { }
        }
        public class varLaborCost : PX.Data.BQL.BqlString.Constant<varLaborCost>
        {
            public varLaborCost() : base(VarLaborCost) { }
        }
        public class varMatlCost : PX.Data.BQL.BqlString.Constant<varMatlCost>
        {
            public varMatlCost() : base(VarMatlCost) { }
        }
        public class varMachHrs : PX.Data.BQL.BqlString.Constant<varMachHrs>
        {
            public varMachHrs() : base(VarMachHrs) { }
        }
        public class varQtyComp : PX.Data.BQL.BqlString.Constant<varQtyComp>
        {
            public varQtyComp() : base(VarQtyComp) { }
        }
        public class varQtyTot : PX.Data.BQL.BqlString.Constant<varQtyTot>
        {
            public varQtyTot() : base(VarQtyTot) { }
        }

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                    new string[] { 
                        FixedType, 
                        VarLaborHrs, 
                        VarLaborCost, 
                        VarMatlCost, 
                        VarMachHrs, 
                        VarQtyComp, 
                        VarQtyTot },
                    new string[] { 
                        Messages.FixedType, 
                        Messages.VarLaborHrs, 
                        Messages.VarLaborCost, 
                        Messages.VarMatlCost, 
                        Messages.VarMachHrs, 
                        Messages.VarQtyComp,
                        Messages.VarQtyTot }) { }
        }
    }
}