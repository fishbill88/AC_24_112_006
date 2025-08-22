<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TX206000.aspx.cs" Inherits="Page_TX206000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource EnableAttributes="true" ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="TxZone" TypeName="PX.Objects.TX.TaxZoneMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
		</CallbackCommands>
	</px:PXDataSource>
    <script type="text/javascript">
   window.addEventListener('load', function ()
   {
    if (px_callback)
    {
     px_callback.addHandler(function (context)
     {
         if (context.info.name == "RepaintTab" && context.controlID == tab1ID)
      {
             var tab = document.querySelector('#' + tab1ID);
          var style = tab.parentNode.style;
       if (tab.object.getVisible())
       {
        style.backgroundColor = style.borderLeft = style.borderRight = '';
       } else
       {
        style.backgroundColor = '#eeeeee';
        style.borderLeft = style.borderRight = ' 1px solid #bdbdbd';
       }
      }
     });
    }
   });
 </script>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%"  Caption="Tax Zone" DataMember="TxZone" NoteIndicator="True" 
        FilesIndicator="True" ActivityIndicator="true" ActivityField="NoteActivity" MarkRequired="Dynamic">
		
        <Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
			<px:PXSelector ID="edTaxZoneID" runat="server" DataField="TaxZoneID" />
			<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" CommitChanges="True"/>
			<px:PXSelector ID="edDfltTaxCategoryID" runat="server" DataField="DfltTaxCategoryID" AllowEdit="True" />
		    <px:PXCheckBox CommitChanges="True" ID="chkIsExternal" runat="server" DataField="IsExternal" />
			<px:PXSelector CommitChanges="True" ID="edTaxPluginID" runat="server" DataField="TaxPluginID" AllowEdit="True" />
            <px:PXSelector CommitChanges="True" ID="edTaxVendorID" runat="server" DataField="TaxVendorID" />
			<px:PXDropDown CommitChanges="True" ID="edAPTaxType" runat="server" DataField="ExternalAPTaxType" />
            <px:PXCheckBox CommitChanges="True" ID="chkIsManualVATZone" runat="server" DataField="IsManualVATZone" />
            <px:PXSelector ID="edTaxID" runat="server" DataField="TaxID" />
            <px:PXCheckBox SuppressLabel="True" ID="chkShowTaxTabExpr" runat="server" DataField="ShowTaxTabExpr" />
            <px:PXCheckBox SuppressLabel="True" ID="chkShowZipTabExpr" runat="server" DataField="ShowZipTabExpr" />
        </Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="PXTab1" runat="server" Height="210px" Width="100%">
		<Items>
			<px:PXTabItem Text="Applicable Taxes" BindingContext="form" VisibleExp="DataControls[&quot;chkShowTaxTabExpr&quot;].Value=1">
				<Template>
					<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="top" AllowSearch="true" SkinID="DetailsInTab" >
						<Levels>
							<px:PXGridLevel DataMember="Details">
								<Columns>
									<px:PXGridColumn DataField="TaxID" />
									<px:PXGridColumn DataField="Tax__Descr" />
									<px:PXGridColumn DataField="Tax__TaxType" Type ="DropDownList" />
									<px:PXGridColumn DataField="Tax__TaxCalcRule" Type ="DropDownList" />
									<px:PXGridColumn DataField="Tax__TaxApplyTermsDisc" Type ="DropDownList"/>
									<px:PXGridColumn DataField="Tax__DirectTax" Type ="CheckBox"/>
								</Columns>
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
									<px:PXDropDown ID="edTax__TaxType" runat="server" DataField="Tax__TaxType" />
									<px:PXSelector ID="edTaxID" runat="server" DataField="TaxID" AllowEdit="True" />
									<px:PXTextEdit ID="edTax__Descr" runat="server" DataField="Tax__Descr" Enabled="False" />
									<px:PXDropDown ID="edTax__TaxCalcRule" runat="server" DataField="Tax__TaxCalcRule" />
									<px:PXDropDown ID="edTax__TaxApplyTermsDisc" runat="server" DataField="Tax__TaxApplyTermsDisc" Enabled="False" />
									<px:PXCheckBox ID="edTax__DirectTax" runat="server" DataField="Tax__DirectTax" Enabled="False" />
								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Ship-To Addresses" BindingContext="form"  VisibleExp="DataControls[&quot;chkShowZipTabExpr&quot;].Value=1">
				<Template>
					<px:PXFormView ID="AddressMappingParameters" runat="server" DataSourceID="ds" DataMember="TxZoneCurrent" RenderStyle="Simple" SkinID="Transparent">
                        <Template>
							<px:PXLayoutRule runat="server" StartRow="true" LabelsWidth="SM" ControlSize="XM" />					
							<px:PXDropDown  ID="edAddresMapingType" runat="server" DataField="MappingType" AutoRefresh ="True" CommitChanges="true"/>
							<px:PXSelector  ID="edAddressMappingCountryID" runat="server" DataField="CountryID"   CommitChanges="true"/>			
						</Template>
					</px:PXFormView>
					<px:PXGrid ID="gridAddressMapping" runat="server" DataSourceID="ds" Height="150px" Width="100%" MarkRequired="Dynamic" ActionsPosition="top" SyncPosition="True" AllowSearch="true"  SkinID="DetailsInTab" AllowPaging="true"  AutoRefresh ="True" >
						<Levels>
							<px:PXGridLevel DataMember="TaxZoneAddressMappings">
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
									<px:PXSelector ID="edCountryId" runat="server" DataField="CountryID" />
									<px:PXSelector ID="edStateId" runat="server" DataField="StateID" AutoRefresh="true" MarkRequired="Dynamic" />
									<px:PXTextEdit ID="edFromPostalCode" runat="server" DataField="FromPostalCode" MarkRequired="Dynamic"/>
									<px:PXTextEdit ID="edToPostalCode" runat="server" DataField="ToPostalCode" MarkRequired="Dynamic"/>
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="CountryID" CommitChanges="true" />
									<px:PXGridColumn DataField="StateID" CommitChanges="true" />
									<px:PXGridColumn DataField="FromPostalCode" TextAlign="Right"  CommitChanges="true" />
									<px:PXGridColumn DataField="ToPostalCode" TextAlign="Right" CommitChanges="true" />
									<px:PXGridColumn DataField="Description" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
					    <Mode AllowUpload="True" />
						<AutoSize Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="210" />
	</px:PXTab>
</asp:Content>
