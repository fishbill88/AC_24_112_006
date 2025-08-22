<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" 
	ValidateRequest="false" CodeFile="IN507000.aspx.cs" Inherits="Page_IN507000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.IN.Turnover.ManageTurnover" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" 
		Caption="Selection" DefaultControlID="edAction" MarkRequired="Dynamic" >
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
			<px:PXDropDown ID="edAction" runat="server" DataField="Action" CommitChanges="True" />
			<px:PXBranchSelector ID="edOrgBAccountID" runat="server" DataField="OrgBAccountID" CommitChanges="True" InitialExpandLevel="0" />
			<px:PXSelector ID="edFromPeriod" runat="server" DataField="FromPeriodID" AutoRefresh="true" CommitChanges="True" />
			<px:PXSelector ID="edToPeriod" runat="server" DataField="ToPeriodID" AutoRefresh="true" CommitChanges="True" />
			<px:PXNumberEdit ID="edNumberOfPeriods" runat="server" DataField="NumberOfPeriods" />
			<px:PXDropDown ID="edCalculateBy" runat="server" DataField="CalculateBy" CommitChanges="True" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" 
		Style="z-index: 100" Width="100%" ActionsPosition="top" 
		AllowPaging="true" AdjustPageSize="Auto" TabIndex="100" 
		SkinID="PrimaryInquire" Caption="Details" SyncPosition="True" NoteIndicator="False" FilesIndicator="False">
		<ActionBar>
			<Actions>
				<Refresh ToolBarVisible="False" MenuVisible="False"></Refresh>
			</Actions>
		</ActionBar>
	    <Levels>
			<px:PXGridLevel DataMember="TurnoverCalcs" >
				<Columns>
					<px:PXGridColumn DataField="Selected" AllowNull="False" AllowCheckAll="True" TextAlign="Center" Type="CheckBox" CommitChanges="True" />
					<px:PXGridColumn DataField="BranchID" AllowNull="False" />
					<px:PXGridColumn DataField="FromPeriodID" AllowNull="False" />
					<px:PXGridColumn DataField="ToPeriodID" AllowNull="False" />
					<px:PXGridColumn DataField="SiteID" />
					<px:PXGridColumn DataField="ItemClassID" />
					<px:PXGridColumn DataField="InventoryID" />
					<px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
