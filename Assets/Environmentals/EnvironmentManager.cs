using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] WorldClock worldClock;
    List<Environmentals> environmentals = new List<Environmentals>();

    public void Add(Environmentals environmental)
    {
        environmentals.Add(environmental);
    }

    private void OnEnable()
    {
        worldClock.OnTick += WorldClock_OnTick;
    }

    private void OnDisable()
    {
        worldClock.OnTick -= WorldClock_OnTick;
    }

    private void WorldClock_OnTick(Turn turn, bool changeOfTurn)
    {
        for (int i=0,l=environmentals.Count; i<l; i++)
        {
            Environmentals env = environmentals[i];
            if (env.gameObject.activeInHierarchy)
            {
                env.Tick();
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
