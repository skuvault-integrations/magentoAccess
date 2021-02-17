using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagentoAccess.MagentoSoapServiceReference;
using MagentoAccess.Misc;
using MagentoAccess.Models.Services.Soap.GetProducts;
using Netco.Extensions;
using Netco.Logging;

namespace MagentoAccess.Services.Soap._1_9_2_1_ce
{
	internal partial class MagentoServiceLowLevelSoap_v_1_9_2_1_ce : IMagentoServiceLowLevelSoap, IMagentoServiceLowLevelSoapGetProductsBySku, IMagentoServiceLowLevelSoapFillProductsDetails
	{
		public virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, CancellationToken cancellationToken, Mark mark = null )
		{
			try
			{
				Func< int, int, Func< int, string >, bool, bool, Task< List< SoapProduct > > > productsSelector = async ( start1, count1, selector1, removeSpecialSymbols, skipExceptions ) =>
				{
					try
					{
						var sourceList = Enumerable.Range( start1, count1 ).Select( selector1 ).ToList();
						if( removeSpecialSymbols )
							sourceList.RemoveAll( x => x == "%00" );
						var productsResponses = await sourceList.ProcessInBatchAsync( this._getProductsMaxThreads, async x => await this.GetProductsPageAsync( productType, productTypeShouldBeExcluded, x, updatedFrom, cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );
						var prods = productsResponses.SelectMany( x => x.Products ).ToList();

						return prods;
					}
					catch( Exception e )
					{
						if( skipExceptions )
						{
							this.LogTraceGetResponseException( e );
							return new List< SoapProduct >();
						}
						else
							throw;
					}
				};

				//10..99(9)
				var productsMainPart = ( await productsSelector( 0, 100, x => "%" + x.ToString( "D2" ), true, false ).ConfigureAwait( false ) ).ToList();
				//0..9
				productsMainPart.AddRange( await productsSelector( 0, 10, x => x.ToString( "D1" ), true, false ).ConfigureAwait( false ) );
				//special symbols
				var productsWithSpecialSymbols = await productsSelector( 0, 1, x => "%*00", false, true ).ConfigureAwait( false );
				productsWithSpecialSymbols.AddRange( await productsSelector( 0, 1, x => "%00", false, true ).ConfigureAwait( false ) );
				productsWithSpecialSymbols = productsWithSpecialSymbols.GroupBy( x => x.Sku ).Select( x => x.First() ).ToList();

				productsMainPart.AddRange( productsWithSpecialSymbols );
				var soapGetProductsResponse = new SoapGetProductsResponse { Products = productsMainPart };

				return soapGetProductsResponse;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		public virtual async Task< SoapGetProductsResponse > GetProductsAsync( string productType, bool productTypeShouldBeExcluded, DateTime? updatedFrom, IReadOnlyCollection< string > skus, CancellationToken cancellationToken, Mark mark = null )
		{
			try
			{
				var skusList = skus.ToList();
				var skusListChunks = skusList.SplitToChunks( 10 );
				var responses = await skusListChunks.ProcessInBatchAsync( this._getProductsMaxThreads, async x => await this.GetProductsPageAsync( productType, productTypeShouldBeExcluded, x, updatedFrom, cancellationToken ).ConfigureAwait( false ) ).ConfigureAwait( false );
				var soapGetProductsResponse = new SoapGetProductsResponse { Products = responses.SelectMany( x => x.Products ) };
				return soapGetProductsResponse;
			}
			catch( Exception exc )
			{
				throw new MagentoSoapException( string.Format( "An error occured during GetProductsAsync()" ), exc );
			}
		}

		protected virtual async Task< SoapGetProductsResponse > GetProductsPageAsync( string productType, bool productTypeShouldBeExcluded, string productIdLike, DateTime? updatedFrom, CancellationToken cancellationToken )
		{
			var filters = new filters { filter = new associativeEntity[ 0 ], complex_filter = new complexFilter[ 0 ] };

			if( productType != null )
				AddFilter( filters, productType, "type", productTypeShouldBeExcluded ? "neq" : "eq" );
			if( updatedFrom.HasValue )
				AddFilter( filters, updatedFrom.Value.ToSoapParameterString(), "updated_at", "from" );
			if( !string.IsNullOrWhiteSpace( productIdLike ) )
				AddFilter( filters, productIdLike, "product_id", "like" );

			var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

			return await this.GetWithAsync(
				res => new SoapGetProductsResponse( res ),
				async ( client, session ) => await client.catalogProductListAsync( session, filters, store ).ConfigureAwait( false ), 600000, cancellationToken ).ConfigureAwait( false );
		}

		protected virtual async Task< SoapGetProductsResponse > GetProductsPageAsync( string productType, bool productTypeShouldBeExcluded, IEnumerable< string > skus, DateTime? updatedFrom, CancellationToken cancellationToken )
		{
			var inCondition = string.Join( ",", ( skus ?? Enumerable.Empty< string >() ) );
			var filters = new filters { filter = new associativeEntity[ 0 ], complex_filter = new complexFilter[ 0 ] };

			if( productType != null )
				AddFilter( filters, productType, "type", productTypeShouldBeExcluded ? "neq" : "eq" );
			if( updatedFrom.HasValue )
				AddFilter( filters, updatedFrom.Value.ToSoapParameterString(), "updated_at", "from" );
			if( !string.IsNullOrWhiteSpace( inCondition ) )
				AddFilter( filters, inCondition, "sku", "in" );

			var store = string.IsNullOrWhiteSpace( this.Store ) ? null : this.Store;

			return await this.GetWithAsync(
				res => new SoapGetProductsResponse( res ),
				async ( client, session ) => await client.catalogProductListAsync( session, filters, store ).ConfigureAwait( false ), 600000, cancellationToken ).ConfigureAwait( false );
		}

		public Task< SoapGetProductsResponse > GetProductsBySkusAsync(  IEnumerable< string > skus, CancellationToken cancellationToken, Mark mark = null )
		{
			throw new NotImplementedException();
		}
	}
}