namespace DataBase_BTree.InputOutputStrategies;

public class DataPageParserStrategy:IParserStrategy
{
    private Dictionary<string, IDataType> _template;
    private int _indexKeyField;

    public DataPageParserStrategy(Dictionary<string, IDataType> template, int indexKeyField)
    {
        _template = template;
        _indexKeyField = indexKeyField;
    }
    public object Parse(Stream stream)
    {
        byte[] intBuffer = new byte[sizeof(int)];
        byte[] boolBuffer = new byte[sizeof(bool)];
        stream.Read(intBuffer);
        
        int size = BitConverter.ToInt32(intBuffer);

        SortedList<int, Record> data = new SortedList<int, Record>();
        for (int i = 0; i < size; i++)
        {
            List<DataCell> cells = new List<DataCell>();
            int key = 0;
            int j = 0;
            foreach (var columnTemplate in _template)
            {
                IDataType type = columnTemplate.Value;
                stream.Read(boolBuffer);
                bool isNull = BitConverter.ToBoolean(boolBuffer);
                object value = null;
                if (!isNull)
                {
                    value = type.Parse(stream);
                    if (j == _indexKeyField)
                    {
                        key = (int)value;
                    }
                }

                DataCell cell = new DataCell(type, value, isNull);
                cells.Add(cell);
                j++;
            }
            Record record = new Record(cells);
            data.Add(key, record);
        }
        return new DataPage(data, size, _indexKeyField);
    }
}