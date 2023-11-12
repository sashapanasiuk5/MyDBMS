namespace DataBase_BTree;

public class DataBaseWriter
{
    private Stream _stream;
    private int _startOfIndexArea;
    private int _endOfIndexArea;
    private int _startOfDataPageArea;
    private int _endOfDataPageArea;



    public DataBaseWriter(Stream stream, int startOfIndexArea, int startOfDataPageArea)
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
        byte[] bytes = node.Serialize();
        _stream.Write(BitConverter.GetBytes(isDataPage));
        _stream.Write(bytes);

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
        byte[] bytes = node.Serialize();
        _stream.Seek(pointer, SeekOrigin.Begin);
        _stream.Write(BitConverter.GetBytes(isDataPage));
        _stream.Write(bytes);
        return pointer;
    }

}