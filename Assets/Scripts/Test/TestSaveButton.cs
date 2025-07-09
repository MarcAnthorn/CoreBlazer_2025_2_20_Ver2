using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSaveButton : MonoBehaviour
{
    public Button btnSave;
    public Button btnLoad;
    public Button btnClear;
    void Awake()
    {
      
    }

    void Start()
    {
        TextManager.SetContentFont(this.gameObject);
        btnSave.onClick.AddListener(()=>{
            SaveManager.Instance.SaveGame();
            Debug.LogWarning("Game Saved!");
        });

        btnLoad.onClick.AddListener(()=>{
            SaveManager.Instance.LoadGame();
            Debug.LogWarning("Game Loaded!");
        });

        btnClear.onClick.AddListener(()=>{
            SaveManager.Instance.ClearGame();
            Debug.LogWarning("Game Cleared!");
        });
    }
}
