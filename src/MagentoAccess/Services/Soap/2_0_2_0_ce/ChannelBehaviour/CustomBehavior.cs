using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace MagentoAccess.Services.Soap._2_0_2_0_ce.ChannelBehaviour
{
	internal class CustomBehavior : IEndpointBehavior
	{
		public string AccessToken;
		public bool LogRawMessages { get; set; } = false;

		public void AddBindingParameters( ServiceEndpoint serviceEndpoint,
			BindingParameterCollection bindingParameters )
		{
			try
			{
				if( serviceEndpoint != null && serviceEndpoint.EndpointBehaviors != null )
				{
					var vs = serviceEndpoint.EndpointBehaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vs != null && vs.Any() )
						serviceEndpoint.EndpointBehaviors.Remove( vs.Single() );
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
				behavior.ClientMessageInspectors.Add( new ClientMessageInspector() { AccessToken = this.AccessToken, LogRawMessages = this.LogRawMessages} );
				if( serviceEndpoint != null && serviceEndpoint.EndpointBehaviors != null )
				{
					var vsBehaviour = serviceEndpoint.EndpointBehaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vsBehaviour != null && vsBehaviour.Any() )
						serviceEndpoint.EndpointBehaviors.Remove( vsBehaviour.Single() );
				}

				//behavior.CallbackDispatchRuntime.MessageInspectors.Add(new MessageInspector2());

				var inspector = new ClientMessageInspector() { AccessToken = this.AccessToken, LogRawMessages = this.LogRawMessages };
				behavior.ClientMessageInspectors.Add( inspector );
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
				if( serviceEndpoint != null && serviceEndpoint.EndpointBehaviors != null )
				{
					var vsBehaviour = serviceEndpoint.EndpointBehaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vsBehaviour != null && vsBehaviour.Any() )
						serviceEndpoint.EndpointBehaviors.Remove( vsBehaviour.Single() );
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
				if( serviceEndpoint != null && serviceEndpoint.EndpointBehaviors != null )
				{
					var vsBehaviour = serviceEndpoint.EndpointBehaviors.Where( i => i.GetType().Namespace.Contains( "VisualStudio" ) );
					if( vsBehaviour != null && vsBehaviour.Any() )
						serviceEndpoint.EndpointBehaviors.Remove( vsBehaviour.Single() );
				}
			}
			catch( Exception )
			{
			}
		}
	}
}