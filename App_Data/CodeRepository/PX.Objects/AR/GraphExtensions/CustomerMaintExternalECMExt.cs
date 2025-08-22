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
using PX.Common;
using System.Linq;
using System.Text;
using PX.TaxProvider;
using PX.Objects.TX;
using PX.Data;
using PX.Objects.CS;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Web.UI;

namespace PX.Objects.AR
{
	public class CustomerMaintExternalECMExt : PXGraphExtension<CustomerMaint>
	{
		public PXSelect<TXSetup> TXSetupRecord;
		public PXFilter<ExemptCustomerFilter> CreateCustomerFilter;
		public PXFilter<RequestECMCertificateFilter> RequestCertificateFilter;

		[PXNotCleanable]
		[PXReadOnlyView]
		public PXSelect<ExemptionCertificate> ExemptionCertificates;

		[PXReadOnlyView]
		public PXSelect<CertificateTemplate> CertificateTemplates;

		[PXReadOnlyView]
		public PXSelect<CertificateReason> CertificateReasons;

		public virtual IEnumerable exemptionCertificates() =>
			Base.CurrentCustomer.Current != null ? ExemptionCertificates.Cache.Cached : Array.Empty<ExemptionCertificate>();

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.eCM>();
		}

		protected virtual void _(Events.RowUpdated<Customer> e)
		{
			if (!e.Cache.ObjectsEqual<Customer.acctName>(e.Row, e.OldRow))
			{
				Base.BAccount.Current.IsECMValid = false;
			}
		}

		#region ECMAction

		public PXAction<Customer> retrieveCertificate;
		[PXUIField(DisplayName = Messages.RetrieveCertificates, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable RetrieveCertificate(PXAdapter adapter)
		{
			ClearExemptionCertificateCache(ExemptionCertificates.Cache);
			if (Base.CurrentCustomer.Current != null)
			{
				GetExemptionCertificates(Base.CurrentCustomer.Current);
				WebDialogResult res = ExemptionCertificates.AskExt();
			}

			return adapter.Get();
		}

		public PXAction<Customer> requestCertificate;
		[PXUIField(DisplayName = Messages.RequestCertificate, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable RequestCertificate(PXAdapter adapter)
		{
			if (Base.CurrentCustomer.Current != null)
			{
				Customer customer = Base.CurrentCustomer.Current;

				GetCertificateTemplates();
				GetCertificateReasons();

				var requestCertificateFilterCache = RequestCertificateFilter.Cache;

				WebDialogResult result = RequestCertificateFilter.AskExt();
				if (result != WebDialogResult.OK)
				{
					return adapter.Get();
				}

				var RequestCertificateParams = RequestCertificateFilter.Current;
				bool hasError = CheckForEmptyFields(requestCertificateFilterCache, RequestCertificateParams);

				if (hasError)
					return adapter.Get();

				PXLongOperation.StartOperation(Base, delegate ()
				{
					CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
					CustomerMaintExternalECMExt graphExt = graph.GetExtension<CustomerMaintExternalECMExt>();

					graphExt.RequestExemptionCertificate(customer, RequestCertificateParams);
					throw new PXOperationCompletedException(AR.Messages.RequestCertificateInECM);
				});
			}
			RequestCertificateFilter.Cache.Clear();
			return adapter.Get();
		}

		public PXAction<Customer> createCustomerInECM;
		[PXUIField(DisplayName = Messages.CreateCustomerInECM, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable CreateCustomerInECM(PXAdapter adapter)
		{
			var customer = Base.CurrentCustomer.Current;
			if (customer != null)
			{
				var mapping = SelectFrom<TaxPluginMapping>
				.InnerJoin<TaxPlugin>.On<TaxPluginMapping.taxPluginID.IsEqual<TaxPlugin.taxPluginID>>
				.InnerJoin<TXSetup>.On<TaxPlugin.taxPluginID.IsEqual<TXSetup.eCMProvider>>
				.AggregateTo<GroupBy<TaxPluginMapping.companyCode>>.View.Select(Base);

				string companyCode = mapping.Count == 1 ? ((TaxPluginMapping)mapping[0]).CompanyCode : string.Empty;

				if (mapping.Count() > 1)
				{
					WebDialogResult dialogResult = this.CreateCustomerFilter.AskExt(true);
					if (dialogResult == WebDialogResult.OK)
					{
						companyCode = CreateCustomerFilter.Current.CompanyCode;
					}
				}

				PXLongOperation.StartOperation(Base, delegate ()
				{
					CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
					CustomerMaintExternalECMExt graphExt = graph.GetExtension<CustomerMaintExternalECMExt>();

					graphExt.CreateECMCustomer(customer, companyCode, out string warning);

					if (!string.IsNullOrEmpty(warning))
					{
						throw new PXOperationCompletedWithWarningException(AR.Messages.WRN_CustomerExistsInECM, warning);
					}
					else
					{
						throw new PXOperationCompletedException(AR.Messages.CustomerCreatedInECM);
					}
				});
			}

			return adapter.Get();
		}

		public PXAction<Customer> updateCustomerInECM;
		[PXUIField(DisplayName = Messages.UpdateCustomerInECM, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable UpdateCustomerInECM(PXAdapter adapter)
		{
			var customer = Base.CurrentCustomer.Current;
			if (customer != null)
			{
				PXLongOperation.StartOperation(Base, delegate ()
				{
					CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
					CustomerMaintExternalECMExt graphExt = graph.GetExtension<CustomerMaintExternalECMExt>();
					graphExt.UpdateECMCustomer(customer);

					throw new PXOperationCompletedException(AR.Messages.CustomerUpdatedInECM);
				});
			}

			return adapter.Get();
		}

		public PXAction<Customer> refreshCertificates;
		[PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Refresh)]
		public virtual IEnumerable RefreshCertificates(PXAdapter adapter)
		{
			if (Base.CurrentCustomer.Current != null)
			{
				ClearExemptionCertificateCache(ExemptionCertificates.Cache);
				GetExemptionCertificates(Base.CurrentCustomer.Current);
			}
			return adapter.Get();
		}

		public PXAction<ExemptionCertificate> viewCertificate;
		[PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ViewCertificate(PXAdapter adapter)
		{
			var certificate = ExemptionCertificates.Current;
			if (certificate != null)
			{
				PXLongOperation.StartOperation(Base, delegate ()
				{
					CustomerMaint graph = PXGraph.CreateInstance<CustomerMaint>();
					CustomerMaintExternalECMExt graphExt = graph.GetExtension<CustomerMaintExternalECMExt>();
					graphExt.ViewExemptCertificate(certificate);
				});
			}
			return adapter.Get();
		}

		#endregion

		#region GetCertificates

		public void ClearExemptionCertificateCache(PXCache certificateCache)
		{
			// We can't use Cache.Clear() method as the cache is marked with [PXNotCleanable] attribute.
			var itemsToRemove = certificateCache.Cached.Cast<ExemptionCertificate>().ToList();
			foreach (var item in itemsToRemove)
			{
				certificateCache.Remove(item);
			}
		}

		public void GetExemptionCertificates(Customer customer)
		{
			if (customer != null)
			{
				string[] companies = !string.IsNullOrEmpty(customer.ECMCompanyCode) ? customer.ECMCompanyCode.Split(',').Select(p => p.Trim()).ToArray() : Array.Empty<string>();
				TXSetup txsetup = TXSetupRecord.Select();

				if (txsetup != null)
				{
					try
					{
						var service = TaxPluginMaint.CreateECMProvider(Base, txsetup.ECMProvider);
						foreach (string company in companies)
						{
							string companyId = CompanyIdForCompanyCode(company);
							GetCertificatesRequest request = BuildGetCertificatesRequest(customer, companyId);
							GetCertificatesResult result = service.GetCertificates(request);

							if (result.IsSuccess)
							{
								foreach (CertificateDetail certificate in result.Certificates)
								{
									ExemptionCertificate exemptionCertificate = new ExemptionCertificate
									{
										CertificateID = certificate.CertificateID,
										ECMCompanyID = certificate.CompanyId,
										CompanyCode = company,
										ExemptionReason = certificate.ExemptionReason,
										State = certificate.State,
										Status = certificate.Status,
										EffectiveDate = certificate.SignedDate,
										ExpirationDate = certificate.ExpirationDate
									};
									ExemptionCertificates.Insert(exemptionCertificate);
								}
							}
							else
							{
								LogMessages(result);
							}
						}
					}
					catch (Exception ex)
					{
						throw new PXException(ex.Message.ToString());
					}
				}
			}
		}

		public virtual GetCertificatesRequest BuildGetCertificatesRequest(Customer customer, string companyId)
		{
			if (customer == null)
				throw new PXArgumentException(nameof(customer), ErrorMessages.ArgumentNullException);

			GetCertificatesRequest request = new GetCertificatesRequest
			{
				CompanyId = companyId,
				CustomerCode = customer.AcctCD
			};

			return request;
		}
		#endregion

		#region RequestCertificate

		public virtual void GetCertificateTemplates()
		{
			var templates = Base.Caches[typeof(CertificateTemplate)].Cached;

			if (templates == null || templates.Count() == 0)
			{
				TXSetup txsetup = TXSetupRecord.Select();
				if (txsetup != null)
				{
					var service = TaxPluginMaint.CreateECMProvider(Base, txsetup.ECMProvider);
					GetCoverLettersResult result = service.GetCoverLetters();

					if (result.IsSuccess)
					{
						foreach (CoverLetter letter in result.Letters)
						{
							CertificateTemplate template = new CertificateTemplate
							{
								TemplateID = letter.LetterId,
								TemplateName = letter.LetterName
							};

							Base.Caches[typeof(CertificateTemplate)].Insert(template);
						}
					}
					else
					{
						LogMessages(result);
					}
				}
			}
		}

		public virtual void GetCertificateReasons()
		{
			var reasons = Base.Caches[typeof(CertificateReason)].Cached;

			if (reasons == null || reasons.Count() == 0)
			{
				TXSetup txsetup = TXSetupRecord.Select();
				if (txsetup != null)
				{
					var service = TaxPluginMaint.CreateECMProvider(Base, txsetup.ECMProvider);
					GetExemptReasonsResult result = service.GetExemptReasons();

					if (result.IsSuccess)
					{
						foreach (ExemptReason reason in result.Reasons)
						{
							CertificateReason certificateReason = new CertificateReason
							{
								ReasonID = reason.ReasonId,
								ReasonName = reason.Reason
							};

							Base.Caches[typeof(CertificateReason)].Insert(certificateReason);
						}
					}
					else
					{
						LogMessages(result);
					}
				}
			}
		}

		public bool CheckForEmptyFields(PXCache certificateViewCache, RequestECMCertificateFilter certificateParams)
		{
			bool hasError = false;
			if (string.IsNullOrEmpty(certificateParams.CompanyCode))
			{
				hasError = true;
				certificateViewCache.RaiseExceptionHandling<RequestECMCertificateFilter.companyCode>(certificateParams, certificateParams.CompanyCode,
				new PXSetPropertyException(Messages.ERR_NullOrEmptyField, PXErrorLevel.Error, "Customer Code"));
			}

			if (string.IsNullOrEmpty(certificateParams.EmailId))
			{
				hasError = true;
				certificateViewCache.RaiseExceptionHandling<RequestECMCertificateFilter.emailId>(certificateParams, certificateParams.EmailId,
				new PXSetPropertyException(Messages.ERR_NullOrEmptyField, PXErrorLevel.Error, "Email"));
			}
			if (string.IsNullOrEmpty(certificateParams.Template))
			{
				hasError = true;
				certificateViewCache.RaiseExceptionHandling<RequestECMCertificateFilter.template>(certificateParams, certificateParams.Template,
				new PXSetPropertyException(Messages.ERR_NullOrEmptyField, PXErrorLevel.Error, "Template"));
			}
			if (string.IsNullOrEmpty(certificateParams.CountryID))
			{
				hasError = true;
				certificateViewCache.RaiseExceptionHandling<RequestECMCertificateFilter.countryID>(certificateParams, certificateParams.CountryID,
				new PXSetPropertyException(Messages.ERR_NullOrEmptyField, PXErrorLevel.Error, "Country ID"));
			}
			if (string.IsNullOrEmpty(certificateParams.State))
			{
				hasError = true;
				certificateViewCache.RaiseExceptionHandling<RequestECMCertificateFilter.state>(certificateParams, certificateParams.State,
				new PXSetPropertyException(Messages.ERR_NullOrEmptyField, PXErrorLevel.Error, "State"));
			}

			return hasError;
		}

		public void RequestExemptionCertificate(Customer customer, RequestECMCertificateFilter filter)
		{
			TXSetup txsetup = TXSetupRecord.Select();
			if (txsetup != null)
			{
				try
				{
					var service = TaxPluginMaint.CreateECMProvider(Base, txsetup.ECMProvider);
					string companyId = CompanyIdForCompanyCode(filter.CompanyCode);

					RequestCertificateRequest request = BuildRequestCertificateRequest(customer, filter, companyId);
					RequestCertificateResult result = service.RequestCertificate(request);

					if (!result.IsSuccess)
					{
						string error = LogMessages(result);
						throw new PXException(error);
					}
				}
				catch (Exception ex)
				{
					throw new PXException(ex.Message.ToString());
				}
			}
		}

		public virtual RequestCertificateRequest BuildRequestCertificateRequest(Customer customer, RequestECMCertificateFilter filter, string companyId)
		{
			RequestCertificateRequest request = new RequestCertificateRequest()
			{
				CompanyId = companyId,
				Recipient = filter.EmailId,
				CertificateTemplate = filter.Template,
				Country = filter.CountryID,
				State = filter.State,
				ExemptionReason = filter.ExemptReason,
				CustomerCode = customer.AcctCD
			};
			return request;
		}

		#endregion

		#region CreateCustomer

		public virtual void CreateECMCustomer(Customer customer, string companyCode, out string warning)
		{
			warning = string.Empty;
			if (customer != null)
			{
				try
				{
					List<string> filterCompanies = companyCode.Split(',').Select(p => p.Trim()).ToList();
					List<string> customerCompanies = (!string.IsNullOrEmpty(customer.ECMCompanyCode) ? customer.ECMCompanyCode.Split(',').Select(p => p.Trim()).ToArray() : Array.Empty<string>()).ToList();

					List<string> companies = filterCompanies.Except(customerCompanies).ToList();
					List<string> existingCompanies = filterCompanies.Intersect(customerCompanies).ToList();

					List<string> newCompanies = new List<string>();
					StringBuilder sb = new StringBuilder();

					bool hasOtherError = false;
					bool isSaveRequired = false;

					TXSetup txsetup = TXSetupRecord.Select();
					if (txsetup != null)
					{
						var service = TaxPluginMaint.CreateECMProvider(Base, txsetup.ECMProvider);
						Base.BAccount.Current = customer;
						Customer copy = (Customer)Base.BAccount.Cache.CreateCopy(customer);

						foreach (string company in companies)
						{
							string companyId = CompanyIdForCompanyCode(company);
							CreateCustomerRequest request = BuildCreateCustomerRequest(customer, companyId);
							CreateCustomerResult result = service.CreateCustomer(request);

							if (result.IsSuccess)
							{
								newCompanies.Add(company);
								isSaveRequired = true;
							}
							else if (!result.IsSuccess && result.IsCustomerExist)
							{
								existingCompanies.Add(company);
								isSaveRequired = true;
							}
							else
							{
								sb.AppendLine(LogMessages(result));
								hasOtherError = true;
							}
						}

						if (existingCompanies.Count() > 0 && newCompanies.Count() == 0)
						{
							warning = string.Join(",", existingCompanies.ToArray());
						}

						if (isSaveRequired)
						{
							customerCompanies.AddRange(newCompanies);
							existingCompanies = existingCompanies.Except(customerCompanies).ToList();
							customerCompanies.AddRange(existingCompanies);

							copy.ECMCompanyCode = string.Join(",", customerCompanies.ToArray());
							copy.IsECMValid = true;
							Base.BAccount.Cache.Update(copy);
							Base.Actions.PressSave();
						}

						if (hasOtherError)
						{
							throw new PXException(sb.ToString());
						}
					}
				}
				catch (Exception ex)
				{
					throw new PXException(ex.Message.ToString());
				}
			}
		}

		public virtual CreateCustomerRequest BuildCreateCustomerRequest(Customer customer, string companyID)
		{
			if (customer == null)
				throw new PXArgumentException(nameof(customer), ErrorMessages.ArgumentNullException);

			CreateCustomerRequest request = new CreateCustomerRequest
			{
				Customer = CreateCustomer(customer, companyID),
			};

			return request;
		}

		public virtual ECMCustomer CreateCustomer(Customer customer, string companyID)
		{
			var defContactAddress = Base.GetExtension<AR.CustomerMaint.DefContactAddressExt>();
			CR.Address billAddr = defContactAddress.DefAddress.Select();

			var defLocationExt = Base.GetExtension<AR.CustomerMaint.DefLocationExt>();
			PX.Objects.CR.Standalone.Location defLocation = defLocationExt.DefLocation.Select();

			var primctr = Base.GetExtension<AR.CustomerMaint.PrimaryContactGraphExt>();
			CR.Contact primContact = primctr.PrimaryContactCurrent.Select();

			ECMCustomer taxCustomer = new ECMCustomer()
			{
				CompanyId = companyID,
				CustomerId = customer.BAccountID,
				CustomerCode = customer.AcctCD,
				CustomerName = customer.AcctName,
				TaxIdNumber = defLocation?.TaxRegistrationID,
				Address = new TaxAddress()
				{
					AddressLine1 = billAddr?.AddressLine1,
					AddressLine2 = billAddr?.AddressLine2,
					City = billAddr?.City,
					Region = billAddr?.State,
					PostalCode = billAddr?.PostalCode,
					Country = billAddr?.CountryID,
				},
				Contact = new TaxContact()
				{
					EMail = primContact?.EMail,
					Fax = primContact?.Fax,
					FullName = primContact?.DisplayName,
					PhoneNumber = primContact?.Phone1
				}
			};
			return taxCustomer;
		}

		#endregion

		#region UpdateCustomer
		public virtual void UpdateECMCustomer(Customer customer)
		{
			if (customer != null)
			{
				try
				{
					bool hasError = false;
					StringBuilder sb = new StringBuilder();
					string[] companies = !string.IsNullOrEmpty(customer.ECMCompanyCode) ? customer.ECMCompanyCode.Split(',').Select(p => p.Trim()).ToArray() : Array.Empty<string>();

					TXSetup txsetup = TXSetupRecord.Select();
					if (txsetup != null)
					{
						var service = TaxPluginMaint.CreateECMProvider(Base, txsetup.ECMProvider);
						Base.BAccount.Current = customer;
						Customer copy = (Customer)Base.BAccount.Cache.CreateCopy(customer);

						foreach (string company in companies)
						{
							string companyId = CompanyIdForCompanyCode(company);
							UpdateCustomerRequest request = BuildUpdateCustomerRequest(customer, companyId);
							UpdateCustomerResult result = service.UpdateCustomer(request);

							if (!result.IsSuccess)
							{
								sb.AppendLine(LogMessages(result));
								hasError = true;
								break;
							}
						}

						if (hasError)
						{
							throw new PXException(sb.ToString());
						}

						copy.IsECMValid = true;
						Base.BAccount.Cache.Update(copy);
						Base.Actions.PressSave();
					}
				}
				catch (Exception ex)
				{
					throw new PXException(ex.Message.ToString());
				}
			}
		}

		public virtual UpdateCustomerRequest BuildUpdateCustomerRequest(Customer customer, string companyID)
		{
			if (customer == null)
				throw new PXArgumentException(nameof(customer), ErrorMessages.ArgumentNullException);

			UpdateCustomerRequest request = new UpdateCustomerRequest
			{
				Customer = CreateCustomer(customer, companyID)
			};

			return request;
		}

		#endregion

		#region ViewCertificate

		public virtual void ViewExemptCertificate(ExemptionCertificate certificate)
		{
			if (certificate != null)
			{
				TXSetup txsetup = TXSetupRecord.Select();
				if (txsetup != null)
				{
					var service = TaxPluginMaint.CreateECMProvider(Base, txsetup.ECMProvider);
					ViewCertificateRequest request = BuildViewCertificateRequest(certificate);
					ViewCertificateResult result = service.ViewCertificate(request);

					if (result.IsSuccess)
					{
						PX.SM.FileInfo file = new PX.SM.FileInfo(result.Filename, null, result.Data);
						throw new PXRedirectToFileException(file, false);
					}
					else
					{
						LogMessages(result);
						throw new PXException(Messages.ERR_CertificateNotExist);
					}
				}
			}
		}

		public virtual ViewCertificateRequest BuildViewCertificateRequest(ExemptionCertificate certificate)
		{
			ViewCertificateRequest request = new ViewCertificateRequest
			{
				CompanyId = CompanyIdForCompanyCode(certificate.CompanyCode),
				CertificateID = certificate.CertificateID,
			};

			return request;
		}
		#endregion

		#region GetCompany

		public virtual string CompanyIdForCompanyCode(string companyCode)
		{
			TaxPluginMapping result = PXSelectReadonly2<TaxPluginMapping,
				InnerJoin<TaxPlugin, On<TaxPlugin.taxPluginID, Equal<TaxPluginMapping.taxPluginID>>,
				InnerJoin<TXSetup, On<TXSetup.eCMProvider, Equal<TaxPlugin.taxPluginID>>>>,
				Where<TaxPluginMapping.externalCompanyID, IsNotNull,
				And<TaxPluginMapping.companyCode, Equal<Required<TaxPluginMapping.companyCode>>>>>
				.SelectSingleBound(Base, null, companyCode);

			return result?.ExternalCompanyID;
		}

		#endregion

		protected virtual string LogMessages(ResultBase result)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var msg in result.Messages)
			{
				sb.AppendLine(msg);
				PXTrace.WriteError(msg);
			}
			return sb.ToString();
		}
	}

	public class DefContactAddressExtECM : PXGraphExtension<CustomerMaint.DefContactAddressExt, CustomerMaint>
	{
		protected virtual void _(Events.RowUpdated<CR.Address> e)
		{
			if (!e.Cache.ObjectsEqual<
			CR.Address.addressLine1,
			CR.Address.addressLine2,
			CR.Address.city,
			CR.Address.countryID,
			CR.Address.postalCode,
			CR.Address.state>(e.Row, e.OldRow))
			{
				if (Base.BAccount.Current != null)
				{
					Base.BAccount.Current.IsECMValid = false;
					Base.BAccount.Cache.MarkUpdated(Base.BAccount.Current);
				}
			}
		}
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.eCM>();
		}
	}

	public class PrimaryContactGraphExtECMExt : PXGraphExtension<CustomerMaint.PrimaryContactGraphExt, CustomerMaint>
	{
		protected virtual void _(Events.RowUpdated<CR.Contact> e)
		{
			if (!e.Cache.ObjectsEqual<
			CR.Contact.eMail,
			CR.Contact.fax,
			CR.Contact.displayName,
			CR.Contact.phone1>(e.Row, e.OldRow))
			{
				if (Base.BAccount.Current != null)
				{
					Base.BAccount.Current.IsECMValid = false;
					Base.BAccount.Cache.MarkUpdated(Base.BAccount.Current);
				}
			}
		}
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.eCM>();
		}
	}

	public class DefLocationExtECMExt : PXGraphExtension<CustomerMaint.DefLocationExt, CustomerMaint>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.eCM>();
		}

		protected virtual void _(Events.RowUpdated<CR.Standalone.Location> e)
		{
			if (!e.Cache.ObjectsEqual<
			CR.Standalone.Location.taxRegistrationID>(e.Row, e.OldRow))
			{
				if (Base.BAccount.Current != null)
				{
					Base.BAccount.Current.IsECMValid = false;
					Base.BAccount.Cache.MarkUpdated(Base.BAccount.Current);
				}
			}
		}
	}
}
