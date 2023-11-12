namespace DataBase_BTree;

public class RootNode:IndexNode
{
    public RootNode(int firstDataPagePointer)
    {
        _childPointers.Add(firstDataPagePointer);
    }

    public RootNode(SortedSet<int> intermidiateKeys, List<int> childPointers, int size) : base(intermidiateKeys, childPointers, size){}

    public override SplitResults<IndexNode> Split()
    {
        int middleIndex = (_intermidiateKeys.Count - 1) / 2;
        int middleElement = _intermidiateKeys.ElementAt(middleIndex);
        
        SortedSet<int> firstNodeData = new SortedSet<int>(_intermidiateKeys.Take(middleIndex).ToList());
        SortedSet<int> secondNodeData = new SortedSet<int>(_intermidiateKeys.Skip(middleIndex+1).ToList());

        
        
        List<int> firstPointers = new List<int>(_childPointers.GetRange(0, middleIndex+1));
        List<int> secondPointers = new List<int>(_childPointers.GetRange(middleIndex + 1, _size - middleIndex));
        
        _size = 1;
        _childPointers.Clear();
        _intermidiateKeys.Clear();
        _intermidiateKeys.Add(middleElement);

        IndexNode firstNode = new IndexNode(firstNodeData, firstPointers, firstNodeData.Count);
        IndexNode secondNode = new IndexNode(secondNodeData, secondPointers, secondNodeData.Count);
        return new SplitResults<IndexNode>(true, middleElement, firstNode, secondNode);
    }

    public void AddSplitKey(int splitKey, int firstNodePointer , int secondNodePointer)
    {
        _intermidiateKeys.Add(splitKey);
        _childPointers.Add(firstNodePointer);
        _childPointers.Add(secondNodePointer);
    }


    public override int FindPointer(int key)
    {
        if (_size == 0)
        {
            return _childPointers[0];
        }
        return base.FindPointer(key);
    }
}