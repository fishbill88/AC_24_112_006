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
using PX.Objects.PM;
using System;

namespace PX.Objects.PR
{
	public class ProjectTaskNoDefaultAttribute : ProjectTaskAttribute
	{
		public ProjectTaskNoDefaultAttribute(Type projectField) : base(projectField)
		{
			AllowNull = true;
		}

		protected override void OnProjectUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			object taskCD = (sender.GetValuePending(e.Row, _FieldName) as string) ?? (sender.GetValueExt(e.Row, _FieldName) as PXSegmentedState).Value;
			if (taskCD != null && taskCD != PXCache.NotSetValue)
			{
				base.OnProjectUpdated(sender, e);
			}
		}
	}
}
