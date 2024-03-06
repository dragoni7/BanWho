using MediatR;

namespace BanWho.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<TResponse>
{

}
