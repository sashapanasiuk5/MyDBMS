namespace DataBase_BTree.InputOutputStrategies;

public class IndexNodeParserStrategy:IParserStrategy
{
    public object Parse(Stream stream)
    {
        byte[] intBuffer = new byte[sizeof(int)];
        stream.Read(intBuffer);
        int size = BitConverter.ToInt32(intBuffer);
        SortedSet<int> intermediateKeys = new SortedSet<int>();
        List<int> childPointers = new List<int>();

        int startIndex = sizeof(int);
        if (size != 0)
        {
            
            for (int i = 0; i < size; i++)
            {
                stream.Read(intBuffer);
                int key = BitConverter.ToInt32(intBuffer);
                intermediateKeys.Add(key);
                startIndex += sizeof(int);
            }
        
            for (int i = 0; i < size+1; i++)
            {
                stream.Read(intBuffer);
                int pointer = BitConverter.ToInt32(intBuffer);
                startIndex += sizeof(int);
                childPointers.Add(pointer);
            }
        }

        return new IndexNode(intermediateKeys, childPointers, size);
    }
}