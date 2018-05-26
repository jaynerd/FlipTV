using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public Manager manager;
    public Image circle;
    public TextMeshProUGUI label;
    public AudioClip tune_a;
    public AudioClip tune_b;

    AudioSource aSource;
    int time = 3;
    float unfillSpeed = Settings.timerUnfillSpeed;

    void Awake() { aSource = GetComponent<AudioSource>(); }

    public void Initialize()
    {
        time = 3;
        circle.fillAmount = 1f;
        label.text = "";
    }

    public IEnumerator Countdown()
    {
    
        yield return new WaitForSeconds(1.2f);
        label.text = time.ToString();
        aSource.PlayOneShot(tune_a);
        while (true)
        {
            circle.fillAmount = 1f;
            StartCoroutine("Unfill");
            yield return new WaitForSeconds(1f);
            aSource.PlayOneShot(tune_a);

            time--;
      
            if (time.Equals(0))
            {
                aSource.PlayOneShot(tune_b);
                manager.Begin();
                yield break;
            }
          
            label.text = time.ToString();
        }
    }

    public IEnumerator Unfill()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.03f);
            circle.fillAmount = Mathf.Lerp(circle.fillAmount, 0f, unfillSpeed * Time.deltaTime);
            if (circle.fillAmount < 0.005f)
            {
                StopCoroutine("Unfill");
                yield break;
            }
        }
    }
}