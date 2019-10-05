using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] WorldClock worldClock;
    [SerializeField] List<Enemy> enemies = new List<Enemy>();

    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    private void OnEnable()
    {
        worldClock.OnTick += WorldClock_OnTick;
    }

    private void OnDisable()
    {
        worldClock.OnTick -= WorldClock_OnTick;
    }

    Turn previousTurn = Turn.Player;

    private void WorldClock_OnTick(Turn turn)
    {
        if (turn == Turn.Enemies)
        {
            if (previousTurn != turn)
            {
                ResetTicks();
            }
            if (!Tick())
            {
                worldClock.GiveTurnTo(Turn.Player);
            }
        }
        previousTurn = turn;
    }

    void ResetTicks()
    {
        for (int i = 0, l=enemies.Count; i < l; i++)
        {
            enemies[i].ResetTicks();
        }
    }

    bool Tick()
    {
        bool anyTicked = false;
        for (int i=0, l=enemies.Count; i<l; i++)
        {
            Enemy enemy = enemies[i];
            if (enemy.gameObject.activeInHierarchy)
            {
                if (enemy.Tick()) anyTicked = true;
            }
        }
        return anyTicked;
    }
}
