using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel<StartPanel>
{
    public Button btn_Start;

    
    void Start()
    {
        
        btn_Start.onClick.AddListener(() =>
        {
            HidePanel();
            GamePanel.Instance.ShowPanel();
            GamePanel.Instance.IsPlaying = true;
        });
    }
}
