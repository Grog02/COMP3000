using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.LowLevel;
using TMPro;
using UnityEditor.Rendering;

public class Audio : MonoBehaviour
{
    
    public static Audio instance;

    [SerializeField] AudioSource music;
    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }    
    private void Start() 
    {
        music.Play();    
    }
    
    
}
