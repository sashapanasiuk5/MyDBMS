namespace DataBase_BTree.InputOutputStrategies;

public class IndexNodeParserStrategy:IParserStrategy
{
    public object Parse(byte[] bytes)
    {
        int size = BitConverter.ToInt32(bytes);
        SortedSet<int> intermediateKeys = new SortedSet<int>();
        List<int> childPointers = new List<int>();

        int startIndex = sizeof(int);
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

        return new IndexNode(intermediateKeys, childPointers, size);
    }
}