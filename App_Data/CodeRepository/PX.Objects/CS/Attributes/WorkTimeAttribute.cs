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

#nullable enable
using PX;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CR.Extensions;
using PX.Objects.CS.Services.WorkTimeCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PX.Objects.CS
{
	[PXInt]
	[PXDBInt]
	[PXDefault(MessagesNoPrefix.Disabled)] // PXDefault is for UI only. FieldUpdating will handle string->int properly.
	public class WorkTimeAttribute : PXEntityAttribute, IPXFieldSelectingSubscriber, IPXFieldUpdatingSubscriber
	{
		public string? AvailabilityFieldName { get; set; }

		public IBqlSearch SearchCalendarId { get; }
		public BqlCommand SearchCalendarIdBqlCommand { get; }
		
		protected virtual string DisplayFormat => "{0:000}{1:00}{2:00}";
		protected virtual int MaskLength { get; } 
		protected virtual Regex ParsePattern { get; } = new Regex(@"(?<days>[\d| ]{1,3})(?<hours>[\d| ]{0,2})(?<minutes>[\d| ]{0,2})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

		public WorkTimeAttribute(Type calendarIdSearch)
		{
			// ReSharper disable VirtualMemberCallInConstructor
			SearchCalendarId           = GetCalendarIdSearchCommand(calendarIdSearch);
			SearchCalendarIdBqlCommand = (BqlCommand) SearchCalendarId;
			MaskLength = MessagesNoPrefix.WorkTimeMask.Count(c => c.IsIn('#', '0'));
			// ReSharper enable VirtualMemberCallInConstructor
		}

		protected virtual IBqlSearch GetCalendarIdSearchCommand(Type calendarIdSearch)
		{
			return calendarIdSearch switch
			{
				null => throw new ArgumentNullException(nameof(calendarIdSearch)),

				_ when typeof(IBqlSearch).IsAssignableFrom(calendarIdSearch)
					=> (IBqlSearch) BqlCommand.CreateInstance(calendarIdSearch),

				_ when calendarIdSearch.IsNested && typeof(IBqlField).IsAssignableFrom(calendarIdSearch)
					=> (IBqlSearch) BqlCommand.CreateInstance(typeof(Search<>), calendarIdSearch),

				_ => throw new PXArgumentException(nameof(calendarIdSearch), ErrorMessages.CantCreateForeignKeyReference, calendarIdSearch)
			};
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null && IsAvailable(sender, e.Row))
			{
				if (e.ReturnValue is int intValue
					&& TryGetWorkTimeCalculator(sender.Graph, e.Row) is { IsValid: true } calculator)
				{
					var span         = TimeSpan.FromMinutes(intValue);
					var workTimeSpan = calculator.ToWorkTimeSpan(span);

					e.ReturnState = GetFieldState(value: string.Format(DisplayFormat, workTimeSpan.RoundWorkdays, workTimeSpan.RoundHours, workTimeSpan.RoundMinutes), isAvailable: true);
				}
				else
				{
					e.ReturnState = GetFieldState(isAvailable: true);
				}
			}
			else
			{
				e.ReturnState = GetFieldState(isAvailable: false);
			}
		}

		public virtual void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			e.NewValue = e.NewValue switch
			{
				int value => value,
				string value
					when TryParseWorkTimeInfo(value) is { } workTimeSpan
					  && TryGetWorkTimeCalculator(sender.Graph, e.Row) is { IsValid: true } calculator
					=> (int) calculator.ToWorkTimeSpan(workTimeSpan).TotalMinutes,
				_ => 0,
			};
		}

		protected virtual WorkTimeInfo? TryParseWorkTimeInfo(string value)
		{
			if (ParsePattern.Match(value) is { Success: true } success)
			{
				var days = success.Groups["days"].Value.Replace(' ', '0');
				var hours = success.Groups["hours"].Value.Replace(' ', '0');
				var minutes = success.Groups["minutes"].Value.Replace(' ', '0');

				return new WorkTimeInfo(
					string.IsNullOrEmpty(days) ? 0 : int.Parse(days),
					string.IsNullOrEmpty(hours) ? 0 : int.Parse(hours),
					string.IsNullOrEmpty(minutes) ? 0 : int.Parse(minutes)
				);
			}

			return null;
		}

		protected virtual IWorkTimeCalculator? TryGetWorkTimeCalculator(PXGraph graph, object? row)
		{
			if (TryGetCalendarId(graph, row) is { } calendarId)
				return WorkTimeCalculatorProvider.GetWorkTimeCalculator(calendarId);

			return null;
		}

		protected virtual string? TryGetCalendarId(PXGraph graph, object? row)
		{
			if (row == null)
				return null;

			var view = graph.TypedViews.GetView(SearchCalendarIdBqlCommand, false);
			var obj  = view.SelectSingleBound(new[] { row });
			if (obj == null)
				return null;

			var itemType = BqlCommand.GetItemType(SearchCalendarId.GetField());
			if (itemType == null)
				return null;

			var item = PXResult.Unwrap(obj, itemType);
			if (item == null)
				return null;

			var cache = graph.Caches[itemType];
			var field = cache.GetField(SearchCalendarId.GetField());

			return cache.GetValue(item, field) as string;
		}

		protected virtual bool IsAvailable(PXCache sender, object row)
		{
			return AvailabilityFieldName is null || sender.GetValue(row, AvailabilityFieldName) is true;
		}

		protected virtual PXStringState GetFieldState(object? value = null, bool isAvailable = true)
		{
			var newState = (PXStringState)PXStringState.CreateInstance(
				value, length: isAvailable is true ? MaskLength : MessagesNoPrefix.Disabled.Length, isUnicode: null, fieldName: FieldName,
				isKey: IsKey, null, inputMask: PXMessages.LocalizeNoPrefix(MessagesNoPrefix.WorkTimeMask), allowedValues: null,
				allowedLabels: null, exclusiveValues: null, defaultValue: null, neutralLabels: null);

			if (isAvailable is false)
			{
				newState.Enabled = false;
				newState.InputMask = string.Empty;
				newState.Value = PXMessages.LocalizeNoPrefix(MessagesNoPrefix.Disabled);
			}

			return newState;
		}
	}
}
