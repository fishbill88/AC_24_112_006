<px:PXSmartPanel ID="pnlSendTestEmail" runat="server" Caption="Send Test Email" CaptionVisible="true" LoadOnDemand="true" Key="SendTestEmailFilter"
	AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
	CallBackMode-PostData="Page">
	<div style="padding: 5px">
		<px:PXFormView ID="frmSendTestEmail" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="SendTestEmailFilter">
			<ContentStyle BackColor="Transparent" BorderStyle="None" />
			<Template>
				<px:PXTextEdit ID="edEmailAddress" runat="server" DataField="EmailAddress" Required="True"/>
			</Template>
		</px:PXFormView>
	</div>
	<px:PXPanel ID="PXPanel7" runat="server" SkinID="Buttons">
		<px:PXButton ID="PXButton14" runat="server" DialogResult="OK" Text="OK" CommandSourceID="ds" />
		<px:PXButton ID="PXButton15" runat="server" DialogResult="Cancel" Text="Cancel" CommandSourceID="ds" />
	</px:PXPanel>
</px:PXSmartPanel>