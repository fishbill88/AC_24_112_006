    <px:PXSmartPanel ID="BOMCostSettings" runat="server" Caption="BOM Cost Settings" CaptionVisible="True"
		DesignView="Content" LoadOnDemand="true" Key="rollsettings" AutoCallBack-Enabled="true"
		AutoCallBack-Target="formCostSummary" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page" Width="300px" Height="150px" CloseButtonDialogResult="Abort">
        <px:PXFormView ID="RollSettingsForm" runat="server" DataSourceID="ds" CaptionVisible="False"
			DataMember="rollsettings" SkinID="Transparent" Width="100%">
			<Template>
			    <px:PXLayoutRule ID="PXLayoutRule22" runat="server" StartGroup="True" GroupCaption="" StartColumn="True" LabelsWidth="S" 
                    ControlSize="S" Merge="True" />
                <px:PXNumberEdit ID="edLotSize" runat="server" Enabled="True" DataField="LotSize" CommitChanges="true" />
                <px:PXLayoutRule ID="PXLayoutRule23" runat="server" StartGroup="True" GroupCaption="" StartColumn="True" LabelsWidth="S" 
                    ControlSize="S" Merge="True" />
                <px:PXDropDown ID="edSnglMlti" runat="server" AllowNull="False" DataField="SnglMlti" CommitChanges="true" />
            </Template>
		</px:PXFormView>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton7" runat="server" DialogResult="OK" CommandSourceID="ds" Text="OK"></px:PXButton>
            <px:PXButton ID="PXButton8" runat="server" DialogResult="Abort" CommandSourceID="ds" Text="Cancel"></px:PXButton>
		</px:PXPanel>
	</px:PXSmartPanel>