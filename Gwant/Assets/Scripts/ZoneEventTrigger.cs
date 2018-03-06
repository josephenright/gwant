﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ZoneEventTrigger : MonoBehaviour, IPointerClickHandler {
    
    static Color32 defaultColour = new Color32(255, 255, 255, 255);
    static Color32 highlightColour = new Color32(0, 128, 255, 255);

    public Color32 DefaultColour { get { return defaultColour; } set { defaultColour = value; } }
    public Color32 HighlightColour { get { return highlightColour; } set { highlightColour = value; } }

    public Image image;
    bool highlighted;
    public bool Highlighted { get { return highlighted; }
        set
        {
            if (value)
                image.color = HighlightColour;
            else
                image.color = DefaultColour;
            highlighted = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Highlighted)
        {
            print("CLICK: " + gameObject + " CARD: " + CardEventTrigger.SelectedCard);
            //play selected card
            CardGO cardGO = CardEventTrigger.SelectedCard.GetComponent<CardGO>();
            cardGO.MoveTo(GetComponent<Zone>());
            Manager.manager.UnHighlightZones();
            Zone z = GetComponent<Zone>();
            if (z.Type == Zone.Types.Melee || z.Type == Zone.Types.Ranged ||
                z.Type == Zone.Types.Siege)
                ((Battlefield)z).CalcStats();
            if (z.Type == Zone.Types.Horn)
            {
                ((HornZone)z).SpecialHorn = cardGO;
                //recalc stats for associated battlefield
                //z.Battlefield.CalcStats();
            }
            CardEventTrigger.Deselect(CardEventTrigger.SelectedCard);
        }
    }

    void Start()
    {
        if (image == null)
            image = GetComponent<Image>();
        Highlighted = false;
    }

    private void Reset()
    {
        image = GetComponent<Image>();
        Highlighted = false;
    }
}
