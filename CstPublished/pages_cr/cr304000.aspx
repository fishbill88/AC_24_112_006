<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CR304000.aspx.cs" Inherits="Page_CR304000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource EnableAttributes="true" ID="ds" UDFTypeField="ClassID" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.OpportunityMaint" PrimaryView="Opportunity">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="CreateQuote" Visible="True" PopupVisible="True"/> <%--Important--%>
            <px:PXDSCallbackCommand Name="ShowMatrixPanel" Visible="False" />
        </CallbackCommands>
        <DataTrees>
            <px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
        </DataTrees>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Opportunity Summary" DataMember="Opportunity" FilesIndicator="True"
        NoteIndicator="True" LinkIndicator="True" BPEventsIndicator="True" DefaultControlID="edOpportunityID" TabIndex="100">
        <CallbackCommands>
            <Save PostData="Self" ></Save>
        </CallbackCommands>        
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
            <px:PXSelector ID="edOpportunityID" runat="server" DataField="OpportunityID" FilterByAllFields="True" AutoRefresh="True" ></px:PXSelector>
            <px:PXDropDown CommitChanges="True" ID="edStatus" runat="server" AllowNull="False" DataField="Status" ></px:PXDropDown>
            <px:PXSelector CommitChanges="True" ID="edClassID" runat="server" DataField="ClassID" AllowEdit="True" TextMode="Search" FilterByAllFields="True" edit="1" AutoRefresh="True" ></px:PXSelector>
            <px:PXDropDown ID="edStageID" runat="server" AllowNull="False" DataField="StageID" CommitChanges="True" ></px:PXDropDown>
            <px:PXDateTimeEdit DataField="CloseDate" ID="edCloseDate" runat="server" CommitChanges="true" ></px:PXDateTimeEdit>
            <px:PXLayoutRule runat="server" ColumnSpan="2" ></px:PXLayoutRule>
            <px:PXTextEdit ID="edSubject" runat="server" AllowNull="False" DataField="Subject" CommitChanges="True" ></px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
            <px:PXSegmentMask CommitChanges="True" ID="edBAccountID" runat="server" DataField="BAccountID" AllowEdit="True" FilterByAllFields="True" TextMode="Search" AutoRefresh="True"></px:PXSegmentMask>
            <px:PXSelector ID="edLocationID" runat="server" DataField="LocationID"  AllowEdit="True" TextMode="Search" DisplayMode="Hint" FilterByAllFields="True" CommitChanges="True" AutoRefresh="True" ></px:PXSelector>
			<px:PXLayoutRule runat="server" Merge="True" ></px:PXLayoutRule>
            <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID" TextField="displayName" AllowEdit="True" AutoRefresh="True" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" edit="1" OnEditRecord="edContactID_EditRecord"></px:PXSelector>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_offline&quot; >&lt;/Images>
      " runat="server" ID="edContactTeamsCardOffline" CommandSourceID="ds" CommandName="ContactTeamsCardOffline" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
				<Images Normal="svg:teams@teams_offline" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_available&quot; >&lt;/Images>
      " runat="server" ID="edContactTeamsCardAvailable" CommandSourceID="ds" CommandName="ContactTeamsCardAvailable" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
				<Images Normal="svg:teams@teams_available" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_busy&quot; >&lt;/Images>
      " runat="server" ID="edContactTeamsCardBusy" CommandSourceID="ds" CommandName="ContactTeamsCardBusy" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
				<Images Normal="svg:teams@teams_busy" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_away&quot; >&lt;/Images>
      " runat="server" ID="edContactTeamsCardAway" CommandSourceID="ds" CommandName="ContactTeamsCardAway" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
				<Images Normal="svg:teams@teams_away" ></Images>
			</px:PXButton>
			<px:PXLayoutRule runat="server" Merge="False" ></px:PXLayoutRule>			
            <pxa:PXCurrencyRate DataField="CuryID" ID="edCury" runat="server" RateTypeView="currencyinfo" DataMember="_Currency_" DataSourceID="ds"></pxa:PXCurrencyRate>
			<px:PXLayoutRule runat="server" Merge="True" ></px:PXLayoutRule>
            <px:PXSelector CommitChanges="True" ID="edOwnerID" runat="server" DataField="OwnerID" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" AutoRefresh="true" ></px:PXSelector>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_offline&quot; >&lt;/Images>
      " runat="server" ID="edOwnerTeamsCardOffline" CommandSourceID="ds" CommandName="OwnerTeamsCardOffline" CssClass="Button teamsImageButton" Width="20" Height="24">
				<Images Normal="svg:teams@teams_offline" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_available&quot; >&lt;/Images>
      " runat="server" ID="edOwnerTeamsCardAvailable" CommandSourceID="ds" CommandName="OwnerTeamsCardAvailable" CssClass="Button teamsImageButton" Width="20" Height="24">
				<Images Normal="svg:teams@teams_available" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_busy&quot; >&lt;/Images>
      " runat="server" ID="edOwnerTeamsCardBusy" CommandSourceID="ds" CommandName="OwnerTeamsCardBusy" CssClass="Button teamsImageButton" Width="20" Height="24">
				<Images Normal="svg:teams@teams_busy" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@teams_away&quot; >&lt;/Images>
      " runat="server" ID="edOwnerTeamsCardAway" CommandSourceID="ds" CommandName="OwnerTeamsCardAway" CssClass="Button teamsImageButton" Width="20" Height="24">
				<Images Normal="svg:teams@teams_away" ></Images>
			</px:PXButton>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" ></px:PXLayoutRule>
            <px:PXCheckBox ID="chkManualTotalEntry" runat="server" DataField="ManualTotalEntry" CommitChanges="true"></px:PXCheckBox>
            <px:PXNumberEdit runat="server" ID="edCuryEstimateTotal" Enabled="False" DataField="AMCuryEstimateTotal" ></px:PXNumberEdit>
            <px:PXNumberEdit ID="edQuotedAmount" runat="server" DataField="QuotedAmount" CommitChanges="True" Visible="false"></px:PXNumberEdit>
            <px:PXNumberEdit CommitChanges="True" ID="edCuryAmount" runat="server" DataField="CuryAmount" ></px:PXNumberEdit>
            <px:PXNumberEdit CommitChanges="True" ID="edCuryLineDiscountTotal" runat="server" DataField="CuryLineDiscountTotal" ></px:PXNumberEdit>
            <px:PXNumberEdit CommitChanges="True" ID="edCuryDiscTot" runat="server" DataField="CuryDiscTot" ></px:PXNumberEdit>
            <px:PXNumberEdit CommitChanges="True" ID="edCuryTaxTotal" runat="server" DataField="CuryTaxTotal"  Enabled="False"></px:PXNumberEdit>
            <px:PXNumberEdit ID="edTotalAmount" runat="server" DataField="TotalAmount" CommitChanges="True" Visible="false"></px:PXNumberEdit> 
            <px:PXNumberEdit ID="edCuryProductsAmount" runat="server" DataField="CuryProductsAmount" Enabled="False"></px:PXNumberEdit>        
            <px:PXCheckBox ID="chkServiceManagement" runat="server" DataField="ChkServiceManagement" ></px:PXCheckBox>
	<px:PXLayoutRule runat="server" ID="STCstPXLayoutRulehp1" StartColumn="True" ControlSize="M" LabelsWidth="SM" />
	<px:PXTextEdit runat="server" ID="edUsrHubspotDealID" DataField="UsrHubspotDealID" TextMode="MultiLine" Height="60px" /></Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="280px" DataSourceID="ds" DataMember="OpportunityCurrent" OnDataBound="tab_DataBound">
        <Items>

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

            <px:PXTabItem Text="Details" Key="ProductsTab">
                <Template>
                    <px:PXGrid ID="ProductsGrid" SkinID="Details" runat="server" Width="100%" Height="500px" DataSourceID="ds" ActionsPosition="Top" BorderWidth="0px" SyncPosition="true">
                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
                        <Mode AllowUpload="True" AllowDragRows="true"/>
                        <CallbackCommands PasteCommand="PasteLine">
                            <Save PostData="Container" />
                        </CallbackCommands>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Add Matrix Item" CommandSourceID="ds" CommandName="ShowMatrixPanel" />
                                <px:PXToolBarButton Text="Insert Row" SyncText="false" ImageSet="main" ImageKey="AddNew">
																	<AutoCallBack Target="ProductsGrid" Command="AddNew" Argument="1"></AutoCallBack>
																	<ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Cut Row" SyncText="false" ImageSet="main" ImageKey="Copy">
																	<AutoCallBack Target="ProductsGrid" Command="Copy"></AutoCallBack>
																	<ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Insert Cut Row" SyncText="false" ImageSet="main" ImageKey="Paste">
																	<AutoCallBack Target="ProductsGrid" Command="Paste"></AutoCallBack>
																	<ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
								<px:PXToolBarButton Text="Configure" DependOnGrid="ProductsGrid" StateColumn="IsConfigurable">
									<AutoCallBack Command="ConfigureEntry" Target="ds" />
								</px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Products">
                                <Mode InitNewRow="true"/>
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edSubItemID" runat="server" DataField="SubItemID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="ProductsGrid" Name="CROpportunityProducts.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>                                                                    
									<px:PXSelector CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="True"/>
                                    <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="ProductsGrid" Name="CROpportunityProducts.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXSegmentMask ID="edVendorID" runat="server" DataField="VendorID" AllowEdit="true" CommitChanges="true" AutoRefresh="True">
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask ID="edVendorLocationID" runat="server" DataField="VendorLocationID" AllowEdit="true" AutoRefresh="true">
                                    </px:PXSegmentMask>
                                </RowTemplate>
                                <Columns>
									<px:PXGridColumn DataField="IsConfigurable" Type="CheckBox" TextAlign="Center" Width="90px" />
									<px:PXGridColumn DataField="AMParentLineNbr" TextAlign="Center" Width="85px" />
									<px:PXGridColumn DataField="AMIsSupplemental" Type="CheckBox" TextAlign="Center" Width="85px" />
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="CCCCCCCCCCCCCCCCCCCC" AutoCallBack="True" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-&gt;AA" />
                                    <px:PXGridColumn DataField="Descr" />
                                    <px:PXGridColumn DataField="IsFree" AllowNull="False" TextAlign="Center" Type="CheckBox" AutoCallBack="True"/>
                                    <px:PXGridColumn DataField="BillingRule" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="SiteID" DisplayFormat="&gt;AAAAAAAAAA" CommitChanges="true" />
                                    <px:PXGridColumn DataField="Quantity" TextAlign="Right" AutoCallBack="True" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="EstimatedDuration" AutoCallBack="True"/>
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" AutoCallBack="True" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="CuryUnitPrice" AllowNull="False" TextAlign="Right" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="CuryExtPrice" AllowNull="False" TextAlign="Right" AutoCallBack="True"  />
                                    <px:PXGridColumn DataField="DiscPct" AllowNull="False" TextAlign="Right" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="CuryDiscAmt" TextAlign="Right" AllowNull="False" AutoCallBack="True" />                                    
                                    <px:PXGridColumn DataField="CuryAmount" TextAlign="Right" AllowNull="False" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="ManualDisc" TextAlign="Center" AllowNull="False" AutoCallBack="True" Type="CheckBox" />                                    
                                    <px:PXGridColumn DataField="SkipLineDiscounts" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="DiscountID" TextAlign="Left" AutoCallBack="True" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" TextAlign="Left" />
                                    <px:PXGridColumn DataField="TaxCategoryID" DisplayFormat="&gt;aaaaaaaaaa" />
                                    <px:PXGridColumn DataField="ManualPrice" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" /> 
                                    <px:PXGridColumn DataField="TaskID" DisplayFormat="&gt;#####" RenderEditorText="true" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="CostCodeID" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="POCreate" TextAlign="Right" AutoCallBack="True" Type="CheckBox" />
                                    <px:PXGridColumn DataField="CuryUnitCost" TextAlign="Right" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="VendorID" AutoCallBack="True" />
                                    <px:PXGridColumn DataField="VendorLocationID" AutoCallBack="True"/>
                                    <px:PXGridColumn DataField="SortOrder" TextAlign="Right" />
                                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" />
									<px:PXGridColumn DataField="AMConfigKeyID" Width="90" CommitChanges="true" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Quotes" BindingContext="form" RepaintOnDemand="false">
            <Template>
                <px:PXGrid ID="formQuotes" runat="server" DataSourceID="ds" Width="100%" SkinID="Inquire" BorderStyle="None" SyncPosition="True">
                    <Levels>                        
                        <px:PXGridLevel DataMember="Quotes">  
                            <RowTemplate>
                                <px:PXCheckBox ID="chkIsPrimary" runat="server" DataField="IsPrimary">                                    
                                </px:PXCheckBox>                                     
                            </RowTemplate>                          
                            <Columns>
                                <px:PXGridColumn DataField="IsPrimary" TextAlign="Center" AllowNull="False" Type="CheckBox" LinkCommand="PrimaryQuote"/> 
                                <px:PXGridColumn DataField="QuoteNbr" LinkCommand="ViewQuote"  />
                                <px:PXGridColumn DataField="QuoteType" />
                                <px:PXGridColumn DataField="Subject" />                                
                                <px:PXGridColumn DataField="Status" RenderEditorText="True" />
                                <px:PXGridColumn DataField="DocumentDate" />
                                <px:PXGridColumn DataField="ExpirationDate" />
                                <px:PXGridColumn DataField="CuryID" />
								<px:PXGridColumn DataField="ManualTotalEntry" Type="CheckBox" TextAlign="Center" />
	                            <px:PXGridColumn DataField="CuryAmount" TextAlign="Right" />
                                <px:PXGridColumn DataField="CuryDiscTot" TextAlign="Right" />
                                <px:PXGridColumn DataField="CuryTaxTotal" TextAlign="Right" />
	                            <px:PXGridColumn DataField="CuryProductsAmount" TextAlign="Right" />
                                <px:PXGridColumn DataField="BAccountID" />
                                <px:PXGridColumn DataField="LocationID" />
                                <px:PXGridColumn DataField="ContactID" />
                                <px:PXGridColumn DataField="QuoteProjectID" LinkCommand="ViewProject" />
                                <px:PXGridColumn DataField="CuryCostTotal" />
                                <px:PXGridColumn DataField="CuryGrossMarginAmount" />
                                <px:PXGridColumn DataField="GrossMarginPct" />
                            </Columns>                            
                        </px:PXGridLevel>
                    </Levels>
                    <ActionBar PagerVisible="False" DefaultAction="ViewQuote" >
                        <CustomItems>                            
	                        <px:PXToolBarButton ImageKey="AddNew" Tooltip="Create Quote" DisplayStyle="Image">
		                        <AutoCallBack Command="CreateQuote" Target="ds" />		                        
	                        </px:PXToolBarButton>
                            <px:PXToolBarButton Text="Copy Quote" Key="CopyQuote">
	                            <AutoCallBack Target="ds" Command="CopyQuote"/>	                            
                            </px:PXToolBarButton>
	                        <px:PXToolBarButton Text="" Key="PrimaryQuote">
		                        <AutoCallBack Target="ds" Command="PrimaryQuote"/>		                        
	                        </px:PXToolBarButton>	                        
                        </CustomItems>
                    </ActionBar>
	                <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    <AutoSize Enabled="True" />
                </px:PXGrid>
            </Template>
        </px:PXTabItem>
			<px:PXTabItem Text="Estimates" BindingContext="form" RepaintOnDemand="false">
				<Template>
					<px:PXGrid runat="server" SyncPosition="True" Height="200px" SkinID="DetailsInTab" Width="100%" ID="gridEstimates" AutoCallBack="Refresh" DataSourceID="ds">
						<AutoSize Enabled="True" />
						<AutoCallBack Enabled="True" Target="gridEstimates" Command="Refresh" />
						<Levels>
							<px:PXGridLevel DataMember="OpportunityEstimateRecords" DataKeyNames="OpportunityID,EstimateID">
								<RowTemplate>
									<px:PXSelector runat="server" DataField="AMEstimateItem__BranchID" ID="edEstBranch" />
									<px:PXSelector runat="server" DataField="AMEstimateItem__InventoryCD" ID="edEstInventoryCD" />
									<px:PXTextEdit runat="server" DataField="AMEstimateItem__ItemDesc" ID="edEstItemDesc" />
									<px:PXSelector runat="server" DataField="AMEstimateItem__SiteID" ID="edEstSiteID" AutoRefresh="True" />
									<px:PXSelector runat="server" DataField="AMEstimateItem__UOM" ID="edEstUOM" AutoRefresh="True" />
									<px:PXNumberEdit runat="server" DataField="OrderQty" ID="edEstOrderQty" />
									<px:PXNumberEdit runat="server" DataField="CuryUnitPrice" ID="edEstCuryUnitPrice" />
									<px:PXNumberEdit runat="server" DataField="CuryExtPrice" ID="edEstCuryExtPrice" />
									<px:PXSelector runat="server" DataField="EstimateID" ID="edEstEstimateID" AutoRefresh="True" />
									<px:PXSelector runat="server" DataField="RevisionID" CommitChanges="True" ID="edEstRevisionID" AutoRefresh="True" />
									<px:PXSelector runat="server" DataField="TaxCategoryID" ID="edEstTaxCategoryID" />
									<px:PXSelector runat="server" DataField="AMEstimateItem__OwnerID" ID="edEstOwnerID" />
									<px:PXSelector runat="server" DataField="AMEstimateItem__EngineerID" ID="edEstEngineerID" AutoRefresh="True" />
									<px:PXDateTimeEdit runat="server" ID="edEstRequestDate" DataField="AMEstimateItem__RequestDate" />
									<px:PXDateTimeEdit runat="server" ID="edEstPromiseDate" DataField="AMEstimateItem__PromiseDate" />
									<px:PXSelector runat="server" DataField="AMEstimateItem__EstimateClassID" ID="edEstEstimateClassID" AutoRefresh="True" />
                                </RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="AMEstimateItem__BranchID" />
									<px:PXGridColumn DataField="AMEstimateItem__InventoryCD" />
									<px:PXGridColumn DataField="AMEstimateItem__ItemDesc" />
									<px:PXGridColumn DataField="AMEstimateItem__SiteID" />
									<px:PXGridColumn DataField="AMEstimateItem__UOM" />
									<px:PXGridColumn DataField="OrderQty" TextAlign="Right" />
									<px:PXGridColumn DataField="CuryUnitPrice" TextAlign="Right" />
									<px:PXGridColumn DataField="CuryExtPrice" TextAlign="Right" />
									<px:PXGridColumn DataField="EstimateID" LinkCommand="ViewEstimate" />
									<px:PXGridColumn DataField="RevisionID" />
									<px:PXGridColumn DataField="TaxCategoryID" />
									<px:PXGridColumn DataField="AMEstimateItem__OwnerID" />
									<px:PXGridColumn DataField="AMEstimateItem__EngineerID" />
									<px:PXGridColumn DataField="AMEstimateItem__RequestDate" />
									<px:PXGridColumn DataField="AMEstimateItem__PromiseDate" />
									<px:PXGridColumn DataField="AMEstimateItem__EstimateClassID" />
                               </Columns>
                            </px:PXGridLevel>
                        </Levels>
						<ActionBar>
							<CustomItems>
								<px:PXToolBarButton Text="Add" CommandSourceID="ds" CommandName="AddEstimate">
									<AutoCallBack>
										<Behavior PostData="Self" CommitChanges="True" RepaintControlsIDs="gridEstimates" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
								<px:PXToolBarButton Text="Quick Estimate" DependOnGrid="gridEstimates" StateColumn="EstimateID">
									<AutoCallBack Command="QuickEstimate" Target="ds" />
                                </px:PXToolBarButton>
								<px:PXToolBarButton Text="Remove" CommandSourceID="ds" CommandName="RemoveEstimate">
									<AutoCallBack>
										<Behavior PostData="Self" CommitChanges="True" RepaintControlsIDs="gridEstimates" />
									</AutoCallBack>
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Contact" RepaintOnDemand="False">
                <Template>
                    <px:PXFormView ID="edOpportunityCurrent" runat="server" DataMember="OpportunityCurrent" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXCheckBox ID="edAllowOverrideContactAddress" runat="server" DataField="AllowOverrideContactAddress" CommitChanges="true"/>
                        </Template>
                        <ContentStyle BackColor="Transparent"/>
                    </px:PXFormView>   

                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True" LabelsWidth="S" ControlSize="XM" />
                    <px:PXFormView ID="edOpportunity_Contact" runat="server" DataMember="Opportunity_Contact" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" GroupCaption="Contact" StartGroup="True"/>
        			        <px:PXTextEdit ID="edFirstName" runat="server" DataField="FirstName" CommitChanges="true"/>
        			        <px:PXTextEdit ID="edLastName" runat="server" DataField="LastName" SuppressLabel="False" CommitChanges="true"/>
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" CommitChanges="true"/>					        					        
        			        <px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" SuppressLabel="False" CommitChanges="true"/>
                            <px:PXMailEdit ID="edEMail" runat="server" CommandSourceID="ds" DataField="EMail" CommitChanges="True"/>
                            <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" ControlSize="SM" />
                            <px:PXDropDown ID="edPhone1Type" runat="server" DataField="Phone1Type" SuppressLabel="True" CommitChanges="True" Width="104px"/>
                            <px:PXLabel ID="LPhone1" runat="server" SuppressLabel="true" />
        			        <px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" LabelWidth="0px" Size="XM" LabelID="LPhone1" SuppressLabel="True" CommitChanges="true"/>
                            <px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" ControlSize="SM" SuppressLabel="True" />
                            <px:PXDropDown ID="edPhone2Type" runat="server" DataField="Phone2Type" SuppressLabel="True" CommitChanges="True" Width="104px"/>
                            <px:PXLabel ID="LPhone2" runat="server" SuppressLabel="true" />
        			        <px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" LabelWidth="0px" Size="XM" LabelID="LPhone2" SuppressLabel="True" CommitChanges="true"/>
                            <px:PXLayoutRule ID="PXLayoutRule12" runat="server" Merge="True" ControlSize="SM" SuppressLabel="True" />
                            <px:PXDropDown ID="edPhone3Type" runat="server" DataField="Phone3Type" SuppressLabel="True" CommitChanges="True" Width="104px"/>
                            <px:PXLabel ID="LPhone3" runat="server" SuppressLabel="true"  />
        			        <px:PXMaskEdit ID="edPhone3" runat="server" DataField="Phone3" LabelWidth="0px" Size="XM" SuppressLabel="True" LabelID="LPhone3"/>
                            <px:PXLayoutRule ID="PXLayoutRule5" runat="server" Merge="True" ControlSize="SM" SuppressLabel="True" />
                            <px:PXDropDown ID="edFaxType" runat="server" DataField="FaxType" SuppressLabel="True" CommitChanges="True" Width="104px" />
                            <px:PXLabel ID="LFax" runat="server" SuppressLabel="true" />
        			        <px:PXMaskEdit ID="edFax" runat="server" DataField="Fax" LabelWidth="0px" Size="XM" LabelID="LFax" SuppressLabel="True" CommitChanges="true"/>
        			        <px:PXLayoutRule runat="server" />
                            <px:PXLinkEdit ID="edWebSite" runat="server" DataField="WebSite" CommitChanges="True"/>

        			        <px:PXLayoutRule runat="server" />					
                       </Template>
                        <ContentStyle BackColor="Transparent" />
                    </px:PXFormView>
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />   
                    <px:PXFormView ID="edOpportunity_Address" runat="server" DataMember="Opportunity_Address" DataSourceID="ds" RenderStyle="Simple">
                        <Template>
                            <px:PXLayoutRule runat="server" GroupCaption="Address" StartGroup="True" />                            
                            <px:PXButton ID="btnAddressLookup" runat="server" CommandName="AddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
        					<px:PXButton ID="btnViewMainOnMap" runat="server" CommandName="ViewMainOnMap" CommandSourceID="ds" TabIndex="-1"
        						Size="xs" Text="View On Map" />
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" CommitChanges="true"/>
        					<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" CommitChanges="true"/>
        					<px:PXTextEdit ID="edCity" runat="server" DataField="City" CommitChanges="true"/>
                            <px:PXSelector ID="edState" runat="server" AutoRefresh="True" DataField="State" CommitChanges="true"
                                           FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" />
                            <px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" Size="S" CommitChanges="True" />
        					<px:PXSelector ID="edCountryID" runat="server" AllowEdit="True" DataField="CountryID"
        						FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" CommitChanges="True" />
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
                       </Template>                       
                    </px:PXFormView>
                    <px:PXLayoutRule ID="PXLayoutRule16" runat="server" GroupCaption="Personal Data Privacy" StartGroup="True" />
                         <px:PXFormView ID="edOpportunity_Contact_GDPR" runat="server" DataMember="Opportunity_Contact" DataSourceID="ds" RenderStyle="Simple" SkinID="Transparent">
                            <Template>
                                <px:PXLayoutRule ID="PXLayoutRule16" runat="server" ControlSize="M" LabelsWidth="M"/>
                                <px:PXCheckBox ID="PXCheckBox1" runat="server" DataField="ConsentAgreement" AlignLeft="True" CommitChanges="True"/>
                                <px:PXDateTimeEdit ID="edConsentDate" runat="server" DataField="ConsentDate" CommitChanges="True"/>
                                <px:PXDateTimeEdit ID="edConsentExpirationDate" runat="server" DataField="ConsentExpirationDate" CommitChanges="True"/>
                            </Template>
                            <ContentLayout OuterSpacing="None"/>
                            <ContentStyle BackColor="Transparent" BorderStyle="None"/>
                    </px:PXFormView>

                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="CRM Info">
                <Template>
                    <px:PXPanel ID="PXPanel1" runat="server" SkinID="transparent">
                        <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="CRM" />
                        <px:PXDropDown ID="edResolution" runat="server" AllowNull="False" DataField="Resolution" CommitChanges="True" />
                        <px:PXSelector DataField="WorkgroupID" ID="WorkgroupID" runat="server"  CommitChanges="True" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" />
                        <px:PXCheckBox ID="edOverrideSalesTerritory" runat="server" DataField="OverrideSalesTerritory" Size="XL" CommitChanges="True" />
                        <px:PXSelector ID="edSalesTerritoryID" runat="server" DataField="SalesTerritoryID" TextMode="Search" AutoRefresh="True" CommitChanges="true" />
                        <px:PXSegmentMask DataField="ParentBAccountID" ID="edParentBAccountID" runat="server" AllowEdit="True" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" AutoRefresh="True" />
                        <px:PXSelector DataField="LanguageID" ID="edLanguageID" runat="server"  CommitChanges="True" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" />
                        <px:PXCheckBox ID="edIsActive" runat="server" DataField="IsActive" />
                        <px:PXLayoutRule runat="server" EndGroup="True" />

                        <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" GroupCaption="Activities" />
                        <px:PXTextEdit ID="edLastIncomingActivity" runat="server" DataField="ActivityStatistics.LastIncomingActivityDate" CommitChanges="true" Enabled="False" AllowEdit="True" />
                        <px:PXTextEdit ID="edLastOutgoingActivity" runat="server" DataField="ActivityStatistics.LastOutgoingActivityDate" CommitChanges="true" Enabled="False" />
                        <px:PXLayoutRule runat="server" EndGroup="True"> 
                        </px:PXLayoutRule> 

                        <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Forecasting" />
                        <px:PXFormView ID="panelfordatamember" runat="server" DataMember="ProbabilityCurrent" DataSourceID="ds" RenderStyle="Simple">
                            <Template>
                                <px:PXLayoutRule ID="PXLayoutRule14" runat="server" SuppressLabel="False" LabelsWidth="SM" />
                                <px:PXNumberEdit ID="edProbability" runat="server" DataField="Probability" Enabled="False" Size="M" />
                            </Template>
                            <ContentStyle BackColor="Transparent" />
                        </px:PXFormView>
                        <px:PXNumberEdit ID="edCuryWgtAmount" runat="server" DataField="CuryWgtAmount" Enabled="False" Size="M"/>
                        <px:PXTextEdit ID="edClosingDate" runat="server" DataField="ClosingDate" CommitChanges="true" Enabled="False" />
                        <px:PXLayoutRule runat="server" EndGroup="True" />  

                        <px:PXLayoutRule ID="PXLayoutRule4" runat="server" LabelsWidth="SM" ControlSize="XM" GroupCaption="Source" />
                        <px:PXDropDown ID="edSource" runat="server" DataField="Source" CommitChanges="True" />
                        <px:PXSelector ID="edCampaignSourceID" runat="server" DataField="CampaignSourceID" AllowEdit="True" TextMode="Search" DisplayMode="Hint" FilterByAllFields="True" CommitChanges="true" AutoRefresh="True" />
                        <px:PXSelector ID="edLeadID" runat="server" DataField="LeadID" TextField="displayName" DisplayMode="Text" Enabled="False" AllowEdit="True" AutoRefresh="True" />
                        <px:PXLayoutRule runat="server" EndGroup="True"> 
                        </px:PXLayoutRule> 
                    </px:PXPanel>
                    <px:PXRichTextEdit ID="edDescription" runat="server" DataField="Details" Style="border-top: 1px solid #BBBBBB; border-left: 0px; border-right: 0px; margin: 0px;
                                                                                                      padding: 0px; width: 100%;" AllowAttached="true" AllowSearch="true" AllowMacros="true"  AllowLoadTemplate="false" AllowSourceMode="true">
                        <LoadTemplate TypeName="PX.SM.SMNotificationMaint" DataMember="Notifications" ViewName="NotificationTemplate" ValueField="notificationID" TextField="Name" DataSourceID="ds" Size="M"/>
                        <AutoSize Enabled="True" MinHeight="216" />
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
			<px:PXTabItem Text="Financial">
                <Template>
					<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True" />
					<px:PXFormView ID="formA" runat="server" DataMember="Billing_Address" DataSourceID="ds" RenderStyle="Simple">
						<Template>
							<px:PXLayoutRule runat="server" StartGroup="True" ControlSize="XM" LabelsWidth="SM" GroupCaption="Bill-To Address" />
							<px:PXCheckBox ID="edOverrideAddress" runat="server" Size="SM" DataField="OverrideAddress" CommitChanges="true" />
							<px:PXButton ID="btnAddressLookupBilling"  runat="server" CommandName="BillingAddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
							<px:PXButton Size="xs" ID="btnViewMainOnMap" runat="server" CommandName="ViewBillingOnMap" CommandSourceID="ds" Text="View on Map" TabIndex="-1" />
							<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
							<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2"/>
							<px:PXTextEdit ID="edCity" runat="server" DataField="City" />
							<px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds" CommitChanges="True">
								<CallBackMode PostData="Container" />
								<Parameters>
									<px:PXControlParam ControlID="formA" Name="CRBillingAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
										Type="String" />
								</Parameters>
							</px:PXSelector>
							<px:PXMaskEdit CommitChanges="True" ID="edPostalCode" runat="server" DataField="PostalCode" Size="s" />
							<px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
							<px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" /> 
							<px:PXLayoutRule runat="server" EndGroup="True" /> 
						</Template>
						<ContentStyle BackColor="Transparent" BorderStyle="None"/>
					</px:PXFormView>
					<px:PXFormView ID="formB" DataMember="Billing_Contact" runat="server" DataSourceID="ds"  RenderStyle="Simple" SyncPosition="True">
                        <Template>
							<px:PXLayoutRule runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Bill-To Info" />
							<px:PXCheckBox ID="edOverrideContact" runat="server" Size="SM" DataField="OverrideContact" CommitChanges="true" />
							<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName"/>
							<px:PXTextEdit ID="edAttention" runat="server" DataField="Attention"/>
                            <px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" ControlSize="SM" />
							<px:PXDropDown ID="edPhone1Type" runat="server" DataField="Phone1Type" SuppressLabel="True" Width="134px"/>
                            <px:PXLabel ID="LPhone1" runat="server" SuppressLabel="true" />
							<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" LabelWidth="0px" Size="XM" LabelID="LPhone1" SuppressLabel="True" />
                            <px:PXLayoutRule runat="server" />
                            <px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" ControlSize="SM" SuppressLabel="True" />
							<px:PXDropDown ID="edPhone2Type" runat="server" DataField="Phone2Type" SuppressLabel="True" Width="134px"/>
                            <px:PXLabel ID="LPhone2" runat="server" SuppressLabel="true" />
							<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" LabelWidth="0px" Size="XM" LabelID="LPhone2" SuppressLabel="True" />
                            <px:PXLayoutRule runat="server" />
							<px:PXMailEdit ID="edEmail" runat="server" DataField="Email" />
							<px:PXLayoutRule runat="server" EndGroup="True" />
						</Template>
						<ContentStyle BackColor="Transparent" BorderStyle="None"/>
					</px:PXFormView>
					<px:PXLayoutRule runat="server" StartColumn="True"/>
					<px:PXFormView ID="formD" runat="server" DataMember="OpportunityCurrent" DataSourceID="ds" RenderStyle="Simple" SyncPosition="True">
						<Template>
							<px:PXLayoutRule runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Financial Settings" />
							<px:PXSegmentMask ID="edBranchID" runat="server" DataField="BranchID" CommitChanges="True" AutoRefresh="True" />
							<px:PXSelector ID="edTermsID" runat="server" DataField="TermsID" CommitChanges="true"/>
                            <px:PXLayoutRule runat="server" EndGroup="True" />
                        </Template>
						<ContentStyle BackColor="Transparent" BorderStyle="None"/>
					</px:PXFormView>
					<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
					<px:PXFormView ID="formE" runat="server" DataMember="OpportunityCurrent" DataSourceID="ds" RenderStyle="Simple" SyncPosition="True">
						<Template>
							<px:PXLayoutRule runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Other Settings" />  
							<px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID" CommitChanges="True" AutoRefresh="True" AllowAddNew="True" />
							<px:PXTextEdit ID="edExternalRef" runat="server" DataField="ExternalRef"  CommitChanges="true" />
						</Template>
						<ContentStyle BackColor="Transparent" BorderStyle="None"/>
                    </px:PXFormView>
				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Shipping">
                <Template>
                    <%-- column 1 --%>

                    					<px:PXFormView ID="formA" runat="server" DataMember="Shipping_Address" DataSourceID="ds" RenderStyle="Simple">
						<Template>
							<px:PXLayoutRule runat="server" StartGroup="True" ControlSize="XM" LabelsWidth="SM" GroupCaption="Ship-To Address" />
							<px:PXCheckBox ID="edOverrideAddress" runat="server" Size="SM" DataField="OverrideAddress" CommitChanges="true" />
							<px:PXButton ID="btnAddressLookupShipping"  runat="server" CommandName="ShippingAddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
							<px:PXButton Size="xs" ID="btnViewMainOnMap" runat="server" CommandName="ViewShippingOnMap" CommandSourceID="ds" Text="View on Map" TabIndex="-1" />
							<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
							<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2"/>
							<px:PXTextEdit ID="edCity" runat="server" DataField="City" />
							<px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds" CommitChanges="True">
								<CallBackMode PostData="Container" />
								<Parameters>
									<px:PXControlParam ControlID="formA" Name="CRShippingAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
										Type="String" />
								</Parameters>
							</px:PXSelector>
							<px:PXMaskEdit CommitChanges="True" ID="edPostalCode" runat="server" DataField="PostalCode" Size="s" />
							<px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
							<px:PXNumberEdit ID="edLatitude" runat="server" DataField="Latitude" AllowNull="True" />
							<px:PXNumberEdit ID="edLongitude" runat="server" DataField="Longitude" AllowNull="True" />
							<px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" /> 
							<px:PXLayoutRule runat="server" EndGroup="True" /> 
						</Template>
						<ContentStyle BackColor="Transparent" BorderStyle="None"/>
                    </px:PXFormView>
					<px:PXFormView ID="formB" DataMember="Shipping_Contact" runat="server" DataSourceID="ds"  RenderStyle="Simple" SyncPosition="True">
						<Template>
							<px:PXLayoutRule runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Ship-To Info" />
							<px:PXCheckBox ID="edOverrideContact" runat="server" Size="SM" DataField="OverrideContact" CommitChanges="true" />
							<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName"/>
							<px:PXTextEdit ID="edAttention" runat="server" DataField="Attention"/>
							<px:PXLayoutRule ID="PXLayoutRule2" runat="server" Merge="True" ControlSize="SM" />
							<px:PXDropDown ID="edPhone1Type" runat="server" DataField="Phone1Type" SuppressLabel="True" Width="134px"/>
							<px:PXLabel ID="LPhone1" runat="server" SuppressLabel="true" />
							<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" LabelWidth="0px" Size="XM" LabelID="LPhone1" SuppressLabel="True" />
							<px:PXLayoutRule runat="server" />
							<px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" ControlSize="SM" SuppressLabel="True" />
							<px:PXDropDown ID="edPhone2Type" runat="server" DataField="Phone2Type" SuppressLabel="True" Width="134px"/>
							<px:PXLabel ID="LPhone2" runat="server" SuppressLabel="true" />
							<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" LabelWidth="0px" Size="XM" LabelID="LPhone2" SuppressLabel="True" />
                    <px:PXLayoutRule runat="server"  />
							<px:PXMailEdit ID="edEmail" runat="server" DataField="Email" />
							<px:PXLayoutRule runat="server" EndGroup="True" />
						</Template>
						<ContentStyle BackColor="Transparent" BorderStyle="None"/>
					</px:PXFormView>

                    <%-- column 2 --%>

                    <px:PXLayoutRule runat="server" StartColumn="True" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Tax Settings" LabelsWidth="SM" ControlSize="XM" />
                        <px:PXTextEdit ID="edTaxRegistrationID" runat="server" DataField="TaxRegistrationID" />
                        <px:PXSelector ID="edTaxZoneID" runat="server" DataField="TaxZoneID" CommitChanges="true"/>
                        <px:PXDropDown ID="edTaxCalcMode" runat="server" DataField="TaxCalcMode" CommitChanges="true"/>
                        <px:PXTextEdit ID="edExternalTaxExemptionNumber" runat="server" DataField="ExternalTaxExemptionNumber" />
                        <px:PXDropDown ID="edAvalaraCustomerUsageType" runat="server" DataField="AvalaraCustomerUsageType" />
                    <px:PXLayoutRule runat="server" EndGroup="True" />


                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Shipping Instructions" />
                    <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                        <px:PXSegmentMask ID="edSiteID" runat="server" DataField="SiteID" AllowEdit="True" CommitChanges="True" AutoRefresh="True" />
                        <px:PXSelector CommitChanges="True" ID="edCarrierID" runat="server" DataField="CarrierID" AllowEdit="True" />
                        <px:PXSelector ID="edShipTermsID" runat="server" DataField="ShipTermsID" AllowEdit="True" />
                        <px:PXSelector ID="edShipZoneID" runat="server" DataField="ShipZoneID" AllowEdit="True" />
                        <px:PXSelector ID="edFOBPointID" runat="server" DataField="FOBPointID" AllowEdit="True" />
                        <px:PXCheckBox ID="chkResedential" runat="server" DataField="Resedential" TabIndex="-1" />
                        <px:PXCheckBox ID="chkSaturdayDelivery" runat="server" DataField="SaturdayDelivery" TabIndex="-1" />
                        <px:PXCheckBox ID="chkInsurance" runat="server" DataField="Insurance" TabIndex="-1" />
                        <px:PXDropDown ID="edShipComplete" runat="server" DataField="ShipComplete" />
                    <px:PXLayoutRule runat="server"  EndGroup="True"  />

                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Attributes">
                <Template>
                    <px:PXGrid ID="PXGridAnswers" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%" Height="200px" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Answers">
                                <Columns>
                                    <px:PXGridColumn DataField="isRequired" TextAlign="Center" Type="CheckBox" AllowResize="True" />
                                    <px:PXGridColumn DataField="AttributeID" TextAlign="Left" AllowShowHide="False" TextField="AttributeID_description" />
                                    <px:PXGridColumn DataField="Value" AllowShowHide="False" AllowSort="False" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="200" />
                        <ActionBar>
                            <Actions>
                                <Search Enabled="False" />
                            </Actions>
                        </ActionBar>
                        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>

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

            <px:PXTabItem Text="Taxes" LoadOnDemand="true" Key="TaxDetailsTab">
                <Template>
                    <px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" SkinID="Details" ActionsPosition="Top" BorderWidth="0px">
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Levels>
                            <px:PXGridLevel DataMember="Taxes">
                                <Columns>
                                    <px:PXGridColumn DataField="TaxID" AllowUpdate="False" CommitChanges="true" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="TaxRate" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxableAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryExemptedAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="TaxUOM" TextAlign="Right" />
									<px:PXGridColumn AllowNull="False" DataField="TaxableQty" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__TaxType" Label="Tax Type" RenderEditorText="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__PendingTax" Label="Pending VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ReverseTax" Label="Reverse VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ExemptTax" Label="Exempt From VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__StatisticalTax" Label="Statistical VAT" TextAlign="Center" Type="CheckBox" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Discounts" BindingContext="form" RepaintOnDemand="false" Key="DiscountDetailsTab">
                <Template>
                    <px:PXGrid ID="formDiscountDetail" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" BorderStyle="None" SyncPosition="True">
                        <Levels>
                            <px:PXGridLevel DataMember="DiscountDetails">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXCheckBox ID="chkSkipDiscount" runat="server" DataField="SkipDiscount" />
                                    <px:PXSelector ID="edDiscountID" runat="server" DataField="DiscountID"
                                        AllowEdit="True" edit="1" />
                                    <px:PXSelector ID="edDiscountSequenceID" runat="server" DataField="DiscountSequenceID" AllowEdit="True" AutoRefresh="True" edit="1" />
                                    <px:PXDropDown ID="edType" runat="server" DataField="Type" Enabled="False" />
                                    <px:PXCheckBox ID="chkIsManual" runat="server" DataField="IsManual" />
                                    <px:PXNumberEdit ID="edCuryDiscountableAmt" runat="server" DataField="CuryDiscountableAmt" />
                                    <px:PXNumberEdit ID="edDiscountableQty" runat="server" DataField="DiscountableQty" />
                                    <px:PXNumberEdit ID="edCuryDiscountAmt" runat="server" DataField="CuryDiscountAmt" CommitChanges="true" />
                                    <px:PXNumberEdit ID="edDiscountPct" runat="server" DataField="DiscountPct" CommitChanges="true" />
                                    <px:PXSegmentMask ID="edFreeItemID" runat="server" DataField="FreeItemID" AllowEdit="True" />
                                    <px:PXNumberEdit ID="edFreeItemQty" runat="server" DataField="FreeItemQty" />
                                    <px:PXTextEdit ID="edExtDiscCode" runat="server" DataField="ExtDiscCode" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SkipDiscount" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="DiscountID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="Type" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="IsManual" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="CuryDiscountableAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="DiscountableQty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryDiscountAmt" TextAlign="Right" CommitChanges="true" />
                                    <px:PXGridColumn DataField="DiscountPct" TextAlign="Right" CommitChanges="true" />
                                    <px:PXGridColumn DataField="FreeItemID" DisplayFormat="&gt;CCCCC-CCCCCCCCCCCCCCC" />
                                    <px:PXGridColumn DataField="FreeItemQty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="ExtDiscCode" />
                                    <px:PXGridColumn DataField="Description" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
			<px:PXTabItem Text="Sync Status">
				<Template>
					<px:PXGrid ID="syncGrid" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" SkinID="Inquire" SyncPosition="true">
						<Levels>
							<px:PXGridLevel DataMember="SyncRecs" DataKeyNames="SyncRecordID">
								<Columns>
									<px:PXGridColumn DataField="SYProvider__Name" />
									<px:PXGridColumn DataField="RemoteID" CommitChanges="True" LinkCommand="GoToSalesforce" />
									<px:PXGridColumn DataField="Status" />
									<px:PXGridColumn DataField="Operation" />
									<px:PXGridColumn DataField="LastErrorMessage" />
									<px:PXGridColumn DataField="LastAttemptTS" DisplayFormat="g" />
									<px:PXGridColumn DataField="AttemptCount" />
									<px:PXGridColumn DataField="SFEntitySetup__ImportScenario" />
									<px:PXGridColumn DataField="SFEntitySetup__ExportScenario" />
								</Columns>                               
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<ActionBar>                        
							<CustomItems>
								<px:PXToolBarButton Key="SyncSalesforce">
									<AutoCallBack Command="SyncSalesforce" Target="ds"/>
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode InitNewRow="true" />
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>			
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="250" MinWidth="300" />
    </px:PXTab>
</asp:Content>
<asp:Content ID="Dialogs" ContentPlaceHolderID="phDialogs" runat="Server">

    <px:PXSmartPanel ID="panelCreateServiceOrder" runat="server" Style="z-index: 108; left: 27px; position: absolute; top: 99px;" Caption="Create Service Order" Width="380px" Height="130px" AutoRepaint="true"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="CreateServiceOrderFilter" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
        <px:PXFormView ID="formCreateServiceOrder" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False" DataMember="CreateServiceOrderFilter">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
                <px:PXSelector ID="edSrvOrdType" runat="server" AllowNull="False" DataField="SrvOrdType" DisplayMode="Text" CommitChanges="True" AutoRefresh="True" AllowEdit="True" />
                <px:PXSelector ID="edBranchLocationID" runat="server" AllowNull="False" DataField="BranchLocationID" DisplayMode="Text" CommitChanges="True" AutoRefresh="True" AllowEdit="True"/>
            </Template>
        </px:PXFormView>

        <div style="padding: 5px; text-align: right;">
                <px:PXButton ID="btnSave2" runat="server" CommitChanges="True" DialogResult="OK" Text="OK" Height="20"/>
                <px:PXButton ID="btnCancel2" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
        </div>
    </px:PXSmartPanel>

     <px:PXSmartPanel ID="PanelCopyQuote" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" Caption="Copy Quote"
        CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true" Key="CopyQuoteInfo" AutoCallBack-Enabled="true" AutoCallBack-Target="formCopyQuote" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" AcceptButtonID="PXButton8" CancelButtonID="PXButton9">
        <px:PXFormView ID="formCopyQuote" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Services Settings" CaptionVisible="False" SkinID="Transparent"
            DataMember="CopyQuoteInfo">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector ID="edOpportunityId" runat="server" DataField="OpportunityId" CommitChanges="true" AutoRefresh="True" />
                <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" CommitChanges="true"/>                
                <px:PXCheckBox ID="edRecalculatePrices" runat="server" DataField="RecalculatePrices" CommitChanges="true"/>
                <px:PXCheckBox ID="edOverrideManualPrices" runat="server" DataField="OverrideManualPrices" CommitChanges="true" Style="margin-left: 25px"/>                
                <px:PXCheckBox ID="edRecalculateDiscounts" runat="server" DataField="RecalculateDiscounts" CommitChanges="true"/>
                <px:PXCheckBox ID="edOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" CommitChanges="true" Style="margin-left: 25px"/>
                <px:PXCheckBox ID="edOverrideManualDocGroupDiscounts" runat="server" DataField="OverrideManualDocGroupDiscounts" CommitChanges="true" Style="margin-left: 25px"/>   
            </Template>
        </px:PXFormView>
        <div style="padding: 5px; text-align: right;">
            <px:PXButton ID="PXButton8" runat="server" Text="OK" DialogResult="Yes" Width="63px" Height="20px"></px:PXButton>
            <px:PXButton ID="PXButton9" runat="server" DialogResult="No" Text="Cancel" Width="63px" Height="20px" Style="margin-left: 5px" />
        </div>
    </px:PXSmartPanel> 

    <px:PXSmartPanel ID="PanelRecalculate" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" Caption="Recalculate Prices"
        CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true" Key="recalcdiscountsfilter" AutoCallBack-Enabled="true" AutoCallBack-Target="formRecalculate" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" AcceptButtonID="PXButton4" CancelButtonID="PXButton7">
        <px:PXFormView ID="formRecalculate" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Services Settings" CaptionVisible="False" SkinID="Transparent"
            DataMember="recalcdiscountsfilter">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                    <px:PXDropDown ID="edRecalcTerget" runat="server" DataField="RecalcTarget" CommitChanges="true" />
                    <px:PXCheckBox ID="edRecalculatePrices" runat="server" DataField="RecalcUnitPrices" CommitChanges="true"/>
                    <px:PXCheckBox ID="edOverrideManualPrices" runat="server" DataField="OverrideManualPrices" CommitChanges="true" Style="margin-left: 25px"/>                
                    <px:PXCheckBox ID="edRecalculateDiscounts" runat="server" DataField="RecalcDiscounts" CommitChanges="true"/>
                    <px:PXCheckBox ID="edOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" CommitChanges="true" Style="margin-left: 25px"/>
                    <px:PXCheckBox ID="edOverrideManualDocGroupDiscounts" runat="server" DataField="OverrideManualDocGroupDiscounts" CommitChanges="true" Style="margin-left: 25px"/> 
                </Template>
            </px:PXFormView>        
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
             <div style="padding: 5px; text-align: right;">
            <px:PXButton ID="PXButton4" runat="server" Text="OK" DialogResult="OK" Width="63px" Height="20px"></px:PXButton>
            <px:PXButton ID="PXButton7" runat="server" DialogResult="No" Text="Cancel" Width="63px" Height="20px" Style="margin-left: 5px" />
        </div>
        </px:PXPanel>
    </px:PXSmartPanel>

	<px:PXSmartPanel ID="PanelCreateContact" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" Caption="Create Contact"
	CaptionVisible="True" LoadOnDemand="true" AutoReload="true" Key="ContactInfo" AutoCallBack-Enabled="true" AutoCallBack-Target="formCreateContact" AutoCallBack-Command="Refresh" CloseButtonDialogResult="Abort"
	Width="520px" Height="314px" AllowResize="False">
	
	<px:PXTab ID="tabCreateContact" runat="server" Width="100%" DataSourceID="ds" DataMember="ContactInfo">
	
		<Items>

			<px:PXTabItem Text="Main" RepaintOnDemand="True">
				<Template>
					<px:PXFormView ID="formCreateContact" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="242px" Caption="Creation Dialog" CaptionVisible="False" SkinID="Transparent"
									DataMember="ContactInfo">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
							<px:PXTextEdit ID="edFirstName" runat="server" DataField="FirstName" CommitChanges="True" TabIndex="10"/>
							<px:PXTextEdit ID="edLastName" runat="server" DataField="LastName" CommitChanges="True" TabIndex="20"/>
							<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" CommitChanges="True" TabIndex="30"/>
							<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" CommitChanges="True" TabIndex="40"/>
							<px:PXLayoutRule runat="server" Merge="True" />
								<px:PXDropDown ID="edPhone1Type" runat="server" DataField="Phone1Type" Size="S" SuppressLabel="True" CommitChanges="True" TabIndex="-1"/>
								<px:PXLabel ID="lblPhone1" runat="server" Text=" " SuppressLabel="true" />
								<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" SuppressLabel="True" LabelWidth="84px" CommitChanges="True" TabIndex="50"/>
							<px:PXLayoutRule runat="server" />
							<px:PXLayoutRule runat="server" Merge="True" />
								<px:PXDropDown ID="edPhone2Type" runat="server" DataField="Phone2Type" Size="S" SuppressLabel="True" CommitChanges="True" TabIndex="-1"/>
								<px:PXLabel ID="lblPhone2" runat="server" Text=" " SuppressLabel="true" />
								<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" SuppressLabel="True" LabelWidth="84px" CommitChanges="True" TabIndex="60"/>
							<px:PXLayoutRule runat="server" />
							<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True" TabIndex="70"/>
							<px:PXSelector ID="edContactClass" runat="server" DataField="ContactClass" CommitChanges="True" TabIndex="80" AutoRefresh="True"/>
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Attributes" RepaintOnDemand="False">
				<Template>
					<px:PXGrid ID="grdContactInfoAttributes" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="ContactInfoAttributes">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="User-Defined Fields" RepaintOnDemand="false">
				<Template>
					<px:PXGrid ID="grdContactInfoUDF" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="ContactInfoUDF">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

		</Items>

	</px:PXTab>

	<px:PXPanel ID="CreateContactBtn" runat="server" SkinID="Transparent">
		<px:PXButton ID="CreateContactBtnBack" runat="server" CommandSourceID="ds" CallbackUpdatable="true"
		             style="float: left; margin: 2px" Text="Back" DialogResult="No" Visible="false"/>
		
		<px:PXButton ID="CreateContactBtnCancel" runat="server" CommandName="CreateContactCancel"
		             style="float: right; margin: 2px" Text="Cancel" DialogResult="Abort"/>

		<px:PXButton ID="CreateContactBtnConvert" runat="server" CommandSourceID="ds" CommandName="CreateContactFinish"
		             style="float: right; margin: 2px" Text="Create" DialogResult="OK"/>
		<px:PXButton ID="CreateContactBtnReview" runat="server"  CommandSourceID="ds" CommandName="CreateContactFinishRedirect"
		             style="float: right; margin: 2px" Text="Create and review" DialogResult="Yes"/>
	</px:PXPanel>

</px:PXSmartPanel>

	<px:PXSmartPanel ID="PanelCreateBothContactAndAccount" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" Caption="Create Account"
	CaptionVisible="True" LoadOnDemand="true" Key="AccountInfo" AutoCallBack-Enabled="true" AutoCallBack-Target="formCreateContactForBoth" AutoCallBack-Command="Refresh"
	AcceptButtonID="PXButtonOK" CancelButtonID="PXButtonCancel" AutoReload="True" AllowResize="False">

	<px:PXTab ID="tabCreateAccount" runat="server" Width="100%" DataSourceID="ds" DataMember="ContactInfo">
	
		<Items>

			<px:PXTabItem Text="Main" RepaintOnDemand="True">
				<Template>

					<px:PXLayoutRule runat="server" GroupCaption="Business Account"/>
					<px:PXFormView ID="formCreateAccountForBoth" runat="server" DataSourceID="ds" Style="z-index: 100" Width="500px" Caption="Creation Dialog" CaptionVisible="False" SkinID="Transparent"
									DataMember="AccountInfo">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
							<px:PXMaskEdit ID="edBAccountID" runat="server" DataField="BAccountID" CommitChanges="True" TabIndex="10"/>
							<px:PXTextEdit ID="edAccountName" runat="server" DataField="AccountName" CommitChanges="True" TabIndex="20"/>
							<px:PXSelector ID="edAccountClass" runat="server" DataField="AccountClass" CommitChanges="True" TabIndex="30" AutoRefresh="True"/>
							<px:PXCheckBox ID="edLinkContactToAccount" runat="server" DataField="LinkContactToAccount" CommitChanges="true" TabIndex="40"/>
						</Template>
					</px:PXFormView>

					<px:PXLayoutRule runat="server" GroupCaption="Contact"/>
					<px:PXFormView ID="formCreateContactForBoth" runat="server" DataSourceID="ds" Style="z-index: 100" Width="500px" Caption="Creation Dialog" CaptionVisible="False" SkinID="Transparent"
									DataMember="ContactInfo">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
							<px:PXTextEdit ID="edFirstName" runat="server" DataField="FirstName" CommitChanges="True" TabIndex="50"/>
							<px:PXTextEdit ID="edLastName" runat="server" DataField="LastName" CommitChanges="True" TabIndex="60"/>
							<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" CommitChanges="True" TabIndex="70"/>
							<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" CommitChanges="True" TabIndex="80"/>
							<px:PXLayoutRule runat="server" Merge="True" />
								<px:PXDropDown ID="edPhone1Type" runat="server" DataField="Phone1Type" Size="S" SuppressLabel="True" CommitChanges="True" TabIndex="-1"/>
								<px:PXLabel ID="lblPhone1" runat="server" Text=" " SuppressLabel="true" />
								<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" SuppressLabel="True" LabelWidth="84px" CommitChanges="True" TabIndex="90"/>
							<px:PXLayoutRule runat="server" />
							<px:PXLayoutRule runat="server" Merge="True" />
								<px:PXDropDown ID="edPhone2Type" runat="server" DataField="Phone2Type" Size="S" SuppressLabel="True" CommitChanges="True" TabIndex="-1"/>
								<px:PXLabel ID="lblPhone2" runat="server" Text=" " SuppressLabel="true" />
								<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" SuppressLabel="True" LabelWidth="84px" CommitChanges="True" TabIndex="100"/>
							<px:PXLayoutRule runat="server" />
							<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True" TabIndex="110"/>
							<px:PXSelector ID="edContactClass" runat="server" DataField="ContactClass" CommitChanges="True" TabIndex="120" AutoRefresh="True"/>
						</Template>
					</px:PXFormView>

				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Attributes" RepaintOnDemand="False">
				<Template>

					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Business Account"/>
						<px:PXGrid ID="grdAccountInfoAttributesBoth" runat="server" Width="500px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
							<Levels>
								<px:PXGridLevel DataMember="AccountInfoAttributes">
									<Columns>
										<px:PXGridColumn DataField="DisplayName" Width="250px"/>
										<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="250px"/>
									</Columns>
									<Layout ColumnsMenu="False" />
									<Mode AllowAddNew="false" AllowDelete="false" />
								</px:PXGridLevel>
							</Levels>
							<Mode InitNewRow="true" />
							<AutoSize Enabled="True" MinHeight="150" />
						</px:PXGrid>
					<px:PXLayoutRule runat="server" />

					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Contact"/>
						<px:PXGrid ID="grdContactInfoAttributesBoth" runat="server" Width="500px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
							<Levels>
								<px:PXGridLevel DataMember="ContactInfoAttributes">
									<Columns>
										<px:PXGridColumn DataField="DisplayName" Width="250px"/>
										<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="250px"/>
									</Columns>
									<Layout ColumnsMenu="False" />
									<Mode AllowAddNew="false" AllowDelete="false" />
								</px:PXGridLevel>
							</Levels>
							<Mode InitNewRow="true" />
							<AutoSize Enabled="True" MinHeight="150" />
						</px:PXGrid>
					<px:PXLayoutRule runat="server" />

				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="User-Defined Fields" RepaintOnDemand="False">
				<Template>

					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Business Account"/>
						<px:PXGrid ID="grdAccountInfoUDFBoth" runat="server" Width="500px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
							<Levels>
								<px:PXGridLevel DataMember="AccountInfoUDF">
									<Columns>
										<px:PXGridColumn DataField="DisplayName" Width="250px"/>
										<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="250px"/>
									</Columns>
									<Layout ColumnsMenu="False" />
									<Mode AllowAddNew="false" AllowDelete="false" />
								</px:PXGridLevel>
							</Levels>
							<Mode InitNewRow="true" />
							<AutoSize Enabled="True" MinHeight="150" />
						</px:PXGrid>
					<px:PXLayoutRule runat="server" />

					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Contact"/>
						<px:PXGrid ID="grdContactInfoUDFBoth" runat="server" Width="500px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
							<Levels>
								<px:PXGridLevel DataMember="ContactInfoUDF">
									<Columns>
										<px:PXGridColumn DataField="DisplayName" Width="250px"/>
										<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="250px"/>
									</Columns>
									<Layout ColumnsMenu="False" />
									<Mode AllowAddNew="false" AllowDelete="false" />
								</px:PXGridLevel>
							</Levels>
							<Mode InitNewRow="true" />
							<AutoSize Enabled="True" MinHeight="150" />
						</px:PXGrid>
					<px:PXLayoutRule runat="server" />

				</Template>
			</px:PXTabItem>

		</Items>

	</px:PXTab>


	<px:PXPanel ID="CreateContactAndAccountBtn" runat="server" SkinID="Buttons">
		<px:PXButton ID="CreateContactAndAccountBtnReview" runat="server" Text="Create and review" DialogResult="Yes"></px:PXButton>
		<px:PXButton ID="CreateContactAndAccountBtnConvert" runat="server" Text="Create" DialogResult="OK"></px:PXButton>
		<px:PXButton ID="CreateContactAndAccountBtnCancel" runat="server" DialogResult="Cancel" Text="Cancel" Style="margin-left: 5px"/>
	</px:PXPanel>
</px:PXSmartPanel>
	
	<px:PXSmartPanel ID="panelTeamsContact" runat="server"  Caption="Teams Contact" Width="400px" Height="290px" AutoRepaint="true"
	CaptionVisible="true" Key="TeamsContactCard" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" 
	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
	<px:PXFormView ID="formTeamsContact" runat="server" DataSourceID="ds" DataMember="TeamsContactCard" AllowCollapse="false" Width="100%" >
		<ContentStyle BackColor="Transparent" BorderStyle="None" />
		<Template>
			<px:PXImageView ID="edTeamsMemberPhoto" runat="server" DataField="PhotoFileName" Width="64" Height="64" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" />
			<px:PXTextEdit ID="edDisplayName" runat="server" DataField="DisplayName" SuppressLabel="True" Width="190px" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="XXS" ControlSize="XXS" />
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@offline&quot; />
      " ID="edStatusIconContactOffline" runat="server" CommandSourceID="ds" CommandName="StatusIconContactOffline" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@offline" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@available&quot; />
      " ID="edStatusIconContactAvailable" runat="server" CommandSourceID="ds" CommandName="StatusIconContactAvailable" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@available" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@busy&quot; />
      " ID="edStatusIconContactBusy" runat="server" CommandSourceID="ds" CommandName="StatusIconContactBusy" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@busy" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@away&quot; />
      " ID="edStatusIconContactAway" runat="server" CommandSourceID="ds" CommandName="StatusIconContactAway" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@away" ></Images>
			</px:PXButton>
			<px:PXTextEdit ID="edStatus" runat="server" DataField="TeamsStatus"  ControlSize="M" CssClass="teamsStatusLabel" SuppressLabel="True" Width="132px" />
			<px:PXLayoutRule runat="server" Merge="False" StartRow="True" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="edJobTitle" runat="server" DataField="JobTitle" Style="margin-top:20px" />
			<px:PXTextEdit ID="edCompanyName" runat="server" DataField="CompanyName" />
			<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" />
			<px:PXTextEdit ID="edMobilePhone" runat="server" DataField="MobilePhone" />
		</Template>
	</px:PXFormView>

	<div style="padding-left:74px; text-align: center;">
		<px:PXButton Text="
      &lt;Images Normal=&quot;svg:teams@teams_chat&quot; />
    " ID="edContactChat" runat="server" CommandSourceID="ds" CommandName="ContactChat" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Chat">
			<Images Normal="svg:teams@teams_chat" ></Images>
		</px:PXButton>
		<px:PXButton Text="
      &lt;Images Normal=&quot;svg:teams@teams_call&quot; />
    " ID="edContactCall" runat="server" CommandSourceID="ds" CommandName="ContactCall" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Call">
			<Images Normal="svg:teams@teams_call" ></Images>
		</px:PXButton>
		<px:PXButton Text="
      &lt;Images Normal=&quot;svg:teams@teams_event&quot; />
    " ID="edContactMeeting" runat="server" CommandSourceID="ds" CommandName="ContactMeeting" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Meeting">
			<Images Normal="svg:teams@teams_event" ></Images>
		</px:PXButton>
	</div>
</px:PXSmartPanel>

<px:PXSmartPanel ID="panelTeamsOwner" runat="server"  Caption="Teams Contact" Width="400px" Height="290px" AutoRepaint="true"
	CaptionVisible="true" Key="TeamsOwnerCard" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" 
	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
	<px:PXFormView ID="formTeamsOwner" runat="server" DataSourceID="ds" DataMember="TeamsOwnerCard" AllowCollapse="false" Width="100%" >
		<ContentStyle BackColor="Transparent" BorderStyle="None" />
		<Template>
			<px:PXImageView ID="edTeamsMemberPhoto" runat="server" DataField="PhotoFileName" Width="64" Height="64" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" />
			<px:PXTextEdit ID="edDisplayName" runat="server" DataField="DisplayName" SuppressLabel="True" Width="190px" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="XXS" ControlSize="XXS" />
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@offline&quot; />
      " ID="edStatusIconOwnerOffline" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerOffline" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@offline" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@available&quot; />
      " ID="edStatusIconOwnerAvailable" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerAvailable" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@available" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@busy&quot; />
      " ID="edStatusIconOwnerBusy" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerBusy" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@busy" ></Images>
			</px:PXButton>
			<px:PXButton Text="
        &lt;Images Normal=&quot;svg:teams@away&quot; />
      " ID="edStatusIconOwnerAway" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerAway" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@away" ></Images>
			</px:PXButton>
			<px:PXTextEdit ID="edStatus" runat="server" DataField="TeamsStatus" ControlSize="M" CssClass="teamsStatusLabel" SuppressLabel="True" Width="132px" />
			<px:PXLayoutRule runat="server" Merge="False" StartRow="True" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="edJobTitle" runat="server" DataField="JobTitle" Style="margin-top:20px" />
			<px:PXTextEdit ID="edCompanyName" runat="server" DataField="CompanyName" />
			<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" />
			<px:PXTextEdit ID="edMobilePhone" runat="server" DataField="MobilePhone" />
		</Template>
	</px:PXFormView>

	<div style="padding-left:74px; text-align: center;">
		<px:PXButton Text="
      &lt;Images Normal=&quot;svg:teams@teams_chat&quot; />
    " ID="edOwnerChat" runat="server" CommandSourceID="ds" CommandName="OwnerChat" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Chat">
			<Images Normal="svg:teams@teams_chat" ></Images>
		</px:PXButton>
		<px:PXButton Text="
      &lt;Images Normal=&quot;svg:teams@teams_call&quot; />
    " ID="edOwnerCall" runat="server" CommandSourceID="ds" CommandName="OwnerCall" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Call">
			<Images Normal="svg:teams@teams_call" ></Images>
		</px:PXButton>
		<px:PXButton Text="
      &lt;Images Normal=&quot;svg:teams@teams_event&quot; />
    " ID="edOwnerMeeting" runat="server" CommandSourceID="ds" CommandName="OwnerMeeting" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Meeting">
			<Images Normal="svg:teams@teams_event" ></Images>
		</px:PXButton>
	</div>
</px:PXSmartPanel>

	<px:PXSmartPanel ID="panelCreateSalesOrder" Key="CreateOrderParams" runat="server" Style="z-index: 108; left: 27px; position: absolute; top: 99px;" Caption="Create Sales Order" CaptionVisible="true"
	DesignView="Hidden" LoadOnDemand="true" AllowResize="False" AutoResize="True" AutoCallBack-Enabled="true" AutoCallBack-Target="formCopyTo" AutoCallBack-Command="Refresh"	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
	AcceptButtonID="btnCreateSalesOrderOK" CancelButtonID="btnCreateSalesOrderCancel" CloseButtonDialogResult="Abort" AutoReload="True" Width="520px" >

	<px:PXTab ID="tabCreateSalesOrder" runat="server" DataSourceID="ds" DataMember="CreateOrderParams"  Width="100%">
		<Items>
			<px:PXTabItem Text="Main" RepaintOnDemand="True">
				<Template>
					<px:PXFormView runat="server" ID="frmCreateSalesOrderSummary" DataSourceID="ds" DataMember="CreateOrderParams"
								   Style="z-index: 100" Width="100%" CaptionVisible="False" DefaultControlID="edOrderNbr">
						<Template>
							<px:PXLayoutRule runat="server" GroupCaption="Sales Order" />
							<px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
							<px:PXSelector ID="edOrderType" runat="server" AllowNull="False" DataField="OrderType" DisplayMode="Hint" CommitChanges="True" />
							<px:PXTextEdit ID="edOrderNbr" runat="server" DataField="OrderNbr" />
							<px:PXCheckBox ID="edMakeQuotePrimary" runat="server" DataField="MakeQuotePrimary" CommitChanges="true"/>
							<px:PXCheckBox ID="PXCheckBox2" runat="server" DataField="RecalculatePrices" CommitChanges="true" />
							<px:PXCheckBox ID="PXCheckBox3" runat="server" DataField="OverrideManualPrices" CommitChanges="true" Style="margin-left: 25px" />
							<px:PXCheckBox ID="PXCheckBox4" runat="server" DataField="RecalculateDiscounts" CommitChanges="true"/>
							<px:PXCheckBox ID="PXCheckBox5" runat="server" DataField="OverrideManualDiscounts" CommitChanges="true" Style="margin-left: 25px"/>
							<px:PXCheckBox ID="PXCheckBox6" runat="server" DataField="OverrideManualDocGroupDiscounts" CommitChanges="true" Style="margin-left: 25px" /> 
							<px:PXCheckBox ID="edConfirmManualAmount" runat="server" DataField="ConfirmManualAmount" Width="480px" CommitChanges="true" UncheckImages="" />
							<px:PXCheckBox ID="cstChkAMIncludeEstimate" runat="server" DataField="AMIncludeEstimate" />
							<px:PXCheckBox ID="cstChkAMCopyConfigurations" runat="server" DataField="AMCopyConfigurations" />
						</Template>
					</px:PXFormView>

					<px:PXFormView ID="frmCustomerInfo" runat="server" DataSourceID="ds" Width="100%" Caption="Creation Dialog" CaptionVisible="False" SkinID="Transparent"
									DataMember="CustomerInfo">
						<Template>
							<px:PXLayoutRule runat="server" GroupCaption="Customer"/>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
							<px:PXSelector ID="edAcctCD" runat="server" DataField="AcctCD" DisplayMode="Hint" TabIndex="40"/>
							<px:PXSelector ID="edClassID" runat="server" DataField="ClassID" TabIndex="60" AutoRefresh="True"/>
							<px:PXMailEdit ID="edEmail" runat="server" DataField="Email" TabIndex="70"/>
							
							<px:PXLayoutRule runat="server" StartRow="True" SuppressLabel="True" />
							<px:PXTextEdit
								runat="server"
								ID="edCreateCustomerWarning"
								Encode="True"
								DataField="WarningMessage"
								TextMode="MultiLine"
								DisableSpellcheck="True"
								Height="70px"
							/>

						</Template>
					</px:PXFormView>


				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Attributes" RepaintOnDemand="False" BindingContext="frmCustomerInfo" VisibleExp="DataControls[&quot;edAcctCD&quot;].Hidden == false">
				<Template>
					<px:PXLayoutRule runat="server" GroupCaption="Customer"/>
					<px:PXGrid ID="grdCustomerInfoAttributes" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="CustomerInfoAttributes">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="User-Defined Fields" RepaintOnDemand="false" BindingContext="frmCustomerInfo" VisibleExp="DataControls[&quot;edAcctCD&quot;].Hidden == false">
				<Template>
					<px:PXLayoutRule runat="server" GroupCaption="Customer"/>
					<px:PXGrid ID="grdCustomerInfoOrderUDF" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="CustomerInfoUDF">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

		</Items>
	</px:PXTab>

	<px:PXPanel ID="pnlCreateSalesOrderButtons" runat="server" SkinID="Transparent">
		<px:PXButton ID="btnCreateSalesOrderCreateCustomer" runat="server" CommandSourceID="ds" CommandName="CreateCustomerInPanel"
			style="float: left; margin: 2px" DialogResult="Retry"/>
		
		<px:PXButton ID="btnCreateSalesOrderCancel" runat="server"
			style="float: right; margin: 2px" Text="Cancel" DialogResult="Abort"/>

		<px:PXButton ID="btnCreateSalesOrderOK" runat="server" CommandSourceID="ds" CommandName="CreateSalesOrderInPanel"
			style="float: right; margin: 2px" Text="Create And Review" DialogResult="OK"/>

	</px:PXPanel>

	<style type="text/css">
		#ctl00_phDialogs_panelCreateSalesOrder_tabCreateSalesOrder_t0_frmCustomerInfo_edCreateCustomerWarning
		{
			border: 0px;
			background-color: #FFFACD !important;
			border-radius: 5px;
			padding: 10px;
			padding-left: 40px;
			vertical-align: middle;
			height: auto;

			background-image: url("data:image/svg+xml;utf8,<svg width='24' height='24' viewBox='0 0 24 24' fill='none' xmlns='http://www.w3.org/2000/svg'><path fill-rule='evenodd' clip-rule='evenodd' d='M11.1195 3.13515C11.4971 2.43395 12.5029 2.43395 12.8805 3.13515L21.7063 19.5259C22.065 20.1921 21.5825 21 20.8258 21H3.17422C2.41754 21 1.93501 20.1921 2.29374 19.5259L11.1195 3.13515ZM13.5 16.5C13.5 17.3284 12.8284 18 12 18C11.1716 18 10.5 17.3284 10.5 16.5C10.5 15.6716 11.1716 15 12 15C12.8284 15 13.5 15.6716 13.5 16.5ZM13.5 8.99999C13.5 8.17157 12.8284 7.49999 12 7.49999C11.1716 7.49999 10.5 8.17157 10.5 8.99999V12C10.5 12.8284 11.1716 13.5 12 13.5C12.8284 13.5 13.5 12.8284 13.5 12V8.99999Z' fill='%23FFAE4E'/></svg>");
			background-position: 10px 10px;
			background-repeat: no-repeat;
		}
	</style>
</px:PXSmartPanel>

	<px:PXSmartPanel ID="PanelCreateInvoice" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" Caption="Create Invoice"
	CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true" Key="CreateInvoicesParams" AutoCallBack-Enabled="true" AutoCallBack-Target="formCreateInvoice" AutoCallBack-Command="Refresh"
	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" AcceptButtonID="PXButton5" CancelButtonID="PXButton6">
	<px:PXFormView ID="formCreateInvoice" runat="server" DataSourceID="ds" Style="z-index: 100; text-align: left;" Width="100%" Caption="Services Settings" CaptionVisible="False" SkinID="Transparent"
		DataMember="CreateInvoicesParams" DefaultControlID="edRefNbr">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartRow="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="edRefNbr" runat="server" DataField="RefNbr" />
			<px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartRow="True" LabelsWidth="SM" ControlSize="XM" SuppressLabel="True" />
			<px:PXCheckBox ID="edMakeQuotePrimary" runat="server" DataField="MakeQuotePrimary" CommitChanges="true"/>
			<px:PXCheckBox ID="edRecalculatePrices" runat="server" DataField="RecalculatePrices" CommitChanges="true" />
			<px:PXCheckBox ID="edOverrideManualPrices" runat="server" DataField="OverrideManualPrices" CommitChanges="true" Style="margin-left: 25px" />
			<px:PXCheckBox ID="edRecalculateDiscounts" runat="server" DataField="RecalculateDiscounts" CommitChanges="true" />
			<px:PXCheckBox ID="edOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" CommitChanges="true" Style="margin-left: 25px" />
			<px:PXCheckBox ID="edOverrideManualDocGroupDiscounts" runat="server" DataField="OverrideManualDocGroupDiscounts" CommitChanges="true" Style="margin-left: 25px" />
			<px:PXCheckBox ID="edConfirmManualAmount" runat="server" DataField="ConfirmManualAmount" Width="480px" CommitChanges="true" />
		</Template>
	</px:PXFormView>
	<div style="padding: 5px; text-align: right;">
		<px:PXButton ID="PXButton5" runat="server" Text="Create" CommitChanges="True" DialogResult="Yes" Width="63px" Height="20px" Style="margin-left: 5px" />
		<px:PXButton ID="PXButton6" runat="server" Text="Cancel" CommitChanges="True" DialogResult="Cancel" Width="63px" Height="20px" Style="margin-left: 5px" />
	</div>
</px:PXSmartPanel>

	<%
PX.Objects.CS.PXAddressLookup.RegisterClientScript(this, ds.DataGraph);
%>

<px:PXSmartPanel ID="AddressLookupPanel" runat="server" 
	Style="z-index: 108; position: absolute; left: 27px; top: 99px;" 
	Caption="Address Lookup"
	Width="75%"
	DefaultControlID="searchBox"
	height="440px"
	DataSourceID="ds"
	CaptionVisible="True" 
	Key="AddressLookupFilter" 
	CancelButtonID="AddressEntityBtnCancel"
	AllowResize="true"
	ShowMaximizeButton="True" 
	AutoRepaint="true"
	ClientEvents-AfterShow="addressLookupAPI.initializeMaps"
	ClientEvents-BeforeHide="addressLookupAPI.clearSearch">
<style>
.flex-container {
	display: flex; /* or inline-flex */
	flex-direction: column;
	height: 100%;
}
.flex-item {
}
.mapContainer, #addressautocompletemap{
	z-index: 1000;
}
.customNavBar {
	position:absolute;
	top: 10px;
	left: 10px;
	z-index: 10001;
}
.pac-container {
	z-index: 10001;
}
.pac-container:after {
	/* Disclaimer: not needed to show 'powered by Google' if also a Google Map is shown */

	background-image: none !important;
	height: 0px;
}
</style>
<script type='text/javascript'>
var addressLookupVars = (function () {
	var _searchQuery = "";
	return {
		setSearchQuery: function (v) { 
				_searchQuery = v;
			},
		getSearchQuery: function () { 
				return _searchQuery;
			}
	}
})();

var addressLookupPanel = (function() {

	function _addHhandleCallback(context, error) {
		if (context == null || context.info == null)
			return;

		var _searchQuery = _GetSearchQuery();
		if ((typeof _searchQuery == 'undefined') ||
			(_searchQuery == null) ||
			(_searchQuery == "") ||
			_searchQuery == addressLookupVars.getSearchQuery()) {
			addressLookupVars.setSearchQuery(_searchQuery);
			addressLookupAPI.disableInfoWindows();
			return;
		}
		addressLookupVars.setSearchQuery(_searchQuery);
		addressLookupAPI.geocodeQuery(_searchQuery);
	}

	function _GetFormattedAddress() {
		if(	!(px_alls.SearchResponseAddressLine1.getValue() + 
			px_alls.SearchResponseAddressLine2.getValue() +
			px_alls.SearchResponseCity.getValue() +
			px_alls.SearchResponseState.getValue() + 
			px_alls.SearchResponsePostalCode.getValue())
		) {
			return null;
		}

		return _GetFormattedAddressHelper(
			", ",
			[
				px_alls.SearchResponseAddressLine1.getValue(),
				px_alls.SearchResponseAddressLine2.getValue(),
				px_alls.SearchResponseCity.getValue(),
				_GetFormattedAddressHelper(
					" ", 
					[
						px_alls.SearchResponseState.getValue(), 
						px_alls.SearchResponsePostalCode.getValue()
					]
				),
				px_alls.SearchResponseCountry.getValue()
			]
		);
	}

	function _GetFormattedAddressHelper(separator, args) {
		var result = "";
		args.forEach(
			function(arg) {
				if (arg != null && arg != "") {
					if (result != "") {
						result = result + separator;
					}
					result = result + arg;
				}
			});
		return result;
	}

	function _GetSearchQuery() {
		var _addrBeginning = _GetFormattedAddressHelper(
				" ", 
				[
					px_alls.SearchResponseAddressLine1.getValue(),
					px_alls.SearchResponseAddressLine2.getValue(),
					px_alls.SearchResponseCity.getValue(),
					px_alls.SearchResponseState.getValue(),
					px_alls.SearchResponsePostalCode.getValue()
				]
			);

		if(_addrBeginning == null || _addrBeginning == "") {
			return "";
		}
		return _GetFormattedAddressHelper(
			" ",
			[
				_addrBeginning,
				px_alls.SearchResponseCountry.getValue()
			]
		);
	}

	function _CleanSearchResponseValues() {
		px_alls.SearchResponseAddressLine1.updateValue("");
		px_alls.SearchResponseAddressLine2.updateValue("");
		px_alls.SearchResponseAddressLine3.updateValue("");
		px_alls.SearchResponseCity.updateValue("");
		px_alls.SearchResponseState.updateValue("");
		px_alls.SearchResponseLongitude.updateValue("");
	}

	return {
		addHhandleCallback: _addHhandleCallback,
		GetFormattedAddressHelper: _GetFormattedAddressHelper,
		GetFormattedAddress: _GetFormattedAddress,
		CleanSearchResponseValues: _CleanSearchResponseValues
	}
})();
</script>

<div class="flex-container" >
	<div class="flex-item" style="height: inherit;">
			<div class="mapContainer" id="mapcontainer"  style="height: inherit;" >
				<div id='searchBoxContainer' class="customNavBar" style="position: absolute;top:10px; left:10px;right:200px; max-width: 600px">
					<px:PXFormView AutoRepaint="true" DefaultControlID="searchBox"  ID="AddressLookupPanelformAddress" runat="server" DataSourceID="ds"
						CaptionVisible="False" SkinID="Transparent"
						DataMember="AddressLookupFilter">
						<Template>
							<px:PXTextEdit ID="searchBox" SuppressLabel="True" style="width: 100%;" runat="server" DataField="SearchAddress">
								<ClientEvents Initialize="addressLookupAPI.bindAutocompleteSearchControl" />
							</px:PXTextEdit>
							<px:PXTextEdit ID="addressLookupViewName" runat="server" DataField="ViewName" />

							<px:PXTextEdit ID="SearchResponseAddressLine1" runat="server" DataField="AddressLine1" />
							<px:PXTextEdit ID="SearchResponseAddressLine2" runat="server" DataField="AddressLine2" />
							<px:PXTextEdit ID="SearchResponseAddressLine3" runat="server" DataField="AddressLine3" />
							<px:PXTextEdit ID="SearchResponseCity" runat="server" DataField="City" />
							<px:PXTextEdit ID="SearchResponseCountry" runat="server" DataField="CountryID" />
							<px:PXTextEdit ID="SearchResponseState" runat="server" DataField="State" />
							<px:PXTextEdit ID="SearchResponsePostalCode" runat="server" DataField="PostalCode" />
							<px:PXTextEdit ID="SearchResponseLatitude" runat="server" DataField="Latitude" />
							<px:PXTextEdit ID="SearchResponseLongitude" runat="server" DataField="Longitude" />
						</Template>
					</px:PXFormView>
				</div>
				<div id="addressautocompletemap" style="height:100%;"></div>
			</div>
	</div>
	<div class="flex-item">
		<px:PXPanel ID="AddressEntityBtn" Style="height: 40px;" runat="server"  height="40px" SkinID="Buttons">
			<px:PXButton CommandName="AddressLookupSelectAction" CommandSourceID="ds" ID="AddressLookupSelectAction" runat="server" Text="Select" DialogResult="Cancel">
				<ClientEvents Click="addressLookupAPI.fillInAddress" />
			</px:PXButton>
			<px:PXButton ID="AddressEntityBtnCancel" runat="server" DialogResult="Cancel" Text="Cancel" Style="margin-left: 5px;margin-right: 5px;" />
		</px:PXPanel>
	</div>
</div>
</px:PXSmartPanel>

	<px:PXSmartPanel ID="InventoryMatrixEntrySmartPanel" runat="server" Caption="Add Matrix Item: Table View" CaptionVisible="True" LoadOnDemand="True" Key="Header"
	AutoCallBack-Target="MatrixEntryFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" Width="100%">

	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXFormView ID="MatrixEntryFormView" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Header"
        CaptionVisible="False" SkinID="Transparent">
        <Template>
            <px:PXLayoutRule ID="InventoryMatrixEntrySmartPanelFormLayout" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
			<px:PXSelector ID="edEntryTemplateItemID" runat="server" DataField="TemplateItemID" CommitChanges="true" />
			<px:PXTextEdit ID="edEntryDescription" runat="server" DataField="Description" />
			<px:PXSelector ID="selEntrySiteID" runat="server" DataField="SiteID" CommitChanges="true" AutoRefresh="true" />
        </Template>
    </px:PXFormView>
    
	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXGrid ID="MatrixItems" runat="server" DataSourceID="ds" SkinID="DetailsInTab" SyncPosition="True" Height="400px"
		RepaintColumns="True" OnAfterSyncState="MatrixItems_AfterSyncState" OnInit="MatrixItems_OnInit">
        <Levels>
			<px:PXGridLevel DataMember="MatrixItems">
				<RowTemplate>
					<px:PXSegmentMask ID="matrixItemsInventoryCD" runat="server" DataField="InventoryCD" AllowEdit="True" />
					<px:PXSegmentMask ID="matrixItemsDfltSiteID" runat="server" DataField="DfltSiteID" AllowEdit="True" />
					<px:PXSegmentMask ID="matrixItemsItemClassID" runat="server" DataField="ItemClassID" AllowEdit="True" />
					<px:PXSelector ID="matrixItemsTaxCategoryID" runat="server" DataField="TaxCategoryID" AllowEdit="True" />
                </RowTemplate>
                <Columns>
					<px:PXGridColumn DataField="AttributeValue0" CommitChanges="true" />
					<px:PXGridColumn DataField="UOM" />
					<px:PXGridColumn DataField="Qty" TextAlign="Right" />
					<px:PXGridColumn DataField="InventoryCD" DisplayFormat="&gt;CCCCC-CCCCCCCCCCCCCCC" />
					<px:PXGridColumn DataField="Descr" />
					<px:PXGridColumn DataField="New" Type="CheckBox" AllowShowHide="False" />
					<px:PXGridColumn DataField="StkItem" Type="CheckBox" />
					<px:PXGridColumn DataField="BasePrice" AllowShowHide="False" />
					<px:PXGridColumn DataField="ItemClassID" />
					<px:PXGridColumn DataField="TaxCategoryID" />
                </Columns>
			</px:PXGridLevel>
        </Levels>
		<Mode InitNewRow="True" />
    </px:PXGrid>

    <px:PXPanel runat="server" ID="InventoryMatrixEntrySmartButtons" SkinID="Buttons">
		<px:PXButton ID="InventoryMatrixEntrySmartPanelButtonToLookup" runat="server" DialogResult="Yes" Text="Open Matrix View" Height="20" />
		<px:PXButton ID="InventoryMatrixEntrySmartPanelButtonOK" runat="server" CommitChanges="True" DialogResult="OK" Text="Add and close" Height="20">
			<AutoCallBack Command="Commit" Target="ds">
				<Behavior CommitChanges="True" />
			</AutoCallBack>
		</px:PXButton>
		<px:PXButton ID="InventoryMatrixEntrySmartPanelButtonCancel" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>
</px:PXSmartPanel>

<px:PXSmartPanel ID="InventoryMatrixLookupSmartPanel" runat="server" Caption="Add Matrix Item: Matrix View" CaptionVisible="True" LoadOnDemand="True" Key="Matrix"
	AutoCallBack-Target="MatrixFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" Width="100%">

	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXFormView ID="MatrixFormView" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Header"
        CaptionVisible="False" SkinID="Transparent">
        <Template>
            <px:PXLayoutRule ID="InventoryMatrixLookupSmartPanelFormLayout" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
			<px:PXSelector ID="edTemplateItemID" runat="server" DataField="TemplateItemID" CommitChanges="true" />
			<px:PXSelector ID="selColumnAttributeID" runat="server" DataField="ColAttributeID" CommitChanges="true" AutoRefresh="true" />
			<px:PXSelector ID="selRowAttributeID" runat="server" DataField="RowAttributeID" CommitChanges="true" AutoRefresh="true" />
            <px:PXCheckBox ID="edShowAvailable" runat="server" DataField="ShowAvailable" AlignLeft="true" CommitChanges="true" />
			<px:PXSelector ID="selSiteID" runat="server" DataField="SiteID" CommitChanges="true" AutoRefresh="true" />
        </Template>
    </px:PXFormView>
    
	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXGrid ID="MatrixAttributes" runat="server" DataSourceID="ds" SkinID="Inquire" SyncPosition="True" Height="50px" Width="600px"
		AutoGenerateColumns="Recreate" RepaintColumns="True" GenerateColumnsBeforeRepaint="True" OnAfterSyncState="MatrixAttributes_AfterSyncState">
        <Levels>
            <px:PXGridLevel DataMember="AdditionalAttributes" />
        </Levels>
        <Mode AllowAddNew="False" AllowDelete="False" />
    </px:PXGrid>
	
	<px:PXLayoutRule runat="server" StartColumn="True" StartRow="true" />

    <px:PXGrid ID="MatrixMatrix" runat="server" DataSourceID="ds" SkinID="Inquire" SyncPosition="True" Height="400px" StatusField="MatrixAvailability"
		AutoGenerateColumns="Recreate" RepaintColumns="True" GenerateColumnsBeforeRepaint="True" OnAfterSyncState="MatrixMatrix_AfterSyncState" OnRowDataBound="MatrixMatrix_RowDataBound">
        <Levels>
            <px:PXGridLevel DataMember="Matrix" />
        </Levels>
        
        <Mode AllowAddNew="False" AllowDelete="False" />
		<ClientEvents AfterCellChange="matrixGrid_cellClick" />
    </px:PXGrid>

    <px:PXPanel runat="server" ID="InventoryMatrixLookupSmartButtons" SkinID="Buttons">
		<px:PXButton ID="InventoryMatrixLookupSmartPanelButtonToEntry" runat="server" DialogResult="Yes" Text="Open Table View" Height="20" />
		<px:PXButton ID="InventoryMatrixLookupSmartPanelButtonOK" runat="server" CommitChanges="True" DialogResult="OK" Text="Add and close" Height="20">
			<AutoCallBack Command="Commit" Target="ds">
				<Behavior CommitChanges="True" />
			</AutoCallBack>
		</px:PXButton>
		<px:PXButton ID="InventoryMatrixLookupSmartPanelButtonCancel" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>
</px:PXSmartPanel>

<script type="text/javascript">
	// Updates availability when user clicks matrix cell
	var matrixGridOldColumnName = null;
	function matrixGrid_cellClick(grid, ev) {
		var ds = px_alls["ds"];
		var columnName = ev.cell.column.dataField;
		if (ds != null && columnName != null && (matrixGridOldColumnName == null || matrixGridOldColumnName != columnName)) {
			var showavail = px_alls["edShowAvailable"];
			if (showavail != null && showavail.getValue() == true) {
				ds.executeCallback("MatrixGridCellChanged", columnName); matrixGridOldColumnName = columnName;
			}
		}
	}
</script>

	<px:PXSmartPanel ID="PanelCreateQuote" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" Caption="Create Quote"
					 CaptionVisible="True" LoadOnDemand="true" ShowAfterLoad="true" Key="QuoteInfo" AutoCallBack-Enabled="true" AutoCallBack-Target="formCreateQuote" AutoCallBack-Command="Refresh"
					 CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" AcceptButtonID="CreateQuoteBtnConvert" CancelButtonID="CreateQuoteBtnCancel">
	<px:PXTab ID="tabPanelCreateQuote" runat="server" Width="100%" DataSourceID="ds" DataMember="QuoteInfo">
		<Items>
			<px:PXTabItem Text="Main" RepaintOnDemand="True">
				<Template>
					<px:PXFormView ID="formCreateQuote" runat="server" DataSourceID="ds" Width="100%" Caption="Services Settings" CaptionVisible="False" SkinID="Transparent"
					   DataMember="QuoteInfo">
						<Template>
							<px:PXLayoutRule ID="PXLayoutRule6" runat="server" LabelsWidth="S" ControlSize="XM" />
							<px:PXDropDown ID="edQuoteType" runat="server" DataField="QuoteType" CommitChanges="true"/>
							<px:PXCheckBox ID="edAddProductsFromOpportunity" runat="server" DataField="AddProductsFromOpportunity" CommitChanges="true"/>
							<px:PXCheckBox ID="edMakeNewQuotePrimary" runat="server" DataField="MakeNewQuotePrimary" CommitChanges="true"/>
							<px:PXCheckBox ID="edRecalculatePrices" runat="server" DataField="RecalculatePrices" CommitChanges="true"/>
							<px:PXCheckBox ID="edOverrideManualPrices" runat="server" DataField="OverrideManualPrices" CommitChanges="true" Style="margin-left: 25px"/>
							<px:PXCheckBox ID="edRecalculateDiscounts" runat="server" DataField="RecalculateDiscounts" CommitChanges="true"/>
							<px:PXCheckBox ID="edOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" CommitChanges="true" Style="margin-left: 25px"/>
							<px:PXCheckBox ID="edOverrideManualDocGroupDiscounts" runat="server" DataField="OverrideManualDocGroupDiscounts" CommitChanges="true" Style="margin-left: 25px"/>
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="User-Defined Fields" RepaintOnDemand="false">
				<Template>
					<px:PXGrid ID="grdCreateQuoteInfoUDF" runat="server" Width="500px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="CreateQuoteInfoUDF">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="250px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="250px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
	</px:PXTab>
	<px:PXPanel ID="formCreateQuoteBtn" runat="server" SkinID="Buttons">
		<px:PXButton ID="CreateQuoteBtnReview" runat="server" Text="Create and Review" CommitChanges="True" DialogResult="Yes"></px:PXButton>
		<px:PXButton ID="CreateQuoteBtnConvert" runat="server" Text="Create" CommitChanges="True" DialogResult="No" Style="margin-left: 5px" />
		<px:PXButton ID="CreateQuoteBtnCancel" runat="server" Text="Cancel" CommitChanges="True" DialogResult="Cancel" Style="margin-left: 5px" />
	</px:PXPanel>
</px:PXSmartPanel>

	<px:PXSmartPanel runat="server" ID="AddEstimatePanel" LoadOnDemand="True" CaptionVisible="True" Caption="Add Estimate" Key="OrderEstimateItemFilter">
		<px:PXFormView runat="server" SkinID="Transparent" CaptionVisible="False" Width="100%" ID="estimateAddForm" DataSourceID="ds" DataMember="OrderEstimateItemFilter">
			<Template>
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule01" StartColumn="True" Merge="True" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" DataField="EstimateID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" ID="panelEstimateID" />
				<px:PXCheckBox runat="server" AutoCallBack="True" CommitChanges="True" DataField="AddExisting" ID="panelAddExisting" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule02" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" DataField="RevisionID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" ID="panelRevisionID" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule03" Merge="True" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" DataField="InventoryCD" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" ID="panelInventoryCD" />
				<px:PXCheckBox runat="server" DataField="IsNonInventory" ID="panelEstimateIsNonInventory" />
				<px:PXLayoutRule runat="server" StartColumn="False" LabelsWidth="SM" ControlSize="M" />
				<px:PXSegmentMask runat="server" DataField="SubItemID" ID="edSubItemID" />
				<px:PXSelector runat="server" DataField="SiteID" ID="edSiteID" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule04" LabelsWidth="S" ControlSize="XL" />
				<px:PXTextEdit runat="server" CommitChanges="True" DataField="ItemDesc" ID="panelItemDesc" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule05" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" DataField="EstimateClassID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" ID="panelEstimateClassID" />
				<px:PXSelector runat="server" DataField="ItemClassID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" ID="panelItemClassID" />
				<px:PXSelector runat="server" DataField="UOM" CommitChanges="True" ID="panelEstimateUOM" />
				<px:PXSelector runat="server" DataField="BranchID" CommitChanges="True" ID="panelBranchID" />
			</Template>
		</px:PXFormView>
		<px:PXPanel runat="server" ID="AddEstButtonPanel" SkinID="Buttons">
			<px:PXButton runat="server" ID="AddEstButton1" Text="OK" DialogResult="OK" CommandSourceID="ds" />
			<px:PXButton runat="server" ID="AddEstButton2" Text="Cancel" DialogResult="Cancel" CommandSourceID="ds" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel runat="server" ID="QuickEstimatePanel" LoadOnDemand="True" CloseButtonDialogResult="Abort" AutoReload="True" CaptionVisible="True" Caption="Quick Estimate" Key="SelectedEstimateRecord">
		<px:PXFormView runat="server" SkinID="Transparent" CaptionVisible="False" Width="100%" ID="QuickEstimateForm" DefaultControlID="EstimateID" SyncPosition="True" DataSourceID="ds" DataMember="SelectedEstimateRecord">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector runat="server" DataField="EstimateID" ID="panelQuickEstimateID" AutoRefresh="True" />
				<px:PXSelector runat="server" DataField="RevisionID" ID="panelQuickRevisionID" AutoRefresh="True" />
				<px:PXLayoutRule runat="server" Merge="true" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector runat="server" DataField="InventoryCD" ID="panelInventoryCD" />
				<px:PXCheckBox runat="server" DataField="IsNonInventory" ID="panelIsNonInventory" />
				<px:PXLayoutRule runat="server" StartColumn="False" LabelsWidth="SM" ControlSize="M" />
				<px:PXSegmentMask runat="server" DataField="SubItemID" ID="edSubItemID" />
				<px:PXSelector runat="server" DataField="SiteID" ID="edSiteID" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="L" />
				<px:PXTextEdit runat="server" DataField="ItemDesc" ID="panelItemDesc" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector runat="server" DataField="EstimateClassID" CommitChanges="True" ID="panelEstimateClassID" AutoRefresh="True" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="FixedLaborCost" ID="edFixedLaborCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="FixedLaborOverride" ID="edFixedLaborOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="VariableLaborCost" ID="edVariableLaborCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="VariableLaborOverride" ID="edVariableLaborOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="MachineCost" ID="edMachineCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="MachineOverride" ID="edMachineOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="MaterialCost" ID="edMaterialCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="MaterialOverride" ID="edMaterialOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="ToolCost" ID="edToolCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="ToolOverride" ID="edToolOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="FixedOverheadCost" ID="edFixedOverheadCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="FixedOverheadOverride" ID="edFixedOverheadOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="VariableOverheadCost" ID="edVariableOverheadCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="VariableOverheadOverride" ID="edVariableOverheadOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="SubcontractCost" ID="edSubcontractCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="SubcontractOverride" ID="edSubcontractOverride" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" DataField="ExtCostDisplay" ID="edCuryExtCost" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="ReferenceMaterialCost" ID="edReferenceMaterialCost" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="OrderQty" ID="panelQuickOrderQty" />
				<px:PXLayoutRule runat="server" ControlSize="M" />
				<px:PXSelector runat="server" DataField="UOM" ID="panelQuickUOM" />
				<px:PXLayoutRule runat="server" ControlSize="XL" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="CuryUnitCost" ID="panelQuickCuryUnitCost" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="MarkupPct" ID="panelQuickMarkupPct" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="CuryUnitPrice" ID="panelQuickCuryUnitPrice" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="PriceOverride" ID="edQuick1" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="CuryExtPrice" ID="panelQuickCuryExtPrice" />
			</Template>
		</px:PXFormView>
		<px:PXPanel runat="server" ID="QuickEstButtonPanel" SkinID="Buttons">
			<px:PXButton runat="server" ID="QuickEstButton1" Text="OK" DialogResult="OK" CommandSourceID="ds" />
			<px:PXButton runat="server" ID="QuickEstButton2" Text="Cancel" DialogResult="Abort" CommandSourceID="ds" />
		</px:PXPanel>
	</px:PXSmartPanel>
    <px:PXSmartPanel ID="ServiceOrderAppointmentSettingsPanel" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;"
	Caption="Create Service Order/Appointment" CaptionVisible="True"
	LoadOnDemand="True" Key="DocumentSettings" AutoCallBack-Enabled="True"
	AutoCallBack-Target="formServiceOrderAppointmentSettings" AutoCallBack-Command="Refresh"
	AcceptButtonID="PXButtonOK" CancelButtonID="PXButtonCancel"
	AllowResize="False" Width="535px">
	<px:PXLayoutRule runat="server" GroupCaption="Document Settings"/>
	<px:PXFormView ID="formServiceOrderAppointmentSettings" runat="server"
		DataSourceID="ds" Width="100%" Caption="Document Settings" CaptionVisible="False"
		SkinID="Transparent" DataMember="DocumentSettings" MarkRequired="Dynamic">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>

			<px:PXSelector ID="edSrvOrdType" runat="server" DataField="SrvOrdType" CommitChanges="True" TabIndex="10"/>
			<px:PXSelector ID="edBranchID" runat="server" DataField="BranchID" CommitChanges="True" TabIndex="20"/>
			<px:PXSelector ID="edBranchLocationID" runat="server" DataField="BranchLocationID" TabIndex="30" CommitChanges="True" AutoRefresh="True"/>
			<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" CommitChanges="True" TabIndex="40"/>
			<px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID" CommitChanges="True" TabIndex="50" />
			<px:PXSelector ID="edProjectTaskID" runat="server" DataField="ProjectTaskID" CommitChanges="True" TabIndex="60"/>
			
			<px:PXLayoutRule runat="server" LabelsWidth="M" ControlSize="XM" GroupCaption="Service Order Settings" StartGroup="True" />
			<px:PXDateTimeEdit ID="edOrderDate" runat="server" DataField="OrderDate" CommitChanges="True" TabIndex="70"/>
							
			<px:PXLayoutRule runat="server" Merge="True" />							
			<px:PXDateTimeEdit ID="edSLAETA_Date" runat="server" DataField="SLAETA_Date" TabIndex="90"/>
			<px:PXDateTimeEdit ID="edSLAETA_Time" runat="server" DataField="SLAETA_Time" TimeMode="True" SuppressLabel="True" TabIndex="90"/>
							
			<px:PXLayoutRule runat="server" />
			<px:PXSelector ID="edAssignedEmpID" runat="server" DataField="AssignedEmpID" TabIndex="100"/>
			<px:PXSelector ID="edProblemID" runat="server" DataField="ProblemID" TabIndex="110"/>
			<px:PXSelector ID="edContactID" runat="server"  DataField="ContactID" AllowEdit="True" DataSourceID="ds" AllowAddNew="True" AutoRefresh="True" CommitChanges="True" TabIndex="115"/>

			<px:PXLayoutRule runat="server" LabelsWidth="M" ControlSize="XM" GroupCaption="Appointment Settings" StartGroup="True" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXDateTimeEdit ID="edScheduledDateTimeBegin_Date" runat="server" CommitChanges="True" DataField="ScheduledDateTimeBegin_Date"  TabIndex="120"/>
			<px:PXDateTimeEdit ID="edScheduledDateTimeBegin_Time" runat="server" CommitChanges="True" DataField="ScheduledDateTimeBegin_Time" TimeMode="True" SuppressLabel="True" TabIndex="130"/>
							
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXDateTimeEdit ID="edScheduledDateTimeEnd_Date" runat="server" CommitChanges="True" DataField="ScheduledDateTimeEnd_Date"  TabIndex="140"/>
			<px:PXDateTimeEdit ID="edScheduledDateTimeEnd_Time" runat="server" CommitChanges="True" DataField="ScheduledDateTimeEnd_Time" TimeMode="True" SuppressLabel="True" TabIndex="150"/>
			<px:PXCheckBox ID="edHandleManuallyScheduleTime" runat="server" DataField="HandleManuallyScheduleTime" AlignLeft="False" CommitChanges="True" TabIndex="160"/>
		</Template>
	</px:PXFormView>

	<px:PXPanel ID="ServiceOrderAppointmentSettingsBtn" runat="server" SkinID="Buttons">
		<px:PXButton ID="CreateAndReview" runat="server" Text="Create and review" DialogResult="Yes" />
		<px:PXButton ID="Create" runat="server" Text="Create" DialogResult="OK" />
		<px:PXButton ID="Cancel" runat="server" DialogResult="Cancel" Text="Cancel" Style="margin-left: 5px"/>
	</px:PXPanel>
</px:PXSmartPanel>
</asp:Content>
