using UnityEngine;
using static GameEnums;

[System.Serializable]
public class Collectible
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;

    [TextArea(3, 6)]
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private CollectibleType _type;
    [SerializeField] private bool isWearable;

    public void SetId(int id)
    {
        _id = id;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetDescription(string description)
    {
        _description = description;
    }

    public void SetIcon(Sprite icon)
    {
        _icon = icon;
    }

    public void SetType(CollectibleType type)
    {
        _type = type;
    }

    public int getId()
    {
        return _id;
    }

    public string getName()
    {
        return _name;
    }

    public string getDesc()
    {
        return _description;
    }

    public Sprite getIcon()
    {
        return _icon;
    }

    public CollectibleType getType()
    {
        return _type;
    }

    public bool getIsWearble()
    {
        return isWearable;
    }
}
