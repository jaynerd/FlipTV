using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Color cameraStartColor = Color.black;
    public static Color cameraTitleColor = new Color(0.1f, 0.1f, 0.1f);
    public static Color cameraInGameColor = Color.white;

    public static int gameManagerLayer = 8;

    public static Vector3 remoteOrgPos = new Vector3(0f, -8f, 0f);
    public static Vector3 remoteTrgPos = new Vector3(0f, -3f, 0f);

    public static float cameraShakeDuration = 0.1f;

    public static float screenFadeDelay = 0.2f;
    public static float screenFadeDuration = 1f;
    public static float screenGlitchSpeed = 80f;
    public static float powerButtonColorizeSpeed = 5f;
    public static float remoteRelocateSpeed = 4f;

    public static float timerUnfillSpeed = 15f;
    public static float televisionJumpForce = 300f;

    public static float gameStartDelay = 10f;

    public static float decagonOrbitSpeed = 1f;
    public static float decagonSpeedUpInterval = 3f;
    public static float decagonSpeedIncrement = 0.003f;
    public static float decagonVerticalDistance = 1f;
    public static float decagonHorizontalDistance = 1.1f;
}