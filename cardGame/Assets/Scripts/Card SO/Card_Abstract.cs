using UnityEngine;

//[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public abstract class Card_Abstract : ScriptableObject
{
    [SerializeField()] protected int _energy;
    [SerializeField()] protected bool _isFriendly;
    [SerializeField()] protected int _blockValue;
    [SerializeField()] protected int _healValue;
    [SerializeField()] protected int _damageValue;
    public int EnergyValue { get => _energy; }
    public bool IsFriendly{ get => _isFriendly; }
    public int Block { get => _blockValue; }
    public int DamageValue { get => _damageValue; }
    public int HealValue { get => _healValue; }
}