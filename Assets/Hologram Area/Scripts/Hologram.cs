using UnityEngine;
using System.Collections;
using System;

public class Hologram : MonoBehaviour 
{
    public Camera[] cameras;

    public float hologramArea = 1f;

    public enum Direction
    {
        topDown, bottomUp
    };

    public Direction type = new Direction();

    void OnValidate()
    {
        //These methods only get called when an inspector value for the script has been modified.
        Scale();
        CameraClipping();
        CameraRotation();
        CameraRect(GetMainGameViewSize());
    }

    void Start()
    {
        CameraRect(new Vector2(Screen.width, Screen.height));
        StartCoroutine(FadeViewport());
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Scale()
    {
        transform.localScale = new Vector3(hologramArea, hologramArea, hologramArea);
    }

    void CameraClipping()
    {
        float centerToEdge = hologramArea / 2f;
        float EdgeToCamera = centerToEdge / (float)(Math.Tan(30 * (Math.PI / 180)));

        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].farClipPlane = EdgeToCamera + hologramArea;
            cameras[i].nearClipPlane = EdgeToCamera;
        }
    }

    void CameraRotation()
    {
        if (type == Direction.topDown)
        {
            cameras[1].transform.eulerAngles = new Vector3(0, 0, 0);
            cameras[2].transform.eulerAngles = new Vector3(0, 180, 180);
        }
        else if (type == Direction.bottomUp)
        {
            cameras[1].transform.eulerAngles = new Vector3(0, 0, 180);
            cameras[2].transform.eulerAngles = new Vector3(0, 180, 0);
        }

        cameras[3].transform.eulerAngles = new Vector3(0, 90, 90);
        cameras[4].transform.eulerAngles = new Vector3(0, 270, 270);
    }

    void CameraRect(Vector2 res)
    {
        float aspectRatio = res.y / res.x;

        float cameraSizeX = 1 / 3f;
        float cameraCenterX = 1 / 2f * cameraSizeX;

        float cameraSizeY = 1 / 3f;
        float cameraCenterY = 1 / 2f * cameraSizeY;

        if (aspectRatio < 1)
        {
            cameraSizeX = cameraSizeX * aspectRatio;
            cameraCenterX = cameraCenterX * aspectRatio;
        }
        else if (aspectRatio > 1)
        {
            cameraSizeY = cameraSizeY / aspectRatio;
            cameraCenterY = cameraCenterY / aspectRatio;
        }

        cameras[1].rect = new Rect(1 / 2f - cameraCenterX, 1 / 2f - (3 * cameraCenterY), cameraSizeX, cameraSizeY);
        cameras[2].rect = new Rect(1 / 2f - cameraCenterX, 1 / 2f + cameraCenterY, cameraSizeX, cameraSizeY);

        if (type == Direction.topDown)
        {
            cameras[3].rect = new Rect(1 / 2f - (3 * cameraCenterX), 1 / 2f - cameraCenterY, cameraSizeX, cameraSizeY);
            cameras[4].rect = new Rect(1 / 2f + cameraCenterX, 1 / 2f - cameraCenterY, cameraSizeX, cameraSizeY);
        }
        else if (type == Direction.bottomUp)
        {
            cameras[3].rect = new Rect(1 / 2f + cameraCenterX, 1 / 2f - cameraCenterY, cameraSizeX, cameraSizeY);
            cameras[4].rect = new Rect(1 / 2f - (3 * cameraCenterX), 1 / 2f - cameraCenterY, cameraSizeX, cameraSizeY);
        }
    }

    Vector2 GetMainGameViewSize()
    {
        Type T = Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }

    IEnumerator FadeViewport()
    {
        float elapsedTime = 0f;
        float time = 5f;

        while (elapsedTime < time)
        {
            float color = Mathf.Lerp(1f, 0f, elapsedTime / time);
            cameras[0].backgroundColor = new Color(color, color, color, color);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}