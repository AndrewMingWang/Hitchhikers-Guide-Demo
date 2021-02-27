﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Color BaseColor;
    public Color HoverColor;

    [HideInInspector]
    public bool Hovered = false;
    [HideInInspector]
    public GameObject OccupyingBuilding = null;

    private Material _tileMaterial;

    public GameObject Top;

    private void Awake()
    {
        _tileMaterial = Top.GetComponent<Renderer>().materials[1];
        _tileMaterial.SetColor("_Color", BaseColor);
    }

    public void SetHoverColor()
    {
        _tileMaterial.SetColor("_Color", HoverColor);
    }

    public void SetBaseColor()
    {
        _tileMaterial.SetColor("_Color", BaseColor);
    }
}
