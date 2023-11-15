using System.Runtime.Serialization;

namespace DataBase_BTree;

public class DataPage: DataBaseNode
{
    public SortedList<int, Record> _data;
    public DataPage()
    {
        _size = 0;
        _data = new SortedList<int, Record>();
    }

    public DataPage(SortedList<int, Record> data, int size)
    {
        _size = size;
        _data = data;
    }

    public override SplitResults<DataBaseNode> Split()
    {
        int middleIndex = (_data.Count - 1) / 2 + 1;
        int middle = _data.GetKeyAtIndex(middleIndex-1);
        int secondPageSize = _size - middleIndex;
        
        SortedList<int, Record> secondPageData = new SortedList<int, Record>(_data.Skip(middleIndex).ToDictionary(x => x.Key, x => x.Value));
        _data = new SortedList<int, Record>(_data.Take(middleIndex).ToDictionary(x => x.Key, x => x.Value));
        
        _size = middleIndex;
        return new SplitResults<DataBaseNode>(false, middle, this, new DataPage(secondPageData, secondPageSize));
    }

    public override int StealFromSibling(DataBaseNode siblingNode, bool isRightSibling, int SplitKey)
    {
        DataPage sibling = (DataPage)siblingNode;
        SortedList<int, Record> union = new SortedList<int, Record>(_data.Union(sibling._data).ToDictionary(x => x.Key, x => x.Value));
        
        int middleIndex = (union.Count - 1) / 2 + 1;
        int middle = union.GetKeyAtIndex(middleIndex-1);
        if (isRightSibling)
        {
            ((DataPage)sibling)._data = new SortedList<int, Record>(union.Skip(middleIndex).ToDictionary(x => x.Key, x => x.Value));
            _data = new SortedList<int, Record>(union.Take(middleIndex).ToDictionary(x => x.Key, x => x.Value));
        }
        else
        { 
            _data = new SortedList<int, Record>(union.Skip(middleIndex).ToDictionary(x => x.Key, x => x.Value));
            sibling._data = new SortedList<int, Record>(union.Take(middleIndex).ToDictionary(x => x.Key, x => x.Value));
        }

        sibling._size = sibling._data.Count;
        _size = _data.Count;
        return middle;
    }

    public override void MergeWith(DataBaseNode siblingNode, int key, bool isRightSibling)
    {
        DataPage sibling = (DataPage)siblingNode;
        _data = new SortedList<int, Record>(_data.Union(sibling._data).ToDictionary(x => x.Key, x => x.Value));
        _size = _data.Count;
    }

    public override bool Add(Record record)
    {
        bool needToSplit = _size == MaxSize;
        _data.Add(record.Key, record);
        _size++;
        return needToSplit;
    }

    public void Delete(int key)
    {
        if (_data.ContainsKey(key))
        {
            _size--;
            _data.Remove(key);
        }
        else
        {
            throw new Exception("Key not found");
        }

    }

    public Record Find(int key)
    {
        if (_data.ContainsKey(key))
        {
            return _data[key];
        }
        else
        {
            throw new Exception("Record doesnt exist");
        }
    }

    public static int GetBinarySize()
    {
        return sizeof(int) + 2*MaxSize * sizeof(int);
    }
    public int GetLastKey()
    {
        return _data.Last().Key;
    }

    public override byte[] Serialize()
    {
        byte[] bytes = new byte[GetBinarySize()];
        BitConverter.GetBytes(_size).CopyTo(bytes,0);
        int index = sizeof(int);

        foreach (var item in _data)
        {
            BitConverter.GetBytes(item.Key).CopyTo(bytes, index);
            BitConverter.GetBytes(item.Value.Value).CopyTo(bytes, index+sizeof(int));
            index += 2 * sizeof(int);
        }

        return bytes;
    }

}