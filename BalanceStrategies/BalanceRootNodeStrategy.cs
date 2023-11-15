namespace DataBase_BTree;

public class BalanceRootNodeStrategy:BalanceStrategy
{
    private SetNewRoot _set;
    public delegate void SetNewRoot(DataBaseNode root);
    public BalanceRootNodeStrategy(TableReader reader, TableWriter writer, SetNewRoot setRoot) : base(reader, writer)
    {
        _set = setRoot;
    }

    public override bool KeepBalanceAfterDeleting(DataBaseNode node, int nodePointer, IndexNode parentNode, int parentPointer)
    {
        
        int pointer = ((RootNode)node).GetFirstChildPointer();
        DataBaseNode child = _reader.ReadNode(pointer);
        if (child is IndexNode)
        {
            RootNode newRoot = new RootNode((IndexNode)_reader.ReadNode(pointer));
            _set(newRoot);
        }
        else
        {
            _set(child);
        }

        return false;
    }

    protected override (bool needToBalance, Record parentNodeRecord) HandleSplitResults(SplitResults<DataBaseNode> splitResults, DataBaseNode node, int nodePointer)
    {
        int firstNodePointer = _writer.WriteNode(splitResults.FirstSplitNode);
        int secondNodePointer = _writer.WriteNode(splitResults.SecondSplitNode);
        RootNode newRoot = new RootNode(firstNodePointer, secondNodePointer, splitResults.SplitKey);
        _set(newRoot);
        return (false, null);
    }
}