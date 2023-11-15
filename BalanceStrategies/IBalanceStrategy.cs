namespace DataBase_BTree;

public interface IBalanceStrategy
{
    public (bool needToBalance, Record parentNodeRecord) KeepBalanceAfterAdding(Record record, DataBaseNode node, int nodePointer);

    public bool KeepBalanceAfterDeleting(DataBaseNode node, int nodePointer, IndexNode parentNode, int parentPointer);
}