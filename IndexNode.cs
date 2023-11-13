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

    public override bool Delete(int pointer)
    {
        bool needToMerge = _size <= MinSize;

        int key = FindKey(pointer);
        _intermidiateKeys.Remove(key);
        _childPointers.Remove(pointer);
        _size--;
        return needToMerge;
    }

    public override int StealFromSibling(DataBaseNode siblingNode, bool isRightSibling, int SplitKey)
    {
        IndexNode sibling = (IndexNode)siblingNode;
        SortedSet<int> unionKeys = new SortedSet<int>(_intermidiateKeys.Union(sibling._intermidiateKeys));
        unionKeys.Add(SplitKey);

        if (isRightSibling)
        {
            _childPointers.AddRange(sibling._childPointers);
        }
        else
        {
            _childPointers.InsertRange(0,sibling._childPointers);
        }

        int middleIndex = (unionKeys.Count - 1) / 2;
        int middle = unionKeys.ElementAt(middleIndex);
        if (isRightSibling)
        {
            ((IndexNode)sibling)._intermidiateKeys = new SortedSet<int>(unionKeys.Skip(middleIndex+1).ToList());
            _intermidiateKeys = new SortedSet<int>(unionKeys.Take(middleIndex).ToList());

            ((IndexNode)sibling)._childPointers =
                _childPointers.GetRange(middleIndex + 1, _childPointers.Count - middleIndex - 1);
            
            _childPointers.RemoveRange(middleIndex + 1, _childPointers.Count - middleIndex - 1);
        }
        else
        { 
            _intermidiateKeys = new SortedSet<int>(unionKeys.Skip(middleIndex+1).ToList());
            ((IndexNode)sibling)._intermidiateKeys = new SortedSet<int>(unionKeys.Take(middleIndex).ToList());
            
            ((IndexNode)sibling)._childPointers =
                _childPointers.GetRange(0, _childPointers.Count - middleIndex - 1);
            
            _childPointers.RemoveRange(0, _childPointers.Count - middleIndex - 1);
        }

        sibling._size = sibling._intermidiateKeys.Count;
        _size = _intermidiateKeys.Count;
        return middle;
    }

    public bool HasKey(int key)
    {
        return _intermidiateKeys.Contains(key);
    }

    public void ReplaceKey(int oldKey, int newKey)
    {
        _intermidiateKeys.Remove(oldKey);
        _intermidiateKeys.Add(newKey);
    }
    public void SetNewKey(int fistPointer, int secondPointer, int key)
    {
        int firstPointerIndex = _childPointers.IndexOf(fistPointer);
        int secondPointerIndex = _childPointers.IndexOf(secondPointer);
        int index = (int)Math.Floor((double)(firstPointerIndex + secondPointerIndex) / 2);
        _intermidiateKeys.Remove(_intermidiateKeys.ElementAt(index));
        _intermidiateKeys.Add(key);
    }

    public virtual int FindKey(int pointer)
    {
        int index = _childPointers.IndexOf(pointer);
        if (index == _childPointers.Count - 1)
            return _intermidiateKeys.Last();

        return _intermidiateKeys.ElementAt(index);
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
    public override bool CanSplit()
    {
        return _size > MinSize;
    }
    
    public override void MergeWith(DataBaseNode siblingNode, int key, bool isRightSibling)
    {
        IndexNode sibling = (IndexNode)siblingNode;
        _intermidiateKeys = new SortedSet<int>(_intermidiateKeys.Union(sibling._intermidiateKeys));
        _intermidiateKeys.Add(key);
        sibling._childPointers.Reverse();
        foreach (var pointer in sibling._childPointers)
        {
            if (isRightSibling)
            {
                _childPointers.Insert(0, pointer);
            }
            else
            {
                _childPointers.Add(pointer);
            }
        }
        _size = _intermidiateKeys.Count;
    }

    public (int? firstPointer, int? secondPointer) GetPointerSiblings(int pointer)
    {
        int index = _childPointers.IndexOf(pointer);
        if (index == 0)
        {
            return (null, _childPointers[1]);
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