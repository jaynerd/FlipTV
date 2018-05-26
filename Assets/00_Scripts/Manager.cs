using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prime31.TransitionKit;
using TMPro;
using Glitch;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    public Camera mainCam;
    public MainCam cam;
    public Button powerButton;
    public Button restartButton;
    public Button rewardButton;
    public AnalogGlitch glitch;
    public Texture2D starTexture;
    public TextMeshProUGUI titleLabelA;
    public TextMeshProUGUI titleLabelB;
    public GameObject gameOverPanel;
    public Television television;
    public GameObject remote;
    public Decagon[] decagons;
    public Rectangle[] rectangles;
    public Transform glass;
    public Timer timer;

    public TextMeshProUGUI highscoreLabel;
    public TextMeshProUGUI scoreLabel;

    bool isRewarded = false;
    bool isTargetColorSet = false;

    Vector3 remoteOrgPos = Settings.remoteOrgPos;
    Vector3 remoteTrgPos = Settings.remoteTrgPos;

    Color startColor = Settings.cameraStartColor;
    Color titleColor = Settings.cameraTitleColor;
    Color inGameColor = Settings.cameraInGameColor;
    Color[] colors;

    AudioSource aSource;
    public AudioClip win;
    public AudioClip yes;

    int point = 0;
    int score = 0;
    int highscore = 0;

    float fadeDelay = Settings.screenFadeDelay;
    float fadeDuration = Settings.screenFadeDuration;
    float glitchSpeed = Settings.screenGlitchSpeed;
    float colorizeSpeed = Settings.powerButtonColorizeSpeed;
    float relocateSpeed = Settings.remoteRelocateSpeed;

    float startDelay = Settings.gameStartDelay;
    float orgOrbitSpeed = Settings.decagonOrbitSpeed;
    float curOrbitSpeed = Settings.decagonOrbitSpeed;
    float extraOrbitSpeed = 0f;
    float orbitSpeedIncrement = Settings.decagonSpeedIncrement;
    float speedUpInterval = Settings.decagonSpeedUpInterval;

    void Awake()
    {
        Instance = this;

        mainCam.backgroundColor = startColor;
        cam = mainCam.GetComponent<MainCam>();
        powerButton.onClick.AddListener(() => TurnOn());
        restartButton.onClick.AddListener(() => Initialize());
        rewardButton.onClick.AddListener(() => Reward());
        colors = new Color[rectangles.Length];
        for (int i = 0; i < rectangles.Length; i++)
        {
            colors[i] = rectangles[i].originalColor;
        }
        aSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        CancelInvoke();
        StopAllCoroutines();
        point = 0;
        if (isRewarded)
        {
            score = PlayerPrefs.GetInt("SavedScore");
        }
        else
        {
            score = 0;
        }
        isTargetColorSet = false;
        gameOverPanel.SetActive(false);
        timer.gameObject.SetActive(false);
        powerButton.gameObject.SetActive(false);
        titleLabelA.gameObject.SetActive(false);
        titleLabelB.gameObject.SetActive(false);
        Fade(startColor);
        Invoke("EnableTitleComponent", fadeDelay);
        InvokeRepeating("RandomizeJitter", 1f, 1f);
        television.Initialize();
        remote.transform.position = remoteOrgPos;
        curOrbitSpeed = orgOrbitSpeed;
        extraOrbitSpeed = 0f;
        for (int i = 0; i < decagons.Length; i++)
        {
            decagons[i].gameObject.SetActive(false);
        }
        GameObject glass = GameObject.Find("Glass(Clone)");
        GameObject fragments = GameObject.Find("Combined Fragments");
        if (glass != null)
        {
            Destroy(glass);
            if (fragments != null)
            {
                Destroy(fragments);
            }
        }
    }

    void Fade(Color color)
    {
        FadeTransition fader = new FadeTransition()
        {
            nextScene = 99,
            fadedDelay = fadeDelay,
            duration = fadeDuration,
            fadeToColor = color
        };
        TransitionKit.instance.transitionWithDelegate(fader);
        StartCoroutine(ChangeBackgroundColor(titleColor));
    }

    void EnableTitleComponent()
    {
        powerButton.gameObject.SetActive(true);
        titleLabelA.gameObject.SetActive(true);
        StartCoroutine("Colorize");
    }

    void DisableTitleComponent()
    {
        powerButton.gameObject.SetActive(false);
        titleLabelA.gameObject.SetActive(false);
        StopCoroutine("Colorize");
    }

    void RandomizeJitter()
    {
        float randomDelay = Random.Range(0.5f, 3f);
        float randomDrift = Random.Range(0.1f, 1f);
        float randomJitter = Random.Range(0.1f, 1f);
        StartCoroutine(Jitter(randomDelay, randomDrift, randomJitter));
    }

    void TurnOn()
    {
        CancelInvoke();
        glitch.colorDrift = 0f;
        glitch.scanLineJitter = 0f;
        DisableTitleComponent();
        ImageMaskTransition mask = new ImageMaskTransition()
        {
            maskTexture = starTexture,
            backgroundColor = new Color(0f, 0f, 0f, 0.9f),
            nextScene = 99
        };
        TransitionKit.instance.transitionWithDelegate(mask);
        for (int i = 0; i < decagons.Length; i++)
        {
            decagons[i].Relocate();
        }
        StartCoroutine(ChangeBackgroundColor(inGameColor));
        ActivateTimer();
    }

    void Shuffle()
    {
        for (int i = colors.Length - 1; i >= 0; i--)
        {
            int r = Random.Range(0, i + 1);
            Color tempColor = colors[i];
            colors[i] = colors[r];
            colors[r] = tempColor;
            rectangles[i].SetNewIndex(colors[i], i);
        }
        aSource.PlayOneShot(win);
        television.SetNewPattern(colors);
    }

    void ActivateTimer()
    {
        timer.Initialize();
        timer.gameObject.SetActive(true);
        timer.StartCoroutine("Countdown");
    }

    void ActivateTelevision()
    {
        television.SetFree();
    }

    void ActivateGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        if (isRewarded)
        {
            rewardButton.interactable = false;
           isRewarded = false;
        }
        else
        {
           rewardButton.interactable = true;
        }
        highscore = PlayerPrefs.GetInt("HighScore");
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("HighScore", highscore);
        }
        highscoreLabel.text = highscore.ToString();
        scoreLabel.text = score.ToString();
    }

    void ActivateDecagon()
    {
        for (int i = 0; i < decagons.Length; i++)
        {
            decagons[i].Initialize();
        }
    }

    void Shatter()
    {
        Instantiate(glass, Vector3.zero, Quaternion.identity);
        InvokeRepeating("Shoot", 0.05f, 0.05f);
    }

    void Shoot()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(ray, out hit))
        {
            hit.collider.gameObject.SendMessage("Damage", 2f, SendMessageOptions.DontRequireReceiver);
        }
    }

    void Reward()
    {
        isRewarded = true;
        PlayerPrefs.SetInt("SavedScore", score);
        Initialize();
    }

    Color SetTargetColor(Color iColor)
    {
        Color targetColor = new Color();
        if (!isTargetColorSet)
        {
            if (iColor.r > 0.9f && iColor.g < 0.1f)
            {
                targetColor = Color.yellow;
            }
            else if (iColor.r > 0.9f && iColor.g > 0.1f)
            {
                targetColor = Color.green;
            }
            else if (iColor.r < 0.1f && iColor.b < 0.1f)
            {
                targetColor = Color.cyan;
            }
            else
            {
                targetColor = Color.red;
            }
        }
        isTargetColorSet = true;
        return targetColor;
    }

    IEnumerator Jitter(float delay, float drift, float jitter)
    {
        yield return new WaitForSeconds(delay);
        glitch.colorDrift = drift;
        glitch.scanLineJitter = jitter;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            glitch.colorDrift = Mathf.Lerp(glitch.colorDrift, 0, glitchSpeed * 0.5f * Time.deltaTime);
            glitch.scanLineJitter = Mathf.Lerp(glitch.scanLineJitter, 0, glitchSpeed * 0.5f * Time.deltaTime);
            if (glitch.colorDrift < 0.05f)
            {
                break;
            }
            glitch.colorDrift = 0f;
            glitch.scanLineJitter = 0f;
        }
    }

    IEnumerator Colorize()
    {
        Color powerButtonColor = powerButton.image.color;
        Color targetColor = SetTargetColor(powerButtonColor);
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (isTargetColorSet)
            {
                powerButtonColor = Color.Lerp(powerButtonColor, targetColor, colorizeSpeed * Time.deltaTime);
                powerButton.image.color = powerButtonColor;
                Vector3 powerButtonColorVector = new Vector3(powerButtonColor.r, powerButtonColor.g, powerButtonColor.b);
                Vector3 targetColorVector = new Vector3(targetColor.r, targetColor.g, targetColor.b);
                if (Vector3.Distance(powerButtonColorVector, targetColorVector) < 0.1f)
                {
                    isTargetColorSet = false;
                    targetColor = SetTargetColor(powerButtonColor);
                }
            }
        }
    }

    IEnumerator ChangeBackgroundColor(Color color)
    {
        yield return new WaitForSeconds(fadeDelay);
        mainCam.backgroundColor = color;
    }

    IEnumerator RelocateRemote()
    {
        yield return new WaitForSeconds(fadeDelay);
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            remote.transform.position = Vector3.Lerp(remote.transform.position, remoteTrgPos, relocateSpeed * Time.deltaTime);
            if (Vector3.Distance(remote.transform.position, remoteTrgPos) < 0.025f)
            {
                yield break;
            }
        }
    }

    IEnumerator SpeedUp()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            yield return new WaitForSeconds(speedUpInterval);
            extraOrbitSpeed += orbitSpeedIncrement;
            curOrbitSpeed += extraOrbitSpeed;
        }
    }

    public void Begin()
    {
        timer.gameObject.SetActive(false);
        titleLabelB.gameObject.SetActive(true);
        titleLabelB.text = "Fliptv";
        StartCoroutine("RelocateRemote");
        StartCoroutine("SpeedUp");
        Invoke("ActivateTelevision", fadeDelay * 5f);
        Invoke("ActivateDecagon", startDelay * 0.1f);
    }

    public void AddPoint()
    {
       
        point++;
        if (point.Equals(4))
        {
            point = 0;
            AddScore();
        }
    }

    public void AddScore()
    {
        aSource.PlayOneShot(yes);
        score++;
        titleLabelB.text = "ch." + score;
        if ((score % 3).Equals(0))
        {
            Shuffle();
        }
        StartCoroutine(Glitch());
    }

    public void GameOver()
    {
        Shatter();
        StopAllCoroutines();
        cam.StartCoroutine("Shake");
        StartCoroutine("Glitch");
        Invoke("ActivateGameOverPanel", 1f);
        InvokeRepeating("RandomizeJitter", 1f, 1f);
        mainCam.backgroundColor = Color.gray;
        for (int i = 0; i < decagons.Length; i++)
        {
            decagons[i].Halt();
        }
    }

    public float GetOrbitSpeed()
    {
        return curOrbitSpeed;
    }

    public Color GetNextColor(int colorIndex)
    {
        return colors[colorIndex];
    }

    public Color GetRandomColor(Decagon decagon)
    {
        int r = Random.Range(0, decagons.Length);
        decagon.colorCode = r;
        return colors[r];
    }

    public IEnumerator Glitch()
    {
        glitch.colorDrift = 0.5f;
        glitch.scanLineJitter = 0.5f;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            glitch.colorDrift = Mathf.Lerp(glitch.colorDrift, 0, glitchSpeed * Time.deltaTime);
            glitch.scanLineJitter = Mathf.Lerp(glitch.scanLineJitter, 0, glitchSpeed * Time.deltaTime);
            if (glitch.colorDrift < 0.05f)
            {
                break;
            }
        }
        glitch.colorDrift = 0f;
        glitch.scanLineJitter = 0f;
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        Initialize();
    //    }
    //}
}