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
using System.Collections.Generic;
using System.Linq;

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CN.Common.Services;
using PX.Objects.CN.Compliance.AP.CacheExtensions;
using PX.Objects.CN.Compliance.CL.DAC;
using PX.Objects.CN.Compliance.Descriptor;
using PX.Objects.CS;
using PX.Objects.PM;

namespace PX.Objects.CN.Compliance.CL.Services
{
	internal class ComplianceDocumentService
	{
		private readonly PXGraph graph;
		private readonly CommonAttributeColumnCreator columnCreator;
		private readonly PXSelectBase<ComplianceDocument> complianceDocuments;
		private readonly string complianceDocumentsViewName;

		public ComplianceDocumentService(PXGraph graph, PXSelectBase<CSAttributeGroup> attributeGroups,
			PXSelectBase<ComplianceDocument> complianceDocuments,
			string complianceDocumentsViewName)
		{
			this.graph = graph;
			this.complianceDocuments = complianceDocuments;
			this.complianceDocumentsViewName = complianceDocumentsViewName;
			columnCreator = new CommonAttributeColumnCreator(graph, attributeGroups);
		}

		public void GenerateColumns(PXCache cache, string documentAnswerView)
		{
			columnCreator.GenerateColumns(cache, complianceDocumentsViewName, documentAnswerView);
		}

		public void AddExpirationDateEventHandlers()
		{
			graph.FieldSelecting.AddHandler<ComplianceDocument.expirationDate>((cache, arguments) =>
				ValidateExpirationDateOnFieldSelecting(arguments.Row as ComplianceDocument,
					complianceDocuments.Cache));
			graph.FieldVerifying.AddHandler<ComplianceDocument.expirationDate>((cache, arguments) =>
				ValidateExpirationDateOnFieldVerifying(arguments.Row as ComplianceDocument,
					complianceDocuments.Cache, arguments.NewValue as DateTime?));
		}

		public void UpdateExpirationIndicator(ComplianceDocument document)
		{
			if (document != null)
			{
				document.IsExpired = document.ExpirationDate < graph.Accessinfo.BusinessDate;
			}
		}

		public void ValidateComplianceDocuments(PXCache eventCache, IEnumerable<ComplianceDocument> documents,
			PXCache documentsCache)
		{
			if (eventCache != null && eventCache.Updated.Any_())
			{
				return;
			}
			var expiredDocuments = documents.Where(d => d.ExpirationDate < graph.Accessinfo.BusinessDate);
			expiredDocuments.ForEach(d => RaiseComplianceDocumentIsExpiredException(
				documentsCache, d, d.ExpirationDate));
		}

		public IEnumerable<ComplianceDocument> GetComplianceDocuments<TField>(object value)
			where TField : IBqlField
		{
			return new PXSelect<ComplianceDocument, Where<TField, Equal<Required<TField>>>>(graph)
				.Select(value).FirstTableItems;
		}

		public void ValidateApAdjustment<TField>(APAdjust adjustment)
			where TField : IBqlField
		{
			if (adjustment.IsSelfAdjustment())
				return;

			APInvoice apInvoice = SelectFrom<APInvoice>
				.Where<APInvoice.refNbr.IsEqual<P.AsString>
					.And<APInvoice.docType.IsEqual<P.AsString>>>.View
				.Select(graph, adjustment.AdjdDocType, adjustment.AdjdRefNbr);

			if (apInvoice != null)
			{
				var id = ComplianceDocumentReferenceRetriever.GetComplianceDocumentReferenceId(graph, apInvoice);

				if (id != null)
				{
					var hasExpiredComplianceDocuments =
						ValidateRelatedField<APAdjust, ComplianceDocument.billID, TField>(adjustment, id);
					ValidateRelatedRow<APAdjust, ApAdjustExt.hasExpiredComplianceDocuments>(adjustment,
						hasExpiredComplianceDocuments);
				}
			}
		}

		public bool ValidateRelatedField<TEntity, TComplianceDocumentField, TEntityField>(
			TEntity entity, object fieldValue)
			where TEntity : class, IBqlTable, new()
			where TComplianceDocumentField : IBqlField
			where TEntityField : IBqlField
		{
			var cache = graph.Caches<TEntity>();
			var hasExpiredDocuments = DoExpiredDocumentsExist<
				Where<TComplianceDocumentField, Equal<Required<TComplianceDocumentField>>>>(fieldValue);
			RaiseOrClearExceptionForRelatedField<TEntityField>(cache, entity, hasExpiredDocuments,
				PXErrorLevel.Warning);
			return hasExpiredDocuments;
		}

		public bool ValidateRelatedProjectField<TEntity, TEntityField>(TEntity entity, object fieldValue)
			where TEntity : class, IBqlTable, new()
			where TEntityField : IBqlField
			=> ValidateProjectRelatedField<TEntity, TEntityField, BqlNone>(entity, fieldValue as int?);

		public bool ValidateProjectVendorRelatedField<TEntity, TEntityField>(TEntity entity,
			int? projectID, int? vendorID)
			where TEntity : class, IBqlTable, new()
			where TEntityField : IBqlField
			=> ValidateProjectRelatedField<TEntity, TEntityField,
				Where<ComplianceDocument.vendorID, Equal<Required<ComplianceDocument.vendorID>>,
					Or<Required<ComplianceDocument.vendorID>, IsNull>>
				>(entity, projectID, vendorID, vendorID);

		public bool ValidateProjectRelatedField<TEntity, TEntityField, TWhere>(TEntity entity,
			int? projectID, params object[] args)
			where TEntity : class, IBqlTable, new()
			where TEntityField : IBqlField
			where TWhere : IBqlWhere, new()
		{
			var values = new List<object> { projectID };
			values.AddRange(args);

			var hasError = !ProjectDefaultAttribute.IsNonProject(projectID)
				&& (typeof(TWhere) == typeof(BqlNone)
				? DoExpiredDocumentsExist<
					Where<ComplianceDocument.projectID, Equal<Required<ComplianceDocument.projectID>>>
					>(projectID)
				: DoExpiredDocumentsExist<
					Where<ComplianceDocument.projectID, Equal<Required<ComplianceDocument.projectID>>,
						And<TWhere>>
					>(values.ToArray()));

			RaiseOrClearExceptionForRelatedField<TEntityField>(
				graph.Caches<TEntity>(),
				entity,
				hasError,
				PXErrorLevel.Warning);
			return hasError;
		}

		public void ValidateRelatedRow<TEntity, THasExpiredDocumentsField>(TEntity entity, bool rowHasExpiredCompliance)
			where TEntity : class, IBqlTable, new()
			where THasExpiredDocumentsField : IBqlField
		{
			var cache = graph.Caches<TEntity>();
			cache.SetValue<THasExpiredDocumentsField>(entity, rowHasExpiredCompliance);
			RaiseOrClearExceptionForRelatedField<THasExpiredDocumentsField>(cache, entity, rowHasExpiredCompliance,
				PXErrorLevel.RowWarning);
		}

		private static void RaiseOrClearExceptionForRelatedField<TField>(
			PXCache cache, object entity, bool validationErrorNeeded, PXErrorLevel errorLevel)
			where TField : IBqlField
		{
			if (validationErrorNeeded)
			{
				RaiseCorrectExceptionForRelatedField<TField>(
					cache, entity, ComplianceMessages.ExpiredComplianceMessage, errorLevel);
			}
			else
			{
				cache.ClearFieldErrorIfExists<TField>(entity, ComplianceMessages.ExpiredComplianceMessage);
			}
		}

		private bool DoExpiredDocumentsExist<TWhere>(params object[] args)
			where TWhere : IBqlWhere, new()
		{
			var select = new PXSelect<ComplianceDocument,
				Where<ComplianceDocument.expirationDate,
					Less<Required<ComplianceDocument.expirationDate>>>>(graph);
			select.WhereAnd<TWhere>();

			var values = new List<object> { graph.Accessinfo.BusinessDate };
			values.AddRange(args);
			return select.Select(values.ToArray()).Any();
		}

		private static void RaiseCorrectExceptionForRelatedField<TField>(PXCache cache, object entity,
			string errorMessage, PXErrorLevel errorLevel)
			where TField : IBqlField
		{
			var previousErrorMessage = PXUIFieldAttribute.GetError<TField>(cache, entity);
			if (previousErrorMessage == null)
			{
				RaiseExceptionForRelatedField<TField>(cache, entity, errorMessage, errorLevel);
			}
		}

		private static void RaiseExceptionForRelatedField<TField>(PXCache cache, object entity, string errorMessage,
			PXErrorLevel errorLevel)
			where TField : IBqlField
		{
			var exception = new PXSetPropertyException<TField>(errorMessage, errorLevel);
			cache.RaiseExceptionHandling<TField>(entity, cache.GetValue<TField>(entity), exception);
		}

		private void ValidateExpirationDateOnFieldSelecting(ComplianceDocument document, PXCache documentsCache)
		{
			if (document != null && document.ExpirationDate < graph.Accessinfo.BusinessDate)
			{
				RaiseComplianceDocumentIsExpiredException(documentsCache, document, document.ExpirationDate);
			}
		}

		private void ValidateExpirationDateOnFieldVerifying(ComplianceDocument document, PXCache documentsCache,
			DateTime? expirationDate)
		{
			documentsCache.ClearItemAttributes();
			if (expirationDate != null && expirationDate < graph.Accessinfo.BusinessDate)
			{
				RaiseComplianceDocumentIsExpiredException(documentsCache, document, expirationDate);
			}
		}

		private static void RaiseComplianceDocumentIsExpiredException(PXCache cache, ComplianceDocument document,
			DateTime? expirationDate)
		{
			RaiseSingleIsExpiredException<ComplianceDocument.expirationDate>(
				cache, document, expirationDate, PXErrorLevel.Warning);
			RaiseSingleIsExpiredException<ComplianceDocument.isExpired>(
				cache, document, document.IsExpired, PXErrorLevel.RowWarning);
		}

		private static void RaiseSingleIsExpiredException<TField>(PXCache cache, ComplianceDocument document,
			object fieldValue, PXErrorLevel errorLevel)
			where TField : IBqlField
		{
			var exception = new PXSetPropertyException<TField>(
				ComplianceMessages.ComplianceDocumentIsExpiredMessage, errorLevel);
			cache.RaiseExceptionHandling<TField>(document, fieldValue, exception);
		}
	}
}
