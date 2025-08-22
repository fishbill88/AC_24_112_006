	<px:PXSmartPanel ID="pnlPublishToUI" runat="server" Caption="Publish to the UI" CaptionVisible="true" LoadOnDemand="true" 
		Key="PublishToUIDialog" AutoCallBack-Target="formPublishToUI" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="true" 
		AcceptButtonID="btnPublish" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" CommandSourceID="ds" AllowResize="false"
		DesignView="Content" CreateOnDemand="false" CloseButtonDialogResult="Abort" AutoCallBack-Behavior-RepaintControls="All" 
		AutoRepaint="true" AutoReload="true" ShowCloseButton="False">
		<px:PXFormView ID="formPublishToUI" runat="server" DataSourceID="ds" DataMember="PublishToUIDialog" AllowCollapse="False" 
			SkinID="Transparent" Style="z-index: 100" Width="100%">
			<Template>
				<px:PXTextEdit runat="server" DataField="Title" ID="edTitle" CommitChanges="true" IsClientControl="false"/>
				<px:PXSelector runat="server" DataField="Workspace" ID="PXWorkspace" CommitChanges="true"/>
				<px:PXSelector runat="server" DataField="Category" ID="PXCategory" CommitChanges="true"/>
				<px:PXMaskEdit runat="server" DataField="ScreenID" ID="edScreenID" CommitChanges="true" IsClientControl="false"/>
				<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Access Rights"/>
				<px:PXGroupBox ID="gbAccessRights" runat="server" CommitChanges="True" DataField="AccessRights"
					RenderSimple="True" RenderStyle="Simple">
					<Template>
						<px:PXRadioButton ID="rbAllGranted" runat="server" GroupName="gbAccessRights" Value="G"/>
						<px:PXRadioButton ID="rbAllRevoked" runat="server" GroupName="gbAccessRights" Value="R"/>
						<px:PXLayoutRule ID="PXLayoutRuleCopy" runat="server" Merge="true"/>
						<px:PXRadioButton ID="rbCopy" runat="server" GroupName="gbAccessRights" Value="C"/>
						<px:PXSelector runat="server" DataField="CopyFromScreenID" ID="PXCopyFromScreenID" SuppressLabel="true" DisplayMode="Text"
													 FilterByAllFields="true" CommitChanges="true" Width="150px" />
						<px:PXLayoutRule ID="PXLayoutRuleEnd" runat="server"/>
					</Template>
					<ContentLayout LabelsWidth="S" Layout="Stack"/>
				</px:PXGroupBox>
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="pnlPushToUIButton" runat="server" SkinID="Buttons">
			<px:PXButton ID="btnPublish" runat="server" DialogResult="OK" Text="Publish">
				<AutoCallBack Target="formPublishToUI" Command="Save"/>
			</px:PXButton>
			<px:PXButton ID="btnClosePanel" runat="server" DialogResult="Abort" Text="Cancel"/>
		</px:PXPanel>
	</px:PXSmartPanel>