using UnityEngine;

public class Weapon : MonoBehaviour
{

    public string weaponName;
    [SerializeField] float damage;
    int _comboLength;

    BoxCollider _boxCollider;

    public float Damage { get => damage; set => damage = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Deal to " + other.transform.name + "for " + Damage + "damge");
    }

    public void EnableTriggerBox()
    {
        _boxCollider.enabled = true;
    }

    public void DisableTriggerBox()
    {
        _boxCollider.enabled = false;
    }
}
