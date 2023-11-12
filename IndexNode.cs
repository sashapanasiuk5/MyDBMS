using System.Runtime.Serialization;

namespace DataBase_BTree;

public class IndexNode: DataBaseNode
{
    public List<int> _childPointers;
    protected int _size;
    public SortedSet<int> _intermidiateKeys;
    public IndexNode(SortedSet<int> intermidiateKeys, List<int> childPointers, int size): base()
    {
        _intermidiateKeys = intermidiateKeys;
        _childPointers = childPointers;
        _size = size;
        
    }

    public IndexNode()
    {
        _childPointers = new List<int>();
        _intermidiateKeys = new SortedSet<int>();
        _size = 0;
    }

    public int GetSize()
    {
        return _size;
    }
    
    public static int GetBinarySize()
    {
        return MaxSize * sizeof(int) + (MaxSize + 1) * sizeof(int) + sizeof(int) + sizeof(bool);
    }

    public override byte[] Serialize()
    {
        byte[] nodeInBytes = new byte[GetBinarySize()];
        bool isRoot = this is RootNode;
        BitConverter.GetBytes(isRoot).CopyTo(nodeInBytes, 0);
        BitConverter.GetBytes(_size).CopyTo(nodeInBytes, sizeof(bool));
        int index = sizeof(bool) + sizeof(int);
        foreach (var item in _intermidiateKeys)
        {
            BitConverter.GetBytes(item).CopyTo(nodeInBytes, index);
            index += sizeof(int);
        }

        if (_size == 0 && this is not RootNode)
            return nodeInBytes;
        
        for (int i = 0; i < _size+1; i++)
        {
            BitConverter.GetBytes(_childPointers[i]).CopyTo(nodeInBytes, index);
            index += sizeof(int);
        }

        return nodeInBytes;
    }
    public override bool Add(Record record)
    {
        bool needToSplit = _size == MaxSize;
        
        _intermidiateKeys.Add(record.Key);
        
        int index = _intermidiateKeys.ToList().IndexOf(record.Key)+1;
        
        _size++;
        if (index == _childPointers.Count)
        {
            _childPointers.Add(record.Value);
        }
        else
        {
            _childPointers.Insert(index,record.Value);  
        }

        return needToSplit;
    }

    public bool Delete(int key, int pointer)
    {
        bool needToMerge = _size == MinSize;

        
        return needToMerge;
    }

    public void SetNewKey(int fistPointer, int secondPointer, int key)
    {
        int firstPointerIndex = _childPointers.IndexOf(fistPointer);
        int secondPointerIndex = _childPointers.IndexOf(secondPointer);
        int index = (int)Math.Floor((double)(firstPointerIndex + secondPointerIndex) / 2);
        _intermidiateKeys.Remove(_intermidiateKeys.ElementAt(index));
        _intermidiateKeys.Add(key);
    }
    
    public virtual int FindPointer(int key)
    {
        int first = 0;
        int last = _intermidiateKeys.Count-1;
        int middle = 0;
        while (first <= last && middle < _intermidiateKeys.Count && middle >= 0)
        {
            middle = (int)Math.Floor((double)((first+last)/2));
            int middleSecond = middle;
            int middleFirst;
            if (middleSecond != 0)
            {
                middleFirst = middleSecond - 1;
            }
            else
            {
                middleFirst = middleSecond;
            }
            
            if (_intermidiateKeys.ElementAt(middleFirst) >= key)
            {
                last = middle - 1;
            }else if (_intermidiateKeys.ElementAt(middleSecond) < key)
            {
                first = middle + 1;
            }
            else
            {
                return _childPointers[middle];
            }
        }
        
        return _childPointers[first];;
    }

    public override SplitResults<DataBaseNode> Split()
    {
        int middleIndex = (_intermidiateKeys.Count - 1) / 2;
        int middleElement = _intermidiateKeys.ElementAt(middleIndex);
        
        SortedSet<int> secondNodeData = new SortedSet<int>(_intermidiateKeys.Skip(middleIndex+1).ToList());
        _intermidiateKeys = new SortedSet<int>(_intermidiateKeys.Take(middleIndex).ToList());
        
        List<int> secondNodePointers = new List<int>(_childPointers.GetRange(middleIndex + 1, _size - middleIndex));
        _childPointers.RemoveRange(middleIndex+1, _childPointers.Count-middleIndex-1);
        _size = _intermidiateKeys.Count;
        IndexNode secondNode = new IndexNode(secondNodeData, secondNodePointers, secondNodeData.Count);
        return new SplitResults<DataBaseNode>(false, middleElement, this, secondNode);
    }

    public (int firstPointer, int? secondPointer) GetPointerSiblings(int pointer)
    {
        int index = _childPointers.IndexOf(pointer);
        if (index == 0)
        {
            return (_childPointers[1], null);
        }
        else if (index == _childPointers.Count - 1)
        {
            return (_childPointers[index - 1], null);
        }
        else
        {
            return (_childPointers[index - 1], _childPointers[index + 1]);
        }
    }
}