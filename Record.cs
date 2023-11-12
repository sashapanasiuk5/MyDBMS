namespace DataBase_BTree;

public class Record
{
    public int Key { get; private set; }
    public int Value { get; private set; }

    public Record(int key, int value)
    {
        Key = key;
        Value = value;
    }
}