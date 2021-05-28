using UnityEngine;

public class ZoneInfo : MonoBehaviour
{
    public AudioClip ambienceZoneAudioClip;
    public bool haveFog;

    public ZoneInfo Initialize(ZoneInfo zoneInfo)
    {
        ambienceZoneAudioClip = zoneInfo.ambienceZoneAudioClip;
        haveFog = zoneInfo.haveFog;
        return this;
    }
}
