using DataBase_BTree.InputOutputStrategies;

namespace DataBase_BTree;

public class TableReader
{
    private Stream _stream;
    
    private int _indexNodeSize;
    private int _dataPageSize;
    
    private Dictionary<string, IDataType> _template;
    public TableReader(Stream stream, int indexNodeSize, int dataPageSize, Dictionary<string, IDataType> template)
    {
        _stream = stream;
        _template = template;
        _indexNodeSize = indexNodeSize;
        _dataPageSize = dataPageSize;
    }
    public DataBaseNode ReadNode(int position)
    {
        byte[] nodeInBytes;
        _stream.Seek(position, SeekOrigin.Begin);
        
        byte[] isPageBuffer = new byte[sizeof(bool)];
        bool isDataPage;
        _stream.Read(isPageBuffer);
        isDataPage = BitConverter.ToBoolean(isPageBuffer,0);

        IParserStrategy parserStrategy;
        if (!isDataPage)
        {
            nodeInBytes = new byte[_indexNodeSize];
            _stream.Read(nodeInBytes);
            parserStrategy = new IndexNodeParserStrategy();
        }
        else
        {
            nodeInBytes = new byte[_dataPageSize];
            _stream.Read(nodeInBytes);
            parserStrategy = new DataPageParserStrategy(_template, 0);
        }

        return (DataBaseNode)parserStrategy.Parse(nodeInBytes);
    } 
    
    
}