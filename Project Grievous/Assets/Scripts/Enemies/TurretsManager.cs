using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretsManager : MonoBehaviour
{
    public List<Turret> Turrets { get; private set; }
    public int NumberOfTurretsTotal { get; private set; }
    public int NumberOfTurretsRemaining => Turrets.Count;

    void Awake()
    {
        Turrets = new List<Turret>();
    }

    public void RegisterTurret(Turret turret)
    {
        Turrets.Add(turret);

        NumberOfTurretsTotal++;
    }

    public void UnregisterTurret(Turret turretKilled)
    {
        int turretsRemainingNotification = NumberOfTurretsRemaining - 1;

        //EnemyKillEvent evt = Events.EnemyKillEvent;
        //evt.Turret = turretKilled.gameObject;
        //evt.RemainingEnemyCount = enemiesRemainingNotification;
        //EventManager.Broadcast(evt);

        // removes the enemy from the list, so that we can keep track of how many are left on the map
        Turrets.Remove(turretKilled);
    }
}
