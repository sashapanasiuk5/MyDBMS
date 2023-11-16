namespace DataBase_BTree;

public interface IDataType
{
    public byte[] SerializeData(object data);

    public object Parse(Stream stream);

    public int GetTypeSize();

    public IDataType Clone();

    public int GetTypeArgument();
}