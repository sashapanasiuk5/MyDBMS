namespace DataBase_BTree;

public class BalanceStrategy:IBalanceStrategy
{
    protected TableReader _reader;
    protected TableWriter _writer;
    public BalanceStrategy(TableReader reader, TableWriter writer)
    {
        _writer = writer;
        _reader = reader;
    }
    public virtual (bool needToBalance, Record parentNodeRecord) KeepBalanceAfterAdding(Record record, DataBaseNode node, int nodePointer)
    {
        SplitResults<DataBaseNode> splitResults= node.Split();
        return HandleSplitResults(splitResults, node, nodePointer);
    }

    public virtual bool KeepBalanceAfterDeleting(DataBaseNode node, int nodePointer, IndexNode parentNode, int parentPointer)
    {
        bool needToMerge = false;
        List<(DataBaseNode node, int pointer, bool isRight)> siblings = GetSiblings(nodePointer, parentNode);
        bool result = TryStealFromSibling( node, nodePointer, parentNode, parentPointer, siblings);
        
        if (result)
        {
            _writer.WriteNode(node, nodePointer);
            _writer.WriteNode(parentNode, parentPointer);
        }
        else
        {
            (DataBaseNode sibling, int siblingPointer, bool isRight) = siblings[0];
            needToMerge = MergeNodes(node, nodePointer, sibling, siblingPointer, isRight, parentNode);
            SaveNodes(node, nodePointer, parentNode, parentPointer);
        }

        return needToMerge;
    }
    
    protected virtual bool MergeNodes(DataBaseNode node, int nodePointer ,DataBaseNode sibling, int siblingPointer, bool isRight, IndexNode parentNode)
    {
        int splitKey = parentNode.FindKey(nodePointer, siblingPointer);
        node.MergeWith(sibling, splitKey, isRight);
        bool needToMerge = parentNode.isMinimum();
        parentNode.DeleteKeyByPointers(nodePointer, siblingPointer);

        return needToMerge;
    }


    protected virtual void SaveNodes(DataBaseNode node, int nodePointer, IndexNode parentNode, int parentPointer)
    {
        _writer.WriteNode(node, nodePointer);
        _writer.WriteNode(parentNode, parentPointer);
    }

    protected virtual (bool needToBalance, Record parentNodeRecord) HandleSplitResults(SplitResults<DataBaseNode> splitResults, DataBaseNode node, int nodePointer)
    {
        int splitedNodePointer = _writer.WriteNode(splitResults.SecondSplitNode);
        _writer.WriteNode(node, nodePointer);
        return (true, new Record(splitResults.SplitKey, splitedNodePointer));
    }

    private bool TryStealFromSibling(DataBaseNode node, int nodePointer, IndexNode nodeParent, int parentPointer, List<(DataBaseNode node, int pointer, bool isRight)> siblings)
    {
        bool canSteel = false;
        DataBaseNode chosenSibling = null;
        int siblingPointer = 0;
        bool isRightSibling = false;
        foreach (var sibling in siblings)
        {
            if (!sibling.node.isMinimum())
            {
                (chosenSibling, siblingPointer, isRightSibling) = sibling;
                canSteel = true;
            }
        }
        if (canSteel)
        {
            int SplitKey = 0;
            if (node is IndexNode)
                SplitKey = nodeParent.FindKey(nodePointer, siblingPointer);
            int newKey = node.StealFromSibling(chosenSibling, isRightSibling, SplitKey);
            nodeParent.SetNewKey(siblingPointer, nodePointer, newKey);
            _writer.WriteNode(chosenSibling, siblingPointer);
        }

        return canSteel;
    }
    private List<(DataBaseNode node, int pointer, bool isRight)> GetSiblings(int nodePointer, IndexNode nodeParent)
    {
        (int? leftSiblingPointer, int? rightSiblingPointer) = nodeParent.GetPointerSiblings(nodePointer);
        List<(DataBaseNode node, int pointer, bool isRight)> availableSiblings = new List<(DataBaseNode node, int pointer, bool isRight)>();

        DataBaseNode? leftSibling = null;
        DataBaseNode? rightSibling = null;
        if (leftSiblingPointer.HasValue)
        {
            leftSibling = _reader.ReadNode(leftSiblingPointer.Value);
            availableSiblings.Add((leftSibling, leftSiblingPointer.Value, false));
        }

        if (rightSiblingPointer.HasValue)
        {
            rightSibling = _reader.ReadNode(rightSiblingPointer.Value);
            availableSiblings.Add((rightSibling, rightSiblingPointer.Value, true));
        }

        return availableSiblings;
    }
    
}