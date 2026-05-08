using UnityEngine;

public class Tool_Axe : MonoBehaviour, ITool
{
    [SerializeField] private int toolId;

    private ToolData data;

    private new Collider collider;

    private string toolPosName = "ToolPos";
    private Transform toolPos;

    private GameObject rootObject;

    private bool bEquip;
    private bool bUse;

    private void Awake()
    {
        collider = GetComponent<Collider>();

        rootObject = transform.root.gameObject;
        Debug.Assert(rootObject != null, "RootObject is null");

        toolPos = rootObject.transform.FindChildByName(toolPosName);
        Debug.Assert(toolPosName != null, "ToolPosName is null");

        transform.SetParent(toolPos, false);
    }

    // ToolData 적용
    public void Init(ToolData data)
    {
        if (data == null)
        {
            Debug.LogError("ToolData is null");
            return;
        }

        this.data = data;
        Debug.Log($"Apply Completely\nID : {data.id}\nName : {data.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 충돌 처리 방지
        if (other.gameObject == rootObject)
            return;

        // 나무에만 충돌 가능
        if (other.CompareTag("Tree"))
        {
            data.durability -= data.reduce;

            if (data.durability < 0)
                Destroy(gameObject);
        }
    }

    // 소환, 충돌 처리만 담당

    public void Equip()
    {
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
