using UnityEngine;

public class CloudScroll : MonoBehaviour
{
    private Material _material;
    void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float offsetX = Time.time * .002f;
        float offsetY = Time.time * .004f;
        _material.SetTextureOffset("_BaseMap", new Vector2(offsetX, offsetY));
    }
}
