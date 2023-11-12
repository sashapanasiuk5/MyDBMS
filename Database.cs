namespace DataBase_BTree;

public class Database
{
    public IndexetedStructure _indexedStructure;
    private Stream _dbFile;

    public void Close()
    {
        _dbFile.Close();
    }

    public Database(string filename)
    {
        _dbFile = File.Open(filename, FileMode.Open);
        _indexedStructure = new IndexetedStructure(_dbFile);
        _indexedStructure.Init();
    }

    private Database(FileStream stream, IndexetedStructure structure)
    {
        _dbFile = stream;
        _indexedStructure = structure;
    }

    public static Database Create(string filename)
    {
        FileStream _dbFile = File.Open(filename, FileMode.Create);
        IndexetedStructure indexedStructure = new IndexetedStructure(_dbFile);
        indexedStructure.Create();
        return new Database(_dbFile, indexedStructure);
    }

    public void AddRecord(int key, int value) => _indexedStructure.Add(new Record(key, value));

    public void Delete(int key) => _indexedStructure.Delete(key);

    public Record Find(int key) => _indexedStructure.Find(key);

    public void Print()
    {
        _indexedStructure.PrintAll();
    }
}