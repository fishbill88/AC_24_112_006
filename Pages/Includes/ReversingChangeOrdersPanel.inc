<px:PXSmartPanel runat="server" ID="ReversingChangeOrdersPanel" Height="396px" Width="850px" LoadOnDemand="true" AutoRepaint="true" CaptionVisible="True" Caption="Reversing Change Orders" Key="ReversingChangeOrders" AutoCallBack-Enabled="True" AutoCallBack-Target="ReversingChangeOrdersGrid" AutoCallBack-Command="Refresh">
    <px:PXGrid runat="server" SyncPosition="true" Height="240px" SkinID="Inquire" Width="100%" ID="ReversingChangeOrdersGrid" DataSourceID="ds">
        <AutoSize Enabled="true" />
        <Mode AllowAddNew="False" AllowUpdate="False" AllowDelete="False" />
        <Levels>
            <px:PXGridLevel DataMember="ReversingChangeOrders" >
                <Columns>
                    <px:PXGridColumn DataField="RefNbr" LinkCommand="ViewCurrentReversingChangeOrder"/>
                    <px:PXGridColumn DataField="Description" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
    </px:PXGrid>
</px:PXSmartPanel>