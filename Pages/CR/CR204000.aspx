<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="CR204000.aspx.cs" Inherits="Page_CR204000"
    Title="Mailing Lists" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource EnableAttributes="true" ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.CRMarketingListMaint"
        PrimaryView="MailLists" HeaderDescriptionField="Name">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="Last" PostData="Self" />
            <px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="ViewActivity" DependOnGrid="gridActivities" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="syncHubSpot" Visible="false" />
            <px:PXDSCallbackCommand Name="pullFromHubSpot" Visible="false" />
            <px:PXDSCallbackCommand Name="pushToHubSpot" Visible="false" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="MailLists" Caption="Marketing List Info"
        FilesIndicator="True"
        NoteIndicator="True"
        ActivityIndicator="False" ActivityField="NoteActivity"
        DefaultControlID="edMailListCode">
        <Template>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />

            <px:PXSegmentMask ID="edMailListCode" runat="server" DataField="MailListCode" AutoRefresh="True" />

            <px:PXTextEdit ID="edName" runat="server" DataField="Name" AllowNull="False" />
            <px:PXDropDown ID="edType" runat="server" DataField="Type" CommitChanges="True"/>

            <px:PXSelector ID="edGIDesignID" runat="server" DataField="GIDesignID" CommitChanges="true" DisplayMode="Text" 
                           AllowEdit="True" OnEditRecord="anySelector_OnEditRecord" 
                           EditImages-Normal="control@WebN" EditImages-Disabled="control@WebD"
                           EditImages-Hover="control@WebH" EditImages-Pushed="control@WebP" AutoRefresh="True" />
            <px:PXSelector ID="edSharedGIFilter" runat="server" DataField="SharedGIFilter" CommitChanges="true" DisplayMode="Text" AutoRefresh="true" />

            <px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />

            <px:PXDropDown ID="edStatus" runat="server" DataField="Status" />
            <px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" CommitChanges="True" AutoRefresh="True" TextField="acctname" />
            <px:PXSelector ID="edWorkgroupID" runat="server" DataField="WorkgroupID" TextMode="Search" CommitChanges="True" DisplayMode="Text" FilterByAllFields="True" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" DataMember="MailListsCurrent" Width="100%" DataSourceID="ds" RepaintOnDemand="false">
        <Items>
            <px:PXTabItem Text="List Members" RepaintOnDemand="true" LoadOnDemand="true">
                <AutoCallBack Command="Refresh" Target="grdSubscribers" />
                <Template>

                    <px:PXGrid ID="grdSubscribers" runat="server" DataSourceID="ds" AllowPaging="True" ActionsPosition="Top"
                               SkinID="Details" SyncPosition="True" AllowSearch="True" AllowFilter="True" Width="100%" AdjustPageSize="Auto"
                               FastFilterFields="ContactID,Contact__DisplayName,Contact__FullName,Contact__Salutation,Contact__BAccountID,Contact__Email">
                        <Levels>
                            <px:PXGridLevel DataMember="ListMembers">
                                <Columns>
                                    <px:PXGridColumn DataField="Selected" Type="CheckBox" Visible="True" SyncVisible="False" TextAlign="Center" AllowCheckAll="True" />
                                    <px:PXGridColumn DataField="IsSubscribed" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="Contact__ContactType" />
                                    <px:PXGridColumn DataField="ContactID" TextField="ContactBAccount__Contact" AutoCallBack="true"
                                                     DisplayMode="Text" TextAlign="Left" LinkCommand="ListMembers_Contact_ViewDetails" Width="280px" />
                                    <px:PXGridColumn DataField="Contact__Salutation" />
                                    <px:PXGridColumn DataField="Contact__BAccountID" LinkCommand="ListMembers_BAccount_ViewDetails" />
                                    <px:PXGridColumn DataField="Contact__FullName" />
                                    <px:PXGridColumn DataField="Contact__Email" />
                                    <px:PXGridColumn DataField="CreatedDateTime" AllowUpdate="False" DisplayFormat="g" Visible="True" />
                                    <px:PXGridColumn DataField="Contact__IsActive" AllowNull="False" TextAlign="Center" Type="CheckBox" Visible="False" SyncVisible="False" />
                                    <px:PXGridColumn DataField="Contact__ClassID" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Phone1" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__LastModifiedDateTime" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__CreatedDateTime" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Source" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__AssignDate" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__DuplicateStatus" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Phone2" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Phone3" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__DateOfBirth" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Fax" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__WebSite" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__ConsentAgreement" Visible="False" SyncVisible="False" Type="CheckBox"/>
                                    <px:PXGridColumn DataField="Contact__ConsentDate" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__ConsentExpirationDate" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__ParentBAccountID" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Gender" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Method" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__NoCall" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__NoEMail" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__NoFax" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__NoMail" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__NoMarketing" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__NoMassMail" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__CampaignID" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Phone1Type" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Phone2Type" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Phone3Type" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__FaxType" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__MaritalStatus" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Spouse" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Status" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__Resolution" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__LanguageID" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__OwnerID" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Contact__OwnerID_description" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address__AddressLine1" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address__AddressLine2" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address__City" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address__State" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address__PostalCode" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address__CountryID" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="BAccount__ClassID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="BAccount__WorkgroupID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="BAccount__OwnerID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="BAccount__ParentBAccountID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="BAccount__CampaignSourceID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="Address2__AddressLine1" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address2__AddressLine2" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address2__City" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address2__State" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address2__PostalCode" Visible="False" SyncVisible="False"/>
                                    <px:PXGridColumn DataField="Address2__CountryID" Visible="False" SyncVisible="False"/>
                                </Columns>
                                <RowTemplate>
                                    <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID" AutoGenerateColumns="False" AutoRefresh="True">
                                        <GridProperties FastFilterFields="MemberName,FullName,Salutation,EMail">
                                            <Columns>
                                                <px:PXGridColumn DataField="ContactType" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="MemberName" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="Salutation" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="FullName" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="EMail" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="Phone1" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="IsActive" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="BAccountID" Visible="False" SyncVisible="True" />
                                                <px:PXGridColumn DataField="ClassID" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="LastModifiedDateTime" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="CreatedDateTime" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Source" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="AssignDate" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="DuplicateStatus" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Phone2" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Phone3" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="DateOfBirth" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Fax" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Gender" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Method" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="NoCall" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="NoEMail" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="NoFax" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="NoMail" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="NoMarketing" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="NoMassMail" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="CampaignID" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Phone1Type" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Phone2Type" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Phone3Type" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="FaxType" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="MaritalStatus" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Spouse" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Status" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="Resolution" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="LanguageID" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="OwnerID" Visible="False" SyncVisible="False"/>
                                                <px:PXGridColumn DataField="OwnerID_description" Visible="False" SyncVisible="False"/>
                                            </Columns>
                                        </GridProperties>
                                    </px:PXSelector>
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <Mode AllowUpload="True"></Mode>
                        <AutoSize Enabled="True" MinHeight="100" MinWidth="100"/>
                        <ActionBar>
                            <PagerSettings Mode="NextPrevFirstLast" />
                            <Actions>
                                <Save Enabled="False" />
                                <FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
                                <Search ToolBarVisible="Top" />
                                <Upload ToolBarVisible="Top" />
                                <Delete ToolBarVisible="False" MenuVisible="False"  />
                            </Actions>
                            <CustomItems>
                                <px:PXToolBarButton Key="cmdDeleteSelectedMembers" Tooltip="Delete Selected Rows"
                                                    ImageKey="RecordDel" ImageSet="main" DisplayStyle="Image">
                                    <AutoCallBack Command="DeleteSelectedMembers" Target="ds"/>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdAddMembersMenu">
                                    <AutoCallBack Command="AddMembersMenu" Target="ds"/>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdManageSubscriptionMenu">
                                    <AutoCallBack Command="ManageSubscriptionMenu" Target="ds"/>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdCopyMembers">
                                    <AutoCallBack Command="CopyMembers" Target="ds"/>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="cmdClearMembers">
                                    <AutoCallBack Command="ClearMembers" Target="ds"/>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>

			<px:PXTabItem Text="Used in Campaigns" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="grdCampaigns" runat="server" Height="400px" Width="100%" Style="z-index: 100"
						AllowPaging="True" ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SyncPosition="True"
						FastFilterFields="CRCampaign__CampaignID,CRCampaign__CampaignName,CRCampaignMembers__MarketingListID,CRCampaign__PromoCodeID"
						SkinID="Details">
						<Levels>
							<px:PXGridLevel DataMember="MarketingCampaigns">
								<Columns>
									<px:PXGridColumn DataField="CampaignID" AutoCallBack="true" LinkCommand="MarketingCampaigns_CRCampaign_ViewDetails" />
									<px:PXGridColumn DataField="CRCampaign__CampaignName" />
									<px:PXGridColumn DataField="CRCampaign__Status"  Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CRCampaign__StartDate" />
									<px:PXGridColumn DataField="CRCampaign__EndDate" />
									<px:PXGridColumn DataField="CRCampaign__PromoCodeID" />
									<px:PXGridColumn DataField="CRCampaign__OwnerID" LinkCommand="MarketingCampaigns_OwnerID_ViewDetails" />
									<px:PXGridColumn DataField="LastUpdateDate" Width="130px" />

									<px:PXGridColumn DataField="CRCampaign__CreatedByID" LinkCommand="CreatedByID_ViewDetails"  Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CRCampaign__CreatedDateTime"  Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CRCampaign__LastModifiedByID" LinkCommand="LastModifiedByID_ViewDetails"  Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CRCampaign__LastModifiedDateTime"  Visible="false" SyncVisible="false" />
								</Columns>
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar DefaultAction="cmdViewDoc">
							<Actions>
								<FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
								<Search ToolBarVisible="Top" />
								<AddNew ToolBarVisible="False"/>
								<Delete ToolBarVisible="False"/>
							</Actions>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Activities" LoadOnDemand="True" RepaintOnDemand="False" Visible="false">
				<%-- Visible could be turned on in customization --%>
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
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdAddEvent">
									<AutoCallBack Command="NewEvent" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdAddEmail">
									<AutoCallBack Command="NewMailActivity" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdAddActivity">
									<AutoCallBack Command="NewActivity" Target="ds" />
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
									<px:PXGridColumn DataField="ClassIcon"					Width="31px" AllowFilter="False" AllowResize="False" AllowSort="False" />
									<px:PXGridColumn DataField="ClassInfo" />
									<px:PXGridColumn DataField="RefNoteID" Visible="False" AllowShowHide="False" />
									<px:PXGridColumn DataField="Subject" LinkCommand="ViewActivity" />
									<px:PXGridColumn DataField="UIStatus" />
									<px:PXGridColumn DataField="Released" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="StartDate" DisplayFormat="g" Width="125px" />
									<px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" Width="125px" />
									<px:PXGridColumn DataField="TimeSpent" />
									<px:PXGridColumn DataField="CreatedByID" Visible="False" AllowShowHide="False" />
									<px:PXGridColumn DataField="CreatedByID_Creator_Username" Visible="False" SyncVisible="False" SyncVisibility="False" Width="108px">
										<NavigateParams>
											<px:PXControlParam Name="PKID" ControlID="gridActivities" PropertyName="DataValues[&quot;CreatedByID&quot;]" />
										</NavigateParams>
									</px:PXGridColumn>
									<px:PXGridColumn DataField="WorkgroupID" />
									<px:PXGridColumn DataField="OwnerID" LinkCommand="OpenActivityOwner" DisplayMode="Text" />
									<px:PXGridColumn DataField="BAccountID" LinkCommand="<stub>" Visible="False" SyncVisible="False" />
									<px:PXGridColumn DataField="ContactID" DisplayMode="Text" LinkCommand="<stub>" Visible="False" SyncVisible="False" />
									<px:PXGridColumn DataField="ProjectID" Visible="False" SyncVisible="False" />
									<px:PXGridColumn DataField="ProjectTaskID" Visible="False" SyncVisible="False" />
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

            <!--#include file="~\Pages\HS\HubSpotTab.inc"-->

        </Items>
        <AutoSize Enabled="True" Container="Window" />
    </px:PXTab>

    <px:PXSmartPanel ID="pnlCopyMembers" runat="server" Key="CopyMembersFilterView"
                     Caption="Copy Members" CaptionVisible="true" AutoRepaint="True" AllowResize="False"
                     Width="520px" Height="314px" Style="z-index: 108; position: absolute; left: 27px; top: 99px;"
                     CancelButtonID="btnCopyMembersCancel" AcceptButtonID="btnCopyMembersNext" CloseButtonDialogResult="Abort">

        <px:PXFormView ID="frmCopyMembers" runat="server" CaptionVisible="False" DataMember="CopyMembersFilterView" DataSourceID="ds"
                       SkinID="Transparent" Style="z-index: 100" Width="100%" Height="85%">
            <Template>
                <px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartColumn="True" LabelsWidth="L" ControlSize="XM" ColumnWidth="100%" />
                <px:PXLabel ID="lblCopyMembers" runat="server" Text="Select one of the following options to continue:"
                            Style="position: absolute; top: 14px; left:19px; width: 95%; font-style: italic"/>
                <px:PXLayoutRule runat="server" />
                <px:PXGroupBox ID="gbxCopyOption" DataField="AddMembersOption" runat="server" RenderStyle="Simple" RenderSimple="True" CommitChanges="True">
                    <Template>
                        <px:PXLayoutRule ID="PXLayoutRule19" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                        <px:PXRadioButton ID="rdbNew" runat="server" Value="0" />
                        <px:PXRadioButton ID="rdbExisting" runat="server" Value="1" />
                    </Template>
                </px:PXGroupBox>
                
            </Template>
            <CallbackCommands>
                <Save RepaintControls="None" />
            </CallbackCommands>
        </px:PXFormView>
        <px:PXPanel ID="pnlCopyMembersButtons" runat="server" CommandSourceID="ds" SkinID="Buttons">
            <px:PXButton ID="btnCopyMembersNext" runat="server" CommandSourceID="ds" Text="Next" DialogResult="OK"/>
            <px:PXButton ID="btnCopyMembersCancel" runat="server" CommandSourceID="ds" Text="Cancel" DialogResult="Abort"/>
        </px:PXPanel>
    </px:PXSmartPanel>

    <px:PXSmartPanel ID="pnlAddMembersToNewList" runat="server" Key="AddMembersToNewListFilterView"
                     Caption="Add Members to a New Marketing List" CaptionVisible="true" AutoRepaint="True" AllowResize="False"
                     Width="520px" Height="314px" Style="z-index: 108; position: absolute; left: 27px; top: 99px;"
                     CancelButtonID="btnAddMembersToNewListCancel" CloseButtonDialogResult="Abort">

        <px:PXTab ID="tabAddMembersMain" runat="server" DataSourceID="ds" DataMember="AddMembersToNewListFilterView"
                  Width="100%" Height="85%">
            <Items>

                <px:PXTabItem Text="Main" RepaintOnDemand="True">
                    <Template>
                            <template>
                                <px:PXLayoutRule ID="PXLayoutRule10" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />

                                <px:PXSegmentMask ID="edMailListCode" runat="server" DataField="MailListCode" CommitChanges="True" />
                                <px:PXTextEdit ID="edName" runat="server" DataField="Name" AllowNull="False" />
                                <px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" CommitChanges="True" AutoRefresh="True" TextField="acctname" />

                            </template>
                    </Template>
                </px:PXTabItem>

                <px:PXTabItem Text="User-Defined Fields" RepaintOnDemand="false">
                    <Template>
                        <px:PXGrid ID="grdAddMembersToNewListUdf" runat="server" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true"
                                   Width="100%" Height="100%">
                            <Levels>
                                <px:PXGridLevel DataMember="AddMembersToNewListFilterUdfView">
                                    <Columns>
                                        <px:PXGridColumn DataField="DisplayName" Width="250px"/>
                                        <px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="250px"/>
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

        <px:PXPanel ID="pnlAddMembersToNewListButtons" runat="server" CommandSourceID="ds" SkinID="Buttons">
            <px:PXButton ID="btnAddMembersToNewListCreateAndReview" runat="server" CommandSourceID="ds" Text="Create and Review" DialogResult="Yes"/>
            <px:PXButton ID="btnAddMembersToNewListCreate" runat="server" CommandSourceID="ds" Text="Create" DialogResult="OK"/>
            <px:PXButton ID="btnAddMembersToNewListCancel" runat="server" CommandSourceID="ds" Text="Cancel" DialogResult="Abort"/>
        </px:PXPanel>
    </px:PXSmartPanel>

    <px:PXSmartPanel ID="pnlAddMembersToExistingLists" runat="server" Key="AddMembersToExistingListsFilterView"
                     Caption="Add Members to Marketing Lists" CaptionVisible="true" AutoRepaint="True" AllowResize="False" AutoReload="True"
                     Width="520px" Height="314px" Style="z-index: 108; position: absolute; left: 27px; top: 99px;"
                     CancelButtonID="btnAddMembersToExistingListsCancel" CloseButtonDialogResult="Abort">

        <px:PXGrid runat="server" ID="grdAddMembersToExistingLists" DataSourceID="ds" SkinID="Details"
                   AutoAdjustColumns="true" FilesIndicator="false" NoteIndicator="false" ActionsPosition="Top"
                   AllowSearch="true" AllowPaging="False"
                   AllowFilter="true" FastFilterFields="MailListCode,Name,OwnerID"
                   Width="100%" Height="85%">
            <Levels>
                <px:PXGridLevel DataMember="AddMembersToExistingListsFilterView" >
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="True" />
                        <px:PXGridColumn DataField="MailListCode" LinkCommand="AddMembersToExistingListsFilterView_ViewDetails"/>
                        <px:PXGridColumn DataField="Name" />
                        <px:PXGridColumn DataField="Status" />
                        <px:PXGridColumn DataField="OwnerID" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="OwnerID_description" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="CreatedDateTime" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="LastModifiedDateTime" SyncVisible="false" Visible="false"/>

                    </Columns>
                    <Mode AllowAddNew="false" AllowColMoving="false" AllowDelete="false"
                          AllowFormEdit="false" AllowSort="true" AllowUpdate="true"
                          AllowDragRows="false" AllowRowSizing="false" InitNewRow="false"
                          AllowUpload="false"/>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" />
            <ActionBar PagerVisible="False">
                <Actions>
                    <FilterBar ToolBarVisible="Top" Order="0" GroupIndex="0" />
                    <AddNew ToolBarVisible="False"/>
                    <Delete ToolBarVisible="False"/>
                    <ExportExcel ToolBarVisible="False"/>
                </Actions>
            </ActionBar>
        </px:PXGrid>
        <px:PXPanel ID="pnlAddMembersToExistingListsButtons" runat="server" CommandSourceID="ds" SkinID="Buttons">
            <px:PXButton ID="btnAddMembersToExistingListsOK" runat="server" CommandSourceID="ds" Text="Copy" DialogResult="OK"/>
            <px:PXButton ID="btnAddMembersToExistingListsCancel" runat="server" CommandSourceID="ds" Text="Cancel" DialogResult="Abort"/>
        </px:PXPanel>
    </px:PXSmartPanel>


    <px:PXSmartPanel ID="pnlAddMembersFromGI" runat="server" Key="AddMembersFromGIFilterView"
                     Caption="Add Members" CaptionVisible="true" AutoRepaint="True" AllowResize="False"
                     Width="520px" Style="z-index: 108; position: absolute; left: 27px; top: 99px;"
                     CancelButtonID="btnAddMembersFromGIButtonsCancel" CloseButtonDialogResult="Abort">

        <px:PXFormView ID="frmAddMembersFromGI" runat="server" CaptionVisible="False" DataMember="AddMembersFromGIFilterView" DataSourceID="ds"
                       Width="100%" Height="85%" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule20" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                <px:PXSelector runat="server" ID="edGIDesignID" DataField="GIDesignID" CommitChanges="True"
                               AllowEdit="True" OnEditRecord="anySelector_OnEditRecord"
                               EditImages-Normal="control@WebN" EditImages-Disabled="control@WebD"
                               EditImages-Hover="control@WebH" EditImages-Pushed="control@WebP" />
                <px:PXSelector runat="server" ID="edSharedGIFilter" DataField="SharedGIFilter" CommitChanges="true" DisplayMode="Text" AutoRefresh="true" />
            </Template>
            <CallbackCommands>
                <Save RepaintControls="None" />
            </CallbackCommands>
        </px:PXFormView>
        <px:PXPanel ID="pnlAddMembersFromGIButtons" runat="server" CommandSourceID="ds" SkinID="Buttons">
            <px:PXButton ID="btnAddMembersFromGIButtonsOK" runat="server" CommandSourceID="ds" Text="Add" DialogResult="OK"/>
            <px:PXButton ID="btnAddMembersFromGIButtonsCancel" runat="server" CommandSourceID="ds" Text="Cancel" DialogResult="Abort"/>
        </px:PXPanel>
    </px:PXSmartPanel>

    <px:PXSmartPanel ID="pnlAddMembersFromMarketingLists" runat="server" Key="AddMembersFromMarketingListsFilterView" 
                     Caption="Add Members from Marketing Lists" CaptionVisible="true" AutoRepaint="True" AllowResize="False" AutoReload="True"
                     Width="520px" Height="314px" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" ShowMaximizeButton="True"
                     CancelButtonID="btnAddMembersFromMarketingListsCancel" CloseButtonDialogResult="Abort">

        <px:PXGrid runat="server" ID="grdAddMembersFromMarketingLists" DataSourceID="ds"  SkinID="Details"
                   AutoAdjustColumns="true" FilesIndicator="false" NoteIndicator="false" ActionsPosition="Top"
                   AllowSearch="true" AllowPaging="False" SyncPosition="true"
                   AllowFilter="true" FastFilterFields="MailListCode,Name,OwnerID"
                   Width="100%" Height="85%">
            <Levels>
                <px:PXGridLevel DataMember="AddMembersFromMarketingListsFilterView" >
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="True" />
                        <px:PXGridColumn DataField="MailListCode" LinkCommand="AddMembersFromMarketingListsFilterView_ViewDetails" />
                        <px:PXGridColumn DataField="Name" />
                        <px:PXGridColumn DataField="Status" />
                        <px:PXGridColumn DataField="OwnerID" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="OwnerID_description" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="CreatedDateTime" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="LastModifiedDateTime" SyncVisible="false" Visible="false"/>

                    </Columns>
                    <Mode AllowAddNew="false" AllowColMoving="false" AllowDelete="false"
                          AllowFormEdit="false" AllowSort="true" AllowUpdate="true"
                          AllowDragRows="false" AllowRowSizing="false" InitNewRow="false"
                          AllowUpload="false"/>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" />
            <ActionBar PagerVisible="False">
                <Actions>
                    <FilterBar ToolBarVisible="Top" Order="0" GroupIndex="0" />
                    <AddNew ToolBarVisible="False"/>
                    <Delete ToolBarVisible="False"/>
                    <ExportExcel ToolBarVisible="False"/>
                </Actions>
            </ActionBar>
        </px:PXGrid>
        <px:PXPanel ID="pnlAddMembersFromMarketingListsButtons" runat="server" CommandSourceID="ds" SkinID="Buttons">
            <px:PXButton ID="btnAddMembersFromMarketingListsOK" runat="server" CommandSourceID="ds" Text="Add" DialogResult="OK"/>
            <px:PXButton ID="btnAddMembersFromMarketingListsCancel" runat="server" CommandSourceID="ds" Text="Cancel" DialogResult="Abort"/>
        </px:PXPanel>
    </px:PXSmartPanel>

    <px:PXSmartPanel ID="pnlAddMembersFromCampaigns" runat="server" Key="AddMembersFromCampaignsFilterView"
                     Caption="Add Members from Campaigns" CaptionVisible="true" AutoRepaint="True" AllowResize="False" AutoReload="True"
                     Width="520px" Height="314px" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" ShowMaximizeButton="True"
                     CancelButtonID="btnAddMembersFromCampaignsCancel" CloseButtonDialogResult="Abort">

        <px:PXGrid runat="server" ID="grdAddMembersFromCampaigns" DataSourceID="ds" SkinID="Details"
                   AutoAdjustColumns="true" FilesIndicator="false" NoteIndicator="false" ActionsPosition="Top"
                   AllowSearch="true" AllowPaging="False" SyncPosition="true"
                   AllowFilter="true" FastFilterFields="CampaignID,CampaignName,PromoCodeID,OwnerID"
                   Width="100%" Height="85%">
            <Levels>
                <px:PXGridLevel DataMember="AddMembersFromCampaignsFilterView" >
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="True" />
                        <px:PXGridColumn DataField="CampaignID" LinkCommand="AddMembersFromCampaignsFilterView_ViewDetails" />
                        <px:PXGridColumn DataField="CampaignName" />
                        <px:PXGridColumn DataField="CampaignType" />
                        <px:PXGridColumn DataField="Status" />
                        <px:PXGridColumn DataField="StartDate" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="EndDate" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="PromoCodeID" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="CreatedDateTime" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="LastModifiedDateTime" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="OwnerID" SyncVisible="false" Visible="false"/>
                        <px:PXGridColumn DataField="OwnerID_description" SyncVisible="false" Visible="false"/>
                    </Columns>
                    <Mode AllowAddNew="false" AllowColMoving="false" AllowDelete="false"
                          AllowFormEdit="false" AllowSort="true" AllowUpdate="true"
                          AllowDragRows="false" AllowRowSizing="false" InitNewRow="false"
                          AllowUpload="false"/>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" />
            <ActionBar PagerVisible="False">
                <Actions>
                    <FilterBar ToolBarVisible="Top" Order="0" GroupIndex="0" />
                    <AddNew ToolBarVisible="False"/>
                    <Delete ToolBarVisible="False"/>
                    <ExportExcel ToolBarVisible="False"/>
                </Actions>
            </ActionBar>
        </px:PXGrid>
        <px:PXPanel ID="pnlAddMembersFromCampaignsButtons" runat="server" CommandSourceID="ds" SkinID="Buttons">
            <px:PXButton ID="btnAddMembersFromCampaignsOK" runat="server" CommandSourceID="ds" Text="Add" DialogResult="OK"/>
            <px:PXButton ID="btnAddMembersFromCampaignsCancel" runat="server" CommandSourceID="ds" Text="Cancel" DialogResult="Abort"/>
        </px:PXPanel>
    </px:PXSmartPanel>

</asp:Content>
