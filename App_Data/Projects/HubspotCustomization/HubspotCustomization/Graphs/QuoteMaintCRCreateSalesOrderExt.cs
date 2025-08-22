using HubspotCustomization;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.SO;

public class QuoteMaintCRCreateSalesOrderExt : PXGraphExtension<PX.Objects.CR.QuoteMaint.CRCreateSalesOrderExt,
        QuoteMaint>
{
    public static bool IsActive() => true;
    public delegate void DoCreateSalesOrderDelegate();
    [PXOverride]
    public void DoCreateSalesOrder(DoCreateSalesOrderDelegate baseMethod)
    {
        SOOrderEntry SOGraph = PXGraph.CreateInstance<SOOrderEntry>();
        PXGraph.InstanceCreated.AddHandler(delegate (SOOrderEntry graph)
        {
            graph.RowUpdated.AddHandler<SOOrder>(delegate (PXCache sender, PXRowUpdatedEventArgs e)
            {
                SOOrder order = e.Row as SOOrder;
                if (order != null)
                {
                    string hubspotDealID = Base.Quote.Current.GetExtension<CRQuoteExt>().UsrHubspotDealID;
                    graph.Document.Cache.SetValueExt<SOOrderExt.usrHubspotDealID>(order,
                        hubspotDealID);
                }
            });

        });
        baseMethod();
    }
}