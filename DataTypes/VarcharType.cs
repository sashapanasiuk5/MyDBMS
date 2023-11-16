using System.Text;

namespace DataBase_BTree;

public class VarcharType:IDataType
{
    private int _length;

    public VarcharType(int length)
    {
        _length = length;
    }
    public IDataType Clone()
    {
        return new VarcharType(_length);
    }
    public byte[] SerializeData(object data)
    {
        int size = ((string)data).Length;
        byte[] bytes = new byte[size + sizeof(int)];
        BitConverter.GetBytes(size).CopyTo(bytes,0);
        Encoding.UTF8.GetBytes((string)data).CopyTo(bytes,sizeof(int));
        return bytes;
    }

    public object Parse(Stream stream)
    {
        byte[] intBuffer = new byte[sizeof(int)];
        stream.Read(intBuffer);
        int size = BitConverter.ToInt32(intBuffer);

        byte[] stringBuffer = new byte[size];
        stream.Read(stringBuffer);
        return Encoding.UTF8.GetString(stringBuffer, 0, size);
    }

    public int GetTypeArgument() => _length;

    public int GetTypeSize()
    {
        return _length + sizeof(int);
    }
}