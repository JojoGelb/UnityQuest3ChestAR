using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scale : MonoBehaviour
{

    private float normalScale = 1f;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeScale()
    {
        float scale = slider.value;
        this.gameObject.transform.localScale = new Vector3(normalScale * scale, normalScale * scale, normalScale * scale);
    }
}
