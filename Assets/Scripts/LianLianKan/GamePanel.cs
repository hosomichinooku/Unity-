using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel<GamePanel>
{
    private GameObject flowerPref;
    private GameObject[,] tiles;
    public Image img_hp;
    public Button btn_fresh;
    public Button btn_pause;
    public TextMeshProUGUI txt_level;
    public TextMeshProUGUI txt_freshNum;
    private float hpWidth;
    public List<Sprite> flowerSprites;
    public GameObject choicedEffect;
    private GameObject nowChoiced;
    public bool IsPlaying = false;
    private int freshNum = 3;
    private float totalTime = 240;
    private float remainedTime;
    private int remainedFlower;
    public LineRenderer line;
    private int gameMode = 1;//1正常，2整体向上，3整体向下，4整体向左，5整体向右，6上下，7左右，8居中
    protected override void Awake()
    {
        remainedTime = totalTime;
        base.Awake();
        HidePanel();
        tiles = new GameObject[10,14];
        remainedFlower = 140;
        flowerPref = Resources.Load<GameObject>("GameObjects/LianLianKan/flower");
    }
    void Start()
    {
        GenerateTiles();
        choicedEffect.SetActive(false);
        hpWidth = img_hp.rectTransform.rect.width;
        txt_freshNum.text = "点击刷新:" + freshNum.ToString();
        btn_fresh.onClick.AddListener(() =>
        {
            if(freshNum >0)
            {
                MessUp();
                freshNum--;
                txt_freshNum.text = "点击刷新:" + freshNum.ToString();
            }
        });
        btn_pause.onClick.AddListener(() =>
        {
            HidePanel();
            IsPlaying = false;
            PausePanel.Instance.ShowPanel();
        });
    }
    private void GenerateTiles()
    {
        for(int i = 0;i<tiles.GetLength(1);i=i+2)
        {
            for(int j = 0;j<tiles.GetLength(0);j++)
            {
                int n = UnityEngine.Random.Range(0,flowerSprites.Count);
                GameObject fl = Instantiate(flowerPref);
                SetFlowerPosition(fl.transform,j,i);
                fl.GetComponent<SpriteRenderer>().sprite = flowerSprites[n];
                fl = Instantiate(flowerPref);
                SetFlowerPosition(fl.transform,j,i+1);
                fl.GetComponent<SpriteRenderer>().sprite = flowerSprites[n];
            }
        }
        MessUp(1000);
    }
    private void SetFlowerPosition(Transform ts,int x,int y)
    {
        ts.position = new Vector2(-6.5f+y,4f-x);
        tiles[x,y] = ts.gameObject;
    }
    private (int,int) GetFlowerPosition(Transform ts) =>((int)(4f-ts.position.y),(int)(ts.position.x+6.5f));
    private void MoveFlower2(Transform ts,int i2,int j2)
    {
        (int i,int j) = GetFlowerPosition(ts);
        if((i,j)==(i2,j2)) return;
        if(!tiles[i2,j2])
        {
            SetFlowerPosition(ts,i2,j2);
            if(tiles[i,j]) tiles[i,j] = null;
        }
        else
        {
            GameObject g = tiles[i2,j2];
            SetFlowerPosition(g.transform,i,j);
            SetFlowerPosition(ts,i2,j2);
        }

    }
    public void MessUp(int n = 100)
    {
        for(int i =0;i<n;i++)
        {
            int x = UnityEngine.Random.Range(0,tiles.GetLength(0));
            int y = UnityEngine.Random.Range(0,tiles.GetLength(1));
            if(!tiles[x,y]) continue;
            int x2 = UnityEngine.Random.Range(0,tiles.GetLength(0));
            int y2 = UnityEngine.Random.Range(0,tiles.GetLength(1));
            MoveFlower2(tiles[x,y].transform,x2,y2);
        }
    }
    public void ClickFlower(Transform ts)
    {
        if(!nowChoiced)
        {
            nowChoiced = ts.gameObject;
            choicedEffect.transform.position = ts.position;
            choicedEffect.SetActive(true);
        }
        else
        {
            if(ts.gameObject == nowChoiced) return;
            choicedEffect.SetActive(false);
            if(ts.GetComponent<SpriteRenderer>().sprite!=nowChoiced.GetComponent<SpriteRenderer>().sprite)
            {
                nowChoiced = null;
            }
            else
            {
                line.positionCount = 0;
                line.SetPosition(++line.positionCount-1,new Vector3(nowChoiced.transform.position.x,nowChoiced.transform.position.y,-1)); 
                (int x1,int y1) = GetFlowerPosition(ts);
                (int x2,int y2) = GetFlowerPosition(nowChoiced.transform);
                if(IsConnect(new Vector2(x1,y1),new Vector2(x2,y2)))
                {
                    line.SetPosition(++line.positionCount-1,new Vector3(ts.position.x,ts.position.y,-1));
                    line.gameObject.SetActive(true);
                    RemoveFlower(ts);
                    RemoveFlower(nowChoiced.transform);
                    if(remainedFlower<=0)
                    {
                        gameMode++;
                        txt_level.text = $"第{gameMode}关";
                        if(gameMode>9)
                        {
                            HidePanel();
                            EndPanel.Instance.ShowPanel();
                            return;
                        }
                            
                        GenerateTiles();
                        remainedFlower = 140;
                        remainedTime = totalTime;
                    }
                }
                else
                {
                    nowChoiced = null;
                }
            }
        }
    }
    private void RemoveFlower(Transform ts)
    {
        (int x,int y) = GetFlowerPosition(ts);
        if(!tiles[x,y]) return;
        Destroy(tiles[x,y]);
        tiles[x,y] = null;
        remainedFlower--;
        if(gameMode == 1) return;
        if(gameMode == 2)
        {
            for(int i = x-1;i>=0;i--)
            {
                if(tiles[i,y])
                    MoveFlower2(tiles[i,y].transform,i+1,y);
            }
        }
        if(gameMode == 3)
        {
            for(int i = x+1;i<tiles.GetLength(0);i++)
            {
                if(tiles[i,y])
                    MoveFlower2(tiles[i,y].transform,i-1,y);
            }
        }
        if(gameMode ==4 )
        {
            for(int i = y+1;i<tiles.GetLength(1);i++)
            {
                if(tiles[x,i])
                    MoveFlower2(tiles[x,i].transform,x,i-1);
            }
        }
        if(gameMode == 5)
        {
            for(int i = y-1;i>=0;i--)
            {
                if(tiles[x,i])
                    MoveFlower2(tiles[x,i].transform,x,i+1);
            }
        }
        if(gameMode == 6)
        {
            for(int i = y-1;i>tiles.GetLength(1)/2;i--)
            {
                if(tiles[x,i])
                    MoveFlower2(tiles[x,i].transform,x,i+1);
            }
            for(int i = y+1;i<=tiles.GetLength(1)/2;i++)
            {
                if(tiles[x,i])
                    MoveFlower2(tiles[x,i].transform,x,i-1);
            }
        }
        if(gameMode == 7)
        {
            for(int i = x-1;i>tiles.GetLength(0)/2;i--)
            {
                if(tiles[i,y])
                    MoveFlower2(tiles[i,y].transform,i+1,y);
            }
            for(int i = x+1;i<=tiles.GetLength(0)/2;i++)
            {
                if(tiles[i,y])
                    MoveFlower2(tiles[i,y].transform,i-1,y);
            }
        }
        if(gameMode == 8)
        {
            for(int i = x-1;i>=0&&i<=tiles.GetLength(0)/2;i--)
            {
                if(tiles[i,y])
                    MoveFlower2(tiles[i,y].transform,i+1,y);
            }
            for(int i = x+1;i<tiles.GetLength(0)&&i>tiles.GetLength(0)/2;i++)
            {
                if(tiles[i,y])
                    MoveFlower2(tiles[i,y].transform,i-1,y);
            }
        }
        if(gameMode ==9)
        {
            for(int i = y-1;i>=0&&i<=tiles.GetLength(1)/2;i--)
            {
                if(tiles[x,i])
                    MoveFlower2(tiles[x,i].transform,x,i+1);
            }
            for(int i = y+1;i<tiles.GetLength(1)&&i>tiles.GetLength(1)/2;i++)
            {
                if(tiles[x,i])
                    MoveFlower2(tiles[x,i].transform,x,i-1);
            }
        }
    }
    private bool IsConnect(Vector2 pos1,Vector2 pos2)
    {
        if(IsStraight(pos1,pos2)) return true;
        if(IsCornerOnce(pos1,pos2)) return true;
        if(IsCornerTwice(pos1,pos2)) return true;
        return false;
    }
    private bool IsStraight(Vector2 pos1,Vector2 pos2)
    {
        if(pos1.x!=pos2.x&&pos1.y!=pos2.y) return false;
        if(pos1.x==pos2.x)
        {
            int start = (int)math.min(pos1.y,pos2.y);
            int end = (int)math.max(pos1.y,pos2.y);
            if(start - end == 1) return true;
            start ++;
            while(start<end)
            {
                if(IsExist((int)pos1.x,start)) return false;
                start++;
            }
        }
        else if(pos1.y==pos2.y)
        {
            int start = (int)math.min(pos1.x,pos2.x);
            int end = (int)math.max(pos1.x,pos2.x);
            if(start - end == 1) return true;
            start ++;
            while(start<end)
            {
                if(IsExist(start,(int)pos1.y)) return false;
                start++;
            }
        }
        return true;
    }
    private bool IsExist(int x,int y)
    {
        if(x<0||x>=tiles.GetLength(0)||y<0||y>=tiles.GetLength(1)) return false;
        if(tiles[x,y]) return true;
        return false;
    }
    private bool IsCornerOnce(Vector2 pos1,Vector2 pos2)
    {
        if(pos1.x==pos2.x||pos1.y==pos2.y) return false;
        if(IsStraight(new Vector2(pos1.x,pos2.y),pos2)&&IsStraight(new Vector2(pos1.x,pos2.y),pos1)&&!IsExist((int)pos1.x,(int)pos2.y))
        {
            line.SetPosition(++line.positionCount-1,new Vector3(-6.5f+pos2.y,4-pos1.x,-1));
            return true;
        }
        if(IsStraight(new Vector2(pos2.x,pos1.y),pos2)&&IsStraight(new Vector2(pos2.x,pos1.y),pos1)&&!IsExist((int)pos2.x,(int)pos1.y))
        {
            line.SetPosition(++line.positionCount-1,new Vector3(-6.5f+pos1.y,4-pos2.x,-1));
            return true;
        }
        return false;
    }
    private bool IsCornerTwice(Vector2 pos1,Vector2 pos2)
    {
        for(int i = (int)pos1.x+1;i<=tiles.GetLength(0);i++)
        {
            if(i<tiles.GetLength(0))
                if(tiles[i,(int)pos1.y]) break;
            if(IsCornerOnce(new Vector2(i,pos1.y),pos2)) 
            {
                line.SetPosition(++line.positionCount-1,new Vector3(-6.5f+pos1.y,4-i,-1));
                return true;
            }
        }
        for(int i = (int)pos1.x-1;i>=-1;i--)
        {
            if(i>=0)
                if(tiles[i,(int)pos1.y]) break;
            if(IsCornerOnce(new Vector2(i, pos1.y), pos2))
            {
                line.SetPosition(++line.positionCount-1,new Vector3(-6.5f+pos1.y,4-i,-1));
                return true;
            }
        }
        for(int i = (int)pos1.y+1;i<=tiles.GetLength(1);i++)
        {
            if(i<tiles.GetLength(1))
                if(tiles[(int)pos1.x,i]) break;
            if(IsCornerOnce(new Vector2(pos1.x,i),pos2))
            {
                line.SetPosition(++line.positionCount-1,new Vector3(-6.5f+i,4-pos1.x,-1));
                return true;
            }
        }
        for(int i = (int)pos1.y-1;i>=-1;i--)
        {
            if(i>=0)
                if(tiles[(int)pos1.x,i]) break;
            if(IsCornerOnce(new Vector2(pos1.x, i), pos2))
            {
                line.SetPosition(++line.positionCount-1,new Vector3(-6.5f+i,4-pos1.x,-1));
                return true;
            }
        }
        return false;
    }
    public void Update()
    {
        // if(Input.GetKeyUp(KeyCode.A))
        // {
        //     if(nowChoiced)
        //     {
        //         (int x,int y) = GetFlowerPosition(nowChoiced.transform);
        //         RemoveFlower(nowChoiced.transform);
        //         nowChoiced = null; 
        //     }
        // }
        if(IsPlaying)
        {
            remainedTime-= Time.deltaTime;
            if(remainedTime<=0)
            {
                HidePanel();
                EndPanel.Instance.ShowPanel();
                return;
            }
            img_hp.rectTransform.sizeDelta = new Vector2(remainedTime/totalTime*hpWidth,img_hp.rectTransform.sizeDelta.y);
        }
    }
}
