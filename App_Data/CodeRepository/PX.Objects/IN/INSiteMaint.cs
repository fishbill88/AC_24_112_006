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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Objects.CR;
using PX.Objects.CS;
using Branch = PX.Objects.GL.Branch;

namespace PX.Objects.IN
{
	public class INSiteMaint : PXGraph<INSiteMaint, INSite>
	{
		#region Views
		public
			SelectFrom<BAccount>.
			View _bAccount;

		public
			SelectFrom<INSite>.
			Where<
				INSite.siteID.IsNotNull.
				And<INSite.siteID.IsNotEqual<SiteAttribute.transitSiteID>>.
				And<MatchUser>>.
			View site; //TODO: it is workaround AC-56410

		public
			SelectFrom<INSite>.
			Where<INSite.siteID.IsEqual<INSite.siteID.FromCurrent>>.
			View siteaccounts;

		[PXFilterable]
		[PXImport(typeof(INSite))]
		public
			SelectFrom<INLocation>.
			Where<INLocation.FK.Site.SameAsCurrent>.
			View location;

		[PXFilterable]
		[PXImport(typeof(INSite))]
		public
			SelectFrom<INCart>.
			Where<INCart.FK.Site.SameAsCurrent>.
			View carts;

		[PXFilterable]
		[PXImport(typeof(INSite))]
		public
			SelectFrom<INTote>.
			Where<INTote.FK.Site.SameAsCurrent>.
			View totes;

		public
			SelectFrom<INTote>.
			Where<INTote.FK.Cart.SameAsCurrent>.
			View totesInCart;

		public
			PXSetup<Branch>.
			Where<Branch.branchID.IsEqual<INSite.branchID.AsOptional>>
			branch;

		public
			SelectFrom<Address>.
			Where<
				Address.bAccountID.IsEqual<INSite.bAccountID.FromCurrent>.
				And<Address.addressID.IsEqual<INSite.addressID.FromCurrent>>>.
			View Address;

		public
			SelectFrom<Contact>.
			Where<
				Contact.bAccountID.IsEqual<INSite.bAccountID.FromCurrent>.
				And<Contact.contactID.IsEqual<INSite.contactID.FromCurrent>>>.
			View Contact;

		public
			SelectFrom<INItemSite>.
			Where<INItemSite.FK.Site.SameAsCurrent>.
			View itemsiterecords;

		public PXSetup<INSetup> insetup;
		#endregion

		#region Buttons
		public PXChangeID<INSite, INSite.siteCD> changeID;

		public PXAction<INSite> viewRestrictionGroups;
		[PXLookupButton(CommitChanges = true)]
		[PXUIField(DisplayName = GL.Messages.ViewRestrictionGroups, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable ViewRestrictionGroups(PXAdapter adapter)
		{
			if (site.Current != null)
			{
				INAccessDetail graph = CreateInstance<INAccessDetail>();
				graph.Site.Current = graph.Site.Search<INSite.siteCD>(site.Current.SiteCD);
				throw new PXRedirectRequiredException(graph, false, "Restricted Groups");
			}
			return adapter.Get();
		}

		[PXCancelButton]
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select)]
		protected new virtual IEnumerable Cancel(PXAdapter a)
		{
			int? siteID = getDefaultSiteID();
			if (siteID == null || PXAccess.FeatureInstalled<FeaturesSet.warehouse>())
			{
				INSite current = null;
				foreach (INSite headerCanceled in new PXCancel<INSite>(this, "Cancel").Press(a))
					current = headerCanceled;

				yield return current;
			}
			else
				yield return (INSite)
					SelectFrom<INSite>.
					Where<
						INSite.siteID.IsEqual<@P.AsInt>.
						And<MatchUser>>.
					View.Select(this, siteID);
		}

		public PXAction<INSite> validateAddresses;
		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = CS.Messages.ValidateAddresses, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, FieldClass = CS.Messages.ValidateAddress)]
		public virtual IEnumerable ValidateAddresses(PXAdapter adapter)
		{
			if (site.Current != null)
			{
				Address address = Address.Current;
				if (address != null && address.IsValidated == false)
					PXAddressValidator.Validate(this, address, true, true);
			}
			return adapter.Get();
		}

		public PXAction<INSite> viewOnMap;
		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = CR.Messages.ViewOnMap, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable ViewOnMap(PXAdapter adapter)
		{
			BAccountUtility.ViewOnMap(Address.Current);
			return adapter.Get();
		}

		public PXAction<INSite> viewTotesInCart;
		[PXButton(CommitChanges = true), PXUIField(DisplayName = "Assigned Totes", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public virtual IEnumerable ViewTotesInCart(PXAdapter adapter)
		{
			totesInCart.AskExt();
			return adapter.Get();
		}
		#endregion

		#region Initialization
		public INSiteMaint()
		{
			if (insetup.Current == null)
				throw new PXSetupNotEnteredException(ErrorMessages.SetupNotEntered, typeof(INSetup), PXMessages.LocalizeNoPrefix(Messages.INSetup));

			if (!PXAccess.FeatureInstalled<FeaturesSet.warehouse>())
			{
				site.Cache.AllowInsert = getDefaultSiteID() == null;
				Next.SetVisible(false);
				Previous.SetVisible(false);
				Last.SetVisible(false);
				First.SetVisible(false);
			}

			PXUIFieldAttribute.SetVisible<INSite.pPVAcctID>(siteaccounts.Cache, null, true);
			PXUIFieldAttribute.SetVisible<INSite.pPVSubID>(siteaccounts.Cache, null, true);

			PXUIFieldAttribute.SetDisplayName<Contact.salutation>(Caches[typeof(Contact)], CR.Messages.Attention);
			PXUIFieldAttribute.SetDisplayName<INSite.overrideInvtAccSub>(siteaccounts.Cache, PXAccess.FeatureInstalled<FeaturesSet.subAccount>() ? Messages.OverrideInventoryAcctSub : Messages.OverrideInventoryAcct);

			PXUIFieldAttribute.SetEnabled<Contact.fullName>(Caches[typeof(Contact)], null);

			PXImportAttribute importAttribute = location.Attributes.OfType<PXImportAttribute>().First();
			importAttribute.MappingPropertiesInit += MappingPropertiesInit;
		}

		public sealed override void Configure(PXScreenConfiguration config) =>
			Configure(config.GetScreenConfigurationContext<INSiteMaint, INSite>());
		protected static void Configure(WorkflowContext<INSiteMaint, INSite> context)
		{
			var locationLabels = new
			{
				ActionName = "INLocationLabels",
				ReportID = "IN619000",
				Parameters = new
				{
					Site = "WarehouseID"
				}
			};

			#region Categories
			var otherCategory = Common.CommonActionCategories.Get(context).Other;
			#endregion

			context.AddScreenConfigurationFor(screen =>
			{
				return screen
					.WithActions(actions =>
					{
						actions.Add(g => g.changeID, a => a.WithCategory(otherCategory));
						actions.Add(g => g.validateAddresses, a => a.WithCategory(otherCategory));

						actions.Add(g => g.viewRestrictionGroups, a => a.WithCategory(PredefinedCategory.Inquiries));

						actions.AddNew(locationLabels.ActionName, a => a
							.WithCategory(PredefinedCategory.Reports)
							.DisplayName(Messages.INLocationLabels)
							.IsRunReportScreen(report =>
							{
								return report
									.ReportID(locationLabels.ReportID)
									.WithWindowMode(PXBaseRedirectException.WindowMode.New)
									.WithAssignments(rass =>
									{
										rass.Add(locationLabels.Parameters.Site, z => z.SetFromField<INSite.siteCD>());
									});
							}));
					})
					.WithCategories(categories =>
					{
						categories.Add(otherCategory);
						categories.Update(FolderType.InquiriesFolder, category => category.PlaceAfter(otherCategory));
						categories.Update(FolderType.ReportsFolder, category => category.PlaceAfter(FolderType.InquiriesFolder));
					});
			});
		}
		#endregion

		#region DAC's overrides
		#region Address
		[PXDefault(typeof(Search<Branch.countryID, Where<Branch.branchID.IsEqual<INSite.branchID.FromCurrent>>>))]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void _(Events.CacheAttached<Address.countryID> e) { }
		#endregion
		#region INLocation
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(INSite.siteID), DefaultForInsert = true, DefaultForUpdate = true)]
		[PXParent(typeof(INLocation.FK.Site))]
		protected virtual void _(Events.CacheAttached<INLocation.siteID> e) { }
		#endregion
		#region INSite
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIVerify(typeof(Where<Selector<INSite.receiptLocationID, INLocation.active>, NotEqual<False>>), PXErrorLevel.Error, Messages.LocationIsNotActive, true)]
		protected virtual void _(Events.CacheAttached<INSite.receiptLocationID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIVerify(typeof(Where<Selector<INSite.shipLocationID, INLocation.active>, NotEqual<False>>), PXErrorLevel.Error, Messages.LocationIsNotActive, true)]
		protected virtual void _(Events.CacheAttached<INSite.shipLocationID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIVerify(typeof(Where<Selector<INSite.dropShipLocationID, INLocation.active>, NotEqual<False>>), PXErrorLevel.Error, Messages.LocationIsNotActive, true)]
		protected virtual void _(Events.CacheAttached<INSite.dropShipLocationID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIVerify(typeof(Where<Selector<INSite.returnLocationID, INLocation.active>, NotEqual<False>>), PXErrorLevel.Error, Messages.LocationIsNotActive, true)]
		protected virtual void _(Events.CacheAttached<INSite.returnLocationID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIVerify(typeof(Where<Selector<INSite.nonStockPickingLocationID, INLocation.active>, NotEqual<False>>), PXErrorLevel.Error, Messages.LocationIsNotActive, true)]
		protected virtual void _(Events.CacheAttached<INSite.nonStockPickingLocationID> e) { }
		#endregion
		#endregion

		#region Events
		#region INSite
		protected virtual void _(Events.FieldVerifying<INSite, INSite.active> e)
		{
			bool? isActive = (bool?)e.NewValue;
			if (e.Row == null || isActive != true) return;

			Branch branch = PXSelectorAttribute.Select<Branch.branchID>(e.Cache, e.Row) as Branch;
			if (branch != null && branch.Active != true)
			{
				throw new PXSetPropertyException<INSite.active>(Messages.WarehouseCannotBeActivated, e.Row.SiteCD, branch.BranchCD);
			}
		}

		protected virtual void _(Events.RowInserted<INSite> e)
		{
			try
			{
				var addr = new Address
				{
					BAccountID = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current)
				};
				addr = (Address)Address.Cache.Insert(addr);

				var cont = new Contact
				{
					BAccountID = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current),
					DefAddressID = addr.AddressID
				};
				cont = (Contact)Contact.Cache.Insert(cont);
			}
			finally
			{
				Address.Cache.IsDirty = false;
				Contact.Cache.IsDirty = false;
			}
		}

		protected virtual void _(Events.RowUpdated<INSite> e)
		{
			if (!e.Cache.ObjectsEqual<INSite.branchID>(e.Row, e.OldRow))
			{
				bool found = false;
				Address address = null;
				foreach (Address record in Address.Cache.Inserted)
				{
					record.BAccountID = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current);
					record.CountryID = (string)branch.Cache.GetValue<Branch.countryID>(branch.Current);
					found = true;
					address = record;
					break;
				}

				if (!found)
				{
					object old_branch = branch.View.SelectSingleBound(new object[] { e.OldRow });
					address = (Address)Address.View.SelectSingleBound(new object[] { old_branch, e.OldRow }) ?? new Address();

					address.BAccountID = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current);
					address.CountryID = (string)branch.Cache.GetValue<Branch.countryID>(branch.Current);

					address = Address.Update(address);
				}
				else
				{
					Address.Cache.Normalize();
				}

				found = false;
				foreach (Contact cont in Contact.Cache.Inserted)
				{
					cont.BAccountID = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current);
					cont.DefAddressID = address?.AddressID;

					found = true;
				}

				if (!found)
				{
					object old_branch = branch.View.SelectSingleBound(new object[] { e.OldRow });
					Contact cont = (Contact)Contact.View.SelectSingleBound(new object[] { old_branch, e.OldRow }) ?? new Contact();

					cont.BAccountID = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current);
					cont.DefAddressID = address?.AddressID;

					Contact.Update(cont);
				}
				else
				{
					Contact.Cache.Normalize();
				}
			}

			if (e.Row != null && e.OldRow != null && !PXAccess.FeatureInstalled<FeaturesSet.warehouse>() && !e.Cache.ObjectsEqual<INSite.replenishmentClassID>(e.Row, e.OldRow))
			{
				string replenishmentClassID = e.Row.ReplenishmentClassID;
				if (replenishmentClassID != null)
				{
					foreach (INItemSite initemsite in SelectFrom<INItemSite>.Where<INItemSite.siteID.IsEqual<INSite.siteID.FromCurrent>>.View.Select(this))
					{
						initemsite.ReplenishmentClassID = replenishmentClassID;
						INItemSiteMaint.DefaultItemReplenishment(this, initemsite);
						INItemSiteMaint.DefaultSubItemReplenishment(this, initemsite);
						this.Caches[typeof(INItemSite)].Update(initemsite);
					}
				}
			}
		}

		protected virtual void _(Events.FieldVerifying<INSite, INSite.receiptLocationID> e)
		{
			// temporary fix for a platform issue with query cache - AC-301969
			location.Cache.ClearQueryCache();
		}

		protected virtual void _(Events.FieldVerifying<INSite, INSite.shipLocationID> e)
		{
			// temporary fix for a platform issue with query cache - AC-301969
			location.Cache.ClearQueryCache();
		}

		protected virtual void _(Events.FieldVerifying<INSite, INSite.returnLocationID> e)
		{
			// temporary fix for a platform issue with query cache - AC-301969
			location.Cache.ClearQueryCache();
		}

		protected virtual void _(Events.FieldVerifying<INSite, INSite.dropShipLocationID> e)
		{
			// temporary fix for a platform issue with query cache - AC-301969
			location.Cache.ClearQueryCache();
		}

		protected virtual void _(Events.FieldVerifying<INSite, INSite.nonStockPickingLocationID> e)
		{
			// temporary fix for a platform issue with query cache - AC-301969
			location.Cache.ClearQueryCache();
		}

		protected virtual void _(Events.RowUpdating<INSite> e)
		{
			UpateSiteLocation<INSite.receiptLocationID, INSite.receiptLocationIDOverride>(e.Cache, e.Args);
			UpateSiteLocation<INSite.shipLocationID, INSite.shipLocationIDOverride>(e.Cache, e.Args);
		}

		protected virtual void _(Events.RowDeleted<INSite> e)
		{
			Address.Cache.Delete(Address.Current);
			Contact.Cache.Delete(Contact.Current);
		}

		protected virtual void _(Events.RowSelected<INSite> e)
		{
			INAcctSubDefault.Required(e.Cache, e.Args);
			if (e.Row != null)
			{
				viewRestrictionGroups.SetEnabled(e.Row.SiteCD != null);

				foreach (INLocation deletedLocation in location.Cache.Cached)
				{
					if (location.Cache.GetStatus(deletedLocation).IsIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
					{
						if (deletedLocation.LocationID == e.Row.ReceiptLocationID)
							e.Cache.RaiseExceptionHandling<INSite.receiptLocationID>(e.Row, deletedLocation.LocationCD, new PXSetPropertyException(ErrorMessages.ForeignRecordDeleted));
						if (deletedLocation.LocationID == e.Row.ShipLocationID)
							e.Cache.RaiseExceptionHandling<INSite.shipLocationID>(e.Row, deletedLocation.LocationCD, new PXSetPropertyException(ErrorMessages.ForeignRecordDeleted));
						if (deletedLocation.LocationID == e.Row.ReturnLocationID)
							e.Cache.RaiseExceptionHandling<INSite.returnLocationID>(e.Row, deletedLocation.LocationCD, new PXSetPropertyException(ErrorMessages.ForeignRecordDeleted));
						if (deletedLocation.LocationID == e.Row.DropShipLocationID)
							e.Cache.RaiseExceptionHandling<INSite.dropShipLocationID>(e.Row, deletedLocation.LocationCD, new PXSetPropertyException(ErrorMessages.ForeignRecordDeleted));
						if (deletedLocation.LocationID == e.Row.NonStockPickingLocationID)
							e.Cache.RaiseExceptionHandling<INSite.nonStockPickingLocationID>(e.Row, deletedLocation.LocationCD, new PXSetPropertyException(ErrorMessages.ForeignRecordDeleted));
					}
				}
			}
		}

		protected virtual void _(Events.RowPersisting<INSite> e)
		{
			INAcctSubDefault.Required(e.Cache, e.Args);
			if (e.Operation.Command().IsIn(PXDBOperation.Insert, PXDBOperation.Update))
			{
				var site = (INSite)e.Row;
				if (site.OverrideInvtAccSub != true)
				{
					PXDefaultAttribute.SetPersistingCheck<INSite.invtAcctID>(e.Cache, e.Row, PXPersistingCheck.Nothing);
					PXDefaultAttribute.SetPersistingCheck<INSite.invtSubID>(e.Cache, e.Row, PXPersistingCheck.Nothing);
				}
				if (site.ReceiptLocationIDOverride == true || site.ShipLocationIDOverride == true)
				{
					List<PXDataFieldParam> prm = new List<PXDataFieldParam>();
					if (site.ReceiptLocationIDOverride == true)
						prm.Add(new PXDataFieldAssign(typeof(INItemSite.dfltReceiptLocationID).Name, PXDbType.Int, site.ReceiptLocationID));
					if (site.ShipLocationIDOverride == true)
						prm.Add(new PXDataFieldAssign(typeof(INItemSite.dfltShipLocationID).Name, PXDbType.Int, site.ShipLocationID));
					prm.Add(new PXDataFieldRestrict(typeof(INItemSite.siteID).Name, PXDbType.Int, site.SiteID));

					// Acuminator disable once PX1043 SavingChangesInEventHandlers [Justification]
					PXDatabase.Update<INItemSite>(prm.ToArray());
				}

				if (site.Active != true)
				{
					bool cantDeactivateSite = (INRegister)
						SelectFrom<INRegister>.
						Where<
							INRegister.released.IsNotEqual<True>.
							And<
								INRegister.siteID.IsEqual<INSite.siteID.FromCurrent>.
								Or<INRegister.toSiteID.IsEqual<INSite.siteID.FromCurrent>>>>.
						View.SelectSingleBound(this, new[] { e.Row }) != null;

					cantDeactivateSite = cantDeactivateSite || (INTran)
						SelectFrom<INTran>.
						Where<
							INTran.released.IsNotEqual<True>.
							And<
								INTran.siteID.IsEqual<INSite.siteID.FromCurrent>.
								Or<INTran.toSiteID.IsEqual<INSite.siteID.FromCurrent>>>>.
						View.SelectSingleBound(this, new[] { e.Row }) != null;

					if (cantDeactivateSite)
						e.Cache.RaiseExceptionHandling<INSite.active>(e.Row, null, new PXSetPropertyException(Messages.CantDeactivateSite));
				}
			}
		}

		protected virtual void _(Events.ExceptionHandling<INSite, INSite.receiptLocationID> e)
		{
			if (IsImport)
			{
				_WrongLocations[0] = e.NewValue as string;
				e.Cancel = true;
			}
		}

		protected virtual void _(Events.ExceptionHandling<INSite, INSite.returnLocationID> e)
		{
			if (IsImport)
			{
				_WrongLocations[1] = e.NewValue as string;
				e.Cancel = true;
			}
		}

		protected virtual void _(Events.ExceptionHandling<INSite, INSite.dropShipLocationID> e)
		{
			if (IsImport)
			{
				_WrongLocations[2] = e.NewValue as string;
				e.Cancel = true;
			}
		}

		protected virtual void _(Events.ExceptionHandling<INSite, INSite.shipLocationID> e)
		{
			if (IsImport)
			{
				_WrongLocations[3] = e.NewValue as string;
				e.Cancel = true;
			}
		}

		protected virtual void _(Events.ExceptionHandling<INSite, INSite.nonStockPickingLocationID> e)
		{
			if (IsImport)
			{
				_WrongLocations[4] = e.NewValue as string;
				e.Cancel = true;
			}
		}

		protected string[] _WrongLocations = { null, null, null, null, null };
		#endregion

		#region INLocation
		protected virtual void _(Events.RowInserting<INLocation> e)
		{
			if (e.Row != null)
			{
				var located = (INLocation)e.Cache.Locate(e.Row);
				if (located != null)
					e.Row.LocationID = located.LocationID;
			}
		}

		protected virtual void _(Events.RowInserted<INLocation> e)
		{
			string cd;
			if (site.Current != null && IsImport && (cd = e.Row.LocationCD) != null)
			{
				if (_WrongLocations[0] == cd)
				{
					site.Current.ReceiptLocationID = e.Row.LocationID;
					_WrongLocations[0] = null;
				}
				if (_WrongLocations[1] == cd)
				{
					site.Current.ReturnLocationID = e.Row.LocationID;
					_WrongLocations[1] = null;
				}
				if (_WrongLocations[2] == cd)
				{
					site.Current.DropShipLocationID = e.Row.LocationID;
					_WrongLocations[2] = null;
				}
				if (_WrongLocations[3] == cd)
				{
					site.Current.ShipLocationID = e.Row.LocationID;
					_WrongLocations[3] = null;
				}
				if (_WrongLocations[4] == cd)
				{
					site.Current.NonStockPickingLocationID = e.Row.LocationID;
					_WrongLocations[4] = null;
				}
			}
		}

		protected virtual void _(Events.RowSelected<INLocation> e)
		{
			if (e.Row == null) return;

			if (e.Row.PrimaryItemID != null && InventoryItem.PK.Find(this, e.Row.PrimaryItemID) == null)
				PXUIFieldAttribute.SetWarning<INLocation.primaryItemID>(e.Cache, e.Row, Messages.ItemWasDeleted);
			PXUIFieldAttribute.SetEnabled<INLocation.isCosted>(e.Cache, e.Row, e.Row.ProjectID == null);

			bool warn = ShowWarningProjectLowestPickPriority(e.Row);
			PXUIFieldAttribute.SetWarning<INLocation.pickPriority>(e.Cache, e.Row, warn ? Messages.LocationWithProjectLowestPickPriority : null);
		}

		protected virtual void _(Events.RowPersisting<INLocation> e)
		{
			if (e.Row == null) return;

			if (e.Operation.Command() == PXDBOperation.Delete)
			{
				INItemSite itemSite =
					SelectFrom<INItemSite>.
					Where<
						INItemSite.siteID.IsEqual<INSite.siteID.FromCurrent>.
						And<
							INItemSite.dfltReceiptLocationID.IsEqual<P.AsInt>.
							Or<INItemSite.dfltShipLocationID.IsEqual<P.AsInt>>>>.
					View.Select(this, e.Row.LocationID, e.Row.LocationID);

				if (itemSite != null)
				{
					InventoryItem initem = InventoryItem.PK.Find(this, itemSite.InventoryID) ?? new InventoryItem();
					if (e.Cache.RaiseExceptionHandling<INLocation.locationID>(e.Row, e.Row.LocationCD, new PXSetPropertyException(Messages.LocationInUseInItemWarehouseDetails, PXErrorLevel.Error, e.Row.LocationCD.TrimEnd(), initem.InventoryCD.TrimEnd())))
					{
						throw new PXRowPersistingException(PXDataUtils.FieldName<INLocation.locationID>(), e.Row.LocationID, Messages.LocationInUseInItemWarehouseDetails, e.Row.LocationCD.TrimEnd(), initem.InventoryCD.TrimEnd());
					}
				}

				INPIClassLocation piLocation =
					SelectFrom<INPIClassLocation>.
					InnerJoin<INPIClass>.On<INPIClassLocation.FK.PIClass>.
					Where<
						INPIClass.siteID.IsEqual<INSite.siteID.FromCurrent>.
						And<INPIClassLocation.locationID.IsEqual<P.AsInt>>>.
					View.Select(this, e.Row.LocationID);

				if (piLocation != null)
				{
					if (e.Cache.RaiseExceptionHandling<INLocation.locationID>(e.Row, e.Row.LocationCD, new PXSetPropertyException(Messages.LocationInUseInPIType, PXErrorLevel.Error, e.Row.LocationCD.TrimEnd(), piLocation.PIClassID.TrimEnd())))
					{
						throw new PXRowPersistingException(PXDataUtils.FieldName<INLocation.locationID>(), e.Row.LocationID, Messages.LocationInUseInPIType, e.Row.LocationCD.TrimEnd(), piLocation.PIClassID.TrimEnd());
					}
				}
			}
		}

		protected virtual void _(Events.FieldVerifying<INLocation, INLocation.isCosted> e)
		{
			if (e.Row == null) return;

			bool enable;
			if ((bool?)e.NewValue == true)
			{
				INLocationStatusByCostCenter status =
					SelectFrom<INLocationStatusByCostCenter>.
					Where<
						INLocationStatusByCostCenter.siteID.IsEqual<INLocation.siteID.FromCurrent>.
						And<INLocationStatusByCostCenter.locationID.IsEqual<INLocation.locationID.FromCurrent>>.
						And<INLocationStatusByCostCenter.qtyOnHand.IsNotEqual<decimal0>>>.
					View.SelectSingleBound(this, new[] { e.Row });
				enable = status == null;
			}
			else
			{
				INCostStatus status =
					SelectFrom<INCostStatus>.
					Where<
						INCostStatus.costSiteID.IsEqual<INLocation.locationID.FromCurrent>.
						And<INCostStatus.qtyOnHand.IsGreater<decimal0>>>.
					View.SelectSingleBound(this, new[] { e.Row });
				enable = status == null;
			}

			if (!enable)
				throw new PXSetPropertyException(Messages.LocationCostedWarning, PXErrorLevel.Error);

			if ((bool?)e.NewValue == true)
				e.Cache.RaiseExceptionHandling<INLocation.isCosted>(e.Row, true, new PXSetPropertyException(Messages.LocationCostedSetWarning, PXErrorLevel.RowWarning));
		}
		#endregion

		#region Address
		protected virtual void _(Events.RowInserted<Address> e)
		{
			if (e.Row != null)
				site.Current.AddressID = e.Row.AddressID;
		}

		protected virtual void _(Events.FieldDefaulting<Address, Address.bAccountID> e)
		{
			if (branch.Current != null)
			{
				e.NewValue = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current);
				e.Cancel = true;
			}
		}

		protected virtual void _(Events.FieldUpdated<Address, Address.countryID> e)
		{
			e.Row.State = null;
			e.Row.PostalCode = null;
		}
		#endregion

		#region Contact
		protected virtual void _(Events.RowInserted<Contact> e)
		{
			if (e.Row != null)
				site.Current.ContactID = e.Row.ContactID;
		}

		protected virtual void _(Events.FieldDefaulting<Contact, Contact.bAccountID> e)
		{
			if (branch.Current != null)
			{
				e.NewValue = (int?)branch.Cache.GetValue<Branch.bAccountID>(branch.Current);
				e.Cancel = true;
			}
		}

		protected virtual void _(Events.FieldVerifying<Contact, Contact.defAddressID> e) => e.Cancel = true;

		protected virtual void _(Events.FieldDefaulting<Contact, Contact.contactType> e) => e.NewValue = ContactTypesAttribute.BAccountProperty;
		#endregion
		#endregion

		public override void Persist()
		{
			using (var ts = new PXTransactionScope())
			{
				foreach (INSite record in site.Cache.Deleted)
				{
					PXDatabase.Delete<INSiteStatusByCostCenter>(
						new PXDataFieldRestrict<INSiteStatusByCostCenter.siteID>(PXDbType.Int, 4, record.SiteID, PXComp.EQ),
						new PXDataFieldRestrict<INSiteStatusByCostCenter.qtyOnHand>(PXDbType.Decimal, 8, 0m, PXComp.EQ),
						new PXDataFieldRestrict<INSiteStatusByCostCenter.qtyAvail>(PXDbType.Decimal, 8, 0m, PXComp.EQ));

					PXDatabase.Delete<INLocationStatusByCostCenter>(
						new PXDataFieldRestrict<INLocationStatusByCostCenter.siteID>(PXDbType.Int, 4, record.SiteID, PXComp.EQ),
						new PXDataFieldRestrict<INLocationStatusByCostCenter.qtyOnHand>(PXDbType.Decimal, 8, 0m, PXComp.EQ),
						new PXDataFieldRestrict<INLocationStatusByCostCenter.qtyAvail>(PXDbType.Decimal, 8, 0m, PXComp.EQ));

					PXDatabase.Delete<INLotSerialStatusByCostCenter>(
						new PXDataFieldRestrict<INLotSerialStatusByCostCenter.siteID>(PXDbType.Int, 4, record.SiteID, PXComp.EQ),
						new PXDataFieldRestrict<INLotSerialStatusByCostCenter.qtyOnHand>(PXDbType.Decimal, 8, 0m, PXComp.EQ),
						new PXDataFieldRestrict<INLotSerialStatusByCostCenter.qtyAvail>(PXDbType.Decimal, 8, 0m, PXComp.EQ));

					InventoryItem item =
						SelectFrom<InventoryItem>.
						InnerJoin<INSiteStatusByCostCenter>.On<INSiteStatusByCostCenter.FK.InventoryItem>.
						Where<INSiteStatusByCostCenter.siteID.IsEqual<@P.AsInt>>.
						View.ReadOnly.SelectWindowed(this, 0, 1, record.SiteID);

					if (item?.InventoryCD != null)
						throw new PXRowPersistingException(typeof(INSite.siteCD).Name, record, Messages.SiteUsageDeleted, item.InventoryCD.TrimEnd());
				}

				ts.Complete();
			}

			base.Persist();
			location.Cache.Clear();
		}

		public override void Clear()
		{
			base.Clear();
			_WrongLocations[0] = null;
			_WrongLocations[1] = null;
			_WrongLocations[2] = null;
			_WrongLocations[3] = null;
			_WrongLocations[4] = null;
		}

		protected virtual bool ShowWarningProjectLowestPickPriority(INLocation row)
		{
			if (row.ProjectID == null)
				return false;

			return location.Select().RowCast<INLocation>().Any(
				l => l.Active == true
					 && l.ProjectID == null
					 && l.PickPriority >= row.PickPriority);
		}

		private void MappingPropertiesInit(object sender, PXImportAttribute.MappingPropertiesInitEventArgs e)
		{
			string isCostedFieldName = location.Cache.GetField(typeof(INLocation.isCosted));
			if (!e.Names.Contains(isCostedFieldName))
			{
				e.Names.Add(isCostedFieldName);
				e.DisplayNames.Add(PXUIFieldAttribute.GetDisplayName<INLocation.taskID>(location.Cache));
			}

			string projectTaskFieldName = location.Cache.GetField(typeof(INLocation.taskID));
			if (!e.Names.Contains(projectTaskFieldName))
			{
				e.Names.Add(projectTaskFieldName);
				e.DisplayNames.Add(PXUIFieldAttribute.GetDisplayName<INLocation.taskID>(location.Cache));
			}
		}

		protected void UpateSiteLocation<Field, FieldResult>(PXCache cache, PXRowUpdatingEventArgs e)
			where Field : IBqlField
			where FieldResult : IBqlField
		{
			int? newValue = (int?)cache.GetValue<Field>(e.NewRow);
			int? value = (int?)cache.GetValue<Field>(e.Row);
			if (value != newValue && e.ExternalCall)
			{
				INItemSite itemsite = SelectFrom<INItemSite>.Where<INItemSite.siteID.IsEqual<@P.AsInt>>.View.SelectWindowed(this, 0, 1, cache.GetValue<INSite.siteID>(e.Row));
				if (itemsite != null && site.Ask(Messages.Warning, Messages.SiteLocationOverride, MessageButtons.YesNo) == WebDialogResult.Yes)
					cache.SetValue<FieldResult>(e.NewRow, true);
				else
					cache.SetValue<FieldResult>(e.NewRow, false);
			}
		}

		protected int? getDefaultSiteID()
		{
			using (PXDataRecord record = PXDatabase.SelectSingle<INSite>(
				new PXDataField<INSite.siteID>(),
				new PXDataFieldOrder<INSite.siteID>()))
			{
				if (record != null)
					return record.GetInt32(0);
			}
			return null;
		}

		#region Address Lookup Extension
		/// <exclude/>
		public class INSiteMaintAddressLookupExtension : CR.Extensions.AddressLookupExtension<INSiteMaint, INSite, Address>
		{
			protected override string AddressView => nameof(Base.Address);
			protected override string ViewOnMap => nameof(Base.viewOnMap);
		}
		#endregion
	}
}
