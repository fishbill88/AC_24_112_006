<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PO507000.aspx.cs" Inherits="Page_PO507000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="Filter" TypeName="PX.Objects.PO.ValidatePODocumentAddressProcess" >
		<CallbackCommands/>
	</px:PXDataSource>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Filter"> 
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="SM"/>
			<px:PXSelector ID="edCountry" runat="server" DataField="Country" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
			<px:PXDropDown ID="edDocumentType" runat="server" DataField="DocumentType" AllowNull="True" NullText="All" CommitChanges="True" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="SM" LabelsWidth="XS"/>
			<px:PXCheckBox ID="edIsOverride" runat="server" DataField="IsOverride" />
		</Template>	
	</px:PXFormView>
	<px:PXGrid ID="grid1" runat="server" Height="400px" Width="100%" Style="z-index: 100" AllowPaging="true" AdjustPageSize="Auto" AllowSearch="true" DataSourceID="ds"
        BatchUpdate="True" SkinID="PrimaryInquire" Caption="Documents" FastFilterFields="DocumentNbr" SyncPosition="true">
        <Levels>
            <px:PXGridLevel DataMember="DocumentAddresses">
                <Columns>
                    <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" />
					<px:PXGridColumn DataField="DocumentNbr" LinkCommand="ViewDocument" />
					<px:PXGridColumn DataField="DocumentType" />
					<px:PXGridColumn DataField="Status" />
					<px:PXGridColumn DataField="AddressLine1" />
					<px:PXGridColumn DataField="AddressLine2" />
					<px:PXGridColumn DataField="City" />
					<px:PXGridColumn DataField="State" />
					<px:PXGridColumn DataField="PostalCode" />
                    <px:PXGridColumn DataField="CountryID" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
</asp:Content>