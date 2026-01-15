using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLTenantCopyItems
{
    public sealed class QLContactMaintExt : PXGraphExtension<ContactMaint>
    {
        public static bool IsActive() => true;

        public List<string> otherTenantNames;

        public bool IsExtEnabled { get; set; } = true;
        protected void _(Events.RowPersisted<Contact> e)
        {
            if (this.IsExtEnabled)
            {
                Contact updatedContact = e.Row;
                if ((updatedContact == null ? false : e.TranStatus == PXTranStatus.Open))
                {
                    Customer relatedCustomer = PXSelectBase<Customer, PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Config>.Select(base.Base, new object[] { updatedContact.BAccountID });
                    if (relatedCustomer != null)
                    {
                        foreach (string s in this.otherTenantNames)
                        {
                            string userLogin = string.Concat("admin@", s);
                            using (PXLoginScope ls = new PXLoginScope(userLogin, Array.Empty<string>()))
                            {
                                ContactMaint contactMaint = this.GetContactGraph(relatedCustomer.AcctCD, updatedContact.DisplayName);
                                if (e.Operation == PXDBOperation.Delete)
                                {
                                    contactMaint.Contact.DeleteCurrent();
                                    contactMaint.Actions.PressSave();
                                }
                            }
                        }
                    }
                }
            }
        }

        public ContactMaint GetContactGraph(string acctCD, string displayName)
        {
            ContactMaint graph = PXGraph.CreateInstance<ContactMaint>();
            graph.GetExtension<QLContactMaintExt>().IsExtEnabled = false;
            PXResult<Contact> contact = PXSelectBase<Contact, PXSelectJoin<Contact, InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Contact.bAccountID>>>, Where<Contact.displayName, Equal<Required<Contact.displayName>>, And<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>>.Config>.Select(graph, new object[] { displayName, acctCD }).FirstOrDefault<PXResult<Contact>>();
            if (contact != null)
            {
                graph.ContactCurrent.Current = contact.GetItem<Contact>();
            }
            return graph;
        }

        public override void Initialize()
        {
            using (CustomSqlConnection sqlConnection = new CustomSqlConnection(PXDatabase.Provider.GetConnectionString()))
            {
                this.otherTenantNames = sqlConnection.GetOtherCompanyNames(base.Base.Accessinfo.CompanyName);
            }
        }
    }
}