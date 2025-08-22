using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using System.Collections.Generic;

namespace HubspotCustomization
{
    public class SOReleaseInvoice_Extension : PXGraphExtension<PX.Objects.SO.SOReleaseInvoice>
    {
        public static bool IsActive() => true;

        // Use instance-level cache instead of static to avoid cross-session issues
        private readonly Dictionary<int?, string> _contactEmailCache = new Dictionary<int?, string>();
        private bool _cacheLoaded = false;

        protected virtual void _(Events.RowSelected<ARInvoice> e)
        {
            ARInvoice invoice = e.Row;

            if (invoice == null) return;

            // Only show email field for email action
            bool isEmailAction = Base.Filter.Current.Action == PX.Objects.SO.SOReleaseInvoice.WellKnownActions.SOInvoiceScreen.EmailInvoice;
            
            PXUIFieldAttribute.SetVisible<ARInvoiceExt.usrEmail>(e.Cache, null, isEmailAction);
            PXUIFieldAttribute.SetVisibility<ARInvoiceExt.usrEmail>(e.Cache, null, 
                isEmailAction ? PXUIVisibility.Visible : PXUIVisibility.Invisible);

            // Only populate email if this is an email action and we have a customer
            if (invoice.CustomerID != null)
            {
                string email = GetCustomerEmail(invoice.CustomerID.Value);
                e.Cache.SetValueExt<ARInvoiceExt.usrEmail>(invoice, email ?? string.Empty);
            }
            else
            {
                // Clear email field when not in email action
                e.Cache.SetValueExt<ARInvoiceExt.usrEmail>(invoice, string.Empty);
            }
        }

        protected virtual void _(Events.RowUpdated<PX.Objects.SO.SOInvoiceFilter> e)
        {
            // Clear cache when filter changes
            if (e.Row?.Action != e.OldRow?.Action)
            {
                _contactEmailCache.Clear();
                _cacheLoaded = false;
            }
        }

        // Clear cache when graph is initialized/reused
        public override void Initialize()
        {
            base.Initialize();
            _contactEmailCache.Clear();
            _cacheLoaded = false;
        }

        private string GetCustomerEmail(int customerID)
        {
            // Check cache first
            if (_contactEmailCache.ContainsKey(customerID))
            {
                return _contactEmailCache[customerID];
            }

            // Load email for this specific customer
            string email = LoadCustomerEmail(customerID);
            
            // Cache the result (even if null/empty)
            _contactEmailCache[customerID] = email;
            
            return email;
        }

        private string LoadCustomerEmail(int customerID)
        {
            try
            {
                // First try to get the primary contact for the customer
                Contact contact = PXSelectJoin<Contact,
                    InnerJoin<Customer, On<Customer.defContactID, Equal<Contact.contactID>>>
                    , Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>
                    >.Select(Base, customerID);

                if (contact != null && !string.IsNullOrEmpty(contact.EMail))
                {
                    return contact.EMail;
                }

                // Fallback: try to find any contact associated with this customer
                contact = PXSelect<Contact,
                    Where<Contact.bAccountID, Equal<Required<Contact.bAccountID>>
                        , And<Contact.eMail, IsNotNull>>
                    >.Select(Base, customerID);

                if (contact != null && !string.IsNullOrEmpty(contact.EMail))
                {
                    return contact.EMail;
                }

                return string.Empty;
            }
            catch (System.Exception ex)
            {
                PXTrace.WriteError($"Error loading email for customer {customerID}: {ex.Message}");
                return string.Empty;
            }
        }
    }
}