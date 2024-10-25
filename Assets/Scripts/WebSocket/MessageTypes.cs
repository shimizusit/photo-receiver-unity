using System;

[Serializable]
public class WebSocketMessage
{
    public string imageData;  // base64エンコードされた画像データ
    public string timestamp;  // ISO 8601形式のタイムスタンプ
    public MessageMetadata metadata;
}

[Serializable]
public class MessageMetadata
{
    public string description;
    public float processingTime;
    public string originalFilename;
}
