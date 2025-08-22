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

namespace PX.Objects.Common
{
	public static class UIState
	{
		public static void RaiseOrHideError<T>(PXCache cache, object row, bool isIncorrect, string message, PXErrorLevel errorLevel, params object[] parameters)
			where T : IBqlField
		{
			if (isIncorrect)
			{
				cache.RaiseExceptionHandling<T>(row, PXFieldState.UnwrapValue(cache.GetValueExt<T>(row)), new PXSetPropertyException(message, errorLevel, parameters));
			}
			else
			{
				cache.RaiseExceptionHandling<T>(row, PXFieldState.UnwrapValue(cache.GetValueExt<T>(row)), null);
			}
		}

		public static void RaiseOrHideErrorByErrorLevelPriority<T>(PXCache cache, object row, bool isIncorrect, string message, PXErrorLevel errorLevel, params object[] parameters)
			where T : IBqlField
		{
			if (IsHigherErrorLevelExist<T>(cache, row, errorLevel))
			{
				return;
			}

			RaiseOrHideError<T>(cache, row, isIncorrect, message, errorLevel, parameters);
		}

		public static bool IsHigherErrorLevelExist<T>(PXCache cache, object row, PXErrorLevel errorLevel)
			where T : IBqlField
		{
			PXFieldState state = (PXFieldState)cache.GetStateExt<T>(row);
			if (state?.ErrorLevel > errorLevel)
			{
				return true;
			}

			return false;
		}
	}
}
