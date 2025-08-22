    <px:PXSmartPanel ID="PanelMakePlanning" runat="server" Caption="Planning BOM Levels" CaptionVisible="True"
        DesignView="Content" LoadOnDemand="true" Key="PlanningBomLevelsFilter" AutoCallBack-Enabled="true"
        AutoCallBack-Target="formPlanningBomLevels" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page" Width="175px" Height="125px">
        <px:PXFormView ID="formPlanningBomLevels" runat="server" DataSourceID="ds" CaptionVisible="False"
            DataMember="PlanningBomLevelsFilter" SkinID="Transparent" Width="100%">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule16" runat="server" StartGroup="True" GroupCaption="" StartColumn="True" LabelsWidth="S" ControlSize="S" />
                <px:PXCheckBox ID="edItem" runat="server" DataField="Item" AlignLeft="True" />
                <px:PXCheckBox ID="edWarehouse" runat="server" DataField="Warehouse" AlignLeft="True" />
                <px:PXCheckBox ID="edSubItem" runat="server" DataField="SubItem" AlignLeft="True" />
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanelMakePlanningBtn" runat="server" SkinID="Transparent">
            <px:PXButton ID="PXButtonMakePlanningOk" runat="server" DialogResult="OK" Text="Update" CommandSourceID="ds" />
            <px:PXButton ID="PXButtonMakePlanningCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
	</px:PXSmartPanel>