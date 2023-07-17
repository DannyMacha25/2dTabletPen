using System;
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

    public void ChangeCap(int n)
    {
        if (n <= 0)
        {
            return; // Should probably throw an exception lmao :3
        }

        if (n < cap)
        {
            if (elements.Count > n)
            {
                elements.RemoveRange(n, elements.Count - 1);
            }
        }

        cap = n;
    }

}
