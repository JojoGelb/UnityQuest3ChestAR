using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCreator : MonoBehaviour
{
    public GameObject canvasPrefab;

    // Start is called before the first frame update
    void Start()
    {
        CreateCanvasWithImage(PieceType.None);
    }

    public void CreateCanvasWithImage(PieceType pieceTypeSelected) { 

        GameObject canvasInstance = Instantiate(canvasPrefab);
        ImageSelector imageSelector = canvasInstance.GetComponent<ImageSelector>();

        if (imageSelector != null) {
            imageSelector.SetImage(pieceTypeSelected);
        }
    }
}
