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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CN.Compliance.CL.DAC;
using PX.Objects.CN.Compliance.Descriptor;
using System.Collections.Generic;

namespace PX.Objects.CN.Compliance.CL.Descriptor.Attributes
{
	public class ComplianceDocumentPolicyUniqueAttribute : PXEventSubscriberAttribute, IPXRowPersistingSubscriber
	{
		public void RowPersisting(PXCache cache, PXRowPersistingEventArgs args)
		{
			if (args.Operation != PXDBOperation.Delete
				&& args.Row is ComplianceDocument complianceDocument)
			{
				var insuranceDocumentTypeId = new PXSelectReadonly<ComplianceAttributeType,
										Where<ComplianceAttributeType.type, Equal<ComplianceDocumentType.insurance>>>(cache.Graph)
											.SelectSingle()?.ComplianceAttributeTypeID;

				if (insuranceDocumentTypeId == null || complianceDocument.DocumentType != insuranceDocumentTypeId)
				{
					return;
				}

				if (IsDuplicate(cache, complianceDocument, (int)insuranceDocumentTypeId))
				{
					cache.RaiseExceptionHandling<ComplianceDocument.policy>(args.Row, complianceDocument.Policy,
						new PXSetPropertyException(ComplianceMessages.UniqueConstraintMessage, PXErrorLevel.RowError));
				}
			}
		}

		private bool IsDuplicate(PXCache cache, ComplianceDocument complianceDocument, int insuranceDocumentTypeId)
		{
			var parameters = new List<object>() { insuranceDocumentTypeId };

			var query = new SelectFrom<ComplianceDocument>
				.Where<ComplianceDocument.documentType.IsEqual<P.AsInt>>
				.View(cache.Graph);

			if (complianceDocument.Policy != null)
			{
				query.WhereAnd<Where<ComplianceDocument.policy.IsEqual<P.AsString>>>();
				parameters.Add(complianceDocument.Policy);
			}
			else
			{
				query.WhereAnd<Where<ComplianceDocument.policy.IsNull>>();
			}

			if (complianceDocument.VendorID != null)
			{
				query.WhereAnd<Where<ComplianceDocument.vendorID.IsEqual<P.AsInt>>>();
				parameters.Add(complianceDocument.VendorID);
			}
			else
			{
				query.WhereAnd<Where<ComplianceDocument.vendorID.IsNull>>();
			}

			if (complianceDocument.ProjectID != null)
			{
				query.WhereAnd<Where<ComplianceDocument.projectID.IsEqual<P.AsInt>>>();
				parameters.Add(complianceDocument.ProjectID);
			}
			else
			{
				query.WhereAnd<Where<ComplianceDocument.projectID.IsNull>>();
			}

			if (complianceDocument.EffectiveDate != null)
			{
				query.WhereAnd<Where<ComplianceDocument.effectiveDate.IsEqual<P.AsDateTime>>>();
				parameters.Add(complianceDocument.EffectiveDate);
			}
			else
			{
				query.WhereAnd<Where<ComplianceDocument.effectiveDate.IsNull>>();
			}

			if (complianceDocument.ExpirationDate != null)
			{
				query.WhereAnd<Where<ComplianceDocument.expirationDate.IsEqual<P.AsDateTime>>>();
				parameters.Add(complianceDocument.ExpirationDate);
			}
			else
			{
				query.WhereAnd<Where<ComplianceDocument.expirationDate.IsNull>>();
			}

			if (complianceDocument.Limit != null)
			{
				query.WhereAnd<Where<ComplianceDocument.limit.IsEqual<P.AsDecimal>>>();
				parameters.Add(complianceDocument.Limit);
			}
			else
			{
				query.WhereAnd<Where<ComplianceDocument.limit.IsNull>>();
			}

			if (complianceDocument.DocumentTypeValue != null)
			{
				query.WhereAnd<Where<ComplianceDocument.documentTypeValue.IsEqual<P.AsInt>>>();
				parameters.Add(complianceDocument.DocumentTypeValue);
			}
			else
			{
				query.WhereAnd<Where<ComplianceDocument.documentTypeValue.IsNull>>();
			}

			return query.Select(parameters.ToArray()).FirstTableItems.HasAtLeast(2);
		}
	}
}
