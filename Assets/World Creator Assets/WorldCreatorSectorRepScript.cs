﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreatorSectorRepScript : MonoBehaviour
{
    SpriteRenderer sprite;
    LineRenderer rend;
    public Sector sector;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rend = GetComponentInParent<LineRenderer>();
        transform.position = rend.bounds.center;
        transform.localScale = rend.bounds.size;
        sprite.color = SectorColors.colors[0];
    }

    void Update() 
    {
        transform.position = rend.bounds.center;
        transform.localScale = rend.bounds.size;
        ChangeColor();
    }

    void ChangeColor() 
    {
        if(sector) sprite.color = SectorColors.colors[(int)sector.type];
    }
}
