using UnityEngine;

[System.Serializable]
public class Collectible
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _isSecret;

    public void setId(int id)
    {
        _id = id;
    }

    public void setName(string name)
    {
        _name = name;
    }

    public void setDescription(string description)
    {
        _description = description;
    }

    public void setIcon(Sprite icon)
    {
        _icon = icon;
    }

    private void ToggleSecret()
    {
        _isSecret = !_isSecret;
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

    public bool getIsSecret()
    {
        return _isSecret;
    }
}
