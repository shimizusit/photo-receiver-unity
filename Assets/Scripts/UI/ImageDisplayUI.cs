using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ImageDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RawImage displayImage;
    [SerializeField] private TextMeshProUGUI timestampText;
    [SerializeField] private TextMeshProUGUI processingTimeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private ParticleSystem updateEffect;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private WebSocketClient webSocketClient;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = displayImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = displayImage.gameObject.AddComponent<CanvasGroup>();
        }

        webSocketClient = FindObjectOfType<WebSocketClient>();
        if (webSocketClient != null)
        {
            webSocketClient.OnMessageReceived += HandleNewImage;
            webSocketClient.OnConnected += HandleConnected;
            webSocketClient.OnDisconnected += HandleDisconnected;
        }
    }

    private async void HandleNewImage(WebSocketMessage message)
    {
        try
        {
            // Base64デコード
            byte[] imageData = Convert.FromBase64String(message.imageData);
            
            // テクスチャの作成
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(imageData);
            
            // フェードアニメーション付きで画像を更新
            await ImageUtils.FadeImage(displayImage, texture, fadeDuration, fadeCurve);
            
            // メタデータの更新
            UpdateMetadata(message);
            
            // エフェクトの再生
            if (updateEffect != null)
            {
                updateEffect.Play();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Image processing error: {ex.Message}");
        }
    }

    private void UpdateMetadata(WebSocketMessage message)
    {
        if (timestampText != null)
        {
            DateTime timestamp = DateTime.Parse(message.timestamp);
            timestampText.text = timestamp.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
        }

        if (processingTimeText != null)
        {
            processingTimeText.text = $"処理時間: {message.metadata.processingTime:F1}ms";
        }

        if (descriptionText != null && !string.IsNullOrEmpty(message.metadata.description))
        {
            descriptionText.text = message.metadata.description;
        }
    }

    private void HandleConnected()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void HandleDisconnected()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (webSocketClient != null)
        {
            webSocketClient.OnMessageReceived -= HandleNewImage;
            webSocketClient.OnConnected -= HandleConnected;
            webSocketClient.OnDisconnected -= HandleDisconnected;
        }
    }
}
