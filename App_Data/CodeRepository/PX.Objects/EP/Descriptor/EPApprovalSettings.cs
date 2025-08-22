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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using PX.Api;
using PX.Common;
using PX.Data;
using PX.Data.BQL;

namespace PX.Objects.EP
{
	public class EPApprovalSettings<TSetupApproval, TSetupDocTypeField, TAllDocTypes, THoldStatus, TPendingApprovalStatus, TRejectedStatus> : IPrefetchable
		where TSetupApproval : IBqlTable, IAssignedMap
		where TSetupDocTypeField : IBqlField, IImplement<IBqlString>
		where THoldStatus : IBqlOperand, IConstant<string>, IImplement<IBqlString>
		where TPendingApprovalStatus : IBqlOperand, IConstant<string>, IImplement<IBqlString>
		where TRejectedStatus : IBqlOperand, IConstant<string>, IImplement<IBqlString>
	{
		public static ImmutableHashSet<string> ApprovableDocTypes => Slot._approvableDocTypes;

		public class IsApprovable<TDocTypeField> : CustomPredicate
			where TDocTypeField : IBqlField, IImplement<IBqlString>
		{
			public IsApprovable() : base(Slot._docTypeToCondition.GetOrAdd(typeof(TDocTypeField), key => Slot.IsApprovableCondition(key))) { }
		}

		public class IsDocumentApprovable<TDocTypeField, TStatusField> : CustomPredicate
			where TDocTypeField : IBqlField, IImplement<IBqlString>
			where TStatusField : IBqlField, IImplement<IBqlString>
		{
			public IsDocumentApprovable() : base(new Where<TStatusField, In3<TPendingApprovalStatus, TRejectedStatus>, Or<IsApprovable<TDocTypeField>>>()) { }
		}

		public class IsDocumentLockedByApproval<TDocTypeField, TStatusField> : CustomPredicate
			where TDocTypeField : IBqlField, IImplement<IBqlString>
			where TStatusField : IBqlField, IImplement<IBqlString>
		{
			public IsDocumentLockedByApproval() : base(new Where<TStatusField, NotEqual<THoldStatus>, And<IsApprovable<TDocTypeField>>>()) { }
		}

		#region Slot static
		private static readonly string SlotKey = typeof(EPApprovalSettings<TSetupApproval, TSetupDocTypeField, TAllDocTypes, THoldStatus, TPendingApprovalStatus, TRejectedStatus>).ToCodeString(preserveFullNames: true);

		private static EPApprovalSettings<TSetupApproval, TSetupDocTypeField, TAllDocTypes, THoldStatus, TPendingApprovalStatus, TRejectedStatus> Slot =>
			PXDatabase.GetSlot<EPApprovalSettings<TSetupApproval, TSetupDocTypeField, TAllDocTypes, THoldStatus, TPendingApprovalStatus, TRejectedStatus>>(SlotKey, typeof(TSetupApproval));

		private static readonly ImmutableDictionary<string, Type> DocTypeConstants =
			typeof(TAllDocTypes)
			.GetNestedTypes()
			.Where(t => typeof(IConstant).IsAssignableFrom(t))
			.Select(t =>
			(
				Type: t,
				Value: ((IConstant)Activator.CreateInstance(t)).Value.ToString()
			))
			.Distinct(p => p.Value)
			.ToImmutableDictionary(p => p.Value, p => p.Type);
		#endregion

		#region Slot instance
		private ImmutableHashSet<string> _approvableDocTypes = ImmutableHashSet<string>.Empty;
		private readonly ConcurrentDictionary<Type, IBqlUnary> _docTypeToCondition = new ConcurrentDictionary<Type, IBqlUnary>();

		void IPrefetchable.Prefetch()
		{
			if (PXAccess.FeatureInstalled<CS.FeaturesSet.approvalWorkflow>())
			{
				HashSet<string> hashDocTypes = new HashSet<string>();
				foreach (PXDataRecord rec in PXDatabase.SelectMulti<TSetupApproval>(
					new PXDataField<TSetupDocTypeField>(),
					new PXDataFieldValue(nameof(IAssignedMap.IsActive), true)))
				{
					hashDocTypes.Add(rec.GetString(0).Trim());
				}

				_approvableDocTypes = hashDocTypes.ToImmutableHashSet();
			}
			else
			{
				_approvableDocTypes = ImmutableHashSet<string>.Empty;
			}

			_docTypeToCondition.Clear();
		}

		private IBqlUnary IsApprovableCondition(Type docTypeField)
		{
			Type isApprovableCondition = null;

			foreach (string approvableDocType in _approvableDocTypes)
				if (DocTypeConstants.TryGetValue(approvableDocType, out var docTypeConstant))
					isApprovableCondition = isApprovableCondition == null
						? BqlCommand.Compose(typeof(Where<,>), docTypeField, typeof(Equal<>), docTypeConstant)
						: BqlCommand.Compose(typeof(Where<,,>), docTypeField, typeof(Equal<>), docTypeConstant, typeof(Or<>), isApprovableCondition);

			return isApprovableCondition == null
				? new Where<True, Equal<False>>()
				: (IBqlUnary)Activator.CreateInstance(BqlCommand.Compose(typeof(Where<>), isApprovableCondition));
		}
		#endregion
	}
}
