using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerDodgeUI : NetworkBehaviour
{
    [SerializeField] RectTransform barTransform;
    PlayerMovement playerMovement;

    public override void Spawned()
    {

    }

    private void Update()
    {
        if (playerMovement == null)
        {
            NetworkObject localPlayer = Runner.GetPlayerObject(Runner.LocalPlayer);
            if (localPlayer != null)
            {
                if (transform.IsChildOf(localPlayer.transform))
                {
                    playerMovement = GetComponentInParent<PlayerMovement>();
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            float xScale = (playerMovement.MaxDodgeCooldown - playerMovement.CurrentDodgeCooldown) / playerMovement.MaxDodgeCooldown;
            barTransform.localScale = new Vector3(xScale, 1);
        }
    }
}
