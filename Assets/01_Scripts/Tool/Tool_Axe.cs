using UnityEngine;

public class Tool_Axe : MonoBehaviour, ITool
{
    private ToolData data;

    private bool bEquip;
    private bool bUse;

    public void Init(ToolData data)
    {
        this.data = data;
    }

    public void Equip()
    {
        if (bEquip)
            return;

        bEquip = true;
    }

    public void UnEquip()
    {
        bEquip = false;
    }

    public void Use()
    {
        if (bUse)
            return;
    }
}
