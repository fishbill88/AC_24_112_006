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

namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Data.ReferentialIntegrity.Attributes;
	using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLConsolSetup)]
	public partial class GLConsolSetup : PXBqlTable, PX.Data.IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<GLConsolSetup>.By<setupID>
		{
			public static GLConsolSetup Find(PXGraph graph, Int32? setupID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, setupID, options);
		}
		public static class FK
		{
			public class Branch : GL.Branch.PK.ForeignKeyOf<GLConsolSetup>.By<branchID> { }
			public class Ledger : GL.Ledger.PK.ForeignKeyOf<GLConsolSetup>.By<ledgerId> { }
		}
		#endregion

		#region Selected
		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        protected bool? _Selected = false;
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public bool? Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                _Selected = value;
            }
        }
        #endregion
		#region SetupID
		public abstract class setupID : PX.Data.BQL.BqlInt.Field<setupID> { }
		protected Int32? _SetupID;
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? SetupID
		{
			get
			{
				return this._SetupID;
			}
			set
			{
				this._SetupID = value;
			}
		}
		#endregion
		#region IsActive
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
		protected Boolean? _IsActive;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual Boolean? IsActive
		{
			get
			{
				return this._IsActive;
			}
			set
			{
				this._IsActive = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
		protected Int32? _BranchID;
		[Branch(DisplayName = "Consolidation Branch", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region LedgerId
		public abstract class ledgerId : PX.Data.BQL.BqlInt.Field<ledgerId> { }
		protected Int32? _LedgerId;
		[PXDBInt()]
		[PXDefault(typeof(Search<Branch.ledgerID, Where<Branch.branchID, Equal<Current<GLConsolSetup.branchID>>>>))]
		[PXUIField(DisplayName = "Consolidation Ledger", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search2<Ledger.ledgerID, LeftJoin<Branch, On<Branch.ledgerID, Equal<Ledger.ledgerID>>>, Where<Ledger.balanceType, NotEqual<BudgetLedger>, And<Where<Ledger.balanceType, NotEqual<LedgerBalanceType.actual>, Or<Branch.branchID, Equal<Current<GLConsolSetup.branchID>>>>>>>),
						SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual Int32? LedgerId
		{
			get
			{
				return this._LedgerId;
			}
			set
			{
				this._LedgerId = value;
			}
		}
		#endregion
		#region SegmentValue
		public abstract class segmentValue : PX.Data.BQL.BqlString.Field<segmentValue> { }
		protected String _SegmentValue;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Consolidation Segment Value", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Search<SegmentValue.value, Where<SegmentValue.dimensionID, 
			Equal<SubAccountAttribute.dimensionName>, And<SegmentValue.segmentID,Equal<Current<GLSetup.consolSegmentId>>>>>))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String SegmentValue
		{
			get
			{
				return this._SegmentValue;
			}
			set
			{
				this._SegmentValue = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		protected String _Description;
		[PXDefault()]
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Consolidation Unit", Visibility = PXUIVisibility.Visible)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region Login
		public abstract class login : PX.Data.BQL.BqlString.Field<login> { }
		protected String _Login;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "Username", Visibility = PXUIVisibility.Visible)]
		[PXDefault]
		public virtual String Login
		{
			get
			{
				return this._Login;
			}
			set
			{
				this._Login = value;
			}
		}
		#endregion
		#region Password
		public abstract class password : PX.Data.BQL.BqlString.Field<password> { }
		protected String _Password;		
		[PXRSACryptStringAttribute(IsUnicode = true)]
		[PXUIField(DisplayName = "Password", Visibility = PXUIVisibility.Visible)]
		[PXDefault]
		public virtual String Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				this._Password = value;
			}
		}
		#endregion
		#region Url
		public abstract class url : PX.Data.BQL.BqlString.Field<url> { }
		protected String _Url;
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName = "URL", Visibility = PXUIVisibility.Visible)]
		[PXDefault]
		public virtual String Url
		{
			get
			{
				return this._Url;
			}
			set
			{
				this._Url = value;
			}
		}
		#endregion
		#region SourceLedgerCD
		public abstract class sourceLedgerCD : PX.Data.BQL.BqlString.Field<sourceLedgerCD> { }
		protected String _SourceLedgerCD;
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Source Ledger")]
		[PXSelector(typeof(Search<GLConsolLedger.ledgerCD, Where<GLConsolLedger.setupID, Equal<Optional<GLConsolSetup.setupID>>>>))]
		public virtual String SourceLedgerCD
		{
			get
			{
				return this._SourceLedgerCD;
			}
			set
			{
				this._SourceLedgerCD = value;
			}
		}
		#endregion
		#region SourceBranchCD
		public abstract class sourceBranchCD : PX.Data.BQL.BqlString.Field<sourceBranchCD> { }
		protected String _SourceBranchCD;
		[PXDBString(30, IsUnicode = true, InputMask = "")] //InputMask = "" for using dash ("-") character
		[PXUIField(DisplayName = "Source Company/Branch")]
		[PXSelector(typeof(Search<GLConsolBranch.branchCD,
			Where<GLConsolBranch.setupID, Equal<Optional<GLConsolSetup.setupID>>>,
			OrderBy<Asc<GLConsolBranch.description>>>),
			typeof(GLConsolBranch.displayName),
			typeof(GLConsolBranch.description),
			typeof(GLConsolBranch.ledgerCD),
			DescriptionField = typeof(GLConsolBranch.displayName),
			SelectorMode = PXSelectorMode.DisplayModeText)]
		public virtual String SourceBranchCD
		{
			get
			{
				return this._SourceBranchCD;
			}
			set
			{
				this._SourceBranchCD = value;
			}
		}
		#endregion
		#region PasteFlag
		public abstract class pasteFlag : PX.Data.BQL.BqlBool.Field<pasteFlag> { }
		protected Boolean? _PasteFlag;
		[PXDBBool()]
		[PXUIField(DisplayName = "Paste Segment Value", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? PasteFlag
		{
			get
			{
				return this._PasteFlag;
			}
			set
			{
				this._PasteFlag = value;
			}
		}
		#endregion
		#region LastPostPeriod
		public abstract class lastPostPeriod : PX.Data.BQL.BqlString.Field<lastPostPeriod> { }
		protected String _LastPostPeriod;
		[GL.FinPeriodID()]
		[PXUIField(DisplayName = "Last Post Period", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String LastPostPeriod
		{
			get
			{
				return this._LastPostPeriod;
			}
			set
			{
				this._LastPostPeriod = value;
			}
		}
		#endregion
		#region StartPeriod
		public abstract class startPeriod : PX.Data.BQL.BqlString.Field<startPeriod> { }
		protected String _StartPeriod;
		[GL.FinPeriodSelector]
		[PXUIField(DisplayName = "Start Period")]
		public virtual String StartPeriod
		{
			get
			{
				return this._StartPeriod;
			}
			set
			{
				this._StartPeriod = value;
			}
		}
		#endregion
		#region EndPeriod
		public abstract class endPeriod : PX.Data.BQL.BqlString.Field<endPeriod> { }
		protected String _EndPeriod;
		[GL.FinPeriodSelector]
		[PXUIField(DisplayName = "End Period")]
		public virtual String EndPeriod
		{
			get
			{
				return this._EndPeriod;
			}
			set
			{
				this._EndPeriod = value;
			}
		}
		#endregion
		#region LastConsDate
		public abstract class lastConsDate : PX.Data.BQL.BqlDateTime.Field<lastConsDate> { }
		protected DateTime? _LastConsDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Consolidation Date", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual DateTime? LastConsDate
		{
			get
			{
				return this._LastConsDate;
			}
			set
			{
				this._LastConsDate = value;
			}
		}
		#endregion
		#region BypassAccountSubValidation
		public abstract class bypassAccountSubValidation : PX.Data.BQL.BqlBool.Field<bypassAccountSubValidation> { }
		protected Boolean? _BypassAccountSubValidation;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Bypass Account/Sub Validation")]
		public virtual Boolean? BypassAccountSubValidation
		{
			get
			{
				return this._BypassAccountSubValidation;
			}
			set
			{
				this._BypassAccountSubValidation = value;
			}
		}
		#endregion
		#region HttpClientTimeout
		public abstract class httpClientTimeout : PX.Data.BQL.BqlInt.Field<httpClientTimeout> { }

		/// <summary>
		/// Timeout for Http request to get data from subsidiaries
		/// </summary>
		[PXDBInt(MinValue = 0)]
		[PXDefault(18000)] // 5 hours in seconds
		public virtual int? HttpClientTimeout { get; set; }
		#endregion
		#region ProcessTimeLimit
		public abstract class processTimeLimit : PX.Data.BQL.BqlInt.Field<processTimeLimit>
		{
			public const int Min15 = 15;
			public const int Min30 = 30;
			public const int Min45 = 45;
			public const int Min60 = 60;
			public const int Min90 = 90;
			public const int Min120 = 120;
			public const int Min180 = 180;
			public const int Min240 = 240;
			public const int Min300 = 300;
			public const int Min360 = 360;
			public const int Min420 = 420;
			public const int Min480 = 480;
			public const int Min540 = 540;
			public const int Min600 = 600;
			public const int Min660 = 660;
			public const int Min720 = 720;
			public const int Min780 = 780;
			public const int Min840 = 840;
			public const int Min900 = 900;
			public const int Min960 = 960;
			public const int Min1020 = 1020;
			public const int Min1080 = 1080;
			public const int Min1140 = 1140;
			public const int Min1200 = 1200;
			public const int Min1260 = 1260;
			public const int Min1320 = 1320;
			public const int Min1380 = 1380;
			public const int Min1440 = 1440;

			public class ListAttribute : PXIntListAttribute
			{
				public ListAttribute() : base(
					new int[]
					{
					Min15, Min30, Min45, Min60, Min90, Min120, Min180, Min240, Min300, Min360, Min420, Min480, Min540, Min600, Min660,
					Min720, Min780, Min840, Min900, Min960, Min1020, Min1080, Min1140, Min1200, Min1260, Min1320, Min1380, Min1440
					},
					new string[]
					{
					Messages.Min15,
					Messages.Min30,
					Messages.Min45,
					Messages.Min60,
					Messages.Min90,
					Messages.Min120,
					Messages.Min180,
					Messages.Min240,
					Messages.Min300,
					Messages.Min360,
					Messages.Min420,
					Messages.Min480,
					Messages.Min540,
					Messages.Min600,
					Messages.Min660,
					Messages.Min720,
					Messages.Min780,
					Messages.Min840,
					Messages.Min900,
					Messages.Min960,
					Messages.Min1020,
					Messages.Min1080,
					Messages.Min1140,
					Messages.Min1200,
					Messages.Min1260,
					Messages.Min1320,
					Messages.Min1380,
					Messages.Min1440
					})
				{ }
			}

			public class min15 : PX.Data.BQL.BqlInt.Constant<min15>
			{
				public min15() : base(Min15) {; }
			}

			public class min30 : PX.Data.BQL.BqlInt.Constant<min30>
			{
				public min30() : base(Min30) {; }
			}

			public class min45 : PX.Data.BQL.BqlInt.Constant<min45>
			{
				public min45() : base(Min45) {; }
			}

			public class min60 : PX.Data.BQL.BqlInt.Constant<min60>
			{
				public min60() : base(Min60) {; }
			}

			public class min90 : PX.Data.BQL.BqlInt.Constant<min90>
			{
				public min90() : base(Min90) {; }
			}

			public class min120 : PX.Data.BQL.BqlInt.Constant<min120>
			{
				public min120() : base(Min120) {; }
			}

			public class min180 : PX.Data.BQL.BqlInt.Constant<min180>
			{
				public min180() : base(Min180) {; }
			}

			public class min240 : PX.Data.BQL.BqlInt.Constant<min240>
			{
				public min240() : base(Min240) {; }
			}

			public class min300 : PX.Data.BQL.BqlInt.Constant<min300>
			{
				public min300() : base(Min300) {; }
			}

			public class min360 : PX.Data.BQL.BqlInt.Constant<min360>
			{
				public min360() : base(Min360) {; }
			}

			public class min420 : PX.Data.BQL.BqlInt.Constant<min420>
			{
				public min420() : base(Min420) {; }
			}

			public class min480 : PX.Data.BQL.BqlInt.Constant<min480>
			{
				public min480() : base(Min480) {; }
			}

			public class min540 : PX.Data.BQL.BqlInt.Constant<min540>
			{
				public min540() : base(Min540) {; }
			}

			public class min600 : PX.Data.BQL.BqlInt.Constant<min600>
			{
				public min600() : base(Min600) {; }
			}

			public class min660 : PX.Data.BQL.BqlInt.Constant<min660>
			{
				public min660() : base(Min660) {; }
			}

			public class min720 : PX.Data.BQL.BqlInt.Constant<min720>
			{
				public min720() : base(Min720) {; }
			}

			public class min780 : PX.Data.BQL.BqlInt.Constant<min780>
			{
				public min780() : base(Min780) {; }
			}

			public class min840 : PX.Data.BQL.BqlInt.Constant<min840>
			{
				public min840() : base(Min840) {; }
			}

			public class min900 : PX.Data.BQL.BqlInt.Constant<min900>
			{
				public min900() : base(Min900) {; }
			}

			public class min960 : PX.Data.BQL.BqlInt.Constant<min960>
			{
				public min960() : base(Min960) {; }
			}

			public class min1020 : PX.Data.BQL.BqlInt.Constant<min1020>
			{
				public min1020() : base(Min1020) {; }
			}

			public class min1080 : PX.Data.BQL.BqlInt.Constant<min1080>
			{
				public min1080() : base(Min1080) {; }
			}

			public class min1140 : PX.Data.BQL.BqlInt.Constant<min1140>
			{
				public min1140() : base(Min1140) {; }
			}

			public class min1200 : PX.Data.BQL.BqlInt.Constant<min1200>
			{
				public min1200() : base(Min1200) {; }
			}

			public class min1260 : PX.Data.BQL.BqlInt.Constant<min1260>
			{
				public min1260() : base(Min1260) {; }
			}

			public class min1320 : PX.Data.BQL.BqlInt.Constant<min1320>
			{
				public min1320() : base(Min1320) {; }
			}

			public class min1380 : PX.Data.BQL.BqlInt.Constant<min1380>
			{
				public min1380() : base(Min1380) {; }
			}

			public class min1440 : PX.Data.BQL.BqlInt.Constant<min1440>
			{
				public min1440() : base(Min1440) {; }
			}

		}
		/// <summary>
		/// The maximum time limit allowed for the consolidation process before it is forced to stop
		/// </summary>
		[PXDBInt]
		[PXDefault(processTimeLimit.Min300)]
		[PXUIField(DisplayName = "Process Time Limit")]
		[processTimeLimit.List]
		public int? ProcessTimeLimit { get; set; }
		#endregion
	}
}
