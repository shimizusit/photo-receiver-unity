using UnityEngine;
using NativeWebSocket;
using System;
using System.Threading.Tasks;

public class WebSocketClient : MonoBehaviour
{
    public string wsUrl = "ws://localhost:8000/ws";
    public float reconnectInterval = 3f;

    private WebSocket websocket;
    private bool isConnecting = false;
    private float reconnectTimer = 0f;

    public event Action<WebSocketMessage> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string> OnError;

    private void Start()
    {
        ConnectToServer();
    }

    private async void ConnectToServer()
    {
        if (isConnecting) return;
        isConnecting = true;

        try
        {
            websocket = new WebSocket(wsUrl);

            websocket.OnOpen += () =>
            {
                Debug.Log("WebSocket Connected");
                isConnecting = false;
                OnConnected?.Invoke();
            };

            websocket.OnClose += (e) =>
            {
                Debug.Log("WebSocket Closed");
                isConnecting = false;
                OnDisconnected?.Invoke();
            };

            websocket.OnError += (e) =>
            {
                Debug.LogError($"WebSocket Error: {e}");
                isConnecting = false;
                OnError?.Invoke(e);
            };

            websocket.OnMessage += (bytes) =>
            {
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                try
                {
                    var wsMessage = JsonUtility.FromJson<WebSocketMessage>(message);
                    OnMessageReceived?.Invoke(wsMessage);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Message parsing error: {ex.Message}");
                    OnError?.Invoke("メッセージの解析に失敗しました");
                }
            };

            await websocket.Connect();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Connection error: {ex.Message}");
            isConnecting = false;
            OnError?.Invoke("サーバーへの接続に失敗しました");
        }
    }

    private void Update()
    {
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();

            // 切断時の再接続処理
            if (websocket.State == WebSocketState.Closed)
            {
                reconnectTimer += Time.deltaTime;
                if (reconnectTimer >= reconnectInterval)
                {
                    reconnectTimer = 0f;
                    ConnectToServer();
                }
            }
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    private void OnDestroy()
    {
        if (websocket != null)
        {
            websocket.CancelConnection();
        }
    }
}
