using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] TMP_Text nameText;
    [Networked] public string Name { get; set; }

    ChangeDetector nameChangeDetector;
    bool updated = false;

    public override void Spawned()
    {
        nameChangeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        Team team = GetComponent<Team>();
        if (team.LocalPlayer)
        {
            NetworkManager netManager = FindObjectOfType<NetworkManager>();
            SetName(netManager.LocalPlayerName);
        }
        nameText.text = Name;
    }

    public override void Render()
    {
        foreach (var change in nameChangeDetector.DetectChanges(this))
        {
            if (change == nameof(Name))
            {
                nameText.text = Name;
            }
        }
    }

    public void SetName(string name)
    {
        Name = name;
        nameText.text = Name;
    }
}
