<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="IN503000.aspx.cs" Inherits="Page_IN503000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.IN.MassConvertStockNonStock" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" 
		Style="z-index: 100" Width="100%" DataMember="Filter" Caption="Selection" 
		DefaultControlID="edSiteID" TabIndex="100">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXDropDown ID="edFilterAction" runat="server" DataField="Action" CommitChanges="True" />
			<px:PXSegmentMask ID="edFilterItemClassID" runat="server" DataField="ItemClassID" CommitChanges="True" AutoRefresh="True" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="S" ControlSize="M" />
			<px:PXSegmentMask ID="edFilterInventoryID" runat="server" DataField="InventoryID" CommitChanges="True" AutoRefresh="True" />
			<px:PXButton runat="server" ID="btnItemList" Text="List" CommandName="SelectItems" CommandSourceID="ds"/>
			<px:PXLayoutRule runat="server" EndGroup="True" />
			<px:PXLayoutRule runat="server" StartColumn="True" />
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Non-Stock Item Settings" LabelsWidth="S" ControlSize="M" />
			<px:PXSegmentMask ID="edNonStockItemClassID" runat="server" DataField="NonStockItemClassID" CommitChanges="True" />
			<px:PXSelector ID="edNonStockPostClassID" runat="server" DataField="NonStockPostClassID" CommitChanges="True" />
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Stock Item Settings" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="SM" />
			<px:PXSegmentMask ID="edStockItemClassID" runat="server" DataField="StockItemClassID" CommitChanges="True" />
			<px:PXDropDown ID="edStockValMethod" runat="server" DataField="StockValMethod" CommitChanges="True" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="SM" />
			<px:PXSelector ID="edStockPostClassID" runat="server" DataField="StockPostClassID" CommitChanges="True" />
			<px:PXSelector ID="edStockLotSerClassID" runat="server" DataField="StockLotSerClassID" CommitChanges="True" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" 
		Style="z-index: 100" Width="100%" ActionsPosition="top" BatchUpdate="True"
		SkinID="PrimaryInquire" Caption="Details" AdjustPageSize="Auto" AllowPaging="True" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="ItemList">
				<RowTemplate>
					<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True" />
					<px:PXSegmentMask ID="edInventoryCD" runat="server" DataField="InventoryCD" Enabled="False" HintField="Descr" AllowEdit="True" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="Selected" AllowCheckAll="True" DataType="Boolean" TextAlign="Center" Type="CheckBox" />
					<px:PXGridColumn AllowUpdate="False" DataField="InventoryCD" />
					<px:PXGridColumn AllowUpdate="False" DataField="Descr" />
					<px:PXGridColumn AllowUpdate="False" DataField="ItemType" />
					<px:PXGridColumn AllowUpdate="False" DataField="ValMethod" />
					<px:PXGridColumn AllowUpdate="False" DataField="ItemClassID" />
					<px:PXGridColumn AllowUpdate="False" DataField="PostClassID" />
					<px:PXGridColumn AllowUpdate="False" DataField="LotSerClassID" />
					<px:PXGridColumn AllowUpdate="False" DataField="BaseUnit" />
					<px:PXGridColumn AllowUpdate="False" DataField="TaxCategoryID" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
	<!--#include file="~\Pages\IN\Includes\InventoryLinkFilter.inc"-->
</asp:Content>
