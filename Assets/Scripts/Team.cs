using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Team : NetworkBehaviour
{
    [HideInInspector][Networked] public int TeamIndex { get; private set; } = 0;

    Renderer[] rends;

    public void SetTeam(int teamIndex)
    {
        if (HasStateAuthority)
        {
            TeamIndex = teamIndex;
            rends = GetComponentsInChildren<Renderer>();
            Material teamMat = TeamManager.GetTeamColor(TeamIndex);
            if (teamMat != null)
            {
                for (int i = 0; i < rends.Length; i++)
                {
                    rends[i].sharedMaterial = teamMat;
                }
            }

        }
    }


}
