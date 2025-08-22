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
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

namespace PX.Objects.EP
{
	[PXDBInt]
	[PXInt]
	[PXUIField(DisplayName = "Assignment Map")]
	public class AssignmentMapAttribute : ApprovalAssignmentMapAttribute
	{
		public AssignmentMapAttribute(Type entityType) : base(entityType, true) { }
	}

	[PXDBInt]
	[PXInt]
	[PXUIField(DisplayName = "Approval Map")]
	public class ApprovalMapAttribute : ApprovalAssignmentMapAttribute
	{
		public ApprovalMapAttribute(Type entityType) : base(entityType, false) { }
	}

	public abstract class ApprovalAssignmentMapAttribute : PXEntityAttribute
	{
		#region ctor

		protected ApprovalAssignmentMapAttribute(Type entityType, bool assignment, Type customSearchField = null, Type customSearchQuery = null, Type[] fieldList = null, PXSelectorMode selectorMode = PXSelectorMode.DisplayModeText)
		{
			PXSelectorAttribute attr =
				new PXSelectorAttribute(customSearchQuery ?? CreateSelect(entityType, assignment, customSearchField),
					fieldList: fieldList ?? new Type[]
					{
						typeof(EPAssignmentMap.name),
					})
				{
					DescriptionField = typeof(EPAssignmentMap.name),
					SelectorMode = selectorMode,
					Filterable = true,
					DirtyRead = true
				};

			_Attributes.Add(attr);

			_SelAttrIndex = _Attributes.Count - 1;
		}

		protected virtual Type CreateSelect(Type entityType, bool assignment, Type customSearchField)
		{
			return BqlTemplate.OfCommand<
					SelectFrom<
						EPAssignmentMap>
					.Where<
						EPAssignmentMap.entityType.IsEqual<BqlPlaceholder.A.AsOperand>
						.And<EPAssignmentMap.mapType.IsIn<EPMapType.legacy, BqlPlaceholder.B.AsOperand>>>
					.SearchFor<BqlPlaceholder.C>>
				.Replace<BqlPlaceholder.A>(entityType)
				.Replace<BqlPlaceholder.B>(assignment ? typeof(EPMapType.assignment) : typeof(EPMapType.approval))
				.Replace<BqlPlaceholder.C>(customSearchField ?? typeof(EPAssignmentMap.assignmentMapID))
				.ToType();
		}

		#endregion
	}
}
