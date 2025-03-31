using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AVGPanel : BasePanel
{
    //当前演出进行到的指令：
    public DialogueOrder currentOrder;
    public Transform defaultPos;
    public Transform leftPos;
    public Transform midPos;
    public Transform rightPos;
    //最新从表中读取出来的位置（可能是出现的位置，可能是目标移动的位置）
    private Transform currentTargetPos;
    public Image imgBackground;


    public string currentBackgroundName;
    public string bgmName;
    //当前对话场景中包含的所有NPC（以NPC名字的形式存储）
    public Dictionary<string, GameObject> currentNPCDic = new Dictionary<string, GameObject>();

    // public List<DialogueOption> optionList = new List<DialogueOption>();

    protected override void Init()
    {

    }

    //接收单个指令，并且处理指令
    private void ProcessOrder(DialogueOrder order, E_OrderType type)
    {
        //当前指令是：普通指令 / 选项后对话指令
        if(type == E_OrderType.Common)
        {
            //处理背景：
            currentBackgroundName = order.backgroundName;
            if(currentBackgroundName != null)
            {
                imgBackground.sprite = Resources.Load<Sprite>(currentBackgroundName);
            }

            //处理出现的NPC
            //这个需要分情况，
            //如果是NPC已经在场景中存在，那么就是移动位置
            //如果不存在，才是浮现的效果出现在对应的位置；
            string npcName = order.showUpNPCName.ToString();

            //处理当前的位置：
            switch(order.positionId)
            {
                case 0:
                    currentTargetPos = defaultPos;
                break;
                case 1:
                    currentTargetPos = leftPos;
                break;
                case 2:
                    currentTargetPos = midPos;
                break;
                case 3:
                    currentTargetPos = rightPos;
                break;
                default:
                    Debug.LogWarning("当前获取的位置id不存在，错误位置：AVGPanel");
                break;
            }

            //处理NPC的出现（或者是位置重置）
            if(currentNPCDic.ContainsKey(npcName))
            {
                //如果包含，那么就是重置位置:
                MoveNPC(npcName, currentTargetPos);
            }
            else
            {
                //不包含，就是加入List，同时让NPC出现在对应的位置
                InitNPC(npcName, currentTargetPos);
            }


            //处理消失的逻辑：
            //直接复用npcName这个变量：
            npcName = order.disappearNPCName.ToString();
            if(npcName != null)
            {
                EraseNPC(npcName);
            }

            //处理对话者逻辑：
            npcName = order.conversationNPCName.ToString();
            if(npcName != null)
            {
                ConverseWithNPC(npcName);
            }
        }

        //当前指令是：选项类型
        else if(type == E_OrderType.Option)
        {
            //如果是选项类型，那么外部就会一直执行指令；直到中断指令出现
        }
        
        //当前类型是：中断指令
        else
        {
            //中断指令的出现，说明之前处理的是选项；因此需要处理选项；
            //值得注意的是，Common指令之后一定不会出现中断指令；
        }



    }



    //移动NPC的方法：传入的参数是目标移动位置：
    private void MoveNPC(string name, Transform targetPos)
    {
        float moveDuration = 1f;
        LeanTween.move(currentNPCDic[name], targetPos, moveDuration)
            .setEase(LeanTweenType.easeInOutQuad);
    }

    //初始化NPC的方法：出现并且将其放置在对应的位置：
    private void InitNPC(string name, Transform targetPos)
    {
        GameObject npc = Instantiate(Resources.Load<GameObject>("NPC/" + name), targetPos, false);
        currentNPCDic.Add(name, npc);

        //处理其渐显的逻辑：
        Image npcImage = npc.gameObject.GetComponent<Image>();
        Color color = npcImage.color;
        color.a = 0;
        npcImage.color = color;

        float moveDuration = 0.4f;
        LeanTween.alpha(npcImage.rectTransform, 1, moveDuration);

    }

    //处理NPC消失的方法：
    private void EraseNPC(string name)
    {
        if(!currentNPCDic.ContainsKey(name))
        {
            Debug.LogWarning($"当前要移除的NPC不存在，NPC名：{name}");
            return;
        }

        //从字典中移除：
        GameObject nowRemoveNPC = currentNPCDic[name];
        currentNPCDic.Remove(name);

        Image npcImage = nowRemoveNPC.gameObject.GetComponent<Image>();
        float moveDuration = 0.4f;
        LeanTween.alpha(npcImage.rectTransform, 0, moveDuration)
            .setOnComplete(()=>{
                Destroy(nowRemoveNPC);
            });     
    }

    //处理NPC对话逻辑：
    private void ConverseWithNPC(string name)
    {
        foreach(var key in currentNPCDic.Keys)
        {
            if(key != name)
            {
                //如果不是当前对话的NPC，则调暗Image；
                DarkenNPCImage(key);
            }
        }

        //然后在对话框中显示对话内容
    }


    //处理NPC的Image暗化的逻辑：参数是NPC名字
    //用于在对话的时候，将所有不是当前对话的Image调暗；
    private void DarkenNPCImage(string name)
    {
        Image targetImage = currentNPCDic[name].GetComponent<Image>();
        float factor = 0.4f;
        Color currentColor = targetImage.color;
        currentColor.r *= factor;       // 调整红色通道
        currentColor.g *= factor;       // 调整绿色通道
        currentColor.b *= factor;       // 调整蓝色通道

        targetImage.color = currentColor;  // 应用新的颜色
    }


}


public enum E_NPCName{
    奈亚拉 = 0,
    优格 = 1,
    纱布 = 2,  
}

public enum E_OrderType{
    Common = 0,
    Option = 1,
    Break = 2,

}

public class DialogueOrder
{
    //当前指令所属的演出id；
    public int rootId;
    //当前指令的id
    public int orderId;
    //位置的flag；
    public int positionId;
    //人物效果的flag：1是抖动，2是镜像；
    public int effectId;
    //下一个指令的索引id；
    public int nextOrderId;
    
    //当前要展示的NPC名称（枚举类）
    public E_NPCName showUpNPCName;
    //当前要消失的NPC名称
    public E_NPCName disappearNPCName;
    //当前对话的NPC名称
    public E_NPCName conversationNPCName;

    //当前需要展示的背景名（也是背景资源相对路径）
    public string backgroundName;
    //开始播放的Audioclip资源名；
    public string audioClipStartName;
    //结束的音效资源名
    public string audioClipEndName;

    //包含了选项文本和对话文本两个东西的string：
    public string orderText;





}