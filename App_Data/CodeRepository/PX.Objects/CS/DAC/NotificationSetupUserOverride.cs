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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.SM;
using System;

namespace PX.Objects.CS
{
	[PXCacheName("User's Notification Setup")]
	public class NotificationSetupUserOverride : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<NotificationSetupUserOverride>.By<userID, setupID>
		{
			public static NotificationSetupUserOverride Find(PXGraph graph, Guid? userID, Guid? setupID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, userID, setupID, options);
		}

		public static class FK
		{
			public class DefaultSetup : NotificationSetup.PK.ForeignKeyOf<NotificationSetupUserOverride>.By<setupID> { }
			public class User : Users.PK.ForeignKeyOf<NotificationSetupUserOverride>.By<userID> { }
		}
		#endregion

		#region UserID
		[PXDBGuid(IsKey = true)]
		[PXDefault(typeof(Search<Users.pKID, Where<Users.pKID, Equal<Current<AccessInfo.userID>>>>))]
		public virtual Guid? UserID { get; set; }
		public abstract class userID : PX.Data.BQL.BqlGuid.Field<userID> { }
		#endregion
		#region SetupID
		[PXDBGuid(IsKey = true)]
		[PXSelector(typeof(NotificationSetup.setupID), SubstituteKey = typeof(NotificationSetup.notificationCD))]
		[PXUIField(DisplayName = "Mailing ID")]
		public virtual Guid? SetupID { get; set; }
		public abstract class setupID : PX.Data.BQL.BqlGuid.Field<setupID> { }
		#endregion
		#region ReportID
		[PXString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Report ID", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Selector<setupID, NotificationSetup.reportID>))]
		public virtual String ReportID { get; set; }
		public abstract class reportID : PX.Data.BQL.BqlString.Field<reportID> { }
		#endregion
		#region DefaultPrinterID
		[PXDefault]
		[PXPrinterSelector(DisplayName = "Default Printer", Visibility = PXUIVisibility.SelectorVisible)]
		[PXForeignReference(typeof(Field<defaultPrinterID>.IsRelatedTo<SMPrinter.printerID>))]
		public virtual Guid? DefaultPrinterID { get; set; }
		public abstract class defaultPrinterID : PX.Data.BQL.BqlGuid.Field<defaultPrinterID> { }
		#endregion

		#region Active
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active { get; set; }
		public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
		#endregion

		#region ShipVia
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Ship Via")]
		[PXUIEnabled(typeof(Where<Selector<setupID, NotificationSetup.module>, Equal<CA.PXModule.so>>))]
		[PXDefault(typeof(Search<NotificationSetup.shipVia, Where<NotificationSetup.setupID.IsEqual<setupID.FromCurrent>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(Search<Carrier.carrierID>), DescriptionField = typeof(Carrier.description), CacheGlobal = true)]
		public virtual String ShipVia { get; set; }
		public abstract class shipVia : PX.Data.BQL.BqlString.Field<shipVia> { }
		#endregion

		#region tstamp
		[PXDBTimestamp]
		public virtual Byte[] tstamp { get; set; }
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		#endregion
		#region CreatedByID
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID { get; set; }
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		#endregion
		#region CreatedByScreenID
		[PXDBCreatedByScreenID]
		public virtual String CreatedByScreenID { get; set; }
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		#endregion
		#region CreatedDateTime
		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime { get; set; }
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		#endregion
		#region LastModifiedByID
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		#endregion
		#region LastModifiedByScreenID
		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID { get; set; }
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		#endregion
		#region LastModifiedDateTime
		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		#endregion
	}
}