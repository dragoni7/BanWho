using MediatR;

namespace BanMeInfrastructure.Messages
{
	public class PatchUpdateMessage : IRequest
	{
		public IServiceProvider ServiceProvider { get; private set; }

		public PatchUpdateMessage(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}
	}
}
