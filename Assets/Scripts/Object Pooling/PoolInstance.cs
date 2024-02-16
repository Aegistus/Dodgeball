using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An instance of a pool object at runtime
/// </summary>
public class PoolInstance : MonoBehaviour
{
    public float lifeTime;

    private ParticleSystem particles;
    private AudioSource audioSource;
    private bool justSpawned = true;

    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        if (!justSpawned)
        {
            if (particles)
            {
                particles.Play();
            }
            if (audioSource)
            {
                audioSource.Play();
            }
            if (lifeTime > 0)
            {
                StartCoroutine(EndLifeTime());
            }
        }
        else
        {
            justSpawned = false;
        }
    }

    private IEnumerator EndLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);
    }
}
