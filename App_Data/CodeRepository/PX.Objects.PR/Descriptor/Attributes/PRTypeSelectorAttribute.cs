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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Payroll;
using PX.Payroll.Data;
using PX.Payroll.Proxy;

namespace PX.Objects.PR
{
	[PXCacheName(Messages.PRTaxType)]
	[Serializable]
	public class PRDynType : PXBqlTable, IBqlTable
	{
		#region TypeID
		public abstract class id : IBqlField { }
		[PXInt(IsKey = true)]
		[PXUIField(DisplayName = "Type ID", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public int? ID { get; set; }
		#endregion
		#region TypeName
		public abstract class name : IBqlField { }
		[PXString]
		[PXUIField(DisplayName = "Name", Visibility = PXUIVisibility.SelectorVisible)]
		public string Name { get; set; }
		#endregion
		#region Description
		public abstract class description : IBqlField { }
		[PXString(250)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public string Description { get; set; }
		#endregion
	}

	public abstract class PRTypeSelectorBaseAttribute : PXCustomSelectorAttribute, IDynamicSettingsUpdateObserver, IPXFieldDefaultingSubscriber
	{
		[InjectDependency]
		protected IPayrollSettingsCache SettingsCache { get; set; }

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		protected List<PRTypeMeta> _PRTypeList = null;
		protected Type _PRTypeClass;
		private bool _ShowOnlyWithDescription;
		private bool _SetDefaultValue;
		private int? _DefaultValue;
		private Type _CountryIDField = null;
		private string _CountryID;

		protected bool AllowNull { get; set; } = true;

		public PRTypeSelectorBaseAttribute(Type prTypeClass, bool showOnlyWithDescription, bool setDefaultValue, Type countryIDField, int? defaultValue = null)
			: base(typeof(PRDynType.id))
		{
			Init(prTypeClass, showOnlyWithDescription, setDefaultValue, defaultValue);
			_CountryIDField = countryIDField;
		}

		public PRTypeSelectorBaseAttribute(Type prTypeClass, bool showOnlyWithDescription, bool setDefaultValue, string countryID, int? defaultValue = null)
			: base(typeof(PRDynType.id))
		{
			Init(prTypeClass, showOnlyWithDescription, setDefaultValue, defaultValue);
			_CountryID = countryID;
		}

		private void Init(Type prTypeClass, bool showOnlyWithDescription, bool setDefaultValue, int? defaultValue = null)
		{
			this._ShowOnlyWithDescription = showOnlyWithDescription;
			this._SetDefaultValue = setDefaultValue;
			this._DefaultValue = defaultValue;
			this.ValidateValue = false;
			this.SubstituteKey = typeof(PRDynType.name);
			if (showOnlyWithDescription)
			{
				this.DescriptionField = typeof(PRDynType.description);
			}

			_PRTypeClass = prTypeClass;
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
        }

		public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			base.FieldVerifying(sender, e);

			if (!AllowNull && e.NewValue == null)
			{
				throw new PXSetPropertyException(Messages.FieldCantBeNull);
			}

			PXUIFieldAttribute.SetError(sender, e.Row, _FieldName, null);
		}

		public abstract bool UseReportingType { get; }

		protected virtual List<PRTypeMeta> GetPRTypeList(Type prTypeClass, bool useReportingType)
		{
			return SettingsCache.GetPRTypeList(prTypeClass, useReportingType);
		}

		protected virtual IEnumerable GetRecords()
		{
			foreach (var taxType in GetPRTypeList(_PRTypeClass, UseReportingType).Where(x => CountryMatches(x)))
			{
				if (!_ShowOnlyWithDescription || taxType.HasDescription)
				{
					var name = taxType.Name.ToUpper();
					var description = _ShowOnlyWithDescription
									  ? taxType.Description
									  : name;

					yield return new PRDynType
					{
						ID = taxType.ID,
						Name = name,
						Description = description
					};
				}
			}
		}

		protected static int? GetDefaultID(bool useReportingType, Type prTypeClass, string countryID)
		{
			return GetAll(useReportingType, prTypeClass, countryID).FirstOrDefault(x => x.IsDefault)?.ID;
		}

		protected static int? GetDefaultID<PRType>(bool useReportingType, string countryID) where PRType : IPRTypeClass
		{
			return GetDefaultID(useReportingType, typeof(PRType), countryID);
		}

		protected static int? GetAatrixMapping<PRType>(bool useReportingType, int itemID, string countryID) where PRType : IPRTypeClass
		{
			return GetAll(useReportingType, typeof(PRType), countryID).FirstOrDefault(x => x.ID == itemID)?.AatrixMapping?.Field;
		}

		protected static List<PRTypeMeta> GetAll(bool useReportingType, Type prTypeClass, bool useCaching = true)
		{
			Dictionary<(Type, bool), List<PRTypeMeta>> prTypeCache;
			if (useCaching && _PRTypeCache != null && _PRTypeCache.TryGetTarget(out prTypeCache) && prTypeCache.ContainsKey((prTypeClass, useReportingType)))
			{
				return prTypeCache[(prTypeClass, useReportingType)];
			}
			else
			{
				using (var payrollAssemblyScope = new PXPayrollAssemblyScope<PayrollMetaProxy>())
				{
					return payrollAssemblyScope.Proxy.GetPRTypeMeta(prTypeClass, useReportingType);
				}
			}
		}

		protected static List<PRTypeMeta> GetAll(bool useReportingType, Type prTypeClass, string countryID, bool useCaching = true)
		{
			return GetAll(useReportingType, prTypeClass, useCaching).Where(x => CountryMatches(x, countryID)).ToList();
		}

		public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (!_SetDefaultValue)
				return;

			e.NewValue = GetPRTypeList(_PRTypeClass, UseReportingType).FirstOrDefault(x => CountryMatches(x) && x.IsDefault)?.ID ?? _DefaultValue.GetValueOrDefault(0);
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public void DynamicSettingsUpdated()
		{
		}

		protected static bool CountryMatches(PRTypeMeta taxType, string countryID)
		{
			if (string.IsNullOrEmpty(countryID))
			{
				return true;
			}

			return PRSubnationalEntity.GetFederal(countryID)?.Abbr == taxType.State;
		}

		private bool CountryMatches(PRTypeMeta taxType)
		{
			if (_CountryIDField != null)
			{
				PXCache cache = _Graph.Caches[BqlCommand.GetItemType(_CountryIDField)];
				_CountryID = cache.GetValue(cache.Current, _CountryIDField.Name) as string;
			}

			return CountryMatches(taxType, _CountryID);
		}

		#region PR Type Caching
		private static readonly object CacheLock = new object();
		private static WeakReference<Dictionary<(Type, bool), List<PRTypeMeta>>> _PRTypeCache = null;

		public class PRTypeSelectorCache : IDisposable, IDynamicSettingsUpdateObserver
		{
			private Dictionary<(Type, bool), List<PRTypeMeta>> _CacheInstance;

			public PRTypeSelectorCache(Type[] cacheTypes)
			{
				lock (CacheLock)
				{
					if (_PRTypeCache != null && _PRTypeCache.TryGetTarget(out _CacheInstance))
					{
						foreach (Type t in cacheTypes)
						{
							if (!_CacheInstance.ContainsKey((t, false)))
							{
								_CacheInstance[(t, false)] = GetAll(false, t, false);
							}
							if (!_CacheInstance.ContainsKey((t, true)))
							{
								_CacheInstance[(t, true)] = GetAll(true, t, false);
							}
						}
					}
					else
					{
						_CacheInstance = new Dictionary<(Type, bool), List<PRTypeMeta>>();
						foreach (Type t in cacheTypes)
						{
							_CacheInstance[(t, false)] = GetAll(false, t, false);
							_CacheInstance[(t, true)] = GetAll(true, t, false);
						}

						if (_PRTypeCache != null)
						{
							_PRTypeCache.SetTarget(_CacheInstance);
						}
						else
						{
							_PRTypeCache = new WeakReference<Dictionary<(Type, bool), List<PRTypeMeta>>>(_CacheInstance);
						}
					}
				}

				PXPayrollAssemblyScope.SubscribeDynamicSettingsUpdate(this);
			}

			public void Dispose()
			{
				_CacheInstance = null;
				GC.Collect();
			}

			public void DynamicSettingsUpdated()
			{
				if (_CacheInstance?.Keys != null)
				{
				foreach ((Type t, bool isReportingType) in _CacheInstance?.Keys)
				{
					_CacheInstance[(t, isReportingType)] = GetAll(isReportingType, t, false);
				}
			}
		}
		}
		#endregion PR Type Caching
	}

	public class PRTypeSelectorAttribute : PRTypeSelectorBaseAttribute
	{
		public PRTypeSelectorAttribute(Type prTypeClass, Type countryIDField, bool showOnlyWithDescription = true)
			: base(prTypeClass, showOnlyWithDescription, false, countryIDField) 
		{ }

		public PRTypeSelectorAttribute(Type prTypeClass, string countryID, bool showOnlyWithDescription = true)
			: base(prTypeClass, showOnlyWithDescription, false, countryID) 
		{ }

		public PRTypeSelectorAttribute(Type prTypeClass, int defaultValue, Type countryIDField, bool showOnlyWithDescription = true)
			: base(prTypeClass, showOnlyWithDescription, true, countryIDField, defaultValue)
		{
			AllowNull = false;
		}

		public PRTypeSelectorAttribute(Type prTypeClass, int defaultValue, string countryID, bool showOnlyWithDescription = true)
			: base(prTypeClass, showOnlyWithDescription, true, countryID, defaultValue)
		{
			AllowNull = false;
		}

		public static int? GetDefaultReportingType<PRType>(int itemID, string countryID) where PRType : IPRTypeClass
		{
			return GetAll(false, typeof(PRType), countryID).FirstOrDefault(x => x.ID == itemID)?.DefaultReportType;
		}

		public static CalculationType? GetCalculationMethod<PRType>(int itemID, string countryID) where PRType : IPRTypeClass
		{
			return GetAll(false, typeof(PRType), countryID).FirstOrDefault(x => x.ID == itemID)?.CalculationMethod;
		}

		public override bool UseReportingType { get => false; }

		public static int? GetDefaultID<PRType>(string countryID) where PRType : IPRTypeClass
		{
			return PRTypeSelectorBaseAttribute.GetDefaultID<PRType>(false, countryID);
		}

		public static int? GetAatrixMapping<PRType>(int itemID, string countryID) where PRType : IPRTypeClass
		{
			return PRTypeSelectorBaseAttribute.GetAatrixMapping<PRType>(false, itemID, countryID);
		}

		public static List<PRTypeMeta> GetAll<PRType>(string countryID)
		{
			return GetAll(false, typeof(PRType), countryID);
		}
	}

	public class PRReportingTypeSelectorAttribute : PRTypeSelectorBaseAttribute
	{
        private PX.Payroll.TaxCategory _ReportTypeScope;
        
        public PRReportingTypeSelectorAttribute(Type prTypeClass, Type countryIDField, PX.Payroll.TaxCategory reportTypeScope = PX.Payroll.TaxCategory.Any, bool setDefaultValue = true, bool showOnlyWithDescription = true)
			: base(prTypeClass, showOnlyWithDescription, setDefaultValue, countryIDField)
		{
            _ReportTypeScope = reportTypeScope;
		}

		public PRReportingTypeSelectorAttribute(Type prTypeClass, string countryID, PX.Payroll.TaxCategory reportTypeScope = PX.Payroll.TaxCategory.Any, bool setDefaultValue = true, bool showOnlyWithDescription = true)
			: base(prTypeClass, showOnlyWithDescription, setDefaultValue, countryID)
		{
			_ReportTypeScope = reportTypeScope;
		}

		protected override List<PRTypeMeta> GetPRTypeList(Type prTypeClass, bool useReportingType)
		{
			if (_ReportTypeScope != PX.Payroll.TaxCategory.Any)
			{
				return SettingsCache.GetPRTypeList(prTypeClass, useReportingType)
					.Where(x => x.ReportTypeScope == _ReportTypeScope || x.ReportTypeScope == PX.Payroll.TaxCategory.Any)
					.ToList();
			}

			return base.GetPRTypeList(prTypeClass, useReportingType);
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
        }

        public static PX.Payroll.TaxCategory? GetReportingTypeScope<PRType>(int itemID, string countryID) where PRType : IPRTypeClass
		{
			return GetAll(true, typeof(PRType), countryID).FirstOrDefault(x => x.ID == itemID)?.ReportTypeScope;
		}

		public override bool UseReportingType { get => true; }

		public static int? GetDefaultID<PRType>(string countryID) where PRType : IPRTypeClass
		{
			return PRTypeSelectorBaseAttribute.GetDefaultID<PRType>(true, countryID);
		}

		public static int? GetAatrixMapping<PRType>(int itemID, string countryID) where PRType : IPRTypeClass
		{
			return PRTypeSelectorBaseAttribute.GetAatrixMapping<PRType>(true, itemID, countryID);
		}

		public static List<PRTypeMeta> GetAll<PRType>(string countryID)
		{
			return PRTypeSelectorBaseAttribute.GetAll(true, typeof(PRType), countryID);
		}
	}
}
