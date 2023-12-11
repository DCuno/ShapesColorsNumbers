using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    // Sounds
    public AudioClip[] pops = new AudioClip[3];
    public AudioClip[] teleports = new AudioClip[3];
    public AudioClip[] voices_shapes = new AudioClip[6];
    public AudioClip[] voices_colors = new AudioClip[7];
    public AudioClip[] voices_numbers = new AudioClip[100];
    public AudioClip[] music = new AudioClip[1];

    private static Audio[] instance = new Audio[2];
    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private void Awake()
    {
        if (instance[0] == null)
        {
            instance[0] = this;
            DontDestroyOnLoad(instance[0]);
        }
        else if (instance[1] == null)
        {
            instance[1] = this;
            DontDestroyOnLoad(instance[1]);
        }
        else
        {
            Destroy(gameObject);
        }

        // Loading Audio
        music[0] = Resources.Load<AudioClip>("Audio/Music/princess-15181");
        pops[0] = Resources.Load<AudioClip>("Audio/pop1");
        pops[1] = Resources.Load<AudioClip>("Audio/pop2");
        pops[2] = Resources.Load<AudioClip>("Audio/pop3");
        teleports[0] = Resources.Load<AudioClip>("Audio/teleport1");
        teleports[1] = Resources.Load<AudioClip>("Audio/teleport2");
        teleports[2] = Resources.Load<AudioClip>("Audio/teleport3");
        voices_shapes[0] = Resources.Load<AudioClip>("Audio/shapes_triangle");
        voices_shapes[1] = Resources.Load<AudioClip>("Audio/shapes_square");
        voices_shapes[2] = Resources.Load<AudioClip>("Audio/shapes_pentagon");
        voices_shapes[3] = Resources.Load<AudioClip>("Audio/shapes_hexagon");
        voices_shapes[4] = Resources.Load<AudioClip>("Audio/shapes_circle");
        voices_shapes[5] = Resources.Load<AudioClip>("Audio/shapes_star");
        voices_colors[0] = Resources.Load<AudioClip>("Audio/colors_red");
        voices_colors[1] = Resources.Load<AudioClip>("Audio/colors_orange");
        voices_colors[2] = Resources.Load<AudioClip>("Audio/colors_yellow");
        voices_colors[3] = Resources.Load<AudioClip>("Audio/colors_green");
        voices_colors[4] = Resources.Load<AudioClip>("Audio/colors_blue");
        voices_colors[5] = Resources.Load<AudioClip>("Audio/colors_purple");
        voices_colors[6] = Resources.Load<AudioClip>("Audio/colors_white");
        for (int i = 0; i < 100; i++)
        {
            voices_numbers[i] = Resources.Load<AudioClip>("Audio/numbers_" + (i + 1));
        }

        _sfxSource = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<AudioSource>();
        _musicSource = GameObject.FindGameObjectWithTag("MusicSource").GetComponent<AudioSource>();
    }

    public void TeleportSound()
    {
        int x = Random.Range(0, teleports.Length);
        _sfxSource.PlayOneShot(teleports[x]);
    }

    public void PopSound()
    {
        int x = Random.Range(0, pops.Length);
        _sfxSource.PlayOneShot(pops[x]);
    }

    public void VoiceSound(Spawner.Topics voice, Spawner.Shape shape)
    {
        if (voice == Spawner.Topics.Shapes)
        {
            _sfxSource.PlayOneShot(voices_shapes[(int)shape]);
        }
    }
    
    public void VoiceSound(Spawner.Topics voice, Spawner.Colors color)
    {
        if (voice == Spawner.Topics.Colors)
        {
            _sfxSource.PlayOneShot(voices_colors[(int)color]);
        }
    }

    public void VoiceSound(Spawner.Topics voice)
    {
        if (voice == Spawner.Topics.Numbers)
        {
            // Need to increment the polygon count if the pop text isn't on. Because it increments in pop text otherwise.
            if (voice/*_text*/ != Spawner.Topics.Numbers)
                _sfxSource.PlayOneShot(voices_numbers[GetComponentInParent<Spawner>().Count++]);
            else
                _sfxSource.PlayOneShot(voices_numbers[GetComponentInParent<Spawner>().Count]);
        }
    }
}
