using System.Runtime.Serialization;

namespace DataBase_BTree;

public class DataPage: DataBaseNode
{
    private int _size;
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

    public int StealFromSibling(DataPage sibling, bool isRightSibling)
    {
        SortedList<int, Record> union = new SortedList<int, Record>(_data.Union(sibling._data).ToDictionary(x => x.Key, x => x.Value));
        
        int middleIndex = (union.Count - 1) / 2 + 1;
        int middle = union.GetKeyAtIndex(middleIndex-1);
        if (isRightSibling)
        {
            sibling._data = new SortedList<int, Record>(union.Skip(middleIndex).ToDictionary(x => x.Key, x => x.Value));
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

    public void MergeWith(DataPage page)
    {
        _data = new SortedList<int, Record>(_data.Union(page._data).ToDictionary(x => x.Key, x => x.Value));
        _size = _data.Count;
    }

    public override bool Add(Record record)
    {
        bool needToSplit = _size == MaxSize;
        _data.Add(record.Key, record);
        _size++;
        return needToSplit;
    }

    public bool Delete(int key)
    {
        bool needToMerge = _size == MinSize;
        _size--;
        _data.Remove(key);
        return needToMerge;
    }

    public Record Find(int key) => _data[key];

    public static int GetBinarySize()
    {
        return sizeof(int) + 2*MaxSize * sizeof(int);
    }

    public bool CanSplit()
    {
        return _size > MinSize;
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