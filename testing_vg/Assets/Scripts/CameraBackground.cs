using UnityEngine;

public class CameraBackground : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Obtém o componente SpriteRenderer do objeto
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Obtém a primeira câmera disponível
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // Cria o WebCamTexture com a primeira câmera disponível
            webCamTexture = new WebCamTexture(devices[0].name);

            // Define a textura da webcam no SpriteRenderer
            spriteRenderer.material.mainTexture = webCamTexture;

            // Inicia o feed da webcam
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("Nenhuma webcam encontrada!");
        }
    }

    void Update()
    {
        // Verifica se o WebCamTexture ainda está ativo
        if (webCamTexture != null && !webCamTexture.isPlaying)
        {
            Debug.LogError("A webcam não está funcionando corretamente.");
        }
    }

    void OnApplicationQuit()
    {
        // Para o feed da webcam quando a aplicação for fechada
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
