using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn { None, Player, Enemies };

public delegate void TickEvent(Turn turn, bool changeOfTurn);

public class WorldClock : MonoBehaviour
{
    public event TickEvent OnTick;
    Turn turn = Turn.None;
    Turn nextTurn = Turn.Player;

    [SerializeField] float tickTime = 3f;
    int turns = 1;

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
        while (true)
        {
            yield return new WaitForSeconds(tickTime);
            bool changeOfTurn = turn != nextTurn;
            turn = nextTurn;
            OnTick?.Invoke(turn, changeOfTurn);
            if (turn == Turn.Player && changeOfTurn) turns++;            
        }
    }

    public void GiveTurnTo(Turn turn)
    {
        nextTurn = turn;
    }
}
