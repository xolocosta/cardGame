using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollow : MonoBehaviour
{
    [SerializeField] private float _offset;

    private GameObject _canvas;
    private IHealth _health;
    private IBlock _block;
    private Text _healthText;
    private Text _blockText;
    private GameObject _healthBar;
    private GameObject _instance;
    private Slider _slider;
    private Camera _camera;

    private void OnEnable()
    {
        _health = GetComponentInParent<IHealth>();
        _block = GetComponentInParent<IBlock>();

        _canvas = GameObject.Find("Canvas");
        _healthBar = Resources.Load<GameObject>("Prefabs/HealthBar");
        _instance = Instantiate(_healthBar);
        _instance.transform.SetParent(_canvas.transform);
        _instance.transform.localScale = new Vector2(1.5f, 1.5f);
        _slider = _instance.GetComponent<Slider>();
        _slider.minValue = 0;
        _slider.maxValue = _health.Health;
        _slider.value = _health.CurrentHP;

        _healthText = _instance.transform.Find("Health Value").GetComponent<Text>();
        _blockText = _instance.transform.Find("Block Value").GetComponent<Text>();

        _camera = Camera.main;
    }
    private void Update()
    {
        _instance.transform.position = new Vector2(transform.position.x, transform.position.y + _offset);
        _slider.value = _health.CurrentHP;
        if (_health.CurrentHP <= 0)
        {
            Destroy(_instance);
            Destroy(this);
        }
        _healthText.text = $"{_health.CurrentHP.ToString()}/{_health.Health.ToString()}";
        _blockText.text = _block.Block.ToString();

    }
}
