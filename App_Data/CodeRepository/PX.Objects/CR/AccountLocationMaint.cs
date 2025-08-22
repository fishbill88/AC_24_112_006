/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR.Extensions.Relational;

namespace PX.Objects.CR
{
	public class AccountLocationMaint : LocationMaint
	{
		#region ctor

		public AccountLocationMaint()
		{
			MenuAction.AddMenuAction(ViewCustomerLocation);
			MenuAction.AddMenuAction(ViewVendorLocation);
			Actions.Move(nameof(Delete), nameof(CopyPaste), true);
		}

		#endregion


		#region Actions

		public PXCopyPasteAction<Location> CopyPaste;
		public PXMenuAction<Location> MenuAction;

		public PXAction<BAccount> ViewCustomerLocation;
		[PXUIField(DisplayName = "View Customer Location", Visible = false)]
		[PXButton]
		protected virtual void viewCustomerLocation()
		{
			if (Location.Current is Location location
				&& location.LocType.IsIn(LocTypeList.CustomerLoc, LocTypeList.CombinedLoc))
			{
				var graph = PXGraph.CreateInstance<CustomerLocationMaint>();
				graph.Location.Current = location;
				PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.Same);
			}
		}

		public PXAction<BAccount> ViewVendorLocation;
		[PXUIField(DisplayName = "View Vendor Location", Visible = false)]
		[PXButton]
		protected virtual void viewVendorLocation()
		{
			if (Location.Current is Location location
				&& location.LocType.IsIn(LocTypeList.VendorLoc, LocTypeList.CombinedLoc))
			{
				var graph = PXGraph.CreateInstance<VendorLocationMaint>();
				graph.Location.Current = location;
				PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.Same);
			}
		}

		#endregion


		#region Events

		[PXUIField(DisplayName = "Default Branch")]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void _(Events.CacheAttached<Location.cBranchID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.Visible), true)]
		protected virtual void _(Events.CacheAttached<Address.latitude> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.Visible), true)]
		protected virtual void _(Events.CacheAttached<Address.longitude> e) { }

		protected virtual void _(Events.RowSelected<Location> e)
		{
			if (e.Row is Location loc)
			{
				ViewCustomerLocation.SetEnabled(loc.LocType.IsIn(LocTypeList.CustomerLoc, LocTypeList.CombinedLoc));
				ViewVendorLocation.SetEnabled(loc.LocType.IsIn(LocTypeList.VendorLoc, LocTypeList.CombinedLoc));
			}
		}
		#endregion

		#region Extensions

		/// <exclude/>
		// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
		public class LocationBAccountSharedContactOverrideGraphExt : SharedChildOverrideGraphExt<AccountLocationMaint, LocationBAccountSharedContactOverrideGraphExt>
		{
			#region Initialization 

			protected override DocumentMapping GetDocumentMapping()
			{
				return new DocumentMapping(typeof(Location))
				{
					RelatedID = typeof(Location.bAccountID),
					ChildID = typeof(Location.defContactID),
					IsOverrideRelated = typeof(Location.overrideContact)
				};
			}

			protected override RelatedMapping GetRelatedMapping()
			{
				return new RelatedMapping(typeof(BAccount))
				{
					RelatedID = typeof(BAccount.bAccountID),
					ChildID = typeof(BAccount.defContactID)
				};
			}

			protected override ChildMapping GetChildMapping()
			{
				return new ChildMapping(typeof(Contact))
				{
					ChildID = typeof(Contact.contactID),
					RelatedID = typeof(Contact.bAccountID),
				};
			}

			#endregion
		}

		/// <exclude/>
		// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
		public class LocationBAccountSharedAddressOverrideGraphExt : SharedChildOverrideGraphExt<AccountLocationMaint, LocationBAccountSharedAddressOverrideGraphExt>
		{
			#region Initialization 

			protected override DocumentMapping GetDocumentMapping()
			{
				return new DocumentMapping(typeof(Location))
				{
					RelatedID = typeof(Location.bAccountID),
					ChildID = typeof(Location.defAddressID),
					IsOverrideRelated = typeof(Location.overrideAddress)
				};
			}

			protected override RelatedMapping GetRelatedMapping()
			{
				return new RelatedMapping(typeof(BAccount))
				{
					RelatedID = typeof(BAccount.bAccountID),
					ChildID = typeof(BAccount.defAddressID)
				};
			}

			protected override ChildMapping GetChildMapping()
			{
				return new ChildMapping(typeof(Address))
				{
					ChildID = typeof(Address.addressID),
					RelatedID = typeof(Address.bAccountID),
				};
			}

			#endregion
		}

		#endregion
	}
}
