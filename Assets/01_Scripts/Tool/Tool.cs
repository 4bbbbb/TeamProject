using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    None,
    Axe,
    Hoe,
    Pickaxe,
    Shovel,
    WateringCan,
    FishingRod,
}

public class Tool : MonoBehaviour
{
    [SerializeField] private ToolType type;
    [SerializeField] private int upgrade;
    [SerializeField] private int durability;

    Dictionary<int, ToolData> dictToolData = new Dictionary<int, ToolData>();

    public void LoadFrom(ToolData toolData)
    {
        if (toolData == null)
            return;


    }
}
