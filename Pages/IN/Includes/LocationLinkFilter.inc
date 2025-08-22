<px:PXSmartPanel ID="LocationListDialog" runat="server" Caption="Location List" CaptionVisible="True" Key="selectedLocations" LoadOnDemand="True" AutoRepaint="True" AutoReload="True" CloseButtonDialogResult="OK">
	<px:PXGrid ID="gridLocationList" runat="server" DataSourceID="ds" SkinID="Details" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="selectedLocations">
				<Columns>
					<px:PXGridColumn DataField="LocationID" />
					<px:PXGridColumn DataField="Descr" />
				</Columns>
				<RowTemplate>
					<px:PXSegmentMask ID="lldLocationID" runat="server" DataField="LocationID" CommitChanges="True" />
				</RowTemplate>
				<Layout ColumnsMenu="False" />
			</px:PXGridLevel>
		</Levels>
		<AutoSize Enabled="True" MinHeight="300" />
		<Mode AllowAddNew="True" AllowDelete="True" AllowUpdate="True" AllowUpload="True" AllowFormEdit="False" />
	</px:PXGrid>
</px:PXSmartPanel>