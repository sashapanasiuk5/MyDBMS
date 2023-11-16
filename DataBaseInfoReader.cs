using System.Text;

namespace DataBase_BTree.InputOutputStrategies;

public struct DataBaseInfo
{
    public string TableName { get; private set; }
    public Dictionary<string, IDataType> Template { get; private set; }
    public int IndexOfKey { get; private set; }
    public int RootNodePointer { get; private set; }
    public int IndexStructurePointer { get; private set; }
    public int IndexStructureMaxSize { get; private set; }

    public DataBaseInfo(string tableName, Dictionary<string, IDataType> template, int indexOfKey, int indexStructurePointer, int indexStructureMaxSize, int rootNodePointer)
    {
        TableName = tableName;
        Template = template;
        IndexOfKey = indexOfKey;
        IndexStructurePointer = indexStructurePointer;
        IndexStructureMaxSize = indexStructureMaxSize;
        RootNodePointer = rootNodePointer;
    }
}


public class DataBaseInfoReader
{
    private int _maxSizeOfArea;
    private Dictionary<int, IDataType> _availableTypes;
    public DataBaseInfoReader(int maxSizeOfArea)
    {
        _maxSizeOfArea = maxSizeOfArea;
        //_availableTypes = availableTypes;
    }
    public List<DataBaseInfo> Read(Stream stream)
    {
        List<DataBaseInfo> infoList = new List<DataBaseInfo>();
        stream.Seek(0, SeekOrigin.Begin);
        byte[] intBuffer = new byte[sizeof(int)];
        stream.Read(intBuffer);
        int countOfTables = BitConverter.ToInt32(intBuffer);
        for (int i = 0; i < countOfTables; i++)
        {
            
            stream.Read(intBuffer);
            int nameSize = BitConverter.ToInt32(intBuffer);
            byte[] nameBuffer = new byte[nameSize];
            stream.Read(nameBuffer);
            string tableName = Encoding.UTF8.GetString(nameBuffer);

            stream.Read(intBuffer);
            int countOfColumns = BitConverter.ToInt32(intBuffer);
            Dictionary<string, IDataType> _columnInfo = new Dictionary<string, IDataType>();
            for (int j = 0; j < countOfColumns; j++)
            {
                stream.Read(intBuffer);
                int columnNameSize = BitConverter.ToInt32(intBuffer);
                byte[] columnNameBuffer = new byte[columnNameSize];
                stream.Read(columnNameBuffer);
                string columnName = Encoding.UTF8.GetString(columnNameBuffer);
                int typeID = stream.ReadByte();

                stream.Read(intBuffer);
                int typeArgument = BitConverter.ToInt32(intBuffer);
                _columnInfo.Add(columnName, GetType(typeID, typeArgument));
                
            }

            int indexOfKey = stream.ReadByte();
            stream.Read(intBuffer);
            int indexStructurePointer = BitConverter.ToInt32(intBuffer);
            
            stream.Read(intBuffer);
            int rootNodePointer = BitConverter.ToInt32(intBuffer);
            
            stream.Read(intBuffer);
            int indexStructureSize = BitConverter.ToInt32(intBuffer);
            DataBaseInfo info = new DataBaseInfo(tableName, _columnInfo, indexOfKey, indexStructurePointer,
                indexStructureSize, rootNodePointer);
            infoList.Add(info);
        }
        return infoList;
    }
    
    private IDataType GetType(int typeID, int typeArgument)
    {
        switch (typeID)
        {
            case 0:
                return new IntegerType();
            case 1:
                return new CharType(typeArgument);
            case 2:
                return new VarcharType(typeArgument);
            default:
                throw new Exception("Incorrect typeID");
        }
    }

}