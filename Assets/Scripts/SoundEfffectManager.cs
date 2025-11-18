using UnityEngine;
using UnityEngine.UI;

public class SoundEfffectManager : MonoBehaviour
{
    private static SoundEfffectManager Instance;
    static AudioSource audioSource;
    static SoundEffectLibrary library;
    [SerializeField] private Slider sfxSlider;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            library = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }
    void Start()
    {
        //sfxSlider.onValueChanged.AddListener(delegate { OnValueChange(); });
    }
    public static void Play(string audioName)
    {
        AudioClip audioClip = library.GetRandomClip(audioName);
        if (audioClip != null) 
        {
            audioSource.PlayOneShot(audioClip);
        }
        else { Debug.Log("null"); }
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
    public void OnValueChange()
    {
        SetVolume(sfxSlider.value);
    }
}
