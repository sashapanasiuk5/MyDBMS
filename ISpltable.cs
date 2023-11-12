namespace DataBase_BTree;

public interface ISplitable<T>
{
    public SplitResults<T> Split();
}