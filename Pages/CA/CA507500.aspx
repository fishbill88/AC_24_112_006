<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA507500.aspx.cs" Inherits="Page_CA507500" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" PrimaryView="BankFeeds" TypeName="PX.Objects.CA.CABankFeedImport" SuspendUnloading="False">
	   <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="100%" SkinID="PrimaryInquire"  TabIndex="100" SyncPosition="true" AdjustPageSize="Auto"> 
		<Levels>
			<px:PXGridLevel DataMember="BankFeeds">
			     <Columns>
                   <px:PXGridColumn DataField="Selected" Label="Selected" TextAlign="Center" Type="CheckBox" AutoCallBack="true" AllowCheckAll="true" />
                    <px:PXGridColumn DataField="BankFeedID" Width="100px" LinkCommand="ViewBankFeed">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Type" Width="70px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Descr" Width="300px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Institution">
					</px:PXGridColumn>
                    <px:PXGridColumn DataField="Status">
                    </px:PXGridColumn>
					<px:PXGridColumn DataField="RetrievalStatus" Width="120px">
                    </px:PXGridColumn>
					<px:PXGridColumn DataField="RetrievalDate" Width="120px">
                    </px:PXGridColumn>
					<px:PXGridColumn DataField="ErrorMessage">
                    </px:PXGridColumn>
					<px:PXGridColumn DataField="AccountQty">
                    </px:PXGridColumn>
					<px:PXGridColumn DataField="UnmatchedAccountQty" Width="90px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" />
	</px:PXGrid>
</asp:Content>
