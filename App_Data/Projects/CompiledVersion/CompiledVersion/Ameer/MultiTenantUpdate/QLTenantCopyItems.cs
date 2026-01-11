using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR.Standalone;
using PX.Objects.CS;
using System;
using System.Linq;
using System.Reflection;

namespace QLTenantCopyItems
{
    public class QLCustomerMaintVisibilityRestrictionExt : PXGraphExtension<CustomerMaintVisibilityRestriction, CustomerMaint>
    {
        protected void Customer_RowUpdating(PXCache cache, PXRowUpdatingEventArgs e, PXRowUpdating del)
        {
            int? cOrgBAccountID;
            int? nullable;
            WebDialogResult? maintVisibilityRestrictionDialogResult;
            WebDialogResult webDialogResult;
            bool flag;
            bool flag1;
            QLCustomerMaintExt ext = base.Base.GetExtension<QLCustomerMaintExt>();
            FieldInfo ResetLocationBranch = typeof(CustomerMaintVisibilityRestriction).GetField("ResetLocationBranch", BindingFlags.Instance | BindingFlags.NonPublic);
            if (!ext.IsExtEnabled)
            {
                CustomerMaintVisibilityRestriction base1 = base.Base1;
                nullable = ((Customer)e.Row).COrgBAccountID;
                cOrgBAccountID = ((Customer)e.NewRow).COrgBAccountID;
                ResetLocationBranch.SetValue(base1, !(nullable.GetValueOrDefault() == cOrgBAccountID.GetValueOrDefault() & nullable.HasValue == cOrgBAccountID.HasValue));
                cOrgBAccountID = ((Customer)e.Row).COrgBAccountID;
                if (cOrgBAccountID.GetValueOrDefault() != 0 | !cOrgBAccountID.HasValue)
                {
                    cOrgBAccountID = ((Customer)e.NewRow).COrgBAccountID;
                    if (cOrgBAccountID.GetValueOrDefault() != 0 | !cOrgBAccountID.HasValue)
                    {
                        goto Label1;
                    }
                    flag = base.Base.GetExtension<CustomerMaint.LocationDetailsExt>().Locations.Select<Location>(Array.Empty<object>()).ToList<Location>().Any<Location>((Location l) => l.CBranchID.HasValue);
                    goto Label0;
                }
            Label1:
                flag = false;
            Label0:
                if (flag)
                {
                    QLCustomerDialogResultsExt dialogResultExt = PXCache<Customer>.GetExtension<QLCustomerDialogResultsExt>((Customer)e.Row);
                    CustomerMaintVisibilityRestriction customerMaintVisibilityRestriction = base.Base1;
                    maintVisibilityRestrictionDialogResult = dialogResultExt.MaintVisibilityRestrictionDialogResult;
                    webDialogResult = WebDialogResult.No;
                    ResetLocationBranch.SetValue(customerMaintVisibilityRestriction, maintVisibilityRestrictionDialogResult.GetValueOrDefault() == webDialogResult & maintVisibilityRestrictionDialogResult.HasValue);
                }
            }
            else
            {
                CustomerMaintVisibilityRestriction base11 = base.Base1;
                cOrgBAccountID = ((Customer)e.Row).COrgBAccountID;
                nullable = ((Customer)e.NewRow).COrgBAccountID;
                ResetLocationBranch.SetValue(base11, !(cOrgBAccountID.GetValueOrDefault() == nullable.GetValueOrDefault() & cOrgBAccountID.HasValue == nullable.HasValue));
                nullable = ((Customer)e.Row).COrgBAccountID;
                if (nullable.GetValueOrDefault() != 0 | !nullable.HasValue)
                {
                    nullable = ((Customer)e.NewRow).COrgBAccountID;
                    if (nullable.GetValueOrDefault() != 0 | !nullable.HasValue)
                    {
                        goto Label3;
                    }
                    flag1 = base.Base.GetExtension<CustomerMaint.LocationDetailsExt>().Locations.Select<Location>(Array.Empty<object>()).ToList<Location>().Any<Location>((Location l) => l.CBranchID.HasValue);
                    goto Label2;
                }
            Label3:
                flag1 = false;
            Label2:
                if (flag1)
                {
                    QLCustomerDialogResultsExt dialogResultExt = PXCache<Customer>.GetExtension<QLCustomerDialogResultsExt>((Customer)e.Row);
                    CustomerMaintVisibilityRestriction customerMaintVisibilityRestriction1 = base.Base1;
                    WebDialogResult? nullable1 = new WebDialogResult?(base.Base.GetExtension<CustomerMaint.LocationDetailsExt>().Locations.Ask("Warning", "Do you want to keep the value in the Shipping Branch box?", MessageButtons.YesNo));
                    dialogResultExt.MaintVisibilityRestrictionDialogResult = nullable1;
                    maintVisibilityRestrictionDialogResult = nullable1;
                    webDialogResult = WebDialogResult.No;
                    ResetLocationBranch.SetValue(customerMaintVisibilityRestriction1, maintVisibilityRestrictionDialogResult.GetValueOrDefault() == webDialogResult & maintVisibilityRestrictionDialogResult.HasValue);
                }
            }
        }

        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.visibilityRestriction>();
        }
    }
}