using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.Repositories;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.CR.Extensions;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QLTenantCopyItems
{
    public sealed class QLCustomerMaintExt : PXGraphExtension<CustomerMaint>
    {
        public static bool IsActive() => true;

        public string BAcctCD_SourceNotModifiedCarrierAccounts = null;

        public List<CarrierPluginCustomer> SourceNotModifiedCarrierAccounts;

        public List<CarrierPluginCustomer> SourceModifiedCarrierAccounts;

        public List<string> otherTenantNames;

        public bool IsExtEnabled { get; set; } = true;
        protected void _(Events.RowPersisting<Customer> e)
        {
            if (this.IsExtEnabled)
            {
                Customer row = e.Row;
                if (row != null)
                {
                    PXGraph newGraph = PXGraph.CreateInstance<PXGraph>();
                    this.SourceNotModifiedCarrierAccounts = GraphHelper.RowCast<CarrierPluginCustomer>(PXSelectBase<CarrierPluginCustomer, PXSelect<CarrierPluginCustomer, Where<CarrierPluginCustomer.customerID, Equal<Required<Customer.bAccountID>>>>.Config>.Select(newGraph, new object[] { row.BAccountID })).ToList<CarrierPluginCustomer>();
                }
            }
        }

        protected void _(Events.RowPersisted<Customer> e)
        {
            try
            {
                if (this.IsExtEnabled)
                {
                    Customer updatedCustomer = e.Row;
                    if (updatedCustomer != null)
                    {
                        QLCustomerDialogResultsExt sourceDialogResultExt = PXCache<Customer>.GetExtension<QLCustomerDialogResultsExt>(updatedCustomer);
                        string callerTenantName = base.Base.Accessinfo.CompanyName;
                        string callerUserName = base.Base.Accessinfo.UserName;
                        string callerLoginScopeParam = string.Concat(callerUserName, "@", callerTenantName);
                        string updatedBAccountCD = updatedCustomer.AcctCD;
                        PXGraph sourceGraphForPXSelect = PXGraph.CreateInstance<PXGraph>();
                        foreach (string s in this.otherTenantNames)
                        {
                            string userLogin = string.Concat("admin@", s);
                            using (PXLoginScope ls = new PXLoginScope(userLogin, Array.Empty<string>()))
                            {
                                PXGraph targetGraphForPXSelect = PXGraph.CreateInstance<PXGraph>();
                                if (PXSelectBase<Customer, PXSelect<Customer, Where<Customer.acctCD, Equal<Required<Customer.acctCD>>>>.Config>.Select(targetGraphForPXSelect, new object[] { updatedBAccountCD }).FirstOrDefault<PXResult<Customer>>() != null)
                                {
                                    if (e.TranStatus == PXTranStatus.Open)
                                    {
                                        CustomerMaint targetMaint = this.GetCustomerGraph(updatedBAccountCD);
                                        if (e.Operation != PXDBOperation.Delete)
                                        {
                                            Customer targetCustomer = targetMaint.BAccount.Current;
                                            QLCustomerDialogResultsExt targetDialogResultExt = targetMaint.BAccount.Cache.GetExtension<QLCustomerDialogResultsExt>(targetCustomer);
                                            targetDialogResultExt.ConsolidateStatementsFieldUpdated = sourceDialogResultExt.ConsolidateStatementsFieldUpdated;
                                            targetDialogResultExt.ConsolidateToParentFieldUpdated = sourceDialogResultExt.ConsolidateToParentFieldUpdated;
                                            targetDialogResultExt.CustomerClassIDFieldVerifying = sourceDialogResultExt.CustomerClassIDFieldVerifying;
                                            targetDialogResultExt.GenerateOnDemandStatementDialogResult = sourceDialogResultExt.GenerateOnDemandStatementDialogResult;
                                            targetDialogResultExt.MaintVisibilityRestrictionDialogResult = sourceDialogResultExt.MaintVisibilityRestrictionDialogResult;
                                            targetDialogResultExt.SharedCreditPolicyFieldUpdated = sourceDialogResultExt.SharedCreditPolicyFieldUpdated;
                                            this.UpdateCustomerWithGraph(targetMaint, base.Base, sourceGraphForPXSelect, callerLoginScopeParam);
                                        }
                                        else
                                        {
                                            targetMaint.BAccount.DeleteCurrent();
                                            targetMaint.Actions.PressSave();
                                        }
                                    }
                                }
                                else if (e.TranStatus == PXTranStatus.Completed)
                                {
                                    if (e.Operation != PXDBOperation.Delete)
                                    {
                                        CustomerMaint.DefContactAddressExt sourceDefContactAddressExt = base.Base.GetExtension<CustomerMaint.DefContactAddressExt>();
                                        Contact sourceDefContact = null;
                                        using (PXLoginScope pXLoginScope = new PXLoginScope(userLogin, Array.Empty<string>()))
                                        {
                                            sourceDefContact = sourceDefContactAddressExt.DefContact.Current ?? sourceDefContactAddressExt.DefContact.SelectSingle(Array.Empty<object>());
                                        }
                                        CustomerMaint targetMaint = this.GetNewCustomerGraph(updatedBAccountCD, updatedCustomer, sourceDefContact, sourceGraphForPXSelect, callerLoginScopeParam, targetGraphForPXSelect);
                                        Customer targetCustomer = targetMaint.BAccount.Current;
                                        QLCustomerDialogResultsExt targetDialogResultExt = targetMaint.BAccount.Cache.GetExtension<QLCustomerDialogResultsExt>(targetCustomer);
                                        targetDialogResultExt.ConsolidateStatementsFieldUpdated = sourceDialogResultExt.ConsolidateStatementsFieldUpdated;
                                        targetDialogResultExt.ConsolidateToParentFieldUpdated = sourceDialogResultExt.ConsolidateToParentFieldUpdated;
                                        targetDialogResultExt.CustomerClassIDFieldVerifying = sourceDialogResultExt.CustomerClassIDFieldVerifying;
                                        targetDialogResultExt.GenerateOnDemandStatementDialogResult = sourceDialogResultExt.GenerateOnDemandStatementDialogResult;
                                        targetDialogResultExt.MaintVisibilityRestrictionDialogResult = sourceDialogResultExt.MaintVisibilityRestrictionDialogResult;
                                        targetDialogResultExt.SharedCreditPolicyFieldUpdated = sourceDialogResultExt.SharedCreditPolicyFieldUpdated;
                                        this.UpdateCustomerWithGraph(targetMaint, base.Base, sourceGraphForPXSelect, callerLoginScopeParam);
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                if ((!ex.Message.ToLower().Contains("last name") ? true : !ex.Message.ToLower().Contains("cannot be empty")))
                {
                    throw ex;
                }
            }
        }

        protected void Customer_ConsolidateStatements_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated baseMethod)
        {
            WebDialogResult? consolidateStatementsFieldUpdated;
            WebDialogResult webDialogResult;
            bool? consolidateStatements;
            bool valueOrDefault;
            bool flag;
            Customer customer = (Customer)e.Row;
            if (customer != null)
            {
                Type type = typeof(CustomerMaint);
                MethodInfo GetChildAccounts = type.GetMethod("GetChildAccounts", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo UpdateChildAccounts = type.GetMethod("UpdateChildAccounts", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo genericUpdateChildAccounts = UpdateChildAccounts.MakeGenericMethod(new Type[] { typeof(Customer.consolidateStatements) });
                if (this.IsExtEnabled)
                {
                    if (customer.ParentBAccountID.HasValue)
                    {
                        consolidateStatements = customer.ConsolidateStatements;
                        if (consolidateStatements.GetValueOrDefault() & consolidateStatements.HasValue)
                        {
                            Customer customerParent = base.Base.GetCustomerParent(customer);
                            if (customerParent != null)
                            {
                                customer.SendStatementByEmail = customerParent.SendStatementByEmail;
                                customer.PrintStatements = customerParent.PrintStatements;
                                customer.StatementType = customerParent.StatementType;
                                customer.PrintCuryStatements = customerParent.PrintCuryStatements;
                            }
                        }
                    }
                    else
                    {
                        string message = PXMessages.LocalizeFormatNoPrefix("Do you wish to update the {0} setting for all child accounts of this customer?", new object[] { PXUIFieldAttribute.GetDisplayName<Customer.consolidateStatements>(sender) });
                        QLCustomerDialogResultsExt dialogResultExt = sender.GetExtension<QLCustomerDialogResultsExt>(customer);
                        IEnumerable<Customer> customers = (IEnumerable<Customer>)GetChildAccounts.Invoke(base.Base, new object[] { false, false, false });
                        IEnumerable<Customer> enumr = customers;
                        if (!customers.Any<Customer>() || !e.ExternalCall)
                        {
                            flag = false;
                        }
                        else
                        {
                            WebDialogResult? nullable = new WebDialogResult?(base.Base.CurrentCustomer.Ask(message, MessageButtons.YesNo));
                            dialogResultExt.ConsolidateStatementsFieldUpdated = nullable;
                            consolidateStatementsFieldUpdated = nullable;
                            webDialogResult = WebDialogResult.Yes;
                            flag = consolidateStatementsFieldUpdated.GetValueOrDefault() == webDialogResult & consolidateStatementsFieldUpdated.HasValue;
                        }
                        if (flag)
                        {
                            genericUpdateChildAccounts.Invoke(base.Base, new object[] { sender, customer, enumr, null });
                        }
                    }
                }
                else if (customer.ParentBAccountID.HasValue)
                {
                    consolidateStatements = customer.ConsolidateStatements;
                    if (consolidateStatements.GetValueOrDefault() & consolidateStatements.HasValue)
                    {
                        Customer customerParent = base.Base.GetCustomerParent(customer);
                        if (customerParent != null)
                        {
                            customer.SendStatementByEmail = customerParent.SendStatementByEmail;
                            customer.PrintStatements = customerParent.PrintStatements;
                            customer.StatementType = customerParent.StatementType;
                            customer.PrintCuryStatements = customerParent.PrintCuryStatements;
                        }
                    }
                }
                else
                {
                    PXMessages.LocalizeFormatNoPrefix("Do you wish to update the {0} setting for all child accounts of this customer?", new object[] { PXUIFieldAttribute.GetDisplayName<Customer.consolidateStatements>(sender) });
                    QLCustomerDialogResultsExt dialogResultExt = sender.GetExtension<QLCustomerDialogResultsExt>(customer);
                    IEnumerable<Customer> customers1 = (IEnumerable<Customer>)GetChildAccounts.Invoke(base.Base, new object[] { false, false, false });
                    IEnumerable<Customer> enumr = customers1;
                    if (!customers1.Any<Customer>() || !e.ExternalCall)
                    {
                        valueOrDefault = false;
                    }
                    else
                    {
                        consolidateStatementsFieldUpdated = dialogResultExt.ConsolidateStatementsFieldUpdated;
                        webDialogResult = WebDialogResult.Yes;
                        valueOrDefault = consolidateStatementsFieldUpdated.GetValueOrDefault() == webDialogResult & consolidateStatementsFieldUpdated.HasValue;
                    }
                    if (valueOrDefault)
                    {
                        genericUpdateChildAccounts.Invoke(base.Base, new object[] { sender, customer, enumr, null });
                    }
                }
            }
        }

        protected void Customer_ConsolidateToParent_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated baseMethod)
        {
            WebDialogResult? consolidateToParentFieldUpdated;
            WebDialogResult webDialogResult;
            bool? sharedCreditPolicy;
            bool? consolidateToParent;
            bool flag;
            bool valueOrDefault;
            bool valueOrDefault1;
            bool flag1;
            bool valueOrDefault2;
            Customer customer = (Customer)e.Row;
            if (customer != null)
            {
                Type type = typeof(CustomerMaint);
                MethodInfo GetChildAccounts = type.GetMethod("GetChildAccounts", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo UpdateChildAccounts = type.GetMethod("UpdateChildAccounts", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo genericUpdateChildAccounts = UpdateChildAccounts.MakeGenericMethod(new Type[] { typeof(Customer.consolidateToParent) });
                if (this.IsExtEnabled)
                {
                    if (customer.ParentBAccountID.HasValue)
                    {
                        consolidateToParent = customer.SharedCreditPolicy;
                        if (consolidateToParent.GetValueOrDefault() & consolidateToParent.HasValue)
                        {
                            consolidateToParent = customer.ConsolidateToParent;
                            if (consolidateToParent.GetValueOrDefault() & consolidateToParent.HasValue)
                            {
                                goto Label3;
                            }
                            consolidateToParent = (bool?)e.OldValue;
                            flag = true;
                            flag1 = consolidateToParent.GetValueOrDefault() == flag & consolidateToParent.HasValue;
                            goto Label2;
                        }
                    Label3:
                        flag1 = false;
                    Label2:
                        if (flag1)
                        {
                            sender.SetValueExt<Customer.sharedCreditPolicy>(customer, false);
                        }
                    }
                    else
                    {
                        string message = PXMessages.LocalizeFormatNoPrefix("Do you wish to update the {0} setting for all child accounts of this customer?", new object[] { PXUIFieldAttribute.GetDisplayName<Customer.consolidateToParent>(sender) });
                        QLCustomerDialogResultsExt dialogResultExt = sender.GetExtension<QLCustomerDialogResultsExt>(customer);
                        IEnumerable<Customer> customers = (IEnumerable<Customer>)GetChildAccounts.Invoke(base.Base, new object[] { false, false, false });
                        IEnumerable<Customer> enumr = customers;
                        if (!customers.Any<Customer>() || !e.ExternalCall)
                        {
                            valueOrDefault2 = false;
                        }
                        else
                        {
                            WebDialogResult? nullable = new WebDialogResult?(base.Base.CurrentCustomer.Ask(message, MessageButtons.YesNo));
                            dialogResultExt.ConsolidateToParentFieldUpdated = nullable;
                            consolidateToParentFieldUpdated = nullable;
                            webDialogResult = WebDialogResult.Yes;
                            valueOrDefault2 = consolidateToParentFieldUpdated.GetValueOrDefault() == webDialogResult & consolidateToParentFieldUpdated.HasValue;
                        }
                        if (valueOrDefault2)
                        {
                            genericUpdateChildAccounts.Invoke(base.Base, new object[] { sender, customer, enumr, null });
                        }
                        Customer customer1 = customer;
                        sharedCreditPolicy = customer1.SharedCreditPolicy;
                        consolidateToParent = customer.ConsolidateToParent;
                        customer1.SharedCreditPolicy = (sharedCreditPolicy.GetValueOrDefault() || !consolidateToParent.GetValueOrDefault() && !sharedCreditPolicy.HasValue ? consolidateToParent : sharedCreditPolicy);
                    }
                }
                else if (customer.ParentBAccountID.HasValue)
                {
                    sharedCreditPolicy = customer.SharedCreditPolicy;
                    if (sharedCreditPolicy.GetValueOrDefault() & sharedCreditPolicy.HasValue)
                    {
                        sharedCreditPolicy = customer.ConsolidateToParent;
                        if (sharedCreditPolicy.GetValueOrDefault() & sharedCreditPolicy.HasValue)
                        {
                            valueOrDefault = false;
                        }
                        sharedCreditPolicy = (bool?)e.OldValue;
                        flag = true;
                        valueOrDefault = sharedCreditPolicy.GetValueOrDefault() == flag & sharedCreditPolicy.HasValue;
                        if (valueOrDefault)
                        {
                            sender.SetValueExt<Customer.sharedCreditPolicy>(customer, false);
                        }
                    }
                }
                else
                {
                    PXMessages.LocalizeFormatNoPrefix("Do you wish to update the {0} setting for all child accounts of this customer?", new object[] { PXUIFieldAttribute.GetDisplayName<Customer.consolidateToParent>(sender) });
                    QLCustomerDialogResultsExt dialogResultExt = sender.GetExtension<QLCustomerDialogResultsExt>(customer);
                    IEnumerable<Customer> customers1 = (IEnumerable<Customer>)GetChildAccounts.Invoke(base.Base, new object[] { false, false, false });
                    IEnumerable<Customer> enumr = customers1;
                    if (!customers1.Any<Customer>() || !e.ExternalCall)
                    {
                        valueOrDefault1 = false;
                    }
                    else
                    {
                        consolidateToParentFieldUpdated = dialogResultExt.ConsolidateToParentFieldUpdated;
                        webDialogResult = WebDialogResult.Yes;
                        valueOrDefault1 = consolidateToParentFieldUpdated.GetValueOrDefault() == webDialogResult & consolidateToParentFieldUpdated.HasValue;
                    }
                    if (valueOrDefault1)
                    {
                        genericUpdateChildAccounts.Invoke(base.Base, new object[] { sender, customer, enumr, null });
                    }
                    Customer customer2 = customer;
                    consolidateToParent = customer2.SharedCreditPolicy;
                    sharedCreditPolicy = customer.ConsolidateToParent;
                    customer2.SharedCreditPolicy = (consolidateToParent.GetValueOrDefault() || !sharedCreditPolicy.GetValueOrDefault() && !consolidateToParent.HasValue ? sharedCreditPolicy : consolidateToParent);
                }
            }
        }

        protected void Customer_CustomerClassID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e, PXFieldVerifying baseMethod)
        {
            WebDialogResult? customerClassIDFieldVerifying;
            WebDialogResult webDialogResult;
            bool valueOrDefault;
            bool flag;
            FieldInfo doCopyClassSettingsField = typeof(CustomerMaint).GetField("doCopyClassSettings", BindingFlags.Instance | BindingFlags.NonPublic);
            Customer customer = (Customer)e.Row;
            CustomerClass obj = (CustomerClass)PXSelectorAttribute.Select<Customer.customerClassID>(cache, customer, e.NewValue);
            if (!this.IsExtEnabled)
            {
                doCopyClassSettingsField.SetValue(base.Base, false);
                if (obj != null)
                {
                    QLCustomerDialogResultsExt dialogResultExt = cache.GetExtension<QLCustomerDialogResultsExt>(customer);
                    doCopyClassSettingsField.SetValue(base.Base, true);
                    if (cache.GetStatus(customer) == PXEntryStatus.Inserted)
                    {
                        valueOrDefault = false;
                    }
                    else
                    {
                        customerClassIDFieldVerifying = dialogResultExt.CustomerClassIDFieldVerifying;
                        webDialogResult = WebDialogResult.No;
                        valueOrDefault = customerClassIDFieldVerifying.GetValueOrDefault() == webDialogResult & customerClassIDFieldVerifying.HasValue;
                    }
                    if (valueOrDefault)
                    {
                        doCopyClassSettingsField.SetValue(base.Base, false);
                    }
                }
            }
            else
            {
                doCopyClassSettingsField.SetValue(base.Base, false);
                if (obj != null)
                {
                    QLCustomerDialogResultsExt dialogResultExt = cache.GetExtension<QLCustomerDialogResultsExt>(customer);
                    doCopyClassSettingsField.SetValue(base.Base, true);
                    if (cache.GetStatus(customer) == PXEntryStatus.Inserted)
                    {
                        flag = false;
                    }
                    else
                    {
                        WebDialogResult? nullable = new WebDialogResult?(base.Base.BAccount.Ask("Warning", "The customer class will be changed. Click Yes if you want to replace the customer settings with the default settings provided by the selected customer class. Click No if you want to keep the original customer settings.", MessageButtons.YesNo));
                        dialogResultExt.CustomerClassIDFieldVerifying = nullable;
                        customerClassIDFieldVerifying = nullable;
                        webDialogResult = WebDialogResult.No;
                        flag = customerClassIDFieldVerifying.GetValueOrDefault() == webDialogResult & customerClassIDFieldVerifying.HasValue;
                    }
                    if (flag)
                    {
                        doCopyClassSettingsField.SetValue(base.Base, false);
                    }
                }
            }
        }

        protected void Customer_SharedCreditPolicy_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e, PXFieldUpdated baseMethod)
        {
            WebDialogResult? sharedCreditPolicyFieldUpdated;
            WebDialogResult webDialogResult;
            bool? sharedCreditPolicy;
            bool valueOrDefault;
            bool flag;
            Customer customer = (Customer)e.Row;
            if (customer != null)
            {
                Type type = typeof(CustomerMaint);
                MethodInfo GetChildAccounts = type.GetMethod("GetChildAccounts", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo UpdateChildAccounts = type.GetMethod("UpdateChildAccounts", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo genericUpdateChildAccounts = UpdateChildAccounts.MakeGenericMethod(new Type[] { typeof(Customer.sharedCreditPolicy) });
                MethodInfo GetSharedCreditChildStatus = type.GetMethod("GetSharedCreditChildStatus", BindingFlags.Instance | BindingFlags.NonPublic);
                if (this.IsExtEnabled)
                {
                    if (customer.ParentBAccountID.HasValue)
                    {
                        sharedCreditPolicy = customer.SharedCreditPolicy;
                        if (!(sharedCreditPolicy.GetValueOrDefault() & sharedCreditPolicy.HasValue))
                        {
                            sharedCreditPolicy = (bool?)e.OldValue;
                            if (sharedCreditPolicy.GetValueOrDefault() & sharedCreditPolicy.HasValue)
                            {
                                customer.CreditLimit = new decimal?(decimal.Zero);
                            }
                        }
                        else
                        {
                            Customer customerParent = base.Base.GetCustomerParent(customer);
                            if (customerParent != null)
                            {
                                object[] parameters = new object[] { customerParent.Status, null };
                                string sharedCreditChildStatus = (string)GetSharedCreditChildStatus.Invoke(base.Base, parameters);
                                Func<Customer, bool> func = (Func<Customer, bool>)parameters[1];
                                customer.Status = (func(customer) ? sharedCreditChildStatus : customer.Status);
                                customer.CreditRule = customerParent.CreditRule;
                                customer.CreditLimit = customerParent.CreditLimit;
                                customer.CreditDaysPastDue = customerParent.CreditDaysPastDue;
                                customer.PrintDunningLetters = customerParent.PrintDunningLetters;
                                customer.MailDunningLetters = customerParent.MailDunningLetters;
                            }
                        }
                    }
                    else
                    {
                        string message = PXMessages.LocalizeFormatNoPrefix("Do you wish to update the {0} setting for all child accounts of this customer?", new object[] { PXUIFieldAttribute.GetDisplayName<Customer.sharedCreditPolicy>(sender) });
                        QLCustomerDialogResultsExt dialogResultExt = sender.GetExtension<QLCustomerDialogResultsExt>(customer);
                        IEnumerable<Customer> customers = (IEnumerable<Customer>)GetChildAccounts.Invoke(base.Base, new object[] { false, true, false });
                        IEnumerable<Customer> enumr = customers;
                        if (!customers.Any<Customer>() || !e.ExternalCall)
                        {
                            flag = false;
                        }
                        else
                        {
                            WebDialogResult? nullable = new WebDialogResult?(base.Base.CurrentCustomer.Ask(message, MessageButtons.YesNo));
                            dialogResultExt.SharedCreditPolicyFieldUpdated = nullable;
                            sharedCreditPolicyFieldUpdated = nullable;
                            webDialogResult = WebDialogResult.Yes;
                            flag = sharedCreditPolicyFieldUpdated.GetValueOrDefault() == webDialogResult & sharedCreditPolicyFieldUpdated.HasValue;
                        }
                        if (flag)
                        {
                            genericUpdateChildAccounts.Invoke(base.Base, new object[] { sender, customer, enumr });
                        }
                    }
                }
                else if (customer.ParentBAccountID.HasValue)
                {
                    sharedCreditPolicy = customer.SharedCreditPolicy;
                    if (!(sharedCreditPolicy.GetValueOrDefault() & sharedCreditPolicy.HasValue))
                    {
                        sharedCreditPolicy = (bool?)e.OldValue;
                        if (sharedCreditPolicy.GetValueOrDefault() & sharedCreditPolicy.HasValue)
                        {
                            customer.CreditLimit = new decimal?(decimal.Zero);
                        }
                    }
                    else
                    {
                        Customer customerParent = base.Base.GetCustomerParent(customer);
                        if (customerParent != null)
                        {
                            object[] parameters = new object[] { customerParent.Status, null };
                            string sharedCreditChildStatus = (string)GetSharedCreditChildStatus.Invoke(base.Base, parameters);
                            Func<Customer, bool> func = (Func<Customer, bool>)parameters[1];
                            customer.Status = (func(customer) ? sharedCreditChildStatus : customer.Status);
                            customer.CreditRule = customerParent.CreditRule;
                            customer.CreditLimit = customerParent.CreditLimit;
                            customer.CreditDaysPastDue = customerParent.CreditDaysPastDue;
                            customer.PrintDunningLetters = customerParent.PrintDunningLetters;
                            customer.MailDunningLetters = customerParent.MailDunningLetters;
                        }
                    }
                }
                else
                {
                    PXMessages.LocalizeFormatNoPrefix("Do you wish to update the {0} setting for all child accounts of this customer?", new object[] { PXUIFieldAttribute.GetDisplayName<Customer.sharedCreditPolicy>(sender) });
                    QLCustomerDialogResultsExt dialogResultExt = sender.GetExtension<QLCustomerDialogResultsExt>(customer);
                    IEnumerable<Customer> customers1 = (IEnumerable<Customer>)GetChildAccounts.Invoke(base.Base, new object[] { false, true, false });
                    IEnumerable<Customer> enumr = customers1;
                    if (!customers1.Any<Customer>() || !e.ExternalCall)
                    {
                        valueOrDefault = false;
                    }
                    else
                    {
                        sharedCreditPolicyFieldUpdated = dialogResultExt.SharedCreditPolicyFieldUpdated;
                        webDialogResult = WebDialogResult.Yes;
                        valueOrDefault = sharedCreditPolicyFieldUpdated.GetValueOrDefault() == webDialogResult & sharedCreditPolicyFieldUpdated.HasValue;
                    }
                    if (valueOrDefault)
                    {
                        genericUpdateChildAccounts.Invoke(base.Base, new object[] { sender, customer, enumr });
                    }
                }
            }
        }

        [PXOverride]
        public IEnumerable GenerateOnDemandStatement(PXAdapter adapter, Func<PXAdapter, IEnumerable> baseMethod)
        {
            Type type;
            IEnumerable enumerable;
            WebDialogResult? generateOnDemandStatementDialogResult;
            DateTime? statementDate;
            string statementCycleId;
            string str;
            Customer customer = base.Base.CurrentCustomer.Current;
            if (customer != null)
            {

                type = typeof(CustomerMaint);
                MethodInfo VerifyCanHaveSeparateStatement = type.GetMethod("VerifyCanHaveSeparateStatement", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo VerifyCanHaveOnDemandStatement = type.GetMethod("VerifyCanHaveOnDemandStatement", BindingFlags.Instance | BindingFlags.NonPublic);
                ARStatementCycle cycle = ARStatementCycle.PK.Find(Base, customer.StatementCycleId);
                if (!this.IsExtEnabled)
                {
                    VerifyCanHaveSeparateStatement.Invoke(base.Base, new object[] { customer });
                    VerifyCanHaveOnDemandStatement.Invoke(base.Base, new object[] { customer });
                    generateOnDemandStatementDialogResult = PXCache<Customer>.GetExtension<QLCustomerDialogResultsExt>(customer).GenerateOnDemandStatementDialogResult;
                    if (generateOnDemandStatementDialogResult.GetValueOrDefault() == WebDialogResult.OK & generateOnDemandStatementDialogResult.HasValue)
                    {
                        CustomerMaint.OnDemandStatementParameters current = base.Base.OnDemandStatementDialog.Current;
                        int num = 0;
                        if ((current == null ? false : current.StatementDate.HasValue))
                        {
                            statementDate = current.StatementDate;
                            num = statementDate.HasValue ? 1 : 0;
                        }
                        if (num != 0)
                        {
                            statementDate = this.Base.OnDemandStatementDialog.Current.StatementDate;
                            PXLongOperation.StartOperation(this, () => StatementCycleProcessBO.GenerateOnDemandStatement(
                  PXGraph.CreateInstance<StatementCycleProcessBO>(),
                  cycle,
                  customer,
                                    (DateTime)statementDate));
                            return adapter.Get();
                        }
                    }
                }
                else
                {
                    VerifyCanHaveSeparateStatement.Invoke(base.Base, new object[] { customer });
                    VerifyCanHaveOnDemandStatement.Invoke(base.Base, new object[] { customer });
                    QLCustomerDialogResultsExt dialogResultExt = PXCache<Customer>.GetExtension<QLCustomerDialogResultsExt>(customer);
                    WebDialogResult? nullable = new WebDialogResult?(base.Base.OnDemandStatementDialog.AskExt());
                    dialogResultExt.GenerateOnDemandStatementDialogResult = nullable;
                    generateOnDemandStatementDialogResult = nullable;
                    if (generateOnDemandStatementDialogResult.GetValueOrDefault() == WebDialogResult.OK & generateOnDemandStatementDialogResult.HasValue)
                    {
                        CustomerMaint.OnDemandStatementParameters current = base.Base.OnDemandStatementDialog.Current;
                        int num = 0;
                        if ((current == null ? false : current.StatementDate.HasValue))
                        {
                            statementDate = current.StatementDate;
                            num = statementDate.HasValue ? 1 : 0;
                        }
                        if (num != 0)
                        {
                            statementDate = this.Base.OnDemandStatementDialog.Current.StatementDate;
                            PXLongOperation.StartOperation(this, () => StatementCycleProcessBO.GenerateOnDemandStatement(
                                    PXGraph.CreateInstance<StatementCycleProcessBO>(),
                                    cycle,
                                    customer,
                                    (DateTime)statementDate));
                            return adapter.Get();
                        }
                    }
                }
            }
            return adapter.Get();
        }

        public CustomerMaint GetCustomerGraph(string acctCD)
        {
            CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
            graph.GetExtension<QLCustomerMaintExt>().IsExtEnabled = false;
            PXResult<Customer> customer = PXSelectBase<Customer, PXSelect<Customer, Where<Customer.acctCD, Equal<Required<Customer.acctCD>>>>.Config>.Select(graph, new object[] { acctCD }).FirstOrDefault<PXResult<Customer>>();
            if (customer != null)
            {
                graph.BAccount.Current = graph.BAccount.Search<Customer.acctCD>(customer.GetItem<Customer>().AcctCD, Array.Empty<object>());
            }
            return graph;
        }

        public CustomerMaint GetNewCustomerGraph(string acctCD, Customer updatedCustomer, Contact defContact, PXGraph sourcePXSelectGraph, string sourceLoginScopeParam, PXGraph targetPXSelectGraph)
        {
            int? nullable;
            object customerClassID;
            object subCD;
            int? subID;
            object obj;
            int? subID1;
            object accountCD;
            int? accountID;
            CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
            graph.GetExtension<QLCustomerMaintExt>().IsExtEnabled = false;
            Customer cust = (Customer)graph.CurrentCustomer.Cache.CreateInstance();
            graph.CurrentCustomer.Cache.SetValueExt<Customer.acctCD>(cust, acctCD);
            graph.CurrentCustomer.Cache.SetValueExt<Customer.acctName>(cust, updatedCustomer.AcctName);
            PXCache cache = graph.CurrentCustomer.Cache;
            Customer customer = cust;
            if (updatedCustomer != null)
            {
                customerClassID = updatedCustomer.CustomerClassID;
            }
            else
            {
                customerClassID = null;
            }
            cache.SetValueExt<Customer.customerClassID>(customer, customerClassID);
            graph.CurrentCustomer.Cache.SetValueExt<Customer.statementType>(cust, updatedCustomer.StatementType);
            graph.CurrentCustomer.Cache.SetValueExt<Customer.statementCycleId>(cust, updatedCustomer.StatementCycleId);
            Account sourceDiscTakenAcct = null;
            Sub sourceDiscTakenSub = null;
            using (PXLoginScope pXLoginScope = new PXLoginScope(sourceLoginScopeParam, Array.Empty<string>()))
            {
                sourceDiscTakenAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { updatedCustomer.DiscTakenAcctID });
                sourceDiscTakenSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { updatedCustomer.DiscTakenSubID });
            }
            int? targetDiscTakenAcctID = null;
            int? targetDiscTakenSubID = null;
            PXGraph pXGraph = targetPXSelectGraph;
            object[] objArray = new object[1];
            if (sourceDiscTakenSub != null)
            {
                subCD = sourceDiscTakenSub.SubCD;
            }
            else
            {
                subCD = null;
            }
            objArray[0] = subCD;
            Sub sub = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(pXGraph, objArray)).FirstOrDefault<Sub>();
            if (sub != null)
            {
                subID = sub.SubID;
            }
            else
            {
                nullable = null;
                subID = nullable;
            }
            targetDiscTakenSubID = subID;
            if (sourceDiscTakenAcct != null)
            {
                PXGraph pXGraph1 = targetPXSelectGraph;
                object[] objArray1 = new object[1];
                if (sourceDiscTakenAcct != null)
                {
                    accountCD = sourceDiscTakenAcct.AccountCD;
                }
                else
                {
                    accountCD = null;
                }
                objArray1[0] = accountCD;
                Account account = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(pXGraph1, objArray1)).FirstOrDefault<Account>();
                if (account != null)
                {
                    accountID = account.AccountID;
                }
                else
                {
                    nullable = null;
                    accountID = nullable;
                }
                targetDiscTakenAcctID = accountID;
                graph.CurrentCustomer.Cache.SetValueExt<Customer.discTakenAcctID>(cust, targetDiscTakenAcctID);
            }
            if (sourceDiscTakenSub != null)
            {
                PXGraph pXGraph2 = targetPXSelectGraph;
                object[] objArray2 = new object[1];
                if (sourceDiscTakenSub != null)
                {
                    obj = sourceDiscTakenSub.SubCD;
                }
                else
                {
                    obj = null;
                }
                objArray2[0] = obj;
                Sub sub1 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(pXGraph2, objArray2)).FirstOrDefault<Sub>();
                if (sub1 != null)
                {
                    subID1 = sub1.SubID;
                }
                else
                {
                    nullable = null;
                    subID1 = nullable;
                }
                targetDiscTakenSubID = subID1;
                graph.CurrentCustomer.Cache.SetValueExt<Customer.discTakenSubID>(cust, targetDiscTakenSubID);
            }
            cust = graph.CurrentCustomer.Insert(cust);
            CustomerMaint.DefContactAddressExt defContactAddressExt = graph.GetExtension<CustomerMaint.DefContactAddressExt>();
            Contact targetDefContact = defContactAddressExt.DefContact.Current ?? defContactAddressExt.DefContact.SelectSingle(Array.Empty<object>());

            if (defContact != null)
            {
                //update contact if changes found
                defContactAddressExt.DefContact.Cache.SetValueExt<Contact.eMail>(targetDefContact, defContact.EMail);
                defContactAddressExt.DefContact.Cache.Update(targetDefContact);
            }

            graph.Actions.PressSave();
            return graph;
        }

        public override void Initialize()
        {
            using (CustomSqlConnection sqlConnection = new CustomSqlConnection(PXDatabase.Provider.GetConnectionString()))
            {
                this.otherTenantNames = sqlConnection.GetOtherCompanyNames(base.Base.Accessinfo.CompanyName);
            }
        }

        public void UpdateCustomerWithGraph(CustomerMaint target, CustomerMaint source, PXGraph sourcePXSelectGraph, string sourceLoginScopeParam)
        {
            int? primaryContactID;
            int? contactID;
            object acctCD;
            int? bAccountID;
            object obj;
            int? bAccountID1;
            object branchCD;
            int? branchID;
            object contractCD;
            int? contractID;
            object siteCD;
            int? siteID;
            object accountCD;
            int? accountID;
            object subCD;
            int? subID;
            object accountCD1;
            int? accountID1;
            object subCD1;
            int? subID1;
            object obj1;
            int? nullable1;
            object subCD2;
            int? subID2;
            object accountCD2;
            int? accountID2;
            object obj2;
            int? nullable2;
            object accountCD3;
            int? accountID3;
            object subCD3;
            int? subID3;
            object obj3;
            int? nullable3;
            object subCD4;
            int? subID4;
            object accountCD4;
            int? accountID4;
            object obj4;
            int? nullable4;
            object cashAccountCD;
            int? cashAccountID;
            bool valueOrDefault;
            int? nullable5;
            object status;
            object customerClassID;
            bool hasValue;
            CustomerMaint.DefContactAddressExt sourceDefContactAddressExt = source.GetExtension<CustomerMaint.DefContactAddressExt>();
            CustomerMaint.PrimaryContactGraphExt sourcePrimaryContactGraphExt = source.GetExtension<CustomerMaint.PrimaryContactGraphExt>();
            CustomerMaint.PaymentDetailsExt sourcePaymentDetailsExt = source.GetExtension<CustomerMaint.PaymentDetailsExt>();
            CustomerMaint.DefLocationExt sourceDefLocationExt = source.GetExtension<CustomerMaint.DefLocationExt>();
            CustomerMaint.DefContactAddressExt targetDefContactAddressExt = target.GetExtension<CustomerMaint.DefContactAddressExt>();
            CustomerMaint.PrimaryContactGraphExt targetPrimaryContactGraphExt = target.GetExtension<CustomerMaint.PrimaryContactGraphExt>();
            CustomerMaint.PaymentDetailsExt targetPaymentDetailsExt = target.GetExtension<CustomerMaint.PaymentDetailsExt>();
            CustomerMaint.DefLocationExt targetDefLocationExt = target.GetExtension<CustomerMaint.DefLocationExt>();
            CustomerMaint.ContactDetailsExt targetContactsDetailsExt = target.GetExtension<CustomerMaint.ContactDetailsExt>();
            CustomerMaint.LocationDetailsExt targetLocationsDetailsExt = target.GetExtension<CustomerMaint.LocationDetailsExt>();
            PXGraph targetGraphForPXSelect = PXGraph.CreateInstance<PXGraph>();
            target.GetExtension<CRPrimaryContactGraphExt<CustomerMaint, CustomerMaint.ContactDetailsExt, Customer, Customer.bAccountID, Customer.primaryContactID>>();
            Address sourceDefAddress = null;
            Contact sourceDefContact = null;
            Contact sourcePrimaryContact = null;
            Customer sourceCurrentCustomer = null;
            CustomerMaint.CustomerBalanceSummary sourceCustomerBalanceSummary = null;
            CustomerPaymentMethod sourceDefCustomerPaymentMethodInstance = null;
            Address sourceBillAddress = null;
            Contact sourceBillContact = null;
            PX.Objects.CR.Standalone.Location sourceDefLocation = null;
            Address sourceDefLocationAddress = null;
            Contact sourceDefLocationContact = null;
            Customer sourceBAccount = null;
            Branch sourceCBranch = null;
            PMProject sourceCDefProject = null;
            INSite sourceCSite = null;
            Account sourceCARAccount = null;
            Sub sourceCARSub = null;
            Account sourceCSalesAcct = null;
            Sub sourceCSalesSub = null;
            Account sourceCDiscountAcct = null;
            Sub sourceCDiscountSub = null;
            Account sourceCFreightAcct = null;
            Sub sourceCFreightSub = null;
            Account sourceDiscTakenAcct = null;
            Sub sourceDiscTakenSub = null;
            Account sourcePrepaymentAcct = null;
            Sub sourcePrepaymentSub = null;
            Account sourceCRetainageAcct = null;
            Sub sourceCRetainageSub = null;
            CashAccount sourceCashAccount = null;
            List<CustomerPaymentMethodInfo> sourceCustomerPaymentMethodsInfo = null;
            BAccount sourceParentBAccount = null;
            BAccount sourceCOrgBAccount = null;
            using (PXLoginScope pXLoginScope = new PXLoginScope(sourceLoginScopeParam, Array.Empty<string>()))
            {
                sourceDefAddress = sourceDefContactAddressExt.DefAddress.SelectSingle(Array.Empty<object>());
                sourceDefContact = sourceDefContactAddressExt.DefContact.SelectSingle(Array.Empty<object>());
                primaryContactID = source.BAccount.Current.PrimaryContactID;
                if (primaryContactID.HasValue)
                {
                    sourcePrimaryContact = sourcePrimaryContactGraphExt.PrimaryContactCurrent.SelectSingle(Array.Empty<object>());
                }
                sourceCurrentCustomer = source.CurrentCustomer.SelectSingle(Array.Empty<object>());
                sourceCustomerBalanceSummary = source.CustomerBalance.SelectSingle(Array.Empty<object>());
                sourceDefCustomerPaymentMethodInstance = sourcePaymentDetailsExt.DefPaymentMethodInstance.SelectSingle(Array.Empty<object>());
                sourceBillAddress = source.BillAddress.SelectSingle(Array.Empty<object>());
                sourceBillContact = source.BillContact.SelectSingle(Array.Empty<object>());
                sourceDefLocation = sourceDefLocationExt.DefLocation.SelectSingle(Array.Empty<object>());
                sourceDefLocationAddress = sourceDefLocationExt.DefLocationAddress.SelectSingle(Array.Empty<object>());
                sourceDefLocationContact = sourceDefLocationExt.DefLocationContact.SelectSingle(Array.Empty<object>());
                sourceBAccount = source.BAccount.Current;
                sourceCustomerPaymentMethodsInfo = GraphHelper.RowCast<CustomerPaymentMethodInfo>(sourcePaymentDetailsExt.PaymentMethods.Select(Array.Empty<object>())).ToList<CustomerPaymentMethodInfo>();
                if (sourceDefLocation != null)
                {
                    sourceCBranch = PXSelectBase<Branch, PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CBranchID });
                    sourceCDefProject = PXSelectBase<PMProject, PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CDefProjectID });
                    sourceCSite = PXSelectBase<INSite, PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CSiteID });
                    sourceCARAccount = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CARAccountID });
                    sourceCARSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CARSubID });
                    sourceCSalesAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CSalesAcctID });
                    sourceCSalesSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CSalesSubID });
                    sourceCDiscountAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CDiscountAcctID });
                    sourceCDiscountSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CDiscountSubID });
                    sourceCFreightAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CFreightAcctID });
                    sourceCFreightSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CFreightSubID });
                    sourceCRetainageAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CRetainageAcctID });
                    sourceCRetainageSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefLocation.CRetainageSubID });
                }
                if (sourceCurrentCustomer != null)
                {
                    sourceParentBAccount = PXSelectBase<BAccount, PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceCurrentCustomer.ParentBAccountID });
                    sourceCOrgBAccount = PXSelectBase<BAccount, PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceCurrentCustomer.COrgBAccountID });
                    sourceDiscTakenAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceCurrentCustomer.DiscTakenAcctID });
                    sourceDiscTakenSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceCurrentCustomer.DiscTakenSubID });
                    sourcePrepaymentAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceCurrentCustomer.PrepaymentAcctID });
                    sourcePrepaymentSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceCurrentCustomer.PrepaymentSubID });
                }
                if (sourceDefCustomerPaymentMethodInstance == null)
                {
                    sourceCashAccount = null;
                }
                else
                {
                    sourceCashAccount = PXSelectBase<CashAccount, PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceDefCustomerPaymentMethodInstance.CashAccountID });
                }
                this.SourceModifiedCarrierAccounts = GraphHelper.RowCast<CarrierPluginCustomer>(base.Base.Carriers.Select(Array.Empty<object>())).ToList<CarrierPluginCustomer>();
            }
            List<Contact> targetContacts = GraphHelper.RowCast<Contact>(targetContactsDetailsExt.Contacts.Select(Array.Empty<object>())).ToList<Contact>();
            if ((sourcePrimaryContact == null ? false : sourcePrimaryContact.MemberName != null))
            {
                foreach (Contact c in targetContacts)
                {
                    if (c.MemberName == sourcePrimaryContact.MemberName)
                    {
                        targetContactsDetailsExt.Contacts.Cache.Current = c;
                        targetPrimaryContactGraphExt.MakeContactPrimary.Press();
                        break;
                    }
                }
            }
            List<PX.Objects.CR.Standalone.Location> targetLocations = GraphHelper.RowCast<PX.Objects.CR.Standalone.Location>(targetLocationsDetailsExt.Locations.Select(Array.Empty<object>())).ToList<PX.Objects.CR.Standalone.Location>();

            if (sourceDefLocation != null)
            {
                foreach (PX.Objects.CR.Standalone.Location l in targetLocations)
                {
                    if (l.LocationCD == sourceDefLocation.LocationCD)
                    {
                        targetLocationsDetailsExt.Locations.Cache.Current = l;
                        targetDefLocationExt.SetDefaultLocation.Press();
                        try
                        {
                            target.Actions.PressSave();
                        }
                        catch (Exception exception)
                        {
                            Exception e = exception;
                            if ((!e.Message.ToLower().Contains("last name") ? true : !e.Message.ToLower().Contains("cannot be empty")))
                            {
                                throw e;
                            }
                        }
                        break;
                    }
                }
            }

            Customer targetCurrentCustomer = target.CurrentCustomer.SelectSingle(Array.Empty<object>());
            Address targetDefAddress = targetDefContactAddressExt.DefAddress.SelectSingle(Array.Empty<object>());
            Contact targetDefContact = targetDefContactAddressExt.DefContact.SelectSingle(Array.Empty<object>());
            Contact targetPrimaryContact = targetPrimaryContactGraphExt.PrimaryContactCurrent.SelectSingle(Array.Empty<object>());
            CustomerMaint.CustomerBalanceSummary targetCustomerBalanceSummary = target.CustomerBalance.SelectSingle(Array.Empty<object>());
            PX.Objects.CR.Standalone.Location targetDefLocation = targetDefLocationExt.DefLocation.SelectSingle(Array.Empty<object>());
            List<CustomerPaymentMethodInfo> targetCustomerPaymentMethodsInfo = GraphHelper.RowCast<CustomerPaymentMethodInfo>(targetPaymentDetailsExt.PaymentMethods.Select(Array.Empty<object>())).ToList<CustomerPaymentMethodInfo>();
            foreach (CustomerPaymentMethodInfo m in targetCustomerPaymentMethodsInfo)
            {
                if (m.PaymentMethodID == sourceCurrentCustomer.DefPaymentMethodID)
                {
                    targetPaymentDetailsExt.PaymentMethods.Cache.SetValueExt<CustomerPaymentMethodInfo.isDefault>(m, true);
                    targetPaymentDetailsExt.PaymentMethods.Cache.Update(m);
                    break;
                }
            }
            Customer targetBAccount = target.BAccount.Current;
            CustomerMaint customerMaint = target;
            object[] objArray = new object[1];
            if (sourceParentBAccount != null)
            {
                acctCD = sourceParentBAccount.AcctCD;
            }
            else
            {
                acctCD = null;
            }
            objArray[0] = acctCD;
            BAccount bAccount = GraphHelper.RowCast<BAccount>(PXSelectBase<BAccount, PXSelect<BAccount, Where<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>.Config>.Select(customerMaint, objArray)).FirstOrDefault<BAccount>();
            if (bAccount != null)
            {
                bAccountID = bAccount.BAccountID;
            }
            else
            {
                primaryContactID = null;
                bAccountID = primaryContactID;
            }
            int? targetParentBAccountID = bAccountID;
            PXGraph pXGraph = targetGraphForPXSelect;
            object[] objArray1 = new object[1];
            if (sourceCOrgBAccount != null)
            {
                obj = sourceCOrgBAccount.AcctCD;
            }
            else
            {
                obj = null;
            }
            objArray1[0] = obj;
            BAccount bAccount1 = GraphHelper.RowCast<BAccount>(PXSelectBase<BAccount, PXSelect<BAccount, Where<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>.Config>.Select(pXGraph, objArray1)).FirstOrDefault<BAccount>();
            if (bAccount1 != null)
            {
                bAccountID1 = bAccount1.BAccountID;
            }
            else
            {
                primaryContactID = null;
                bAccountID1 = primaryContactID;
            }
            int? targetCOrgBAccountID = bAccountID1;
            CustomerMaint customerMaint1 = target;
            object[] objArray2 = new object[1];
            if (sourceCBranch != null)
            {
                branchCD = sourceCBranch.BranchCD;
            }
            else
            {
                branchCD = null;
            }
            objArray2[0] = branchCD;
            Branch branch = GraphHelper.RowCast<Branch>(PXSelectBase<Branch, PXSelect<Branch, Where<Branch.branchCD, Equal<Required<Branch.branchCD>>>>.Config>.Select(customerMaint1, objArray2)).FirstOrDefault<Branch>();
            if (branch != null)
            {
                branchID = branch.BranchID;
            }
            else
            {
                primaryContactID = null;
                branchID = primaryContactID;
            }
            int? targetCBranchID = branchID;
            CustomerMaint customerMaint2 = target;
            object[] objArray3 = new object[1];
            if (sourceCDefProject != null)
            {
                contractCD = sourceCDefProject.ContractCD;
            }
            else
            {
                contractCD = null;
            }
            objArray3[0] = contractCD;
            PMProject pMProject = GraphHelper.RowCast<PMProject>(PXSelectBase<PMProject, PXSelect<PMProject, Where<PMProject.contractCD, Equal<Required<PMProject.contractCD>>>>.Config>.Select(customerMaint2, objArray3)).FirstOrDefault<PMProject>();
            if (pMProject != null)
            {
                contractID = pMProject.ContractID;
            }
            else
            {
                primaryContactID = null;
                contractID = primaryContactID;
            }
            int? targetCDefProjectID = contractID;
            CustomerMaint customerMaint3 = target;
            object[] objArray4 = new object[1];
            if (sourceCSite != null)
            {
                siteCD = sourceCSite.SiteCD;
            }
            else
            {
                siteCD = null;
            }
            objArray4[0] = siteCD;
            INSite nSite = GraphHelper.RowCast<INSite>(PXSelectBase<INSite, PXSelect<INSite, Where<INSite.siteCD, Equal<Required<INSite.siteCD>>>>.Config>.Select(customerMaint3, objArray4)).FirstOrDefault<INSite>();
            if (nSite != null)
            {
                siteID = nSite.SiteID;
            }
            else
            {
                primaryContactID = null;
                siteID = primaryContactID;
            }
            int? targetCSiteID = siteID;
            CustomerMaint customerMaint4 = target;
            object[] objArray5 = new object[1];
            if (sourceCARAccount != null)
            {
                accountCD = sourceCARAccount.AccountCD;
            }
            else
            {
                accountCD = null;
            }
            objArray5[0] = accountCD;
            Account account = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerMaint4, objArray5)).FirstOrDefault<Account>();
            if (account != null)
            {
                accountID = account.AccountID;
            }
            else
            {
                primaryContactID = null;
                accountID = primaryContactID;
            }
            int? targetCARAccountID = accountID;
            CustomerMaint customerMaint5 = target;
            object[] objArray6 = new object[1];
            if (sourceCARSub != null)
            {
                subCD = sourceCARSub.SubCD;
            }
            else
            {
                subCD = null;
            }
            objArray6[0] = subCD;
            Sub sub = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerMaint5, objArray6)).FirstOrDefault<Sub>();
            if (sub != null)
            {
                subID = sub.SubID;
            }
            else
            {
                primaryContactID = null;
                subID = primaryContactID;
            }
            int? targetCARSubID = subID;
            CustomerMaint customerMaint6 = target;
            object[] objArray7 = new object[1];
            if (sourceCSalesAcct != null)
            {
                accountCD1 = sourceCSalesAcct.AccountCD;
            }
            else
            {
                accountCD1 = null;
            }
            objArray7[0] = accountCD1;
            Account account1 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerMaint6, objArray7)).FirstOrDefault<Account>();
            if (account1 != null)
            {
                accountID1 = account1.AccountID;
            }
            else
            {
                primaryContactID = null;
                accountID1 = primaryContactID;
            }
            int? targetCSalesAcctID = accountID1;
            CustomerMaint customerMaint7 = target;
            object[] objArray8 = new object[1];
            if (sourceCSalesSub != null)
            {
                subCD1 = sourceCSalesSub.SubCD;
            }
            else
            {
                subCD1 = null;
            }
            objArray8[0] = subCD1;
            Sub sub1 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerMaint7, objArray8)).FirstOrDefault<Sub>();
            if (sub1 != null)
            {
                subID1 = sub1.SubID;
            }
            else
            {
                primaryContactID = null;
                subID1 = primaryContactID;
            }
            int? targetCSalesSubID = subID1;
            CustomerMaint customerMaint8 = target;
            object[] objArray9 = new object[1];
            if (sourceCDiscountAcct != null)
            {
                obj1 = sourceCDiscountAcct.AccountCD;
            }
            else
            {
                obj1 = null;
            }
            objArray9[0] = obj1;
            Account account2 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerMaint8, objArray9)).FirstOrDefault<Account>();
            if (account2 != null)
            {
                nullable1 = account2.AccountID;
            }
            else
            {
                primaryContactID = null;
                nullable1 = primaryContactID;
            }
            int? targetCDiscountAcctID = nullable1;
            CustomerMaint customerMaint9 = target;
            object[] objArray10 = new object[1];
            if (sourceCDiscountSub != null)
            {
                subCD2 = sourceCDiscountSub.SubCD;
            }
            else
            {
                subCD2 = null;
            }
            objArray10[0] = subCD2;
            Sub sub2 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerMaint9, objArray10)).FirstOrDefault<Sub>();
            if (sub2 != null)
            {
                subID2 = sub2.SubID;
            }
            else
            {
                primaryContactID = null;
                subID2 = primaryContactID;
            }
            int? targetCDiscountSubID = subID2;
            CustomerMaint customerMaint10 = target;
            object[] objArray11 = new object[1];
            if (sourceCFreightAcct != null)
            {
                accountCD2 = sourceCFreightAcct.AccountCD;
            }
            else
            {
                accountCD2 = null;
            }
            objArray11[0] = accountCD2;
            Account account3 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerMaint10, objArray11)).FirstOrDefault<Account>();
            if (account3 != null)
            {
                accountID2 = account3.AccountID;
            }
            else
            {
                primaryContactID = null;
                accountID2 = primaryContactID;
            }
            int? targetCFreightAcctID = accountID2;
            CustomerMaint customerMaint11 = target;
            object[] objArray12 = new object[1];
            if (sourceCFreightSub != null)
            {
                obj2 = sourceCFreightSub.SubCD;
            }
            else
            {
                obj2 = null;
            }
            objArray12[0] = obj2;
            Sub sub3 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerMaint11, objArray12)).FirstOrDefault<Sub>();
            if (sub3 != null)
            {
                nullable2 = sub3.SubID;
            }
            else
            {
                primaryContactID = null;
                nullable2 = primaryContactID;
            }
            int? targetCFreightSubID = nullable2;
            CustomerMaint customerMaint12 = target;
            object[] objArray13 = new object[1];
            if (sourceDiscTakenAcct != null)
            {
                accountCD3 = sourceDiscTakenAcct.AccountCD;
            }
            else
            {
                accountCD3 = null;
            }
            objArray13[0] = accountCD3;
            Account account4 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerMaint12, objArray13)).FirstOrDefault<Account>();
            if (account4 != null)
            {
                accountID3 = account4.AccountID;
            }
            else
            {
                primaryContactID = null;
                accountID3 = primaryContactID;
            }
            int? targetDiscTakenAcctID = accountID3;
            CustomerMaint customerMaint13 = target;
            object[] objArray14 = new object[1];
            if (sourceDiscTakenSub != null)
            {
                subCD3 = sourceDiscTakenSub.SubCD;
            }
            else
            {
                subCD3 = null;
            }
            objArray14[0] = subCD3;
            Sub sub4 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerMaint13, objArray14)).FirstOrDefault<Sub>();
            if (sub4 != null)
            {
                subID3 = sub4.SubID;
            }
            else
            {
                primaryContactID = null;
                subID3 = primaryContactID;
            }
            int? targetDiscTakenSubID = subID3;
            CustomerMaint customerMaint14 = target;
            object[] objArray15 = new object[1];
            if (sourcePrepaymentAcct != null)
            {
                obj3 = sourcePrepaymentAcct.AccountCD;
            }
            else
            {
                obj3 = null;
            }
            objArray15[0] = obj3;
            Account account5 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerMaint14, objArray15)).FirstOrDefault<Account>();
            if (account5 != null)
            {
                nullable3 = account5.AccountID;
            }
            else
            {
                primaryContactID = null;
                nullable3 = primaryContactID;
            }
            int? targetPrepaymentAcctID = nullable3;
            CustomerMaint customerMaint15 = target;
            object[] objArray16 = new object[1];
            if (sourcePrepaymentSub != null)
            {
                subCD4 = sourcePrepaymentSub.SubCD;
            }
            else
            {
                subCD4 = null;
            }
            objArray16[0] = subCD4;
            Sub sub5 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerMaint15, objArray16)).FirstOrDefault<Sub>();
            if (sub5 != null)
            {
                subID4 = sub5.SubID;
            }
            else
            {
                primaryContactID = null;
                subID4 = primaryContactID;
            }
            int? targetPrepaymentSubID = subID4;
            CustomerMaint customerMaint16 = target;
            object[] objArray17 = new object[1];
            if (sourceCRetainageAcct != null)
            {
                accountCD4 = sourceCRetainageAcct.AccountCD;
            }
            else
            {
                accountCD4 = null;
            }
            objArray17[0] = accountCD4;
            Account account6 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerMaint16, objArray17)).FirstOrDefault<Account>();
            if (account6 != null)
            {
                accountID4 = account6.AccountID;
            }
            else
            {
                primaryContactID = null;
                accountID4 = primaryContactID;
            }
            int? targetCRetainageAcctID = accountID4;
            CustomerMaint customerMaint17 = target;
            object[] objArray18 = new object[1];
            if (sourceCRetainageSub != null)
            {
                obj4 = sourceCRetainageSub.SubCD;
            }
            else
            {
                obj4 = null;
            }
            objArray18[0] = obj4;
            Sub sub6 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerMaint17, objArray18)).FirstOrDefault<Sub>();
            if (sub6 != null)
            {
                nullable4 = sub6.SubID;
            }
            else
            {
                primaryContactID = null;
                nullable4 = primaryContactID;
            }
            int? targetCRetainageSubID = nullable4;
            CustomerMaint customerMaint18 = target;
            object[] objArray19 = new object[1];
            if (sourceCashAccount != null)
            {
                cashAccountCD = sourceCashAccount.CashAccountCD;
            }
            else
            {
                cashAccountCD = null;
            }
            objArray19[0] = cashAccountCD;
            CashAccount cashAccount = GraphHelper.RowCast<CashAccount>(PXSelectBase<CashAccount, PXSelect<CashAccount, Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>.Config>.Select(customerMaint18, objArray19)).FirstOrDefault<CashAccount>();
            if (cashAccount != null)
            {
                cashAccountID = cashAccount.CashAccountID;
            }
            else
            {
                primaryContactID = null;
                cashAccountID = primaryContactID;
            }
            int? targetCashAccountID = cashAccountID;
            if ((targetBAccount == null ? false : sourceBAccount != null))
            {
                PXCache cache = target.BAccount.Cache;
                Customer customer = targetBAccount;
                if (sourceBAccount != null)
                {
                    status = sourceBAccount.Status;
                }
                else
                {
                    status = null;
                }
                cache.SetValueExt<BAccount.status>(customer, status);
                PXCache pXCache = target.BAccount.Cache;
                Customer customer1 = targetBAccount;
                if (sourceBAccount != null)
                {
                    customerClassID = sourceBAccount.CustomerClassID;
                }
                else
                {
                    customerClassID = null;
                }
                pXCache.SetValueExt<Customer.customerClassID>(customer1, customerClassID);
                primaryContactID = sourceBAccount.PrimaryContactID;
                if (primaryContactID.HasValue)
                {
                    hasValue = false;
                }
                else
                {
                    primaryContactID = targetBAccount.PrimaryContactID;
                    hasValue = primaryContactID.HasValue;
                }
                if (hasValue)
                {
                    target.BAccount.Cache.SetValueExt<BAccount.primaryContactID>(targetBAccount, null);
                }
                target.BAccount.Cache.Update(targetBAccount);
            }
            if ((targetCurrentCustomer == null ? false : sourceCurrentCustomer != null))
            {
                target.CurrentCustomer.Cache.SetValueExt<BAccount.acctName>(targetCurrentCustomer, sourceCurrentCustomer.AcctName);
                target.CurrentCustomer.Cache.SetValueExt<BAccount.acctReferenceNbr>(targetCurrentCustomer, sourceCurrentCustomer.AcctReferenceNbr);
                target.CurrentCustomer.Cache.SetValueExt<Customer.termsID>(targetCurrentCustomer, sourceCurrentCustomer.TermsID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.statementCycleId>(targetCurrentCustomer, sourceCurrentCustomer.StatementCycleId);
                target.CurrentCustomer.Cache.SetValueExt<Customer.autoApplyPayments>(targetCurrentCustomer, sourceCurrentCustomer.AutoApplyPayments);
                target.CurrentCustomer.Cache.SetValueExt<Customer.finChargeApply>(targetCurrentCustomer, sourceCurrentCustomer.FinChargeApply);
                target.CurrentCustomer.Cache.SetValueExt<Customer.smallBalanceAllow>(targetCurrentCustomer, sourceCurrentCustomer.SmallBalanceAllow);
                target.CurrentCustomer.Cache.SetValueExt<Customer.smallBalanceLimit>(targetCurrentCustomer, sourceCurrentCustomer.SmallBalanceLimit);
                target.CurrentCustomer.Cache.SetValueExt<Customer.allowOverrideCury>(targetCurrentCustomer, sourceCurrentCustomer.AllowOverrideCury);
                target.CurrentCustomer.Cache.SetValueExt<Customer.curyID>(targetCurrentCustomer, sourceCurrentCustomer.CuryID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.allowOverrideRate>(targetCurrentCustomer, sourceCurrentCustomer.AllowOverrideRate);
                target.CurrentCustomer.Cache.SetValueExt<Customer.curyRateTypeID>(targetCurrentCustomer, sourceCurrentCustomer.CuryRateTypeID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.paymentsByLinesAllowed>(targetCurrentCustomer, sourceCurrentCustomer.PaymentsByLinesAllowed);
                target.CurrentCustomer.Cache.SetValueExt<Customer.retainageApply>(targetCurrentCustomer, sourceCurrentCustomer.RetainageApply);
                target.CurrentCustomer.Cache.SetValueExt<Customer.retainagePct>(targetCurrentCustomer, sourceCurrentCustomer.RetainagePct);
                target.CurrentCustomer.Cache.SetValueExt<Customer.creditRule>(targetCurrentCustomer, sourceCurrentCustomer.CreditRule);
                target.CurrentCustomer.Cache.SetValueExt<Customer.creditLimit>(targetCurrentCustomer, sourceCurrentCustomer.CreditLimit);
                target.CurrentCustomer.Cache.SetValueExt<Customer.creditDaysPastDue>(targetCurrentCustomer, sourceCurrentCustomer.CreditDaysPastDue);
                target.CurrentCustomer.Cache.SetValueExt<Customer.overrideBillAddress>(targetCurrentCustomer, sourceCurrentCustomer.OverrideBillAddress);
                target.CurrentCustomer.Cache.SetValueExt<Customer.overrideBillContact>(targetCurrentCustomer, sourceCurrentCustomer.OverrideBillContact);
                target.CurrentCustomer.Cache.SetValueExt<Customer.parentBAccountID>(targetCurrentCustomer, targetParentBAccountID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.sharedCreditPolicy>(targetCurrentCustomer, sourceCurrentCustomer.SharedCreditPolicy);
                try
                {
                    target.CurrentCustomer.Cache.SetValueExt<Customer.consolidateToParent>(targetCurrentCustomer, sourceCurrentCustomer.ConsolidateToParent);
                }
                catch
                {
                }
                try
                {
                    target.CurrentCustomer.Cache.SetValueExt<Customer.consolidateStatements>(targetCurrentCustomer, sourceCurrentCustomer.ConsolidateStatements);
                }
                catch
                {
                }
                target.CurrentCustomer.Cache.SetValueExt<Customer.mailInvoices>(targetCurrentCustomer, sourceCurrentCustomer.MailInvoices);
                target.CurrentCustomer.Cache.SetValueExt<Customer.mailDunningLetters>(targetCurrentCustomer, sourceCurrentCustomer.MailDunningLetters);
                target.CurrentCustomer.Cache.SetValueExt<Customer.sendStatementByEmail>(targetCurrentCustomer, sourceCurrentCustomer.SendStatementByEmail);
                target.CurrentCustomer.Cache.SetValueExt<Customer.printInvoices>(targetCurrentCustomer, sourceCurrentCustomer.PrintInvoices);
                target.CurrentCustomer.Cache.SetValueExt<Customer.printDunningLetters>(targetCurrentCustomer, sourceCurrentCustomer.PrintDunningLetters);
                target.CurrentCustomer.Cache.SetValueExt<Customer.printStatements>(targetCurrentCustomer, sourceCurrentCustomer.PrintStatements);
                target.CurrentCustomer.Cache.SetValueExt<Customer.statementType>(targetCurrentCustomer, sourceCurrentCustomer.StatementType);
                target.CurrentCustomer.Cache.SetValueExt<Customer.printCuryStatements>(targetCurrentCustomer, sourceCurrentCustomer.PrintCuryStatements);
                target.CurrentCustomer.Cache.SetValueExt<Customer.defPaymentMethodID>(targetCurrentCustomer, sourceCurrentCustomer.DefPaymentMethodID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.suggestRelatedItems>(targetCurrentCustomer, sourceCurrentCustomer.SuggestRelatedItems);
                target.CurrentCustomer.Cache.SetValueExt<Customer.discTakenAcctID>(targetCurrentCustomer, targetDiscTakenAcctID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.discTakenSubID>(targetCurrentCustomer, targetDiscTakenSubID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.prepaymentAcctID>(targetCurrentCustomer, targetPrepaymentAcctID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.prepaymentSubID>(targetCurrentCustomer, targetPrepaymentSubID);
                target.CurrentCustomer.Cache.SetValueExt<Customer.cOrgBAccountID>(targetCurrentCustomer, targetCOrgBAccountID);
                target.CurrentCustomer.Cache.Update(targetCurrentCustomer);
            }
            Address targetBillAddress = target.BillAddress.SelectSingle(Array.Empty<object>());
            Contact targetBillContact = target.BillContact.SelectSingle(Array.Empty<object>());
            if ((targetDefAddress == null ? false : sourceDefAddress != null))
            {
                targetDefContactAddressExt.DefAddress.Cache.SetValueExt<Address.addressLine1>(targetDefAddress, sourceDefAddress.AddressLine1);
                targetDefContactAddressExt.DefAddress.Cache.SetValueExt<Address.addressLine2>(targetDefAddress, sourceDefAddress.AddressLine2);
                targetDefContactAddressExt.DefAddress.Cache.SetValueExt<Address.city>(targetDefAddress, sourceDefAddress.City);
                targetDefContactAddressExt.DefAddress.Cache.SetValueExt<Address.state>(targetDefAddress, sourceDefAddress.State);
                targetDefContactAddressExt.DefAddress.Cache.SetValueExt<Address.postalCode>(targetDefAddress, sourceDefAddress.PostalCode);
                targetDefContactAddressExt.DefAddress.Cache.SetValueExt<Address.countryID>(targetDefAddress, sourceDefAddress.CountryID);
                targetDefContactAddressExt.DefAddress.Cache.Update(targetDefAddress);
            }
            if ((targetDefContact == null ? false : sourceDefContact != null))
            {
                if (sourceDefContact.FirstName != null)
                {
                    targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.firstName>(targetDefContact, sourceDefContact.FirstName);
                }
                if (sourceDefContact.LastName != null)
                {
                    targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.lastName>(targetDefContact, sourceDefContact.LastName);
                }
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.phone1Type>(targetDefContact, sourceDefContact.Phone1Type);
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.phone2Type>(targetDefContact, sourceDefContact.Phone2Type);
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.faxType>(targetDefContact, sourceDefContact.FaxType);
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.phone1>(targetDefContact, sourceDefContact.Phone1);
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.phone2>(targetDefContact, sourceDefContact.Phone2);
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.fax>(targetDefContact, sourceDefContact.Fax);
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.eMail>(targetDefContact, sourceDefContact.EMail);
                targetDefContactAddressExt.DefContact.Cache.SetValueExt<Contact.webSite>(targetDefContact, sourceDefContact.WebSite);
                try
                {
                    targetDefContactAddressExt.DefContact.Cache.Update(targetDefContact);
                }
                catch (Exception exception1)
                {
                    Exception e = exception1;
                    if ((!e.Message.ToLower().Contains("last name") ? true : !e.Message.ToLower().Contains("cannot be empty")))
                    {
                        throw e;
                    }
                }
            }
            if ((targetPrimaryContact == null || sourcePrimaryContact == null ? false : sourcePrimaryContact.MemberName != null))
            {
                if (sourcePrimaryContact.FirstName != null)
                {
                    targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.firstName>(targetPrimaryContact, sourcePrimaryContact.FirstName);
                }
                if (sourcePrimaryContact.LastName != null)
                {
                    targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.lastName>(targetPrimaryContact, sourcePrimaryContact.LastName);
                }
                targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.salutation>(targetPrimaryContact, sourcePrimaryContact.Salutation);
                targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.eMail>(targetPrimaryContact, sourcePrimaryContact.EMail);
                targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.phone1Type>(targetPrimaryContact, sourcePrimaryContact.Phone1Type);
                targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.phone2Type>(targetPrimaryContact, sourcePrimaryContact.Phone2Type);
                targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.phone1>(targetPrimaryContact, sourcePrimaryContact.Phone1);
                targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.SetValueExt<Contact.phone2>(targetPrimaryContact, sourcePrimaryContact.Phone2);
                try
                {
                    targetPrimaryContactGraphExt.PrimaryContactCurrent.Cache.Update(targetPrimaryContact);
                }
                catch (Exception exception2)
                {
                    Exception e = exception2;
                    if ((!e.Message.ToLower().Contains("last name") ? true : !e.Message.ToLower().Contains("cannot be empty")))
                    {
                        throw e;
                    }
                }
            }
            if ((targetCustomerBalanceSummary == null ? false : sourceCustomerBalanceSummary != null))
            {
                target.CustomerBalance.Cache.SetValueExt<CustomerMaint.CustomerBalanceSummary.balance>(targetCustomerBalanceSummary, sourceCustomerBalanceSummary.Balance);
                target.CustomerBalance.Cache.SetValueExt<CustomerMaint.CustomerBalanceSummary.signedDepositsBalance>(targetCustomerBalanceSummary, sourceCustomerBalanceSummary.SignedDepositsBalance);
                target.CustomerBalance.Cache.SetValueExt<CustomerMaint.CustomerBalanceSummary.retainageBalance>(targetCustomerBalanceSummary, sourceCustomerBalanceSummary.RetainageBalance);
                target.CustomerBalance.Cache.SetValueExt<CustomerMaint.CustomerBalanceSummary.unreleasedBalance>(targetCustomerBalanceSummary, sourceCustomerBalanceSummary.UnreleasedBalance);
                target.CustomerBalance.Cache.SetValueExt<CustomerMaint.CustomerBalanceSummary.openOrdersBalance>(targetCustomerBalanceSummary, sourceCustomerBalanceSummary.OpenOrdersBalance);
                target.CustomerBalance.Cache.SetValueExt<CustomerMaint.CustomerBalanceSummary.remainingCreditLimit>(targetCustomerBalanceSummary, sourceCustomerBalanceSummary.RemainingCreditLimit);
                target.CustomerBalance.Cache.SetValueExt<CustomerMaint.CustomerBalanceSummary.oldInvoiceDate>(targetCustomerBalanceSummary, sourceCustomerBalanceSummary.OldInvoiceDate);
                target.CustomerBalance.Cache.Update(targetCustomerBalanceSummary);
            }
            if ((targetBillAddress == null ? false : sourceBillAddress != null))
            {
                target.BillAddress.Cache.SetValueExt<Address.addressLine1>(targetBillAddress, sourceBillAddress.AddressLine1);
                target.BillAddress.Cache.SetValueExt<Address.addressLine2>(targetBillAddress, sourceBillAddress.AddressLine2);
                target.BillAddress.Cache.SetValueExt<Address.city>(targetBillAddress, sourceBillAddress.City);
                target.BillAddress.Cache.SetValueExt<Address.state>(targetBillAddress, sourceBillAddress.State);
                target.BillAddress.Cache.SetValueExt<Address.postalCode>(targetBillAddress, sourceBillAddress.PostalCode);
                target.BillAddress.Cache.SetValueExt<Address.countryID>(targetBillAddress, sourceBillAddress.CountryID);
                target.BillAddress.Cache.Update(targetBillAddress);
            }
            if (targetBillContact == null || sourceBillContact == null)
            {
                valueOrDefault = false;
            }
            else
            {
                primaryContactID = targetBillContact.ContactID;
                contactID = targetDefContact.ContactID;
                valueOrDefault = !(primaryContactID.GetValueOrDefault() == contactID.GetValueOrDefault() & primaryContactID.HasValue == contactID.HasValue);
            }
            if (valueOrDefault)
            {
                if (targetBillContact.FirstName != null)
                {
                    target.BillContact.Cache.SetValueExt<Contact.firstName>(targetBillContact, sourceBillContact.FirstName);
                }
                if (targetBillContact.LastName != null)
                {
                    target.BillContact.Cache.SetValueExt<Contact.lastName>(targetBillContact, sourceBillContact.LastName);
                }
                target.BillContact.Cache.SetValueExt<Contact.fullName>(targetBillContact, sourceBillContact.FullName);
                target.BillContact.Cache.SetValueExt<Contact.attention>(targetBillContact, sourceBillContact.Attention);
                target.BillContact.Cache.SetValueExt<Contact.phone1Type>(targetBillContact, sourceBillContact.Phone1Type);
                target.BillContact.Cache.SetValueExt<Contact.phone2Type>(targetBillContact, sourceBillContact.Phone2Type);
                target.BillContact.Cache.SetValueExt<Contact.faxType>(targetBillContact, sourceBillContact.FaxType);
                target.BillContact.Cache.SetValueExt<Contact.phone1>(targetBillContact, sourceBillContact.Phone1);
                target.BillContact.Cache.SetValueExt<Contact.phone2>(targetBillContact, sourceBillContact.Phone2);
                target.BillContact.Cache.SetValueExt<Contact.fax>(targetBillContact, sourceBillContact.Fax);
                target.BillContact.Cache.SetValueExt<Contact.eMail>(targetBillContact, sourceBillContact.EMail);
                target.BillContact.Cache.SetValueExt<Contact.webSite>(targetBillContact, sourceBillContact.WebSite);
                try
                {
                    target.BillContact.Cache.Update(targetBillContact);
                }
                catch (Exception exception3)
                {
                    Exception e = exception3;
                    if ((!e.Message.ToLower().Contains("last name") ? true : !e.Message.ToLower().Contains("cannot be empty")))
                    {
                        throw e;
                    }
                }
            }
            if ((targetDefLocation == null ? false : sourceDefLocation != null))
            {
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.overrideAddress>(targetDefLocation, sourceDefLocation.OverrideAddress);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.overrideContact>(targetDefLocation, sourceDefLocation.OverrideContact);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cBranchID>(targetDefLocation, targetCBranchID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cPriceClassID>(targetDefLocation, sourceDefLocation.CPriceClassID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cDefProjectID>(targetDefLocation, targetCDefProjectID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.taxRegistrationID>(targetDefLocation, sourceDefLocation.TaxRegistrationID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cTaxZoneID>(targetDefLocation, sourceDefLocation.CTaxZoneID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cAvalaraExemptionNumber>(targetDefLocation, sourceDefLocation.CAvalaraExemptionNumber);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cAvalaraCustomerUsageType>(targetDefLocation, sourceDefLocation.CAvalaraCustomerUsageType);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cSiteID>(targetDefLocation, targetCSiteID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cCarrierID>(targetDefLocation, sourceDefLocation.CCarrierID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cShipTermsID>(targetDefLocation, sourceDefLocation.CShipTermsID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cShipZoneID>(targetDefLocation, sourceDefLocation.CShipZoneID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cFOBPointID>(targetDefLocation, sourceDefLocation.CFOBPointID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cResedential>(targetDefLocation, sourceDefLocation.CResedential);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cSaturdayDelivery>(targetDefLocation, sourceDefLocation.CSaturdayDelivery);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cInsurance>(targetDefLocation, sourceDefLocation.CInsurance);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cShipComplete>(targetDefLocation, sourceDefLocation.CShipComplete);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cOrderPriority>(targetDefLocation, sourceDefLocation.COrderPriority);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cLeadTime>(targetDefLocation, sourceDefLocation.CLeadTime);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cCalendarID>(targetDefLocation, sourceDefLocation.CCalendarID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cARAccountID>(targetDefLocation, targetCARAccountID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cARSubID>(targetDefLocation, targetCARSubID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cSalesAcctID>(targetDefLocation, targetCSalesAcctID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cSalesSubID>(targetDefLocation, targetCSalesSubID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cDiscountAcctID>(targetDefLocation, targetCDiscountAcctID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cDiscountSubID>(targetDefLocation, targetCDiscountSubID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cFreightAcctID>(targetDefLocation, targetCFreightAcctID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cFreightSubID>(targetDefLocation, targetCFreightSubID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cRetainageAcctID>(targetDefLocation, targetCRetainageAcctID);
                targetDefLocationExt.DefLocation.Cache.SetValueExt<PX.Objects.CR.Standalone.Location.cRetainageSubID>(targetDefLocation, targetCRetainageSubID);
                targetDefLocationExt.DefLocation.Cache.Update(targetDefLocation);
            }
            Address targetDefLocationAddress = targetDefLocationExt.DefLocationAddress.SelectSingle(Array.Empty<object>());
            Contact targetDefLocationContact = targetDefLocationExt.DefLocationContact.SelectSingle(Array.Empty<object>());
            if ((targetDefLocationAddress == null ? false : sourceDefLocationAddress != null))
            {
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.addressLine1>(targetDefLocationAddress, sourceDefLocationAddress.AddressLine1);
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.addressLine2>(targetDefLocationAddress, sourceDefLocationAddress.AddressLine2);
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.city>(targetDefLocationAddress, sourceDefLocationAddress.City);
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.state>(targetDefLocationAddress, sourceDefLocationAddress.State);
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.postalCode>(targetDefLocationAddress, sourceDefLocationAddress.PostalCode);
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.countryID>(targetDefLocationAddress, sourceDefLocationAddress.CountryID);
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.latitude>(targetDefLocationAddress, sourceDefLocationAddress.Latitude);
                targetDefLocationExt.DefLocationAddress.Cache.SetValueExt<Address.longitude>(targetDefLocationAddress, sourceDefLocationAddress.Longitude);
                targetDefLocationExt.DefLocationAddress.Cache.Update(targetDefLocationAddress);
            }
            if ((targetDefLocationContact == null ? false : sourceDefLocationContact != null))
            {
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.fullName>(targetDefLocationContact, sourceDefLocationContact.FullName);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.attention>(targetDefLocationContact, sourceDefLocationContact.Attention);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.phone1Type>(targetDefLocationContact, sourceDefLocationContact.Phone1Type);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.phone2Type>(targetDefLocationContact, sourceDefLocationContact.Phone2Type);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.faxType>(targetDefLocationContact, sourceDefLocationContact.FaxType);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.phone1>(targetDefLocationContact, sourceDefLocationContact.Phone1);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.phone2>(targetDefLocationContact, sourceDefLocationContact.Phone2);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.fax>(targetDefLocationContact, sourceDefLocationContact.Fax);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.eMail>(targetDefLocationContact, sourceDefLocationContact.EMail);
                targetDefLocationExt.DefLocationContact.Cache.SetValueExt<Contact.webSite>(targetDefLocationContact, sourceDefLocationContact.WebSite);
                targetDefLocationExt.DefLocationContact.Cache.Update(targetDefLocationContact);
            }
            CustomerPaymentMethod targetDefCustomerPaymentMethodInstance = targetPaymentDetailsExt.DefPaymentMethodInstance.SelectSingle(Array.Empty<object>());
            if ((targetDefCustomerPaymentMethodInstance == null ? false : sourceDefCustomerPaymentMethodInstance != null))
            {
                targetPaymentDetailsExt.DefPaymentMethodInstance.Cache.SetValueExt<CustomerPaymentMethod.cashAccountID>(targetDefCustomerPaymentMethodInstance, targetCashAccountID);
                targetPaymentDetailsExt.DefPaymentMethodInstance.Cache.SetValueExt<CustomerPaymentMethod.descr>(targetDefCustomerPaymentMethodInstance, sourceDefCustomerPaymentMethodInstance.Descr);
                targetPaymentDetailsExt.DefPaymentMethodInstance.Cache.Update(targetDefCustomerPaymentMethodInstance);
            }
            List<CarrierPluginCustomer> list = GraphHelper.RowCast<CarrierPluginCustomer>(target.Carriers.Select(Array.Empty<object>())).ToList<CarrierPluginCustomer>();
            List<CarrierPluginCustomer> sourceDeletedCarriers = (
              from notModified in this.SourceNotModifiedCarrierAccounts
              where !this.SourceModifiedCarrierAccounts.Any<CarrierPluginCustomer>((CarrierPluginCustomer modified) => {
                  int? recordID = notModified.RecordID;
                  int? nullable = modified.RecordID;
                  return recordID.GetValueOrDefault() == nullable.GetValueOrDefault() & recordID.HasValue == nullable.HasValue;
              })
              select notModified).ToList<CarrierPluginCustomer>();
            foreach (CarrierPluginCustomer carrierPluginCustomer in sourceDeletedCarriers)
            {
                CarrierPluginCustomer carrierToDelete = list.FirstOrDefault<CarrierPluginCustomer>((CarrierPluginCustomer x) => x.CarrierPluginID == carrierPluginCustomer.CarrierPluginID);
                if (carrierToDelete != null)
                {
                    target.Carriers.Delete(carrierToDelete);
                    list.Remove(carrierToDelete);
                }
            }
            int?[] correspondingIDsFromSource = new int?[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                CarrierPluginCustomer fromSource = this.SourceNotModifiedCarrierAccounts.FirstOrDefault<CarrierPluginCustomer>((CarrierPluginCustomer r) => r.CarrierPluginID == list[i].CarrierPluginID);
                if (fromSource != null)
                {
                    nullable5 = fromSource.RecordID;
                }
                else
                {
                    contactID = null;
                    nullable5 = contactID;
                }
                int? nullable6 = nullable5;
                if (nullable6.HasValue)
                {
                    CarrierPluginCustomer sourceModifiedCarrier = this.SourceModifiedCarrierAccounts.FirstOrDefault<CarrierPluginCustomer>((CarrierPluginCustomer r) => {
                        int? recordID = r.RecordID;
                        int? nullable = nullable6;
                        return recordID.GetValueOrDefault() == nullable.GetValueOrDefault() & recordID.HasValue == nullable.HasValue;
                    });
                    PXDatabase.Update<CarrierPluginCustomer>(new PXDataFieldParam[] { new PXDataFieldRestrict<CarrierPluginCustomer.recordID>((object)list[i].RecordID), new PXDataFieldAssign<CarrierPluginCustomer.isActive>((object)sourceModifiedCarrier.IsActive), new PXDataFieldAssign<CarrierPluginCustomer.carrierPluginID>(sourceModifiedCarrier.CarrierPluginID), new PXDataFieldAssign<CarrierPluginCustomer.carrierAccount>(sourceModifiedCarrier.CarrierAccount), new PXDataFieldAssign<CarrierPluginCustomer.postalCode>(sourceModifiedCarrier.PostalCode), new PXDataFieldAssign<CarrierPluginCustomer.lastModifiedDateTime>((object)sourceModifiedCarrier.LastModifiedDateTime) });
                }
            }
            List<CarrierPluginCustomer> sourceNewCarriers = (
              from modified in this.SourceModifiedCarrierAccounts
              where !this.SourceNotModifiedCarrierAccounts.Any<CarrierPluginCustomer>((CarrierPluginCustomer notModified) => {
                  int? recordID = notModified.RecordID;
                  int? nullable = modified.RecordID;
                  return recordID.GetValueOrDefault() == nullable.GetValueOrDefault() & recordID.HasValue == nullable.HasValue;
              })
              select modified).ToList<CarrierPluginCustomer>();
            foreach (CarrierPluginCustomer sourceNewCarrier in sourceNewCarriers)
            {
                CarrierPluginCustomer targetNewCarrier = target.Carriers.Insert();
                target.Carriers.Cache.SetValueExt<CarrierPluginCustomer.isActive>(targetNewCarrier, sourceNewCarrier.IsActive);
                target.Carriers.Cache.SetValueExt<CarrierPluginCustomer.carrierPluginID>(targetNewCarrier, sourceNewCarrier.CarrierPluginID);
                target.Carriers.Cache.SetValueExt<CarrierPluginCustomer.carrierAccount>(targetNewCarrier, sourceNewCarrier.CarrierAccount);
                target.Carriers.Cache.SetValueExt<CarrierPluginCustomer.postalCode>(targetNewCarrier, sourceNewCarrier.PostalCode);
                target.Carriers.Cache.Update(targetNewCarrier);
            }
            try
            {
                target.Actions.PressSave();
            }
            catch (Exception exception4)
            {
                Exception e = exception4;
                if ((!e.Message.ToLower().Contains("last name") ? true : !e.Message.ToLower().Contains("cannot be empty")))
                {
                    throw e;
                }
            }
        }
    }
}