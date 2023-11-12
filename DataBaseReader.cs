namespace DataBase_BTree;

public class DataBaseReader
{
    private Stream _stream;
    public DataBaseReader(Stream stream)
    {
        _stream = stream;
    }
    /*
    public DataBaseNode ReadNode()
    {
        byte[] nodeInBytes = new byte[_indexNodeByteSize];
        _stream.Read(nodeInBytes);
        return ParseIndexNode(nodeInBytes);
    }*/
    
    public object ReadNode(int position)
    {
        byte[] nodeInBytes;
        _stream.Seek(position, SeekOrigin.Begin);
        
        byte[] isPageBuffer = new byte[sizeof(bool)];
        bool isDataPage;
        _stream.Read(isPageBuffer);
        isDataPage = BitConverter.ToBoolean(isPageBuffer,0);

        if (!isDataPage)
        {
            nodeInBytes = new byte[IndexNode.GetBinarySize()];
            _stream.Read(nodeInBytes);
            return ParseIndexNode(nodeInBytes);
        }
        else
        {
            nodeInBytes = new byte[DataPage.GetBinarySize()];
            _stream.Read(nodeInBytes);
            return ParseDataPage(nodeInBytes);
        }
    } 
    
    
    private IndexNode ParseIndexNode(byte[] bytes)
    {
        bool isRoot = BitConverter.ToBoolean(bytes,0);
        int size = BitConverter.ToInt32(bytes, sizeof(bool));
        SortedSet<int> intermediateKeys = new SortedSet<int>();
        List<int> childPointers = new List<int>();

        int startIndex = sizeof(int) + sizeof(bool);
        if (size != 0)
        {
            
            for (int i = 0; i < size; i++)
            {
                int key = BitConverter.ToInt32(bytes, startIndex);
                intermediateKeys.Add(key);
                startIndex += sizeof(int);
            }
        
            for (int i = 0; i < size+1; i++)
            {
                int pointer = BitConverter.ToInt32(bytes, startIndex);
                startIndex += sizeof(int);
                childPointers.Add(pointer);
            }
        }

        if (isRoot)
        {
            return new RootNode(intermediateKeys, childPointers, size);
        }
        return new IndexNode(intermediateKeys, childPointers, size);
    }

    private DataPage ParseDataPage(byte[] bytes)
    {
        int size = BitConverter.ToInt32(bytes, 0);
        int index = sizeof(int); 
        SortedList<int, Record> data = new SortedList<int, Record>();
        int key, value;
        for (int i = 0; i < size; i++)
        {
            key = BitConverter.ToInt32(bytes, index);
            value = BitConverter.ToInt32(bytes, index+sizeof(int));
            data.Add(key,new Record(key,value));
            index += 2*sizeof(int);
        }
        return new DataPage(data, size);
    }
}