using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory              //背包
{
    public List<Prop> props = new List<Prop>();
    public Dictionary<Prop, int> CountOfProps = new Dictionary<Prop, int>();//供外部访问的数据结构

    public void AddProp(Prop prop)
    {
        if (CountOfProps.ContainsKey(prop))     //加这个是为了防止下面的CountOfProps[prop]空引用
        {
            if (CountOfProps[prop] < prop.maxLimit)
            {
                AddInDic(prop);
                props.Add(prop);
                return;
            }
        }
        else
        {
            //props.Add(prop);
            AddInDic(prop);
        }

        Debug.LogWarning($"道具：{prop.name}已满！");
    }
    public void RemoveProp(Prop prop)
    {
        if (props.Contains(prop))           //按照List内的prop来判断
        {
            props.Remove(prop);
            RemoveInDic(prop);
            return;
        }

        Debug.LogError($"背包中没有该道具：{prop.name}");
    }

    private void AddInDic(Prop prop)
    {
        if (!CountOfProps.ContainsKey(prop))
        {
            CountOfProps.Add(prop, 0);
        }
        CountOfProps[prop]++;
    }
    private void RemoveInDic(Prop prop)
    {
        if (!CountOfProps.ContainsKey(prop))
        {
            CountOfProps.Add(prop, 0);
            Debug.LogError($"背包中没有该道具：{prop.name}");
            return;
        }
        CountOfProps[prop]--;
    }

}
