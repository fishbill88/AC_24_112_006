    <px:PXSmartPanel ID="PanelRef" runat="server" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True"
        AutoCallBack-Target="gridref" Caption="Reference Designators" CaptionVisible="True"
		DesignView="Content" Height="350px" Width="600px" LoadOnDemand="true">
		<px:PXGrid ID="gridref" runat="server" DataSourceID="ds" SkinID="DetailsInTab" MatrixMode="true" Width="100%" Height="100%">
			<Levels>
				<px:PXGridLevel DataKeyNames="BOMID,OperationID,MatlLineID,LineID,EffStartDate" DataMember="BomRefRecords">
					<RowTemplate>
						<px:PXTextEdit ID="edRefDes" runat="server" DataField="RefDes" CommitChanges="true" />
						<px:PXTextEdit ID="edDescrRef" runat="server" DataField="Descr" CommitChanges="true" />
					</RowTemplate>
					<Columns>
						<px:PXGridColumn DataField="RefDes" Width="120px" CommitChanges="true" />
                        <px:PXGridColumn DataField="Descr" Width="200px" CommitChanges="true" />
                        <px:PXGridColumn DataField="BOMID" />
                        <px:PXGridColumn DataField="OperationID" />
                        <px:PXGridColumn DataField="LineID" TextAlign="Right" />
                        <px:PXGridColumn DataField="MatlLineID" TextAlign="Right" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="True" />
			<ActionBar ActionsText="False">
			</ActionBar>
			<Parameters>
				<px:PXSyncGridParam ControlID="gridmatl" />
			</Parameters>
		</px:PXGrid>
		<px:PXPanel ID="PXPanelRefDesBtn" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="OK" />
            <px:PXButton ID="PXButton2" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>