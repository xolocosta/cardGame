using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IEnergy, IHealth, IBlock, IDamageable, IAttackDuration
{
    private const string _ATTACK_TRIGGER_NAME = "Attack 2";
    private const string _CLEAVE_ATTACK_TRIGGER_NAME = "Attack 1";
    private const string _TAKE_HIT_TRIGGER_NAME = "Hit";

    [SerializeField] private Player_SO _playerValues;
    [SerializeField] private Vector2 _energyPos;
    public int EnergyValue { get => _energy; }
    public int Health { get => _health; }
    public int CurrentHP { get => _currentHP; }
    public int Block { get => _block; }

    private int _energy;
    private int _health;
    private int _currentHP;
    private int _block;

    private GameObject _energyImage;
    private Text _energyText;
    private GameObject _canvas;

    private Animator _animator;
    private AnimatorStateInfo _animatorStateInfo;
    private float _attackDuration = 0f;

    private void Start()
    {
        GetValuesFromSO();
        _currentHP = _health;

        Subcribe();

        _canvas = GameObject.Find("Canvas");
        _energyImage = Instantiate(Resources.Load<GameObject>("Prefabs/Energy Image"), _canvas.transform);
        _energyImage.transform.localScale = new Vector2(1.2f, 1.2f);
        _energyImage.transform.position = _energyPos;
        _energyText = _energyImage.transform.Find("Energy Value").GetComponent<Text>();

        _animator = transform.Find("Player GFX").GetComponent<Animator>();

        GetComponent<HealthBarFollow>().enabled = true;
    }
    private void Update()
    {
        _energyText.text = $"{_energy.ToString()}/4";
        _energyImage.transform.position = _energyPos;
    }
    private void Subcribe()
    {
        GameManager.OnSwitchedToPlayerTurn += ResetEnergy;
        GameManager.OnPlayerBlocked += AddBlock;
        GameManager.OnPlayerHealed += Heal;
        GameManager.OnRestart += UnSubscribe;

        GameManager.OnPlayerAttackedEnemy += AttackEnemy;
        GameManager.OnPlayerAttackedAllEnemies += AttackAllEnemies;

        EnemyManager.OnPlayerDied += UnSubscribe;
    }
    private void UnSubscribe()
    {
        GameManager.OnSwitchedToPlayerTurn -= ResetEnergy;
        GameManager.OnPlayerBlocked -= AddBlock;
        GameManager.OnPlayerHealed -= Heal;
        GameManager.OnRestart -= UnSubscribe;

        GameManager.OnPlayerAttackedEnemy -= AttackEnemy;
        GameManager.OnPlayerAttackedAllEnemies -= AttackAllEnemies;

        EnemyManager.OnPlayerDied -= UnSubscribe;
    }
    public void GetValuesFromSO()
    {
        _health = _playerValues.Health;
        _block = _playerValues.Block;
    }
    private void ResetEnergy()
        => _energy = 4;

    private void AddBlock(int blockValue)
        => _block += blockValue;
    private void Heal(int healValue)
    {
        _currentHP += healValue;
        if (_currentHP >= _health)
            _currentHP = _health;
    }
    public bool TakeDamage(int damage)
    {
        TakeHit();

        _block -= damage;
        if (_block < 0)
        {
            int difference = System.Math.Abs(_block);
            _block = 0;
            _currentHP -= difference;
            if (_currentHP <= 0)
            {
                _currentHP = 0;
                return true;
            }
        }
        return false;
    }
    public bool SpendEnergy(int energyValue)
    {
        _energy -= energyValue;
        if (_energy < 0)
            return true;
        
        return false;
    }
    public void SetDuration(float duration)
        => _attackDuration = duration;
    private void AttackEnemy()
        => _animator.SetTrigger(_ATTACK_TRIGGER_NAME);
    private void AttackAllEnemies()
        => _animator.SetTrigger(_CLEAVE_ATTACK_TRIGGER_NAME);
    private void TakeHit()
        => _animator.SetTrigger(_TAKE_HIT_TRIGGER_NAME);
    public bool GetAttackDuration(out float duration)
    {
        duration = _attackDuration;
        return duration != 0;
    }
}
