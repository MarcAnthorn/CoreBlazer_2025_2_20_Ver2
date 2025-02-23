using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//这是GodItemPanel和CommonItemPanel的基类，用于存储两者需要实现的抽象函数和共同成员；
public abstract class ItemPanel : BasePanel
{
    
    protected abstract void RefreshItem();

    
}
