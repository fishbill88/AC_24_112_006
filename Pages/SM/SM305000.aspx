<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM305000.aspx.cs" Inherits="Page_SM305000" Title="Teams" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.MSTeams.Graph.SM.TeamsChannelMaint" PrimaryView="Teams">
        <CallbackCommands>
			<px:PXDSCallbackCommand Name="TestNotification" DependOnGrid="gridChannels" Visible="False" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXSplitContainer runat="server" ID="sp1" Orientation="Vertical" SplitterPosition="700">
        <AutoSize Enabled="True" Container="Window" />
        <Template1>
			<px:PXGrid ID="gridTeams" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="100%"
					   SkinID="Details" NotesIndicator="False" NoteField="" FilesIndicator="False" SyncPosition="true" AllowPaging="true" Caption="Teams" CaptionVisible="true">
				<ActionBar>
					<Actions>
						<Refresh MenuVisible="False" ToolBarVisible="False" />
						<AddNew MenuVisible="False" ToolBarVisible="False" />
						<Delete MenuVisible="False" ToolBarVisible="False" />
					</Actions>
				</ActionBar>
				<AutoCallBack ActiveBehavior="True" Command="Refresh" Target="tab">
					<Behavior CommitChanges="True" RepaintControlsIDs="tab,gridMembers,gridChannels"/>
				</AutoCallBack>
				<AutoSize Container="Window" Enabled="True" MinHeight="150" />
				<Mode AllowAddNew="False" AllowDelete="False" />
				<Levels>
					<px:PXGridLevel DataMember="Teams">
						<Columns>
							<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" Width="80px" />
							<px:PXGridColumn DataField="TeamPhoto" AllowFocus="False" DisplayMode="Value" TextAlign="Center" Type="Icon" Width="64px" />
							<px:PXGridColumn DataField="DisplayName" Width="150px" />
							<px:PXGridColumn DataField="Description" Width="200px" />
						</Columns>
					</px:PXGridLevel>
				</Levels>
			</px:PXGrid>
		</Template1>
		<Template2>
			<px:PXGrid ID="gridChannels" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="100%" Caption="Channels" CaptionVisible="true"
					   SkinID="Details" NotesIndicator="False" NoteField="" FilesIndicator="False" SyncPosition="true" AllowPaging="true">
				<ActionBar>
					<Actions>
						<Refresh MenuVisible="False" ToolBarVisible="False" />
						<AddNew MenuVisible="False" ToolBarVisible="False" />
						<Delete MenuVisible="False" ToolBarVisible="False" />
					</Actions>
					<CustomItems>
						<px:PXToolBarButton CommandSourceID="ds" CommandName="TestNotification" StateColumn="IsNotificationConfigured" />
					</CustomItems>
				</ActionBar>
				<AutoSize Container="Window" Enabled="True" MinHeight="150" />
				<Mode AllowAddNew="False" AllowDelete="False" />
				<Levels>
					<px:PXGridLevel DataMember="Channels">
						<Columns>
							<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" Width="80px" />
							<px:PXGridColumn DataField="DisplayName" Width="200px" />
							<px:PXGridColumn DataField="NotificationUrl" Width="350px" />
							<px:PXGridColumn DataField="IsNotificationConfigured" AllowShowHide="False" Visible="False"/>
						</Columns>
					</px:PXGridLevel>
				</Levels>
			</px:PXGrid>
        </Template2>
    </px:PXSplitContainer>
</asp:Content>