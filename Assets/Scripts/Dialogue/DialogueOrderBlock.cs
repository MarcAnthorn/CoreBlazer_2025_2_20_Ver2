using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//用于组织同属一个演出id下的所有order；
//读取表格的时候，以DialogueOrderBlock为基本单位读取；即一个演出id对应一个DialogueOrderBlock；
//然后在其下，直接通过orderId下存储对应order的方式，读取该演出id下的所有指令行就行；
//DialogueOrderBlock被DialogueManager中的orderBlockDic组织；也是AVGPanel的持有实例；
public class DialogueOrderBlock
{
    //自己对应的演出id；
    public int rootId;
    //自己所组织的所有order；通过orderId - order实例的方式，以dictionary的方式组织；
    public Dictionary<int, DialogueOrder> orderDic = new Dictionary<int, DialogueOrder>();


}


