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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using CalendarIdSearch = PX.Data.BQL.Fluent.SelectFrom<PX.Objects.CR.CRCaseClass>
	.Where<PX.Objects.CR.CRCaseClass.caseClassID.IsEqual<PX.Objects.CR.CRClassSeverityTime.caseClassID.FromCurrent>>
	.SearchFor<PX.Objects.CR.CRCaseClass.calendarID>;

namespace PX.Objects.CR
{
	[PXCacheName(Messages.TimeReactionBySeverity)]
	public partial class CRClassSeverityTime : PXBqlTable, PX.Data.IBqlTable
	{
		#region Keys

		public class PK : PrimaryKeyOf<CRClassSeverityTime>.By<caseClassID, severity>
		{
			public static CRClassSeverityTime Find(PXGraph graph, string caseClassID, string severity, PKFindOptions options = PKFindOptions.None) => FindBy(graph, caseClassID, severity, options);
		}
		public static class FK
		{
			public class CaseClass : CRCaseClass.PK.ForeignKeyOf<CRClassSeverityTime>.By<caseClassID> { }
		}

		#endregion

		#region CaseClassID

		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Case Class ID")]
		[PXSelector(typeof(CRCaseClass.caseClassID))]
		public virtual string CaseClassID { get; set; }

		public abstract class caseClassID : BqlString.Field<caseClassID> { }

		#endregion

		#region Severity

		[PXDBString(1, IsKey = true, IsFixed = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Severity")]
		[CRCaseSeverity(BqlField = typeof(CRCase.severity))]
		public virtual string Severity { get; set; }

		public abstract class severity : BqlString.Field<severity> { }

		#endregion


		// Initial Response

		#region TrackInitialResponseTime

		/// <summary>
		/// This field provides an ability to set <see cref="InitialResponseTimeTarget">Target Initial Response Time</see>
		/// and <see cref="InitialResponseGracePeriod">Initial Response Extension</see>.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enable")]
		public virtual bool? TrackInitialResponseTime { get; set; }

		public abstract class trackInitialResponseTime : BqlBool.Field<trackInitialResponseTime> { }

		#endregion

		#region InitialResponseTimeTarget

		/// <summary>
		/// The timeframe within which the service provider is obliged to provide a response to the initial communication on a particular service case by a service user.
		/// </summary>
		[WorkTime(typeof(CalendarIdSearch), AvailabilityFieldName = nameof(TrackInitialResponseTime))]
		[PXUIField(DisplayName = "Target Initial Response Time")]
		public virtual int? InitialResponseTimeTarget { get; set; }

		public abstract class initialResponseTimeTarget : BqlInt.Field<initialResponseTimeTarget> { }

		#endregion

		#region InitialResponseGracePeriod

		/// <summary>
		/// This field defines the <see cref="InitialResponseTimeTarget">Target Initial Response Time</see> when a severity was changed in a case
		/// and the target for the new severity is less than the actual time passed since the time counter started.
		/// The grace period gives the service provider enough time to react to an increased case severity.
		/// </summary>
		[WorkTime(typeof(CalendarIdSearch), AvailabilityFieldName = nameof(TrackInitialResponseTime))]
		[PXUIField(DisplayName = "Initial Response Extension")]
		[PXFormula(typeof(IIf<Where<IsNull<Current<initialResponseGracePeriod>, int0>, Equal<int0>>, initialResponseTimeTarget, initialResponseGracePeriod>))]
		public virtual int? InitialResponseGracePeriod { get; set; }

		public abstract class initialResponseGracePeriod : BqlInt.Field<initialResponseGracePeriod> { }

		#endregion


		// Response

		#region TrackResponseTime

		/// <summary>
		/// This field provides an ability to set <see cref="ResponseTimeTarget">Target Response Time</see> and <see cref="ResponseGracePeriod">Response Extension</see>.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enable")]
		public virtual bool? TrackResponseTime { get; set; }

		public abstract class trackResponseTime : BqlBool.Field<trackResponseTime> { }

		#endregion

		#region ResponseTimeTarget

		/// <summary>
		/// The timeframe within which the service provider is obliged to provide a response to the communication by a service user according.
		/// </summary>
		[WorkTime(typeof(CalendarIdSearch), AvailabilityFieldName = nameof(TrackResponseTime))]
		[PXUIField(DisplayName = "Target Response Time")]
		public virtual int? ResponseTimeTarget { get; set; }

		public abstract class responseTimeTarget : BqlInt.Field<responseTimeTarget> { }

		#endregion

		#region ResponseGracePeriod

		/// <summary>
		/// These fields define the <see cref="ResponseTimeTarget">Target Response Time</see> when a severity was changed in a case
		/// and the target for the new severity is less than the actual time passed since the time counter started.
		/// The grace period gives the service provider enough time to react to an increased case severity.
		/// </summary>
		[WorkTime(typeof(CalendarIdSearch), AvailabilityFieldName = nameof(TrackResponseTime))]
		[PXUIField(DisplayName = "Response Extension")]
		[PXFormula(typeof(IIf<Where<IsNull<Current<responseGracePeriod>, int0>, Equal<int0>>, responseTimeTarget, responseGracePeriod>))]
		public virtual int? ResponseGracePeriod { get; set; }

		public abstract class responseGracePeriod : BqlInt.Field<responseGracePeriod> { }

		#endregion


		// Resolution

		#region TrackResolutionTime

		/// <summary>
		/// This field provides an ability to set <see cref="ResolutionTimeTarget">Target Resolution Time</see> and <see cref="ResolutionGracePeriod">Resolution Extension</see>.
		/// </summary>
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enable")]
		public virtual bool? TrackResolutionTime { get; set; }

		public abstract class trackResolutionTime : BqlBool.Field<trackResolutionTime> { }

		#endregion

		#region ResolutionTimeTarget

		/// <summary>
		/// The timeframe within which the service provider is obliged to provide a reply to the service user that contains the resolution for the inquiry.
		/// </summary>
		[WorkTime(typeof(CalendarIdSearch), AvailabilityFieldName = nameof(TrackResolutionTime))]
		[PXUIField(DisplayName = "Target Resolution Time")]
		public virtual int? ResolutionTimeTarget { get; set; }

		public abstract class resolutionTimeTarget : BqlInt.Field<resolutionTimeTarget> { }

		#endregion

		#region ResolutionGracePeriod

		/// <summary>
		/// This field defines the <see cref="ResolutionTimeTarget">Target Resolution Time</see> when a severity was changed in a case
		/// and the target for the new severity is less than the actual time passed since the time counter started.
		/// The grace period gives the service provider enough time to react to an increased case severity.
		/// </summary>
		[WorkTime(typeof(CalendarIdSearch), AvailabilityFieldName = nameof(TrackResolutionTime))]
		[PXUIField(DisplayName = "Resolution Extension")]
		[PXFormula(typeof(IIf<Where<IsNull<Current<resolutionGracePeriod>, int0>, Equal<int0>>, resolutionTimeTarget, resolutionGracePeriod>))]
		public virtual int? ResolutionGracePeriod { get; set; }

		public abstract class resolutionGracePeriod : BqlInt.Field<resolutionGracePeriod> { }

		#endregion


		// system

		#region tstamp

		[PXDBTimestamp()]
		public virtual Byte[] tstamp { get; set; }

		public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }

		#endregion

		#region CreatedByID

		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID { get; set; }

		public abstract class createdByID : BqlGuid.Field<createdByID> { }

		#endregion

		#region CreatedByScreenID

		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID { get; set; }

		public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }

		#endregion

		#region CreatedDateTime

		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime { get; set; }

		public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }

		#endregion

		#region LastModifiedByID

		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }

		public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }

		#endregion

		#region LastModifiedByScreenID

		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID { get; set; }

		public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }

		#endregion

		#region LastModifiedDateTime

		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime { get; set; }

		public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }

		#endregion
	}
}
