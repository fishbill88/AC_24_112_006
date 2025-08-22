<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="ST301001.aspx.cs" Inherits="Page_ST301001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Document" TypeName="ItemRequestCustomization.ItemRequestEntry">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" TabIndex="18864">
		<AutoSize Enabled="True" MinHeight="200" Container="Window" />
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True" LabelsWidth="SM"/>
		    <%--<px:PXSelector ID="edRefNbr" runat="server" CommitChanges="True" DataField="RefNbr">
            </px:PXSelector>--%>
            <px:PXTextEdit ID="edInventoryCD" runat="server" DataField="InventoryCD" />
            <%--<px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="true">
            </px:PXSegmentMask>--%>
            <px:PXDateTimeEdit ID="edDateSubmitted" runat="server" DataField="DateSubmitted" />
           <%-- <px:PXDropDown ID="edStatus" runat="server" DataField="Status" CommitChanges="True" IsClientControl="True">
            </px:PXDropDown>--%>
            <px:PXTextEdit ID="edItemDescription" runat="server" DataField="ItemDescription" Height="100px" TextMode="MultiLine"/>
            <px:PXTextEdit ID="edItemSpecs" runat="server" DataField="ItemSpecs" Height="80px" TextMode="MultiLine"/>
            <px:PXSegmentMask CommitChanges="True" ID="edItemClassID" runat="server" DataField="ItemClassID" AllowEdit="True" />
            <px:PXSelector CommitChanges="True" ID="edPostingClassID" runat="server" DataField="PostClassID" AllowEdit="True" />
            <px:PXSelector CommitChanges="True" ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AllowEdit="True" />
		    
            <px:PXSegmentMask CommitChanges="True" ID="edDefaultWarehouse" runat="server" DataField="DefaultWarehouse" AllowEdit="True" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM">
            </px:PXLayoutRule>
            <px:PXTextEdit ID="edRequestorName" runat="server" DataField="RequestorName" />
            <px:PXSegmentMask ID="edCustomerID" runat="server" DataField="CustomerID" CommitChanges="True">
            </px:PXSegmentMask>
            <px:PXTextEdit ID="edCustomerAlternateID" runat="server" DataField="CustomerAlternateID"/>
            <px:PXDropDown ID="edPlanningMethod" runat="server" DataField="PlanningMethod" CommitChanges="True" IsClientControl="True">
            </px:PXDropDown>
            
            <px:PXLayoutRule runat="server" StartGroup="true" LabelsWidth="SM" GroupCaption="Inventory Planning (1)">
            </px:PXLayoutRule>
		    <px:PXSelector ID="edReplenishmentClass1" runat="server" CommitChanges="True" DataField="ReplenishmentClass1">
            </px:PXSelector>
		    <px:PXSelector ID="edSeasonality1" runat="server" CommitChanges="True" DataField="Seasonality1">
            </px:PXSelector>
            <px:PXDropDown ID="edSource1" runat="server" DataField="Source1" CommitChanges="True" IsClientControl="True">
            </px:PXDropDown>
            <px:PXDropDown ID="edMethod1" runat="server" DataField="Method1" CommitChanges="True" IsClientControl="True">
            </px:PXDropDown>

            
            <px:PXLayoutRule runat="server" StartGroup="true" LabelsWidth="SM" GroupCaption="Vendors">
            </px:PXLayoutRule>
            
            <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID"/>
            <px:PXSegmentMask CommitChanges="True" ID="edVendorLocationID" runat="server" DataField="VendorLocationID"/>
            <px:PXSelector CommitChanges="True" ID="edVendorSiteID" runat="server" DataField="VendorSiteID"  />
            <px:PXTextEdit ID="edVendorInventoryID" runat="server" DataField="VendorInventoryID"/>
            
            <px:PXLayoutRule runat="server" StartGroup="true" LabelsWidth="SM" GroupCaption="Inventory Planning (2)">
            </px:PXLayoutRule>
		    <px:PXSelector ID="edReplenishmentClass2" runat="server" CommitChanges="True" DataField="ReplenishmentClass2">
            </px:PXSelector>
		    <px:PXSelector ID="edSeasonality2" runat="server" CommitChanges="True" DataField="Seasonality2">
            </px:PXSelector>
            <px:PXDropDown ID="edSource2" runat="server" DataField="Source2" CommitChanges="True" IsClientControl="True">
            </px:PXDropDown>
            <px:PXDropDown ID="edMethod2" runat="server" DataField="Method2" CommitChanges="True" IsClientControl="True">
            </px:PXDropDown>

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M">
            </px:PXLayoutRule>
            
            <px:PXCheckBox ID="edIsAKit" runat="server" DataField="IsAKit" AlignLeft="true"/>
            <px:PXTextEdit ID="edPartNumber" runat="server" DataField="PartNumber"/>
             <%--<px:PXTextEdit ID="edModelNumber" runat="server" DataField="ModelNumber" />--%>
             <%--<px:PXDropDown ID="edSerialized" runat="server" DataField="Serialized" />--%>
		    <px:PXSelector ID="edSerialClassID" runat="server" CommitChanges="True" DataField="SerialClassID">
            </px:PXSelector>
             <px:PXTextEdit ID="edProductBrand" runat="server" DataField="ProductBrand" />
            <px:PXSelector ID="edStdUnitOfMeasure" runat="server" AllowEdit="True" AutoRefresh="True" CommitChanges="True" DataField="StdUnitOfMeasure"/>

            <px:PXNumberEdit ID="edMinOrderQty" runat="server" DataField="MinOrderQty" />
            <px:PXNumberEdit ID="edListPrice" runat="server" DataField="ListPrice" />
            <px:PXNumberEdit ID="edOurCost" runat="server" DataField="OurCost" />
            <px:PXTextEdit ID="edNotesCatalogLink" runat="server" DataField="NotesCatalogLink" Width="300px" />
            <px:PXSelector runat="server" ID="edCountryOfOrigin" DataField="CountryOfOrigin" />
            <px:PXTextEdit ID="edHTSCode" runat="server" DataField="HTSCode" />
            <px:PXNumberEdit ID="edWeightLbs" runat="server" DataField="WeightLbs" />

		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	
</asp:Content>
