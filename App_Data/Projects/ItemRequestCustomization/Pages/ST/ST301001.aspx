<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="ST301001.aspx.cs" Inherits="Page_ST301001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="PXDataSource" runat="server" Visible="True" Width="100%" PrimaryView="Document" TypeName="Find.Graphs.ATAIEnrollPDCEntry">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">

    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Document" Width="100%" Height="100%" TabIndex="1" 
    DefaultControlID="edRefNbr" AllowAutoHide="False" AllowCollapse="False">
		<AutoSize Enabled="True" MinHeight="200" Container="Window" />
        <Template>
            <px:PXTextEdit ID="edRefNbr" runat="server" DataField="RefNbr" Width="200px" />
            <px:PXDateTimeEdit ID="edDateSubmitted" runat="server" DataField="DateSubmitted" Width="200px" />
            <px:PXTextEdit ID="edRequestorName" runat="server" DataField="RequestorName" Width="200px" />
            <px:PXDropDown ID="edStatus" runat="server" DataField="Status" Width="200px" />
            <px:PXTextEdit ID="edPartNumber" runat="server" DataField="PartNumber" Width="200px" />
            <px:PXTextEdit ID="edItemDescription" runat="server" DataField="ItemDescription" Width="300px" />
            <px:PXTextEdit ID="edModelNumber" runat="server" DataField="ModelNumber" Width="200px" />
            <px:PXDropDown ID="edSerialized" runat="server" DataField="Serialized" Width="200px" />
            <px:PXTextEdit ID="edProductBrand" runat="server" DataField="ProductBrand" Width="200px" />
            <px:PXDropDown ID="edItemClassID" runat="server" DataField="ItemClassID" Width="200px" />
            <px:PXTextEdit ID="edStdUnitOfMeasure" runat="server" DataField="StdUnitOfMeasure" Width="200px" />
            <px:PXDropDown ID="edDefaultWarehouse" runat="server" DataField="DefaultWarehouse" Width="200px" />
            <px:PXNumberEdit ID="edMinOrderQty" runat="server" DataField="MinOrderQty" Width="200px" />
            <px:PXNumberEdit ID="edListPrice" runat="server" DataField="ListPrice" Width="200px" />
            <px:PXNumberEdit ID="edOurCost" runat="server" DataField="OurCost" Width="200px" />
            <px:PXTextEdit ID="edNotesCatalogLink" runat="server" DataField="NotesCatalogLink" Width="300px" />
            <px:PXTextEdit ID="edCountryOfOrigin" runat="server" DataField="CountryOfOrigin" Width="200px" />
            <px:PXTextEdit ID="edHTSCode" runat="server" DataField="HTSCode" Width="200px" />
            <px:PXNumberEdit ID="edWeightLbs" runat="server" DataField="WeightLbs" Width="200px" />
            <px:PXDropDown ID="edCustomerID" runat="server" DataField="CustomerID" Width="200px" />
            <px:PXTextEdit ID="edCustomerAlternateID" runat="server" DataField="CustomerAlternateID" Width="200px" />
            <px:PXTextEdit ID="edPlanningMethod" runat="server" DataField="PlanningMethod" Width="200px" />
            <px:PXTextEdit ID="edReplenishmentClass1" runat="server" DataField="ReplenishmentClass1" Width="200px" />
            <px:PXTextEdit ID="edSeasonality1" runat="server" DataField="Seasonality1" Width="200px" />
            <px:PXTextEdit ID="edSource1" runat="server" DataField="Source1" Width="200px" />
            <px:PXTextEdit ID="edMethod1" runat="server" DataField="Method1" Width="200px" />
            <px:PXTextEdit ID="edReplenishmentClass2" runat="server" DataField="ReplenishmentClass2" Width="200px" />
            <px:PXTextEdit ID="edSeasonality2" runat="server" DataField="Seasonality2" Width="200px" />
            <px:PXTextEdit ID="edSource2" runat="server" DataField="Source2" Width="200px" />
            <px:PXTextEdit ID="edMethod2" runat="server" DataField="Method2" Width="200px" />
        </Template>
    </px:PXFormView>
</asp:Content>