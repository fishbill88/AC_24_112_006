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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using PX.Objects.Localizations.CA.Messages;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.GL.DAC;
using PX.SM;
using PX.Data.BQL.Fluent;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.Localizations.CA.MISCT5018Vendor;
using System.Collections.Generic;
using PX.Objects.CS;
using System.Linq;
using PX.Common;
using PX.Data.BQL;
using PX.Objects.Localizations.CA.AP;
using PX.Objects.Localizations.CA.AP.DAC;
using PX.Objects.TX;
using static PX.Data.BQL.BqlPlaceholder;
using PX.Objects.Common;

namespace PX.Objects.Localizations.CA
{
	public class T5018Fileprocessing : PXGraph<T5018Fileprocessing>, ICaptionable
	{
		private const string THRESHOLD_UNMET = "_THRESHOLD_UNMET";

		public PXSave<T5018MasterTable> Save;

		public PXAction<T5018MasterTable> Cancel;

		public PXInsert<T5018MasterTable> Insert;

		public PXDelete<T5018MasterTable> Delete;

		public PXAction<T5018MasterTable> PrepareOriginal;

		public PXAction<T5018MasterTable> PrepareAmendment;

		public PXAction<T5018MasterTable> Validate;

		public PXAction<T5018MasterTable> Generate;

		public SelectFrom<T5018MasterTable>.View MasterView;

		public PXSetup<APSetup> APSetup;

		public SelectFrom<T5018MasterTable>.
			Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.FromCurrent>.
			And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.FromCurrent>>.
			And<T5018MasterTable.revision.IsEqual<T5018MasterTable.revision.FromCurrent>>>.
			View MasterViewSummary;

		public SelectFrom<T5018EFileRow>.
			Where<T5018EFileRow.organizationID.IsEqual<T5018MasterTable.organizationID.FromCurrent>.
			And<T5018EFileRow.year.IsEqual<T5018MasterTable.year.FromCurrent>>.
			And<T5018EFileRow.revision.IsEqual<T5018MasterTable.revision.FromCurrent>>.
			And<T5018EFileRow.totalServiceAmount.IsGreaterEqual<T5018MasterTable.thresholdAmount.FromCurrent>.
				Or<T5018EFileRow.reportType.IsEqual<T5018EFileRow.reportType.canceled>>>.
			And<T5018MasterTable.filingType.FromCurrent.IsEqual<T5018MasterTable.filingType.original>.
				Or<T5018EFileRow.amendmentRow.IsEqual<True>>>>.
			View DetailsView;

		public SelectFrom<APAdjustEFileRevision>.View Transactions;

		public T5018Fileprocessing()
		{
			//workaround because overriden Delete action does not format the message properly
			string prefix;
			string localizedMessage = PXMessages.Localize(ActionsMessages.ConfirmDeleteExplicit, out prefix);
			string ConfirmationMessage = String.Format(localizedMessage, MasterView.Cache.GetName());
			this.Delete.SetConfirmationMessage(ConfirmationMessage);
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXDefault(typeof(CanadianAPSetup.t5018ThresholdAmount))]
		protected void _(Events.CacheAttached<T5018MasterTable.thresholdAmount> e) { }

		/// <summary>
		///Graph caption shown in the screen
		/// </summary>
		public string Caption()
		{
			return "";
		}

		protected void _(Events.FieldUpdated<T5018MasterTable.organizationID> e)
		{
			T5018MasterTable row = e.Row as T5018MasterTable;
			FinYear[] currentYears =
					Select<FinYear>().
					Where
					(
						FY =>
						FY.OrganizationID == row.OrganizationID &&
						FY.StartDate <= Accessinfo.BusinessDate.Value
					).OrderByDescending
					(
						FY => FY.Year
					).Take(2).ToArray();

			FinYear defaultYear = currentYears.Count() > 1 ? currentYears[1] : currentYears[0];
			e.Cache.SetValueExt<T5018MasterTable.year>(e.Row, defaultYear.Year);
		}

		protected virtual void _(Events.FieldUpdated<T5018MasterTable.year> e)
		{
			T5018MasterTable row = e.Row as T5018MasterTable;
			if (string.IsNullOrEmpty(e.NewValue as string))
			{
				e.Cache.SetValue<T5018MasterTable.revision>(e.Cache.Current, "");
			}
			else
			{
				string latestRevision =
					SelectFrom<T5018MasterTable>.
					Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
					And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>>.
					OrderBy<T5018MasterTable.createdDateTime.Desc>.
					View.ReadOnly.Select(this, new object[] { row.OrganizationID, e.NewValue }).TopFirst?.Revision;
				e.Cache.SetValueExt<T5018MasterTable.revision>(e.Row, latestRevision ?? T5018Messages.NewValue);
			}
			switch (GetReportingYear(row))
			{
				case T5018OrganizationSettings.t5018ReportingYear.CalendarYear:
					row.FromDate = Int32.TryParse(row.Year, out int year) ? new DateTime?(new DateTime(year, 1, 1)) : null;
					row.ToDate = Int32.TryParse(row.Year, out year) ? new DateTime?(new DateTime(year, 12, 31)) : null;
					break;

				case T5018OrganizationSettings.t5018ReportingYear.FiscalYear:
				default:
					FinYear finYear = FinYear.PK.Find(this, row.OrganizationID, row.Year);
					row.FromDate = finYear?.StartDate;
					row.ToDate = finYear?.EndDate - TimeSpan.FromDays(1);
					break;
			}
		}

		#region Key fields defaulting

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.organizationID> e)
		{
			e.NewValue = PXAccess.GetParentOrganizationID(PXAccess.GetBranchID());
			e.Cancel = true;
		}

		protected void _(Events.FieldDefaulting<T5018MasterTable.year> e)
		{
			T5018MasterTable row = e.Row as T5018MasterTable;
			if (row == null) return;
			switch (GetReportingYear(row))
			{
				case T5018OrganizationSettings.t5018ReportingYear.CalendarYear:
					e.NewValue = Accessinfo.BusinessDate.Value.AddYears(-1).Year.ToString();
					break;

				case T5018OrganizationSettings.t5018ReportingYear.FiscalYear:
				default:
					var currentYears = SelectFrom<FinYear>.
					Where<FinYear.organizationID.IsEqual<FinYear.organizationID.AsOptional>.
					And<FinYear.startDate.IsLessEqual<FinYear.startDate.AsOptional>>>.
					OrderBy<FinYear.endDate.Desc>.View.ReadOnly.Select(this, row.OrganizationID, Accessinfo.BusinessDate.Value);

					FinYear defaultYear = currentYears.Count() > 1 ? currentYears[1] : currentYears[0];

					e.NewValue = defaultYear.Year;
					break;
			}

			e.Cancel = true;
		}

		#endregion

		#region Summary Fields Defaulting

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.filingType> e)
		{
			T5018MasterTable table =
				SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
				And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>>.
				View.Select(this, (e.Row as T5018MasterTable)?.OrganizationID, (e.Row as T5018MasterTable)?.Year);

			e.NewValue = table == null ? T5018MasterTable.filingType.Original : T5018MasterTable.filingType.Amendment;
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.addressLine1> e)
		{
			Organization organization = OrganizationMaint.FindOrganizationByID(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			Address address =
					SelectFrom<Address>.
					InnerJoin<BAccountR>.On<BAccountR.defAddressID.IsEqual<Address.addressID>>.
					Where<BAccountR.bAccountID.IsEqual<Organization.bAccountID.AsOptional>>.View.Select(this, new object[] { organization?.BAccountID });

			e.NewValue = address?.AddressLine1;
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.addressLine2> e)
		{
			Organization organization = OrganizationMaint.FindOrganizationByID(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			Address address =
					SelectFrom<Address>.
					InnerJoin<BAccountR>.On<BAccountR.defAddressID.IsEqual<Address.addressID>>.
					Where<BAccountR.bAccountID.IsEqual<Organization.bAccountID.AsOptional>>.View.Select(this, new object[] { organization?.BAccountID });

			e.NewValue = address?.AddressLine2;
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.city> e)
		{
			Organization organization = OrganizationMaint.FindOrganizationByID(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			Address address =
					SelectFrom<Address>.
					InnerJoin<BAccountR>.On<BAccountR.defAddressID.IsEqual<Address.addressID>>.
					Where<BAccountR.bAccountID.IsEqual<Organization.bAccountID.AsOptional>>.View.Select(this, new object[] { organization?.BAccountID });

			e.NewValue = address?.City;
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.province> e)
		{
			Organization organization = OrganizationMaint.FindOrganizationByID(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			Address address =
					SelectFrom<Address>.
					InnerJoin<BAccountR>.On<BAccountR.defAddressID.IsEqual<Address.addressID>>.
					Where<BAccountR.bAccountID.IsEqual<Organization.bAccountID.AsOptional>>.View.Select(this, new object[] { organization?.BAccountID });

			e.NewValue = address?.State;
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.country> e)
		{
			Organization organization = OrganizationMaint.FindOrganizationByID(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			Address address =
					SelectFrom<Address>.
					InnerJoin<BAccountR>.On<BAccountR.defAddressID.IsEqual<Address.addressID>>.
					Where<BAccountR.bAccountID.IsEqual<Organization.bAccountID.AsOptional>>.View.Select(this, new object[] { organization?.BAccountID });

			switch (address?.CountryID)
			{
				case "CA":
					e.NewValue = "CAN";
					break;

				case "US":
					e.NewValue = "USA";
					break;

				default:
					e.NewValue = "";
					break;
			}
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.postalCode> e)
		{
			Organization organization = OrganizationMaint.FindOrganizationByID(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			Address address =
					SelectFrom<Address>.
					InnerJoin<BAccountR>.On<BAccountR.defAddressID.IsEqual<Address.addressID>>.
					Where<BAccountR.bAccountID.IsEqual<Organization.bAccountID.AsOptional>>.View.Select(this, new object[] { organization?.BAccountID });

			e.NewValue = address?.PostalCode;
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.acctName> e) => e.NewValue = OrganizationMaint.FindOrganizationByID(this, ((T5018MasterTable)e.Row)?.OrganizationID)?.OrganizationName;

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.programNumber> e)
		{
			T5018OrganizationSettings t5018OrganizationSettings = T5018OrganizationSettings.PK.Find(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			e.NewValue = t5018OrganizationSettings?.ProgramNumber;
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.transmitterNumber> e)
		{
			T5018OrganizationSettings t5018OrganizationSettings = T5018OrganizationSettings.PK.Find(this, ((T5018MasterTable)e.Row)?.OrganizationID);
			e.NewValue = t5018OrganizationSettings?.TransmitterNumber ?? "MM555555";
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.name> e) => e.NewValue = OrganizationMaint.GetDefaultContact(this, ((T5018MasterTable)e.Row)?.OrganizationID)?.Attention;

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.areaCode> e) => e.NewValue = PhoneNumberMatch(((T5018MasterTable)e.Row)?.OrganizationID)?.Groups[1].Value.Replace("(", "").Replace(")", "");

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.phone> e)
		{
			Match match = PhoneNumberMatch(((T5018MasterTable)e.Row)?.OrganizationID);
			if (match != null && match.Success)
			{
				e.NewValue = match.Groups[2].Value.Contains("-") ? match.Groups[2].Value : match.Groups[2].Value.Substring(0, 3) + "-" + match.Groups[2].Value.Substring(3);
			}
			else
			{
				e.NewValue = "";
			}
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.extensionNbr> e)
		{
			Match match = PhoneNumberMatch(((T5018MasterTable)e.Row)?.OrganizationID);
			if (match != null && match.Success)
				e.NewValue = match.Groups[3].Value;
			else
				e.NewValue = "";
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.email> e) => e.NewValue = OrganizationMaint.GetDefaultContact(this, ((T5018MasterTable)e.Row)?.OrganizationID)?.EMail;

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.fromDate> e)
		{
			T5018MasterTable row = e.Row as T5018MasterTable;

			switch (GetReportingYear(row))
			{
				case T5018OrganizationSettings.t5018ReportingYear.CalendarYear:
					e.NewValue = Int32.TryParse(row.Year, out int year) ? new DateTime?(new DateTime(year, 1, 1)) : null;
					break;

				case T5018OrganizationSettings.t5018ReportingYear.FiscalYear:
				default:
					FinYear finYear = FinYear.PK.Find(this, row.OrganizationID, row.Year);
					e.NewValue = finYear?.StartDate;
					break;
			}
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.toDate> e)
		{
			T5018MasterTable row = e.Row as T5018MasterTable;

			switch (GetReportingYear(row))
			{
				case T5018OrganizationSettings.t5018ReportingYear.CalendarYear:
					e.NewValue = Int32.TryParse(row.Year, out int year) ? new DateTime?(new DateTime(year, 12, 31)) : null;
					break;

				case T5018OrganizationSettings.t5018ReportingYear.FiscalYear:
				default:
					FinYear finYear = FinYear.PK.Find(this, row.OrganizationID, row.Year);
					e.NewValue = finYear?.EndDate - TimeSpan.FromDays(1);
					break;
			}
		}

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.revisionSubmitted> e) => e.NewValue = false;

		protected virtual void _(Events.FieldDefaulting<T5018MasterTable.language> e) => e.NewValue = T5018MasterTable.language.English;

		#endregion

		protected virtual void _(Events.RowInserting<T5018MasterTable> e)
		{
			e.Cache.SetValue<T5018MasterTable.revision>(e.Row, PXMessages.LocalizeNoPrefix(T5018Messages.NewValue));
		}

		protected virtual void _(Events.RowPersisting<T5018MasterTable> e)
		{
			if (e.Operation != PXDBOperation.Delete && e.Row != MasterView.Current)
			{
				e.Cancel = true;
				return;
			}

			T5018MasterTable row = e.Row;

			if (String.Equals(row.Revision, T5018Messages.NewValue))
			{

				string latestRevision = SelectFrom<T5018MasterTable>.
									Where<
									T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.And<
									T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>>.
									Order<By<T5018MasterTable.createdDateTime.Desc>>.View.ReadOnly.Select(this, new object[] { row.OrganizationID, row.Year })?.TopFirst?.Revision;

				if (String.IsNullOrEmpty(latestRevision))
					latestRevision = "0";

				latestRevision = (Int32.Parse(latestRevision) + 1).ToString();

				row.Revision = latestRevision;
				e.Cache.Normalize();

				//Solution works only if there are no more than 255 organizations within a tenenat
				//and no more than 99 revisions within a year.
				string NewSubmissionNumber = "";
				//Converting orgid into hex representation with min 2 digits
				string OrgId = row.OrganizationID.Value.ToString("X2");
				NewSubmissionNumber += OrgId.Substring(OrgId.Length - 2);
				NewSubmissionNumber += row.Year;
				//Trimming revision to 2 digits only
				string RevId = row.Revision.PadLeft(2).Replace(" ", "0");
				NewSubmissionNumber += RevId.Substring(RevId.Length - 2);
				e.Row.SubmissionNo = NewSubmissionNumber;
			}
		}

		protected virtual void _(Events.RowPersisted<T5018MasterTable> e)
		{
			if (e.TranStatus == PXTranStatus.Aborted)
			{
				T5018MasterTable latestRevision =
					SelectFrom<T5018MasterTable>.Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.FromCurrent>.
					And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.FromCurrent>>>.
					Order<By<T5018MasterTable.createdDateTime.Desc>>.
					View.ReadOnly.Select(this).TopFirst;

				MasterView.Cache.Current = latestRevision ?? MasterView.Cache.Insert();
			}
		}

		protected void _(Events.RowSelected<T5018MasterTable> e)
		{
			Insert.SetVisible(false);
			T5018MasterTable row = e.Row;
			if (row == null) return;

			if (!row.OrganizationID.HasValue || String.IsNullOrEmpty(row.Revision) || String.IsNullOrEmpty(row.Year))
			{
				Validate.SetEnabled(false);
				Generate.SetEnabled(false);
				PrepareAmendment.SetEnabled(false);
				PrepareOriginal.SetEnabled(false);
				Delete.SetEnabled(false);
				return;
			}

			if (((T5018MasterTable)MasterView.Cache.Current).Revision.Equals(T5018Messages.NewValue))
				MasterView.Cache.IsDirty = false;

			T5018MasterTable t5018MasterTable =
				SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.
				IsEqual<T5018MasterTable.organizationID.FromCurrent>.
				And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.FromCurrent>>>.View.ReadOnly.Select(this);

			PrepareAmendment.SetVisible(t5018MasterTable != null);
			PrepareOriginal.SetVisible(t5018MasterTable == null);

			Save.SetEnabled(row.Revision != null && row.Revision != PXMessages.LocalizeNoPrefix(T5018Messages.NewValue));
			Delete.SetEnabled(row.Revision != null && row.Revision != PXMessages.LocalizeNoPrefix(T5018Messages.NewValue));
			Validate.SetEnabled(row.Revision != null && row.Revision != PXMessages.LocalizeNoPrefix(T5018Messages.NewValue));
			Generate.SetEnabled(row.Revision != null && row.Revision != PXMessages.LocalizeNoPrefix(T5018Messages.NewValue));

			T5018MasterTable table =
				SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.FromCurrent>.
				And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.FromCurrent>>.
				And<T5018MasterTable.createdDateTime.IsGreater<T5018MasterTable.createdDateTime.FromCurrent>>>.View.ReadOnly.Select(this);

			PXUIFieldAttribute.SetEnabled<T5018MasterTable.revisionSubmitted>(e.Cache, row, row.Revision != null && row.Revision != PXMessages.LocalizeNoPrefix(T5018Messages.NewValue) && (table == null || !row.RevisionSubmitted.Value));
			PrepareOriginal.SetEnabled(row.Revision != null && row.Revision == PXMessages.LocalizeNoPrefix(T5018Messages.NewValue));

			PXUIFieldAttribute.SetEnabled<T5018MasterTable.thresholdAmount>(e.Cache, row, row.Revision == T5018Messages.NewValue);
		}

		protected void _(Events.RowDeleting<T5018MasterTable> e)
		{
			if (e.Row == null || !e.Row.OrganizationID.HasValue || String.IsNullOrEmpty(e.Row.Revision) || String.IsNullOrEmpty(e.Row.Year))
			{
				e.Cancel = true;
				return;
			}

			T5018MasterTable latestRevision =
					SelectFrom<T5018MasterTable>.Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
					And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>>.
					Order<By<T5018MasterTable.createdDateTime.Desc>>.
					View.ReadOnly.Select(this, e.Row.OrganizationID, e.Row.Year).TopFirst;

			if (latestRevision == null || !e.Row.Revision.Trim().Equals(latestRevision.Revision.Trim()))
			{
				e.Cancel = true;
				throw new PXException(T5018Messages.NotLatestRevision);
			}
		}

		/// <summary>
		/// Greps a phone number into it's applicable sections
		/// </summary>
		/// <returns>Regex Match collection</returns>
		private Match PhoneNumberMatch(int? id)
		{
			if (id == null) return null;
			string phonePattern = @"^\+?1?\(?(\d{3})\)?[\s\-]?(\d{3}[\s\-]?\d{4})(.*)$";
			Regex phoneRegex = new Regex(phonePattern);
			return phoneRegex.Match(OrganizationMaint.GetDefaultContact(this, id)?.Phone1 ?? "");
		}

		private int? GetReportingYear(T5018MasterTable table)
		{
			T5018OrganizationSettings t5018OrganizationSettings = T5018OrganizationSettings.PK.Find(this, table?.OrganizationID);
			return t5018OrganizationSettings?.T5018ReportingYear;
		}

		[PXUIField(DisplayName = ActionsMessages.Delete, MapEnableRights = PXCacheRights.Delete, MapViewRights = PXCacheRights.Delete)]
		[PXDeleteButton()]
		public virtual IEnumerable delete(PXAdapter a)
		{
			//after delete we should show the last existing revision for current organization and current year
			T5018MasterTable current = a.View.Cache.Current as T5018MasterTable;
			object res = null;
			foreach (T5018MasterTable header in new PXDelete<T5018MasterTable>(this, "Delete").Press(a))
			{
				res = header;
			}
			//For Revision equal to "1" and inserted record, we don't need to change the behavior
			if (current != null)
			{
				int? orgId = current.OrganizationID;
				string year = current.Year;
				if (!String.Equals(current.Revision, "1")
					&& !String.Equals(current.Revision, PXMessages.LocalizeNoPrefix(T5018Messages.NewValue)))
				{
					T5018MasterTable latestRevision;
					latestRevision = SelectFrom<T5018MasterTable>.
						Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
						And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>>.
						Order<By<T5018MasterTable.createdDateTime.Desc>>.
						View.ReadOnly.Select(this, new object[] { orgId, year })?.TopFirst;

					if (latestRevision != null)
					{
						yield return latestRevision;
						yield break;
					}
					yield return current;
					yield break;
				}
			}
			yield return res;
		}

		[PXCancelButton]
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select)]
		public virtual IEnumerable cancel(PXAdapter a)
		{
			T5018MasterTable current = null;
			string organizationCD = null;
			string year = null;
			string revision = null;
			bool cancelOnInsert = false;

			if (a.Searches != null)
			{
				if (a.Searches.Length > 0)
					organizationCD = (string)a.Searches[0];
				if (a.Searches.Length > 1)
					year = (string)a.Searches[1];
				if (a.Searches.Length > 2)
					revision = (string)a.Searches[2];
			}

			Organization org = OrganizationMaint.FindOrganizationByCD(this, organizationCD);
			FinYear finYear = null;

			if (org == null)
			{
				if (a.Searches != null)
				{
					if (a.Searches.Length > 1)
					{
						a.Searches[1] = null;
						year = null;
					}
					if (a.Searches.Length > 2)
						a.Searches[2] = null;
				}
			}
			else
				finYear = FinYear.PK.Find(this, org.OrganizationID, year);

			if (finYear == null && a.Searches != null && a.Searches.Length > 2)
			{
				a.Searches[2] = null;
				year = null;
			}

			if (org != null && finYear != null
				&& String.Equals(revision, PXMessages.LocalizeNoPrefix(T5018Messages.NewValue)))
			{
				cancelOnInsert = true;
			}

			T5018MasterTable t5018MasterTable = null;

			if (org != null)
				t5018MasterTable = SelectFrom<T5018MasterTable>.
					Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
					And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>.
					And<T5018MasterTable.revision.IsEqual<T5018MasterTable.revision.AsOptional>>>.
					View.ReadOnly.Select(this, new object[] { org.OrganizationID, year, revision });
			bool insertNewRevision = false;
			if (t5018MasterTable == null)
			{
				if (a.Searches != null && a.Searches.Length > 2)
					a.Searches[2] = null;
				if (year != null)
					insertNewRevision = true;
			}

			if (year != null && (MasterView.Current != null && (!MasterView.Current.Year.Equals(year)) || cancelOnInsert))
			{
				T5018MasterTable latestRevision;
				latestRevision = SelectFrom<T5018MasterTable>.
					Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
					And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>>.
					Order<By<T5018MasterTable.createdDateTime.Desc>>.
					View.ReadOnly.Select(this, new object[] { org.OrganizationID, year })?.TopFirst;

				if (latestRevision != null && a.Searches.Length > 2)
				{
					a.Searches[2] = latestRevision.Revision;
					insertNewRevision = false;
				}
			}

			foreach (T5018MasterTable headerCanceled in (new PXCancel<T5018MasterTable>(this, "Cancel").Press(a)))
			{
				current = headerCanceled;
			}

			if (insertNewRevision)
			{
				MasterView.Cache.Remove(current);

				T5018MasterTable newRevision = new T5018MasterTable();
				newRevision.OrganizationID = org?.OrganizationID;
				newRevision.Year = year;

				current = MasterView.Insert(newRevision);

				current.Revision = PXMessages.LocalizeNoPrefix(T5018Messages.NewValue);

				MasterView.Cache.Normalize();
			}
			else
				MasterView.Cache.IsDirty = false;

			yield return current;
		}

		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = T5018Messages.ReportButton)]
		public virtual IEnumerable validate(PXAdapter a)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			string reportID = "AP607600";

			PXReportRequiredException ex = null;
			Dictionary<PrintSettings, PXReportRequiredException> reportsToPrint =
				new Dictionary<PrintSettings, PXReportRequiredException>();

			parameters["Transmitter"] = a.Searches[0] as string;
			parameters["T5018Year"] = a.Searches[1] as string;
			parameters["Revision"] = a.Searches[2] as string;

			ex = PXReportRequiredException.CombineReport(ex, reportID, parameters);
			ex.Mode = PXBaseRedirectException.WindowMode.New;

			if (ex != null)
			{
				throw ex;
			}

			return a.Get<T5018MasterTable>();
		}

		public override IEnumerable ExecuteSelect(string viewName, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
		{
			if (viewName == nameof(MasterView) && searches.Length >= 3 && searches[2] != null && searches[2].ToString() != 1.ToString())
			{
				Dictionary<string, T5018MasterTable> dict = PXDatabase.GetSlot<Dictionary<string, T5018MasterTable>>(THRESHOLD_UNMET, typeof(T5018EFileRow));
				int? companyID = OrganizationMaint.FindOrganizationByCD(this, searches[0].ToString())?.OrganizationID.Value;
				//in Modern UI searches[0] is ID instead of CD
				if (companyID == null)
				{
					companyID = OrganizationMaint.FindOrganizationByID(this, searches[0] as int?)?.OrganizationID.Value;
				}
				if ((companyID != null) &&
					dict.TryGetValue(THRESHOLD_UNMET, out T5018MasterTable table) && table != null
					&& table.OrganizationID.ToString() == companyID.ToString()
					&& table.Year.ToString() == searches[1].ToString()
					&& table.Revision == searches[2].ToString())
				{
					PXDatabase.ResetSlot<T5018MasterTable>(THRESHOLD_UNMET, typeof(T5018EFileRow));

					T5018MasterTable latestRevision =
						SelectFrom<T5018MasterTable>.
						Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
						And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>>.
						OrderBy<T5018MasterTable.createdDateTime.Desc>.
						View.ReadOnly.Select(this, new object[] { searches[0], searches[1] }).TopFirst;

					if (latestRevision != null)
					{
						searches[2] = latestRevision.Revision;
					}
				}
			}

			return base.ExecuteSelect(viewName, parameters, searches, sortcolumns, descendings, filters, ref startRow, maximumRows, ref totalRows);
		}

		[PXButton(CommitChanges = true)]
		[PXUIField(DisplayName = T5018Messages.GenerateButton)]
		public void generate()
		{
			if (DetailsView.Select().Count == 0)
				return;

			foreach (var ReportType in DetailsView.Select().GroupBy(e => e.GetItem<T5018EFileRow>().ReportType))
			{

				decimal? subcontractorSumPayments = default(decimal);

				T5018OrganizationSettings t5018OrganizationSettings = T5018OrganizationSettings.PK.Find(this, (MasterView.Current)?.OrganizationID);
				Organization organization = OrganizationMaint.FindOrganizationByID(this, MasterView.Current.OrganizationID);

				XmlDocument xmlDocument = new XmlDocument();
				XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "");
				XmlElement xmlElement = xmlDocument.CreateElement("Submission");
				xmlElement.SetAttribute("xmlns:ccms", "http://www.cra-arc.gc.ca/xmlns/ccms/1-0-0");
				xmlElement.SetAttribute("xmlns:sdt", "http://www.cra-arc.gc.ca/xmlns/sdt/2-2-0");
				xmlElement.SetAttribute("xmlns:ols",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols/1-0-1");
				xmlElement.SetAttribute("xmlns:ols1",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols1/1-0-1");
				xmlElement.SetAttribute("xmlns:ols10",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols10/1-0-1");
				xmlElement.SetAttribute("xmlns:ols100",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols100/1-0-1");
				xmlElement.SetAttribute("xmlns:ols12",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols12/1-0-1");
				xmlElement.SetAttribute("xmlns:ols125",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols125/1-0-1");
				xmlElement.SetAttribute("xmlns:ols140",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols140/1-0-1");
				xmlElement.SetAttribute("xmlns:ols141",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols141/1-0-1");
				xmlElement.SetAttribute("xmlns:ols2",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols2/1-0-1");
				xmlElement.SetAttribute("xmlns:ols5",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols5/1-0-1");
				xmlElement.SetAttribute("xmlns:ols50",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols50/1-0-1");
				xmlElement.SetAttribute("xmlns:ols52",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols52/1-0-1");
				xmlElement.SetAttribute("xmlns:ols6",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols6/1-0-1");
				xmlElement.SetAttribute("xmlns:ols8",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols8/1-0-1");
				xmlElement.SetAttribute("xmlns:ols8-1",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols8-1/1-0-1");
				xmlElement.SetAttribute("xmlns:ols9",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/ols9/1-0-1");
				xmlElement.SetAttribute("xmlns:olsbr",
										"http://www.cra-arc.gc.ca/enov/ol/interfaces/efile/partnership/olsbr/1-0-1");
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
				XmlAttribute xmlAttribute =
					xmlDocument.CreateAttribute("xsi", "noNamespaceSchemaLocation",
												"http://www.w3.org/2001/XMLSchema-instance");
				xmlAttribute.Value = "layout-topologie.xsd";
				xmlElement.SetAttributeNode(xmlAttribute);
				XmlNode xmlNode = xmlDocument.CreateElement("T619");
				XmlNode xmlNode2 = xmlDocument.CreateElement("sbmt_ref_id");
				xmlNode2.InnerText = MasterView.Current.SubmissionNo;
				xmlNode.AppendChild(xmlNode2);
				XmlNode xmlNode3 = xmlDocument.CreateElement("rpt_tcd");
				xmlNode3.InnerText = MasterView.Current.FilingType;
				xmlNode.AppendChild(xmlNode3);
				XmlNode xmlNode4 = xmlDocument.CreateElement("trnmtr_nbr");
				xmlNode4.InnerText = t5018OrganizationSettings?.TransmitterNumber;
				xmlNode.AppendChild(xmlNode4);
				XmlNode xmlNode5 = xmlDocument.CreateElement("trnmtr_tcd");
				xmlNode5.InnerText = "1";
				xmlNode.AppendChild(xmlNode5);
				XmlNode xmlNode6 = xmlDocument.CreateElement("summ_cnt");
				xmlNode6.InnerText = "1";
				xmlNode.AppendChild(xmlNode6);
				XmlNode xmlNode7 = xmlDocument.CreateElement("lang_cd");
				xmlNode7.InnerText = MasterView.Current.Language;
				xmlNode.AppendChild(xmlNode7);
				XmlNode xmlNode8 = xmlDocument.CreateElement("TRNMTR_NM");
				int num2 = 0;
				string text = null;
				string text2 = null;
				if (!string.IsNullOrEmpty(MasterView.Current.AcctName))
				{
					string acctName = MasterView.Current.AcctName;
					for (int i = 0; i < acctName.Length; i++)
					{
						char c = acctName[i];
						if (num2 <= 30)
						{
							text += c;
						}

						num2++;
						if (num2 > 31)
						{
							text2 += c;
						}
					}
				}

				XmlNode xmlNode9 = xmlDocument.CreateElement("l1_nm");
				xmlNode9.InnerText = text;
				xmlNode8.AppendChild(xmlNode9);
				if (num2 > 31)
				{
					XmlNode xmlNode10 = xmlDocument.CreateElement("l2_nm");
					xmlNode10.InnerText = text2;
					xmlNode8.AppendChild(xmlNode10);
				}

				xmlNode.AppendChild(xmlNode8);
				XmlNode xmlNode11 = xmlDocument.CreateElement("TRNMTR_ADDR");
				XmlNode xmlNode12 = xmlDocument.CreateElement("addr_l1_txt");
				xmlNode12.InnerText = MasterView.Current.AddressLine1;
				xmlNode11.AppendChild(xmlNode12);
				XmlNode xmlNode13 = xmlDocument.CreateElement("addr_l2_txt");
				xmlNode13.InnerText = MasterView.Current.AddressLine2;
				xmlNode11.AppendChild(xmlNode13);
				XmlNode xmlNode14 = xmlDocument.CreateElement("cty_nm");
				xmlNode14.InnerText = MasterView.Current.City;
				xmlNode11.AppendChild(xmlNode14);
				XmlNode xmlNode15 = xmlDocument.CreateElement("prov_cd");
				xmlNode15.InnerText = MasterView.Current.Province;
				xmlNode11.AppendChild(xmlNode15);
				XmlNode xmlNode16 = xmlDocument.CreateElement("cntry_cd");
				xmlNode16.InnerText = MasterView.Current.Country;
				xmlNode11.AppendChild(xmlNode16);
				XmlNode xmlNode17 = xmlDocument.CreateElement("pstl_cd");
				xmlNode17.InnerText = MasterView.Current.PostalCode;
				xmlNode11.AppendChild(xmlNode17);
				xmlNode.AppendChild(xmlNode11);
				XmlNode xmlNode18 = xmlDocument.CreateElement("CNTC");
				XmlNode xmlNode19 = xmlDocument.CreateElement("cntc_nm");
				xmlNode19.InnerText = MasterView.Current.Name;
				xmlNode18.AppendChild(xmlNode19);
				XmlNode xmlNode20 = xmlDocument.CreateElement("cntc_area_cd");
				xmlNode20.InnerText = MasterView.Current.AreaCode;
				xmlNode18.AppendChild(xmlNode20);
				XmlNode xmlNode21 = xmlDocument.CreateElement("cntc_phn_nbr");
				xmlNode21.InnerText = MasterView.Current.Phone;
				xmlNode18.AppendChild(xmlNode21);
				XmlNode xmlNode22 = xmlDocument.CreateElement("cntc_extn_nbr");
				xmlNode22.InnerText = MasterView.Current.ExtensionNbr;
				xmlNode18.AppendChild(xmlNode22);
				XmlNode xmlNode23 = xmlDocument.CreateElement("cntc_email_area");
				xmlNode23.InnerText = MasterView.Current.Email;
				xmlNode18.AppendChild(xmlNode23);
				XmlNode xmlNode24 = xmlDocument.CreateElement("sec_cntc_email_area");
				xmlNode24.InnerText = MasterView.Current.SecondEmail;
				xmlNode18.AppendChild(xmlNode24);
				xmlNode.AppendChild(xmlNode18);
				xmlElement.AppendChild(xmlNode);
				XmlNode xmlNode25 = xmlDocument.CreateElement("Return");
				XmlNode xmlNode26 = xmlDocument.CreateElement("T5018");
				xmlNode25.AppendChild(xmlNode26);
				int slipcount = 0;
				foreach (var result in DetailsView.Select().Where(e => e.GetItem<T5018EFileRow>().ReportType == ReportType.Key))
				{
					var item = result.GetItem<T5018EFileRow>();
					BAccountR bAccount2 =
						SelectFrom<BAccountR>.
						Where<BAccountR.acctCD.IsEqual<BAccountR.acctCD.AsOptional>>.View
							.Select(this, item.VAcctCD.Trim());
					T5018VendorExt VendorExtension = PXCache<BAccount>.GetExtension<T5018VendorExt>(bAccount2);
					Contact contact2 =
						SelectFrom<Contact>.
						Where<Contact.contactID.IsEqual<BAccountR.primaryContactID.AsOptional>>.View
						.Select(this, bAccount2.PrimaryContactID);

					Address address2 =
						SelectFrom<Address>.
						Where<Address.bAccountID.IsEqual<BAccountR.bAccountID.AsOptional>>.View
							.Select(this, bAccount2.BAccountID);
					T5018EFileRow previousRow = null;
					if (ReportType.Key == T5018EFileRow.reportType.Canceled)
						previousRow =
							SelectFrom<T5018EFileRow>.
							Where<T5018EFileRow.organizationID.IsEqual<T5018EFileRow.organizationID.AsOptional>.
							And<T5018EFileRow.year.IsEqual<T5018EFileRow.year.AsOptional>.
							And<T5018EFileRow.revision.IsEqual<T5018EFileRow.revision.AsOptional>.
							And<T5018EFileRow.vAcctCD.IsEqual<T5018EFileRow.vAcctCD.AsOptional>>>>>.View
							.Select(this, MasterView.Current.OrganizationID, MasterView.Current.Year,
							(Int32.Parse(MasterView.Current.Revision) - 1).ToString(), item.VAcctCD);

					subcontractorSumPayments += item.ReportType != T5018EFileRow.reportType.Canceled ? item.Amount : previousRow.Amount;
					slipcount++;
					XmlNode xmlNode27 = xmlDocument.CreateElement("T5018Slip");
					xmlNode26.AppendChild(xmlNode27);

					XmlNode xmlNode33 = xmlDocument.CreateElement("sin");
					switch (VendorExtension.BoxT5018)
					{
						case T5018VendorExt.boxT5018.Corporation:
							xmlNode33.InnerText = "";
							break;
						case T5018VendorExt.boxT5018.Partnership:
							xmlNode33.InnerText = "";
							break;
						case T5018VendorExt.boxT5018.Individual:
							xmlNode33.InnerText = VendorExtension.SocialInsNum;
							break;
						default:
							xmlNode33.InnerText = "";
							break;
					}

					xmlNode27.AppendChild(xmlNode33);
					XmlNode xmlNode34 = xmlDocument.CreateElement("rcpnt_bn");
					if (VendorExtension.BusinessNumber != null)
					{
						xmlNode34.InnerText = VendorExtension.BusinessNumber;
					}
					else
					{
						xmlNode34.InnerText = "";
					}

					xmlNode27.AppendChild(xmlNode34);
					XmlNode xmlNode38 = xmlDocument.CreateElement("rcpnt_tcd");
					switch (VendorExtension.BoxT5018)
					{
						case T5018VendorExt.boxT5018.Corporation:
						case T5018VendorExt.boxT5018.Partnership:
							xmlNode38.InnerText = VendorExtension.BoxT5018 == T5018VendorExt.boxT5018.Corporation ? "3" : "4";
							XmlNode xmlNode35 = xmlDocument.CreateElement("CORP_PTNRP_NM");
							XmlNode xmlNode36 = xmlDocument.CreateElement("l1_nm");
							string corpName = item.VendorName.Replace("&", "&amp;");
							xmlNode36.InnerText = corpName.Length > 30 ? corpName.Substring(0, 30) : corpName;
							xmlNode35.AppendChild(xmlNode36);
							XmlNode xmlNode37 = xmlDocument.CreateElement("l2_nm");
							xmlNode37.InnerText = (corpName ?? "").Length > 30 ?
								(corpName.Substring(30).Length > 30 ?
									corpName.Substring(30, 30) :
									corpName.Substring(30)) :
								"";
							xmlNode35.AppendChild(xmlNode37);
							xmlNode27.AppendChild(xmlNode35);
							break;
						case T5018VendorExt.boxT5018.Individual:
							xmlNode38.InnerText = "1";
							XmlNode xmlNode28 = xmlDocument.CreateElement("RCPNT_NM");
							XmlNode xmlNode29 = xmlDocument.CreateElement("snm");
							XmlNode xmlNode30 = xmlDocument.CreateElement("gvn_nm");
							XmlNode xmlNode32 = xmlDocument.CreateElement("init");

							string givenName = contact2?.FirstName != null ? contact2.FirstName.Split(' ')[0] : "";
							givenName = givenName.Length > 12 ? givenName.Substring(0, 12) : givenName;

							string surname = contact2?.LastName ?? "";
							surname = surname.Length > 20 ? surname.Substring(0, 20) : surname;

							xmlNode29.InnerText = surname;
							xmlNode28.AppendChild(xmlNode29);

							xmlNode30.InnerText = givenName;
							xmlNode28.AppendChild(xmlNode30);

							xmlNode32.InnerText = "";
							xmlNode28.AppendChild(xmlNode32);
							xmlNode27.InsertBefore(xmlNode28, xmlNode33);
							break;
						default:
							xmlNode38.InnerText = "";
							break;
					}

					xmlNode27.AppendChild(xmlNode38);
					XmlNode xmlNode39 = xmlDocument.CreateElement("RCPNT_ADDR");
					XmlNode xmlNode40 = xmlDocument.CreateElement("addr_l1_txt");
					xmlNode40.InnerText = address2.AddressLine1;
					xmlNode39.AppendChild(xmlNode40);
					XmlNode xmlNode41 = xmlDocument.CreateElement("addr_l2_txt");
					if (address2.AddressLine2 != null)
					{
						xmlNode41.InnerText = address2.AddressLine2;
					}
					else
					{
						xmlNode41.InnerText = address2.AddressLine2;
					}

					xmlNode39.AppendChild(xmlNode41);
					XmlNode xmlNode42 = xmlDocument.CreateElement("cty_nm");
					xmlNode42.InnerText = address2.City;
					xmlNode39.AppendChild(xmlNode42);
					XmlNode xmlNode43 = xmlDocument.CreateElement("prov_cd");
					xmlNode43.InnerText = address2.State;
					xmlNode39.AppendChild(xmlNode43);
					XmlNode xmlNode44 = xmlDocument.CreateElement("cntry_cd");
					switch (address2?.CountryID)
					{
						case "CA":
							xmlNode44.InnerText = "CAN";
							break;

						case "US":
							xmlNode44.InnerText = "USA";
							break;

						default:
							xmlNode44.InnerText = "";
							break;
					}
					xmlNode39.AppendChild(xmlNode44);
					XmlNode xmlNode45 = xmlDocument.CreateElement("pstl_cd");
					xmlNode45.InnerText = address2.PostalCode;
					xmlNode39.AppendChild(xmlNode45);
					xmlNode27.AppendChild(xmlNode39);
					XmlNode xmlNode46 = xmlDocument.CreateElement("bn");
					xmlNode46.InnerText = item.TaxRegistrationID;
					xmlNode27.AppendChild(xmlNode46);
					XmlNode xmlNode47 = xmlDocument.CreateElement("sbctrcr_amt");
					decimal value = item.ReportType != T5018EFileRow.reportType.Canceled ? item.Amount.Value : previousRow.Amount.Value;
					xmlNode47.InnerText = Math.Round(value, 2).ToString();
					xmlNode27.AppendChild(xmlNode47);
					XmlNode xmlNode48 = xmlDocument.CreateElement("ptnrp_filr_id");
					switch (MasterView.Current.FilingType)
					{
						default:
							xmlNode48.InnerText = "";
							break;
					}

					xmlNode27.AppendChild(xmlNode48);
					XmlNode xmlNode49 = xmlDocument.CreateElement("rpt_tcd");
					xmlNode49.InnerText = item.ReportType;
					xmlNode27.AppendChild(xmlNode49);
					PXProcessing.SetProcessed();
				}

				XmlNode xmlNode50 = xmlDocument.CreateElement("T5018Summary");
				xmlNode26.AppendChild(xmlNode50);
				XmlNode xmlNode51 = xmlDocument.CreateElement("bn");
				xmlNode51.InnerText = MasterView.Current.ProgramNumber;
				xmlNode50.AppendChild(xmlNode51);
				XmlNode xmlNode52 = xmlDocument.CreateElement("PAYR_NM");
				XmlNode xmlNode53 = xmlDocument.CreateElement("l1_nm");
				xmlNode53.InnerText = text;
				xmlNode52.AppendChild(xmlNode53);
				XmlNode xmlNode54 = xmlDocument.CreateElement("l2_nm");
				xmlNode54.InnerText = text2;
				xmlNode52.AppendChild(xmlNode54);
				XmlNode xmlNode55 = xmlDocument.CreateElement("l3_nm");
				xmlNode55.InnerText = "";
				xmlNode52.AppendChild(xmlNode55);
				xmlNode50.AppendChild(xmlNode52);
				XmlNode xmlNode56 = xmlDocument.CreateElement("PAYR_ADDR");
				XmlNode xmlNode57 = xmlDocument.CreateElement("addr_l1_txt");
				xmlNode57.InnerText = MasterView.Current.AddressLine1;
				xmlNode56.AppendChild(xmlNode57);
				XmlNode xmlNode58 = xmlDocument.CreateElement("addr_l2_txt");
				xmlNode58.InnerText = MasterView.Current.AddressLine2;
				xmlNode56.AppendChild(xmlNode58);
				XmlNode xmlNode59 = xmlDocument.CreateElement("cty_nm");
				xmlNode59.InnerText = MasterView.Current.City;
				xmlNode56.AppendChild(xmlNode59);
				XmlNode xmlNode60 = xmlDocument.CreateElement("prov_cd");
				xmlNode60.InnerText = MasterView.Current.Province;
				xmlNode56.AppendChild(xmlNode60);
				XmlNode xmlNode61 = xmlDocument.CreateElement("cntry_cd");
				xmlNode61.InnerText = MasterView.Current.Country;
				xmlNode56.AppendChild(xmlNode61);
				XmlNode xmlNode62 = xmlDocument.CreateElement("pstl_cd");
				xmlNode62.InnerText = MasterView.Current.PostalCode;
				xmlNode56.AppendChild(xmlNode62);
				xmlNode50.AppendChild(xmlNode56);
				XmlNode xmlNode63 = xmlDocument.CreateElement("CNTC");
				XmlNode xmlNode64 = xmlDocument.CreateElement("cntc_nm");
				xmlNode64.InnerText = MasterView.Current.Name;
				xmlNode63.AppendChild(xmlNode64);
				XmlNode xmlNode65 = xmlDocument.CreateElement("cntc_area_cd");
				xmlNode65.InnerText = MasterView.Current.AreaCode;
				xmlNode63.AppendChild(xmlNode65);
				XmlNode xmlNode66 = xmlDocument.CreateElement("cntc_phn_nbr");
				xmlNode66.InnerText = MasterView.Current.Phone;
				xmlNode63.AppendChild(xmlNode66);
				XmlNode xmlNode67 = xmlDocument.CreateElement("cntc_extn_nbr");
				xmlNode67.InnerText = MasterView.Current.ExtensionNbr;
				xmlNode63.AppendChild(xmlNode67);
				xmlNode50.AppendChild(xmlNode63);
				XmlNode xmlNode68 = xmlDocument.CreateElement("PRD_END_DT");
				XmlNode xmlNode69 = xmlDocument.CreateElement("dy");

				DateTime dateTime = MasterView.Current.ToDate.Value;

				xmlNode69.InnerText = dateTime.Day.ToString();
				xmlNode68.AppendChild(xmlNode69);
				XmlNode xmlNode70 = xmlDocument.CreateElement("mo");
				xmlNode70.InnerText = dateTime.Month.ToString();
				xmlNode68.AppendChild(xmlNode70);
				XmlNode xmlNode71 = xmlDocument.CreateElement("yr");
				xmlNode71.InnerText = dateTime.Year.ToString();
				xmlNode68.AppendChild(xmlNode71);
				xmlNode50.AppendChild(xmlNode68);
				XmlNode xmlNode72 = xmlDocument.CreateElement("slp_cnt");
				xmlNode72.InnerText = slipcount.ToString();
				xmlNode50.AppendChild(xmlNode72);
				XmlNode xmlNode73 = xmlDocument.CreateElement("tot_sbctrcr_amt");

				xmlNode73.InnerText = Math.Round(subcontractorSumPayments.Value, 2).ToString();
				xmlNode50.AppendChild(xmlNode73);
				XmlNode xmlNode74 = xmlDocument.CreateElement("rpt_tcd");
				xmlNode74.InnerText = MasterView.Current.FilingType;
				xmlNode50.AppendChild(xmlNode74);
				xmlElement.AppendChild(xmlNode25);
				xmlDocument.AppendChild(xmlElement);
				string text9 = DateTime.Now.ToString("MMddyy");
				string reportType = ReportType.Key;
				string text10 = "T5018-" + organization.OrganizationCD + "-" + text9 + "-R" + MasterView.Current.Revision + "-";
				switch (reportType)
				{
					case T5018EFileRow.reportType.Amended:
						text10 += T5018Messages.Amended;
						break;

					case T5018EFileRow.reportType.Canceled:
						text10 += T5018Messages.Canceled;
						break;

					default:
					case T5018EFileRow.reportType.Original:
						text10 += T5018Messages.Original;
						break;
				}


				if (MasterView.Current.OrganizationID.HasValue && !string.IsNullOrEmpty(MasterView.Current.Year))
				{
					FileInfo fileInfo = new FileInfo(Guid.NewGuid(), text10 + ".xml", null,
													 Encoding.UTF8.GetBytes(xmlDocument.OuterXml));

					UploadFileMaintenance fileGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
					if (fileGraph.SaveFile(fileInfo, FileExistsAction.CreateVersion))
					{
						PXNoteAttribute.SetFileNotes(MasterView.Cache, MasterView.Current, fileInfo.UID.Value);
					}
				}
			}
		}

		public PXAction<UPCompany> ViewDocument;
		[PXButton]
		[PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected IEnumerable viewDocument(PXAdapter adapter)
		{
			if (this.DetailsView.Current != null)
			{
				T5018EFileRow eFileRow = DetailsView.Current;

				Organization organization = SelectFrom<Organization>.Where<Organization.bAccountID.IsEqual<Organization.bAccountID>>.View.Select(this, eFileRow.BAccountID);

				if (eFileRow != null && organization != null)
				{
					throw new PXRedirectByScreenIDException("AP407600", PXBaseRedirectException.WindowMode.New, true, new { AcctCD = organization.OrganizationCD.TrimEnd(), Year = eFileRow.Year, Revision = eFileRow.Revision, VAcctCD = eFileRow.VAcctCD.TrimEnd() });
				}
			}

			return adapter.Get();
		}

		private void Prepare(T5018MasterTable table, bool amendment = false)
		{
			using (var ts = new PXTransactionScope())
			{
				List<int> branches = new List<int>();
				if (OrganizationMaint.FindOrganizationByID(this, table.OrganizationID).OrganizationType == OrganizationTypes.WithoutBranches)
					branches.Add(PXAccess.GetBranchByBAccountID(PXAccess.GetOrganizationBAccountID(table.OrganizationID)).BranchID);
				else
					branches.AddRange(PXAccess.GetChildBranchIDs(table.OrganizationID));

				PXResultset<T5018VendorTransaction> T5018VendorTransactions = new PXResultset<T5018VendorTransaction>();


				foreach (int branchID in branches)
				{
					T5018VendorTransactions.AddRange(
						SelectFrom<T5018VendorTransaction>.
							Where<T5018VendorTransaction.branchID.IsEqual<T5018VendorTransaction.branchID.AsOptional>.
							And<T5018VendorTransaction.docDate.IsGreaterEqual<T5018VendorTransaction.docDate.AsOptional>>.
							And<T5018VendorTransaction.docDate.IsLessEqual<T5018VendorTransaction.docDate.AsOptional>>>.
							OrderBy<
								T5018VendorTransaction.branchID.Asc,
								T5018VendorTransaction.vendorID.Asc,
								T5018VendorTransaction.docType.Asc,
								T5018VendorTransaction.refNbr.Asc>.
							View.Select(this, branchID, table.FromDate, table.ToDate));
				}

				var previousRevision =
						SelectFrom<T5018MasterTable>.
						Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
						And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.AsOptional>>.
						And<T5018MasterTable.createdDateTime.IsLess<T5018MasterTable.createdDateTime.AsOptional>>>.
						OrderBy<T5018MasterTable.createdDateTime.Desc>.View.ReadOnly.Select(this, table.OrganizationID, table.Year, table.CreatedDateTime).TopFirst;

				List<T5018VendorTransaction> docTransactions = new List<T5018VendorTransaction>();
				List<T5018EFileRow> t5018EFileRows = new List<T5018EFileRow>();
				var AmountToReport = new Dictionary<int, decimal>();
				var ServiceAmount = new Dictionary<int, decimal>();
				string firstDocumentLineKey = string.Empty;
				string firstDocumentLineType = string.Empty;
				int count = 0;
				foreach (T5018VendorTransaction T5018Trans in T5018VendorTransactions)
				{
					count++;

					if (firstDocumentLineKey != T5018Trans.RefNbr || firstDocumentLineType != T5018Trans.DocType)
					{
						if (string.IsNullOrEmpty(firstDocumentLineKey) && string.IsNullOrEmpty(firstDocumentLineType)) // the first transaction
						{
							docTransactions.Add(T5018Trans);
						}
						else
						{
							AggregateDocument(docTransactions, t5018EFileRows, AmountToReport, ServiceAmount, table);
							docTransactions.Clear();
							docTransactions.Add(T5018Trans);
						}
						firstDocumentLineKey = T5018Trans.RefNbr;
						firstDocumentLineType = T5018Trans.DocType;

					}
					else
						docTransactions.Add(T5018Trans);

					if (count == T5018VendorTransactions.Count) // the last transaction
					{
						AggregateDocument(docTransactions, t5018EFileRows, AmountToReport, ServiceAmount, table);
						docTransactions.Clear();
					}
				}

				if (t5018EFileRows.Count == 0)
				{
					// to cancel all rows from the previous submission we have to insert records into t5018EFileRows with Amount = 0
					var previousSubmissionRows =
						SelectFrom<T5018EFileRow>.
						Where<T5018EFileRow.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
						And<T5018EFileRow.year.IsEqual<T5018MasterTable.year.AsOptional>>.
						And<T5018EFileRow.revision.IsEqual<T5018MasterTable.revision.AsOptional>>>.View.Select(this, table.OrganizationID, table.Year, previousRevision?.Revision);

					foreach (T5018EFileRow r in previousSubmissionRows)
					{
						t5018EFileRows.Add(new T5018EFileRow
						{
							OrganizationID = table.OrganizationID,
							Amount = 0,
							BAccountID = r.BAccountID,
							Year = table.Year,
							Revision = table.Revision,
							VendorName = r.VendorName.Length <= 60 ? r.VendorName : r.VendorName.Substring(0,60), // BAccount Name has a 255 character max length, but VendorName is limited to 60 due to breaking changes
							OrganizationName = OrganizationMaint.FindOrganizationByID(this, table.OrganizationID).OrganizationName,
							VAcctCD = r.VAcctCD,
							TaxRegistrationID = r.TaxRegistrationID,
							TotalServiceAmount = 0
						});
					}
				}

				foreach (var row in t5018EFileRows)
				{
					T5018EFileRow previousSubmissionRow =
						SelectFrom<T5018EFileRow>.
						Where<T5018EFileRow.organizationID.IsEqual<T5018MasterTable.organizationID.AsOptional>.
						And<T5018EFileRow.year.IsEqual<T5018MasterTable.year.AsOptional>>.
						And<T5018EFileRow.revision.IsEqual<T5018MasterTable.revision.AsOptional>>.
					And<T5018EFileRow.bAccountID.IsEqual<T5018EFileRow.bAccountID.AsOptional>>>.View.Select(this, table.OrganizationID, table.Year, previousRevision?.Revision, row.BAccountID);

					decimal amount = AmountToReport.Count > 0 ? AmountToReport.First(x => x.Key == row.BAccountID).Value : 0;
					decimal service = ServiceAmount.Count > 0 ? ServiceAmount.First(x => x.Key == row.BAccountID).Value : 0;

					row.Amount = amount;
					row.TotalServiceAmount = service;
					row.AmendmentRow = previousSubmissionRow == null ||
						amount != previousSubmissionRow.Amount;

					if (!amendment)
					{
						row.ReportType = T5018EFileRow.reportType.Original;
						row.AmendmentRow = false;
					}
					else
					{
						if (previousSubmissionRow != null && previousSubmissionRow.TotalServiceAmount >= previousRevision.ThresholdAmount && row.TotalServiceAmount < table.ThresholdAmount && previousSubmissionRow.ReportType != T5018EFileRow.reportType.Canceled)
						{
							row.ReportType = T5018EFileRow.reportType.Canceled;
							row.AmendmentRow = true;
						}
						else
							row.ReportType = T5018EFileRow.reportType.Amended;

					}

					T5018EFileRow newRecord = (T5018EFileRow)this.Caches[nameof(T5018EFileRow)].Insert(row);
				}

				if (!t5018EFileRows.Any(row => row.ReportType == T5018EFileRow.reportType.Canceled) &&
					!t5018EFileRows.Where(row => row.TotalServiceAmount >= table.ThresholdAmount).Any(row => row.ReportType == T5018EFileRow.reportType.Original) &&
					!t5018EFileRows.Where(row => row.TotalServiceAmount >= table.ThresholdAmount).Any(row => row.AmendmentRow ?? false))
				{
					this.Caches[nameof(T5018MasterTable)].Delete(table);
					this.Save.Press();
					ts.Complete();
					Dictionary<string, T5018MasterTable> dict = PXDatabase.GetSlot<Dictionary<string, T5018MasterTable>>(THRESHOLD_UNMET, typeof(T5018EFileRow));
					lock (((ICollection)dict).SyncRoot)
					{
						dict[THRESHOLD_UNMET] = table;
					}
					throw new PXException(T5018Messages.NoNewRows);
				}

				this.Save.Press();

				foreach (PXResult<T5018Transactions, APPayment, APAdjust, APTran> record in
					SelectFrom<T5018Transactions>.
					InnerJoin<APPayment>.On<APPayment.docType.IsEqual<T5018Transactions.docType>.
						And<APPayment.refNbr.IsEqual<T5018Transactions.refNbr>>>.
					LeftJoin<APAdjust>.
						On<APAdjust.adjgDocType.IsEqual<T5018Transactions.docType>.
							And<APAdjust.adjgRefNbr.IsEqual<T5018Transactions.refNbr>>>.
					LeftJoin<APTran>.
						On<APTran.tranType.IsEqual<APAdjust.adjdDocType>.
							And<APTran.refNbr.IsEqual<APAdjust.adjdRefNbr>>.
							And<Brackets<APTran.lineNbr.IsEqual<APAdjust.adjdLineNbr>.
								Or<APAdjust.adjdLineNbr.IsEqual<Zero>>>>>.
						InnerJoin<T5018EFileRow>.
						On<T5018Transactions.vendorID.IsEqual<T5018EFileRow.bAccountID>>.
					Where<T5018Transactions.branchID.IsIn<Data.BQL.@P.AsInt>.
						///Excluding voided documents, as they are excluded from calculation of the revision
						And<APPayment.voided.IsEqual<False>>.
						And<Brackets<APAdjust.released.IsEqual<True>>.Or<APAdjust.released.IsNull>>.
						//Only prepayments with "Include" checkbox or application to services are included
						And<Brackets<Brackets<T5018Transactions.docType.IsEqual<APDocType.prepayment>.
								And<Brackets<APTranExt.t5018Service.IsEqual<True>.
									Or<APPaymentExt.includeInT5018Report.IsEqual<True>>>>.
							Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.check>.
								And<APAdjust.adjdDocType.IsEqual<APDocType.invoice>.
								And<APTranExt.t5018Service.IsEqual<True>>>>>.
							Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.check>.
								And<APAdjust.adjdDocType.IsEqual<APDocType.prepayment>>>>.
							Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.quickCheck>>.
								And<APTranExt.t5018Service.IsEqual<True>>>>>>.
						And<T5018Transactions.docDate.IsGreaterEqual<T5018VendorTransaction.adjdDocDate.AsOptional>>.
						And<T5018Transactions.docDate.IsLessEqual<T5018VendorTransaction.adjdDocDate.AsOptional>>.
						And<T5018EFileRow.organizationID.IsEqual<T5018EFileRow.organizationID.AsOptional>>.
						And<T5018EFileRow.year.IsEqual<T5018EFileRow.year.AsOptional>>.
						And<T5018EFileRow.revision.IsEqual<T5018EFileRow.revision.AsOptional>>>.
						View.Select(this, branches, table.FromDate, table.ToDate, table.OrganizationID, table.Year, table.Revision))
				{
					APPayment payment = (APPayment)record;
					APAdjust adjustment = (APAdjust)record;
					APTran tran = (APTran)record;

					APAdjustEFileRevision newTr = new APAdjustEFileRevision
					{
						AdjgDocType = payment.DocType,
						AdjgRefNbr = payment.RefNbr,
						AdjdLineNbr = adjustment.AdjdLineNbr ?? -1,
						AdjdDocType = adjustment.AdjdDocType == null ? string.Empty : adjustment.AdjdDocType,
						AdjdRefNbr = adjustment.AdjdRefNbr == null ? string.Empty : adjustment.AdjdRefNbr,
						AdjNbr = adjustment.AdjNbr == null ? -1 : adjustment.AdjNbr,
						OrganizationID = table.OrganizationID,
						Year = table.Year,
						Revision = table.Revision,
						T5018Service = tran.GetExtension<APTranExt>()?.T5018Service ?? false,
						IncludeInReport = payment?.GetExtension<APPaymentExt>()?.IncludeInT5018Report ?? false,
						Voided = adjustment.Voided ?? false
					};

					if (adjustment.AdjgDocType == APDocType.Check && adjustment.AdjdDocType == APDocType.Prepayment)
					{
						APPayment paymentPPR =
							SelectFrom<APPayment>.
								Where<APPayment.refNbr.IsEqual<APAdjust.adjdRefNbr.AsOptional>.
								And<APPayment.docType.IsEqual<APAdjust.adjdDocType.AsOptional>>>.
							View.Select(this, adjustment.AdjdRefNbr, adjustment.AdjdDocType);

						if (paymentPPR != null)
						{
							APPaymentExt ext = paymentPPR.GetExtension<APPaymentExt>();

							if (ext.IncludeInT5018Report.HasValue && ext.IncludeInT5018Report.Value)
							{
								newTr.IncludeInReport = true;
							}
						}
					}
					this.Transactions.Insert(newTr);
				}

				this.Save.Press();
				ts.Complete();
			}
		}

		protected void AggregateDocument(List<T5018VendorTransaction> docTransactions, List<T5018EFileRow> t5018EFileRows, Dictionary<int, decimal> AmountToReport, Dictionary<int, decimal> ServiceAmount, T5018MasterTable table)
		{
			decimal amountToReport = 0;
			decimal serviceAmount = 0;
			int docVendorId = docTransactions.First().BAccountID.Value;
			T5018VendorTransaction FirstTran = docTransactions.First();
			BAccount FirstTranAccount = VendorMaint.GetByID(this, FirstTran.BAccountID);

			string taxRegistrationID;
			if (FirstTranAccount.GetExtension<T5018VendorExt>().BoxT5018 == T5018VendorExt.boxT5018.Individual)
				taxRegistrationID = FirstTranAccount.GetExtension<T5018VendorExt>().SocialInsNum;
			else
				taxRegistrationID = FirstTranAccount.GetExtension<T5018VendorExt>().BusinessNumber;

			//Assumption - only Checks, Cash Payments and Prepayments are in the list (should be filtered on release)
			var prepaymentRequests = docTransactions.Where(tr => tr.AdjdDocType == APDocType.Prepayment).ToList();

			decimal pprAmount = 0;

			string ppmRefNmr = String.Empty;
			foreach (T5018VendorTransaction tran in prepaymentRequests)
			{
				//Prepayment Request may have more than one APTran
				if (!String.Equals(ppmRefNmr, tran.AdjdRefNbr))
				{
					pprAmount += (decimal)tran.CuryAdjgAmt;
				}
				ppmRefNmr = tran.AdjdRefNbr;
			}

			switch (FirstTran.DocType)
			{
				case APDocType.Check:
				case APDocType.QuickCheck:
					amountToReport = CalculatePaymentsAmountToReport(docTransactions, pprAmount);
					serviceAmount = SumServiceAmount(docTransactions);
					break;

				case APDocType.Prepayment:
					amountToReport = CalculatePrepaymentsAmountToReport(docTransactions, pprAmount);
					serviceAmount = SumServiceAmount(docTransactions);
					break;
			}

			if (!AmountToReport.ContainsKey(docVendorId))
			{
				t5018EFileRows.Add(new T5018EFileRow
				{
					OrganizationID = table.OrganizationID,
					Amount = 0,
					BAccountID = docVendorId,
					Year = table.Year,
					Revision = table.Revision,
					VendorName = FirstTran.AcctName,
					OrganizationName = OrganizationMaint.FindOrganizationByID(this, table.OrganizationID).OrganizationName,
					VAcctCD = FirstTran.AcctCD,
					TaxRegistrationID = taxRegistrationID,
					AmendmentRow = false,
					TotalServiceAmount = 0
				});

				AmountToReport.Add(docVendorId, amountToReport);
				ServiceAmount.Add(docVendorId, serviceAmount);
			}
			else
			{
				AmountToReport[docVendorId] += amountToReport;
				ServiceAmount[docVendorId] += serviceAmount;
			}
		}

		protected decimal CalculatePaymentsAmountToReport(List<T5018VendorTransaction> t5018Trans, decimal pprAmount)
		{
			if (t5018Trans.Any(a => a.T5018Service == true && !a.Voided.Value && !a.APAdjustVoided.Value))
				return (decimal)t5018Trans.First().CuryOrigDocAmt - pprAmount;
			else return 0;
		}

		protected decimal CalculatePrepaymentsAmountToReport(List<T5018VendorTransaction> t5018Trans, decimal pprAmount)
		{
			bool anyApplications = t5018Trans.Any(a => a.APAdjustReleased == true);
			bool hasServicePart = t5018Trans.Any(a => a.T5018Service == true);
			bool appliedInFull = t5018Trans.First().CuryDocBal.HasValue && (decimal)t5018Trans.First().CuryDocBal == 0 && t5018Trans.First().CuryOrigDocAmt > 0;
			bool includeInT5018Report = t5018Trans.First().IncludeInT5018Report == true;
			bool notVoided = !(t5018Trans.First().Voided ?? false);
			decimal docAmount = (decimal)t5018Trans.First().CuryOrigDocAmt - pprAmount;

			if (notVoided)
			{
				if (appliedInFull)
				{
					if (hasServicePart) return docAmount;
				}
				else
				{
					if (includeInT5018Report) return docAmount;
					else
					{
						if (anyApplications && hasServicePart) return docAmount;
					}
				}
			}

			return 0;
		}

		protected decimal SumServiceAmount(List<T5018VendorTransaction> t5018Trans)
		{
			decimal serviceAmount = 0;
			T5018VendorTransaction firstDocLine = t5018Trans.First();
			// If the doc is a prepayment
			if (firstDocLine.DocType == APDocType.Prepayment && (firstDocLine.IncludeInT5018Report ?? false))

				if ((firstDocLine.Voided ?? false))
				{
					serviceAmount += 0;
				}
				else
				{
					//Workaround - sometimes firstDocLine.CuryDocBal is null. Probably error in platform.
					if (firstDocLine.CuryDocBal.HasValue)
					{
						serviceAmount += firstDocLine.CuryDocBal.Value;
					}
					else
					{
						APPayment pmtDoc = SelectFrom<APPayment>
							.Where<APPayment.docType.IsEqual<Data.BQL.@P.AsString>
								.And<APPayment.refNbr.IsEqual<Data.BQL.@P.AsString>>>
							.View.Select(this, firstDocLine.DocType, firstDocLine.RefNbr);

						serviceAmount += pmtDoc.CuryDocBal.Value;
					}
				}


			// If the doc is PayByLine
			if (firstDocLine.AdjdDocType == APDocType.Invoice && firstDocLine.AdjdLineNbr > 0)
			{
				foreach (T5018VendorTransaction tran in t5018Trans)
				{
					// If document has no applications
					if (!tran.T5018Service.HasValue) continue;

					// If the Doc is a retainage bill
					if (tran.OrigRefNbr != null)
					{
						APTran origTran =
							SelectFrom<APTran>.
								Where<APTran.refNbr.IsEqual<APRetainageInvoice.origRefNbr.AsOptional>.
								And<APTran.tranType.IsEqual<APRetainageInvoice.origDocType.AsOptional>>.
								And<APTran.lineNbr.IsEqual<APAdjust.adjdLineNbr.AsOptional>>>.
							View.Select(this, tran.OrigRefNbr, tran.OrigDocType, tran.AdjdLineNbr);

						APTranExt tranExt = origTran.GetExtension<APTranExt>();

						if (!tranExt.T5018Service.HasValue || !tranExt.T5018Service.Value) continue;
					}
					else if (!tran.T5018Service.HasValue || !tran.T5018Service.Value) continue;

					serviceAmount += tran.Voided.Value || tran.APAdjustVoided.Value ? 0 : tran.CuryAdjgAmt.Value;
				}

				return serviceAmount;
			}
			// If the Doc is a retainage bill
			else if (firstDocLine.OrigRefNbr != null)
			{
				var OrigDocTrans =
					SelectFrom<APTran>.
						Where<APTran.refNbr.IsEqual<APRetainageInvoice.origRefNbr.AsOptional>.
						And<APTran.tranType.IsEqual<APRetainageInvoice.origDocType.AsOptional>>>.
					View.Select(this, firstDocLine.OrigRefNbr, firstDocLine.OrigDocType);

				foreach (APTran tran in OrigDocTrans)
				{
					APTranExt tranExt = tran.GetExtension<APTranExt>();
					if (tranExt != null && tranExt.T5018Service.HasValue && tranExt.T5018Service.Value)
					{
						serviceAmount += firstDocLine.Voided.Value || firstDocLine.APAdjustVoided.Value ? 0 : (tran.RetainageAmt.Value / firstDocLine.InvoiceTotal.Value) * firstDocLine.CuryAdjgAmt.Value;
					}
				}
				return serviceAmount;
			}
			// Doc is not pay by line, and isn't a retainage bill
			else
			{
				decimal docDiscount = 0;
				if (firstDocLine.CuryDiscTot.HasValue && firstDocLine.CuryDiscTot > 0)
					docDiscount = (decimal)firstDocLine.CuryDiscTot;

				// Get the final total tax amout(s) at the document level
				PXResultset<APTaxTran> docLevelTaxes = new PXResultset<APTaxTran>();

				if (firstDocLine.AdjdRefNbr != null && firstDocLine.AdjdDocType != null)
				{
					docLevelTaxes = SelectFrom<APTaxTran>.
						Where<APTaxTran.refNbr.IsEqual<APTran.refNbr.AsOptional>.
							And<APTaxTran.tranType.IsEqual<APTran.tranType.AsOptional>>>.
						View.Select(this, firstDocLine.AdjdRefNbr, firstDocLine.AdjdDocType);
				}

				foreach (T5018VendorTransaction tran in t5018Trans)
				{
					//Not calculating Service Amount on applications to prepayment requests
					if (tran.AdjdDocType == APDocType.Prepayment) continue;

					// If document has no applications
					if (!tran.T5018Service.HasValue) continue;

					// skip non T5018Service lines
					if (!tran.T5018Service.HasValue || !tran.T5018Service.Value) continue;

					// skip the discount line
					if (tran.LineType == Objects.SO.SOLineType.Discount)
						continue;

					// If the bill is a parent bill for a retainage one
					// the taxes must be calculated based on the full line amount
					// instead of the transaction amount
					decimal origLineAmt = 0;

					if (firstDocLine.RetainageApply.HasValue && firstDocLine.RetainageApply.Value.Equals(true))
						origLineAmt = (decimal)tran.CuryLineAmt;
					else
						origLineAmt = (decimal)tran.CuryTranAmt;

					if (!tran.CuryTranAmt.HasValue || !tran.InvoiceTotal.HasValue || !tran.CuryAdjgAmt.HasValue)
						serviceAmount += 0;
					else
					{
						decimal lineAmt = 0;
						decimal taxAmt = 0;
						// is the line taxable?
						if (tran.CuryTaxableAmt != null && docLevelTaxes.Count > 0)
						{
							TaxTran tax = docLevelTaxes.First();
							// calculate line amount with discount
							lineAmt = (decimal)(origLineAmt * tax.CuryTaxableAmt / (tax.CuryTaxableAmt + docDiscount));
							// aggregate all taxes for the current line
							foreach (TaxTran taxTran in docLevelTaxes)
								taxAmt += PXCurrencyAttribute.BaseRound(this, lineAmt * taxTran.CuryTaxAmt / taxTran.CuryTaxableAmt);
						}
						else
						{
							// calculate line amount with discount
							lineAmt = (decimal)(origLineAmt * tran.InvoiceTotal / (tran.InvoiceTotal + docDiscount));
						}

						serviceAmount += tran.Voided.Value || tran.APAdjustVoided.Value ? 0 : PXCurrencyAttribute.BaseRound(this, (lineAmt + taxAmt) / tran.InvoiceTotal * tran.CuryAdjgAmt);
					}
				}

				return serviceAmount;
			}
		}

		[PXButton]
		[PXUIField(DisplayName = "Prepare Report")]
		public virtual IEnumerable prepareOriginal(PXAdapter adapter)
		{
			List<int> branches = new List<int>();
			if (OrganizationMaint.FindOrganizationByID(this, MasterView.Current.OrganizationID).OrganizationType == OrganizationTypes.WithoutBranches)
				branches.Add(PXAccess.GetBranchByBAccountID(PXAccess.GetOrganizationBAccountID(MasterView.Current.OrganizationID)).BranchID);
			else
				branches.AddRange(PXAccess.GetChildBranchIDs(MasterView.Current.OrganizationID));

			T5018Transactions NewTransactions = null;
			foreach (int branchID in branches)
			{
				if (NewTransactions == null)
				{
					NewTransactions = SelectFrom<T5018Transactions>.
						InnerJoin<BAccountR>.
						On<BAccountR.bAccountID.IsEqual<T5018Transactions.vendorID>>.
					InnerJoin<APPayment>.On<APPayment.docType.IsEqual<T5018Transactions.docType>.
						And<APPayment.refNbr.IsEqual<T5018Transactions.refNbr>>>.
					LeftJoin<APAdjust>.
						On<APAdjust.adjgDocType.IsEqual<T5018Transactions.docType>.
							And<APAdjust.adjgRefNbr.IsEqual<T5018Transactions.refNbr>>>.
					LeftJoin<APTran>.
						On<APTran.tranType.IsEqual<APAdjust.adjdDocType>.
							And<APTran.refNbr.IsEqual<APAdjust.adjdRefNbr>>.
							And<Brackets<APTran.lineNbr.IsEqual<APAdjust.adjdLineNbr>.
								Or<APAdjust.adjdLineNbr.IsEqual<Zero>>>>>.
					Where<T5018Transactions.branchID.IsEqual<Branch.branchID.AsOptional>.
							And<T5018VendorRExt.vendorT5018.IsEqual<True>>.
						And<APPayment.voided.IsEqual<False>>.
						And<Brackets<APAdjust.released.IsEqual<True>>.Or<APAdjust.released.IsNull>>.
						And<Brackets<APAdjust.voided.IsEqual<False>>.Or<APAdjust.voided.IsNull>>.
						And<Brackets<Brackets<T5018Transactions.docType.IsEqual<APDocType.prepayment>.
							And<Brackets<APAdjust.adjdDocType.IsNotEqual<APDocType.prepayment>.
								Or<Brackets<APAdjust.adjdDocType.IsNull.And<APPaymentExt.includeInT5018Report.IsEqual<True>>>>>>>.
						Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.check>.
							And<APAdjust.adjdDocType.IsEqual<APDocType.invoice>.
							And<APTranExt.t5018Service.IsEqual<True>>>>>.
						Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.check>.
							And<APAdjust.adjdDocType.IsEqual<APDocType.prepayment>>>>.
						Or<Brackets<T5018Transactions.docType.IsEqual<APDocType.quickCheck>>.
							And<APTranExt.t5018Service.IsEqual<True>>>>>.
						And<T5018Transactions.docDate.IsGreaterEqual<T5018MasterTable.fromDate.AsOptional>>.
						And<T5018Transactions.docDate.IsLessEqual<T5018MasterTable.toDate.AsOptional>>>.
						View.Select(this, branchID, MasterView.Current.FromDate, MasterView.Current.ToDate).TopFirst;
				}
			}

			if (NewTransactions == null)
				throw new PXException(T5018Messages.NoNewRows);

			Save.Press();

			T5018MasterTable table = MasterView.Current;

			PXLongOperation.StartOperation(this, delegate
			{
				T5018Fileprocessing graph = CreateInstance<T5018Fileprocessing>();

				graph.Prepare(table);
			});

			yield return MasterView.Current;
		}

		[PXButton]
		[PXUIField(DisplayName = "Amend Report")]
		public virtual IEnumerable prepareAmendment(PXAdapter adapter)
		{
			Save.Press();
			T5018MasterTable preRevision = SelectFrom<T5018MasterTable>.
				Where<T5018MasterTable.organizationID.IsEqual<T5018MasterTable.organizationID.FromCurrent>.
				And<T5018MasterTable.year.IsEqual<T5018MasterTable.year.FromCurrent>>>.
				OrderBy<T5018MasterTable.createdDateTime.Desc>.View.ReadOnly.Select(this).TopFirst;

			if (preRevision == null || !preRevision.RevisionSubmitted.HasValue || !preRevision.RevisionSubmitted.Value)
				throw new PXException(T5018Messages.NoPreviousSubmissions);

			List<int> branches = new List<int>();
			if (OrganizationMaint.FindOrganizationByID(this, MasterView.Current.OrganizationID).OrganizationType == OrganizationTypes.WithoutBranches)
				branches.Add(PXAccess.GetBranchByBAccountID(PXAccess.GetOrganizationBAccountID(MasterView.Current.OrganizationID)).BranchID);
			else
				branches.AddRange(PXAccess.GetChildBranchIDs(MasterView.Current.OrganizationID));

			T5018Transactions NewTransactions = null;
			foreach (int branchID in branches)
			{
				if (NewTransactions == null)
				{
					NewTransactions = SelectFrom<T5018Transactions>.
					InnerJoin<BAccountR>.
						On<T5018Transactions.vendorID.IsEqual<BAccountR.bAccountID>>.
					InnerJoin<APPayment>.On<APPayment.docType.IsEqual<T5018Transactions.docType>.
						And<APPayment.refNbr.IsEqual<T5018Transactions.refNbr>>>.
					LeftJoin<APAdjust>.
						On<APAdjust.adjgDocType.IsEqual<T5018Transactions.docType>.
							And<APAdjust.adjgRefNbr.IsEqual<T5018Transactions.refNbr>>>.
					LeftJoin<APTran>.
						On<APTran.tranType.IsEqual<APAdjust.adjdDocType>.
							And<APTran.refNbr.IsEqual<APAdjust.adjdRefNbr>>.
							And<Brackets<APTran.lineNbr.IsEqual<APAdjust.adjdLineNbr>.
								Or<APAdjust.adjdLineNbr.IsEqual<Zero>>>>>.
					LeftJoin<APAdjustEFileRevision>.
						On<APAdjustEFileRevision.adjgDocType.IsEqual<T5018Transactions.docType>.
							And<APAdjustEFileRevision.adjgRefNbr.IsEqual<T5018Transactions.refNbr>>.
							And<APAdjustEFileRevision.includeInReport.IsEqual<APPaymentExt.includeInT5018Report>>.
							And<Brackets<APTranExt.t5018Service.IsNull>.
								Or<APAdjustEFileRevision.t5018Service.IsEqual<APTranExt.t5018Service>>>.
							And<APAdjustEFileRevision.adjNbr.IsEqual<APAdjust.adjNbr>>.
							And<APAdjustEFileRevision.voided.IsEqual<APAdjust.voided>>>.
					Where<T5018Transactions.branchID.IsEqual<Branch.branchID.AsOptional>.
						And<T5018VendorRExt.vendorT5018.IsEqual<True>>.
						And<Brackets<APAdjust.released.IsEqual<True>>.Or<APAdjust.released.IsNull>>.
						And<APAdjustEFileRevision.organizationID.IsNull>.
						And<T5018Transactions.docDate.IsGreaterEqual<APAdjust.adjdDocDate.AsOptional>>.
						And<T5018Transactions.docDate.IsLessEqual<APAdjust.adjdDocDate.AsOptional>>>.
					View.ReadOnly.Select(this, branchID, MasterView.Current.FromDate, MasterView.Current.ToDate).TopFirst;
				}
			}

			if (NewTransactions == null)
				throw new PXException(T5018Messages.NoNewRows);

			T5018MasterTable table = (T5018MasterTable)MasterView.Cache.Insert(new T5018MasterTable()
			{
				OrganizationID = MasterView.Current.OrganizationID.Value,
				Year = MasterView.Current.Year,
				Revision = T5018Messages.NewValue,
				ProgramNumber = MasterView.Current.ProgramNumber,
				FilingType = T5018MasterTable.filingType.Amendment
			});

			if (table == null)
				table = MasterView.Cache.Inserted.FirstOrDefault_() as T5018MasterTable;

			MasterView.Current = table;
			Save.Press();

			PXLongOperation.StartOperation(this, delegate
			{
				T5018Fileprocessing graph = CreateInstance<T5018Fileprocessing>();
				graph.Prepare(table, true);
			});

			yield return MasterView.Current;
		}
	}
}
