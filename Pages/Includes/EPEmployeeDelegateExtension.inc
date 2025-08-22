<px:PXTabItem LoadOnDemand="True" Text="Delegates">
	<Template>
		<px:PXGrid ID="gridUserEmployeeDelegates" runat="server" DataSourceID="ds" Height="400px" Width="100%" SkinID="DetailsInTab">
			<Levels>
				<px:PXGridLevel DataMember="Delegates">
					<Columns>
						<px:PXGridColumn runat="server" DataField="DelegationOf" CommitChanges="True" />
						<px:PXGridColumn runat="server" DataField="WingmanID" CommitChanges="True" DisplayMode="Text" Width="200px" />
						<px:PXGridColumn runat="server" DataField="WingmanID_EPEmployee_acctName" Visible="false" SyncVisible="False" SyncVisibility="False"/>
						<px:PXGridColumn runat="server" DataField="IsActive" CommitChanges="True" Type="CheckBox" TextAlign="Center" />
						<px:PXGridColumn runat="server" DataField="StartsOn" CommitChanges="True" />
						<px:PXGridColumn runat="server" DataField="ExpiresOn" CommitChanges="True" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="True" />
			<Mode InitNewRow="True" />
		</px:PXGrid>
	</Template>
</px:PXTabItem>
