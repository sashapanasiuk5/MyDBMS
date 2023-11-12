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
    
    public int WriteIndexNode(IndexNode node)
    {
        int nodePointer = _endOfIndexArea;
        _stream.Seek(_endOfIndexArea, SeekOrigin.Begin);
        _stream.Write(BitConverter.GetBytes(false));
        byte[] bytes = node.Serialize();

        _stream.Write(bytes);

        _endOfIndexArea = (int)_stream.Position;
        return nodePointer;
    }
    
    public int WriteIndexNode(IndexNode node, int pointer)
    {
        byte[] bytes = node.Serialize();
        _stream.Seek(pointer, SeekOrigin.Begin);
        _stream.Write(BitConverter.GetBytes(false));
        _stream.Write(bytes);
        return pointer;
    }
    
    public int WriteDataPage(DataPage node)
    {
        int pointer = _endOfDataPageArea;
        _stream.Seek(pointer, SeekOrigin.Begin);
        _stream.Write(BitConverter.GetBytes(true));
        byte[] bytes = node.Serialize();
        _stream.Write(bytes);
        
        _endOfDataPageArea = (int)_stream.Position;
        return pointer;
    }
    
    public int WriteDataPage(DataPage node, int pointer)
    {
        byte[] bytes = node.Serialize();
        _stream.Seek(pointer, SeekOrigin.Begin);
        _stream.Write(BitConverter.GetBytes(true));
        _stream.Write(bytes);
        return pointer;
    }
}