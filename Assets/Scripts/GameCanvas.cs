using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas Instance;
    public VocaConfig vocabularyConfig;
    public GameObject prefLetter;

    private bool isUpdateAnim = false;
    private GameMgr.Vocabulary curVoca = GameMgr.Vocabulary.None;
    private List<GameObject> letters = new List<GameObject>();
    private VocaScene vocaSceneMgr = new VocaScene();

    private Camera mainCam;

    private void Awake()
    {
        GameCanvas.Instance = this;
    }

    void Start()
    {
        this.mainCam = Camera.main;
    }

    private List<GameObject> SpawnVoca()
    {
        List<GameObject> createdVocas = new List<GameObject>();
        string voca = this.curVoca.ToString();
        for (int i = 0; i < voca.Length; i++)
        {
            char c = (char)voca[i];

            // gen letter
            if (i >= letters.Count)
            {
                letters.Add(Instantiate(prefLetter, transform));
            }
            GameObject letterObj = letters[i];

            Sprite loadSprite = Resources.Load<Sprite>("Sprites/" + c.ToString());
            if (loadSprite != null)
            {
                letterObj.name = c.ToString();
                letterObj.GetComponent<Image>().sprite = loadSprite;
                RectTransform letterRt = letterObj.transform as RectTransform;
                letterRt.sizeDelta = new Vector2(loadSprite.rect.size.x, loadSprite.rect.size.y);
                letterObj.SetActive(true);

                // save vocas
                createdVocas.Add(letterObj);
            }
        }

        // hide excess letter
        for (int i = voca.Length; i < letters.Count; i++)
            letters[i].SetActive(false);

        return createdVocas;
    }

    void Update()
    {
        if (!isUpdateAnim)
            return;

        this.vocaSceneMgr.Update();
    }

    public void DoAnimVocabulary(GameMgr.Vocabulary voca)
    {
        this.isUpdateAnim = true;
        this.curVoca = voca;

        RefreshScene();
        this.vocaSceneMgr.Init(this.transform, this.curVoca, vocabularyConfig.GetVocaSetup(voca), SpawnVoca());

        // init vocabulary
        this.vocaSceneMgr.Init();
    }

    private void RefreshScene()
    {
        switch (this.curVoca)
        {
            case GameMgr.Vocabulary.zoom:
                this.vocaSceneMgr = new VocaZoom();
                break;
        }
    }
}
