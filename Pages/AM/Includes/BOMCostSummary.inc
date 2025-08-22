    <px:PXSmartPanel ID="PanelBOMCostRoll" runat="server" Caption="BOM Cost Summary" CaptionVisible="True"
		DesignView="Content" LoadOnDemand="true" Key="bomcostrecs" AutoCallBack-Enabled="true"
		AutoCallBack-Target="formCostSummary" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page" Width="500px" Height="350px" CloseButtonDialogResult="Abort">
        <px:PXFormView ID="formCostSummary" runat="server" DataSourceID="ds" CaptionVisible="False"
			DataMember="bomcostrecs" SkinID="Transparent" Width="100%">
			<Template>
			    <px:PXLayoutRule ID="PXLayoutRule16" runat="server" StartGroup="True" GroupCaption="" StartColumn="True" LabelsWidth="S" ControlSize="S" 
                    Merge="True" />
                <px:PXNumberEdit ID="edLotSize" runat="server" Enabled="False" DataField="LotSize" />
			    <px:PXCheckBox ID="edMultiLevelProcess" runat="server" DataField="MultiLevelProcess" />
				<px:PXLayoutRule ID="PXLayoutRule17" runat="server" StartColumn="True" GroupCaption="Labor" StartRow="True" LabelsWidth="S" ControlSize="S" 
                    Merge="True" />
                <px:PXNumberEdit ID="edFixedLabor" runat="server" DataField="FLaborCost" Enabled="False" />
                <px:PXNumberEdit ID="edVariableLabor" runat="server" DataField="VLaborCost" Enabled="False" />
                <px:PXLayoutRule ID="PXLayoutRule18" runat="server" StartColumn="True" GroupCaption="Overhead" StartRow="True" LabelsWidth="S" ControlSize="S" 
                    Merge="True" />
                <px:PXNumberEdit ID="edFOvdCost" runat="server" DataField="FOvdCost" Enabled="False" />
                <px:PXNumberEdit ID="edVOvdCost" runat="server" DataField="VOvdCost" Enabled="False" />
                <px:PXLayoutRule ID="PXLayoutRule19" runat="server" StartColumn="True" GroupCaption="Other Costs" StartRow="True" LabelsWidth="S" ControlSize="S" 
                    Merge="True" />
                <px:PXNumberEdit ID="edMachine" runat="server" DataField="MachCost" Enabled="False" />
                <px:PXNumberEdit ID="edTools" runat="server" DataField="ToolCost" Enabled="False" />
                <px:PXLayoutRule ID="PXLayoutRule20" runat="server" StartColumn="True" StartRow="True" LabelsWidth="S" ControlSize="S"  
                    Merge="True" />
                <px:PXNumberEdit ID="edMaterial" runat="server" DataField="MatlCost" Enabled="False" />
                <px:PXNumberEdit ID="edSubContract" runat="server" DataField="SubcontractMaterialCost" Enabled="False" />
                <px:PXLayoutRule ID="PXLayoutRule21" runat="server" StartColumn="True" GroupCaption="Totals" StartRow="True" LabelsWidth="S" ControlSize="S" 
                    Merge="True" />
                <px:PXNumberEdit ID="edUnitCost" runat="server" DataField="UnitCost" Enabled="False" />
                <px:PXNumberEdit ID="edTotalCost" runat="server" DataField="TotalCost" Enabled="False" />
			</Template>
		</px:PXFormView>
        <px:PXPanel ID="PanelCostSummary" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton5" runat="server" DialogResult="OK" CommandSourceID="ds" Text="OK"></px:PXButton>
        </px:PXPanel>
	</px:PXSmartPanel>