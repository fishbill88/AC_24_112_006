<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM303030.aspx.cs" Inherits="Pages_SM303030" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Owin.IdentityServerIntegration.DAC.OAuthServerKeyMaint" PrimaryView="AllKeys">
        <ClientEvents CommandPerformed="commandResult" />
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phL" runat="Server">
    <fieldset style="width: 40%">
        <legend>Keys</legend>
    </fieldset>
    <px:PXGrid ID="gridKeys" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="80%" Heigth="100%" ActionsPosition="Top" SkinID="Attributes">
        <Levels>
            <px:PXGridLevel DataMember="AllKeys">
                <Columns>
                    <px:PXGridColumn DataField="SigningKey" Width="160px" TextAlign="Center" Type="CheckBox" />
                    <px:PXGridColumn DataField="KeyID" Width="200px" />
                    <px:PXGridColumn DataField="CreationDateUtc" Width="200px" />
                    <px:PXGridColumn DataField="ExpirationDateUtc" Width="200px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Enabled="True" MinHeight="250" MinWidth="300" />
    </px:PXGrid>
    <px:PXSmartPanel ID="pnlGenerateKey" runat="server" Caption="Generate New Key"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="GenerateOAuthServerKeyView" CreateOnDemand="false"
        AutoCallBack-Enabled="true" AutoCallBack-Target="formGenerateKey" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        ShowAfterLoad="True"
        AcceptButtonID="btnActionOK"
        CancelButtonID="btnActionCancel"
        Width="30em">
        <px:PXFormView ID="formGenerateKey" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False" DataMember="GenerateOAuthServerKeyView">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XL" />
                <px:PXCheckBox runat="server" AlignLeft="true" ID="edDeactivateImmediatly" DataField="DeactivateImmediatly" CommitChanges="true" />
                <px:PXNumberEdit ID="edExpirationPeriod" runat="server" AllowNull="False" DataField="ExpirationPeriod" ValueType="Int32"
                    MinValue="0" MaxValue="365"
                    Hidden="False" LabelWidth="100%">
                </px:PXNumberEdit>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnActionOK" runat="server" DialogResult="OK" Text="OK" />
            <px:PXButton ID="btnActionCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
