<px:PXTabItem Text="Marketing Lists" LoadOnDemand="True">
	<Template>
		<px:PXGrid ID="grdMarketingLists" runat="server" Height="400px" Width="100%" SyncPosition="True"
			AdjustPageSize="Auto" AllowPaging="True" ActionsPosition="Top" AllowSearch="true" DataSourceID="ds"
			SkinID="Inquire" >

			<Levels>
				<px:PXGridLevel DataMember="Subscriptions">
					<Columns>
						<px:PXGridColumn DataField="IsSubscribed" Type="CheckBox" TextAlign="Center" Width="100px" />

						<px:PXGridColumn DataField="CRMarketingList__MailListCode" AutoCallBack="true" Visible="True" SyncVisible="false" TextAlign="Left" DisplayMode="Text" LinkCommand="ViewMarketingList" Width="130px" />
						<px:PXGridColumn DataField="CRMarketingList__Name" />
						<px:PXGridColumn DataField="CRMarketingList__Type" Type="CheckBox" TextAlign="Center" />
						<px:PXGridColumn DataField="CRMarketingList__Status" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="CRMarketingList__WorkgroupID" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="CRMarketingList__OwnerID" Visible="false" SyncVisible="false" LinkCommand="OwnerID_ViewDetails" />
						<px:PXGridColumn DataField="CRMarketingList__Method" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="CRMarketingList__GIDesignID" Visible="false" SyncVisible="false" LinkCommand="GIDesignID_ViewDetails" />
						<px:PXGridColumn DataField="CRMarketingList__CreatedByID" Visible="false" SyncVisible="false" LinkCommand="CreatedByID_ViewDetails" />
						<px:PXGridColumn DataField="CRMarketingList__CreatedDateTime" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="CRMarketingList__LastModifiedByID" Visible="false" SyncVisible="false" LinkCommand="LastModifiedByID_ViewDetails" />
						<px:PXGridColumn DataField="CRMarketingList__LastModifiedDateTime" Visible="false" SyncVisible="false" />
					</Columns>
				</px:PXGridLevel>
			</Levels>

			<ActionBar>
				<CustomItems>
					<px:PXToolBarButton Text="Manage Subscription" Key="cmdManageSubscriptionMenu">
						<MenuItems>
							<px:PXMenuItem>
								<AutoCallBack Command="SubscribeAll" Enabled="true" Target="ds" />
							</px:PXMenuItem>
							<px:PXMenuItem>
								<AutoCallBack Command="UnsubscribeAll" Enabled="true" Target="ds" />
							</px:PXMenuItem>
						</MenuItems>
					</px:PXToolBarButton>
					<px:PXToolBarButton>
						<AutoCallBack Command="ViewMarketingList" Target="ds" />
						<PopupCommand Command="RefreshMarketingListMembers" Target="ds" />
					</px:PXToolBarButton>
				</CustomItems>
			</ActionBar>

			<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />

		</px:PXGrid>
	</Template>
</px:PXTabItem>