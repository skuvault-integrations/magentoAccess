using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce
{
	internal class ClientMessageInspector : IClientMessageInspector
	{
		public string AccessToken;

		public object BeforeSendRequest( ref Message request, IClientChannel channel )
		{
			//legacy behaviour
			HttpRequestMessageProperty httpRequestMessage;
			object httpRequestMessageObject;
			if( request.Properties.TryGetValue( HttpRequestMessageProperty.Name, out httpRequestMessageObject ) )
			{
				httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
				if( string.IsNullOrEmpty( httpRequestMessage.Headers[ "Accept-Encoding" ] ) )
					httpRequestMessage.Headers.Remove( "Accept-Encoding" );
			}
			else
			{
				httpRequestMessage = new HttpRequestMessageProperty();
				httpRequestMessage.Headers.Add( "Accept-Encoding", "" );
				request.Properties.Add( HttpRequestMessageProperty.Name, httpRequestMessage );

			}

			//Auth Mangeot 2.0
			httpRequestMessage.Headers.Add("Authorization", "Bearer " + AccessToken);

			return null;
		}

		public void AfterReceiveReply( ref Message reply, object correlationState )
		{
			var prop =
				reply.Properties[ HttpResponseMessageProperty.Name.ToString() ] as HttpResponseMessageProperty;

			if( prop != null )
			{
				// get the content type headers
				var contentType = prop.Headers[ "Content-Type" ];
			}
		}
	}

	internal class CustomBehavior : IEndpointBehavior
	{
		public string AccessToken;

		public void AddBindingParameters( ServiceEndpoint serviceEndpoint,
			BindingParameterCollection bindingParameters )
		{
			try
			{
				if( serviceEndpoint != null && serviceEndpoint.Behaviors != null )
				{
					var vs = serviceEndpoint.Behaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vs != null && vs.Any() )
						serviceEndpoint.Behaviors.Remove( vs.Single() );
				}
			}
			catch( Exception )
			{
			}
		}

		public void ApplyClientBehavior( ServiceEndpoint serviceEndpoint,
			ClientRuntime behavior )
		{
			try
			{
				//Add the inspector
				behavior.MessageInspectors.Add(new ClientMessageInspector() { AccessToken = this.AccessToken });
				if( serviceEndpoint != null && serviceEndpoint.Behaviors != null )
				{
					var vsBehaviour = serviceEndpoint.Behaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vsBehaviour != null && vsBehaviour.Any() )
						serviceEndpoint.Behaviors.Remove( vsBehaviour.Single() );
				}

				//behavior.CallbackDispatchRuntime.MessageInspectors.Add(new MessageInspector2());

				var inspector = new ClientMessageInspector() { AccessToken = this.AccessToken };
				behavior.MessageInspectors.Add( inspector );
			}
			catch( Exception )
			{
			}
		}

		public void ApplyDispatchBehavior( ServiceEndpoint serviceEndpoint,
			EndpointDispatcher endpointDispatcher )
		{
			try
			{
				if( serviceEndpoint != null && serviceEndpoint.Behaviors != null )
				{
					var vsBehaviour = serviceEndpoint.Behaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vsBehaviour != null && vsBehaviour.Any() )
						serviceEndpoint.Behaviors.Remove( vsBehaviour.Single() );
				}
			}
			catch( Exception )
			{
			}
		}

		public void Validate( ServiceEndpoint serviceEndpoint )
		{
			try
			{
				if( serviceEndpoint != null && serviceEndpoint.Behaviors != null )
				{
					var vsBehaviour = serviceEndpoint.Behaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vsBehaviour != null && vsBehaviour.Any() )
						serviceEndpoint.Behaviors.Remove( vsBehaviour.Single() );
				}
			}
			catch( Exception )
			{
			}
		}
	}
}
