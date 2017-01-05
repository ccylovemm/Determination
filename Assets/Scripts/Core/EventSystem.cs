using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//  Created by CaoChunYang 

public class EventSystem
{
    public class EventFunc
    {
        public object obj;
        public string funcname;
        public System.Reflection.MethodInfo method;
    }

    public static Dictionary<string, List<EventFunc>> events = new Dictionary<string, List<EventFunc>>();

    public static void AddEvent(string eventname, object obj, string funcname)
    {
        RemoveEvent(eventname, obj, funcname);

        List<EventFunc> lst = null;
        EventFunc pair = new EventFunc();
        pair.obj = obj;
        pair.funcname = funcname;
        pair.method = obj.GetType().GetMethod(funcname);

        if (events.TryGetValue(eventname, out lst))
        {
            lst.Add(pair);
        }
        else
        {
            lst = new List<EventFunc>();
            lst.Add(pair);
            events.Add(eventname, lst);
        }
    }

    public static void RemoveEvent(string eventname, object obj, string funcname)
    {
        List<EventFunc> lst = null;
        if (events.TryGetValue(eventname, out lst))
        {
            for (int i = 0; i < lst.Count; ++i)
            {
                if (obj == lst[i].obj && lst[i].funcname == funcname)
                {
                    lst.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public static void DispatchEvent(string eventname, params object[] args)
    {
        List<EventFunc> list = null;
        if (events.TryGetValue(eventname, out list))
        {
            for (int i = 0; i < list.Count; )
            {
                EventFunc func = list[i];
                list[i].method.Invoke(list[i].obj, args);
                if (i >= list.Count) break;
                if (list[i] != func) continue;
                ++i;
            }
        }
    }
}