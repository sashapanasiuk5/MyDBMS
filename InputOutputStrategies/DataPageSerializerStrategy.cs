namespace DataBase_BTree.InputOutputStrategies;

public class DataPageSerializerStrategy:ISerializerStrategy
{
    public void Serialize(object obj, Stream stream)
    {
        DataPage page = (DataPage)obj;
        stream.Write(BitConverter.GetBytes(page.GetSize()));
        
        foreach (var record in page.GetData())
        {
            foreach (var dataCell in record.GetDataCells())
            {
                stream.Write(BitConverter.GetBytes(dataCell.IsNull));
                if (!dataCell.IsNull)
                {
                    stream.Write(dataCell.Type.SerializeData(dataCell.Data));
                }
            }
        }
    }
}