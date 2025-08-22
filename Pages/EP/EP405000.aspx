<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="EP405000.aspx.cs" Inherits="Page_EP405000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server" Visible="false">
	<px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Width="100%" Height="0px"
		PrimaryView="Filter" TypeName="PX.Objects.EP.ActivitiesMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True"
				PopupCommand="Cancel" PopupCommandTarget="ds" />
			<px:PXDSCallbackCommand Name="ViewActivity" Visible="False" CommitChanges="True"
				DependOnGrid="gridActivities" />
		</CallbackCommands>
	</px:PXDataSource>
	<px:PXFormView ID="edFilter" runat="server" DataSourceID="ds" DataMember="Filter" CaptionVisible="false" Height="0px" />
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phF" runat="server">
	<pxa:PXGridWithPreview ID="gridActivities" runat="server" DataSourceID="ds" Width="100%" Height="420px" 
		AllowSearch="True" DataMember="Activities" AllowPaging="true" NoteField="NoteText"
		FilesField="NoteFiles" BorderWidth="1px" BorderColor="Gray" GridSkinID="Details" SplitterStyle="z-index: 100; border-top: solid 1px Gray;  border-bottom: solid 1px Gray"
		PreviewPanelStyle="z-index: 100; background-color: Window" PreviewPanelSkinID="Preview"
		BlankFilterHeader="All Activities" MatrixMode="true" GridStyle="border: 0px" PrimaryViewControlID="edFilter" >
		<ActionBar DefaultAction="cmdViewActivity">
			<Actions>
				<AddNew Enabled="False" />
			</Actions>
			<CustomItems>
				<px:PXToolBarButton Key="cmdAddTask">
				    <AutoCallBack Command="NewTask" Enabled="True" Target="ds" />
				    <PopupCommand Command="Cancel" Enabled="true" Target="ds" />
				</px:PXToolBarButton>
				<px:PXToolBarButton Key="cmdAddEvent">
				    <AutoCallBack Command="NewEvent" Enabled="True" Target="ds" />
				    <PopupCommand Command="Cancel" Enabled="true" Target="ds" />
				</px:PXToolBarButton>
				<px:PXToolBarButton Key="cmdAddEmail">
				    <AutoCallBack Command="NewMailActivity" Enabled="True" Target="ds" />
				    <PopupCommand Command="Cancel" Enabled="true" Target="ds" />
				</px:PXToolBarButton>
				<px:PXToolBarButton Key="cmdAddActivity">
				    <AutoCallBack Command="NewActivity" Enabled="True" Target="ds" />
				    <PopupCommand Command="Cancel" Enabled="true" Target="ds" />
					<ActionBar MenuVisible="true" />
				</px:PXToolBarButton>
				<px:PXToolBarButton Key="cmdViewActivity" Visible="false">
					<ActionBar MenuVisible="false" />
					<AutoCallBack Command="ViewActivity" Enabled="True" Target="ds" />
				</px:PXToolBarButton>
			</CustomItems>
		</ActionBar>
		<Levels>
			<px:PXGridLevel  DataMember="Activities">
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
				<RowTemplate>
					<px:PXSelector ID="edActivityAssignedTo" runat="server" DataField="ActivityOwner__fullname"
						  AllowEdit="true">
					</px:PXSelector>
					<px:PXSelector ID="edActivityOwnerID" runat="server" DataField="CreatedByID" 
						 AllowEdit="true">
					</px:PXSelector>
					<px:PXSelector ID="edActivitySubject" runat="server"  DataField="Subject"
						 AllowEdit="true" />
				</RowTemplate>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Enabled="True" Container="Window" />
		<GridMode AllowAddNew="False" AllowUpdate="False" />
		<PreviewPanelTemplate>
			<px:PXHtmlView ID="edBody" runat="server" DataField="previewHtml" TextMode="MultiLine" MaxLength="50"
				Width="100%" Height="100px" SkinID="Label">
                                      <AutoSize Container="Parent" Enabled="true" />
                                </px:PXHtmlView>
			</px:PXHtmlView> 
		</PreviewPanelTemplate>
	</pxa:PXGridWithPreview>
</asp:Content>
