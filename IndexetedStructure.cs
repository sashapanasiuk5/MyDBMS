namespace DataBase_BTree;


public struct SplitResults<T>
{
    public bool IsRootNode { get; private set; }
    public int SplitKey { get; private set; }
    public T FirstSplitNode { get; private set; }
    public T SecondSplitNode { get; private set; }

    public SplitResults(bool isRootNode, int splitKey, T firstSplitNode, T secondSplitNode)
    {
        IsRootNode = isRootNode;
        SplitKey = splitKey;
        FirstSplitNode = firstSplitNode;
        SecondSplitNode = secondSplitNode;
    }
}

public class IndexetedStructure
{

    private DataBaseReader _reader;
    private DataBaseWriter _writer;
    public DataBaseNode _root;
    private bool _isRootDataPage;
    private int _rootPointer;
    private IBalanceStrategy _balanceStrategy;



    public IndexetedStructure(Stream stream)
    {
        _writer = new DataBaseWriter(stream, 0, 1048576);
        _reader = new DataBaseReader(stream);
    }

    public void Init()
    {
        _root = (RootNode)_reader.ReadNode(0);
    }

    public void Create()
    {
        _root = new DataPage();
        _rootPointer = _writer.WriteNode(_root);
    }

    public void Add(Record record)
    {
        Stack<(IndexNode node, int pointer)> pagePath = new Stack<(IndexNode node, int pointer)>();
        (DataBaseNode node, int nodePointer) = IndexSeek(record.Key, _root, _rootPointer, pagePath);
        
        bool needToBalance = false;
        do
        {
            node.Add(record);
            needToBalance = NeedToBalance(node);
            if (needToBalance)
            {
                _balanceStrategy = ChooseStrategy(node);
                (needToBalance, record) = _balanceStrategy.KeepBalanceAfterAdding(record, node, nodePointer);
                if (needToBalance)
                {
                    (node, nodePointer) = pagePath.Pop();
                }
            }
            else
            {
                _writer.WriteNode(node, nodePointer);
            }
        } while (needToBalance);
    }
    
    public (DataPage page, int pointer) IndexSeek(int key, DataBaseNode node, int nodePointer, Stack<(IndexNode node, int pointer)> path)
    {
        if (node is DataPage)
        {
            return ((DataPage)node, nodePointer);
        }
        path.Push(((IndexNode)node, nodePointer));
        int pointer = ((IndexNode)node).FindPointer(key);
        DataBaseNode nextNode = _reader.ReadNode(pointer);
        return IndexSeek(key, nextNode, pointer, path);
    }

    private IBalanceStrategy ChooseStrategy(DataBaseNode node)
    {
        if(node.Equals(_root))
            return new BalanceRootNodeStrategy(_reader, _writer, (newRoot) =>
            {
                _root = newRoot;
                if (_rootPointer == 0)
                {
                    _writer.WriteNode(newRoot, 0);
                }
                else
                {
                    _rootPointer = _writer.WriteNode(newRoot);
                }
            });
        
        switch (node)
        {
            case IndexNode indexNode:
                return new BalanceIndexNodeStrategy(_reader, _writer);
            case DataPage page:
                return new BalanceDataPageStrategy(_reader, _writer);
            default:
                throw new Exception("This class is not supported");
        }
    }

    public Record Find(int key)
    {
        Stack<(IndexNode node, int pointer)> pagePath = new Stack<(IndexNode node, int pointer)>();
        DataPage page = IndexSeek(key, _root, _rootPointer,pagePath).page;
        
        Record record = page.Find(key);
        if (record.Key != key)
            throw new Exception("Record doesnt exist");

        foreach (var element in pagePath)
        {
            DataBaseNode node = _reader.ReadNode(element.pointer);
        }
        return record;
    }
    
    
    
    
    public void Delete(int key)
    {
        Stack<(IndexNode node, int pointer)> pagePath = new Stack<(IndexNode node, int pointer)>();
        (DataBaseNode node, int nodePointer) = IndexSeek(key,_root, 0, pagePath);

        ((DataPage)node).Delete(key);
        bool needToBalance = NeedToBalance(node);


        IndexNode parentNode = null;
        int parentPointer = 0;
        
        if(pagePath.Count != 0)
            ReplaceKeyInIndexNode((DataPage)node, key, new Stack<(IndexNode node, int pointer)>(pagePath));
        
        while (needToBalance)
        {
            _balanceStrategy = ChooseStrategy(node);
            if(pagePath.Count != 0)
                (parentNode, parentPointer) = pagePath.Pop();
            needToBalance = _balanceStrategy.KeepBalanceAfterDeleting(node, nodePointer, parentNode, parentPointer);
            if (needToBalance)
            {
                node = parentNode;
                nodePointer = parentPointer;
            }
        }

        if (node is DataPage)
        {
            _writer.WriteNode(node, nodePointer);
        }
    }

    private void ReplaceKeyInIndexNode(DataPage page, int key, Stack<(IndexNode node, int pointer)> pagePath)
    {
        (IndexNode parentNode, int parentPointer) = pagePath.Pop();
        if (parentNode.HasKey(key))
        {
            parentNode.ReplaceKey(key, page.GetLastKey());
            _writer.WriteNode(parentNode, parentPointer);
        }
        else if(pagePath.Count != 0)
        {
            ReplaceKeyInIndexNode(page, key, pagePath);
        }
    }

    private bool NeedToBalance(DataBaseNode node)
    {
        if (node.Equals(_root))
        {
            return (node.GetSize() < 0) || (node.GetSize() > 5);
        }
        else
        {
            return (node.GetSize() < 2) || (node.GetSize() > 5);
        }
    }

    public void PrintAll()
    {
        if (_root is DataPage)
        {
            PrintPage((DataPage)_root);
        }
        else
        {
            PrintNode((RootNode)_root);
        }
        
    }

    public void PrintNode(IndexNode node)
    {
        Console.WriteLine("-----------INDEX NODE---------");
        foreach (var key in node._intermidiateKeys)
        {
            Console.Write(key+" ");
        }
        Console.WriteLine();
        foreach (var pointer in node._childPointers)
        {
            object nextNode = _reader.ReadNode(pointer);
            if (nextNode is DataPage)
            {
                PrintPage((DataPage)nextNode);
            }
            else
            {
                PrintNode((IndexNode)nextNode);
            }
        }
        Console.WriteLine("-----------------------------");
    }
    public void PrintPage(DataPage page)
    {
        Console.WriteLine("-------------DATA PAGE-------------");
        foreach (var pair in page._data)
        {
            Console.WriteLine("Key: "+pair.Key+" Data: "+pair.Value.Value);
        }
        Console.WriteLine("-----------------------------------");
    }
}