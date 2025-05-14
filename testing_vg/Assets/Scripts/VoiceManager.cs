using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    TcpListener listener;
    Thread listenerThread;
    string receivedCommand = "";

    public TextMeshProUGUI language;

    public TextMeshProUGUI label;

    void Start()
    {
        listenerThread = new Thread(StartListening);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void StartListening()
    {
        listener = new TcpListener(IPAddress.Any, 5005);
        listener.Start();
        Debug.Log("Esperando conexão do Python...");

        while (true)
        {
            using (TcpClient client = listener.AcceptTcpClient())
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int len = stream.Read(buffer, 0, buffer.Length);
                receivedCommand = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("Comando recebido: " + receivedCommand);
            }
        }
    }


    public void ChangeLanguage()
    {
        
        string idioma = "";
        if (language.text.Equals("Português"))
            idioma = "pt";
        else if (language.text.Equals("English"))
            idioma = "en";

        TcpClient client = new TcpClient("localhost", 5050);
        NetworkStream stream = client.GetStream();
        byte[] msg = Encoding.UTF8.GetBytes(idioma);
        stream.Write(msg, 0, msg.Length);
        stream.Close();
        client.Close();
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(receivedCommand))
        {
            label.text = receivedCommand;
            /*
            // Aqui você reage ao comando
            if (receivedCommand == "cima")
                transform.Translate(Vector3.up * Time.deltaTime * 5);
            else if (receivedCommand == "baixo")
                transform.Translate(Vector3.down * Time.deltaTime * 5);

            receivedCommand = ""; // limpa para o próximo comando*/
        }
    }

    void OnApplicationQuit()
    {
        listener.Stop();
        listenerThread.Abort();
    }
}
