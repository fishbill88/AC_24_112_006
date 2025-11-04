using CompiledVersion.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CN.Common.DAC;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using System;

namespace CompiledVersion.Graphs
{
    public class SOCreateShipment_Extension : PXGraphExtension<SOCreateShipment>
    {
        public static bool IsActive() => true;

        public PXSetup<SOSetup> sosetup;

        protected virtual void _(Events.FieldSelecting<SOOrder, SOOrderExt.usrBillCompleteDisplay> e)
        {
            if (e.Row == null)
                return;
            var setupExt = sosetup.Current?.GetExtension<SOSetupExt>();

            var attributeID = setupExt?.UsrBillComplete;
            if (string.IsNullOrEmpty(attributeID))
                return;

            var billcomple = Base.Caches[typeof(SOOrder)].GetValueExt(e.Row,
                    string.Format("Attribute{0}", attributeID));

            e.ReturnValue = billcomple;
        }

        protected virtual void _(Events.FieldSelecting<SOOrder, SOOrderExt.usrFormTypeDisplay> e)
        {
            if (e.Row == null)
                return;

            var setupExt = sosetup.Current?.GetExtension<SOSetupExt>();
            var attributeID = setupExt?.UsrFormType;
            if (string.IsNullOrEmpty(attributeID))
                return;
            var formType = Base.Caches[typeof(SOOrder)].GetValueExt(e.Row,
                    string.Format("Attribute{0}", attributeID));

            e.ReturnValue = formType;
        }

        // Provide Bill-To Email in processing grid
        protected virtual void _(Events.FieldSelecting<SOOrder, SOOrderExt.usrBillToEmail> e)
        {
            if (e.Row == null) return;
            var row = e.Row;
            var ext = row.GetExtension<SOOrderExt>();
            if (!string.IsNullOrEmpty(ext?.UsrBillToEmail)) { e.ReturnValue = ext.UsrBillToEmail; return; }
            if (row.BillContactID == null) { e.ReturnValue = null; return; }
            SOBillingContact bill = SOBillingContact.PK.Find(Base, row.BillContactID);
            e.ReturnValue = bill?.Email;
         }
    }
}
