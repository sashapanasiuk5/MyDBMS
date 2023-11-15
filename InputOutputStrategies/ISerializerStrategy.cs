namespace DataBase_BTree.InputOutputStrategies;

public interface ISerializerStrategy
{
    public byte[] Serialize(object obj);
}