using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//向外界派发AVGid的管理类
//负责职责：
//1.判断当前的AVG请求者是谁；
//2.将当前队列中的第一个合法AVGid返回出去用于访问正确的AVG演出表
public class AVGDistributeManager : SingletonBaseManager<AVGDistributeManager>
{
    private AVGDistributeManager(){}
    //所有NPC的事件等待队列:
    //所有事件都是取出不放回的；如果队列为空，那么就会重复触发默认对话；
    //奈亚拉队列，字典中id为0
    LinkedList<int> listNana = new LinkedList<int>();

    //优格队列，字典中id为1
    LinkedList<int> listYuge = new LinkedList<int>();

    //莎布队列，字典中id为2
    LinkedList<int> listShabu = new LinkedList<int>();

    //格赫罗斯队列，字典中id为3
    LinkedList<int> listGhroth = new LinkedList<int>();

    //后续存在的NPC队列；
    LinkedList<int> listDagoon = new LinkedList<int>();


    //存储所有事件队列的字典：
    private Dictionary<E_NPCName, LinkedList<int>> dicAVGDistributor = new Dictionary<E_NPCName, LinkedList<int>>();

    //字典是否初始化的flag：
    private bool isDistributorInited = false;

    //获取当前事件的方法：
    public int FetchAVGId(E_NPCName npcName)
    {
        if (!isDistributorInited)
        {
            isDistributorInited = true;
            Init();
        }

        var currentList = dicAVGDistributor[npcName];
        int currentId = currentList.First.Value;
        //如果当前的事件库只包含了1个事件；那么该事件就是默认的事件；
        //不执行移除；
        if (currentList.Count > 1)
        {
            currentList.RemoveFirst();
        }
        return currentId;
    }

    /// <summary>
    /// 向当前事件添加新的AVG事件的方法：
    /// </summary>
    /// <param name="npcName">当前事件所属的NPC</param>
    /// <param name="id">事件id</param>
    /// <param name="priority">当前事件的优先级，如果是1，那么就从队首插入；默认是1，是优先事件，排列在队首</param>
    public void ContributeAVGId(E_NPCName npcName, int id, int priority = 1)
    {
        //优先确保Init在任何操作之前调用；
        if (!isDistributorInited)
        {
            isDistributorInited = true;
            Init();
        }

        //根据优先级，将事件从头/尾部插入：
        if (priority == 1)
        {
            //头部插入：
            dicAVGDistributor[npcName].AddFirst(id);
        }
        else
        {
            //尾部插入：
            dicAVGDistributor[npcName].AddLast(id);
        }


    }

    //初始化字典的方法：
    private void Init()
    {
        dicAVGDistributor.Add(E_NPCName.奈亚拉, listNana);
        dicAVGDistributor.Add(E_NPCName.优格, listYuge);
        dicAVGDistributor.Add(E_NPCName.莎布, listShabu);
        dicAVGDistributor.Add(E_NPCName.格赫罗斯, listGhroth);
        dicAVGDistributor.Add(E_NPCName.达贡, listDagoon);

        //当前的所有NPC的默认事件：
        
    }

   





}



public enum E_NPCName{
    None = -1,
    奈亚拉 = 0,
    优格 = 1,
    莎布 = 2,  
    格赫罗斯 = 3,
    达贡 = 4,
}