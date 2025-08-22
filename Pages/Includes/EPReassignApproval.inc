<px:PXSmartPanel ID="PanelReassignApproval" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;"
    Caption="Reassign Approval"
    CaptionVisible="True" LoadOnDemand="true" AutoReload="true" Key="ReassignApprovalFilter"
    AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" CloseButtonDialogResult="Abort">

    <px:PXFormView ID="formReassignApproval" runat="server" DataMember="ReassignApprovalFilter" DataSourceID="ds" Style="z-index: 100" Width="100%"
        Caption="Reassign Approval" CaptionVisible="False" SkinID="Transparent">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSelector DataField="NewApprover" CommitChanges="True" ID="edNewApprover" runat="server" AutoRefresh="True" />
            <px:PXCheckBox DataField="IgnoreApproversDelegations" CommitChanges="True" ID="chkIgnoreApproversDelegations" runat="server" />
        </Template>
    </px:PXFormView>

    <div style="padding: 10px 10px 5px 5px; text-align: right;">
        <px:PXButton ID="btnReassignApprovalOK" runat="server" Text="Reassign" DialogResult="OK"></px:PXButton>
        <px:PXButton ID="btnReassignApprovalCancel" runat="server" DialogResult="Cancel" Text="Cancel" Style="margin-left: 5px" />
    </div>

</px:PXSmartPanel>