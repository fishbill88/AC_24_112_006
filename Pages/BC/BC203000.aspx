<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="BC203000.aspx.cs" Inherits="Page_BC203000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PX.Commerce.Objects.BCMatrixOptionsMappingMaint"
        PrimaryView="MasterView">	
	</px:PXDataSource>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule LabelsWidth="M" ControlSize="M" StartColumn="True" ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXSelector AutoRefresh="True" NullText="&lt;SELECT>" CommitChanges="True"  AllowEdit="True" runat="server" ID="itemClassSelectorId" DataField="ItemClassID"></px:PXSelector>						
		</Template>
		<AutoSize Container="Window" Enabled="True"></AutoSize>
	</px:PXFormView>
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="phG" Runat="Server">
  <px:PXGrid runat="server" AllowPaging="True" SyncPosition="True" id="gridOptionMapping" MatrixMode="True" AdjustPageSize="Auto"  AllowSearch="true" DataSourceID="ds" Width="100%" Height="150px" AllowUpload="false"
  		SkinID="DetailsWithFilter" AllowAutoHide="false">	
    <Levels>
      <px:PXGridLevel DataMember="OptionMappings">		
          <Columns>			
            <px:PXGridColumn DataField="BCSyncStatus__ExternDescription" TextAlign="Left" Width="200px" AllowDragDrop="true" ></px:PXGridColumn>
			<px:PXGridColumn CommitChanges="True" DataField="ExternID" Width="200px" AllowDragDrop="true"></px:PXGridColumn>
			<px:PXGridColumn CommitChanges="True" DataField="ExternalOptionName" Width="200px"></px:PXGridColumn>
			<px:PXGridColumn CommitChanges="True" DataField="ExternalOptionValue" Width="200px"></px:PXGridColumn>
			<px:PXGridColumn CommitChanges="True" DataField="ItemClassID" Width="200px"></px:PXGridColumn>
			<px:PXGridColumn CommitChanges="True" DataField="MappedAttributeID" DisplayMode="Text" Width="200px"></px:PXGridColumn>			  
			<px:PXGridColumn CommitChanges="True" DataField="MappedValue" DisplayMode="Text" Width="200px"></px:PXGridColumn>
		</Columns>
      </px:PXGridLevel>				
    </Levels>   
	<AutoSize Container="Window" Enabled="True" MinHeight="150"></AutoSize>
	<ActionBar>
		<Actions>
			<Upload Enabled="false"/>
			<AddNew Enabled="false"/>
			<Delete Enabled="false"/>
		</Actions>
	</ActionBar>
	<Mode InitNewRow="True"></Mode>
</px:PXGrid>
</asp:Content>