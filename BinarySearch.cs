namespace DataBase_BTree;

public class BinarySearch<T>
{
    public delegate int GetKey(T element);
    
    public T Find(int key, List<T> array, GetKey getKey)
    {
        int first = 0;
        int last = array.Count-1;
        int middle = 0;
        while (first <= last)
        {
            middle = (int)Math.Floor((double)((first+last)/2));
            if (getKey(array[middle]) > key)
            {
                last = middle - 1;
            }else if (getKey(array[middle]) < key)
            {
                first = middle + 1;
            }
            else
            {
                return array[middle];
            }
        }

        return array[middle];
    }
    public int FindPlace(int key, List<T> array, GetKey getKey)
    {
        int first = 0;
        int last = array.Count-1;
        int middle = 0;
        while (first <= last && middle < array.Count && middle >= 0)
        {
            middle = (int)Math.Floor((double)((first+last)/2));
            int middleSecond = middle;
            int middleFirst;
            if (middleSecond != 0)
            {
                middleFirst = middleSecond - 1;
            }
            else
            {
                middleFirst = middleSecond;
            }
            
            if (getKey(array[middleFirst]) >= key)
            {
                last = middle - 1;
            }else if (getKey(array[middleSecond]) < key)
            {
                first = middle + 1;
            }
            else
            {
                return middle;
            }
        }
        
        return first;
    }
}