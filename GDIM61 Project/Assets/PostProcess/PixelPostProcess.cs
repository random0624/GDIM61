using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class PixelPostProcess : MonoBehaviour
{
    public Material pixelMaterial;

    [Range(60, 720)]
    public int pixelHeight = 180;

    [Range(0f, 3f)]
    public float saturation = 1.5f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        float blockCountY = pixelHeight;
        float blockSizeY = 1.0f / blockCountY;

        float blockCountX = blockCountY * source.width / source.height;
        float blockSizeX = 1.0f / blockCountX;

        pixelMaterial.SetVector("_BlockCount", new Vector2(blockCountX, blockCountY));
        pixelMaterial.SetVector("_BlockSize", new Vector2(blockSizeX, blockSizeY));
        pixelMaterial.SetVector("_HalfBlockSize", new Vector2(blockSizeX, blockSizeY) * 0.5f);
        pixelMaterial.SetFloat("_Saturation", saturation);

        Graphics.Blit(source, destination, pixelMaterial);
    }
}