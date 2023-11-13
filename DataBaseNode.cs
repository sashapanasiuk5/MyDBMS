using System.Runtime.Serialization;

namespace DataBase_BTree;

public abstract class DataBaseNode:IBinarySerializable
{
    public const int Parameter=3;
    public const int MaxSize = 2 * Parameter - 1;
    public const int MinSize = Parameter - 1;

    public abstract bool Add(Record record);
    public abstract bool Delete(int key);

    public abstract bool CanSplit();

    public abstract int StealFromSibling(DataBaseNode siblingNode, bool isRightSibling, int SplitKey);

    public abstract void MergeWith(DataBaseNode siblingNode, int key, bool isRightSibling);
    public abstract SplitResults<DataBaseNode> Split();
    
    public abstract byte[] Serialize();
}

