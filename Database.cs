using DataBase_BTree.InputOutputStrategies;
using IndexedStructure;

namespace DataBase_BTree;

public class Database
{
    private const int SizeOfInfoArea = 1024;
    private Dictionary<string,Table> _tables;
    private Stream _dbFile;
    private int parameter = 3;

    private DataBaseInfoReader _infoReader;
    private DataBaseInfoWriter _infoWriter;
    //private Dictionary<int, IDataType> _availableTypes;

    public void Close()
    {
        _dbFile.Close();
    }

    public Database(string filename)
    {
        _dbFile = File.Open(filename, FileMode.Open);
        _infoReader = new DataBaseInfoReader(SizeOfInfoArea);
        _infoWriter = new DataBaseInfoWriter();
        _tables = new Dictionary<string, Table>();
        InitTables(_infoReader.Read(_dbFile));
        /*_availableTypes = new Dictionary<int, IDataType>();
        _availableTypes.Add(0,new IntegerType());
        _availableTypes.Add(1,new CharType());*/
        /*_indexedStructure = new IndexetedStructure(_dbFile);
        _indexedStructure.Init();*/
    }

    private Database(FileStream stream)
    {
        _dbFile = stream;
        _tables = new Dictionary<string, Table>();
        _infoReader = new DataBaseInfoReader(SizeOfInfoArea);
        _infoWriter = new DataBaseInfoWriter();
        
        InitTables(_infoReader.Read(_dbFile));
    }

    public static Database Create(string filename)
    {
        FileStream _dbFile = File.Open(filename, FileMode.Create);
        /*IndexetedStructure indexedStructure = new IndexetedStructure(_dbFile);
        indexedStructure.Create();*/
        return new Database(_dbFile);
    }

    public void CreateTable(string name, Dictionary<string, IDataType> template, int indexOfKey)
    {
        IndexetedStructure index = null;
        if (_tables.Count == 0)
        {
            index = new IndexetedStructure(_dbFile, template, indexOfKey, SizeOfInfoArea, 9046);
            index.Create();
            index.OnRootRelocated += (sender, args) => _infoWriter.Write(_dbFile, GetInfo());
        }

        Table table = new Table(template, index);
        _tables.Add(name, table);
        _infoWriter.Write(_dbFile,GetInfo());
    }

    private List<DataBaseInfo> GetInfo()
    {
        List<DataBaseInfo> infos = new List<DataBaseInfo>();
        foreach (var table in _tables)
        {
            string tableName = table.Key;
            Dictionary<string, IDataType> template = table.Value.GetTemplate();
            int indexOfKey = table.Value.GetIndexOfKey();
            int indexStructurePointer = table.Value.GetIndexStructurePointer();
            int indexStructureSize = table.Value.GetIndexStructureSize();
            int rootNodePointer = table.Value.GetRootNodePointer();
            infos.Add(new DataBaseInfo(tableName, template, indexOfKey, indexStructurePointer, indexStructureSize, rootNodePointer));
        }

        return infos;
    }

    private void InitTables(List<DataBaseInfo> infos)
    {
        foreach (var info in infos)
        {
            IndexetedStructure index = new IndexetedStructure(_dbFile, info.Template, info.IndexOfKey,
                info.IndexStructurePointer, info.IndexStructureMaxSize, info.RootNodePointer);
            index.Init();
            index.OnRootRelocated += (sender, args) => _infoWriter.Write(_dbFile, GetInfo());
            Table table = new Table(info.Template, index);
            _tables.Add(info.TableName, table);
        }
    }

    public void InsertIntoTable(string name,Dictionary<string, object> values)
    {
        _tables[name].InsertValues(values);
    }
/*
    public void AddRecord(int key, int value) => _indexedStructure.Add(new Record(key, value));

    public void Delete(int key) => _indexedStructure.Delete(key);

    public Record Find(int key) => _indexedStructure.Find(key);
*/
    public void PrintTable(string name)
    {
        _tables[name].Print();
    }
}