using PX.CS;
using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubspotCustomization
{
    public class SOSetupMaint_Extension : PXGraphExtension<PX.Objects.SO.SOSetupMaint>
    {
        public static bool IsActive() => true;

        public PXSelect<CSScreenAttribute> screenattributes; 
        public delegate Boolean PrePersistDelegate();
        [PXOverride]
        public Boolean PrePersist(PrePersistDelegate baseMethod)
        {
            var formtype = PXSelect<CSScreenAttribute,
                Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
                And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>>>>.Select(Base, "SO301000", "FORMTYPE");
            foreach (CSScreenAttribute item in formtype)
            {
                SOSetupExt ext = Base.sosetup.Current.GetExtension<SOSetupExt>();
                item.Hidden = (ext?.UsrHidePrintingMethod ?? false);
                screenattributes.Update(item);
                //Base.Caches[typeof(CSScreenAttribute)].Update(item);
            }
            var formtype2 = PXSelect<CSScreenAttribute,
                Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
                And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>>>>.Select(Base, "SO301000", "FORMTYPE2");
            foreach (CSScreenAttribute item in formtype2)
            {
                SOSetupExt ext = Base.sosetup.Current.GetExtension<SOSetupExt>();
                item.Hidden = (ext?.UsrHidePrintingMethod2 ?? false);
                screenattributes.Update(item);
                //Base.Caches[typeof(CSScreenAttribute)].Update(item);
            }

            return baseMethod();
        }
    }
}
