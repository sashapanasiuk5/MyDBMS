namespace DataBase_BTree;

public class IntegerType: IDataType
{
    public byte[] SerializeData(object data)
    {
        return BitConverter.GetBytes((int)data);
    }

    public object Parse(Stream stream)
    {
        byte[] intBuffer = new byte[sizeof(int)];
        stream.Read(intBuffer);
        return BitConverter.ToInt32(intBuffer);
    }

    public int GetTypeSize()
    {
        return sizeof(int);
    }
}