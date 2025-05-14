using Assets.Scripts;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GestureCursorController : MonoBehaviour
{
    public RectTransform cursorUI;         // O cursor a mover
    public Canvas canvas;                  // O canvas do UI
    public GraphicRaycaster raycaster;     // Para clique
    public EventSystem eventSystem;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    TcpListener listener;
    Thread listenerThread;

    private Vector2 handPos = new Vector2(0.5f, 0.5f);
    private bool click = false;
    private bool prevClick = false;

    void Start()
    {
        listenerThread = new Thread(ConnectToPython);
        listenerThread.IsBackground = true;
        listenerThread.Start();
        // ConnectToPython();
        Canvas cursorCanvas = cursorUI.GetComponentInParent<Canvas>();
        cursorCanvas.overrideSorting = true;
        cursorCanvas.sortingOrder = 9999;
    }

    void Update()
    {
        MoveCursor();

        if (click && !prevClick)
        {
            SimulateClick();
        }

        prevClick = click;
    }
    void ConnectToPython()
    {

        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Debug.Log("Esperando conexão do Python Para Gestos...");
        while (true)
        {
            using (TcpClient client = listener.AcceptTcpClient())
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string json = Encoding.UTF8.GetString(buffer, 0, bytes);

                GestureData data = JsonUtility.FromJson<GestureData>(json);
                handPos = new Vector2(data.x, data.y);
                click = data.gesture == "pinça";
            }
        }
    }

    void MoveCursor()
    {
        Vector2 screenPos = new Vector2(handPos.x * Screen.width, (1 - handPos.y) * Screen.height);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        cursorUI.localPosition = localPoint;
    }

    public void SimulateClick()
    {
        StartCoroutine(ClickWithVisuals());
    }

    private IEnumerator ClickWithVisuals()
    {
        PointerEventData pointer = new PointerEventData(eventSystem)
        {
            pointerId = 0,
            position = cursorUI.position,
            button = PointerEventData.InputButton.Left,
            clickCount = 1
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);
        //raycaster.Raycast(pointer, results);

        if (results.Count == 0)
            yield break;

        foreach (var result in results)
        {
            GameObject target = result.gameObject;

            // Simula hover e pressionar
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerDownHandler);

            // Espera um curto tempo para mostrar visualmente o "pressionado"
            yield return new WaitForSeconds(0.1f);

            // Solta o botão (visual de soltar)
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerUpHandler);
            // Executa o clique
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);
            cursorUI.SetAsLastSibling();

            ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerExitHandler);
        }

        Debug.Log("🖱️ Clique simulado com efeitos visuais e delay!");
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();
    }
}
