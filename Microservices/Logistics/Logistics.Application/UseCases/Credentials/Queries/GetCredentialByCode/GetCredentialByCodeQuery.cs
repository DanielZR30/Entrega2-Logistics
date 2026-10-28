using Logistics.Application.UseCases.Credentials.Queries.GetCredentialsByEmployee;
using Logistics.Application.Utilities.Mediator;

namespace Logistics.Application.UseCases.Credentials.Queries.GetCredentialByCode
{
    public class GetCredentialByCodeQuery : IRequest<CredentialDTO?>
    {
        public string Code { get; set; } = string.Empty;

        public GetCredentialByCodeQuery(string code)
        {
            Code = code;
        }

        public GetCredentialByCodeQuery() { }
    }
}
