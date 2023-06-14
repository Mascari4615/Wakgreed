using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = (float)Screen.width / Screen.height / ((float)16 / 9); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }

        camera.rect = rect;
    }

    private void OnPreCull()
    {
        GL.Clear(true, true, Color.black);
    }
}