<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" 
CodeFile="SM508000.aspx.cs" Inherits="Pages_SM_SM508000" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Width="100%" TypeName="PX.AutocompleteGenerator.UI.AutocompleteSuggesterTrainingProcess"
                     PageLoadBehavior="PopulateSavedValues" Visible="True" PrimaryView="Users">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Process" StartNewGroup="True" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="144px" Style="z-index: 100; left: 0px;
                                                              top: 0px;" Width="100%" AdjustPageSize="Auto" AllowPaging="True" AllowSearch="True"
               BatchUpdate="True" Caption="Users" DataSourceID="ds" SkinID="Inquire">
        <Levels>
            <px:PXGridLevel DataMember="Users">
                <Columns>
                    <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" />
                    <px:PXGridColumn DataField="Username"/>
                    <px:PXGridColumn DataField="CreatedDateTime" Width="132px"/>
                    <px:PXGridColumn DataField="SuccessString" Width="132px"/>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
    </px:PXGrid>
</asp:Content>
