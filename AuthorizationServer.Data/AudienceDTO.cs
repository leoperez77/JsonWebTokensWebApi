using System.Runtime.Serialization;
namespace AuthorizationServer.Data
{
    [DataContract]
    public sealed class AudienceDto
    {
        [DataMember]
        public string ClientId { get; set; }

        [DataMember]
        public string Secret { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Issuer { get; set; }
    }
}
