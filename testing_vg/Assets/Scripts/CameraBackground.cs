using UnityEngine;

public class CameraBackground : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Obt�m o componente SpriteRenderer do objeto
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Obt�m a primeira c�mera dispon�vel
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // Cria o WebCamTexture com a primeira c�mera dispon�vel
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
        // Verifica se o WebCamTexture ainda est� ativo
        if (webCamTexture != null && !webCamTexture.isPlaying)
        {
            Debug.LogError("A webcam n�o est� funcionando corretamente.");
        }
    }

    void OnApplicationQuit()
    {
        // Para o feed da webcam quando a aplica��o for fechada
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
