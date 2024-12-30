using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message
{
    public string name;
    public int ID;
    [Multiline(10)]
    public string[] message;
}
[CreateAssetMenu(fileName = ("DialogSO"), menuName = "Scriptable Object/DialogSO")]
public class DialogSO : ScriptableObject
{
    public Message[] messages;
}

