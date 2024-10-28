using UnityEngine;

public abstract class Player_Abstract : ScriptableObject
{
    public string Name { get => _name; }
    public int Health { get => _health; }
    public int Block { get => _blockValue; }

    [SerializeField()] protected string _name;
    [SerializeField()] protected int _health;
    [SerializeField()] protected int _blockValue;
}
