using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{

    public static bool GetComponentsAtCursorLocation<T>(out List<T> listComponenetAtCursorLocation, Vector3 positionToCheck)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2Ds = Physics2D.OverlapPointAll(positionToCheck);

        T tComponent = default(T);

        for (int i = 0; i < collider2Ds.Length; i ++)
        {
            tComponent = collider2Ds[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;

                componentList.Add(tComponent);
            } else
            {
                tComponent = collider2Ds[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;

                    componentList.Add(tComponent);
                }
            }
        }

        listComponenetAtCursorLocation = componentList;

        return found;
    }
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxLocation, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;

        List<T> componentList = new List<T>();

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(point, size, angle);

        for (int i =0; i < collider2Ds.Length; i ++)
        {
            T tComponent = collider2Ds[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            } else
            {
                tComponent = collider2Ds[i].gameObject.GetComponentInChildren<T>();

                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxLocation = componentList;
        return found;
    }

    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfChildrensToTest, Vector2 point, Vector2 size, float angle)
    {
        Collider2D[] collider2Ds = new Collider2D[numberOfChildrensToTest];

        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2Ds);

        T tComponent = default(T);
        T[] componentArray = new T[collider2Ds.Length - 1];

        for (int i = collider2Ds.Length - 1; i >= 0; i --)
        {
            if (collider2Ds[i] != null)
            {
                tComponent = collider2Ds[i].gameObject.GetComponent<T>();
                if (tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
            
        }

        return componentArray;
    }

}