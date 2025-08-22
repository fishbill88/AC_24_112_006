<px:PXSmartPanel ID="PanelSelectNotificatonTemplate" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px; height: 110px; width: 355px;"
	Caption="Select Template" CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true" Key="NotificationInfo" AutoCallBack-Enabled="true"
	AutoCallBack-Target="formSelectNotificationTemplate" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
	AcceptButtonID="SelectNotificatonTemplateOK" CancelButtonID="SelectNotificatonTemplateCancel">

	<px:PXFormView ID="formSelectNotificationTemplate" runat="server" DataMember="NotificationInfo" DataSourceID="ds" Style="z-index: 100" Width="100%"
		Caption="Notification Template" CaptionVisible="False" SkinID="Transparent">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXSelector DataField="NotificationName" CommitChanges="True" ID="edNtificationName" runat="server" AutoRefresh="True">
				<GridProperties FastFilterFields="Description, SiteMap__Title, ScreenID, Subject" />
			</px:PXSelector>
			<px:PXCheckBox DataField="ReplaceEmailContents" CommitChanges="True" ID="chkReplaceEmailContents" runat="server"/>
		</Template>
	</px:PXFormView>

	<div style="padding: 10px 10px 5px 5px; text-align: right;">
		<px:PXButton ID="SelectNotificatonTemplateOK" runat="server" Text="Select" DialogResult="OK" Width="60px" Height="24px"></px:PXButton>
		<px:PXButton ID="SelectNotificatonTemplateCancel" runat="server" DialogResult="Cancel" Text="Cancel" Width="60px" Height="24px" Style="margin-left: 5px" />
	</div>

</px:PXSmartPanel>