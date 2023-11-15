namespace DataBase_BTree;

public class IntegerType: IDataType
{
    public byte[] SerializeData(object data)
    {
        return BitConverter.GetBytes((int)data);
    }

    public object Parse(byte[] binaryData)
    {
        return BitConverter.ToInt32(binaryData);
    }

    public int GetTypeSize()
    {
        return sizeof(int);
    }
}