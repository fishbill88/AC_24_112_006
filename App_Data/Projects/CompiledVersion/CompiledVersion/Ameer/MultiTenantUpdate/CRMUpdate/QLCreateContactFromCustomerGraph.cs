using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CR.Extensions;
using PX.Objects.CR.Extensions.CRCreateActions;
using System;
using System.Collections;

namespace QLTenantCopyItems
{

    public sealed class QLCreateContactFromCustomerGraphExt : PXGraphExtension<CustomerMaint.CreateContactFromCustomerGraphExt, CustomerMaint>
    {
        public bool IsExtEnabled = true;

        [PXOverride]
        public IEnumerable createContact(PXAdapter adapter, QLCreateContactFromCustomerGraphExt.CreateContactDelegate baseMethod)
        {
            bool flag;
            if (base.Base1.AskExtConvert(false, out flag, Array.Empty<CRPopupValidator>()))
            {
                CustomerMaint customerMaint = base.Base.CloneGraphState<CustomerMaint>();
                PXLongOperation.StartOperation(base.Base, () => {
                    ConversionResult<Contact> result = customerMaint.GetProcessingExtension<CRCreateContactActionBase<CustomerMaint, Customer>>().Convert(null);
                    if (this.IsExtEnabled)
                    {
                        QLCustomerMaintExt mainExtension = this.Base.GetExtension<QLCustomerMaintExt>();
                        if ((mainExtension == null ? false : mainExtension.IsExtEnabled))
                        {
                            foreach (string otherTenant in mainExtension.otherTenantNames)
                            {
                                using (PXLoginScope ls = new PXLoginScope(string.Concat("admin@", otherTenant), Array.Empty<string>()))
                                {
                                    CustomerMaint graphForContactCreation = mainExtension.GetCustomerGraph(this.Base.CurrentCustomer.Current.AcctCD);
                                    QLCreateContactFromCustomerGraphExt thisExt = graphForContactCreation.GetExtension<QLCreateContactFromCustomerGraphExt>();
                                    thisExt.IsExtEnabled = false;
                                    ContactFilter contactFilterOtherTenant = thisExt.Base1.ContactInfo.Current;
                                    ContactFilter contactFilterThisTenant = this.Base1.ContactInfo.Current;
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.firstName>(contactFilterOtherTenant, contactFilterThisTenant.FirstName);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.lastName>(contactFilterOtherTenant, contactFilterThisTenant.LastName);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.salutation>(contactFilterOtherTenant, contactFilterThisTenant.Salutation);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.phone1Type>(contactFilterOtherTenant, contactFilterThisTenant.Phone1Type);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.phone2Type>(contactFilterOtherTenant, contactFilterThisTenant.Phone2Type);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.phone1>(contactFilterOtherTenant, contactFilterThisTenant.Phone1);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.phone2>(contactFilterOtherTenant, contactFilterThisTenant.Phone2);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.email>(contactFilterOtherTenant, contactFilterThisTenant.Email);
                                    thisExt.Base1.ContactInfo.Cache.SetValueExt<ContactFilter.contactClass>(contactFilterOtherTenant, contactFilterThisTenant.ContactClass);
                                    thisExt.Base1.ContactInfo.Cache.Update(contactFilterOtherTenant);
                                    graphForContactCreation.GetProcessingExtension<CRCreateContactActionBase<CustomerMaint, Customer>>().Convert(null);
                                    graphForContactCreation.Actions.PressSave();
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        this.Base1.Redirect(result);
                    }
                });
            }
            return adapter.Get();
        }

        [PXOverride]
        public IEnumerable createContactCancel(PXAdapter adapter, QLCreateContactFromCustomerGraphExt.CreateContactDelegate baseMethod)
        {
            PXTrace.WriteInformation("custom method3");
            return adapter.Get();
        }

        [PXOverride]
        public IEnumerable createContactFinish(PXAdapter adapter, QLCreateContactFromCustomerGraphExt.CreateContactDelegate baseMethod)
        {
            PXTrace.WriteInformation("custom method2");
            return baseMethod(adapter);
        }

        [PXOverride]
        public IEnumerable createContactFinishRedirect(PXAdapter adapter, QLCreateContactFromCustomerGraphExt.CreateContactDelegate baseMethod)
        {
            PXTrace.WriteInformation("custom method4");
            return baseMethod(adapter);
        }

        [PXOverride]
        public IEnumerable createContactRedirect(PXAdapter adapter, QLCreateContactFromCustomerGraphExt.CreateContactDelegate baseMethod)
        {
            PXTrace.WriteInformation("custom method5");
            return baseMethod(adapter);
        }

        public delegate IEnumerable CreateContactDelegate(PXAdapter adapter);
    }
}