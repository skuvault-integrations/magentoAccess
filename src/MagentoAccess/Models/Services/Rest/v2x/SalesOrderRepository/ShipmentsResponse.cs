using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagentoAccess.Models.Services.Rest.v2x.SalesOrderRepository
{
	[ DataContract ]
	public class ShipmentsResponse
	{
		[ DataMember( Name = "items" ) ]
		public IEnumerable< ShipmentResponse > Items { get; set; }

		[ DataMember( Name = "total_count" ) ]
		public int TotalCount { get; set; }
	}

	[ DataContract ]
	public class ShipmentResponse
	{
		[ DataMember( Name = "created_at" ) ]
		public DateTime CreatedAt { get; set; }

		[ DataMember( Name = "updated_at" ) ]
		public DateTime UpdatedAt { get; set; }

		[ DataMember( Name = "entity_id" ) ]
		public long EntityId { get; set; }

		[ DataMember( Name = "increment_id" ) ]
		public string Id { get; set; }

		[ DataMember( Name = "order_id" ) ]
		public string OrderId { get; set; }

		public string OrderIncrementId { get; set; }

		[ DataMember( Name = "items" ) ]
		public IEnumerable< ShipmentResponseItem > Items { get; set; }

		[ DataMember( Name = "tracks" ) ]
		public IEnumerable< ShipmentResponseTrack > Tracks { get; set; }

		[ DataMember( Name = "comments" ) ]
		public IEnumerable< ShipmentResponseComment > Comments { get; set; }
	}

	[ DataContract ]
	public class ShipmentResponseItem
	{
		[ DataMember( Name = "name" ) ]
		public string Name { get; set; }

		[ DataMember( Name = "price" ) ]
		public decimal? Price { get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku { get; set; }

		[ DataMember( Name = "weight" ) ]
		public decimal? Weight { get; set; } 

		[ DataMember( Name = "qty" ) ]
		public int Quantity { get; set; }
	}

	[ DataContract ]
	public class ShipmentResponseTrack
	{
		[ DataMember( Name = "order_id" ) ]
		public long OrderId { get; set; }
		
		[ DataMember( Name = "created_at" ) ]
		public DateTime CreatedAt { get; set; }
		
		[ DataMember( Name = "updated_at" ) ]
		public DateTime UpdatedAt { get; set; }
		
		[ DataMember( Name = "track_number" ) ]
		public string TrackingNumber { get; set; }
		
		[ DataMember( Name = "title" ) ]
		public string Title { get; set; }
		
		[ DataMember( Name = "carrier_code" ) ]
		public string CarrierCode { get; set; }
	}

	public class ShipmentResponseComment
	{
		[ DataMember( Name = "created_at" ) ]
		public DateTime CreatedAt { get; set; }
		
		[ DataMember( Name = "comment" ) ]
		public string Comment { get; set; }
	}
}