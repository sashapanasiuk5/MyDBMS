namespace DataBase_BTree;


public struct SplitResults<T>
{
    public bool IsRootNode { get; private set; }
    //public bool WasNodeSplit { get; private set; }
    public int SplitKey { get; private set; }
    public T FirstSplitNode { get; private set; }
    public T SecondSplitNode { get; private set; }
    //public int? SplitNodePointer { get; private set; }

    /*public SplitResults(bool isRootNode, bool wasNodeSplit, int splitKey, int splitNodePointer)
    {
        IsRootNode = isRootNode;
        WasNodeSplit = wasNodeSplit;
        SplitKey = splitKey;
        SplitNodePointer = splitNodePointer;
    }*/
    
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
    public IndexNode _root;



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
        DataPage _firstDataPage = new DataPage();
        int pointer = _writer.WriteDataPage(_firstDataPage);
        _root = new RootNode(pointer);
        _writer.WriteIndexNode(_root);
    }

    public void Add(Record record)
    {
        Stack<(IndexNode node, int pointer)> pagePath = new Stack<(IndexNode node, int pointer)>();
        pagePath.Push((_root,0));
        (DataPage targetPage, int pagePointer) = IndexSeek(record.Key, _root, pagePath);
        (bool isSplit, int splitKey, int splitNodePointer) = AddToPage(record, targetPage, pagePointer);
        while (isSplit)
        {
            (IndexNode parentNode, int nodePointer) = pagePath.Pop();
            (isSplit, splitKey, splitNodePointer) = TryAddSplitKey(splitKey, splitNodePointer, parentNode, nodePointer);
        }
    }
    
    private (bool isSplited, int splitedKey, int splitedNodePointer) TryAddSplitKey(int splitKey, int splitedPagePointer,IndexNode node, int nodePointer)
    {
        bool needToSplit = node.Add(splitKey, splitedPagePointer);
        if (needToSplit)
        {
            SplitResults<IndexNode> splitResults= node.Split();
            if (splitResults.IsRootNode)
            {
                int firstNodePointer = _writer.WriteIndexNode(splitResults.FirstSplitNode);
                int secondNodePointer = _writer.WriteIndexNode(splitResults.SecondSplitNode);
                ((RootNode)node).AddSplitKey(splitResults.SplitKey, firstNodePointer, secondNodePointer);
                _root = ((RootNode)node);
            }
            else
            {
                int splitedNodePointer = _writer.WriteIndexNode(splitResults.SecondSplitNode);
                _writer.WriteIndexNode(node, nodePointer);
                return (true, splitResults.SplitKey, splitedNodePointer);
            }
        }
        _writer.WriteIndexNode(node, nodePointer);
        return (false, 0, 0);
    }

    private (bool isSplited, int middle, int splitedPagePointer) AddToPage(Record record, DataPage page, int pagePointer)
    {
        bool needToSplit = page.Add(record);
        if(needToSplit)
        {
            SplitResults<DataPage> splitResults = page.Split();
            int secondPagePointer = _writer.WriteDataPage(splitResults.SecondSplitNode);
            _writer.WriteDataPage(page, pagePointer);
            return (true, splitResults.SplitKey, secondPagePointer);
        }
        _writer.WriteDataPage(page, pagePointer);
        return (false,0,0);
    }
    private (DataPage page, int pointer) IndexSeek(int key, IndexNode node, Stack<(IndexNode node, int pointer)> path)
    {
        int pointer = node.FindPointer(key);
        object nextNode = _reader.ReadNode(pointer);
        if (nextNode is DataPage)
        {
            return ((DataPage)nextNode, pointer);
        }
        path.Push(((IndexNode)nextNode, pointer));
        return IndexSeek(key, (IndexNode)nextNode, path);
    }

    public Record Find(int key)
    {
        DataPage page = IndexSeek(key, _root, new Stack<(IndexNode,int)>()).page;
        Record record = page.Find(key);
        if (record.Key != key)
            throw new Exception("Record doesnt exist");
        return record;
    }
    
    
    
    
    public void Delete(int key)
    {
        Stack<(IndexNode node, int pointer)> pagePath = new Stack<(IndexNode node, int pointer)>();
        pagePath.Push((_root,0));
        (DataPage page, int pagePointer) = IndexSeek(key,_root, pagePath);

        (IndexNode parent, int parentPointer) = pagePath.Pop();
        DeleteFromPage(key, page, pagePointer, parent, parentPointer);
        bool isMerged = false;
        do
        {
            
        } while (isMerged);
    }


    private void DeleteFromPage(int key, DataPage page, int pagePointer, IndexNode pageParent, int parentPointer)
    {
        bool needToMerge = page.Delete(key);
        
        while(needToMerge)
        {
            (bool result, DataPage leftSibling, int siblingPointer) = TrySplitSibling(key, page, pagePointer, pageParent);
            if (result)
            {
                _writer.WriteIndexNode(pageParent, parentPointer);
                needToMerge = false;
            }
            else
            {
                
            }
        }
        _writer.WriteDataPage(page, pagePointer);
        return;
    }

    private (bool result, DataPage sibling, int pointer) TrySplitSibling(int key, DataPage page, int pagePointer, IndexNode pageParent)
    {
        (int? leftSiblingPointer, int? rightSiblingPointer) = pageParent.GetPointerSiblings(pagePointer);
        DataPage chosenSibling = new DataPage();
        int chosenPointer = leftSiblingPointer.Value;
        bool isRightSibling = false;
        if (leftSiblingPointer.HasValue)
        {
            DataPage leftSibling = (DataPage)_reader.ReadNode(leftSiblingPointer.Value);
            if (leftSibling.CanSplit())
            {
                chosenSibling = leftSibling;
                chosenPointer = leftSiblingPointer.Value;
            }
        }

        if (rightSiblingPointer.HasValue)
        {
            DataPage rightSibling = (DataPage)_reader.ReadNode(rightSiblingPointer.Value);
            if (rightSibling.CanSplit())
            {
                chosenSibling = rightSibling;
                chosenPointer = rightSiblingPointer.Value;
                isRightSibling = true;
            }
        }
        int newKey = page.StealFromSibling(chosenSibling, isRightSibling);
        pageParent.SetNewKey(chosenPointer, pagePointer, newKey);
        _writer.WriteDataPage(chosenSibling, chosenPointer);
        return (true, chosenSibling, chosenPointer);
    }
    
    
    
    
    
    
    
    
    public void PrintAll()
    {
        PrintNode(_root);
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