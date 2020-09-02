using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas Instance;
    public VocaConfig vocabularyConfig;
    public GameObject prefLetter;

    private const int aASCII = 97;
    private const int zASCII = 122;

    private bool isUpdateAnim = false;
    private GameMgr.Vocabulary curVoca = GameMgr.Vocabulary.None;
    private List<GameObject> letters = new List<GameObject>();
    private List<GameObject> coverObjs = new List<GameObject>();
    private VocaSetup curVocaSetup = new VocaSetup();

    private Vector2 largestSize = new Vector2();
    private Camera mainCam;

    private void Awake()
    {
        GameCanvas.Instance = this;
    }

    void Start()
    {
        this.mainCam = Camera.main;
    }

    private void SpawnCoverObjs()
    {
        for (int i = 0; i < this.curVocaSetup.coverPrefs.Count; i++)
        {
            GameObject coverObj = Instantiate(this.curVocaSetup.coverPrefs[i], transform);
            coverObj.SetActive(false);
            this.coverObjs.Add(coverObj);
        }
    }

    private void SpawnVoca()
    {
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

                // save largest size
                Vector2 letterSize = new Vector2(letterRt.sizeDelta.x * letterRt.lossyScale.x, letterRt.sizeDelta.y * letterRt.localScale.y);
                if (letterSize.x > largestSize.x)
                    largestSize.x = letterSize.x;
                if (letterSize.y > largestSize.y)
                    largestSize.y = letterSize.y;
            }
        }

        // hide excess letter
        for (int i = voca.Length; i < letters.Count; i++)
            letters[i].SetActive(false);
    }

    void Update()
    {
        if (!isUpdateAnim)
            return;

        switch (this.curVoca)
        {
            case GameMgr.Vocabulary.zoom:
                UpdateVocaZoom();
                break;
        }
    }

    public void DoAnimVocabulary(GameMgr.Vocabulary voca)
    {
        this.isUpdateAnim = true;
        this.curVoca = voca;
        this.curVocaSetup = vocabularyConfig.GetVocaSetup(voca);

        SpawnVoca();
        SpawnCoverObjs();

        // random position
        if (this.curVocaSetup != null && this.curVocaSetup.isRandomPos)
            RandomPosition(this.curVoca.ToString());

        // init vocabulary
        InitVoca();
    }

    private void RandomPosition(string voca)
    {
        if (voca.Length > letters.Count)
            return;

        Vector2 rdRange = (transform.parent.transform as RectTransform).sizeDelta;
        rdRange = new Vector2(rdRange.x / 2.0f - this.largestSize.x, rdRange.y / 2.0f - this.largestSize.y);

        for (int i = 0; i < voca.Length; i++)
        {
            GameObject letterObj = letters[i];
            Vector2 rdPos = new Vector2(Random.Range(-rdRange.x, rdRange.x), Random.Range(-rdRange.y, rdRange.y));
            (letterObj.transform as RectTransform).localPosition = rdPos;
        }
    }

    #region init_voca
    private void InitVoca()
    {
        switch (this.curVoca)
        {
            case GameMgr.Vocabulary.zoom:
                InitVocaZoom();
                break;
        }
    }

    private void InitVocaZoom()
    {
        if (coverObjs.Count == 0)
            return;
        // spawn magnifier
        GameObject magnifier = coverObjs[0];
        magnifier.SetActive(true);

        // change parent of all letters
        for (int i = 0; i < GameMgr.Vocabulary.zoom.ToString().Length; i++)
        {
            if (i < letters.Count)
                letters[i].transform.parent = magnifier.transform.GetChild(0);
        }

        magnifier.SetActive(false);
    }
    #endregion

    #region init_voca
    private void UpdateVocaZoom()
    {
        if (coverObjs.Count == 0 || this.mainCam == null)
            return;

        GameObject magnifier = coverObjs[0];
        if (!Input.GetMouseButton(0))
        {
            if (magnifier.active)
                magnifier.SetActive(false);
            return;
        }

        // move magnifier
        if (!magnifier.active)
            magnifier.SetActive(true);

        List<Vector2> tmpPoses = new List<Vector2>();
        for (int i = 0; i < this.curVoca.ToString().Length; i++)
            tmpPoses.Add(letters[i].transform.position);

        magnifier.transform.position = Input.mousePosition;

        for (int i = 0; i < this.curVoca.ToString().Length; i++)
            letters[i].transform.position = tmpPoses[i];
    }
    #endregion
}
