using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        CreateInstance();
    }

    public EventInstance CreateSpatializedInstance(EventReference eventRef, Transform audioParent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);
        return eventInstance;
    }

    public EventInstance PlayOneShotSpatialized(EventReference eventRef, Transform audioParent)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioParent);
        eventInstance.start();
        eventInstance.release();
        return eventInstance;
    }

    public EventInstance PlayOneShot(EventReference eventRef)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        eventInstance.start();
        eventInstance.release();
        return eventInstance;
    }

    public float EventLength(EventReference eventRef)
    {
        EventDescription eventDescription = RuntimeManager.GetEventDescription(eventRef);
        eventDescription.getLength(out int lengthMiliseconds);
        float length = (float)lengthMiliseconds / 1000;
        Debug.Log("EventLength: " + length);
        return length;
    }

    public void SetGameVolume(float givenVolume)
    {
        SetBusVolume(FmodBuses.Master, givenVolume);
    }

    public float GetGameVolume()
    {
        FmodBuses.Master.getVolume(out float volume);
        return volume;
    }

    public void SetInstanceVolume(EventInstance eventInstance, float volume)
    {
        volume = Mathf.Clamp01(volume);
        eventInstance.setVolume(volume);
    }

    public void SetBusVolume(Bus bus, float volume)
    {
        volume = Mathf.Clamp01(volume);
        bus.setVolume(volume);
    }

    public IEnumerator FadeOutBus(Bus bus, float fadeSpeed)
    {
        bus.getVolume(out float volume);
        while (volume > 0)
        {
            SetBusVolume(bus, volume - fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }

        bus.setPaused(true);
    }

    public IEnumerator FadeInBus(Bus bus, float fadeSpeed)
    {
        bus.setPaused(false);

        bus.getVolume(out float volume);
        while (volume < 1)
        {
            SetBusVolume(bus, volume + fadeSpeed);
            yield return new WaitForSecondsRealtime(0);
            bus.getVolume(out volume);
        }
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene.");
        }
        Instance = this;
    }
}

public class FmodBuses
{
    public static Bus Master => RuntimeManager.GetBus("bus:/");
}