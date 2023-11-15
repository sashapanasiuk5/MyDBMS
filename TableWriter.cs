using DataBase_BTree.InputOutputStrategies;

namespace DataBase_BTree;

public class TableWriter
{
    private Stream _stream;
    private int _startOfIndexArea;
    private int _endOfIndexArea;
    private int _startOfDataPageArea;
    private int _endOfDataPageArea;

    private int _indexNodeSize;
    private int _dataPageSize;

    public TableWriter(Stream stream, int startOfIndexArea, int startOfDataPageArea)
    {
        _stream = stream;
        _startOfIndexArea = startOfIndexArea;
        _endOfIndexArea = startOfIndexArea;
        _startOfDataPageArea = startOfDataPageArea;
        _endOfDataPageArea = startOfDataPageArea;

    }
    
    public int WriteNode(DataBaseNode node)
    {
        bool isDataPage = node is DataPage;
        int nodePointer = isDataPage ? _endOfDataPageArea : _endOfIndexArea;
        _stream.Seek(nodePointer, SeekOrigin.Begin);
        
        ISerializerStrategy serializerStrategy = ChooseStrategy(node);
        _stream.Write(BitConverter.GetBytes(isDataPage));
        serializerStrategy.Serialize(node, _stream);

        if (isDataPage)
        {
            _endOfDataPageArea = (int)_stream.Position;
        }
        else
        {
            _endOfIndexArea = (int)_stream.Position;
        }
        return nodePointer;
    }
    public int WriteNode(DataBaseNode node, int pointer)
    {
        bool isDataPage = node is DataPage;
        ISerializerStrategy serializerStrategy = ChooseStrategy(node);
        
        _stream.Seek(pointer, SeekOrigin.Begin);
        _stream.Write(BitConverter.GetBytes(isDataPage));
        serializerStrategy.Serialize(node, _stream);
        return pointer;
    }

    private ISerializerStrategy ChooseStrategy(DataBaseNode node)
    {
        switch (node)
        {
            case IndexNode indexNode:
                return new IndexNodeSerializerStrategy();
                break;
            case DataPage page:
                return new DataPageSerializerStrategy();
            default:
                throw new Exception("Node is not supported");
        }
    }

}