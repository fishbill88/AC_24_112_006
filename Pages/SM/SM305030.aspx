<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM305030.aspx.cs" Inherits="Page_SM305030" Title="Teams" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.MSTeams.Graph.SM.TeamsMemberMaint" PrimaryView="Members" />
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server"> 
	<px:PXGrid ID="gridMembers" runat="server" DataSourceID="ds"
			   AllowPaging="False" FilesIndicator="False" NoteField="" NotesIndicator="False" SkinID="Details" Width="100%">
		<ActionBar>
			<Actions>
				<Refresh MenuVisible="False" ToolBarVisible="False" />
				<AddNew MenuVisible="False" ToolBarVisible="False" />
				<Delete MenuVisible="False" ToolBarVisible="False" />
			</Actions>
		</ActionBar>
		<AutoSize Container="Window" Enabled="True" />
		<Mode AllowAddNew="False" AllowDelete="False" />
		<Levels>
			<px:PXGridLevel DataMember="Members">
				<Columns>
					<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" Width="80px" />
					<px:PXGridColumn DataField="TeamPhoto" AllowFocus="False" DisplayMode="Value" TextAlign="Center" Type="Icon" Width="64px" />
					<px:PXGridColumn DataField="UserPrincipalName" Width="300px" />
					<px:PXGridColumn DataField="DisplayName" Width="250px" />
					<px:PXGridColumn DataField="ContactID" TextAlign="Left" DisplayMode="Text" Width="250px" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
	</px:PXGrid>
</asp:Content>