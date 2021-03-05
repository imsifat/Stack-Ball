using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    

    public GameObject HomeUI, inGameUI, finishUI, gameOverUI;
    public GameObject AllBTNs;
    private bool btns;

    [Header("PreGame")]
    public Button soundBTN;
    public Sprite soundONSpr, soundOFFSpr;
    

    [Header("InGame")]
    public Image levelSlider;
    public Image currentLevelImg;
    public Image nextLevelImg;
    public Text currentLvlTxt;
    public Text nxtLvlTxt;

    [Header("FinishUI")]
    public Text finishLevelText;

    [Header("GameOverUI")]
    public Text gameOverScoreText;
    public Text gameOverBestText;

    private Material PlayerMat;
    private Player player;

    void Awake()
    {
        PlayerMat = FindObjectOfType<Player>().transform.GetChild(0).GetComponent<MeshRenderer>().material;
        player = FindObjectOfType<Player>();

        levelSlider.transform.parent.GetComponent<Image>().color = PlayerMat.color + Color.gray;
        levelSlider.color = PlayerMat.color;
        currentLevelImg.color = PlayerMat.color;
        nextLevelImg.color = PlayerMat.color;

        soundBTN.onClick.AddListener(() => SoundManager.instance.SoundOnOff());
    }

    private void Start()
    {
        currentLvlTxt.text = FindObjectOfType<LevelSpawnner>().level.ToString();
        nxtLvlTxt.text = FindObjectOfType<LevelSpawnner>().level + 1 + "";
    }

    // Update is called once per frame
    void Update()
    {

        if(player.playerState == Player.PlayerState.Prepare)
        {
            if (SoundManager.instance.sound && soundBTN.GetComponent<Image>().sprite != soundONSpr)
                soundBTN.GetComponent<Image>().sprite = soundONSpr;
            else if (!SoundManager.instance.sound && soundBTN.GetComponent<Image>().sprite != soundOFFSpr)
                soundBTN.GetComponent<Image>().sprite = soundOFFSpr;
        }

        if(Input.GetMouseButtonDown(0) && !IgnoreUI() && player.playerState == Player.PlayerState.Prepare)
        {
            player.playerState = Player.PlayerState.Playing;
            HomeUI.SetActive(false);
            inGameUI.SetActive(true);
        }
        if (player.playerState == Player.PlayerState.Finish)
        {
            HomeUI.SetActive(false);
            inGameUI.SetActive(false);
            finishUI.SetActive(true);
            gameOverUI.SetActive(false);

            finishLevelText.text = "Level " + FindObjectOfType<LevelSpawnner>().level;
        }

        if (player.playerState == Player.PlayerState.Died)
        {
            HomeUI.SetActive(false);
            inGameUI.SetActive(false);
            finishUI.SetActive(false);
            gameOverUI.SetActive(true);

            gameOverScoreText.text = ScoreManager.instance.scoreText.text;
            gameOverBestText.text = PlayerPrefs.GetInt("HighScore").ToString();

            if (Input.GetMouseButtonDown(0))
            {
                ScoreManager.instance.ResetScore();
                SceneManager.LoadScene(0);
            }
        }
    }

    private bool IgnoreUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<Ignore>() != null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultList.Count > 0;
    }

    public void levelSliderFill(float Fillamount)
    {
        levelSlider.fillAmount = Fillamount;
    }

    

    public void settingsBTN()
    {
        btns = !btns;
        AllBTNs.SetActive(btns);
        
    }
}
