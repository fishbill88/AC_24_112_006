<px:PXSmartPanel ID="PanelAddSiteStatus" runat="server" Key="ItemInfo" LoadOnDemand="true" Width="1100px" Height="500px"
	Caption="Inventory Lookup" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true">
	<px:PXFormView ID="formSitesStatus" runat="server" CaptionVisible="False" DataMember="ItemFilter" DataSourceID="ds"
		Width="100%" SkinID="Transparent">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="PanelAddSiteStatus_edInventory" runat="server" DataField="Inventory" IsClientControl="False"/>
			<px:PXTextEdit CommitChanges="True" ID="PanelAddSiteStatus_edBarCode" runat="server" DataField="BarCode" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edSiteID" runat="server" DataField="SiteID" AutoRefresh="true" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edItemClassID" runat="server" DataField="ItemClass" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edSubItem" runat="server" DataField="SubItem" AutoRefresh="true" />

			<px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="S" GroupCaption="Selected Mode" StartColumn="true" />
			<px:PXGroupBox CommitChanges="True" RenderStyle="Simple" ID="PanelAddSiteStatus_gpMode" runat="server" Caption="Selected Mode"
				DataField="Mode" Width="280px" Height="25px">
				<Template>
					<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="S" ControlSize="S" />
					<px:PXRadioButton runat="server" ID="PanelAddSiteStatus_rModeSite" Value="0" Text="By Site State" />
					<px:PXRadioButton runat="server" ID="PanelAddSiteStatus_rModeCustomer" Value="1" Text="By Last Sale" />
					<px:PXDateTimeEdit CommitChanges="True" ID="PanelAddSiteStatus_edHistoryDate" runat="server" DataField="HistoryDate" SuppressLabel="true" />
				</Template>
			</px:PXGroupBox>

			<px:PXPanel runat="server" Height="50px" RenderStyle="Simple" ID="PanelAddSiteStatus_pnlOnlyAvailable" >
				<px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="M" />
				<px:PXCheckBox CommitChanges="True" ID="PanelAddSiteStatus_chkOnlyAvailable" AlignLeft="true" runat="server" Checked="True" DataField="OnlyAvailable" />
				<px:PXCheckBox CommitChanges="True" ID="PanelAddSiteStatus_chkDropShipSales" AlignLeft="true" runat="server" Checked="True" DataField="DropShipSales" />
			</px:PXPanel>

			<px:PXLayoutRule runat="server" StartColumn="true" StartGroup="true" LabelsWidth="SM" ControlSize="M" GroupCaption="Shipping Parameters to Apply" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edCustomerLocationIDApply" runat="server" AutoRefresh="True"
					DataField="CustomerLocationID" DataSourceID="ds" />
		</Template>
	</px:PXFormView>
	<script type="text/javascript">
	    function UpdateItemSiteCell(n, c) {
			var activeRow = c.cell.row;
			var sCell = activeRow.getCell("Selected");
			var qCell = activeRow.getCell("QtySelected");
			if (sCell == c.cell) {
				if (sCell.getValue() == true)
					qCell.setValue("1");
				else
					qCell.setValue("0");
			}
			if (qCell == c.cell) {
				if (qCell.getValue() == "0")
					sCell.setValue(false);
				else
					sCell.setValue(true);
			}
		}
    </script>
	<px:PXGrid ID="PanelAddSiteStatus_gripSiteStatus" runat="server" DataSourceID="ds" Style="height: 189px;"
		AutoAdjustColumns="true" Width="100%" SkinID="Details" AdjustPageSize="Auto" AllowSearch="True" FastFilterID="PanelAddSiteStatus_edInventory"
		FastFilterFields="InventoryCD,Descr,AlternateID">
		<CallbackCommands>
			<Refresh CommitChanges="true"></Refresh>
		</CallbackCommands>
		<ClientEvents AfterCellUpdate="UpdateItemSiteCell" />
		<ActionBar PagerVisible="False">
			<PagerSettings Mode="NextPrevFirstLast" />
		</ActionBar>
		<Levels>
			<px:PXGridLevel DataMember="ItemInfo">
				<Mode AllowAddNew="false" AllowDelete="false" />
				<RowTemplate>
					<px:PXSegmentMask ID="PanelAddSiteStatus_editemClass" runat="server" DataField="ItemClassID" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AutoCallBack="true"
						AllowCheckAll="true" />
					<px:PXGridColumn AllowNull="False" DataField="QtySelected" TextAlign="Right" />
					<px:PXGridColumn DataField="SiteID" />
					<px:PXGridColumn DataField="SiteCD" 
						AllowNull="False" SyncNullable ="false" 
						Visible="False" SyncVisible="false" 
						AllowShowHide ="False" SyncVisibility="false" />
					<px:PXGridColumn DataField="ItemClassID" />
					<px:PXGridColumn DataField="ItemClassDescription" />
					<px:PXGridColumn DataField="PriceClassID" />
					<px:PXGridColumn DataField="PriceClassDescription" />
					<px:PXGridColumn DataField="PreferredVendorID" />
					<px:PXGridColumn DataField="PreferredVendorDescription" />
					<px:PXGridColumn DataField="InventoryCD" DisplayFormat="&gt;AAAAAAAAAA" />
					<px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
					<px:PXGridColumn DataField="SubItemCD"
						AllowNull="False" SyncNullable ="false" 
						Visible="False" SyncVisible="false" 
						AllowShowHide ="False" SyncVisibility="false" />
					<px:PXGridColumn DataField="Descr" />
					<px:PXGridColumn DataField="SalesUnit" DisplayFormat="&gt;aaaaaa" />
					<px:PXGridColumn AllowNull="False" DataField="QtyAvailSale" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="QtyOnHandSale" TextAlign="Right" />
					<px:PXGridColumn DataField="CuryID" DisplayFormat="&gt;LLLLL" />
					<px:PXGridColumn AllowNull="False" DataField="QtyLastSale" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="CuryUnitPrice" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="LastSalesDate" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="DropShipLastQty" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="DropShipCuryUnitPrice" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="DropShipLastDate" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="AlternateID" AllowFilter="false" AllowSort="False" />
					<px:PXGridColumn AllowNull="False" DataField="AlternateType" AllowFilter="false" AllowSort="False" />
					<px:PXGridColumn AllowNull="False" DataField="AlternateDescr" AllowFilter="false" AllowSort="False" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Enabled="true" />
	</px:PXGrid>
	<px:PXPanel ID="PanelAddSiteStatus_pnlButtons" runat="server" SkinID="Buttons">
		<px:PXButton ID="PanelAddSiteStatus_btnAdd" runat="server" CommandName="AddSelectedItems" CommandSourceID="ds" Text="Add" SyncVisible="false" />
		<px:PXButton ID="PanelAddSiteStatus_btnAddClose" runat="server" Text="Add & Close" DialogResult="OK" />
		<px:PXButton ID="PanelAddSiteStatus_btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
	</px:PXPanel>
</px:PXSmartPanel>