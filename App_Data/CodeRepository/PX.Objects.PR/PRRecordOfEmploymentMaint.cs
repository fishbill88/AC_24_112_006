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

using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.GL.DAC;
using PX.SM;
using System;
using System.Collections;
using System.Linq;
using System.Xml;

namespace PX.Objects.PR
{
	public class PRRecordOfEmploymentMaint : PXGraph<PRRecordOfEmploymentMaint, PRRecordOfEmployment>
	{
		public SelectFrom<PRRecordOfEmployment>.View Document;
		public SelectFrom<PRRecordOfEmployment>
			.Where<PRRecordOfEmployment.refNbr.IsEqual<PRRecordOfEmployment.refNbr.FromCurrent>>.View CurrentDocument;
		public SelectFrom<Address>.Where<Address.addressID.IsEqual<PRRecordOfEmployment.addressID.FromCurrent>>.View Address;

		public PXSetup<PRSetup> Preferences;

		public SelectFrom<PRROEStatutoryHolidayPay>
			.Where<PRROEStatutoryHolidayPay.refNbr.IsEqual<PRRecordOfEmployment.refNbr.AsOptional>>
			.View StatutoryHolidays;

		public SelectFrom<PRROEOtherMonies>
			.Where<PRROEOtherMonies.refNbr.IsEqual<PRRecordOfEmployment.refNbr.AsOptional>>
			.View OtherMonies;

		public SelectFrom<PRROEInsurableEarningsByPayPeriod>
			.Where<PRROEInsurableEarningsByPayPeriod.refNbr.IsEqual<PRRecordOfEmployment.refNbr.AsOptional>>
			.View InsurableEarnings;

		#region Actions
		public PXAction<PRRecordOfEmployment> Export;
		[PXUIField(DisplayName = "Export", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable export(PXAdapter adapter)
		{
			PXLongOperation.StartOperation(this, delegate ()
			{
				GenerateXmlFile();
			});
			return adapter.Get();
		}

		public PXAction<PRRecordOfEmployment> Reopen;
		[PXUIField(DisplayName = "Reopen", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable reopen(PXAdapter adapter)
		{
			return adapter.Get();
		}

		public PXAction<PRRecordOfEmployment> MarkAsSubmitted;
		[PXUIField(DisplayName = "Mark as Submitted", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable markAsSubmitted(PXAdapter adapter)
		{
			PXLongOperation.StartOperation(this, delegate ()
			{
				string roeUri = "https://www.canada.ca/en/employment-social-development/programs/ei/ei-list/ei-roe/access-roe.html";
				throw new PXRedirectToUrlException(roeUri, PXBaseRedirectException.WindowMode.New, true, "Redirect:" + roeUri);
			});

			return adapter.Get();
		}

		public PXAction<PRRecordOfEmployment> Amend;
		[PXUIField(DisplayName = "Amend", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable amend(PXAdapter adapter)
		{
			PXLongOperation.StartOperation(this, delegate ()
			{
				PRRecordOfEmploymentMaint roeGraph = PXGraph.CreateInstance<PRRecordOfEmploymentMaint>();

				PRRecordOfEmployment originalROE = Document.Current;

				PRRecordOfEmployment amendmentROE = PXCache<PRRecordOfEmployment>.CreateCopy(originalROE);
				amendmentROE.Amendment = true;
				amendmentROE.RefNbr = null;
				amendmentROE.AmendedRefNbr = originalROE.RefNbr;
				amendmentROE.NoteID = Guid.NewGuid();
				amendmentROE = roeGraph.Document.Insert(amendmentROE);
				roeGraph.Actions.PressSave();

				foreach (PRROEInsurableEarningsByPayPeriod originalRecord in roeGraph.InsurableEarnings.Select(originalROE.RefNbr))
				{
					PRROEInsurableEarningsByPayPeriod newRecord = PXCache<PRROEInsurableEarningsByPayPeriod>.CreateCopy(originalRecord);
					newRecord.RefNbr = amendmentROE.RefNbr;
					roeGraph.InsurableEarnings.Insert(newRecord);
				}

				foreach (PRROEStatutoryHolidayPay originalRecord in roeGraph.StatutoryHolidays.Select(originalROE.RefNbr))
				{
					PRROEStatutoryHolidayPay newRecord = PXCache<PRROEStatutoryHolidayPay>.CreateCopy(originalRecord);
					newRecord.RefNbr = amendmentROE.RefNbr;
					roeGraph.StatutoryHolidays.Insert(newRecord);
				}

				foreach (PRROEOtherMonies originalRecord in roeGraph.OtherMonies.Select(originalROE.RefNbr))
				{
					PRROEOtherMonies newRecord = PXCache<PRROEOtherMonies>.CreateCopy(originalRecord);
					newRecord.RefNbr = amendmentROE.RefNbr;
					roeGraph.OtherMonies.Insert(newRecord);
				}

				roeGraph.Actions.PressSave();
				Document.Current = PRRecordOfEmployment.PK.Find(roeGraph, amendmentROE.AmendedRefNbr);
			});
			return adapter.Get();
		}

		public PXAction<PRRecordOfEmployment> ShowFinalPaycheck;
		[PXUIField(DisplayName = "Show Final Paycheck", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		protected virtual void showFinalPaycheck()
		{
			PRRecordOfEmployment recordOfEmployment = Document.Current;
			if (!string.IsNullOrWhiteSpace(recordOfEmployment.OrigDocType) && !string.IsNullOrWhiteSpace(recordOfEmployment.OrigRefNbr))
			{
				PRPayment payment = PRPayment.PK.Find(this, recordOfEmployment.OrigDocType, recordOfEmployment.OrigRefNbr);

				PRPayChecksAndAdjustments graph = PXGraph.CreateInstance<PRPayChecksAndAdjustments>();
				graph.Document.Current = payment;
				throw new PXRedirectRequiredException(graph, true, "Pay Checks and Adjustments");
			}
		}
		#endregion

		#region Event Handlers

		protected void _(Events.RowSelected<PRRecordOfEmployment> e)
		{
			PRRecordOfEmployment currentRecord = e.Row;
			if (e.Row == null)
			{
				return;
			}

			PXEntryStatus entryStatus = e.Cache.GetStatus(e.Row);

			string roeStatus = currentRecord.Status;
			Document.Cache.AllowDelete = roeStatus == ROEStatus.Open || roeStatus == ROEStatus.Exported;

			Export.SetEnabled(entryStatus != PXEntryStatus.Inserted);
			bool amendmentFieldsEnabled = e.Row.OrigDocType == null && e.Row.OrigRefNbr == null && (roeStatus == ROEStatus.Open || roeStatus == ROEStatus.Exported);
			PXUIFieldAttribute.SetEnabled<PRRecordOfEmployment.amendment>(e.Cache, e.Row, amendmentFieldsEnabled);
			PXUIFieldAttribute.SetEnabled<PRRecordOfEmployment.amendedRefNbr>(e.Cache, e.Row, amendmentFieldsEnabled);

			PXUIFieldAttribute.SetEnabled<PRRecordOfEmployment.employeeID>(e.Cache, e.Row, string.IsNullOrWhiteSpace(e.Row.OrigDocType) && string.IsNullOrWhiteSpace(e.Row.OrigRefNbr));
						
			ShowFinalPaycheck.SetEnabled(!string.IsNullOrWhiteSpace(Document.Current.OrigDocType) && !string.IsNullOrWhiteSpace(Document.Current.OrigRefNbr));
		}

		protected virtual void _(Events.FieldUpdated<PRRecordOfEmployment, PRRecordOfEmployment.branchID> e)
		{
			if (e.Row == null)
			{
				return;
			}

			BAccount bAccount = PXSelectJoin<BAccountR,
				InnerJoin<GL.Branch, On<GL.Branch.bAccountID, Equal<BAccountR.bAccountID>>>,
				Where<GL.Branch.branchID, Equal<Required<GL.Branch.branchID>>>>.Select(this, e.NewValue);

			var organizationID = PXAccess.GetParentOrganizationID(e.NewValue as int?);
			var organization = OrganizationMaint.FindOrganizationByID(this, organizationID);

			e.Row.AddressID = bAccount.DefAddressID;
			e.Row.CRAPayrollAccountNumber = PRTaxReportingAccount.PK.Find(e.Cache.Graph, organization.FileTaxesByBranches == true ? bAccount.BAccountID : organization.BAccountID)?.CRAPayrollAccountNumber;
		}

		protected virtual void _(Events.FieldUpdated<Address, Address.countryID> e)
		{
			if (e.Row != null && e.OldValue != e.NewValue)
			{
				e.Row.State = null;
			}
		}

		protected virtual void _(Events.RowInserted<Address> e)
		{
			if (e.Row != null && e.Row.AddressID > 0)
			{
				Document.Current.AddressID = e.Row.AddressID;
			}
		}

		protected virtual void _(Events.RowPersisting<PRRecordOfEmployment> e)
		{
			if (e.Row == null)
			{
				return;
			}

			if (e.Row.CRAPayrollAccountNumber == null)
			{
				e.Cache.RaiseExceptionHandling<PRRecordOfEmployment.craPayrollAccountNumber>(
					e.Row,
					null,
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName(e.Cache, nameof(e.Row.CRAPayrollAccountNumber))));
			}

			if (e.Row.AddressID == null || e.Row.AddressID < 1)
			{
				e.Cache.RaiseExceptionHandling<PRRecordOfEmployment.addressID>(
					e.Row,
					null,
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName(e.Cache, nameof(e.Row.AddressID))));
			}
		}

		protected virtual void _(Events.RowDeleted<PRRecordOfEmployment> e)
		{
			if (!string.IsNullOrWhiteSpace(e.Row?.AmendedRefNbr))
			{
				PRRecordOfEmployment amendedRoe = PRRecordOfEmployment.PK.Find(e.Cache.Graph, e.Row.AmendedRefNbr);
				amendedRoe.Status = ROEStatus.NeedsAmendment;
				Document.Cache.Update(amendedRoe);
			}
		}

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.EmployerNameB4)]
		protected virtual void _(Events.CacheAttached<PRRecordOfEmployment.branchID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.AddressLine1B4)]
		protected virtual void _(Events.CacheAttached<Address.addressLine1> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.AddressLine2B4)]
		protected virtual void _(Events.CacheAttached<Address.addressLine2> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.CityB4)]
		protected virtual void _(Events.CacheAttached<Address.city> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.CountryB4)]
		protected virtual void _(Events.CacheAttached<Address.countryID> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.StateB4)]
		protected virtual void _(Events.CacheAttached<Address.state> e) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.PostalCodeB4)]
		protected virtual void _(Events.CacheAttached<Address.postalCode> e) { }
		#endregion

		public virtual void GenerateXmlFile()
		{
			PRRecordOfEmploymentMaint roeGraph = PXGraph.CreateInstance<PRRecordOfEmploymentMaint>();
			PRRecordOfEmployment roe = Document.Current;
			roeGraph.Document.Current = roe;

			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", "");
			xmlDocument.AppendChild(xmlDeclaration);

			AppendHeader(xmlDocument);
			AppendROETag(xmlDocument, roeGraph);

			byte[] xmlFile;
			using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
			{
				xmlDocument.Save(memoryStream);
				xmlFile = memoryStream.ToArray();
			}

			var fileInfo = new PX.SM.FileInfo(string.Format("RecordOfEmployment_{0}.blk", roe.RefNbr), null, xmlFile);
			throw new PXRedirectToFileException(fileInfo, true);
		}

		public virtual void AppendHeader(XmlDocument xmlDocument)
		{
			string productVersion = PXVersionInfo.Version;

			if (productVersion?.Length > 10)
			{
				productVersion = productVersion.Replace(".", string.Empty);

				if (productVersion.Length > 10)
				{
					productVersion = productVersion.Substring(0, 10);
				}
			}

			XmlElement roeHeader = xmlDocument.CreateElement("ROEHEADER");
			SetAttribute(xmlDocument, roeHeader, "FileVersion", "W-2.0");
			SetAttribute(xmlDocument, roeHeader, "SoftwareVendor", "Acumatica");
			SetAttribute(xmlDocument, roeHeader, "ProductName", "Acumatica Payroll");
			SetAttribute(xmlDocument, roeHeader, "ProductVersion", productVersion);
			xmlDocument.AppendChild(roeHeader);
		}

		public virtual void AppendROETag(XmlDocument xmlDocument, PRRecordOfEmploymentMaint roeGraph)
		{
			PRRecordOfEmployment roe = roeGraph.Document.Current;
			PREmployee currentEmployee = PREmployee.PK.Find(roeGraph, roe.EmployeeID);
			Contact employeeContact = SelectFrom<Contact>.Where<Contact.contactID.IsEqual<P.AsInt>>.View.SelectSingleBound(roeGraph, null, currentEmployee.DefContactID);
			Users currentUser = SelectFrom<Users>.Where<Users.pKID.IsEqual<P.AsGuid>>.View.SelectSingleBound(roeGraph, null, Accessinfo.UserID);
			Address employeeAddress = CR.Address.PK.Find(roeGraph, currentEmployee.DefAddressID);
			BAccount employerAccount = BAccount.PK.Find(this, currentEmployee.ParentBAccountID);
			Organization organization = SelectFrom<Organization>
				.Where<Organization.bAccountID.IsEqual<P.AsInt>>.View.Select(this, currentEmployee.ParentBAccountID);
			if (organization == null)
			{
				GL.Branch branch = SelectFrom<GL.Branch>
					.Where<GL.Branch.bAccountID.IsEqual<P.AsInt>>.View.Select(this, currentEmployee.ParentBAccountID);

				int? organizationID = PXAccess.GetParentOrganizationID(branch.BranchID);
				organization = OrganizationMaint.FindOrganizationByID(this, organizationID);

				if (organization.OrganizationType != OrganizationTypes.WithBranchesBalancing)
				{
					employerAccount = BAccount.PK.Find(this, organization.BAccountID);
				}
			}
			Contact employerContact = Contact.PK.Find(this, employerAccount.DefContactID);
			PREmployeeAttribute socialSecurityNumberAttribute = SelectFrom<PREmployeeAttribute>
				.Where<PREmployeeAttribute.bAccountID.IsEqual<P.AsInt>
					.And<PREmployeeAttribute.canadaReportMapping.IsEqual<P.AsInt>>>
				.View.SelectSingleBound(roeGraph, null, roe.EmployeeID, PX.Payroll.CanadaReportField.EMP.SocialInsuranceNumber);

			string employeeFirstName = GetTruncatedString(employeeContact?.FirstName ?? currentEmployee.AcctName, 20);
			string employeeLastName = GetTruncatedString(employeeContact?.LastName ?? currentEmployee.AcctName, 28);
			string employeeInitials = GetTruncatedString(employeeContact?.MidName, 4);
			string userFirstName = GetTruncatedString(currentUser?.FirstName, 20);
			string userLastName = GetTruncatedString(currentUser?.LastName, 28);
			string phone = employerContact?.Phone1 ?? string.Empty;
			string areaCode = string.Empty;
			string phoneNumber = string.Empty;
			phone = phone.Replace("+1", string.Empty).Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty);
			if (phone.Length >= 10)
			{
				areaCode = phone.Substring(0, 3);
				phoneNumber = phone.Substring(3, 7);
			}
			string addressLine1 = string.Empty;
			if (!string.IsNullOrEmpty(employeeAddress.AddressLine1) && !string.IsNullOrEmpty(employeeAddress.AddressLine2))
			{
				addressLine1 = GetTruncatedString(employeeAddress.AddressLine1 + ", " + employeeAddress.AddressLine2, 35);
			}
			else if (!string.IsNullOrEmpty(employeeAddress.AddressLine1))
			{
				addressLine1 = GetTruncatedString(employeeAddress.AddressLine1, 35);
			}
			else if (!string.IsNullOrEmpty(employeeAddress.AddressLine2))
			{
				addressLine1 = GetTruncatedString(employeeAddress.AddressLine2, 35);
			}
			string addressLine2 = GetTruncatedString(employeeAddress.City, 35);
			string addressLine3 = GetTruncatedString(employeeAddress.State + ", " + employeeAddress.CountryID, 35);
			string postalCode = GetTruncatedString(employeeAddress.PostalCode, 10);

			XmlElement roeTag = xmlDocument.CreateElement("ROE");
			SetAttribute(xmlDocument, roeTag, "PrintingLanguage", "E");
			SetAttribute(xmlDocument, roeTag, "Issue", "D");

			if (roe.Amendment == true && !string.IsNullOrWhiteSpace(roe.AmendedRefNbr))
			{
				AppendNode(xmlDocument, roeTag, "B2", roe.AmendedRefNbr);
			}
			AppendNode(xmlDocument, roeTag, "B5", roe.CRAPayrollAccountNumber);
			AppendNode(xmlDocument, roeTag, "B6", GetPeriodType(roe.PeriodType));
			AppendNode(xmlDocument, roeTag, "B8", socialSecurityNumberAttribute?.Value);

			XmlNode b9 = xmlDocument.CreateElement("B9");

			AppendNode(xmlDocument, b9, "FN", employeeFirstName);
			if (!string.IsNullOrWhiteSpace(employeeInitials))
			{
				AppendNode(xmlDocument, b9, "MN", employeeInitials);
			}
			AppendNode(xmlDocument, b9, "LN", employeeLastName);
			AppendNode(xmlDocument, b9, "A1", addressLine1);
			AppendNode(xmlDocument, b9, "A2", addressLine2);
			AppendNode(xmlDocument, b9, "A3", addressLine3);
			AppendNode(xmlDocument, b9, "PC", postalCode);
			roeTag.AppendChild(b9);

			if (roe.FirstDayWorked != null)
			{
				AppendNode(xmlDocument, roeTag, "B10", roe.FirstDayWorked.Value.ToString("yyyy-MM-dd"));
			}
			if (roe.LastDayForWhichPaid != null)
			{
				AppendNode(xmlDocument, roeTag, "B11", roe.LastDayForWhichPaid.Value.ToString("yyyy-MM-dd"));
			}
			if (roe.FinalPayPeriodEndingDate != null)
			{
				AppendNode(xmlDocument, roeTag, "B12", roe.FinalPayPeriodEndingDate.Value.ToString("yyyy-MM-dd"));
			}

			XmlNode b14 = xmlDocument.CreateElement("B14");
			AppendNode(xmlDocument, b14, "CD", "U");
			AppendNode(xmlDocument, b14, "DT", null);
			roeTag.AppendChild(b14);

			AppendNode(xmlDocument, roeTag, "B15A", roe.TotalInsurableHours.GetValueOrDefault().ToString("0"));

			XmlNode b15c = xmlDocument.CreateElement("B15C");
			int payPeriodNumber = 0;
			foreach (PRROEInsurableEarningsByPayPeriod insurableEarningsByPayPeriod in roeGraph.InsurableEarnings.Select().FirstTableItems.OrderByDescending(x => x.PayPeriodID))
			{
				payPeriodNumber++;
				XmlElement pp = xmlDocument.CreateElement("PP");
				SetAttribute(xmlDocument, pp, "nbr", payPeriodNumber.ToString());

				XmlNode amt = xmlDocument.CreateElement("AMT");
				amt.InnerText = insurableEarningsByPayPeriod.InsurableEarnings.GetValueOrDefault().ToString("0.00");
				pp.AppendChild(amt);
				b15c.AppendChild(pp);
			}
			roeTag.AppendChild(b15c);

			XmlNode b16 = xmlDocument.CreateElement("B16");
			AppendNode(xmlDocument, b16, "CD", roe.ReasonForROE);
			AppendNode(xmlDocument, b16, "FN", userFirstName);
			AppendNode(xmlDocument, b16, "LN", userLastName);
			AppendNode(xmlDocument, b16, "AC", areaCode != string.Empty ? areaCode : null);
			AppendNode(xmlDocument, b16, "TEL", phoneNumber != string.Empty ? phoneNumber : null);
			roeTag.AppendChild(b16);

			XmlNode b17A = xmlDocument.CreateElement("B17A");
			XmlElement vacationPay = xmlDocument.CreateElement("VP");
			SetAttribute(xmlDocument, vacationPay, "nbr", "1");

			// Vacation Pay Code with value "2" represents the option "Paid because no longer working"
			XmlNode payCode = xmlDocument.CreateElement("CD");
			payCode.InnerText = "2";
			vacationPay.AppendChild(payCode);

			// The start and end dates must be empty because Vacation Pay Code is set to the value "2"
			vacationPay.AppendChild(xmlDocument.CreateElement("SDT"));
			vacationPay.AppendChild(xmlDocument.CreateElement("EDT"));

			XmlNode amount = xmlDocument.CreateElement("AMT");
			amount.InnerText = CurrentDocument.Current.VacationPay.ToString();
			vacationPay.AppendChild(amount);

			b17A.AppendChild(vacationPay);
			roeTag.AppendChild(b17A);

			XmlNode b17B = xmlDocument.CreateElement("B17B");
			int holidayNumber = 0;
			foreach (PRROEStatutoryHolidayPay statutoryHolidayPay in roeGraph.StatutoryHolidays.Select())
			{
				holidayNumber++;
				XmlElement sh = xmlDocument.CreateElement("SH");
				SetAttribute(xmlDocument, sh, "nbr", holidayNumber.ToString());

				XmlNode dt = xmlDocument.CreateElement("DT");
				dt.InnerText = statutoryHolidayPay.Date.Value.ToString("yyyy-MM-dd");
				sh.AppendChild(dt);

				XmlNode amt = xmlDocument.CreateElement("AMT");
				amt.InnerText = statutoryHolidayPay.Amount.GetValueOrDefault().ToString("0.00");
				sh.AppendChild(amt);

				b17B.AppendChild(sh);

				if (holidayNumber >= 10)
				{
					break;
				}
			}
			roeTag.AppendChild(b17B);

			XmlNode b17c = xmlDocument.CreateElement("B17C");
			int otherMoniesNumber = 0;
			foreach (PRROEOtherMonies otherMonies in roeGraph.OtherMonies.Select())
			{
				otherMoniesNumber++;
				XmlElement om = xmlDocument.CreateElement("OM");
				SetAttribute(xmlDocument, om, "nbr", otherMoniesNumber.ToString());
				
				XmlElement cd = xmlDocument.CreateElement("CD");
				cd.InnerText = "B11";
				om.AppendChild(cd);

				XmlNode amt = xmlDocument.CreateElement("AMT");
				amt.InnerText = otherMonies.Amount.GetValueOrDefault().ToString("0.00");
				om.AppendChild(amt);

				b17c.AppendChild(om);

				if (otherMoniesNumber >= 3)
				{
					break;
				}
			}
			roeTag.AppendChild(b17c);

			AppendNode(xmlDocument, roeTag, "B18", roe.Comments);

			xmlDocument.DocumentElement.AppendChild(roeTag);
		}



		public static string GetTruncatedString(string value, int length)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return string.Empty;
			}

			if (value.Length < length)
			{
				return value;
			}

			return value.Substring(0, length);
		}

		public static void AppendNode(XmlDocument xmlDocument, XmlNode parentNode, string nodeName, string nodeValue)
		{
			XmlNode xmlNode = xmlDocument.CreateElement(nodeName);
			xmlNode.InnerText = nodeValue;
			parentNode.AppendChild(xmlNode);
		}

		public static void SetAttribute(XmlDocument xmlDocument, XmlElement xmlElement, string attributeName, string attributeValue)
		{
			XmlAttribute xmlAttribute = xmlDocument.CreateAttribute(attributeName);
			xmlAttribute.Value = attributeValue;
			xmlElement.SetAttributeNode(xmlAttribute);
		}

		public static string GetPeriodType(string periodType)
		{
			switch (periodType)
			{
				case FinPeriodType.BiWeek:
					return "W";
				case FinPeriodType.Month:
					return "M";
				case FinPeriodType.BiMonth:
					return "S";
				case FinPeriodType.CustomPeriodsNumber:
					return "S";
				case FinPeriodType.Week:
				default:
					return "W";
			}
		}

		#region Address Lookup Extension
		public class PRRecordOfEmploymentMaintAddressLookupExtension : CR.Extensions.AddressLookupExtension<PRRecordOfEmploymentMaint, PRRecordOfEmployment, Address>
		{
			protected override string AddressView => nameof(Base.Address);
		}

		#endregion
	}
}
