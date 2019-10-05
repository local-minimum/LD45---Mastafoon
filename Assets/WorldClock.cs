using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn { Player, Enemies };

public delegate void TickEvent(Turn turn);

public class WorldClock : MonoBehaviour
{
    public event TickEvent OnTick;
    Turn turn = Turn.Player;
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
            OnTick?.Invoke(turn);
            turn = nextTurn;
        }
    }

    public void GiveTurnTo(Turn turn)
    {
        nextTurn = turn;
        turns++;
    }
}
