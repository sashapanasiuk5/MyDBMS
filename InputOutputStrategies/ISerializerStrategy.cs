namespace DataBase_BTree.InputOutputStrategies;

public interface ISerializerStrategy
{
    public void Serialize(object obj, Stream stream);
}