<px:PXSmartPanel ID="PanelUpdatePassword" runat="server"
	Caption="Update Password" CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true" Key="UpdatePasswordFilterView" AutoCallBack-Enabled="true"
	AutoCallBack-Target="formUpdatePassword" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
	AcceptButtonID="UpdatePasswordOK" CancelButtonID="UpdatePasswordCancel">

	<px:PXFormView ID="formUpdatePassword" runat="server" DataMember="UpdatePasswordFilterView" DataSourceID="ds" Style="z-index: 100" Width="100%"
		Caption="Update Password" CaptionVisible="False" SkinID="Transparent">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="M" />
			<px:PXTextEdit ID="edEmailAccountPassword" runat="server" CommitChanges="True" DataField="EmailAccountPassword" TextMode="Password" Required="True"/>
		</Template>
	</px:PXFormView>

	<div style="padding: 10px 10px 5px 5px; text-align: right;">
		<px:PXButton ID="UpdatePasswordOK" runat="server" Text="Update" DialogResult="OK" Width="60px" Height="24px"></px:PXButton>
		<px:PXButton ID="UpdatePasswordCancel" runat="server" DialogResult="Cancel" Text="Cancel" Width="60px" Height="24px" Style="margin-left: 5px" />
	</div>
</px:PXSmartPanel>