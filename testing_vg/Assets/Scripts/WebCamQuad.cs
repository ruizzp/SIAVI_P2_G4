using UnityEngine;

public class WebCamQuad : MonoBehaviour
{
    WebCamTexture webcamTexture;

    void Start()
    {
        float height = Camera.main.orthographicSize * 2f;
        float width = height * Camera.main.aspect;
        transform.localScale = new Vector3(width, height, 1f);

        Renderer renderer = GetComponent<Renderer>();

        webcamTexture = new WebCamTexture();
        renderer.material.mainTexture = webcamTexture;

        webcamTexture.Play();
    }

    void OnApplicationQuit()
    {
        if (webcamTexture != null)
            webcamTexture.Stop();
    }
}