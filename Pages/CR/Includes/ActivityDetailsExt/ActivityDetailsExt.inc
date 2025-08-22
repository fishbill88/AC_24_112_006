<px:PXTabItem Text="Activities" LoadOnDemand="True" RepaintOnDemand="False">
	<Template>
		<pxa:PXGridWithPreview
				ID="gridActivities" runat="server" DataSourceID="ds" Width="100%" AllowSearch="True" DataMember="Activities" AllowPaging="true" NoteField="NoteText"
				FilesField="NoteFiles" BorderWidth="0px" GridSkinID="Inquire" SplitterStyle="z-index: 100; border-top: solid 1px Gray;  border-bottom: solid 1px Gray"
				PreviewPanelStyle="z-index: 100; background-color: Window" PreviewPanelSkinID="Preview" SyncPosition="True"	BlankFilterHeader="All Activities" MatrixMode="true" PrimaryViewControlID="form">

			<AutoSize Enabled="true" />
			<GridMode AllowAddNew="False" AllowDelete="False" AllowFormEdit="False" AllowUpdate="False" AllowUpload="False" />

			<ActionBar DefaultAction="cmdViewActivity" CustomItemsGroup="0">
				<CustomItems>
					<px:PXToolBarButton Key="cmdAddTask">
						<AutoCallBack Command="NewTask" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdAddEvent">
						<AutoCallBack Command="NewEvent" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdAddEmail">
						<AutoCallBack Command="NewMailActivity" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdAddActivity">
						<AutoCallBack Command="NewActivity" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdViewActivity">
						<AutoCallBack Command="ViewActivity" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Tooltip="Pin or unpin the activity">
						<AutoCallBack Command="TogglePinActivity" Target="ds" />
					</px:PXToolBarButton>
				</CustomItems>
			</ActionBar>

			<Levels>
				<px:PXGridLevel DataMember="Activities">
					<Columns>
						<px:PXGridColumn DataField="IsPinned"					Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="IsCompleteIcon"				Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="PriorityIcon"				Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="CRReminder__ReminderIcon"	Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="ClassIcon"					Width="35px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="ClassInfo" Width="90px" />
						<px:PXGridColumn DataField="Subject" LinkCommand="ViewActivity" Width="400px" />
						<px:PXGridColumn DataField="UIStatus" />
						<px:PXGridColumn DataField="Released" TextAlign="Center" Type="CheckBox" />
						<px:PXGridColumn DataField="StartDate" DisplayFormat="g" Width="125px" />
						<px:PXGridColumn DataField="CompletedDate" DisplayFormat="g" Width="125px" />
						<px:PXGridColumn DataField="TimeSpent" />
						<px:PXGridColumn DataField="CreatedByID" Visible="False" AllowShowHide="False" />
						<px:PXGridColumn DataField="CreatedByID_Creator_Username" Visible="False" SyncVisible="False" SyncVisibility="False" Width="108px">
							<NavigateParams>
								<px:PXControlParam Name="PKID" ControlID="gridActivities" PropertyName="DataValues[&quot;CreatedByID&quot;]" />
							</NavigateParams>
						</px:PXGridColumn>
						<px:PXGridColumn DataField="OwnerID" LinkCommand="OpenActivityOwner" DisplayMode="Text" />
						<px:PXGridColumn DataField="Source" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="BAccountID" LinkCommand="<stub>" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="ContactID" DisplayMode="Text" LinkCommand="<stub>" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="ProjectID" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="ProjectTaskID" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" Width="125px" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="WorkgroupID" Visible="False" SyncVisible="False" />
					</Columns>
				</px:PXGridLevel>
			</Levels>

			<PreviewPanelTemplate>
				<px:PXHtmlView ID="edBody" runat="server" DataField="body" TextMode="MultiLine" MaxLength="50" Width="100%" Height="100px" SkinID="Label" >
					<AutoSize Container="Parent" Enabled="true" />
				</px:PXHtmlView>
			</PreviewPanelTemplate>

		</pxa:PXGridWithPreview>
	</Template>
</px:PXTabItem>