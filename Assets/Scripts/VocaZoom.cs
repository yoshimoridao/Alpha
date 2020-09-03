using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocaZoom : VocaScene
{
    private Camera mainCam;

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
}
