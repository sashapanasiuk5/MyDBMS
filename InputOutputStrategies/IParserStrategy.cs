namespace DataBase_BTree.InputOutputStrategies;

public interface IParserStrategy
{
    public object Parse(Stream stream);
}