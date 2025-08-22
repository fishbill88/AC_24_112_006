<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="CR306040.aspx.cs" Inherits="Page_CR306040"
    Title="Teams Activity" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Width="100%" Visible="true" PrimaryView="Message"
        TypeName="PX.Objects.CR.CRTeamsActivityMaint">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" Visible="False" />
			<px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" ClosePopup="False"/>
            <px:PXDSCallbackCommand CommitChanges="True" Name="saveClose" Visible="False" PopupVisible="True" ClosePopup="True" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="save" PopupVisible="True" />
            <px:PXDSCallbackCommand Name="Delete" PostData="Self" ClosePopup="true" PopupVisible="true" />
            <px:PXDSCallbackCommand Name="sendMessage" ClosePopup="True" />
            <px:PXDSCallbackCommand Name="Message$Select_RefNote" Visible="False" />
            <px:PXDSCallbackCommand Name="Message$Navigate_ByRefNote" Visible="False" />
            <px:PXDSCallbackCommand Name="Message$Attach_RefNote" Visible="False" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="server">
    <px:PXFormView ID="message" runat="server" DataSourceID="ds" DataMember="Message" Width="100%"
        OverflowY="Hidden" NoteIndicator="true" NoteField="NoteText"
        FilesIndicator="true" FilesField="NoteFiles" DefaultControlID="edSubject" 
        AllowCollapse="false">
        <ContentLayout SpacingSize="Small" AutoSizeControls="true" />
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XS" ColumnWidth="65%" />
			<px:PXSelector ID="edChannelID" runat="server" DataField="ChannelID" AutoComplete="False" AutoRefresh="True" CommitChanges="True" 
						   DisplayMode="Text" MinDropHeight="335" Style="margin-bottom: 8px">
				<GridProperties FastFilterFields="SMTeamsTeam__DisplayName,DisplayName">
					<Columns>
						<px:PXGridColumn DataField="SMTeamsTeam__TeamPhoto" AllowFocus="False" DisplayMode="Value" TextAlign="Center" Type="Icon" Width="64px" />
						<px:PXGridColumn DataField="SMTeamsTeam__DisplayName" />
						<px:PXGridColumn DataField="DisplayName" />
					</Columns>
				</GridProperties>
			</px:PXSelector>
			<px:PXSelector ID="edMemberID" runat="server" DataField="MemberID" AutoComplete="False" AutoRefresh="True" CommitChanges="True"
						   DisplayMode="Text" MinDropHeight="335" Style="margin-bottom: 8px" >
				<GridProperties>
					<Columns>
						<px:PXGridColumn DataField="TeamPhoto" AllowFocus="False" DisplayMode="Value" TextAlign="Center" Type="Icon" Width="64px" />
						<px:PXGridColumn DataField="DisplayName" />
						<px:PXGridColumn DataField="UserPrincipalName" />
					</Columns>
				</GridProperties>
			</px:PXSelector>
            <px:PXTextEdit ID="edSubject" runat="server" DataField="Subject" Width="570px" />
            <px:PXLayoutRule ID="PXLayoutRule2" runat="server" ColumnWidth="35%" StartColumn="True" SuppressLabel="True" />
            <px:PXHtmlView ID="edEntityDescription" runat="server" DataField="EntityDescription"
                TextMode="MultiLine" SkinID="Label" Width="250px" Style="border: solid 1px Gray; background-color: White; height: 132px;">
            </px:PXHtmlView>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" DataSourceID="ds" Width="100%" DataMember="CurrentMessage">
        <Items>
            <px:PXTabItem Text="Message">
                <Template>
                    <px:PXRichTextEdit ID="PXRichTextEdit1" runat="server" CommitChanges="True" Style="border-width: 0px; width: 100%;"
						DataField="Body" AllowImageEditor="true" AllowLinkEditor="true" AllowLoadTemplate="false" AllowAttached="true" 
						FilesContainer="message" AllowSearch="true" AllowSourceMode="true">
                        <AutoSize Enabled="True" />
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule11" runat="server" StartColumn="True" LabelsWidth="S"
                        ControlSize="M" />
                    <px:PXLayoutRule ID="PXLayoutRule7" runat="server" ColumnSpan="2" />
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" />
                    <px:PXDateTimeEdit ID="edStartDate_Date" runat="server" DataField="StartDate_Date"
                        CommitChanges="True" />
                    <px:PXDateTimeEdit ID="edStartDate_Time" runat="server" DataField="StartDate_Time"
                        TimeMode="true" SuppressLabel="true" Width="84" CommitChanges="True" />
                    <px:PXLayoutRule ID="PXLayoutRule4" runat="server" />
						<px:PXCheckBox ID="edIsIncome" runat="server" DataField="IsIncome" Enabled="False"/>
                    <px:PXCheckBox ID="edIsPrivate" runat="server" DataField="IsPrivate" />
                    <px:PXLayoutRule ID="PXLayoutRule9" runat="server" LabelsWidth="S" ControlSize="M" />
						<px:PXSelector ID="edWorkgroupID" runat="server" DataField="WorkgroupID" CommitChanges="True" />
                    <px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" CommitChanges="True" AutoRefresh="true" />
                    <pxa:PXRefNoteSelector ID="edRefEntity" runat="server" DataField="Source" NoteIDDataField="RefNoteID"
                        MaxValue="0" MinValue="0" ValueType="Int64">
                        <EditButton CommandName="Message$Navigate_ByRefNote" CommandSourceID="ds" />
                        <LookupButton CommandName="Message$Select_RefNote" CommandSourceID="ds" />
                        <LookupPanel DataMember="Message$RefNoteView" DataSourceID="ds" TypeDataField="Type"
                            IDDataField="RefNoteID" />
                    </pxa:PXRefNoteSelector>
						<px:PXSelector runat="server" ID="edParentNoteID" DataField="ParentNoteID" DisplayMode="Text" TextMode="Search" TextField="Subject" AllowEdit="True" CommitChanges="True"/>					
					    <%--TimeActivity--%>
						<px:PXLayoutRule ID="PXLayoutRule10" runat="server" Merge="True" />
                             <px:PXSegmentMask ID="edProject" runat="server" DataField="TimeActivity.ProjectID" HintField="description" CommitChanges="True" />
                             <px:PXCheckBox ID="edCertifiedjob" runat="server" DataField="TimeActivity.CertifiedJob" />
						<px:PXLayoutRule ID="PXLayoutRule5" runat="server" />
						<px:PXSegmentMask ID="edProjectTaskID" runat="server" DataField="TimeActivity.ProjectTaskID" HintField="description"  AutoRefresh="true" CommitChanges="True"/>
                        <px:PXSegmentMask ID="edCostCodeIDDetails" runat="server" DataField="TimeActivity.CostCodeID" AutoRefresh="true" />
                     <px:PXSelector ID="edLaborItemID" runat="server" DataField="TimeActivity.LabourItemID" CommitChanges="True" />
                             <px:PXSelector ID="edUnionID" runat="server" DataField="TimeActivity.UnionID" />
						<px:PXDropDown ID="edMPStatus" runat="server" AllowNull="False" DataField="MPStatus" CommitChanges="True" />
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />

						<px:PXCheckBox ID="chkTrackTime" runat="server" DataField="TimeActivity.TrackTime" CommitChanges="True" />
                    <px:PXDropDown ID="edStatus" runat="server" AllowNull="False" DataField="TimeActivity.ApprovalStatus"
                        CommitChanges="True" />
						<px:PXSelector ID="edApprover" runat="server" DataField="TimeActivity.ApproverID" Enabled="False" />
						<px:PXSelector ID="edEType" runat="server" DataField="TimeActivity.EarningTypeID" AutoRefresh="true" CommitChanges="True" />
						<px:PXSelector ID="edWorkCodeID" runat="server" DataField="TimeActivity.WorkCodeID" />
                               <px:PXTimeSpan TimeMode="true" ID="edTimeSpent" runat="server" DataField="TimeActivity.TimeSpent" CommitChanges="True" Size="SM" InputMask="hh:mm" />
						<px:PXTimeSpan TimeMode="True" ID="edOvertimeSpent" runat="server" DataField="TimeActivity.OvertimeSpent" InputMask="hh:mm" Enabled="False"/>
						<px:PXCheckBox ID="chkIsBillable" runat="server" DataField="TimeActivity.IsBillable" Text="Billable"
                        CommitChanges="True" />
						<px:PXCheckBox ID="chkReleased" runat="server" DataField="TimeActivity.Released" Text="Released"  />
						<px:PXTimeSpan TimeMode="True" ID="edTimeBillable1" runat="server" DataField="TimeActivity.TimeBillable" InputMask="hh:mm"/>
						<px:PXTimeSpan TimeMode="True" ID="edOvertimeBillable" runat="server" DataField="TimeActivity.OvertimeBillable" InputMask="hh:mm" Enabled="False"/>
						<px:PXTextEdit ID="edTNoteID" runat="server" DataField="TimeActivity.NoteID" Visible="False">
                        <AutoCallBack Command="Cancel" Enabled="True" Target="tab" />
                    </px:PXTextEdit>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Enabled="True" Container="Window" />
    </px:PXTab>
</asp:Content>