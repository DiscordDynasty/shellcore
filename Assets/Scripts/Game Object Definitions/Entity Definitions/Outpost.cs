﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : AirConstruct, IVendor {

    public VendingBlueprint vendingBlueprint;

    public VendingBlueprint GetVendingBlueprint()
    {
        return vendingBlueprint;
    }

    protected override void Start()
    {
        category = EntityCategory.Station;
        base.Start();
    }
    protected override void Awake()
    {
        base.Awake();
    }

    public override void RemovePart(ShellPart part) {
        if (part)
            if (part.gameObject.name != "Shell Sprite")
            {
                Destroy(part.gameObject);
            }
    }

    protected override void Update()
    {
        base.Update();
        targeter.GetTarget(true);
        Bullet[] bullets = GetComponentsInChildren<Bullet>();
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].Tick(null);
        }
    }

    protected override void DeathHandler()
    {
        if (currentHealth[0] <= 0 && !isDead)
        {
            OnDeath(); // switch factions
        }
    }

    protected override void OnDeath()
    {

        // this won't trigger PostDeath() since that only gets called if the timer ticks to a value
        // the timer doesn't tick unless isDead is set to true

        faction = faction == 1 ? 0 : 1;
        for (int i = 0; i < parts.Count; i++)
        {
            RemovePart(parts[i]);
        }
        targeter.SetTarget(null);
        GameObject.Find("SectorManager").GetComponent<BattleZoneManager>().UpdateCounters();
        Start();
    }
}