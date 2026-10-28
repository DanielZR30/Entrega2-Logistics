using Logistics.Application.Utilities.Mediator;

namespace Logistics.Application.UseCases.Credentials.Commands.RevokeCredential
{
    public class RevokeCredentialCommand : IRequest<bool>
    {
        public Guid CredentialId { get; set; }

        public RevokeCredentialCommand(Guid credentialId)
        {
            CredentialId = credentialId;
        }

        public RevokeCredentialCommand() { }
    }
}
