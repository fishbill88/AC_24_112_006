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
using System.Threading;
using System.Threading.Tasks;
using PX.Commerce.Core;

namespace PX.Commerce.Shopify.API.GraphQL
{
	/// <inheritdoc />
	public class PriceListGQLDataProviderFactory : ISPGraphQLDataProviderFactory<PriceListGQLDataProvider>
	{
		/// <inheritdoc />
		public PriceListGQLDataProvider GetProvider(IGraphQLAPIClient graphQLAPIService)
		{
			return new PriceListGQLDataProvider(graphQLAPIService);
		}
	}

	/// <summary>
	/// Performs data operations with PriceList through Shopify's GraphQL API
	/// </summary>
	public class PriceListGQLDataProvider : SPGraphQLDataProvider, IPriceListDataGQLProvider
	{

		/// <summary>
		/// Creates a new instance of the PriceListDataGraphQLProvider that uses the specified GraphQLAPIService.
		/// </summary>
		/// <param name="graphQLAPIClient">The GraphQLAPIService to use to make requests.</param>
		public PriceListGQLDataProvider(IGraphQLAPIClient graphQLAPIClient) : base(graphQLAPIClient)
		{
		}

		/// <inheritdoc />
		public async Task<CompanyLocationCatalogGQL> CreateCatalog(CatalogCreateInput catalogInput, CancellationToken cancellationToken = default)
		{
			List<string> exceedingCompaniesIds = new();

			if (catalogInput?.Context?.CompanyLocationIds.Any() == true)
				catalogInput.Context.CompanyLocationIds = AdjustListLength(catalogInput.Context.CompanyLocationIds, out exceedingCompaniesIds);

			var variables = new Dictionary<Type, object>
			{
				{ typeof(CatalogCreatePayload.Arguments.Input), catalogInput }
			};

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(CatalogCreatePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.CatalogCreate?.UserErrors);

			if (exceedingCompaniesIds.Any())
			{
				return await UpdateRemainingCompanyLocations(response?.CatalogCreate?.Catalog?.Id, exceedingCompaniesIds, cancellationToken);
			}

			return response?.CatalogCreate?.Catalog;
		}

		/// <inheritdoc />
		public async Task<CompanyLocationCatalogGQL> UpdateCatalog(string catalogId, CatalogUpdateInput catalogInput, CancellationToken cancellationToken = default)
		{
			List<string> exceedingCompaniesIds = new();

			if (catalogInput?.Context?.CompanyLocationIds.Any() == true)
				catalogInput.Context.CompanyLocationIds = AdjustListLength(catalogInput.Context.CompanyLocationIds, out exceedingCompaniesIds);

			var variables = new Dictionary<Type, object>
			{
				{ typeof(CatalogUpdatePayload.Arguments.Id), catalogId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.CompanyLocationCatalog)},
				{ typeof(CatalogUpdatePayload.Arguments.Input), catalogInput}
			};
			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(CatalogUpdatePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.CatalogUpdate?.UserErrors);

			if (exceedingCompaniesIds.Any())
			{
				return await UpdateRemainingCompanyLocations(catalogId, exceedingCompaniesIds, cancellationToken);
			}

			return response?.CatalogUpdate?.Catalog;
		}

		/// <inheritdoc />
		public async Task<string> DeleteCatalog(string catalogId, bool deleteDependentResources = true, CancellationToken cancellationToken = default)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(CatalogDeletePayload.Arguments.Id), catalogId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.CompanyLocationCatalog)},
				{ typeof(CatalogDeletePayload.Arguments.DeleteDependentResources), deleteDependentResources}
			};
			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(CatalogDeletePayload), GraphQLQueryType.Mutation, variables, true);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.CatalogDelete?.UserErrors);

			return response?.CatalogDelete?.DeletedId;
		}

		/// <inheritdoc />
		public async Task<PublicationGQL> CreatePublication(PublicationCreateInput publicationInput, CancellationToken cancellationToken = default)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(PublicationCreatePayload.Arguments.Input), publicationInput }
			};

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PublicationCreatePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.PublicationCreate?.UserErrors);

			return response?.PublicationCreate?.Publication;
		}

		/// <inheritdoc />
		public async Task<PriceListGQL> CreatePriceList(PriceListCreateInput priceListInput, CancellationToken cancellationToken = default)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(PriceListCreatePayload.Arguments.Input), priceListInput }
			};

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListCreatePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.PriceListCreate?.UserErrors);

			return response?.PriceListCreate?.PriceList;
		}

		/// <inheritdoc />
		public async Task<PriceListGQL> UpdatePriceList(string priceListId, PriceListUpdateInput priceListInput, CancellationToken cancellationToken = default)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(PriceListUpdatePayload.Arguments.Id), priceListId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.PriceList)},
				{ typeof(PriceListUpdatePayload.Arguments.Input), priceListInput}
			};
			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListUpdatePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.PriceListUpdate?.UserErrors);

			return response?.PriceListUpdate?.PriceList;
		}

		/// <inheritdoc />
		public async Task<string> DeletePriceList(string priceListId, CancellationToken cancellationToken = default)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(PriceListDeletePayload.Arguments.Id), priceListId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.PriceList)}
			};
			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListDeletePayload), GraphQLQueryType.Mutation, variables, true);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.PriceListDelete?.UserErrors);

			return response?.PriceListDelete?.DeletedId;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<PriceListPriceGQL>> AddPriceListFixedPrices(string priceListId, List<PriceListPriceInput> priceListPriceInputs, CancellationToken cancellationToken = default)
		{
			List<PriceListPriceGQL> finalResponse = null;

			while (priceListPriceInputs?.Any() == true)
			{
				List<PriceListPriceInput> priceListsToAdd = AdjustListLength(priceListPriceInputs, out List<PriceListPriceInput> exceedingPriceLists);
			var variables = new Dictionary<Type, object>
			{
				{ typeof(PriceListFixedPricesAddPayload.Arguments.PriceListId), priceListId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.PriceList)},
					{ typeof(PriceListFixedPricesAddPayload.Arguments.Prices), priceListsToAdd}
			};
			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListFixedPricesAddPayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.PriceListFixedPricesAdd?.UserErrors);

				if (finalResponse == null)
				{
					finalResponse = response?.PriceListFixedPricesAdd?.Prices;
				}
				else
				{
					finalResponse.AddRange(response?.PriceListFixedPricesAdd?.Prices ?? Enumerable.Empty<PriceListPriceGQL>());
				}

				priceListPriceInputs = exceedingPriceLists.ToList();
			}

			return finalResponse;
		}

		/// <inheritdoc />
		public async Task<PriceListFixedPricesUpdatePayload> UpdatePriceListFixedPrices(string priceListId, List<PriceListPriceInput> priceListPriceInputs, string[] variantIdsToDelete = null, CancellationToken cancellationToken = default)
		{
			PriceListFixedPricesUpdatePayload finalResponse = null;

			while (variantIdsToDelete?.Any() == true || priceListPriceInputs?.Any() == true)
			{

				List<PriceListPriceInput> variantsToAdd = AdjustListLength(priceListPriceInputs, out List<PriceListPriceInput> exceedingPriceLists);
				string[] variantsToRemove = AdjustListLength(variantIdsToDelete.ToList(), out List<string> exceedingVariantIDsToDelete).ToArray();

			var variables = new Dictionary<Type, object>
			{
				{ typeof(PriceListFixedPricesUpdatePayload.Arguments.PriceListId), priceListId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.PriceList)},
					{ typeof(PriceListFixedPricesUpdatePayload.Arguments.VariantIdsToDelete), variantsToRemove ?? new string[0]},
					{ typeof(PriceListFixedPricesUpdatePayload.Arguments.PricesToAdd), variantsToAdd}
			};
			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListFixedPricesUpdatePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.PriceListFixedPricesUpdate?.UserErrors);

				if (finalResponse == null)
				{
					finalResponse = response?.PriceListFixedPricesUpdate;
				}
				else
				{
					finalResponse.PriceList = response?.PriceListFixedPricesUpdate.PriceList;
					finalResponse.PricesAdded.AddRange(response?.PriceListFixedPricesUpdate.PricesAdded ?? Enumerable.Empty<PriceListPriceGQL>());
					finalResponse.DeletedFixedPriceVariantIds.AddRange(response?.PriceListFixedPricesUpdate.DeletedFixedPriceVariantIds ?? Enumerable.Empty<string>());
				}

				priceListPriceInputs = exceedingPriceLists;
				variantIdsToDelete = exceedingVariantIDsToDelete.ToArray();
			}

			return finalResponse;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<string>> DeletePriceListFixedPrices(string priceListId, string[] variantIdsToDelete, CancellationToken cancellationToken = default)
		{
			List<string> finalResponse = null;

			while (variantIdsToDelete?.Any() == true)
			{
				string[] variantsToRemove = AdjustListLength(variantIdsToDelete.ToList(), out List<string> exceedingVariantIDsToDelete).ToArray();
			var variables = new Dictionary<Type, object>
			{
				{ typeof(PriceListFixedPricesDeletePayload.Arguments.PriceListId), priceListId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.PriceList)},
					{ typeof(PriceListFixedPricesDeletePayload.Arguments.VariantIds), variantsToRemove ?? new string[0]}
			};

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListFixedPricesDeletePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.PriceListFixedPricesDelete?.UserErrors);

				if (finalResponse == null)
				{
					finalResponse = response?.PriceListFixedPricesDelete.DeletedFixedPriceVariantIds;
				}
				else
				{
					finalResponse.AddRange(response?.PriceListFixedPricesDelete.DeletedFixedPriceVariantIds ?? Enumerable.Empty<string>());
				}

				variantIdsToDelete = exceedingVariantIDsToDelete.ToArray();
			}

			return finalResponse;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<CompanyLocationCatalogGQL>> GetCatalogs(string filterString = null, bool includedSubFields = false, CancellationToken cancellationToken = default, params string[] specifiedFieldsOnly)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(QueryArgument.First<CompanyLocationCatalogGQL>), includedSubFields ? DefaultFetchBulkSizeWithSubfields : DefaultFetchBulkSize},
				{ typeof(QueryArgument.After<CompanyLocationCatalogGQL>), null}
			};
			if (string.IsNullOrEmpty(filterString) == false)
			{
				variables[typeof(QueryArgument.Query<CompanyLocationCatalogGQL>)] = filterString;
			}

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(CompanyLocationCatalogGQL), GraphQLQueryType.Connection, variables, includedSubFields, false, specifiedFieldsOnly);

			return await GetAllAsync<CompanyLocationCatalogGQL, CatalogsResponseData, CatalogsResponse>(queryInfo, cancellationToken);
		}

		/// <inheritdoc />
		public async Task<IEnumerable<PriceListGQL>> GetPriceLists(string filterString = null, bool includedSubFields = false, CancellationToken cancellationToken = default, params string[] specifiedFieldsOnly)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(QueryArgument.First<PriceListGQL>), includedSubFields ? DefaultFetchBulkSizeWithSubfields : DefaultFetchBulkSize},
				{ typeof(QueryArgument.After<PriceListGQL>), null}
			};
			if (string.IsNullOrEmpty(filterString) == false)
			{
				variables[typeof(QueryArgument.Query<PriceListGQL>)] = filterString;
			}

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListGQL), GraphQLQueryType.Connection, variables, includedSubFields, false, specifiedFieldsOnly);

			return await GetAllAsync<PriceListGQL, PriceListsResponseData, PriceListsResponse>(queryInfo, cancellationToken);
		}

		/// <inheritdoc />
		public async Task<CompanyLocationCatalogGQL> GetCatalogByID(string id, bool withSubFields = true, bool withSubConnections = false, CancellationToken cancellationToken = default, params string[] specifiedFieldsOnly)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(QueryArgument.ID<CompanyLocationCatalogGQL>), id.ConvertIdToGid(ShopifyGraphQLConstants.Objects.CompanyLocationCatalog)}
			};
			if (withSubConnections)
			{
				variables[typeof(QueryArgument.First<CompanyLocationDataGQL>)] = DefaultPageSize;
				variables[typeof(QueryArgument.After<CompanyLocationDataGQL>)] = null;
			}

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(CompanyLocationCatalogGQL), GraphQLQueryType.Node, variables, withSubFields, withSubConnections, specifiedFieldsOnly);

			return await GetSingleAsync<CompanyLocationCatalogGQL, CompanyLocationDataGQL, CatalogResponse>(queryInfo, nameof(CompanyLocationCatalogGQL.CompanyLocations), cancellationToken);
		}

		/// <inheritdoc />
		public async Task<PriceListGQL> GetPriceListByID(string id, bool withSubFields = true, bool withSubConnections = false, CancellationToken cancellationToken = default, params string[] specifiedFieldsOnly)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(QueryArgument.ID<PriceListGQL>), id.ConvertIdToGid(ShopifyGraphQLConstants.Objects.PriceList)}
			};
			if (withSubConnections)
			{
				variables[typeof(PriceListPriceGQL.Arguments.OriginType)] = PriceListPriceOriginTypeGQL.Fixed;
				variables[typeof(QueryArgument.First<PriceListPriceGQL>)] = DefaultPageSize;
				variables[typeof(QueryArgument.After<PriceListPriceGQL>)] = null;
			}

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(PriceListGQL), GraphQLQueryType.Node, variables, withSubFields, withSubConnections, specifiedFieldsOnly);

			return await GetSingleAsync<PriceListGQL, PriceListPriceGQL, PriceListResponse>(queryInfo, nameof(PriceListGQL.Prices), cancellationToken);
		}

		/// <inheritdoc />
		public async Task<ShopGQL> GetShop(CancellationToken cancellationToken = default, params string[] specifiedFieldsOnly)
		{
			var variables = new Dictionary<Type, object>
			{
				{ typeof(QueryArgument.First<CurrencySettingGQL>), DefaultPageSize},
				{ typeof(QueryArgument.After<CurrencySettingGQL>), null}
			};

			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(ShopGQL), GraphQLQueryType.Node, variables, true, true, specifiedFieldsOnly);

			return await GetSingleAsync<ShopGQL, CurrencySettingGQL, ShopResponse>(queryInfo, nameof(ShopGQL.CurrencySettings), cancellationToken);
		}

		/// <inheritdoc />
		public async Task<CompanyLocationCatalogGQL> UpdateCatalogContext(string catalogId, CatalogContextInput contextToAdd = null, CatalogContextInput contextToRemove = null, CancellationToken cancellationToken = default)
		{
			if (catalogId == null || (contextToAdd == null && contextToRemove == null)) return null;

			var variables = new Dictionary<Type, object>
			{
				{ typeof(CatalogContextUpdatePayload.Arguments.CatalogId), catalogId.ConvertIdToGid(ShopifyGraphQLConstants.Objects.CompanyLocationCatalog)},
			};

			if (contextToAdd != null)
				variables.Add(typeof(CatalogContextUpdatePayload.Arguments.ContextsToAdd), contextToAdd);
			if (contextToRemove != null)
				variables.Add(typeof(CatalogContextUpdatePayload.Arguments.ContextsToRemove), contextToRemove);
			
			var querybuilder = new GraphQLQueryBuilder();
			GraphQLQueryInfo queryInfo = querybuilder.GetQueryResult(typeof(CatalogContextUpdatePayload), GraphQLQueryType.Mutation, variables, true, false);

			var response = await MutationAsync<PriceListMutation>(queryInfo, cancellationToken);
			CheckIfHaveErrors(response?.CatalogContextUpdate?.UserErrors);

			return response?.CatalogContextUpdate?.Catalog;
		}

		/// <summary>
		/// In GraphQl there is a limited number of elements allowed, and because of that it is not possible to synchronize all companies locations at once.
		/// This method intends to update the price list with the remaining company locations.
		/// </summary>
		/// <param name="catalogId"></param>
		/// <param name="locationIdsToAdd"></param>
		/// <param name="cancellationToken"></param>
		public virtual async Task<CompanyLocationCatalogGQL> UpdateRemainingCompanyLocations(string catalogId, IEnumerable<string> locationIdsToAdd, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(catalogId) || locationIdsToAdd?.Any() != true) return null;

			int maxInputSize = GetMaxInputSize();
			CompanyLocationCatalogGQL response = null;

			for (int i = 0; i < locationIdsToAdd.Count(); i += maxInputSize)
			{
				CatalogContextInput catalogUpdateInputToAdd = new() { CompanyLocationIds = locationIdsToAdd.Skip(i).Take(maxInputSize).ToList() };
				response = await UpdateCatalogContext(catalogId, catalogUpdateInputToAdd, null, cancellationToken);
			}

			return response;
		}
	}
}
