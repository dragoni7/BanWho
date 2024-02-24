using MediatR;

namespace BanMe.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<TResponse>
{

}
