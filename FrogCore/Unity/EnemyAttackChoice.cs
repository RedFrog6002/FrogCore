using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FrogCore.Unity;

[Serializable]
public class EnemyAttackChoice
{
    public List<AttackEntry> entries = new List<AttackEntry>();

    public System.Random RNG = null;

    [Serializable]
    public class AttackEntry
    {
        public AttackEntry(string name, Func<IEnumerator> generator, float chance, int maxMisses = -1, int maxRepeats = -1) 
        {
            this.name = name;
            this.generator = generator;
            this.chance = chance;
            this.maxMisses = maxMisses;
            this.maxRepeats = maxRepeats;
        }

        public AttackEntry(string name, float chance, int maxMisses = -1, int maxRepeats = -1) 
        {
            this.name = name;
            this.chance = chance;
            this.maxMisses = maxMisses;
            this.maxRepeats = maxRepeats;
        }

        public AttackEntry()
        {
            name = "";
            chance = 0f;
            maxMisses = -1;
            maxRepeats = -1;
        }

        public IEnumerator GenerateAttack() => generator();

        public float GetChance() => chance;

        public bool MustUse() => (maxMisses >= 0) && (curMisses >= maxMisses);

        public bool CanUse() => (maxRepeats < 0) || (curRepeats < maxRepeats);

        public void UseAttack()
        {
            curMisses = 0;
            curRepeats++;
        }

        public void MissAttack()
        {
            curMisses++;
            curRepeats = 0;
        }

        public string name;

        public Func<IEnumerator> generator;
        public float chance;
        public int maxMisses;
        public int maxRepeats;

        private int curMisses = 0;
        private int curRepeats = 0;
    }

    public void AddEntries(params AttackEntry[] entries)
    {
        foreach (AttackEntry entry in entries)
            AddEntry(entry);
    }

    public void AddEntries(params (string name, float chance)[] entries)
    {
        foreach ((string name, float chance) in entries)
            AddEntry(new AttackEntry(name, chance));
    }

    public void AddEntry(AttackEntry entry) => entries.Add(entry);

    public EnemyAttackChoice(AttackEntry entry) => AddEntry(entry);
    public EnemyAttackChoice(params AttackEntry[] entries) => AddEntries(entries);
    public EnemyAttackChoice(params (string name, float chance)[] entries) => AddEntries(entries);

    public void SetupAttacks(params (string name, Func<IEnumerator> gen)[] generators)
    {
        for (int i = 0; i < generators.Length; i++)
            foreach (AttackEntry entry in entries)
                if (entry.name == generators[i].name)
                    entry.generator = generators[i].gen;
    }

    public IEnumerator DoRandomAttack()
    {
        if (entries.Count <= 0)
            yield break;

        IEnumerable<AttackEntry> currentEntries = entries.Where(entry => entry.MustUse() && entry.CanUse());

        if (currentEntries.Count() <= 0)
            currentEntries = entries.Where(entry => entry.MustUse());

        if (currentEntries.Count() <= 0)
            currentEntries = entries.Where(entry => entry.CanUse());

        if (currentEntries.Count() <= 0)
            currentEntries = entries;

        float fullChance = currentEntries.Select(entry => entry.GetChance()).Sum();
        float randNum;
        if (RNG == null)
            randNum = UnityEngine.Random.Range(0f, fullChance);
        else
            randNum = (float)RNG.NextDouble() * fullChance;

        IEnumerator<AttackEntry> enumerator = currentEntries.GetEnumerator();

        AttackEntry selectedEntry = null;
        float currentChance = 0f;
        while (enumerator.MoveNext())
        {
            AttackEntry entry = enumerator.Current;

            currentChance += entry.GetChance();
            if (currentChance >= randNum)
            {
                selectedEntry = entry;
                break;
            }
        }

        if (selectedEntry is null)
            selectedEntry = entries[0];
        
        foreach (AttackEntry entry in entries)
        {
            if (entry == selectedEntry)
            {
                entry.UseAttack();
            }
            else
            {
                entry.MissAttack();
            }
        }

        yield return selectedEntry.GenerateAttack();
    }
}