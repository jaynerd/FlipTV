using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decagon : MonoBehaviour
{
    public Rectangle rectangle;
    public bool isOrbiting = false;
    [HideInInspector] public int colorCode = 0;

    Manager manager;
    Vector3 iPos;
    SpriteRenderer sRend;

    bool isReady = false;
    bool isInitialized = false;
    float orbitAngle = 0f;
    float orbitRadius = 0f;
    float orbitSpeed = Settings.decagonOrbitSpeed;
    float speedUpInterval = Settings.decagonSpeedUpInterval;
    float verticalDistance = Settings.decagonVerticalDistance;
    float horizontalDistance = Settings.decagonHorizontalDistance;

    void Awake()
    {
        manager = Manager.Instance;
        iPos = transform.position;
        sRend = GetComponent<SpriteRenderer>();
        orbitRadius = transform.position.x;
        isInitialized = true;
    }

    void Update()
    {
        if (isOrbiting)
        {
            Orbit();
        }
    }

    public void Orbit()
    {
        orbitAngle -= orbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(orbitAngle) * orbitRadius * horizontalDistance;
        float y = Mathf.Sin(orbitAngle) * orbitRadius * verticalDistance;
        transform.position = new Vector2(x, y);
    }

    void OnTriggerEnter2D()
    {
        if (isReady)
        {
            if (colorCode.Equals(rectangle.colorIndex))
            {
                manager.AddPoint();
            }
            else
            {
                manager.GameOver();
            }
        }
        else
        {
            isReady = true;
        }
    }

    void OnTriggerExit2D()
    {
        if (isReady)
        {
            Color newColor = manager.GetRandomColor(this);
            sRend.color = newColor;
        }
    }

    public void Initialize()
    {
        orbitSpeed = Settings.decagonOrbitSpeed;
        gameObject.SetActive(true);
        StartCoroutine("UpdateSpeed");
        sRend.color = Color.white;
        isReady = false;
        isOrbiting = true;
        orbitAngle = 0f;
        colorCode = 0;
    }

    public void Relocate()
    {
        if (isInitialized)
        {
            transform.position = iPos;
        }
    }

    public void Halt()
    {
        StopAllCoroutines();
        isOrbiting = false;
    }

    public IEnumerator UpdateSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(speedUpInterval);
            orbitSpeed = manager.GetOrbitSpeed();
        }
    }
}