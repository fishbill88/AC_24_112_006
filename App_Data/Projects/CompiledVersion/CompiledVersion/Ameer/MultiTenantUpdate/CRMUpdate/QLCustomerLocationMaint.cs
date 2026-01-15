using PX.Data;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.AR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLTenantCopyItems
{
    public sealed class QLCustomerLocationMaintExt : PXGraphExtension<CustomerLocationMaint>
    {
        public static bool IsActive() => true;

        public List<string> otherTenantNames;

        public bool IsExtEnabled { get; set; } = true;

        protected void _(Events.RowPersisted<Location> e)
        {
            int? bAccountID;
            string acctCD;
            if (this.IsExtEnabled)
            {
                Location updatedLocation = e.Row;
                if ((updatedLocation == null ? false : e.TranStatus == PXTranStatus.Open))
                {
                    string callerTenantName = base.Base.Accessinfo.CompanyName;
                    string callerUserName = base.Base.Accessinfo.UserName;
                    string callerLoginScopeParam = string.Concat(callerUserName, "@", callerTenantName);
                    PXGraph sourceGraphForPXSelect = PXGraph.CreateInstance<PXGraph>();
                    string updatedLocationCD = updatedLocation.LocationCD;
                    PXGraph pXGraph = sourceGraphForPXSelect;
                    object[] objArray = new object[1];
                    if (updatedLocation != null)
                    {
                        bAccountID = updatedLocation.BAccountID;
                    }
                    else
                    {
                        bAccountID = null;
                    }
                    objArray[0] = bAccountID;
                    BAccount bAccount = GraphHelper.RowCast<BAccount>(PXSelectBase<BAccount, PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>.Config>.Select(pXGraph, objArray)).FirstOrDefault<BAccount>();
                    if (bAccount != null)
                    {
                        acctCD = bAccount.AcctCD;
                    }
                    else
                    {
                        acctCD = null;
                    }
                    string updatedLocationBAcctCD = acctCD;
                    foreach (string s in this.otherTenantNames)
                    {
                        string userLogin = string.Concat("admin@", s);
                        using (PXLoginScope ls = new PXLoginScope(userLogin, Array.Empty<string>()))
                        {
                            PXGraph targetGraphForPXSelect = PXGraph.CreateInstance<PXGraph>();
                            if (PXSelectBase<Location, PXSelectJoin<Location, InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Location.bAccountID>>>, Where<Location.locationCD, Equal<Required<Location.locationCD>>, And<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>>.Config>.Select(targetGraphForPXSelect, new object[] { updatedLocationCD, updatedLocationBAcctCD }).FirstOrDefault<PXResult<Location>>() == null)
                            {
                                CustomerLocationMaint targetMaint = this.GetNewCustomerLocationGraph(updatedLocationBAcctCD, updatedLocationCD, targetGraphForPXSelect);
                                this.UpdateCustomerLocationWithGraph(targetMaint, base.Base, sourceGraphForPXSelect, callerLoginScopeParam);
                            }
                            else
                            {
                                CustomerLocationMaint targetMaint = this.GetCustomerLocationGraph(updatedLocationBAcctCD, updatedLocationCD);
                                if (e.Operation != PXDBOperation.Delete)
                                {
                                    this.UpdateCustomerLocationWithGraph(targetMaint, base.Base, sourceGraphForPXSelect, callerLoginScopeParam);
                                }
                                else
                                {
                                    targetMaint.Location.DeleteCurrent();
                                    targetMaint.Actions.PressSave();
                                }
                            }
                        }
                    }
                }
            }
        }

        public CustomerLocationMaint GetCustomerLocationGraph(string locationBAcctCD, string locationCD)
        {
            CustomerLocationMaint graph = PXGraph.CreateInstance<CustomerLocationMaint>();
            graph.GetExtension<QLCustomerLocationMaintExt>().IsExtEnabled = false;
            PXResult<Location> location = PXSelectBase<Location, PXSelectJoin<Location, InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Location.bAccountID>>>, Where<Location.locationCD, Equal<Required<Location.locationCD>>, And<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>>.Config>.Select(graph, new object[] { locationCD, locationBAcctCD }).FirstOrDefault<PXResult<Location>>();
            if (location != null)
            {
                graph.Location.Current = location.GetItem<Location>();
            }
            return graph;
        }

        public CustomerLocationMaint GetNewCustomerLocationGraph(string locationBAcctCD, string locationCD, PXGraph targetGraphForPXSelect)
        {
            int? nullable;
            CustomerLocationMaint graph = PXGraph.CreateInstance<CustomerLocationMaint>();
            graph.GetExtension<QLCustomerLocationMaintExt>().IsExtEnabled = false;
            BAccount bAccount = GraphHelper.RowCast<BAccount>(PXSelectBase<BAccount, PXSelect<BAccount, Where<BAccount.acctCD, Equal<Required<BAccount.acctCD>>>>.Config>.Select(targetGraphForPXSelect, new object[] { locationBAcctCD })).FirstOrDefault<BAccount>();
            if (bAccount != null)
            {
                nullable = bAccount.BAccountID;
            }
            else
            {
                nullable = null;
            }
            int? bAccountID = nullable;
            if (!bAccountID.HasValue)
            {
                throw new Exception(string.Concat(new string[] { "'", locationBAcctCD, "' customer is not found on ", base.Base.Accessinfo.CompanyName, " tenant" }));
            }
            Location newLocation = (Location)graph.Location.Cache.CreateInstance();
            newLocation.BAccountID = bAccountID;
            newLocation.LocationCD = locationCD;
            newLocation.LocType = "CU";
            graph.Location.Insert(newLocation);
            graph.Location.Cache.IsDirty = false;
            return graph;
        }

        public override void Initialize()
        {
            using (CustomSqlConnection sqlConnection = new CustomSqlConnection(PXDatabase.Provider.GetConnectionString()))
            {
                this.otherTenantNames = sqlConnection.GetOtherCompanyNames(base.Base.Accessinfo.CompanyName);
            }
        }

        public void UpdateCustomerLocationWithGraph(CustomerLocationMaint target, CustomerLocationMaint source, PXGraph sourcePXSelectGraph, string sourceLoginScopeParam)
        {
            int? nullable;
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
            object obj;
            int? accountID1;
            object subCD1;
            int? subID1;
            object accountCD1;
            int? nullable1;
            object obj1;
            int? subID2;
            object accountCD2;
            int? accountID2;
            object subCD2;
            int? nullable2;
            object obj2;
            int? accountID3;
            object subCD3;
            int? subID3;
            Location sourceLocationCurrent = null;
            Location sourceLocation = null;
            Address sourceAddress = null;
            Contact sourceContact = null;
            LocationARAccountSub sourceARAccountSubLocation = null;
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
            Account sourceCRetainageAcct = null;
            Sub sourceCRetainageSub = null;
            using (PXLoginScope pXLoginScope = new PXLoginScope(sourceLoginScopeParam, Array.Empty<string>()))
            {
                sourceLocationCurrent = source.LocationCurrent.SelectSingle(Array.Empty<object>());
                sourceLocation = source.Location.Current;
                sourceAddress = source.Address.SelectSingle(Array.Empty<object>());
                sourceContact = source.Contact.Current;
                sourceARAccountSubLocation = source.ARAccountSubLocation.Current;
                if (sourceLocationCurrent != null)
                {
                    sourceCBranch = PXSelectBase<Branch, PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CBranchID });
                    sourceCDefProject = PXSelectBase<PMProject, PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CDefProjectID });
                    sourceCSite = PXSelectBase<INSite, PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CSiteID });
                    sourceCSalesAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CSalesAcctID });
                    sourceCSalesSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CSalesSubID });
                    sourceCDiscountAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CDiscountAcctID });
                    sourceCDiscountSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CDiscountSubID });
                    sourceCFreightAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CFreightAcctID });
                    sourceCFreightSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceLocationCurrent.CFreightSubID });
                }
                if (sourceARAccountSubLocation != null)
                {
                    sourceCARAccount = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceARAccountSubLocation.CARAccountID });
                    sourceCARSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceARAccountSubLocation.CARSubID });
                    sourceCRetainageAcct = PXSelectBase<Account, PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceARAccountSubLocation.CRetainageAcctID });
                    sourceCRetainageSub = PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subID, Equal<Required<Sub.subID>>>>.Config>.Select(sourcePXSelectGraph, new object[] { sourceARAccountSubLocation.CRetainageSubID });
                }
            }
            Location targetLocationCurrent = target.LocationCurrent.SelectSingle(Array.Empty<object>());
            Location targetLocation = target.Location.Current;
            CustomerLocationMaint customerLocationMaint = target;
            object[] objArray = new object[1];
            if (sourceCBranch != null)
            {
                branchCD = sourceCBranch.BranchCD;
            }
            else
            {
                branchCD = null;
            }
            objArray[0] = branchCD;
            Branch branch = GraphHelper.RowCast<Branch>(PXSelectBase<Branch, PXSelect<Branch, Where<Branch.branchCD, Equal<Required<Branch.branchCD>>>>.Config>.Select(customerLocationMaint, objArray)).FirstOrDefault<Branch>();
            if (branch != null)
            {
                branchID = branch.BranchID;
            }
            else
            {
                nullable = null;
                branchID = nullable;
            }
            int? targetCBranchID = branchID;
            CustomerLocationMaint customerLocationMaint1 = target;
            object[] objArray1 = new object[1];
            if (sourceCDefProject != null)
            {
                contractCD = sourceCDefProject.ContractCD;
            }
            else
            {
                contractCD = null;
            }
            objArray1[0] = contractCD;
            PMProject pMProject = GraphHelper.RowCast<PMProject>(PXSelectBase<PMProject, PXSelect<PMProject, Where<PMProject.contractCD, Equal<Required<PMProject.contractCD>>>>.Config>.Select(customerLocationMaint1, objArray1)).FirstOrDefault<PMProject>();
            if (pMProject != null)
            {
                contractID = pMProject.ContractID;
            }
            else
            {
                nullable = null;
                contractID = nullable;
            }
            int? targetCDefProjectID = contractID;
            CustomerLocationMaint customerLocationMaint2 = target;
            object[] objArray2 = new object[1];
            if (sourceCSite != null)
            {
                siteCD = sourceCSite.SiteCD;
            }
            else
            {
                siteCD = null;
            }
            objArray2[0] = siteCD;
            INSite nSite = GraphHelper.RowCast<INSite>(PXSelectBase<INSite, PXSelect<INSite, Where<INSite.siteCD, Equal<Required<INSite.siteCD>>>>.Config>.Select(customerLocationMaint2, objArray2)).FirstOrDefault<INSite>();
            if (nSite != null)
            {
                siteID = nSite.SiteID;
            }
            else
            {
                nullable = null;
                siteID = nullable;
            }
            int? targetCSiteID = siteID;
            CustomerLocationMaint customerLocationMaint3 = target;
            object[] objArray3 = new object[1];
            if (sourceCARAccount != null)
            {
                accountCD = sourceCARAccount.AccountCD;
            }
            else
            {
                accountCD = null;
            }
            objArray3[0] = accountCD;
            Account account = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerLocationMaint3, objArray3)).FirstOrDefault<Account>();
            if (account != null)
            {
                accountID = account.AccountID;
            }
            else
            {
                nullable = null;
                accountID = nullable;
            }
            int? targetCARAccountID = accountID;
            CustomerLocationMaint customerLocationMaint4 = target;
            object[] objArray4 = new object[1];
            if (sourceCARSub != null)
            {
                subCD = sourceCARSub.SubCD;
            }
            else
            {
                subCD = null;
            }
            objArray4[0] = subCD;
            Sub sub = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerLocationMaint4, objArray4)).FirstOrDefault<Sub>();
            if (sub != null)
            {
                subID = sub.SubID;
            }
            else
            {
                nullable = null;
                subID = nullable;
            }
            int? targetCARSubID = subID;
            CustomerLocationMaint customerLocationMaint5 = target;
            object[] objArray5 = new object[1];
            if (sourceCSalesAcct != null)
            {
                obj = sourceCSalesAcct.AccountCD;
            }
            else
            {
                obj = null;
            }
            objArray5[0] = obj;
            Account account1 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerLocationMaint5, objArray5)).FirstOrDefault<Account>();
            if (account1 != null)
            {
                accountID1 = account1.AccountID;
            }
            else
            {
                nullable = null;
                accountID1 = nullable;
            }
            int? targetCSalesAcctID = accountID1;
            CustomerLocationMaint customerLocationMaint6 = target;
            object[] objArray6 = new object[1];
            if (sourceCSalesSub != null)
            {
                subCD1 = sourceCSalesSub.SubCD;
            }
            else
            {
                subCD1 = null;
            }
            objArray6[0] = subCD1;
            Sub sub1 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerLocationMaint6, objArray6)).FirstOrDefault<Sub>();
            if (sub1 != null)
            {
                subID1 = sub1.SubID;
            }
            else
            {
                nullable = null;
                subID1 = nullable;
            }
            int? targetCSalesSubID = subID1;
            CustomerLocationMaint customerLocationMaint7 = target;
            object[] objArray7 = new object[1];
            if (sourceCDiscountAcct != null)
            {
                accountCD1 = sourceCDiscountAcct.AccountCD;
            }
            else
            {
                accountCD1 = null;
            }
            objArray7[0] = accountCD1;
            Account account2 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerLocationMaint7, objArray7)).FirstOrDefault<Account>();
            if (account2 != null)
            {
                nullable1 = account2.AccountID;
            }
            else
            {
                nullable = null;
                nullable1 = nullable;
            }
            int? targetCDiscountAcctID = nullable1;
            CustomerLocationMaint customerLocationMaint8 = target;
            object[] objArray8 = new object[1];
            if (sourceCDiscountSub != null)
            {
                obj1 = sourceCDiscountSub.SubCD;
            }
            else
            {
                obj1 = null;
            }
            objArray8[0] = obj1;
            Sub sub2 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerLocationMaint8, objArray8)).FirstOrDefault<Sub>();
            if (sub2 != null)
            {
                subID2 = sub2.SubID;
            }
            else
            {
                nullable = null;
                subID2 = nullable;
            }
            int? targetCDiscountSubID = subID2;
            CustomerLocationMaint customerLocationMaint9 = target;
            object[] objArray9 = new object[1];
            if (sourceCFreightAcct != null)
            {
                accountCD2 = sourceCFreightAcct.AccountCD;
            }
            else
            {
                accountCD2 = null;
            }
            objArray9[0] = accountCD2;
            Account account3 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerLocationMaint9, objArray9)).FirstOrDefault<Account>();
            if (account3 != null)
            {
                accountID2 = account3.AccountID;
            }
            else
            {
                nullable = null;
                accountID2 = nullable;
            }
            int? targetCFreightAcctID = accountID2;
            CustomerLocationMaint customerLocationMaint10 = target;
            object[] objArray10 = new object[1];
            if (sourceCFreightSub != null)
            {
                subCD2 = sourceCFreightSub.SubCD;
            }
            else
            {
                subCD2 = null;
            }
            objArray10[0] = subCD2;
            Sub sub3 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerLocationMaint10, objArray10)).FirstOrDefault<Sub>();
            if (sub3 != null)
            {
                nullable2 = sub3.SubID;
            }
            else
            {
                nullable = null;
                nullable2 = nullable;
            }
            int? targetCFreightSubID = nullable2;
            CustomerLocationMaint customerLocationMaint11 = target;
            object[] objArray11 = new object[1];
            if (sourceCRetainageAcct != null)
            {
                obj2 = sourceCRetainageAcct.AccountCD;
            }
            else
            {
                obj2 = null;
            }
            objArray11[0] = obj2;
            Account account4 = GraphHelper.RowCast<Account>(PXSelectBase<Account, PXSelect<Account, Where<Account.accountCD, Equal<Required<Account.accountCD>>>>.Config>.Select(customerLocationMaint11, objArray11)).FirstOrDefault<Account>();
            if (account4 != null)
            {
                accountID3 = account4.AccountID;
            }
            else
            {
                nullable = null;
                accountID3 = nullable;
            }
            int? targetCRetainageAcctID = accountID3;
            CustomerLocationMaint customerLocationMaint12 = target;
            object[] objArray12 = new object[1];
            if (sourceCRetainageSub != null)
            {
                subCD3 = sourceCRetainageSub.SubCD;
            }
            else
            {
                subCD3 = null;
            }
            objArray12[0] = subCD3;
            Sub sub4 = GraphHelper.RowCast<Sub>(PXSelectBase<Sub, PXSelect<Sub, Where<Sub.subCD, Equal<Required<Sub.subCD>>>>.Config>.Select(customerLocationMaint12, objArray12)).FirstOrDefault<Sub>();
            if (sub4 != null)
            {
                subID3 = sub4.SubID;
            }
            else
            {
                nullable = null;
                subID3 = nullable;
            }
            int? targetCRetainageSubID = subID3;
            if (targetLocation != null)
            {
                target.Location.Cache.SetValueExt<Location.status>(targetLocation, sourceLocation.Status);
                target.Location.Cache.Update(targetLocation);
            }
            if (targetLocationCurrent != null)
            {
                target.LocationCurrent.Cache.SetValueExt<Location.descr>(targetLocationCurrent, sourceLocationCurrent.Descr);
                target.LocationCurrent.Cache.SetValueExt<Location.overrideAddress>(targetLocationCurrent, sourceLocationCurrent.OverrideAddress);
                target.LocationCurrent.Cache.SetValueExt<Location.overrideContact>(targetLocationCurrent, sourceLocationCurrent.OverrideContact);
                target.LocationCurrent.Cache.SetValueExt<Location.cBranchID>(targetLocationCurrent, targetCBranchID);
                target.LocationCurrent.Cache.SetValueExt<Location.cPriceClassID>(targetLocationCurrent, sourceLocationCurrent.CPriceClassID);
                target.LocationCurrent.Cache.SetValueExt<Location.cDefProjectID>(targetLocationCurrent, targetCDefProjectID);
                target.LocationCurrent.Cache.SetValueExt<Location.taxRegistrationID>(targetLocationCurrent, sourceLocationCurrent.TaxRegistrationID);
                target.LocationCurrent.Cache.SetValue<Location.cTaxZoneID>(targetLocationCurrent, sourceLocationCurrent.CTaxZoneID);
                target.LocationCurrent.Cache.SetValueExt<Location.cAvalaraExemptionNumber>(targetLocationCurrent, sourceLocationCurrent.CAvalaraExemptionNumber);
                target.LocationCurrent.Cache.SetValueExt<Location.cAvalaraCustomerUsageType>(targetLocationCurrent, sourceLocationCurrent.CAvalaraCustomerUsageType);
                target.LocationCurrent.Cache.SetValueExt<Location.cSiteID>(targetLocationCurrent, targetCSiteID);
                target.LocationCurrent.Cache.SetValueExt<Location.cCarrierID>(targetLocationCurrent, sourceLocationCurrent.CCarrierID);
                target.LocationCurrent.Cache.SetValueExt<Location.cShipTermsID>(targetLocationCurrent, sourceLocationCurrent.CShipTermsID);
                target.LocationCurrent.Cache.SetValueExt<Location.cShipZoneID>(targetLocationCurrent, sourceLocationCurrent.CShipZoneID);
                target.LocationCurrent.Cache.SetValueExt<Location.cFOBPointID>(targetLocationCurrent, sourceLocationCurrent.CFOBPointID);
                target.LocationCurrent.Cache.SetValueExt<Location.cResedential>(targetLocationCurrent, sourceLocationCurrent.CResedential);
                target.LocationCurrent.Cache.SetValueExt<Location.cSaturdayDelivery>(targetLocationCurrent, sourceLocationCurrent.CSaturdayDelivery);
                target.LocationCurrent.Cache.SetValueExt<Location.cInsurance>(targetLocationCurrent, sourceLocationCurrent.CInsurance);
                target.LocationCurrent.Cache.SetValueExt<Location.cShipComplete>(targetLocationCurrent, sourceLocationCurrent.CShipComplete);
                target.LocationCurrent.Cache.SetValueExt<Location.cOrderPriority>(targetLocationCurrent, sourceLocationCurrent.COrderPriority);
                target.LocationCurrent.Cache.SetValueExt<Location.cLeadTime>(targetLocationCurrent, sourceLocationCurrent.CLeadTime);
                target.LocationCurrent.Cache.SetValueExt<Location.cCalendarID>(targetLocationCurrent, sourceLocationCurrent.CCalendarID);
                target.LocationCurrent.Cache.SetValueExt<Location.isARAccountSameAsMain>(targetLocationCurrent, sourceLocationCurrent.IsARAccountSameAsMain);
                target.LocationCurrent.Cache.SetValueExt<Location.cSalesAcctID>(targetLocationCurrent, targetCSalesAcctID);
                target.LocationCurrent.Cache.SetValueExt<Location.cSalesSubID>(targetLocationCurrent, targetCSalesSubID);
                target.LocationCurrent.Cache.SetValueExt<Location.cDiscountAcctID>(targetLocationCurrent, targetCDiscountAcctID);
                target.LocationCurrent.Cache.SetValueExt<Location.cDiscountSubID>(targetLocationCurrent, targetCDiscountSubID);
                target.LocationCurrent.Cache.SetValueExt<Location.cFreightAcctID>(targetLocationCurrent, targetCFreightAcctID);
                target.LocationCurrent.Cache.SetValueExt<Location.cFreightSubID>(targetLocationCurrent, targetCFreightSubID);
                target.LocationCurrent.Cache.Update(targetLocationCurrent);
            }
            Address targetAddress = target.Address.SelectSingle(Array.Empty<object>());
            Contact targetContact = target.Contact.SelectSingle(Array.Empty<object>());
            LocationARAccountSub targetARAccountSubLocation = target.ARAccountSubLocation.SelectSingle(Array.Empty<object>());
            if (targetAddress != null)
            {
                target.Address.Cache.SetValueExt<Address.addressLine1>(targetAddress, sourceAddress.AddressLine1);
                target.Address.Cache.SetValueExt<Address.addressLine2>(targetAddress, sourceAddress.AddressLine2);
                target.Address.Cache.SetValueExt<Address.city>(targetAddress, sourceAddress.City);
                target.Address.Cache.SetValueExt<Address.state>(targetAddress, sourceAddress.State);
                target.Address.Cache.SetValueExt<Address.postalCode>(targetAddress, sourceAddress.PostalCode);
                target.Address.Cache.SetValueExt<Address.countryID>(targetAddress, sourceAddress.CountryID);
                target.Address.Cache.SetValueExt<Address.latitude>(targetAddress, sourceAddress.Latitude);
                target.Address.Cache.SetValueExt<Address.longitude>(targetAddress, sourceAddress.Longitude);
                target.Address.Cache.Update(targetAddress);
            }
            if (targetContact != null)
            {
                target.Contact.Cache.SetValueExt<Contact.fullName>(targetContact, sourceContact.FullName);
                target.Contact.Cache.SetValueExt<Contact.attention>(targetContact, sourceContact.Attention);
                target.Contact.Cache.SetValueExt<Contact.phone1Type>(targetContact, sourceContact.Phone1Type);
                target.Contact.Cache.SetValueExt<Contact.phone2Type>(targetContact, sourceContact.Phone2Type);
                target.Contact.Cache.SetValueExt<Contact.faxType>(targetContact, sourceContact.FaxType);
                target.Contact.Cache.SetValueExt<Contact.phone1>(targetContact, sourceContact.Phone1);
                target.Contact.Cache.SetValueExt<Contact.phone2>(targetContact, sourceContact.Phone2);
                target.Contact.Cache.SetValueExt<Contact.fax>(targetContact, sourceContact.Fax);
                target.Contact.Cache.SetValueExt<Contact.eMail>(targetContact, sourceContact.EMail);
                target.Contact.Cache.SetValueExt<Contact.webSite>(targetContact, sourceContact.WebSite);
                target.Contact.Cache.Update(targetContact);
            }
            if (targetARAccountSubLocation != null)
            {
                target.ARAccountSubLocation.Cache.SetValueExt<LocationARAccountSub.cARAccountID>(targetARAccountSubLocation, targetCARAccountID);
                target.ARAccountSubLocation.Cache.SetValueExt<LocationARAccountSub.cARSubID>(targetARAccountSubLocation, targetCARSubID);
                target.ARAccountSubLocation.Cache.SetValueExt<LocationARAccountSub.cRetainageAcctID>(targetARAccountSubLocation, targetCRetainageAcctID);
                target.ARAccountSubLocation.Cache.SetValueExt<LocationARAccountSub.cRetainageSubID>(targetARAccountSubLocation, targetCRetainageSubID);
                target.ARAccountSubLocation.Cache.Update(targetARAccountSubLocation);
            }
            target.Actions.PressSave();
        }
    }
}       