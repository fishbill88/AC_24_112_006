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
using PX.Objects.CA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Objects.Localizations.CA
{
	public class EFTExportMethod : ACHExportMethod
	{
		public class ListAttribute : PXStringListAttribute, IPXFieldSelectingSubscriber
		{
			public ListAttribute() : base(_allowedValues, _allowedLabes)
			{ }

			public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
			{
				var pm = (PaymentMethod)e.Row;


				if (pm?.DirectDepositFileFormat == EFTDirectDepositType.Code)
				{
					e.ReturnState = (PXStringState)PXStringState.CreateInstance(e.ReturnState, null, null, null, null, null, null, _allowedValues, _customAllowedLabes, null, null, null);
				}
				else
				{
					e.ReturnState = (PXStringState)PXStringState.CreateInstance(e.ReturnState, null, null, null, null, null, null, _allowedValues, _allowedLabes, null, null, null);
				}
			}
		}

		protected static string[] _customAllowedLabes = new string[] { "Export Scenario", "Canadian EFT Plug-In", };
	}
}
