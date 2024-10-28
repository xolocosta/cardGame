using UnityEngine;

//[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public abstract class Enemy_Abstract : ScriptableObject
{
    public string Name { get => _name; }
    public int Health { get => _health; }
    public int Block { get => _blockValue; }
    public int Shield { get => _shieldValue; }
    public int AttackValue { get => _attackValue; }

    [SerializeField()] protected string _name;
    [SerializeField()] protected int _health;
    [SerializeField()] protected int _blockValue;
    [SerializeField()] protected int _shieldValue;
    [SerializeField()] protected int _attackValue;
}
