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
using System.Collections.Generic;
using System.Linq;
using PX.Data;

namespace PX.Objects.Common.EntityInUse
{
	public static class EntityInUseHelper
	{
		public static void MarkEntityAsInUse<Table>(params object[] keys)
			 where Table : IBqlTable
		{
			if (IsEntityInUse<Table>(keys))
			{
				return;
			}

			KeysCollection cacheKeys = GetCacheKeys<Table>();

			if (cacheKeys.Count() != keys.Length)
			{
				throw new PXArgumentException(nameof(keys));
			}

			var fields = new PXDataFieldAssign[cacheKeys.Count];

			for (int i = 0; i < cacheKeys.Count; i++)
			{
				fields[i] = new PXDataFieldAssign(cacheKeys[i], keys[i]);
			}

			try
			{
				PXDatabase.Insert<Table>(fields);
			}
			catch (PXDatabaseException ex) when (ex.ErrorCode == PXDbExceptions.PrimaryKeyConstraintViolation) { }
		}

		public static bool IsEntityInUse<Table>(params object[] keys)
			where Table : IBqlTable
		{
			if (Attribute.IsDefined(typeof(Table), typeof(EntityInUseDBSlotOnAttribute)))
			{
				var slot = PXDatabase.GetSlot<EntityInUseDefinition<Table>>(typeof(EntityInUseDefinition<Table>).FullName, typeof(Table));
				return keys.Length != 0
					? slot.EntitiesInUse.Contains(GetHash<Table>(keys))
					: slot.EntitiesInUse.Any();
			}
			else
			{
				KeysCollection cacheKeys = GetCacheKeys<Table>();

				if (keys.Length != 0 && cacheKeys.Count() != keys.Length)
				{
					throw new PXArgumentException(nameof(keys));
				}

				var fields = new PXDataField[cacheKeys.Count * (keys.Length != 0 ? 2 : 1)];

				for (int i = 0; i < cacheKeys.Count; i++)
				{
					string cacheKey = cacheKeys[i];
					fields[i] = new PXDataField(cacheKey);

					if (keys.Length != 0)
					{
						fields[cacheKeys.Count + i] = new PXDataFieldValue(cacheKey, keys[i]);
					}
				}

				using (PXDataRecord record = PXDatabase.SelectSingle<Table>(fields))
				{
					return record != null;
				}
			}
		}

		private static Lazy<PXGraph> lazyGraph = new Lazy<PXGraph>(() => { return PXGraph.CreateInstance<PXGraph>(); });

		private static KeysCollection GetCacheKeys<Table>()
			where Table : IBqlTable
		{
			return lazyGraph.Value.Caches[typeof(Table)].Keys;
		}

		private static string GetHash<Table>(params object[] keys)
			where Table : IBqlTable
		{
			return string.Join("::", keys.Select(key => (key ?? string.Empty).ToString()));
		}

		private class EntityInUseDefinition<Table> : IPrefetchable
			where Table : IBqlTable
		{
			public readonly HashSet<string> EntitiesInUse = new HashSet<string>();

			public void Prefetch()
			{
				EntitiesInUse.Clear();

				using (new PXConnectionScope())
				{
					PXDataField[] fields = GetCacheKeys<Table>().Select(key => new PXDataField(key)).ToArray();

					foreach (PXDataRecord record in PXDatabase.SelectMulti<Table>(fields))
					{
						var keys = new object[record.FieldCount];

						for (var i = 0; i < keys.Length; i++)
						{
							keys[i] = record.GetValue(i);
						}

						EntitiesInUse.Add(GetHash<Table>(keys));
					}
				}
			}
		}
	}
}