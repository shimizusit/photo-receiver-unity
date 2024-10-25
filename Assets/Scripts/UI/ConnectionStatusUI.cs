using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectionStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusIcon;
    [SerializeField] private Color connectedColor = Color.green;
    [SerializeField] private Color disconnectedColor = Color.red;
    [SerializeField] private GameObject reconnectingPanel;

    private WebSocketClient webSocketClient;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        webSocketClient = FindObjectOfType<WebSocketClient>();

        if (webSocketClient != null)
        {
            webSocketClient.OnConnected += HandleConnected;
            webSocketClient.OnDisconnected += HandleDisconnected;
            webSocketClient.OnError += HandleError;
        }
    }

    private void HandleConnected()
    {
        statusText.text = "接続済み";
        statusIcon.color = connectedColor;
        if (reconnectingPanel != null)
        {
            reconnectingPanel.SetActive(false);
        }
        if (animator != null)
        {
            animator.SetTrigger("Connected");
        }
    }

    private void HandleDisconnected()
    {
        statusText.text = "未接続";
        statusIcon.color = disconnectedColor;
        if (reconnectingPanel != null)
        {
            reconnectingPanel.SetActive(true);
        }
        if (animator != null)
        {
            animator.SetTrigger("Disconnected");
        }
    }

    private void HandleError(string error)
    {
        statusText.text = $"エラー: {error}";
        statusIcon.color = disconnectedColor;
        if (animator != null)
        {
            animator.SetTrigger("Error");
        }
    }

    private void OnDestroy()
    {
        if (webSocketClient != null)
        {
            webSocketClient.OnConnected -= HandleConnected;
            webSocketClient.OnDisconnected -= HandleDisconnected;
            webSocketClient.OnError -= HandleError;
        }
    }
}
