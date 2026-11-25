using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scritable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite sprite;
    [TextArea] public string description;


}
