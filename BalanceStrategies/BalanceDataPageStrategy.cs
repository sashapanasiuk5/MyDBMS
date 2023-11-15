namespace DataBase_BTree;

public class BalanceDataPageStrategy:BalanceStrategy
{
    public BalanceDataPageStrategy(DataBaseReader reader, DataBaseWriter writer) : base(reader, writer)
    {
    }

    protected override bool MergeNodes(DataBaseNode node, int nodePointer, DataBaseNode sibling, int siblingPointer, bool isRight, IndexNode parentNode)
    {
        int splitKey = parentNode.FindKey(nodePointer, siblingPointer);
        node.MergeWith(sibling, splitKey, isRight);
        bool needToMerge = parentNode.isMinimum();
        parentNode.DeleteKeyByPointers(nodePointer, siblingPointer);
        if (isRight)
        {
            int newKey = ((DataPage)node).GetLastKey();
            parentNode.ReplaceKey(splitKey, newKey);
        }

        return needToMerge;
    }
}