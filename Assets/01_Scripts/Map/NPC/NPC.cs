using UnityEngine;
using UnityEngine.InputSystem;

public enum NPCType
{
    FishShop,
    GroceryShop,
    WeaponShop,
    Fisherman,
    Pirate,
    TrainDriver

}

public class NPC : MonoBehaviour
{
    [Header("<< NPC 타입 >>")]
    [SerializeField] private NPCType npcType;

    [Header("<< 상호작용 >>")]    
    [SerializeField] private float interactionRange = 4f;    
    [SerializeField] private bool isPlayerInRange;

    private Transform player;

    public NPCType NPCType => npcType;
    public bool IsPlayerInRange => isPlayerInRange;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        CheckPlayerDistance();

        // *** 나중에 Player Interact로 바꿀예정
        if (isPlayerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            NPCManager.Instance.InteractWithCurrentNPC();
        }
    }

    private void CheckPlayerDistance()
    {
        if(player == null) 
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        bool isInCurrentRange = distance <= interactionRange;

        if (isInCurrentRange == isPlayerInRange)
            return;

        isPlayerInRange = isInCurrentRange;


        isPlayerInRange = isInCurrentRange;

        if (isPlayerInRange)
        {
            Debug.Log($"{npcType} NPC 상호작용 범위에 들어옴");

            if (NPCManager.Instance != null)
                NPCManager.Instance.SetCurrentNPC(this);
        }
        else
        {
            Debug.Log($"{npcType} NPC 상호작용 범위에서 나감");

            if (NPCManager.Instance != null)
                NPCManager.Instance.ClearCurrentNPC(this);
        }
    }   

    private void OnDrawGizmos()
    {
        if (!isPlayerInRange)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
