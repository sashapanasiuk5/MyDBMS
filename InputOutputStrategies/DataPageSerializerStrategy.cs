namespace DataBase_BTree.InputOutputStrategies;

public class DataPageSerializerStrategy:ISerializerStrategy
{
    private int _binarySize;
    public DataPageSerializerStrategy(int binarySize)
    {
        _binarySize = binarySize;
    }
    public byte[] Serialize(object obj)
    {
        DataPage page = (DataPage)obj;
        byte[] bytes = new byte[_binarySize];
        BitConverter.GetBytes(page.GetSize()).CopyTo(bytes,0);
        int index = sizeof(int);
        foreach (var record in page.GetData())
        {
            foreach (var dataCell in record.GetDataCells())
            {
                BitConverter.GetBytes(dataCell.IsNull).CopyTo(bytes,index);
                index += sizeof(bool);
                if (!dataCell.IsNull)
                {
                    dataCell.Type.SerializeData(dataCell.Data).CopyTo(bytes, index);
                }

                index += dataCell.Type.GetTypeSize();
            }
        }

        return bytes;
    }
}