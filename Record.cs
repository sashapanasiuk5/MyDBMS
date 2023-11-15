namespace DataBase_BTree;

public struct DataCell
{
    public IDataType Type { get; private set; }
    public object Data{ get; private set; }
    public bool IsNull{ get; private set; }

    public DataCell(IDataType type, object data, bool isNull)
    {
        Type = type;
        Data = data;
        IsNull = isNull;
    }
}

public class Record
{
    private List<DataCell> _cells;
    private int _size;

    public Record(List<DataCell> cells)
    {
        _cells = cells;
    }

    public Record(int key, int value)
    {
        _cells = new List<DataCell>();
        _cells.Add(new DataCell(new IntegerType(), key, false));
        _cells.Add(new DataCell(new IntegerType(), value, false));
    }

    public List<DataCell> GetDataCells() => _cells;

    public object GetValueAt(int index)
    {
        return _cells[index].Data;
    }

    public int GetBinarySize()
    {
        return _size;
    }
}