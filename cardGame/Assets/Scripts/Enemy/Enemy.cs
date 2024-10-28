using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IHealth, IBlock, IBlockable, IAttackDuration, IAttackTriggerSet
{
    private const string _ATTACK_TRIGGER_NAME = "Attack";
    private const string _TAKE_HIT_TRIGGER_NAME = "Hit";

    [SerializeField] private Enemy_SO _enemyData;

    public string Name { get => _name; }
    public int Health { get => _health; }
    public int CurrentHP { get => _currentHP; }
    public int Block { get => _blockValue; }
    public int AttackValue { get => _attackValue; }

    private string _name;
    private int _health;
    private int _currentHP;
    private int _blockValue;
    private int _shieldValue;
    private int _attackValue;

    private Animator _animator;

    private float _attackDuration = 0f;

    private void Start()
    {
        GetValuesFromSO();
        _currentHP = _health;

        _animator = GetComponentInChildren<Animator>();

        GetComponent<HealthBarFollow>().enabled = true;
    }
    private void GetValuesFromSO()
    {
        _name = _enemyData.Name;
        _health = _enemyData.Health;
        _blockValue = _enemyData.Block;
        _shieldValue = _enemyData.Shield;
        _attackValue = _enemyData.AttackValue;
    }
    public bool TakeDamage(int damage)
    {
        _animator.SetTrigger(_TAKE_HIT_TRIGGER_NAME);

        _blockValue -= damage;
        if (_blockValue < 0)
        {
            int difference = System.Math.Abs(_blockValue);
            _blockValue = 0;
            _currentHP -= difference;
            if (_currentHP <= 0)
            {
                _currentHP = 0;
                Destroy(GetComponent<Collider2D>());
                this.tag = "Untagged";
                Destroy(gameObject, 2f);
                Debug.Log($"Enemy took {damage} damage and died");
                return true;
            }
        }
        Debug.Log($"Enemy took {damage} damage. Health left: {_currentHP}");
        return false;
    }
    public void SetAttackTrigger()
        => _animator.SetTrigger(_ATTACK_TRIGGER_NAME);
    public bool GetAttackDuration(out float duration)
    {
        duration = _attackDuration;
        return duration != 0;
    }
    public void SetDuration(float duration)
        => _attackDuration = duration;
    public void AddBlock(int shieldValue)
        => _blockValue += shieldValue;
    public int GetShieldValue()
        => _shieldValue;

}
