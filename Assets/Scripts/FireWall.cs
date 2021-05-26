using UnityEngine;

public class FireWall : MonoBehaviour
{
    public Material lvl1;
    public Material lvl2;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyBehavior>().Damage(100);   
        }
    }
}
