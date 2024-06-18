using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderChanger : MonoBehaviour
{

    public Material defaultMaterial;
    public Material grabbedMaterial;

    // Reference to the renderer component
    private Renderer objectRenderer;

    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null && defaultMaterial != null) {
            objectRenderer.material = defaultMaterial; 
        }
    }

    public void OnGrab() {
        if (objectRenderer != null && grabbedMaterial != null) { 
            objectRenderer.material = grabbedMaterial;
        }
    }

    public void OnRelease()
    {
        if (objectRenderer != null && defaultMaterial != null) {
            objectRenderer.material = defaultMaterial;
        }
    }
}
