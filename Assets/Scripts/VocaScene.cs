using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocaScene : MonoBehaviour
{
    protected VocaSetup curVocaSetup = new VocaSetup();
    protected GameMgr.Vocabulary curVoca = GameMgr.Vocabulary.None;
    protected List<GameObject> coverObjs = new List<GameObject>();
    protected List<GameObject> letters = new List<GameObject>();

    protected const float lerpReachEnd = 10.0f;
    protected const float offsetDecrease = 0.1f;

    protected Vector2 largestSize = new Vector2();
    protected Transform parentTrans;
    protected bool isComplete = false;

    public void Init(Transform parentTrans, GameMgr.Vocabulary voca, VocaSetup vocaSetup, List<GameObject> letterObjs)
    {
        this.parentTrans = parentTrans;
        this.curVoca = voca;
        this.curVocaSetup = vocaSetup;

        this.letters.Clear();
        this.letters = letterObjs;

        SpawnCoverObjs();

        // save largest size
        foreach (GameObject letterObj in letters)
        {
            RectTransform letterRt = letterObj.transform as RectTransform;
            Vector2 letterSize = new Vector2(letterRt.sizeDelta.x * letterRt.lossyScale.x, letterRt.sizeDelta.y * letterRt.localScale.y);
            if (letterSize.x > largestSize.x)
                largestSize.x = letterSize.x;
            if (letterSize.y > largestSize.y)
                largestSize.y = letterSize.y;
        }
    }

    public virtual void Init()
    {
        if (this.coverObjs.Count == 0)
            return;

        // random position
        if (this.curVocaSetup != null && this.curVocaSetup.isRandomPos)
            RandomPosition(this.curVoca.ToString());
    }

    public virtual void Update()
    {
        if (isComplete)
        {
            DoReachEndPosition();
        }
    }

    protected void RandomPosition(string voca)
    {
        if (voca.Length > letters.Count)
            return;

        Vector2 rdRange = (parentTrans.parent.transform as RectTransform).sizeDelta;
        rdRange = new Vector2(rdRange.x / 2.0f - this.largestSize.x, rdRange.y / 2.0f - this.largestSize.y);

        for (int i = 0; i < voca.Length; i++)
        {
            GameObject letterObj = letters[i];
            Vector2 rdPos = new Vector2(Random.Range(-rdRange.x, rdRange.x), Random.Range(-rdRange.y, rdRange.y));
            (letterObj.transform as RectTransform).localPosition = rdPos;
        }
    }

    protected void DoReachEndPosition()
    {
        if (curVocaSetup.endReachPos.Count != curVoca.ToString().Length)
            return;

        float lerpScale = 1.0f;
        for (int i = 0; i < letters.Count; i++)
        {
            Vector2 reachPos = curVocaSetup.endReachPos[i];
            Vector2 pos = letters[i].transform.localPosition;

            if (!Vector2.Equals(reachPos, pos))
            {
                lerpScale -= offsetDecrease * i;
                pos = Vector2.Lerp(pos, reachPos, lerpScale * lerpReachEnd * Time.deltaTime);

                if (Vector2.Distance(pos, reachPos) <= 0.1f)
                    pos = reachPos;

                letters[i].transform.localPosition = pos;
            }
        }
    }

    protected void SpawnCoverObjs()
    {
        for (int i = 0; i < this.curVocaSetup.coverPrefs.Count; i++)
        {
            GameObject coverObj = Instantiate(this.curVocaSetup.coverPrefs[i], parentTrans);
            coverObj.SetActive(false);
            this.coverObjs.Add(coverObj);
        }
    }
}
