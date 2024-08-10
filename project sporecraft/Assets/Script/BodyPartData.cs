using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable Object/BodyPartsData ")]
public class BodyPartData : ScriptableObject
{
    public enum ItemType {eye }

    [Header("Part Info")]
    public ItemType itemType;
    public int partId;
    public string partName;
    public string partDesc;
    public Sprite partIcon;
    
    

}
