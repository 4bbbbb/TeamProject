using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    [SerializeField] private GameObject[] toolPrefabs;

    [SerializeField] private Transform toolPos;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


}
