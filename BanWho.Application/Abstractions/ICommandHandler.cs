using MediatR;

namespace BanWho.Application.Abstractions;
	public interface ICommandHandler<TCommand> : IRequestHandler<TCommand> where TCommand : ICommand
	{
	}
