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

using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.GL.DAC;

namespace PX.Objects.GL
{
	public abstract class OrganizationLedgerLinkMaintBase<TGraph, TPrimaryDAC> : PXGraphExtension<TGraph> 
		where TGraph : PXGraph 
		where TPrimaryDAC : class, IBqlTable, new()
	{
		public PXAction<TPrimaryDAC> DeleteOrganizationLedgerLink;

		public abstract PXSelectBase<OrganizationLedgerLink> OrganizationLedgerLinkSelect { get; }

		public abstract PXSelectBase<Organization> OrganizationViewBase { get; }

		public abstract PXSelectBase<Ledger> LedgerViewBase { get; }

		protected PXCache<OrganizationLedgerLink> LinkCache => (PXCache<OrganizationLedgerLink>)OrganizationLedgerLinkSelect.Cache;

		protected abstract Type VisibleField { get; }

		public override void Initialize()
		{
			base.Initialize();

			DeleteOrganizationLedgerLink.StateSelectingEvents += DeleteButtonFieldSelectingHandler;
		}

		#region Event Handlers

		protected virtual void OrganizationLedgerLink_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			var link = e.Row as OrganizationLedgerLink;

			PXSetPropertyException ex = CanBeLinkDeleted(link);

			if (ex != null)
			{
				cache.RaiseExceptionHandling(VisibleField.Name, 
												link,
												cache.GetValueExt(link, VisibleField.Name), 
												ex);
				e.Cancel = true;
			}
		}

		protected virtual void OrganizationLedgerLink_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			var link = e.Row as OrganizationLedgerLink;

			if (e.Operation == PXDBOperation.Delete)
			{
				PXSetPropertyException ex = CanBeLinkDeleted(link);

				if (ex != null)
					throw ex;
			}
		}

		protected virtual void OrganizationLedgerLink_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var link = e.Row as OrganizationLedgerLink;

			if (link == null)
				return;

			PXUIFieldAttribute.SetEnabled<OrganizationLedgerLink.organizationID>(cache, link, link.OrganizationID == null);
			PXUIFieldAttribute.SetEnabled<OrganizationLedgerLink.ledgerID>(cache, link, link.LedgerID == null);
		}

		protected virtual void OrganizationLedgerLink_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			var link = e.Row as OrganizationLedgerLink;

			if (link == null)
				return;

			var ledger = GeneralLedgerMaint.FindLedgerByID(Base, link.LedgerID, isReadonly: false);

			if (ledger.BalanceType == LedgerBalanceType.Actual)
			{
				CheckActualLedgerCanBeAssigned(ledger, link.OrganizationID.SingleToArray());
			}
		}

		protected virtual void OrganizationLedgerLink_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			var link = e.Row as OrganizationLedgerLink;

			if (link == null)
				return;

			Ledger ledger = GeneralLedgerMaint.FindLedgerByID(Base, link.LedgerID, isReadonly:false);

			LedgerViewBase.Cache.SmartSetStatus(ledger, PXEntryStatus.Updated);

			if (ledger.BalanceType == LedgerBalanceType.Actual)
			{
				Organization organization = GetUpdatingOrganization(link.OrganizationID);

				if (organization.ActualLedgerID == null)
				{
					Organization organizationCopy = PXCache<Organization>.CreateCopy(organization);
					organizationCopy.ActualLedgerID = link.LedgerID;
					OrganizationViewBase.Update(organizationCopy);
				}
			}
		}

		protected virtual void OrganizationLedgerLink_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		{
			var link = e.Row as OrganizationLedgerLink;

			if (link == null)
				return;

			Ledger ledger = GeneralLedgerMaint.FindLedgerByID(Base, link.LedgerID, isReadonly: false);
			Organization organization = GetUpdatingOrganization(link.OrganizationID);
			PXEntryStatus organizationEntryStatus = OrganizationViewBase.Cache.GetStatus(organization);

			if (ledger.BalanceType == LedgerBalanceType.Actual &&
				organizationEntryStatus != PXEntryStatus.Deleted &&
				organizationEntryStatus != PXEntryStatus.InsertedDeleted)
			{

				Organization organizationCopy = PXCache<Organization>.CreateCopy(organization);

				organizationCopy.ActualLedgerID = null;

				OrganizationViewBase.Update(organizationCopy);
			}
		}

		#endregion


		#region State Handlers

		protected virtual void DeleteButtonFieldSelectingHandler(PXCache sender, PXFieldSelectingEventArgs e)
		{
			e.ReturnState = PXButtonState.CreateDefaultState<TPrimaryDAC>(e.ReturnState);

			((PXButtonState) e.ReturnState).Enabled = OrganizationLedgerLinkSelect.AllowDelete;
		}

		#endregion


		#region Actions

		[PXButton(ImageKey = Web.UI.Sprite.Main.RecordDel, ImageSet = Web.UI.Sprite.AliasMain)]
		[PXUIField(DisplayName = ActionsMessages.Delete, MapViewRights = PXCacheRights.Delete)]
		public virtual IEnumerable deleteOrganizationLedgerLink(PXAdapter adapter)
		{
			var link = LinkCache.Current as OrganizationLedgerLink;

			if (link?.OrganizationID == null || link.LedgerID == null)
				return adapter.Get();

			Ledger ledger = GeneralLedgerMaint.FindLedgerByID(Base, link.LedgerID);

			if (ledger != null && ledger.BalanceType == LedgerBalanceType.Actual)
			{
				LinkCache.Delete(link);
			}
			else
			{
				if (GLUtility.RelatedGLHistoryExists(Base, link.LedgerID, link.OrganizationID))
				{
					Organization org = OrganizationMaint.FindOrganizationByID(Base, link.OrganizationID, true);

					WebDialogResult dialogResult = OrganizationLedgerLinkSelect.Ask(PXMessages.LocalizeFormatNoPrefix(
							Messages.AtLeastOneGeneralLedgerTransactionHasBeenPosted,
							LinkCache.GetValueExt<OrganizationLedgerLink.ledgerID>(link).ToString().Trim(),
							org.OrganizationCD.Trim()),
						MessageButtons.YesNo);

					if (dialogResult == WebDialogResult.Yes)
					{
						LinkCache.Delete(link);
					}
				}
				else
				{
					LinkCache.Delete(link);
				}
			}

			return adapter.Get();
		}

		#endregion


		#region Service Methods

		public virtual void CheckActualLedgerCanBeAssigned(Ledger ledger, int?[] organizationIDs)
		{
			IEnumerable<Organization> organizations = OrganizationMaint.FindOrganizationByIDs(Base, organizationIDs, isReadonly: false);

			foreach (Organization organization in organizations)
			{
				if (organization.ActualLedgerID != null && organization.ActualLedgerID != ledger.LedgerID)
				{
					Ledger existingLedger = GeneralLedgerMaint.FindLedgerByID(Base, organization.ActualLedgerID, isReadonly: false);

					throw new PXSetPropertyException(Messages.LedgerCannotBeAssociatedWithTheCompanyBecauseTheActualLedgerHasBeenAlreadyAssociated,
						ledger.LedgerCD,
						organization.OrganizationCD.Trim(),
						existingLedger.LedgerCD);
				}
				else if (organization.BaseCuryID != ledger.BaseCuryID)
				{
					throw new PXSetPropertyException(Messages.LedgerBaseCurrencyDifferFromCompany,
						organization.OrganizationCD.Trim(),
						ledger.LedgerCD);
				}
			}
		}

		public virtual void SetActualLedgerIDNullInRelatedCompanies(Ledger ledger, int?[] organizationIDs)
		{
			IEnumerable<Organization> organizations = OrganizationMaint.FindOrganizationByIDs(Base, organizationIDs, isReadonly: false);

			foreach (Organization organization in organizations)
			{
				Organization organizationCopy = PXCache<Organization>.CreateCopy(organization);

				organizationCopy.ActualLedgerID = null;

				OrganizationViewBase.Update(organizationCopy);
			}
		}

		protected abstract Organization GetUpdatingOrganization(int? organizationID);

		protected virtual PXSetPropertyException CanBeLinkDeleted(OrganizationLedgerLink link)
		{
			Ledger ledger = GeneralLedgerMaint.FindLedgerByID(Base, link.LedgerID);

			if (ledger != null && ledger.BalanceType == LedgerBalanceType.Actual)
			{
				if (GLUtility.RelatedGLHistoryExists(Base, link.LedgerID, link.OrganizationID))
				{
					Organization org = OrganizationMaint.FindOrganizationByID(Base, link.OrganizationID, true);
					return new PXSetPropertyException(Messages.TheRelationBetweenTheLedgerAndTheCompanyCannotBeRemovedBecauseAtLeastOneGeneralLedgerTransactionHasBeenPosted,
														PXErrorLevel.RowError,														
														LinkCache.GetValueExt<OrganizationLedgerLink.ledgerID>(link),
														org.OrganizationCD.Trim()
														);
				}
			}

			return null;
		}

		

		#endregion
	}
}
