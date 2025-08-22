<px:PXTabItem Text="Payment Links" RepaintOnDemand="false">
	<Template>
		<px:PXFormView ID="PayLinks" runat="server" DataMember="CurrentDocument" RenderStyle="Simple" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule runat="server" ColumnWidth="400px"  ControlSize="SM" LabelsWidth="SM" StartColumn="True" />
				<px:PXSelector CommitChanges="True" ID="PXSelProcCenter" runat="server" DataField="ProcessingCenterID" AutoRefresh="true"  />
				<px:PXDropDown CommitChanges="True" ID="cmbDeliveryMethod" runat="server"  DataField="DeliveryMethod" />
				<px:PXFormView ID="PXFormPayLink1" runat="server" DataMember="PayLink" RenderStyle="Simple">
					<Template>
						<px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="SM" />
						<px:PXTextEdit Width="300px" ID="txtUrl" runat="server" DataField="Url" />
						<px:PXDropDown ID="cmbLinkStatus" runat="server"  DataField="LinkStatus" />
					</Template>
				</px:PXFormView>
				<px:PXLayoutRule runat="server" LabelsWidth="XS" StartColumn="True"  />
				<px:PXButton ID="btnCreateLink" runat="server" CommandName="CreateLink" Width="165px" CommandSourceID="ds" />
				<px:PXButton ID="btnSyncLink" runat="server" CommandName="SyncLink" Width="165px" CommandSourceID="ds" />
				<px:PXButton ID="btnResend" runat="server" CommandName="ResendLink" Width="165px" CommandSourceID="ds" />
			</Template>
		</px:PXFormView>:
	</Template>
</px:PXTabItem>