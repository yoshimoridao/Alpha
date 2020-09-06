using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocaZoom : VocaScene
{
    private Camera mainCam;
    private float delayReachEnd = 0.5f;
    private bool isCountDownReachEnd = false;

    public override void Init()
    {
        if (this.coverObjs.Count == 0)
            return;
        base.Init();
        this.mainCam = Camera.main;

        // spawn magnifier
        GameObject magnifier = this.coverObjs[0];
        magnifier.SetActive(true);

        // change parent of all letters
        for (int i = 0; i < GameMgr.Vocabulary.zoom.ToString().Length; i++)
        {
            if (i < letters.Count)
                letters[i].transform.parent = magnifier.transform.GetChild(0);
        }

        magnifier.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (this.isComplete || this.coverObjs.Count == 0 || this.mainCam == null)
            return;

        if (this.isCountDownReachEnd)
        {
            this.delayReachEnd -= Time.deltaTime;
            if (this.delayReachEnd <= 0)
            {
                this.delayReachEnd = 0;
                this.isComplete = true;
            }

            
            return;
        }

        RectTransform magnifier = coverObjs[0].transform as RectTransform;
        if (!Input.GetMouseButton(0))
        {
            if (magnifier.gameObject.active)
                magnifier.gameObject.SetActive(false);
            return;
        }

        // move magnifier
        if (!magnifier.gameObject.active)
            magnifier.gameObject.SetActive(true);

        // filter visible letter
        List<GameObject> invisibleLetters = new List<GameObject>();
        foreach (var letterObj in letters)
        {
            if (letterObj.transform.parent != magnifier.parent)
                invisibleLetters.Add(letterObj);
        }
        
        // store position of letter
        List<Vector2> tmpPoses = new List<Vector2>();
        for (int i = 0; i < invisibleLetters.Count; i++)
            tmpPoses.Add(invisibleLetters[i].transform.position);

        // change position of magnifier
        magnifier.transform.position = Input.mousePosition;

        // keep position of letter
        float visibleZone = magnifier.sizeDelta.x / 2.0f;
        for (int i = 0; i < invisibleLetters.Count; i++)
        {
            GameObject letterObj = invisibleLetters[i];
            letterObj.transform.position = tmpPoses[i];

            // check visible letter
            float distanceLetter = Vector2.Distance(letterObj.transform.position, magnifier.position);
            if (distanceLetter < visibleZone)
            {
                letterObj.transform.parent = magnifier.parent;

                // end do action
                if (invisibleLetters.Count == 1)
                {
                    magnifier.gameObject.SetActive(false);
                    this.isCountDownReachEnd = true;
                }
            }
        }
    }
}
