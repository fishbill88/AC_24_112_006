<px:PXTabItem Text="Relations" LoadOnDemand="True" RepaintOnDemand="False">
	<Template>
		<px:PXGrid ID="grdRelations" runat="server" Height="400px" Width="100%" AllowPaging="True" SyncPosition="True" MatrixMode="True"
			ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SkinID="Details" AdjustPageSize = "Auto">
			<Levels>
				<px:PXGridLevel DataMember="Relations">
				<Columns>
					<px:PXGridColumn DataField="Role" CommitChanges="True" Width="130px" />
					<px:PXGridColumn DataField="IsPrimary" Type="CheckBox" TextAlign="Center" CommitChanges="True" />
					<px:PXGridColumn DataField="TargetType" CommitChanges="True" Width="120px" />
					<px:PXGridColumn DataField="TargetNoteID" DisplayMode="Text" LinkCommand="RelationsViewTargetDetails" CommitChanges="True" Width="200px" />
					<px:PXGridColumn DataField="Description" Width="250px" />
					<px:PXGridColumn DataField="Status" Width="150px" AllowFilter="False" AllowSort="False"/>
					<px:PXGridColumn DataField="OwnerID" Width="200px" />
					<px:PXGridColumn DataField="EntityID" AutoCallBack="true" LinkCommand="RelationsViewEntityDetails" CommitChanges="True" Width="200px" />
					<px:PXGridColumn DataField="Name" Width="200px" />
					<px:PXGridColumn DataField="ContactID" AutoCallBack="true" TextAlign="Left" TextField="ContactName" DisplayMode="Text" LinkCommand="RelationsViewContactDetails" Width="200px" />
					<px:PXGridColumn DataField="Email" Width="200px" />
					<px:PXGridColumn DataField="AddToCC" Type="CheckBox" TextAlign="Center" Width="90px" />

					<px:PXGridColumn DataField="DocumentDate" Width="200px" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="CreatedDateTime" Width="200px" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="CreatedByID" Width="200px" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="LastModifiedByID" Width="200px" Visible="false" SyncVisible="false" />
				</Columns>
				<RowTemplate>
					<px:PXSelector ID="edTargetNoteID" runat="server" DataField="TargetNoteID" FilterByAllFields="True" AutoRefresh="True" />
					<px:PXSelector ID="edRelEntityID" runat="server" DataField="EntityID" FilterByAllFields="True" AutoRefresh="True" />
					<px:PXSelector ID="edRelContactID" runat="server" DataField="ContactID" FilterByAllFields="True" AutoRefresh="True" />
					<px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" AutoRefresh="True" />

					<px:PXDateTimeEdit ID="edCreatedDateTime" runat="server" DataField="CreatedDateTime_Date" />
				</RowTemplate>
				</px:PXGridLevel>
			</Levels>
			<Mode InitNewRow="True" ></Mode>
			<AutoSize Enabled="True" MinHeight="100" MinWidth="100" ></AutoSize>
		</px:PXGrid>
	</Template>
</px:PXTabItem>