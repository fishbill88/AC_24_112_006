<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"	ValidateRequest="false"
	CodeFile="SM204006.aspx.cs" Inherits="Pages_SM_SM204006" Title="Teams Notification" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="Notifications"
		TypeName="PX.SM.SMTeamsNotificationMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="createBusinessEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="viewBusinessEvent" Visible="False" DependOnGrid="grdSendByEvents" />
		</CallbackCommands>	 
		<DataTrees> 
			<px:PXTreeDataMember TreeView="EntityItems" TreeKeys="Key"/>
            <px:PXTreeDataMember TreeView="PreviousEntityItems" TreeKeys="Key"/>
            <px:PXTreeDataMember TreeView="ScreenEmailItems" TreeKeys="Key"/>
		</DataTrees>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="PXFormView1" runat="server" DataSourceID="ds" DataMember="Notifications"
		Width="100%" DefaultControlID="ednotificationID">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="L" />
              <px:PXSelector runat="server" ID="PXSelector1" DataField="NotificationID" FilterByAllFields="True" AutoRefresh="True" TextField="Name" NullText="<NEW>" DataSourceID="ds">
				<GridProperties>
					<Columns>
						<px:PXGridColumn DataField="NotificationID" Width="60px"  />
						<px:PXGridColumn DataField="Name" Width="120px"/>
                          <px:PXGridColumn DataField="Subject" Width="220px"/>
                          <px:PXGridColumn DataField="ScreenID" Width="60px"/>
					</Columns>
				</GridProperties>
			</px:PXSelector>
 
		  <px:PXTextEdit ID="edName" runat="server" DataField="Name" AlreadyLocalized="False" DefaultLocale=""  />
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" ColumnSpan="2" />
			<px:PXTreeSelector ID="edsubject" runat="server" DataField="Subject" 
				TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
				ShowRootNode="false" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
				AppendSelectedValue="true" AutoRefresh="true" TreeDataMember="EntityItems">
				<DataBindings>
					<px:PXTreeItemBinding DataMember="EntityItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
				</DataBindings>
			</px:PXTreeSelector>
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
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XS" ControlSize="M" />
			<px:PXSelector ID="edScreenID" runat="server" DataField="ScreenID" DisplayMode="Hint" FilterByAllFields="true" CommitChanges="True" />
			<px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="M" />
			<px:PXSelector runat="server" ID="edLocale" DataField="LocaleName" Size="M" DisplayMode="Text" />
            <px:PXLayoutRule runat="server" LabelsWidth="S" 	ControlSize="M" Merge="True" />
			<px:PXCheckBox SuppressLabel="True" ID="chkShowReportTabExpr" runat="server" DataField="ShowReportTabExpr" />
			<px:PXCheckBox SuppressLabel="True" ID="chkShowSendByEventsTabExpr" runat="server" DataField="ShowSendByEventsTabExpr" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Height="150px" Style="z-index: 100" Width="100%"
		DataSourceID="ds" DataMember="CurrentNotification">
		<Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
		<Items>
			<px:PXTabItem Text="Message">
				<Template>
				 <px:PXRichTextEdit ID="edBody" runat="server" EncodeInstructions="true" DataField="Body" Style="border-width: 0px; border-top-width: 1px;
				        width: 100%;" AllowInsertParameter="true" AllowInsertPrevParameter="True" AllowPlaceholders="true" AllowAttached="true" AllowSearch="true" AllowMacros="true" AllowLoadTemplate="true" AllowSourceMode="true"
				        OnBeforePreview="edBody_BeforePreview" OnBeforeFieldPreview="edBody_BeforeFieldPreview" FileAttribute="embedded">
				        <AutoSize Enabled="True" MinHeight="216" />
				        <InsertDatafield DataSourceID="ds" DataMember="EntityItems" TextField="Name" ValueField="Path"	ImageField="Icon" />
				        <InsertDatafieldPrev DataSourceID="ds" DataMember="PreviousEntityItems" TextField="Name" ValueField="Path"	ImageField="Icon" />
                        <LoadTemplate TypeName="PX.SM.SMNotificationMaint" DataMember="NotificationsRO" ViewName="NotificationTemplate" ValueField="notificationID" TextField="Name" DataSourceID="ds" Size="M"/>
			        </px:PXRichTextEdit>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Send by Events" BindingContext="PXFormView1" VisibleExp="DataControls[&quot;chkShowSendByEventsTabExpr&quot;].Value=1">
				<Template>
                    <px:PXGrid ID="grdSendByEvents" runat="server" DataSourceID="ds" Width="100%" SkinID="Details"
						AutoAdjustColumns="True" AllowPaging="False" SyncPosition="True">
						<ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Create Business Event">
                                   <AutoCallBack Command="createBusinessEvent" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="BusinessEvents">
								<RowTemplate>
									<px:PXSelector ID="edEventName" runat="server" DataField="Name" AutoRefresh="True" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="Name" LinkCommand="ViewBusinessEvent" />
									<px:PXGridColumn DataField="Description" />
									<px:PXGridColumn DataField="Active" Type="CheckBox" />
									<px:PXGridColumn DataField="Type" Width="200" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<Mode AllowUpdate="False" />
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="250" />
	</px:PXTab>
	<px:PXSmartPanel ID="pnlCreateBusinessEvent" runat="server" Key="NewEventData" Caption="Create Business Event" CaptionVisible="True"
        AutoReload="True" AutoRepaint="True" Width="500" Height="100">
	    <px:PXFormView ID="frmCreateBusinessEvent" runat="server" DataMember="NewEventData" DataSourceID="ds" Width="500" Height="100" SkinID="Transparent">
		    <Template>
			    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="L" />
			    <px:PXTextEdit runat="server" ID="txtName" DataField="Name" CommitChanges="True" />
		    </Template>
            <AutoSize Enabled="True" />
	    </px:PXFormView>
		<px:PXPanel ID="pnlButtons" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnOK" runat="server" DialogResult="OK" Text="OK" />
			<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>