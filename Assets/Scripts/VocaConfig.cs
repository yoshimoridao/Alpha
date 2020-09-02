using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="VocabularyConfig", menuName = "Config")]
public class VocaConfig : ScriptableObject
{
    [SerializeField]
    List<VocaSetup> vocaSetups = new List<VocaSetup>();

    public VocaSetup GetVocaSetup(GameMgr.Vocabulary voca)
    {
        for (int i = 0; i < vocaSetups.Count; i++)
        {
            if (vocaSetups[i].voca.ToLower() == voca.ToString().ToLower())
                return vocaSetups[i];
        }

        return null;
    }
}

[System.Serializable]
public class VocaSetup
{
    public string voca = string.Empty;
    public List<Vector2> endReachPos = new List<Vector2>();
    public bool isRandomPos = false;
    public List<GameObject> coverPrefs = new List<GameObject>();
}
