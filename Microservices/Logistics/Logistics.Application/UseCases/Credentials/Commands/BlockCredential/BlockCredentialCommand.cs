using Logistics.Application.Utilities.Mediator;

namespace Logistics.Application.UseCases.Credentials.Commands.BlockCredential
{
    public class BlockCredentialCommand : IRequest<bool>
    {
        public Guid CredentialId { get; set; }

        public BlockCredentialCommand(Guid credentialId)
        {
            CredentialId = credentialId;
        }

        public BlockCredentialCommand() { }
    }
}
