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
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using System.Linq;

namespace PX.Objects.FS
{
    public class FSAcctSubDefault
    {
        public class CustomListAttribute : PXStringListAttribute
        {
            public string[] AllowedValues
            {
                get
                {
                    return _AllowedValues;
                }
            }

            public string[] AllowedLabels
            {
                get
                {
                    return _AllowedLabels;
                }
            }

            public CustomListAttribute(string[] allowedValues, string[] allowedLabels)
                : base(allowedValues, allowedLabels)
            {
            }
        }

        /// <summary>
        /// Defines a list of the possible sources for the FS Documents sub-account defaulting: <br/>
        /// Namely: MaskCustomerLocation, MaskItem, MaskServiceOrderType, MaskCompany, MaskBranchLocation <br/>
        /// Mostly, this attribute serves as a container <br/>
        /// </summary>
        public class ClassListAttribute : CustomListAttribute
        {
            public ClassListAttribute()
                : base(new string[] { MaskBranchLocation, MaskCompany, MaskItem, MaskCustomerLocation, MaskPostingClass, MaskSalesPerson, MaskServiceOrderType, MaskWarehouse },
                       new string[] { TX.Messages.MASKBRANCHLOCATION, TX.Messages.MASKCOMPANY, TX.Messages.MASKITEM, TX.Messages.MASKCUSTOMERLOCATION, TX.Messages.MASKPOSTINGCLASS, TX.Messages.MASKSALESPERSON, TX.Messages.MASKSERVICEORDERTYPE, TX.Messages.MASKWAREHOUSE })
            {
            }

            public ClassListAttribute(bool contract)
                : base(new string[] { MaskCustomerLocation, MaskItem, MaskCompany, MaskBranchLocation},
                       new string[] { TX.Messages.MASKCUSTOMERLOCATION, TX.Messages.MASKITEM, TX.Messages.MASKCOMPANY, TX.Messages.MASKBRANCHLOCATION })
            {
            }
        }

        public const string MaskBranchLocation = "A";
        public const string MaskCompany = "C";
        public const string MaskItem = "I";
        public const string MaskCustomerLocation = "L";
        public const string MaskPostingClass = "P";
        public const string MaskSalesPerson = "S";
        public const string MaskServiceOrderType = "T";
        public const string MaskWarehouse = "W";
    }

    public class FSDimensionMaskAttribute : PXDimensionMaskAttribute
    {
        public FSDimensionMaskAttribute(string dimension, string mask, string defaultValue, string[] allowedValues, string[] allowedLabels)
            : base(dimension, mask, defaultValue, allowedValues, allowedLabels)
        {
        }

        public virtual bool IsStringListValueDisabled(string item)
        {
            if (!PXAccess.FeatureInstalled<FeaturesSet.warehouse>()
                    && !PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>()
                        && item == FSAcctSubDefault.MaskWarehouse)
            {
                return false;
            }

            return true;
        }

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);

            var pairs = _allowedValues.Zip(_allowedLabels, (v, l) => new Tuple<string, string>(v, l))
                                      .Where(t => IsStringListValueDisabled(t.Item1));

            _allowedValues = pairs.Select(p => p.Item1).ToArray();
            _allowedLabels = pairs.Select(p => p.Item2).ToArray();
        }
    }

    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Subaccount Mask", Visibility = PXUIVisibility.Visible, FieldClass = SubAccountAttribute.DimensionName)]
    public sealed class SubAccountMaskAttribute : PXEntityAttribute
    {
        private const string DIMENSION_NAME = "SUBACCOUNT";
        private const string MASK_NAME = "FSSrvOrdType";
        private const string CONTRACT_MASK_NAME = "ServiceContract";

        public SubAccountMaskAttribute()
            : base()
        {
            FSDimensionMaskAttribute attr = new FSDimensionMaskAttribute(DIMENSION_NAME, MASK_NAME, FSAcctSubDefault.MaskCustomerLocation, new FSAcctSubDefault.ClassListAttribute().AllowedValues, new FSAcctSubDefault.ClassListAttribute().AllowedLabels);
            attr.ValidComboRequired = false;
            _Attributes.Add(attr);
            _SelAttrIndex = _Attributes.Count - 1;
        }

        public SubAccountMaskAttribute(bool ignoreSrvOrdType)
            : base()
        {
            FSDimensionMaskAttribute attr = new FSDimensionMaskAttribute(DIMENSION_NAME, CONTRACT_MASK_NAME, FSAcctSubDefault.MaskCustomerLocation, new FSAcctSubDefault.ClassListAttribute(true).AllowedValues, new FSAcctSubDefault.ClassListAttribute(true).AllowedLabels);
            attr.ValidComboRequired = false;
            _Attributes.Add(attr);
            _SelAttrIndex = _Attributes.Count - 1;
        }

        public static string MakeSub<Field>(PXGraph graph, string mask, object[] sources, Type[] fields, bool contract = false)
            where Field : IBqlField
        {
            try
            {
                string[] allowedValues = contract == false ? new FSAcctSubDefault.ClassListAttribute().AllowedValues : new FSAcctSubDefault.ClassListAttribute(true).AllowedValues;
                return FSDimensionMaskAttribute.MakeSub<Field>(graph, mask, allowedValues, 0, sources);
            }
            catch (PXMaskArgumentException ex)
            {
                PXCache cache = graph.Caches[BqlCommand.GetItemType(fields[ex.SourceIdx])];
                string fieldName = fields[ex.SourceIdx].Name;
                throw new PXMaskArgumentException(new FSAcctSubDefault.ClassListAttribute().AllowedLabels[ex.SourceIdx], PXUIFieldAttribute.GetDisplayName(cache, fieldName));
            }
        }
    }
}
