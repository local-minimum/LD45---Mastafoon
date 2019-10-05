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

    public int turnsTaken
    {
        get
        {
            return turns;
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
