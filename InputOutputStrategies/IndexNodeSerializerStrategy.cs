namespace DataBase_BTree.InputOutputStrategies;

public class IndexNodeSerializerStrategy:ISerializerStrategy
{
    private int _binarySize;

    public IndexNodeSerializerStrategy(int binarySize)
    {
        _binarySize = binarySize;
    }
    public byte[] Serialize(object obj)
    {
        IndexNode node = (IndexNode)obj;
        int nodeSize = node.GetSize();
        
        byte[] nodeInBytes = new byte[_binarySize];
        BitConverter.GetBytes(nodeSize).CopyTo(nodeInBytes, 0);
        int index = sizeof(int);
        foreach (var item in node.GetKeys())
        {
            BitConverter.GetBytes(item).CopyTo(nodeInBytes, index);
            index += sizeof(int);
        }

        List<int> pointers = node.GetPointers();
        
        for (int i = 0; i < nodeSize+1; i++)
        {
            BitConverter.GetBytes(pointers[i]).CopyTo(nodeInBytes, index);
            index += sizeof(int);
        }

        return nodeInBytes;
    }
}