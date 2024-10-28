using System.Collections;
using System.Linq;
using UnityEngine;

public enum EnemyAction { Attack = 0, Block = 1 }
public class EnemyManager : MonoBehaviour
{
    private System.Collections.Generic.Dictionary<System.Enum, System.Action> _rndNumberToAction = new System.Collections.Generic.Dictionary<System.Enum, System.Action>();

    public static event System.Action OnPlayerDied;
    public static event System.Action OnEnemyTurnEnded;

    private IDamageable _player;
    private System.Collections.Generic.List<Enemy> _enemies;
    private int _rnd = 0;
    private int _currentEnemy = 0;
    private void Start()
    {
        _player = FindObjectOfType<Player>().GetComponent<IDamageable>();
        _enemies = new System.Collections.Generic.List<Enemy>();

        GameManager.OnSwitchedToEnemyTurn += EnemyTurn;
        GameManager.OnRestart += UnSubcribe;

        //_rndNumberToAction[EnemyAction.Attack] = AttackPlayer;
        //_rndNumberToAction[EnemyAction.Block] = BlockEnemy;
    }
    private void UnSubcribe()
    {
        GameManager.OnSwitchedToEnemyTurn -= EnemyTurn;
        GameManager.OnRestart -= UnSubcribe;
    }

    private void EnemyTurn()
    {
        FindEnemies();
        StartCoroutine(ActionWithDelay());
    }
    private IEnumerator ActionWithDelay()
    {
        for (_currentEnemy = 0; _currentEnemy < _enemies.Count; _currentEnemy++)
        {
            yield return new WaitForSeconds(1.5f);
            _rnd = Random.Range(0, 2);
            if (_rnd == 0)
            {
                _enemies[_currentEnemy].GetComponent<IAttackTriggerSet>().SetAttackTrigger();

                float duration = 0f;
                yield return new WaitUntil(() => IsAttackingStarted(out duration));
                yield return new WaitForSeconds(duration);

                if (_player.TakeDamage(_enemies[_currentEnemy].AttackValue))
                    OnPlayerDied?.Invoke();
            }
            else
            {
                BlockEnemy();
            }
            //_rndNumberToAction[(EnemyAction)_rnd].Invoke();
        }
        _currentEnemy = 0;
        _enemies.Clear();
        yield return new WaitForSeconds(1.5f);
        OnEnemyTurnEnded?.Invoke();
    }
    private void FindEnemies()
    {
        _enemies.Clear();

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
            _enemies.Add(enemy.GetComponent<Enemy>());
    }
    //private void AttackPlayer()
    //{
    //    StartCoroutine(WaitAndAttack());
    //    Debug.Log("Enemy attacked Player");
    //}
    //private IEnumerator WaitAndAttack()
    //{
    //    yield return new WaitForSeconds(1f);
    //}
    private bool IsAttackingStarted(out float duration)
        => _enemies[_currentEnemy].GetComponent<IAttackDuration>().GetAttackDuration(out duration);
    private void BlockEnemy()
    {
        IBlockable enemy = _enemies[_currentEnemy] as IBlockable;
        int shieldValue = enemy.GetShieldValue();
        enemy.AddBlock(shieldValue);

        Debug.Log("Enemy shielded himself");
    }
}