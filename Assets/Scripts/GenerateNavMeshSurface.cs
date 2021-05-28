using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenerateNavMeshSurface : MonoBehaviour
{
    public List<Generator> bioms;
    public GameObject player;
    
    // (μ_μ) 
    // is to be able to walk between two biomes
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            
            bool allFinished = true;
            foreach (var biom in bioms)
            {
                if (!biom.isGenerated)
                    allFinished = false;
            }
            
            if(allFinished)
                break;
        }
        
        gameObject.AddComponent<NavMeshSurface>().BuildNavMesh();
        
        player.SetActive(true);
        
        yield return null;
    }
}
