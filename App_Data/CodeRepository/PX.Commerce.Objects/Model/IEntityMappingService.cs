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

using System.Threading;
using System.Threading.Tasks;
using PX.Commerce.Core;

namespace PX.Commerce.Objects
{
	/// <summary>
	/// Provides functionality for instantiating new instances of <see cref="IEntityMappingService{TLocal,TExtern}"/>
	/// </summary>
	public interface IEntityMappingServiceFactory<TSource, TTarget> where TSource : ILocalEntity where TTarget : IExternEntity
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="IEntityMappingService{TLocal,TExtern}"/>
		/// </summary>
		public Task<IEntityMappingService<TSource, TTarget>> GetService(IProcessor processor);
	}

	/// <summary>
	/// Provides functionality for mapping an <typeparamref name="TSource"/> to a  <typeparamref name="TTarget"/>
	/// </summary>
	public interface IEntityMappingService<in TSource, TTarget>
	{

		/// <summary>
		/// Takes an instance of <typeparamref name="TSource"/> and maps it to a new instance of <typeparamref name="TTarget"/>.
		/// </summary>
		public TTarget Map(TSource external, TTarget existing = default);
	}
}
