using UnityEngine.UI;

public class PausePanel : BasePanel<PausePanel>
{
    public Button btn_goon;
    protected override void Awake()
    {
        base.Awake();
        HidePanel();
    }
    void Start()
    {
        btn_goon.onClick.AddListener(() =>
        {
            HidePanel();
            GamePanel.Instance.ShowPanel();
            GamePanel.Instance.IsPlaying = true;
        });
    }
}
