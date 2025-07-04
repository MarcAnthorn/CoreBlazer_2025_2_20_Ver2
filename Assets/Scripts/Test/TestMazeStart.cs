using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//迷宫的初始脚本，处理迷宫的初始化逻辑
public class TestMazeStart : MonoBehaviour
{
    GameObject player;
    public Vector3 originalPoint;
    public Image blackMask;

    void Awake()
    {
        //更新事件库：
        EventManager.Instance.ResetAllLibIds();
    }

    void Start()
    {
        player = Instantiate(Resources.Load<GameObject>("Player"));


        originalPoint = Vector3.zero;

        PlayerController playerScript = player.GetComponent<PlayerController>();

        EventHub.Instance.EventTrigger<bool>("Freeze", true);
        
        //尝试调用：Item_616的buff
        EventHub.Instance.EventTrigger("RandomBenifit");

        //播放进入迷宫的音效：
        SoundEffectManager.Instance.PlaySoundEffect("进入离开迷宫时的音效");
        
        //按照当前的GameLevelManager中的标识进行地图的加载：
        switch (GameLevelManager.Instance.gameLevelType)
        {
            case E_GameLevelType.First:
                ResourcesManager.Instance.LoadAsync<GameObject>("MapPrefabsFormer/MapFirstFloor", (_gameObject) =>
                {
                    Instantiate(_gameObject, originalPoint, Quaternion.identity);
                    playerScript.isDamageLocked = false;
                    playerScript.LMax = 100;

                    //播放特定的BGM：
                    EventHub.Instance.EventTrigger<UnityAction>("HideMask", () =>
                    {
                        SoundEffectManager.Instance.PlayMusic("第一关BGM");
                        EventHub.Instance.EventTrigger<bool>("Freeze", false);
                    });

                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 1114);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 1115);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 1116);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.格赫罗斯, 1112);
                


                });




                break;
            case E_GameLevelType.Second:
                ResourcesManager.Instance.LoadAsync<GameObject>("MapPrefabsFormer/MapSecondFloor", (_gameObject) =>
                {
                    Instantiate(_gameObject, originalPoint, Quaternion.identity);
                    playerScript.isDamageLocked = false;
                    playerScript.LMax = 100;

                    EventHub.Instance.EventTrigger<UnityAction>("HideMask", () =>
                    {
                        //播放特定的BGM：
                        SoundEffectManager.Instance.PlayMusic("第二关BGM");
                        EventHub.Instance.EventTrigger<bool>("Freeze", false);
                    });

                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 1119);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 1120);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 1121);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.格赫罗斯, 1117);
        



                });

                break;
            case E_GameLevelType.Third:

                ResourcesManager.Instance.LoadAsync<GameObject>("MapPrefabsFormer/MapThirdFloor", (_gameObject) =>
                {
                    Instantiate(_gameObject, originalPoint, Quaternion.identity);
                    playerScript.isDamageLocked = false;
                    playerScript.LMax = 100;

                    EventHub.Instance.EventTrigger<UnityAction>("HideMask", () =>
                    {
                        //播放特定的BGM：
                        SoundEffectManager.Instance.PlayMusic("第三关BGM");
                        EventHub.Instance.EventTrigger<bool>("Freeze", false);
                    });

                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.格赫罗斯, 1122);

                });

                break;

            //新手关卡：灯光初始值60；锁定血量不会死亡：
            case E_GameLevelType.Tutorial:
                ResourcesManager.Instance.LoadAsync<GameObject>("MapPrefabsFormer/MapTutorialFloor", (_gameObject) =>
                {
                    Instantiate(_gameObject, originalPoint, Quaternion.identity);
                    playerScript.isDamageLocked = true;
                    playerScript.LMax = 60;


                    EventHub.Instance.EventTrigger<UnityAction>("HideMask", () =>
                    {
                        //播放特定的BGM：
                        SoundEffectManager.Instance.PlayMusic("第二关BGM");
                        EventHub.Instance.EventTrigger<bool>("Freeze", false);
                    });

                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 1109);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 1110);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 1111);

                    //贡献默认对话:
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 2501, 0);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 2502, 0);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 2503, 0);


                });

                break;
        }
        player.transform.position = originalPoint + new Vector3(0.41f, -0.91f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
