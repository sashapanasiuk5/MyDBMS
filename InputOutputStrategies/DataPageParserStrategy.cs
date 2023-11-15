namespace DataBase_BTree.InputOutputStrategies;

public class DataPageParserStrategy:IParserStrategy
{
    private Dictionary<string, IDataType> _template;
    private int _indexKeyField;

    public DataPageParserStrategy(Dictionary<string, IDataType> template, int indexKeyField)
    {
        _template = template;
    }
    public object Parse(byte[] bytes)
    {
        int size = BitConverter.ToInt32(bytes, 0);
        int index = sizeof(int); 
        SortedList<int, Record> data = new SortedList<int, Record>();
        for (int i = 0; i < size; i++)
        {
            List<DataCell> cells = new List<DataCell>();
            int key = 0;
            int j = 0;
            foreach (var columnTemplate in _template)
            {
                IDataType type = columnTemplate.Value;
                int length = type.GetTypeSize();
                byte[] dataCellBytes = new byte[length];

                bool isNull = BitConverter.ToBoolean(bytes, index);
                object value = null;
                if (!isNull)
                {
                    Array.Copy(bytes, index+sizeof(bool), dataCellBytes, 0, length);
                    value = type.Parse(dataCellBytes);
                    if (j == _indexKeyField)
                    {
                        key = (int)value;
                    }
                }

                DataCell cell = new DataCell(type, value, isNull);
                cells.Add(cell);
                j++;
                index += sizeof(bool) + length;
            }
            Record record = new Record(cells);
            data.Add(key, record);
        }
        return new DataPage(data, size);
    }
}