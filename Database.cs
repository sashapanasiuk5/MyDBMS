namespace DataBase_BTree;

public class Database
{
    private List<Table> _tables;
    private Stream _dbFile;
    private int parameter = 3;

    public void Close()
    {
        _dbFile.Close();
    }

    public Database(string filename)
    {
        _dbFile = File.Open(filename, FileMode.Open);
        
        /*_indexedStructure = new IndexetedStructure(_dbFile);
        _indexedStructure.Init();*/
    }

    private Database(FileStream stream)
    {
        _dbFile = stream;
        _tables = new List<Table>();
    }

    public static Database Create(string filename)
    {
        FileStream _dbFile = File.Open(filename, FileMode.Create);
        /*IndexetedStructure indexedStructure = new IndexetedStructure(_dbFile);
        indexedStructure.Create();*/
        return new Database(_dbFile);
    }

    public void CreateTable(Dictionary<string, IDataType> template, int indexOfKey)
    {
        
        IndexetedStructure index = new IndexetedStructure(_dbFile, template, indexOfKey);
        index.Create();
        Table table = new Table(template, index);
        _tables.Add(table);
    }

    public void InsertIntoTable(Dictionary<string, object> values)
    {
        _tables[0].InsertValues(values);
    }
/*
    public void AddRecord(int key, int value) => _indexedStructure.Add(new Record(key, value));

    public void Delete(int key) => _indexedStructure.Delete(key);

    public Record Find(int key) => _indexedStructure.Find(key);
*/
    public void PrintTable()
    {
        _tables[0].Print();
    }
}