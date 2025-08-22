using System;
using PX.Objects.FS;

public partial class Page_FS300200 : PX.Web.UI.PXPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void edContactID_EditRecord(object sender, PX.Web.UI.PXNavigateEventArgs e)
    {
        AppointmentEntry graph = this.ds.DataGraph as AppointmentEntry;
        if (graph != null)
        {
            FSServiceOrder serviceOrder = this.ds.DataGraph.Views["ServiceOrderRelated"].Cache.Current as FSServiceOrder;
            if (serviceOrder.ContactID == null && serviceOrder.CustomerID != null)
            {
                try
                {
                    graph.addNewContact.Press();
                }
                catch (PX.Data.PXRedirectRequiredException e1)
                {
                    PX.Web.UI.PXBaseDataSource ds = this.ds as PX.Web.UI.PXBaseDataSource;
                    PX.Web.UI.PXBaseDataSource.RedirectHelper helper = new PX.Web.UI.PXBaseDataSource.RedirectHelper(ds);
                    helper.TryRedirect(e1);
                }
            }
        }
    }
}
