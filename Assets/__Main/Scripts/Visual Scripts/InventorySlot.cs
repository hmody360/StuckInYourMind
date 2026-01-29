using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Collectible _item;

    public void SetItem(Collectible item)
    {
        _item = item;
    }

    public Collectible GetItem()
    {
        return _item;
    }

    public void OpenItemSlot()
    {
            GameUIManager.instance.SetItemInfo(_item);
            GameUIManager.instance.OpenItemViewer();
    }
}
