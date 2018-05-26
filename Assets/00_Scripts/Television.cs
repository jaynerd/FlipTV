using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Television : MonoBehaviour
{
    public GameObject screen;
    public SpriteRenderer[] panels;
    public AudioClip hitFX;

    Manager manager;
    Vector3 iPos;
    Rigidbody2D rbody;
    AudioSource aSource;

    int managerLayer = Settings.gameManagerLayer;
    float jumpForce = Settings.televisionJumpForce;

    void Awake()
    {
        iPos = transform.position;
        rbody = GetComponent<Rigidbody2D>();
        aSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        manager = Manager.Instance;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(managerLayer))
        {
            aSource.PlayOneShot(hitFX, 2f);
            manager.StartCoroutine("Glitch");
            Invoke("TurnOn", 1f);
        }
    }

    void TurnOn()
    {
        screen.SetActive(false);
    }

    public void Initialize()
    {
        transform.position = iPos;
        rbody.isKinematic = true;
        screen.SetActive(true);
    }

    public void SetFree()
    {
        rbody.isKinematic = false;
    }

    public void SetNewPattern(Color[] colors)
    {
        screen.SetActive(true);
        rbody.AddForce(Vector2.up * jumpForce);
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].color = colors[i];
        }
    }
}