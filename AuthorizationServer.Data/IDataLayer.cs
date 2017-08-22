using System.Collections.Generic;

namespace AuthorizationServer.Data
{
    public interface IDataLayer
    {
        AudienceDto GetAudience(string clientId);
        IEnumerable<AudienceDto> GetAll();
    }
}
