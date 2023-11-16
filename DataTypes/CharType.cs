using System.Text;

namespace DataBase_BTree;

public class CharType:IDataType
{
    private int _length;
    public CharType(int length)
    {
        _length = length;
    }

    public IDataType Clone()
    {
        return new CharType(_length);
    }

    public byte[] SerializeData(object data)
    {
        return Encoding.UTF8.GetBytes((string)data);
    }

    public object Parse(Stream stream)
    {
        byte[] stringBuffer = new byte[_length];
        stream.Read(stringBuffer);
        return System.Text.Encoding.UTF8.GetString(stringBuffer, 0, _length);
    }
    public int GetTypeArgument() => _length;

    public int GetTypeSize()
    {
        return _length;
    }
}