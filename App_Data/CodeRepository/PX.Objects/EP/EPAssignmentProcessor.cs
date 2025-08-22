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
using PX.Data;

namespace PX.Objects.EP
{
	public class EPAssignmentProcessor<Table> : PXGraph<EPAssignmentProcessor<Table>>
		where Table : class, PX.Data.EP.IAssign, IBqlTable, new()
	{
		private readonly PXGraph _Graph;

		public EPAssignmentProcessor(PXGraph graph)
			: this()
		{
			_Graph = graph;
		}

		public EPAssignmentProcessor()
		{
			_Graph = this;
		}

		public virtual bool Assign(Table item, int? assignmentMapID)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			if (assignmentMapID < 0)
				throw new ArgumentOutOfRangeException(nameof(assignmentMapID));

			EPAssignmentMap map =
			PXSelect<EPAssignmentMap, Where<EPAssignmentMap.assignmentMapID, Equal<Required<EPAssignmentMap.assignmentMapID>>>>
				.SelectWindowed(this, 0, 1, assignmentMapID);
			if (map == null) return false;

			ApproveInfo info;

			switch (map.MapType)
			{
				case EPMapType.Legacy:
					try
					{
						info = new EPAssignmentProcessHelper<Table>(_Graph).Assign(item, map, false).FirstOrDefault();
						if (info != null)
						{
							item.OwnerID = info.OwnerID;
							item.WorkgroupID = info.WorkgroupID;

							return true;
						}
					}
					catch
					{
						return false;
					}
					return false;

				case EPMapType.Assignment:
					try
					{
						info = new EPAssignmentHelper<Table>(_Graph).Assign(item, map, false, 0).FirstOrDefault();
						if (info != null)
						{
							item.OwnerID = info.OwnerID;
							item.WorkgroupID = info.WorkgroupID;

							return true;
						}
					}
					catch(Exception ex)
					{
						PXTrace.WriteInformation(ex);
						return false;
					}
					return false;

				case EPMapType.Approval:
					throw new ArgumentException(nameof(assignmentMapID));

				default:
					return false;
			}
		}

		public virtual IEnumerable<ApproveInfo> Approve(Table item, EPAssignmentMap map, int? currentStepSequence)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));

			if (map == null)
				throw new ArgumentOutOfRangeException(nameof(map));
			

			switch (map.MapType)
			{
				case EPMapType.Legacy:
					return new EPAssignmentProcessHelper<Table>(_Graph).Assign(item, map, false);

				case EPMapType.Assignment:
					throw new ArgumentException(Messages.AssignmentNotApprovalMap);

				case EPMapType.Approval:
					return new EPAssignmentHelper<Table>(_Graph).Assign(item, map, true, currentStepSequence);

				default:
					return null;
			}
		}
	}
}
