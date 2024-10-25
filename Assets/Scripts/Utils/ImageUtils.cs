using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public static class ImageUtils
{
    public static async Task FadeImage(RawImage image, Texture newTexture, float duration, AnimationCurve curve)
    {
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = image.gameObject.AddComponent<CanvasGroup>();
        }

        // 現在の画像をフェードアウト
        float elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / (duration / 2);
            canvasGroup.alpha = 1 - curve.Evaluate(normalizedTime);
            await Task.Yield();
        }

        // テクスチャを更新
        image.texture = newTexture;

        // 新しい画像をフェードイン
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / (duration / 2);
            canvasGroup.alpha = curve.Evaluate(normalizedTime);
            await Task.Yield();
        }

        canvasGroup.alpha = 1f;
    }

    public static void ApplyAspectRatioFitter(RawImage image)
    {
        AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter>();
        if (fitter == null)
        {
            fitter = image.gameObject.AddComponent<AspectRatioFitter>();
        }

        if (image.texture != null)
        {
            float aspectRatio = (float)image.texture.width / image.texture.height;
            fitter.aspectRatio = aspectRatio;
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        }
    }
}
