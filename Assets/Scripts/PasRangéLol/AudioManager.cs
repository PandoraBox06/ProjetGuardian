using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    public static AudioManager Instance { get; private set; }

    [field: Header("Avatar")]
    [field: SerializeField] public EventReference walk { get; private set; }
    [field: SerializeField] public EventReference sprint { get; private set; }
    [field: SerializeField] public EventReference dash { get; private set; }
    [field: SerializeField] public EventReference getHit { get; private set; }
    [field: SerializeField] public EventReference death { get; private set; }
    [field: SerializeField] public EventReference doHit { get; private set; }
    [field: Header("Weapons")]
    [field: SerializeField] public EventReference hitSword { get; private set; }
    [field: SerializeField] public EventReference draw { get; private set; }
    [field: SerializeField] public EventReference hitCrossbow { get; private set; }
    [field: SerializeField] public EventReference hitCrossbowWall { get; private set; }
    [field: Header("Ennemis")]
    [field: SerializeField] public EventReference walkEnemy { get; private set; }
    [field: SerializeField] public EventReference getHitEnemy { get; private set; }
    [field: SerializeField] public EventReference attackEnemy { get; private set; }
    [field: SerializeField] public EventReference attackRangeEnemy { get; private set; }
    [field: SerializeField] public EventReference guardEnemy { get; private set; }
    [field: SerializeField] public EventReference deathEnemy { get; private set; }
    [field: Header("UI")]
    [field: SerializeField] public EventReference cursorValidate { get; private set; }
    [field: SerializeField] public EventReference cursorBack { get; private set; }
    [field: SerializeField] public EventReference cursorSelection { get; private set; }
    [field: SerializeField] public EventReference gameOver { get; private set; }
    
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene");
        }
        Instance = this;
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        //Stop and release any created instances
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        //Stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
