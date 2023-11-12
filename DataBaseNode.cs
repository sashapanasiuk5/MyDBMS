using System.Runtime.Serialization;

namespace DataBase_BTree;

public abstract class DataBaseNode:IBinarySerializable
{
    public const int Parameter=3;
    public const int MaxSize = 2 * Parameter - 1;
    public const int MinSize = Parameter - 1;

    public SortedList<int, Record> _data;
    public int _size;
    public DataBaseNode()
    {
        _size = 0;
        _data = new SortedList<int, Record>();
    }

    public virtual bool TryAdd(Record record)
    {
        if (_size == MaxSize)
            return false;
        
        _data.Add(record.Key, record);
        _size++;
        return true;
    }
    public abstract byte[] Serialize();
}

