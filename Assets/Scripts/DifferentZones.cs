using UnityEngine;

public class DifferentZones : MonoBehaviour
{
    public AudioSource audioSource;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Zone")) return;

        // settings block
        var zoneInfo = other.GetComponent<ZoneInfo>();
        
        // sound
        var audioClp = zoneInfo.ambienceZoneAudioClip;
 
        if (audioSource.clip != audioClp)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioClp);
        }
            
        // fog
        RenderSettings.fog = zoneInfo.haveFog;

        // 👽
        Debug.Log("You're in area!51!");
    }
}
