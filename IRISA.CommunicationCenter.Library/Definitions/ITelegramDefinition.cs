using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Library.Definitions
{
    public interface ITelegramDefinition
    {
        int Id { get; }
        string Name { get; }
        string Description { get; }
        string Source { get; }
        string Destination { get; }
        int? ExpiryInMinutes { get; }
        IEnumerable<IFieldDefinition> Fields { get; }
    }
}