<px:PXSmartPanel ID="InventoryItemListDialog" runat="server" Caption="Inventory Item List" CaptionVisible="True" Key="selectedItems" LoadOnDemand="True" AutoRepaint="True" CloseButtonDialogResult="OK">
	<px:PXGrid ID="gridItemList" runat="server" DataSourceID="ds" SkinID="Details" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="selectedItems">
				<Columns>
					<px:PXGridColumn DataField="InventoryID" CommitChanges=True />
					<px:PXGridColumn DataField="Descr" />
				</Columns>
				<RowTemplate>
					<px:PXSegmentMask ID="iidInventoryID" runat="server" DataField="InventoryID" CommitChanges="True" />
				</RowTemplate>
				<Layout ColumnsMenu="False" />
			</px:PXGridLevel>
		</Levels>
		<AutoSize Enabled="True" MinHeight="300" />
		<Mode AllowAddNew="True" AllowDelete="True" AllowUpdate="True" AllowUpload="True" AllowFormEdit="False" />
	</px:PXGrid>
</px:PXSmartPanel>