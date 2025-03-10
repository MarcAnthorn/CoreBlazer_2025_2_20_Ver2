using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapElement
{
    protected int posX;
    protected int posY;
    public int elementId;

    protected MapElement()
    {
        SetPosXY(0, 0);
        SetId(0);
    }

    protected void SetPosXY(int x, int y)
    {
        posX = x;
        posY = y;
    }
    protected int GetPosX()
    {
        return posX;
    }
    protected int GetPosY()
    {
        return posY;
    }

    protected void SetId(int id)     //����ʱ��ʹ�ã���֮��ĳ�ʼ����ͼʱʹ��
    {
        this.elementId = id;
    }
    protected int GetId()
    {
        return elementId;
    }

}

public class Wall : MapElement
{
    public Wall() : base()
    {
        SetId(1);
    }

}
public class LightTower : MapElement
{
    public LightTower() : base()
    {
        SetId(2);
    }

}
public class Ground : MapElement
{
    public Ground() : base()
    {
        SetId(3);
    }

}
