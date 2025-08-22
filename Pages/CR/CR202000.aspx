<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CR202000.aspx.cs" Inherits="Page_CR202000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource EnableAttributes="true" UDFTypeField="CampaignType" ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.CampaignMaint" PrimaryView="Campaign">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewCampaignMemberActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ViewActivity" DependOnGrid="gridActivities" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="AddAction" Visible="False" />
			<px:PXDSCallbackCommand Name="DeleteAction" Visible="False" />
			<px:PXDSCallbackCommand Name="AddOpportunity" CommitChanges="True" Visible="False" />
			<px:PXDSCallbackCommand Name="AddContact" Visible="False" />
			<px:PXDSCallbackCommand Name="LinkToContact" Visible="False" />
			<px:PXDSCallbackCommand Name="LinkToBAccount" Visible="False" />
			<px:PXDSCallbackCommand Name="InnerProcess" Visible="false" />
			<px:PXDSCallbackCommand Name="InnerProcessAll" Visible="false" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
		Caption="Campaign Summary" DataMember="Campaign" FilesIndicator="True" NoteIndicator="True"
		ActivityIndicator="False" ActivityField="NoteActivity" DefaultControlID="edCampaignID">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXSelector ID="edCampaignID" runat="server" DataField="CampaignID" DataSourceID="ds" Size="SM" AutoRefresh="True" />
			<px:PXLayoutRule runat="server" />
			<px:PXSelector ID="edCampaignType" runat="server" AllowNull="False" DataField="CampaignType" AllowEdit="true" CommitChanges="true" Size="SM" AutoRefresh="True" />
			<px:PXLayoutRule runat="server" ColumnSpan="2" />
			<px:PXTextEdit ID="edCampaignName" runat="server" AllowNull="False" DataField="CampaignName" />
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XS" ControlSize="SM" />
			<px:PXDropDown ID="edStatus" runat="server" AllowNull="False" DataField="Status" />

			<px:PXSelector ID="OwnerID" runat="server" DataField="OwnerID" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" AutoRefresh="true" CommitChanges="True" />
		</Template>
	</px:PXFormView>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="290px" DataSourceID="ds" DataMember="CampaignCurrent">
		<AutoSize Container="Window" Enabled="True" MinHeight="250" MinWidth="300" />
		<Items>

			<px:PXTabItem Text="Campaign Details">
				<Template>
					<px:PXPanel ID="PXPanel1" runat="server" RenderStyle="Simple" Style="margin: 9px;">
						
						<px:PXLayoutRule ID="PXLayoutRule6" runat="server" StartColumn="True" LabelsWidth="SM"	ControlSize="SM" />
						<px:PXLayoutRule ID="PXLayoutRule1" runat="server" GroupCaption="Planning" StartGroup="True" ControlSize="SM" />
						<px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" Size="SM" />
						<px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" Size="SM" />
						<px:PXSelector CommitChanges="True" ID="edWorkgroupID" runat="server" DataField="WorkgroupID" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" />
						<px:PXNumberEdit ID="edExpectedResponse" runat="server" DataField="ExpectedResponse" NullText="0.00" Size="SM" />
						<px:PXNumberEdit ID="edPlannedBudget" runat="server" DataField="PlannedBudget" NullText="0.00" Size="SM" />
						<px:PXNumberEdit ID="edExpectedRevenue" runat="server" DataField="ExpectedRevenue" NullText="0.00" Size="SM" />
						<px:PXTextEdit ID="edPromoCodeID" runat="server" AllowNull="False" DataField="PromoCodeID" />

						<px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="False" LabelsWidth="SM"	ControlSize="S" />
						<px:PXLayoutRule ID="PXLayoutRule2" runat="server" GroupCaption="Project Accounting Integration" StartGroup="True" />
						<px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID" AutoRefresh="True" AllowAddNew="True" AllowEdit="True" CommitChanges="True" Size="SM" />
						<px:PXSelector ID="edProjectTaskID" runat="server" DataField="ProjectTaskID" AutoRefresh="True" AllowAddNew="True" AllowEdit="True" CommitChanges="True" Size="SM" OnEditRecord="edProjectTaskID_EditRecord" />
						<px:PXLayoutRule ID="PXLayoutRule9" runat="server" />

						<px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartColumn="True" LabelsWidth="SM"	ControlSize="S" />
						<px:PXLayoutRule ID="PXLayoutRule5" runat="server" GroupCaption="Campaign Statistics" StartGroup="True" />
						
						<px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
						<px:PXNumberEdit ID="edContacts" runat="server" DataField="CalcCampaignCurrent.Contacts" Enabled="false"/>
						<px:PXNumberEdit ID="edMembersContacted" runat="server" DataField="CalcCampaignCurrent.MembersContacted" Enabled="false" />
						<px:PXNumberEdit ID="edMembersResponded" runat="server" DataField="CalcCampaignCurrent.MembersResponded" Enabled="false" />
						<px:PXNumberEdit ID="edLeadsGenerated" runat="server" DataField="CalcCampaignCurrent.LeadsGenerated" Enabled="false" />
						<px:PXNumberEdit ID="edLeadsConverted" runat="server" DataField="CalcCampaignCurrent.LeadsConverted" Enabled="false" />
						<px:PXNumberEdit ID="edOpportunities" runat="server" DataField="CalcCampaignCurrent.Opportunities" Enabled="false" />
						<px:PXNumberEdit ID="edClosedOpportunities" runat="server" DataField="CalcCampaignCurrent.ClosedOpportunities" Enabled="false" />
						<px:PXNumberEdit ID="edOpportunitiesValue" runat="server" DataField="CalcCampaignCurrent.OpportunitiesValue" NullText="0.00" />
						<px:PXNumberEdit ID="edClosedOpportunitiesValue" runat="server" DataField="CalcCampaignCurrent.ClosedOpportunitiesValue" NullText="0.00" />
					</px:PXPanel>
					<px:PXRichTextEdit ID="edDescription" runat="server" Style="border-width: 0px; width: 100%;" DataField="Description" 
						AllowLoadTemplate="false"  AllowDatafields="false"  AllowMacros="true" AllowSourceMode="true" AllowAttached="true" AllowSearch="true">
						<AutoSize Enabled="True" />
					</px:PXRichTextEdit>
				</Template>
			</px:PXTabItem>

			<!--#include file="~\Pages\CR\Includes\ActivityDetailsExt\ActivityDetailsExt.inc"-->

			<px:PXTabItem Text="Marketing Lists" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="grdCampaignMarketingLists" runat="server" SkinID="Details" NoteIndicator="false" FilesIndicator="false"
						Width="100%" Style="z-index: 100" AllowPaging="True" AdjustPageSize="Auto" ActionsPosition="Top" 
						AllowSearch="true" DataSourceID="ds" SyncPosition="true" AllowFilter="true"
						 FastFilterFields="CRMarketingList__MarketingListID,CRMarketingList__MailListCode,CRMarketingList__Name,CRMarketingList__OwnerID_description,CRMarketingList__GIDesignID" >
						<Levels>
							<px:PXGridLevel DataMember="CampaignMarketingLists" >
								<RowTemplate>
									<px:PXCheckBox runat="server" DataField="Selected" ID="CRMarketingListCheckSelect" CommitChanges="true" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="SelectedForCampaign" Type="CheckBox" CommitChanges="true" AutoCallBack="True" TextAlign="Center"/>
									<px:PXGridColumn DataField="MailListCode" LinkCommand="CampaignMarketingLists_CRMarketingList_ViewDetails" Width="155px" />
									<px:PXGridColumn DataField="Name" />
									<px:PXGridColumn DataField="OwnerID" LinkCommand="OwnerID_ViewDetails" />
									<px:PXGridColumn DataField="LastUpdateDate" Width="130px" />

									<px:PXGridColumn DataField="Status" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="WorkgroupID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Method" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Type" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="GIDesignID" Visible="false" SyncVisible="false" LinkCommand="GIDesignID_ViewDetails" />
									<px:PXGridColumn DataField="CreatedByID" Visible="false" SyncVisible="false" LinkCommand="CreatedByID_ViewDetails" />
									<px:PXGridColumn DataField="CreatedDateTime" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="LastModifiedByID" Visible="false" SyncVisible="false" LinkCommand="GIDesignID_ViewDetails" />
									<px:PXGridColumn DataField="LastModifiedDateTime" Visible="false" SyncVisible="false" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="True" AllowUpload="true" />
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100"/>
						<ActionBar DefaultAction="cmdViewDoc">
							<Actions>
								<AddNew Enabled = "false" />
								<Delete Enabled = "false" />
								<Upload Enabled = "false" />
								<Save Enabled="False" />
								<FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
								<Search ToolBarVisible="Top" />
							</Actions>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Members" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="grdCampaignMembers" runat="server" SkinID="Details" Height="400px" NoteIndicator="false"
						Width="100%" Style="z-index: 100" AllowPaging="True" AdjustPageSize="Auto" ActionsPosition="Top"
						AllowSearch="true" DataSourceID="ds" BorderWidth="0px" SyncPosition="true" MatrixMode="true" AllowFilter="true"
						 FastFilterFields="Contact__MemberName, Contact__Salutation, Contact__BAccountID, Contact__FullName, Contact__EMail, Contact__Phone1" >
						<Levels>
							<px:PXGridLevel DataMember="CampaignMembers" >
								<Columns>
									<px:PXGridColumn DataField="Selected" AllowCheckAll="True" AllowNull="False" AllowMove="False" AllowSort="False" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="Contact__ContactType" />
									<px:PXGridColumn DataField="ContactID" TextField="Contact__MemberName" AutoCallBack="true" DisplayMode="Text" CommitChanges="true" TextAlign="Left" LinkCommand="Contact_ViewDetails" Width="280px" />
									<px:PXGridColumn DataField="Contact__Salutation" AllowUpdate="False" />
									<px:PXGridColumn DataField="CRMarketingList__MailListCode" LinkCommand="CampaignMembers_CRMarketingList_ViewDetails" DisplayMode="Hint" />
									<px:PXGridColumn DataField="Contact__BAccountID" AllowUpdate="False" DisplayFormat="CCCCCCCCCC" DisplayMode="Value" LinkCommand="CampaignMembers_BAccount_ViewDetails"/>
									<px:PXGridColumn DataField="Contact__FullName" />
									<px:PXGridColumn DataField="Contact__EMail" AllowUpdate="False" />
									<px:PXGridColumn DataField="Contact__Phone1Type" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__Phone1" AllowUpdate="False" DisplayFormat="+# (###) ###-#### Ext:####" />
									<px:PXGridColumn DataField="Contact__IsActive" AllowNull="False" Visible="false" SyncVisible="false" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="OpportunityCreatedCount" AllowNull="False" />
									<px:PXGridColumn DataField="ActivitiesLogged" AllowNull="False" />
									<px:PXGridColumn DataField="CampaignID" Visible="False" AllowShowHide="False" />
									<px:PXGridColumn DataField="EmailSendCount" AllowNull="False" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__Phone2Type" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__Phone2" DisplayFormat="+#(###) ###-####" Visible="false" />
									<px:PXGridColumn DataField="Contact__Phone3Type" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__Phone3" DisplayFormat="+#(###) ###-####" Visible="false" />
									<px:PXGridColumn DataField="Contact__FaxType" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__Fax" DisplayFormat="+#(###) ###-####" Visible="false" />
									<px:PXGridColumn DataField="Contact__WebSite" Visible="false" />
									<px:PXGridColumn DataField="Contact__DateOfBirth" Visible="false" />
									<px:PXGridColumn DataField="Contact__Gender" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__MaritalStatus" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__Spouse" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__CreatedByID" Visible="false" LinkCommand="CreatedByID_ViewDetails" />
									<px:PXGridColumn DataField="Contact__LastModifiedByID" Visible="false" LinkCommand="LastModifiedByID_ViewDetails" />
									<px:PXGridColumn DataField="Contact__CreatedDateTime" Visible="false" />
									<px:PXGridColumn DataField="Contact__LastModifiedDateTime" Visible="false" />
									<px:PXGridColumn DataField="Contact__WorkgroupID" Visible="false" />
									<px:PXGridColumn DataField="Contact__OwnerID" Visible="false" SyncVisible="false" LinkCommand="OwnerID_ViewDetails" />
									<px:PXGridColumn DataField="Contact__ClassID" AllowNull="False" TextAlign="Center" Visible="false" LinkCommand="CampaignMembers_CRContactClass_ViewDetails"/>
									<px:PXGridColumn DataField="Contact__Source" Visible="false" />
									<px:PXGridColumn DataField="Contact__Title" Visible="false" />
									<px:PXGridColumn DataField="Contact__FirstName" />
									<px:PXGridColumn DataField="Contact__MidName" />
									<px:PXGridColumn DataField="Contact__LastName" />
									<px:PXGridColumn DataField="Address__AddressLine1" Visible="false" />
									<px:PXGridColumn DataField="Address__AddressLine2" Visible="false" />
									<px:PXGridColumn DataField="Contact__Status" />
									<px:PXGridColumn DataField="Contact__IsNotEmployee" Width="0px" AllowShowHide="Server" />
									<px:PXGridColumn DataField="CreatedDateTime" AllowUpdate="False" DisplayFormat="g" Visible="false"/>
									<px:PXGridColumn DataField="Address__City" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Address__State" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Address__PostalCode" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Address__CountryID" Visible="false" SyncVisible="false" LinkCommand="CountryID_ViewDetails" />
									<px:PXGridColumn DataField="Contact__ConsentAgreement" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__ConsentDate" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__ConsentExpirationDate" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__Method" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__NoCall" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__NoMarketing" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__NoEMail" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__NoMassMail" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Contact__ParentBAccountID" Visible="false" SyncVisible="false" LinkCommand="ParentBAccountID_ViewDetails" />
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
                                    <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID" FilterByAllFields="true" AutoRefresh="True" />                                    
								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
						<Mode AllowUpload="True"/>
						<ActionBar DefaultAction="cmdViewDoc">
							<Actions>
								<Delete Enabled = "false" />
								<FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
							</Actions>
							<CustomItems>
								<px:PXToolBarButton Text="Delete selected" Tooltip="Delete Selected Rows"  Key="cmdMultipleDelete" DisplayStyle="Image" ImageKey="RecordDel">
									<AutoCallBack Command="DeleteAction" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdUpdateListMembers" >
									<AutoCallBack Command="UpdateListMembers" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdAddActivity" >
									<AutoCallBack Command="NewCampaignMemberActivity" Target="ds" ></AutoCallBack>
									<ActionBar />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdClearMembers" >
									<AutoCallBack Command="ClearMembers" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<AutoSize Enabled="True" MinHeight="200" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Generated Leads" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="Leads" runat="server" DataSourceID="ds" Height="150px" Width="100%" 
						ActionsPosition="Top" AllowPaging="True" AutoGenerateColumns="AppendDynamic" SkinID="Inquire" ExternalFilter="true">
						<Levels>
							<px:PXGridLevel DataMember="Leads">
								<Columns>
									<px:PXGridColumn DataField="ClassID" AllowNull="False" LinkCommand="Leads_CRLeadClass_ViewDetails" />
									<px:PXGridColumn DataField="DisplayName" AllowUpdate="False" LinkCommand="Leads_ViewDetails">
										<NavigateParams>
											<px:PXControlParam Name="ContactID" ControlID="leads" PropertyName="DataValues[&quot;ContactID&quot;]" />
										</NavigateParams>
									</px:PXGridColumn>
									<px:PXGridColumn DataField="Status" />
									<px:PXGridColumn DataField="Resolution" />
									<px:PXGridColumn DataField="Source" />
									<px:PXGridColumn DataField="FullName" />
									<px:PXGridColumn DataField="OwnerID" DisplayMode="Text" />
									<px:PXGridColumn DataField="WorkgroupID" />
									<px:PXGridColumn DataField="IsActive" AllowNull="False" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Title" Visible="False" />
									<px:PXGridColumn DataField="Salutation" />
									<px:PXGridColumn DataField="ContactID" AllowUpdate="False" Visible="false" AllowShowHide="False" />
									<px:PXGridColumn DataField="EMail" />
									<px:PXGridColumn DataField="Address__AddressLine1" Visible="False" />
									<px:PXGridColumn DataField="Address__AddressLine2" Visible="False" />
									<px:PXGridColumn DataField="Phone1" DisplayFormat="+#(###) ###-####" />
									<px:PXGridColumn DataField="Phone2" DisplayFormat="+#(###) ###-####" />
									<px:PXGridColumn DataField="Phone3" DisplayFormat="+#(###) ###-####" />
									<px:PXGridColumn DataField="Fax" DisplayFormat="+#(###) ###-####" />
									<px:PXGridColumn DataField="WebSite" />
									<px:PXGridColumn DataField="DateOfBirth" />
									<px:PXGridColumn DataField="CreatedByID_Creator_Username" />
									<px:PXGridColumn DataField="LastModifiedByID_Modifier_Username" />
									<px:PXGridColumn DataField="CreatedDateTime" />
									<px:PXGridColumn DataField="LastModifiedDateTime" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
						<ActionBar PagerVisible="False">
							<Actions>
								<Save Enabled="False" />
								<AddNew Enabled="False" />
								<Delete Enabled="False" />
								<EditRecord Enabled="False" />
								<FilterShow Enabled="False" />
								<FilterSet Enabled="False" />
							</Actions>
							<CustomItems>
								<px:PXToolBarButton Key="cmdAddContact" ImageKey="AddNew" Tooltip="Add New Lead" DisplayStyle="Image">
									<AutoCallBack Command="AddContact" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Opportunities" LoadOnDemand="true">
				<Template>
					<px:PXGrid ID="gridOpportunities" runat="server" AutoGenerateColumns="AppendDynamic" DataSourceID="ds" Height="423px"
						Width="100%" AllowSearch="True" ActionsPosition="Top" SkinID="Inquire">
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<Levels>
							<px:PXGridLevel DataMember="Opportunities">
								<Columns>
									<px:PXGridColumn DataField="ClassID" AllowNull="False" LinkCommand="Opportunities_CROpportunityClass_ViewDetails" />
									<px:PXGridColumn DataField="OpportunityID" LinkCommand="Opportunities_ViewDetails" />
									<px:PXGridColumn DataField="Subject" AllowNull="False" />
									<px:PXGridColumn DataField="StageID" AllowNull="False" />
									<px:PXGridColumn DataField="CROpportunityProbability__Probability" AllowNull="False" />
									<px:PXGridColumn DataField="Status" AllowNull="False" />
									<px:PXGridColumn DataField="Resolution" AllowNull="False" />
									<px:PXGridColumn DataField="CuryProductsAmount" AllowNull="False" />
									<px:PXGridColumn DataField="CuryID" AllowNull="False" />
									<px:PXGridColumn DataField="CloseDate" AllowNull="False" />
									<px:PXGridColumn DataField="Source" AllowNull="False" />
									<px:PXGridColumn DataField="OwnerID" AllowNull="False" />
									<px:PXGridColumn DataField="BAccountID" AllowNull="False" LinkCommand="Opportunities_BAccount_ViewDetails" />
									<px:PXGridColumn DataField="ContactID" AllowNull="False" DisplayMode="Text" LinkCommand="Opportunities_Contact_ViewDetails" />
									<px:PXGridColumn DataField="CROpportunityClass__Description" AllowShowHide="true" Visible="false" SyncVisible="false" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						<ActionBar ActionsText="False" DefaultAction="cmdOpportunityDetails" PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Key="cmdAddOpportunity" ImageKey="AddNew" Tooltip="Add New Opportunity" DisplayStyle="Image">
									<AutoCallBack Command="AddOpportunity" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Attributes" LoadOnDemand="true">
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

		</Items>
	</px:PXTab>
</asp:Content>