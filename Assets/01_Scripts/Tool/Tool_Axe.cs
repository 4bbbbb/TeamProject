using UnityEngine;

public class Tool_Axe : MonoBehaviour, ITool
{
    [SerializeField] private int toolId;

    private ToolData data;

    private bool bEquip;
    private bool bUse;

    private void Start()
    {
        ToolData toolData = ToolLoadManager.Instance.GetToolData(toolId);

        if (toolData == null)
        {
            Debug.LogError($"ToolData is null\nID : {toolId}");
            return;
        }

        Init(toolData);
    }

    public void Init(ToolData data)
    {
        this.data = data;
        Debug.Log($"ToolData 적용 완료\nID : {data.id}\nName : {data.name}");
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
