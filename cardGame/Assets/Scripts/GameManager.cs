using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    Idle = 0,
    UnFriendlyCardSelected = 1,
    FriendlyCardSelected = 2,
    EnemiesSelected = 3,
    PlayerSelected = 4,
}
public enum Turn
{
    PlayerTurn = 0,
    EnemyTurn = 1,
    GameOver = 2
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _gameOverObjects;

    private Dictionary<Enum, Action> _turnToAction = new Dictionary<Enum, Action>();
    private Dictionary<Enum, Action<GameObject>> _gameStateToAction = new Dictionary<Enum, Action<GameObject>>();

    public static event Action OnUnFriendlyCardSelected;
    public static event Action OnFriendlyCardSelected;
    public static event Action OnReset;
    public static event Action OnEnemiesSelected;
    public static event Action OnPlayerSelected;

    public static event Action OnSwitchedToPlayerTurn;
    public static event Action OnSwitchedToEnemyTurn;
    public static event Action OnEnemiesDied;
    public static event Action OnRestart;

    public static event Action OnPlayerAttackedEnemy;
    public static event Action OnPlayerAttackedAllEnemies;

    public static event Action<int> OnPlayerHealed;
    public static event Action<int> OnPlayerBlocked;


    private List<IDamageable> _enemies;
    private IEnergy _player;
    private IAttackDuration _playerAttackDuration;
    private Camera _camera;

    private Turn _currentTurn = Turn.PlayerTurn;
    private GameState _gameState = GameState.Idle;
    private List<IDamageable> _selectedEnemies;
    private Card _selectedCard;
    private int _damageValueOfSelectedCard;

    private Vector3 _mousePos;
    private Vector3 _rayDirection;
    private RaycastHit2D _hit;

    private void Start()
    {
        _player = FindObjectOfType<Player>().GetComponent<IEnergy>();
        _playerAttackDuration = FindObjectOfType<Player>().GetComponent<IAttackDuration>();
        _enemies = new List<IDamageable>();
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var item in enemies)
            _enemies.Add(item.GetComponent<IDamageable>());
        _selectedEnemies = new List<IDamageable>();

        _camera = Camera.main;

        Subscribe();
        AssignActionsToDictionaries();
        OnSwitchedToPlayerTurn?.Invoke();
    }
    private void Update()
    {
        _turnToAction[_currentTurn].Invoke();
    }
    private void Subscribe()
    {
        OnPlayerSelected += HealOrBlockPlayer;
        OnEnemiesSelected += HealOrBlockPlayer;
        OnEnemiesSelected += DamageSelectedEnemies;

        OnReset += ResetStates;
        OnSwitchedToPlayerTurn += SwitchToPlayerTurn;
        OnSwitchedToEnemyTurn += SwitchToEnemyTurn;

        EnemyManager.OnPlayerDied += GameOver;
        OnEnemiesDied += GameOver;
        EnemyManager.OnEnemyTurnEnded += EndEnemyTurn;

        OnRestart += UnSubscribe;
    }
    private void UnSubscribe()
    {
        OnPlayerSelected -= HealOrBlockPlayer;
        OnEnemiesSelected -= HealOrBlockPlayer;
        OnEnemiesSelected -= DamageSelectedEnemies;

        OnReset -= ResetStates;
        OnSwitchedToPlayerTurn -= SwitchToPlayerTurn;
        OnSwitchedToEnemyTurn -= SwitchToEnemyTurn;

        EnemyManager.OnPlayerDied -= GameOver;
        OnEnemiesDied -= GameOver;
        EnemyManager.OnEnemyTurnEnded -= EndEnemyTurn;

        OnRestart -= UnSubscribe;
    }
    private void AssignActionsToDictionaries()
    {
        _turnToAction[Turn.PlayerTurn] = CheckInputs;
        _turnToAction[Turn.EnemyTurn] = WhileEnemyTurn;
        _turnToAction[Turn.GameOver] = WhileGameOver;

        _gameStateToAction[GameState.Idle] = x => CheckEndTurn(x);

        _gameStateToAction[GameState.Idle] += x => CheckCardSelection(x);
        _gameStateToAction[GameState.UnFriendlyCardSelected] = x => CheckEnemySelection(x);
        _gameStateToAction[GameState.FriendlyCardSelected] = x => CheckPlayerSelection(x);
        _gameStateToAction[GameState.EnemiesSelected] = x => WhileAttacking();
    }
    //private System.Action<PointerEventData> GetFunction(GameState gameState)
    //    => _gameStateToAction[_gameState];
    //private void CallFunction(GameState gameState, PointerEventData eventData)
    //    => _gameStateToAction[gameState](eventData);

    private void CheckInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePos = Input.mousePosition;
            _rayDirection = _camera.ScreenToWorldPoint(_mousePos);

            _hit = Physics2D.Raycast(_rayDirection, _camera.transform.position);
            if (_hit)
            {
                Debug.Log($"Hit an object {_hit.collider.gameObject.name}");
                CheckStates(_hit.collider.gameObject);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (_gameState == GameState.FriendlyCardSelected || _gameState == GameState.UnFriendlyCardSelected)
            {
                // play some sound
            }
            OnReset?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
            OnSwitchedToEnemyTurn?.Invoke();
        CheckReload();
    }
    private void WhileGameOver()
    {
        CheckReload();
    }
    private void WhileEnemyTurn()
    {
        CheckReload();
    }
    private void WhileAttacking()
    {
        CheckReload();
    }
    private void CheckReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRestart?.Invoke();
            SceneManager.LoadScene(0);
        }
    }
    private void CheckStates(GameObject gameObject)
        => _gameStateToAction[_gameState].Invoke(gameObject);
    private void CheckEndTurn(GameObject gameObject)
    {
        if (gameObject.name == "End Turn")
            OnSwitchedToEnemyTurn?.Invoke();
    }
    private void CheckCardSelection(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Card card))
        {
            if (_player.EnergyValue >= card.EnergyValue)
            {
                _selectedCard = card;
                var subscribe = card as ISelected;
                subscribe.OnSelected();

                card.TryGet(out IFriendly friendly);
                if (friendly.IsFriendly)
                {
                    _gameState = GameState.FriendlyCardSelected;
                    OnFriendlyCardSelected?.Invoke();
                }
                else
                {
                    _gameState = GameState.UnFriendlyCardSelected;
                    OnUnFriendlyCardSelected?.Invoke();
                }
            }
        }
    }
    private void CheckEnemySelection(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Enemy enemy))
        {
            if (_selectedCard.TryGet(out IDamage damage))
            {
                _selectedEnemies.Add(enemy as IDamageable);
                _damageValueOfSelectedCard = damage.DamageValue;
            }
            else if (_selectedCard.TryGet(out IDamageAll damageAll))
            {
                _selectedEnemies = _enemies;
                _damageValueOfSelectedCard = damageAll.DamageValue;
            }

            _gameState = GameState.EnemiesSelected;
            OnEnemiesSelected?.Invoke();
        }
    }
    private void CheckPlayerSelection(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Player player))
        {
            OnPlayerSelected?.Invoke();
            OnReset?.Invoke();
        }
    }
    private void DamageSelectedEnemies()
    {
        if (_selectedEnemies.Count > 1)
            OnPlayerAttackedEnemy?.Invoke();
        else
            OnPlayerAttackedAllEnemies?.Invoke();

        StartCoroutine(WaitAndAttack());
        
        Debug.Log("DamageSelectedEnemies Method is called");
    }
    private IEnumerator WaitAndAttack()
    {
        float duration = 0f;
        yield return new WaitUntil(() => IsAttackStarted(out duration));
        yield return new WaitForSeconds(duration);

        for (int i = _selectedEnemies.Count - 1; i >= 0; i--)
        {
            if (_selectedEnemies[i].TakeDamage(_damageValueOfSelectedCard))
            {
                _enemies.Remove(_selectedEnemies[i]);
                if (_enemies.Count == 0)
                {
                    OnEnemiesDied?.Invoke();
                }
            }
        }
        OnReset?.Invoke();
    }
    private bool IsAttackStarted(out float duration)
        => _playerAttackDuration.GetAttackDuration(out duration);
    private void HealOrBlockPlayer()
    {
        if (_selectedCard.TryGet(out IHeal heal))
            OnPlayerHealed?.Invoke(heal.HealValue);
        if (_selectedCard.TryGet(out IBlock block))
            OnPlayerBlocked?.Invoke(block.Block);

        Debug.Log("HealOrBlockPlayer Method is called");
    }
    private void ResetStates()
    {
        _selectedCard = null;
        _selectedEnemies = new List<IDamageable>();
        _gameState = GameState.Idle;
        Debug.Log("ResetStates is called");
    }
    private void GameOver()
    {
        foreach (var item in _gameOverObjects)
        {
            item.SetActive(true);
            Debug.Log($"{item.name} is set active");
        }

        _currentTurn = Turn.GameOver;
    }
    private void EndEnemyTurn()
    {
        OnSwitchedToPlayerTurn?.Invoke();
    }
    private void SwitchToEnemyTurn()
    {
        _currentTurn = Turn.EnemyTurn;
        Debug.Log("Switched to Enemy Turn");
    }
    private void SwitchToPlayerTurn()
    {
        _currentTurn = Turn.PlayerTurn;
        Debug.Log("Switched to Player Turn");
    }
}
