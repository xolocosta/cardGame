using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour, ISelected, IPlaced
{
    private const float _SPEED = 3.0f;

    [SerializeField] private Card_Abstract _cardData;

    public bool IsFriendly { get => _isFriendly; }
    public int EnergyValue { get => _energy; }
    public int BlockValue { get => _blockValue; }
    public int DamageValue { get => _damageValue; }
    public int HealValue { get => _healValue; }

    private bool _isFriendly;
    private int _energy;
    private int _blockValue;
    private int _healValue;
    private int _damageValue;


    private bool _isSelected;
    public Vector2 DefaultPos { get => _defPos; }
    public Vector2 NewPos { get => _defPos + new Vector2(0f, 1f); }
    private Vector2 _defPos;

    private Vector2 _nextPos;

    private void Start()
    {
        GetValuesFromSO();
    }
    private void GetValuesFromSO()
    {
        _isFriendly = _cardData.IsFriendly;
        _energy = _cardData.EnergyValue;
        _blockValue = _cardData.Block;
        _healValue = _cardData.HealValue;
        _damageValue = _cardData.DamageValue;
    }
    public bool TryGet<T>(out T result)
    {
        result = default(T);
        if (_cardData is T typeInterface)
        {
            result = typeInterface;
            return true;
        }

        return false;
    }

    public void SetPosition()
        => _defPos = transform.position;
    public bool GetSelected()
        => _isSelected;
    public void OnSelected()
    {
        TryGet(out IFriendly friendly);
        if (friendly.IsFriendly)
        {
            GameManager.OnFriendlyCardSelected += SlideUp;
            //GameManager.OnPlayerSelected += OnUnSelected;
        }
        else
        {
            GameManager.OnUnFriendlyCardSelected += SlideUp;
            //GameManager.OnEnemiesSelected += OnUnSelected;
        }

        GameManager.OnReset += SlideDown;
        GameManager.OnReset += OnUnSelected;
        _isSelected = true;
    }

    public void OnUnSelected()
    {
        TryGet(out IFriendly friendly);
        if (friendly.IsFriendly)
        {
            GameManager.OnFriendlyCardSelected -= SlideUp;
            //GameManager.OnPlayerSelected -= OnUnSelected;
        }
        else
        {
            GameManager.OnUnFriendlyCardSelected -= SlideUp;
            //GameManager.OnEnemiesSelected -= OnUnSelected;
        }

        GameManager.OnReset -= SlideDown;
        GameManager.OnReset -= OnUnSelected;
        _isSelected = false;
    }
    private void SlideUp()
    {
        StopAllCoroutines();
        StartCoroutine(WaitForAnimation(true));
    }
    private void SlideDown()
    {
        StopAllCoroutines();
        StartCoroutine(WaitForAnimation(false));
    }
    private IEnumerator WaitForAnimation(bool slideUp = true)
    {
        if (slideUp) _nextPos = NewPos;
        else _nextPos = DefaultPos;

        yield return new WaitUntil(() => IsDurationReached());
        transform.position = _nextPos;
    }
    private bool IsDurationReached()
    {
        transform.position = Vector2.Lerp(transform.position, _nextPos, Time.deltaTime * _SPEED);
        float distance = Vector2.Distance((Vector2)transform.position, _nextPos);

        return distance <= 0.01f;
    }
}
