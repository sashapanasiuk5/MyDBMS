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
        int pointer = _writer.WriteNode(_firstDataPage);
        _root = new RootNode(pointer);
        _writer.WriteNode(_root);
    }

    public void Add(Record record)
    {
        Stack<(IndexNode node, int pointer)> pagePath = new Stack<(IndexNode node, int pointer)>();
        pagePath.Push((_root,0));
        (DataBaseNode node, int nodePointer) = IndexSeek(record.Key, _root, pagePath);
        bool isSplit = false;
        do
        {
            (isSplit, int splitKey, int splitNodePointer) = AddToNode(record, node, nodePointer);
            
            if (isSplit)
            {
                (node, nodePointer) = pagePath.Pop();
                record = new Record(splitKey, splitNodePointer);
            }
        } while (isSplit);
    }
    
    private (bool isSplit, int splitKey, int splitNodePointer) AddToNode(Record record,DataBaseNode node, int nodePointer)
    {
        bool needToSplit = node.Add(record);
        if (needToSplit)
        {
            SplitResults<DataBaseNode> splitResults= node.Split();
            if (splitResults.IsRootNode)
            {
                int firstNodePointer = _writer.WriteNode(splitResults.FirstSplitNode);
                int secondNodePointer = _writer.WriteNode(splitResults.SecondSplitNode);
                ((RootNode)node).AddSplitKey(splitResults.SplitKey, firstNodePointer, secondNodePointer);
                _root = ((RootNode)node);
            }
            else
            {
                int splitedNodePointer = _writer.WriteNode(splitResults.SecondSplitNode);
                _writer.WriteNode(node, nodePointer);
                return (true, splitResults.SplitKey, splitedNodePointer);
            }
        }
        _writer.WriteNode(node, nodePointer);
        return (false, 0, 0);
    }
    private (DataPage page, int pointer) IndexSeek(int key, IndexNode node, Stack<(IndexNode node, int pointer)> path)
    {
        int pointer = node.FindPointer(key);
        DataBaseNode nextNode = _reader.ReadNode(pointer);
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
        (DataBaseNode node, int nodePointer) = IndexSeek(key,_root, pagePath);

        
        (IndexNode parent, int parentPointer) = pagePath.Pop();
        bool needToMerge = DeleteFromNode(key, node, nodePointer, parent, parentPointer);
        
        while (needToMerge)
        {
            node = parent;
            nodePointer = parentPointer;
            (parent, parentPointer) = pagePath.Pop();
            needToMerge = DeleteFromNode(key, node, nodePointer, parent, parentPointer);
        }
    }


    private bool DeleteFromNode(int key, DataBaseNode node, int nodePointer, IndexNode nodeParent, int parentPointer)
    {
        bool needToMerge = true;
        if(node is DataPage)
            needToMerge = node.Delete(key);
        if (needToMerge)
        {
            (bool result, DataBaseNode sibling, int siblingPointer, bool isRightSibling) = TryStealFromSibling( node, nodePointer, nodeParent, parentPointer);
            if (result)
            {
                _writer.WriteNode(nodeParent, parentPointer);
                needToMerge = false;
            }
            else
            {
                int splitKey = nodeParent.FindKey(nodePointer);
                needToMerge = nodeParent.Delete(nodePointer);
                sibling.MergeWith(node, splitKey, isRightSibling);
                if (nodeParent is RootNode)
                {
                    _writer.WriteNode(sibling, 0);
                    _root = new RootNode((IndexNode)sibling);
                    needToMerge = false;
                }
                else
                {
                    _writer.WriteNode(sibling, siblingPointer);
                    _writer.WriteNode(nodeParent, parentPointer);
                }
            }
        }

        if (nodeParent.HasKey(key) && node is DataPage)
        {
            nodeParent.ReplaceKey(key, ((DataPage)node).GetLastKey());
            _writer.WriteNode(nodeParent, parentPointer);
        }

        _writer.WriteNode(node, nodePointer);
        return needToMerge;
    }

    private (bool isStolen, DataBaseNode sibling, int pointer,bool isRightSibling) TryStealFromSibling(DataBaseNode node, int nodePointer, IndexNode nodeParent, int parentPointer)
    {
        (int? leftSiblingPointer, int? rightSiblingPointer) = nodeParent.GetPointerSiblings(nodePointer);
        DataBaseNode chosenSibling = null;
        int chosenPointer = 0;
        bool isRightSibling = false;
        bool canSteel = false;
        
        
        if (leftSiblingPointer.HasValue)
        {
            DataBaseNode leftSibling = _reader.ReadNode(leftSiblingPointer.Value);
            chosenSibling = leftSibling;
            chosenPointer = leftSiblingPointer.Value;
            if (leftSibling.CanSplit())
                canSteel = true;
        }
        
        if (rightSiblingPointer.HasValue)
        {
            DataBaseNode rightSibling = _reader.ReadNode(rightSiblingPointer.Value);
            chosenSibling = rightSibling;
            chosenPointer = rightSiblingPointer.Value;
            isRightSibling = true;
            if (rightSibling.CanSplit())
                canSteel = true;
        }

        if (canSteel)
        {
            int SplitKey = 0;
            if (node is IndexNode)
                SplitKey = nodeParent.FindKey(nodePointer);
            int newKey = node.StealFromSibling(chosenSibling, isRightSibling, SplitKey);
            nodeParent.SetNewKey(chosenPointer, nodePointer, newKey);
            _writer.WriteNode(chosenSibling, chosenPointer);
        }

        return (canSteel, chosenSibling, chosenPointer, isRightSibling);
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