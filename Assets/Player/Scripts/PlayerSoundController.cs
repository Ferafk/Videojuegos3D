using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundController : MonoBehaviour
{
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip shootSound;
    public AudioClip eatSound;
    public AudioClip footstepLeftSound;
    public AudioClip footstepRightSound;

    private AudioSource audioSource;
    private bool isLeftStep = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayLanding()
    {
        audioSource.PlayOneShot(landingSound);
    }

    public void PlayShootSound()
    {
        audioSource.PlayOneShot(shootSound);
    }

    public void PlayEatSound()
    {
        audioSource.PlayOneShot(eatSound);
    }

    public void PlayFootstepSound()
    {
        // Intercalamos los sonidos de pasos
        AudioClip footstep = isLeftStep ? footstepLeftSound : footstepRightSound;
        audioSource.PlayOneShot(footstep);
        isLeftStep = !isLeftStep;  // Cambia entre sonido izquierdo y derecho
    }
}
