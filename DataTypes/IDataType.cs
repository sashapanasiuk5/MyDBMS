namespace DataBase_BTree;

public interface IDataType
{
    public byte[] SerializeData(object data);

    public object Parse(byte[] binaryData);

    public int GetTypeSize();
}