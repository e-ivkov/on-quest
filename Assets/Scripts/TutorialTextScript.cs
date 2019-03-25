using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TutorialTextScript : MonoBehaviour
{
    [System.Serializable]
    public struct MonsterHint
    {
        public MonsterType monster;
        public Sprite sprite;
        public string text;
    }

    public Text HintText;

    public Image HintImage;

    public MonsterHint[] MonsterHintMap;

    public void ShowHint(MonsterType monster)
    {
        var hint = MonsterHintMap.First((_hint) => _hint.monster == monster);
        HintImage.sprite = hint.sprite;
        HintText.text = hint.text;
    }
}
