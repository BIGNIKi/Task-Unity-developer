using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New card", menuName = "Card")]
public class Card : ScriptableObject
{
    public int id;
    public int timeToOpenSeconds;
    public int timeRemainToOpen;

    public new string name;
    public string description;
    public string resourceGameObjectName;

    public Vector3 position;
    public Vector3 scale;

    public Quaternion rotation;


    public bool isOpen;
    public bool isOpeningProcessStarted;
}
