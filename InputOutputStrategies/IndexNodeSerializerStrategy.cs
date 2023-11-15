namespace DataBase_BTree.InputOutputStrategies;

public class IndexNodeSerializerStrategy:ISerializerStrategy
{
    public void Serialize(object obj, Stream stream)
    {
        IndexNode node = (IndexNode)obj;
        int nodeSize = node.GetSize();
        
        stream.Write(BitConverter.GetBytes(nodeSize));
        foreach (var item in node.GetKeys())
        {
            stream.Write(BitConverter.GetBytes(item));
        }

        List<int> pointers = node.GetPointers();
        
        for (int i = 0; i < nodeSize+1; i++)
        {
            stream.Write(BitConverter.GetBytes(pointers[i]));
        }
    }
}