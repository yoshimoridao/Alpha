using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Instance;

    public enum Vocabulary { None, zoom };
    public static Vocabulary curVoca = Vocabulary.None;

    private void Awake()
    {
        GameMgr.Instance = this;
    }

    void Start()
    {
        GameCanvas.Instance.DoAnimVocabulary(Vocabulary.zoom);
    }

    void Update()
    {
        
    }
}
