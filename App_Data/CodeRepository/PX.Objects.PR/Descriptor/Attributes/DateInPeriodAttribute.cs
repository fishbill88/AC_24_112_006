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
using PX.Common;
using PX.Data;

namespace PX.Objects.PR
{
	public class DateInPeriodAttribute : PXDBDateAttribute, IPXFieldVerifyingSubscriber, IPXFieldDefaultingSubscriber
	{
		protected readonly Type _Parent;
		protected readonly Type _PeriodStartDate;
		protected readonly Type _PeriodEndDate;
		protected readonly string _ExistingRowsViewName;

		public DateInPeriodAttribute(Type parent, Type periodStartDate, Type periodEndDate, string existingRowsViewName)
		{
			_Parent = parent;
			_PeriodStartDate = periodStartDate;
			_PeriodEndDate = periodEndDate;
			_ExistingRowsViewName = existingRowsViewName;
		}

		public override void CacheAttached(PXCache sender)
		{
			SetMinAndMaxDate(sender);

			if (sender.Graph.Caches.TryGetValue(_Parent, out PXCache parentCache))
			{
				parentCache.Graph.FieldUpdated.AddHandler(parentCache.GetItemType(), _PeriodStartDate.Name, (cache, e) => SetMinAndMaxDate(cache));
				parentCache.Graph.FieldUpdated.AddHandler(parentCache.GetItemType(), _PeriodEndDate.Name, (cache, e) => SetMinAndMaxDate(cache));
			}

			base.CacheAttached(sender);
		}

		private void SetMinAndMaxDate(PXCache sender)
		{
			if (!sender.Graph.Caches.TryGetValue(_Parent, out PXCache parentCache) || parentCache?.Current == null)
				return;

			DateTime? startDate = parentCache.GetValue(parentCache.Current, _PeriodStartDate.Name) as DateTime?;
			DateTime? endDate = parentCache.GetValue(parentCache.Current, _PeriodEndDate.Name) as DateTime?;

			if (startDate != null && endDate != null)
			{
				_MinValue = startDate.Value;
				_MaxValue = endDate.Value;
			}
		}
		
		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.Row == null)
				return;

			DateTime minDate = _MinValue.GetValueOrDefault();
			DateTime maxDate = _MaxValue.GetValueOrDefault();
			DateTime? newDate = e.NewValue as DateTime?;
			if (newDate == null || newDate < minDate || newDate > maxDate)
			{
				if (!sender.Graph.IsCopyPasteContext)
					e.NewValue = null;
				string exceptionMessage = sender.Graph.IsCopyPasteContext ?
					GetIncorrectDateMessage(minDate, maxDate, newDate) :
					GetIncorrectDateMessage();
				PXUIFieldAttribute.SetError(sender, e.Row, _FieldName, exceptionMessage);
			}
		}

		public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (e.Row == null || _MinValue == null || sender.Graph.IsCopyPasteContext)
				return;
			
			if (!sender.Graph.Views.TryGetValue(_ExistingRowsViewName, out PXView existingRowsView) || existingRowsView.SelectSingle() != null)
				return;

			e.NewValue = _MinValue.Value;
		}

		public static string GetIncorrectDateMessage(DateTime minDate, DateTime maxDate, DateTime? enteredDate)
		{
			return string.Format(Messages.IncorrectPeriodDateWithEnteredDate, GetShortDateString(minDate), GetShortDateString(maxDate), GetShortDateString(enteredDate.GetValueOrDefault()));
		}

		public static string GetIncorrectDateMessage()
		{
			return string.Format(Messages.IncorrectPeriodDate);
		}

		private static string GetShortDateString(DateTime date)
		{
			try
			{
				return date.ToString(LocaleInfo.GetCulture().DateTimeFormat.ShortDatePattern);
			}
			catch
			{
				return date.ToShortDateString();
			}
		}
	}

	public class DateInPaymentPeriodAttribute : DateInPeriodAttribute
	{
		public DateInPaymentPeriodAttribute(Type parent, Type periodStartDate, Type periodEndDate, string existingRowsViewName)
			: base(parent, periodStartDate, periodEndDate, existingRowsViewName)
		{
		}

		public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (sender.Graph.Caches.TryGetValue(_Parent, out PXCache parentCache) && parentCache?.Current != null)
			{
				var payment = (PRPayment) parentCache?.Current;
				if (payment.DocType == PayrollType.VoidCheck)
				{
					return;
				}
			}

			base.FieldVerifying(sender, e);
		}
	}

}
