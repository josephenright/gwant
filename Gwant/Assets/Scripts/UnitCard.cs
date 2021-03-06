﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitCard : Card {

    public enum Sections { Agile, Melee, Ranged, Siege, COUNT }

    Sections section;
    bool hero;
    Abilities ability;

    //x = total strength
    //y = base strength
    Vector2Int strength;
    int morale;
    public bool AbilityUsed = false;

    string muster;
    int scorchThreshold;
    int avenger;

    int bondCount;
    bool horn;

    public UnitCard(int ID, string Name, string Art, Sections Section, int Strength,
        bool Hero, Abilities Ability, int Avenger, string Muster, int ScorchThreshold) : base(ID, Name, Art, false)
    {
        this.Section = Section;
        this.Hero = Hero;
        this.Ability = Ability;
        strength = new Vector2Int();
        SetBaseStrength(Strength);
        //if (Ability == Abilities.Morale)
            //SetBaseMorale(1);
        //else
            //SetBaseMorale(0);
        if (Ability == Abilities.Muster)
            this.Muster = Muster;
        if (Ability == Abilities.Avenger)
            this.Avenger = Avenger;
        if (Ability == Abilities.Scorch)
            this.ScorchThreshold = ScorchThreshold;

    }


    public override void ApplyEffects(Zone zone)
    {
        //Stuff

        //CalcStats(zone);
    }

    public void CalcStats(Battlefield bf, CardGO cardGO, bool apply = true)
    {
        if (!apply) {
            Strength = GetBaseStrength();
            ClearBonds();
            Morale = 0;
            Horn = false;
            cardGO.GetComponent<CardGO>().UpdateStrengthText();
        }
        else if (!Hero)
        {
            //1. apply Weather effects
            if (bf.Weather)
            {
                Strength = 1;
            }
            else
                Strength = GetBaseStrength();
            //2. apply Bond effects
            if (Ability == Abilities.Bond)
            {
                ClearBonds();
                int index = bf.Cards.IndexOf(cardGO);
                int tempindex = -1;
                while (index + tempindex >= 0 &&
                    bf.Cards[index + tempindex].GetComponent<CardGO>().Card.Name == cardGO.Card.Name)
                {
                    AddBond();
                    tempindex--;
                }

                tempindex = 1;
                while (index + tempindex <= bf.Cards.Count - 1 &&
                    bf.Cards[index + tempindex].GetComponent<CardGO>().Card.Name == cardGO.Card.Name)
                {
                    AddBond();
                    tempindex++;
                }

                for (int i = 0; i < Bond; i++)
                {
                    Strength += GetBaseStrength();
                }
            }
            //3. apply Morale effects
            if (bf.Morale > 0 && Ability != Abilities.Morale)
            {
                Morale = bf.Morale;
                Strength += Morale;
            }
            else if (bf.Morale > 1 && Ability == Abilities.Morale)
            {
                Morale = bf.Morale;
                Strength += Morale - 1;
            }
            //4.1 apply Horn effect from Horn unit cards
            Horn = false;
            if (bf.ZoneHorn.HasHorn)
            {
                HornZone hz = bf.ZoneHorn;
                if (hz.SpecialHorn != null)
                {
                    Strength += Strength;
                    Horn = true;
                }
                //check to see this card is not doubling itself
                else if ((Ability == Abilities.Horn && hz.UnitHorns.Count > 1) ||
                    (Ability != Abilities.Horn && hz.UnitHorns.Count > 0))
                {
                    Strength += Strength;
                    Horn = true;
                }

            }
            //update Strength text
            cardGO.GetComponent<CardGO>().UpdateStrengthText();
        }
    }

    public void TriggerAvengerAbility()
    {

    }

    public int Strength { get { return strength.x; } set { strength.x = value; } }
    public int GetBaseStrength() { return strength.y; }
    public int Morale { get { return morale; } set { morale = value; } }
    public int ScorchThreshold { get { return scorchThreshold; } private set { scorchThreshold = value; } }
    public int Avenger { get { return avenger; } private set { avenger = value; } }

    private void SetBaseStrength(int Strength)
    {
        this.Strength = Strength;
        strength.y = Strength;
    }
    //private void SetBaseMorale(int Morale) { this.Morale = Morale; }

    public bool Hero { get { return hero; } private set { hero = value; } }
    public int Bond { get { return bondCount; }  }
    public void AddBond() { bondCount++; }
    public void ClearBonds() { bondCount = 0; }
    public bool Horn { get { return horn; } set { horn = value; } }
    public Sections Section { get { return section; } private set { section = value; } }
    public string Muster
    {
        get
        {
            if (Ability == Abilities.Muster)
                return muster;
            else
                return null;
        }
        private set { muster = value; }
    }
    public override Abilities Ability
    {
        get
        {
            return ability;
            /*
            if (AbilityIsValid(ability))
                return ability;
            else
                return Abilities.None;
            */
        }
        protected set //Must be protected because it's an override
        {
            if (AbilityIsValid(value))
                ability = value;
            else
                ability = Abilities.None;
        }
    }

    protected override bool AbilityIsValid(Abilities ab)
    {
        if ((int)ab < UNIT_END)
            return true;
        return false;
    }

    public Zone.Types GetZone
    {
        get
        {
            if (Section == Sections.Melee)
                return Zone.Types.Melee;
            else if (Section == Sections.Ranged)
                return Zone.Types.Ranged;
            else if (Section == Sections.Siege)
                return Zone.Types.Siege;
            else
                return Zone.Types.NOT_SET_YET;
        }
    }
    
}
