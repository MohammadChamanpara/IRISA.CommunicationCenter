namespace IRISA.CommunicationCenter.Library.Definitions
{
    public interface IFieldDefinition
    {
        string Name { get; }
        string Type { get; }
        int Size { get; }
        bool IsArray { get; }
        string GetValue(byte[] fieldBytes);
        byte[] GetBytes(string field);
    }
}
