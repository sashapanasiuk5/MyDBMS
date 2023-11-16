namespace DataBase_BTree;

public class IntegerType: IDataType
{
    public byte[] SerializeData(object data)
    {
        return BitConverter.GetBytes((int)data);
    }

    public IDataType Clone()
    {
        return new IntegerType();
    }

    public object Parse(Stream stream)
    {
        byte[] intBuffer = new byte[sizeof(int)];
        stream.Read(intBuffer);
        return BitConverter.ToInt32(intBuffer);
    }

    public int GetTypeArgument() => 0;

    public int GetTypeSize()
    {
        return sizeof(int);
    }
}