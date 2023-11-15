using DataBase_BTree.InputOutputStrategies;

namespace DataBase_BTree;

public class TableReader
{
    private Stream _stream;
    
    private int _indexNodeSize;
    private int _dataPageSize;

    private int _indexKeyField;
    
    private Dictionary<string, IDataType> _template;
    public TableReader(Stream stream, Dictionary<string, IDataType> template, int indexKeyField)
    {
        _stream = stream;
        _template = template;
        _indexKeyField = indexKeyField;
    }
    public DataBaseNode ReadNode(int position)
    {
        List<byte> bytes = new List<byte>();
        _stream.Seek(position, SeekOrigin.Begin);
        
        byte[] isPageBuffer = new byte[sizeof(bool)];
        bool isDataPage;
        _stream.Read(isPageBuffer);
        isDataPage = BitConverter.ToBoolean(isPageBuffer,0);

        IParserStrategy parserStrategy;
        if (!isDataPage)
        {
            parserStrategy = new IndexNodeParserStrategy();
        }
        else
        {
            parserStrategy = new DataPageParserStrategy(_template, _indexKeyField);
        }

        return (DataBaseNode)parserStrategy.Parse(_stream);
    } 
    
    
}