using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel :BasePanel<EndPanel>
{
    public Button btn_BackToMenu;
    protected override void Awake()
    {
        base.Awake();
        HidePanel();
    }
    void Start()
    {
        btn_BackToMenu.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LianLianKan");
        });
    }
}
