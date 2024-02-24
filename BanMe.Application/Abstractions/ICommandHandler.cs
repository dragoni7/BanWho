using MediatR;

namespace BanMe.Application.Abstractions;
	public interface ICommandHandler<TCommand> : IRequestHandler<TCommand> where TCommand : ICommand
	{
	}
