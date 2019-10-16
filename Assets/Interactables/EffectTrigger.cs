using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    Location location;
    bool playerHere = false;
    bool enemyHere = false;

    // Start is called before the first frame update
    void Start()
    {
        location = GetComponentInParent<Location>();
        playerHere = location.hasCharacter;
        enemyHere = location.hasEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        bool playerHere = location.hasCharacter;
        bool enemyHere = location.hasEnemy;

        if (playerHere && !this.playerHere)
        {
            location.BroadcastMessage("OnEnterCharacter", SendMessageOptions.DontRequireReceiver);
        } else if (!playerHere && this.playerHere)
        {
            location.BroadcastMessage("OnExitCharacter", SendMessageOptions.DontRequireReceiver);
        }
        if (enemyHere && !this.enemyHere)
        {
            location.BroadcastMessage("OnEnterEnemy", SendMessageOptions.DontRequireReceiver);
        } else if (!enemyHere && this.enemyHere)
        {
            location.BroadcastMessage("OnExitEnemy", SendMessageOptions.DontRequireReceiver);
        }
        this.playerHere = playerHere;
        this.enemyHere = enemyHere;

    }
}
