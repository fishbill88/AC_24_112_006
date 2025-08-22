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
using System;
using PX.Data;

namespace PX.Objects.CS.Services.WorkTimeCalculation
{
	[PXInternalUseOnly]
	public interface IWorkTimeCalculator
	{
		/// <summary>
		/// Indicates whether calendar is valid and can be used for calculations.
		/// This might be due to missing workdays or 0 workday hours or other reasons.
		/// Calls to methods of invalid calendar will throw <see cref="PXInvalidOperationException"/>.
		/// </summary>
		bool IsValid { get; }

		/// <summary>
		/// Validates the calendar and throws <see cref="PXInvalidOperationException"/> with the details if it is invalid.
		/// Relies on <see cref="IsValid"/> property.
		/// </summary>
		void Validate();

		/// <exception cref="PXInvalidOperationException">
		/// Thrown when <see cref="IsValid"/> is false.
		/// </exception>
		WorkTimeSpan ToWorkTimeSpan(TimeSpan timeSpan);

		/// <exception cref="PXInvalidOperationException">
		/// Thrown when <see cref="IsValid"/> is false.
		/// </exception>
		WorkTimeSpan ToWorkTimeSpan(WorkTimeInfo workTimeInfo);

		/// <summary>
		/// Add work time to <paramref name="startDateTime"/> using underlying calendar and return new <see cref="DateTimeInfo"/> with result.
		/// </summary>
		/// <param name="startDateTime"></param>
		/// <param name="workTimeDiff"></param>
		/// <returns>Date time info.</returns>
		/// <remarks>
		/// Currently time subtraction is not supported.
		/// </remarks>
		/// <exception cref="PXNotSupportedException">
		/// Thrown when <paramref name="workTimeDiff"/> has negative value.
		/// </exception>
		/// <exception cref="PXInvalidOperationException">
		/// Thrown when <see cref="IsValid"/> is false.
		/// </exception>
		DateTimeInfo AddWorkTime(DateTimeInfo startDateTime, WorkTimeSpan workTimeDiff);
	}
}
