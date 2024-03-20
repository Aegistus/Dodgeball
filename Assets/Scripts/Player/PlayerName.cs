using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] TMP_Text nameText;
    [Networked] string Name { get; set; }

    ChangeDetector nameChangeDetector;

    public override void Spawned()
    {
        nameChangeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in nameChangeDetector.DetectChanges(this))
        {
            if (change == nameof(Name))
            {
                UpdateName();
            }
        }
    }

    public void SetName(string name)
    {
        Name = name;
    }

    void UpdateName()
    {
        nameText.text = Name;
    }
}
