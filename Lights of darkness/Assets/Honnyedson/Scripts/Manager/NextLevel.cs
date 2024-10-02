using System;
using UnityEngine;

public class PhaseTrigger : MonoBehaviour
{
    public PhaseManager phaseManager; 

    private void Update()
    {
        phaseManager = FindObjectOfType<PhaseManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            phaseManager.TriggerNextPhase(); 
        }
    }
}