using System.Text;
using DataBase_BTree;
using DataBase_BTree.InputOutputStrategies;

namespace IndexedStructure;

public class DataBaseInfoWriter
{
    /*private Dictionary<int, IDataType> _availableTypes;
    public DataBaseInfoWriter(Dictionary<int, IDataType> availableTypes)
    {
        _availableTypes = availableTypes;
    }*/
    public void Write(Stream stream, List<DataBaseInfo> infos)
    {
        stream.Seek(0, SeekOrigin.Begin);
        stream.Write(BitConverter.GetBytes(infos.Count));
        foreach (var info in infos)
        {
            int tableNameSize = info.TableName.Length;
            stream.Write(BitConverter.GetBytes(tableNameSize));
            stream.Write(Encoding.UTF8.GetBytes(info.TableName));
            stream.Write(BitConverter.GetBytes(info.Template.Count));
            foreach (var columnTemplate in info.Template)
            {
                stream.Write(BitConverter.GetBytes(columnTemplate.Key.Length));
                stream.Write(Encoding.UTF8.GetBytes(columnTemplate.Key));
                stream.WriteByte(Convert.ToByte(GetTypeID(columnTemplate.Value)));
                stream.Write(BitConverter.GetBytes(columnTemplate.Value.GetTypeArgument()));
            }
            
            stream.WriteByte(Convert.ToByte(info.IndexOfKey));
            stream.Write(BitConverter.GetBytes(info.IndexStructurePointer));
            stream.Write(BitConverter.GetBytes(info.RootNodePointer));
            stream.Write(BitConverter.GetBytes(info.IndexStructureMaxSize));
        }
    }

    private int GetTypeID(IDataType type)
    {
        switch (type)
        {
            case IntegerType integerType:
                return 0;
            case CharType charType:
                return 1;
            case VarcharType varcharType:
                return 2;
            default:
                throw new Exception("Data type is not supported");
        }
    }
}