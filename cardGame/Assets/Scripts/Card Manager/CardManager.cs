using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Vector2 _drawDeckPos;
    [SerializeField] private Vector2 _discardDeckPos;

    [SerializeField] private System.Collections.Generic.List<Transform> _cardPositions;
    [SerializeField] private System.Collections.Generic.List<GameObject> _deck;

    public System.Collections.Generic.List<GameObject> _playerDeck;
    public System.Collections.Generic.List<GameObject> _playerDeckInstances;
    public System.Collections.Generic.List<GameObject> _drawDeck;
    public System.Collections.Generic.List<GameObject> _discardDeck;

    private GameObject _canvas;
    private GameObject _drawDeckImage;
    private GameObject _discardDeckImage;
    private Text _drawDeckText;
    private Text _discardDeckText;

    private int n = 0;
    private int _rnd;
    private System.Collections.Generic.List<GameObject> _tempList;
    private System.Collections.Generic.List<int> _tempRemove;

    private IEnergy _playerEnergy;
    private void Start()
    {
        _playerEnergy = GameObject.FindGameObjectWithTag("Player").GetComponent<IEnergy>();
        _tempList = new System.Collections.Generic.List<GameObject>();
        _playerDeck = new System.Collections.Generic.List<GameObject>();
        _playerDeckInstances = new System.Collections.Generic.List<GameObject>();
        _drawDeck = new System.Collections.Generic.List<GameObject>();
        _discardDeck = new System.Collections.Generic.List<GameObject>();
        _tempRemove = new System.Collections.Generic.List<int>();

        _canvas = GameObject.Find("Canvas");
        _drawDeckImage = Instantiate(Resources.Load<GameObject>("Prefabs/Draw Image"), _canvas.transform);
        _drawDeckImage.transform.localScale = new Vector2(1.0f, 1.0f);
        _drawDeckImage.transform.position = _drawDeckPos;
        _drawDeckText = _drawDeckImage.transform.Find("Draw Value").GetComponent<Text>();

        _discardDeckImage = Instantiate(Resources.Load<GameObject>("Prefabs/Discard Image"), _canvas.transform);
        _discardDeckImage .transform.localScale = new Vector2(1.0f, 1.0f);
        _discardDeckImage.transform.position = _drawDeckPos;
        _discardDeckText = _discardDeckImage.transform.Find("Discard Value").GetComponent<Text>();

        MoveRandomly(_deck, _drawDeck, false);

        Subscribe();
    }
    private void Update()
    {
        //_drawDeckImage.transform.position = _drawDeckPos;
        _drawDeckText.text = _drawDeck.Count.ToString();

        //_discardDeckImage.transform.position = _discardDeckPos;
        _discardDeckText.text = _discardDeck.Count.ToString();
    }
    private void Subscribe()
    {
        GameManager.OnSwitchedToPlayerTurn += StartPlayerTurn;
        GameManager.OnSwitchedToEnemyTurn += EndPlayerTurn;
        GameManager.OnEnemiesSelected += OnCardPlayed;
        GameManager.OnPlayerSelected += OnCardPlayed;

        GameManager.OnRestart += ClearCardInstances;
        GameManager.OnRestart += UnSubscribe;
    }
    private void UnSubscribe()
    {
        GameManager.OnSwitchedToPlayerTurn -= StartPlayerTurn;
        GameManager.OnSwitchedToEnemyTurn -= EndPlayerTurn;
        GameManager.OnEnemiesSelected -= OnCardPlayed;
        GameManager.OnPlayerSelected -= OnCardPlayed;

        GameManager.OnRestart -= ClearCardInstances;
        GameManager.OnRestart -= UnSubscribe;
    }
    private void StartPlayerTurn()
    {
        DrawCardsToPlayerDeck();
        PutExistingCardsInARow();
        InstantiateCards();
    }
    private void EndPlayerTurn()
    {
        for (n = _playerDeckInstances.Count - 1; n >= 0; n--)
        {
            var card = _playerDeckInstances[n];
            _playerDeckInstances.RemoveAt(n);
            Destroy(card);
        }
        for (n = 0;  n < _playerDeck.Count; n++)
        {
            _discardDeck.Add(_playerDeck[n]);
            _playerDeck.RemoveAt(n);
        }
    }
    private void DrawCardsToPlayerDeck()
    {
        while (_playerDeck.Count < 5)
        {
            if (_drawDeck.Count == 0)
            {
                MoveRandomly(_discardDeck, _drawDeck);
            }
            _playerDeck.Add(_drawDeck[_drawDeck.Count - 1]);
            _drawDeck.RemoveAt(_drawDeck.Count - 1);
            n++;
        }
        n = 0;
    }
    private void InstantiateCards()
    {
        for (n = _playerDeckInstances.Count; n < 5; n++)
        {
            var item = _playerDeck[n];
            var prefabInstance = PrefabUtility.InstantiatePrefab(item, transform.root) as GameObject;
            prefabInstance.transform.position = _cardPositions[n].position;
            prefabInstance.GetComponent<IPlaced>().SetPosition();
            _playerDeckInstances.Add(prefabInstance);
        }
        _tempList.Clear();
        n = 0;
    }
    private void PutExistingCardsInARow()
    {
        if (_playerDeckInstances.Count <= 0)
            return;

        foreach (var item in _playerDeckInstances)
        {
            item.transform.position = _cardPositions[n].position;
            item.GetComponent<IPlaced>().SetPosition();
            n++;
        }
        n = 0;
    }
    private void OnCardPlayed()
    {
        foreach (var item in _playerDeckInstances)
        {
            item.TryGetComponent(out ISelected selected);
            if (selected.GetSelected())
            {
                selected.OnUnSelected();

                item.GetComponent<Card>().TryGet(out IEnergy energy);
                _playerEnergy.SpendEnergy(energy.EnergyValue);

                _discardDeck.Add(_playerDeck[n]);
                _playerDeck.RemoveAt(n);

                _tempRemove.Add(n);
            }
            n++;
        }
        RemovePlayedCard();
        n = 0;
    }
    private void RemovePlayedCard()
    {
        foreach (int index in _tempRemove)
        {
            var item = _playerDeckInstances[index];
            _playerDeckInstances.Remove(item);
            Destroy(item);
        }
        _tempRemove.Clear();
    }


    private void MoveRandomly(System.Collections.Generic.List<GameObject> from, System.Collections.Generic.List<GameObject> to, bool removeFromOriginalList = true)
    {
        if (removeFromOriginalList)
        {
            for (n = from.Count; n > 0 ; n--)
            {
                _rnd = UnityEngine.Random.Range(0, from.Count);
                to.Add(from.ElementAt(_rnd));
                from.RemoveAt(_rnd);
            }
        }
        else
        {
            _tempList = from;
            for (n = _tempList.Count; n > 0 ; n--)
            {
                _rnd = UnityEngine.Random.Range(0, from.Count);
                to.Add(from.ElementAt(_rnd));
                _tempList.RemoveAt(_rnd);
            }
        }
        n = 0;
    }
    private void ShuffleDiscardDeck()
    {
        MoveRandomly(_discardDeck, _tempList);
        _discardDeck = _tempList;
        _tempList.Clear();
    }
    private void ClearCardInstances()
    {
        for (int i = _playerDeckInstances.Count - 1; i >= 0; i--)
        {
            var instance = _playerDeckInstances[i];
            _playerDeckInstances.RemoveAt(i);
            Destroy(instance);
        }
    }
}
