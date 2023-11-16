namespace DataBase_BTree;

public class Table
{
    private IndexetedStructure _indexetedStructure;

    private Dictionary<string, IDataType> _template;

    public Table(Dictionary<string, IDataType> template, IndexetedStructure indexetedStructure)
    {
        _template = template;
        _indexetedStructure = indexetedStructure;
    }

    public void InsertValues(Dictionary<string, object> values)
    {
        List<DataCell> cells = new List<DataCell>();
        foreach (var templateItem in _template)
        {
            DataCell cell;
            if (values.ContainsKey(templateItem.Key))
            {
                cell = new DataCell(templateItem.Value, values[templateItem.Key], false);
            }
            else
            {
                cell = new DataCell(templateItem.Value, null, true);
            }
            cells.Add(cell);
        }

        Record record = new Record(cells);
        _indexetedStructure.Add(record);
    }

    public Dictionary<string, IDataType> GetTemplate() => _template;
    public int GetIndexOfKey() => _indexetedStructure.GetIndexOfKey();
    public int GetIndexStructurePointer() => _indexetedStructure.GetPointer();
    public int GetIndexStructureSize() => _indexetedStructure.GetSize();
    public int GetRootNodePointer() => _indexetedStructure.GetRootPointer();

    public void Print()
    {
        _indexetedStructure.PrintAll();
    }
}