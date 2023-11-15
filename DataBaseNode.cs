using System.Runtime.Serialization;

namespace DataBase_BTree;

public abstract class DataBaseNode
{
    public const int Parameter=3;
    public const int MaxSize = 2 * Parameter - 1;
    public const int MinSize = Parameter - 1;

    protected int _size;
    public virtual bool isMinimum()
    {
        return _size <= MinSize;
    }
    public int GetSize()
    {
        return _size;
    }
    public bool isMaximum()
    {
        return _size == MaxSize;
    }

    public abstract bool Add(Record record);

    public abstract int StealFromSibling(DataBaseNode siblingNode, bool isRightSibling, int SplitKey);

    public abstract void MergeWith(DataBaseNode siblingNode, int key, bool isRightSibling);
    public abstract SplitResults<DataBaseNode> Split();
    
}

