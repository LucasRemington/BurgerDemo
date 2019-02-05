using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueElement
{

    public enum Characters { DialogBox, Protag, Dennis, TutorialMaster};
    public enum AvatarPos { overworld, combat};
    public Characters Character;
    public AvatarPos CharacterPosition;
    public Texture2D CharacterPic;
    public string DialogueText;
    public GUIStyle DialogueTextStyle;
    public float TextPlayBackSpeed;
    public AudioClip PlayBackSoundFile;

}
