<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" 
	ValidateRequest="false" CodeFile="SO501010.aspx.cs" Inherits="Page_SO501010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.SO.ManageSalesAllocations" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" 
		Caption="Selection" DefaultControlID="edAction" MarkRequired="Dynamic" >
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
				<px:PXDropDown ID="edAction" runat="server" DataField="Action" CommitChanges="True" />
				<px:PXSelector ID="edSiteID" runat="server" DataField="SiteID" CommitChanges="True" AutoRefresh="true" />
				<px:PXDropDown ID="edSelectBy" runat="server" DataField="SelectBy" CommitChanges="True" />
				<px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" CommitChanges="True" />
				<px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" CommitChanges="True" />

			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />	
				<px:PXDropDown ID="edOrderType" runat="server" DataField="OrderType" AllowMultiSelect="true" CommitChanges="True" />
				<px:PXDropDown ID="edOrderStatus" runat="server" DataField="OrderStatus" AllowMultiSelect="true" CommitChanges="True" />
				<px:PXNumberEdit ID="edPriority" runat="server" DataField="Priority" CommitChanges="True" Size="M" AllowNull="true" TextAlign="Left" />
				<px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" CommitChanges="True" AutoRefresh="true" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="M" />
                    <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" CommitChanges="True" AutoRefresh="true" />
                    <px:PXButton runat="server" ID="btnItemList" Text="List" CommandName="SelectItems" CommandSourceID="ds"/>
                <px:PXLayoutRule runat="server" EndGroup="True" />

			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector ID="edCustomerClassID" runat="server" DataField="CustomerClassID" CommitChanges="true" />
				<px:PXSegmentMask ID="edCustomerID" runat="server" DataField="CustomerID" CommitChanges="True" AutoRefresh="True" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" 
		Style="z-index: 100" Width="100%" ActionsPosition="top" 
		AllowPaging="true" AdjustPageSize="Auto" TabIndex="100" 
		SkinID="PrimaryInquire" Caption="Details" SyncPosition="True" NoteIndicator="True" FilesIndicator="True">
	    <Levels>
			<px:PXGridLevel DataMember="Allocations" >
				<RowTemplate>
					<px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" AllowEdit="true"/>
					<px:PXSegmentMask ID="edLineInventoryID" runat="server" DataField="InventoryID" AllowEdit="true" />
					<px:PXNumberEdit ID="edQtyToAllocate" runat="server" DataField="QtyToAllocate" MinValue="0" />
					<px:PXNumberEdit ID="edQtyToDeallocate" runat="server" DataField="QtyToDeallocate" MinValue="0" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="Selected" AllowNull="False" AllowCheckAll="True" TextAlign="Center" Type="CheckBox" CommitChanges="True" AllowSort="False" />
					<px:PXGridColumn DataField="OrderPriority" TextAlign="Left" />
					<px:PXGridColumn DataField="OrderType" DisplayFormat="&gt;LL" />
					<px:PXGridColumn DataField="OrderNbr" />
					<px:PXGridColumn DataField="OrderStatus" Type="DropDownList" />
					<px:PXGridColumn DataField="OrderDesc" />
					<px:PXGridColumn DataField="CustomerID" />
					<px:PXGridColumn DataField="CustomerName" />
					<px:PXGridColumn DataField="CustomerClassID" />
					<px:PXGridColumn DataField="SalesPersonID" />
					<px:PXGridColumn DataField="CuryID" />
					<px:PXGridColumn DataField="CuryOrderTotal" />

					<px:PXGridColumn DataField="LineNbr" />
					<px:PXGridColumn DataField="LineSiteID" />
					<px:PXGridColumn DataField="InventoryID" />
					<px:PXGridColumn DataField="BaseUOM" />
					<px:PXGridColumn DataField="TranDesc" />
					<px:PXGridColumn DataField="BaseLineQty" />
					<px:PXGridColumn DataField="UOM" />
					<px:PXGridColumn DataField="LineQty" />
					<px:PXGridColumn DataField="CuryLineAmt" />
					
					<px:PXGridColumn DataField="QtyHardAvail" AllowShowHide="Server" />

					<px:PXGridColumn DataField="SplitSiteID" />
					<px:PXGridColumn DataField="QtyAllocated" />
					<px:PXGridColumn DataField="QtyUnallocated" />

					<px:PXGridColumn DataField="QtyToAllocate" AllowSort="False" AllowFilter="False" AllowShowHide="Server" CommitChanges="True" />
					<px:PXGridColumn DataField="QtyToDeallocate" AllowSort="False" AllowFilter="False" AllowShowHide="Server" CommitChanges="True" />

					<px:PXGridColumn DataField="ShipComplete" Type="DropDownList" />
					<px:PXGridColumn DataField="RequestDate" TextAlign="Right" />
					<px:PXGridColumn DataField="ShipDate" TextAlign="Right" />
					<px:PXGridColumn DataField="OrderDate" TextAlign="Right" />
					<px:PXGridColumn DataField="CancelDate" TextAlign="Right" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
	<!--#include file="~\Pages\IN\Includes\InventoryLinkFilter.inc"-->
</asp:Content>
