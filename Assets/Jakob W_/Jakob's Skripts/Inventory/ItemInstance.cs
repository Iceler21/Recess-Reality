using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]

public class ItemInstance : MonoBehaviour
{

    public ItemData data;

    private SpriteRenderer _sprt;

    private void Awake()
    {
        _sprt = GetComponent<SpriteRenderer>();
        _sprt.sprite = data.sprite;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Inventory.Items.Add(data);
        Destroy(gameObject);
    }
}
