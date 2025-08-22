<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="EP503020.aspx.cs"
	Inherits="Page_EP503020" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.EP.ReassignDelegatedActivitiesProcess" PrimaryView="Records">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont4" ContentPlaceHolderID="phL" runat="server">
	<px:PXGrid AllowSearch="true" AllowFilter="true" ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" ActionsPosition="Top"
		SkinID="PrimaryInquire" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="Records">
				<RowTemplate>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn AllowCheckAll="True" AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" />
					<px:PXGridColumn DataField="DelegationOf" />
					<px:PXGridColumn DataField="OrigOwnerID" />
					<px:PXGridColumn DataField="OwnerID" DisplayMode="Text" />
					<px:PXGridColumn DataField="DelegatedToContactID" DisplayMode="Text" />
					<px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" />
					<px:PXGridColumn DataField="StartsOn" />
					<px:PXGridColumn DataField="ExpiresOn" />
					<px:PXGridColumn DataField="DocType" Width = "100px" />
					<px:PXGridColumn DataField="RefNoteID" Width = "100px" />
					<px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" />
					<px:PXGridColumn DataField="Descr" />
				</Columns>
				<RowTemplate>
					<px:PXSelector ID="edRefNoteID" runat="server" DataField="RefNoteID" FilterByAllFields="True" AutoRefresh="True" />
				</RowTemplate>
				<Layout FormViewHeight=""></Layout>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<Mode AllowUpdate="False" />
	</px:PXGrid>
</asp:Content>
