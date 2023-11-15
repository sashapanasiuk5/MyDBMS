namespace DataBase_BTree;

public class VarcharType:IDataType
{
    private int _length;

    public VarcharType(int length)
    {
        _length = length;
    }
    public byte[] SerializeData(object data)
    {
        throw new NotImplementedException();
    }

    public object Parse(byte[] binaryData)
    {
        throw new NotImplementedException();
    }

    public int GetTypeSize()
    {
        throw new NotImplementedException();
    }
}