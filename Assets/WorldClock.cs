using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn { None, Player, Enemies };

public delegate void TickEvent(Turn turn, bool changeOfTurn);

public class WorldClock : MonoBehaviour
{
    public event TickEvent OnTick;
    Turn turn = Turn.None;
    Turn nextTurn = Turn.None;

    [SerializeField] float tickTime = 3f;
    int turns = 0;

    [SerializeField, Tooltip("Only evaluates to 1s")]
    AnimationCurve _enemyMoveEase;
    public AnimationCurve enemyMoveEase
    {
        get { return _enemyMoveEase;  }
    }

    [SerializeField, Range(0, 1)]
    float _enemyMoveDuration;    
    public float enemyMoveDuration
    {
        get { return _enemyMoveDuration; }
    }

    public int turnsTaken
    {
        get
        {
            return turns;
        }
    }

    public float tickDuration
    {
        get
        {
            return tickTime;
        }
    }

    private void Start()
    {
        StartCoroutine(ticker());
    }

    IEnumerator<WaitForSeconds> ticker() {
        bool changeOfTurn = true;
        OnTick?.Invoke(turn, changeOfTurn);
        while (true)
        {
            yield return new WaitForSeconds(tickTime);            
            if (turn != Turn.None) OnTick?.Invoke(turn, changeOfTurn);
            if (turn == Turn.Player && changeOfTurn)
            {
                turns++;
            }
            changeOfTurn = turn != nextTurn;
            turn = nextTurn;
        }
    }

    public void GiveTurnTo(Turn turn)
    {
        nextTurn = turn;
    }
}
