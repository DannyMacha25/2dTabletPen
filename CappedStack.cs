using System.Collections;
using System.Collections.Generic;

public class CappedStack<T>
{
    private List<T> elements = new List<T>();
    private int cap;

    public CappedStack(int n)
    {
        cap = n;
    }

    /// <summary>
    /// Pushes an element onto the stack, will remove last elements if cap
    /// is reached
    /// </summary>
    /// <param name="item"></param>
    public void Push(T item)
    {
        elements.Insert(0, item);
        
        if(elements.Count > cap)
        {
            elements.RemoveAt(cap);
        }
    }

    /// <summary>
    /// Pops an element from the stack
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        if (elements.Count > 0)
        {
            T item = elements[0];
            elements.RemoveAt(0);
            return item;
        }

        return default(T);
    }

}
