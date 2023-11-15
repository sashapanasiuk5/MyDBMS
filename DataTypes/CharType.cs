namespace DataBase_BTree;

public class CharType:IDataType
{
    private int _length;
    public CharType(int length)
    {
        _length = length;
    }

    public byte[] SerializeData(object data)
    {
        return BitConverter.GetBytes((string)data);
    }

    public object Parse(byte[] binaryData)
    {
        return BitConverter.ToString(binaryData);
    }

    public int GetTypeSize()
    {
        return _length;
    }
}