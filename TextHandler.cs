using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(AudioSource))]

public class TextHandler : MonoBehaviour
{

    [SerializeField] private GameObject skipText;           // Skip text game object (like a banner)
    [SerializeField] private GameObject eventSystem;        // Event system for turn on/off events while text typing
    [SerializeField] private string helloPhrase = "Hello!"; // Phrase which types on start
    [SerializeField] private string[] starterPhraseList;    // List of phrases following after helloPhrase

    private Text _text;                 // Text component
    private AudioSource _textSound;     // Audio Source component (symbol typing sound, like a *beep* or something)
    private Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();  // It's coroutine queue, yes.

    // Some bools (only one just in moment)
    private bool _stopText = false;

    // First frame void
    void Start()
    {
        _text = GetComponent<Text>();
        _textSound = GetComponent<AudioSource>();
        StartCoroutine(CoroutineQueueExecutor());
        TypePhrase(helloPhrase, 1, 1);
        foreach (string phrase in starterPhraseList)
        {
            TypePhrase(phrase, endDelay: 1);
        }
    }

    // Add a coroutine TypePhraseCoroutine in queue
    public void TypePhrase(string phrase, float startDelay = 0.0f, float endDelay = 0.0f)
    {
        _coroutineQueue.Enqueue(TypePhraseCoroutine(phrase, startDelay, endDelay));
    }

    // Coroutine for awesome text typing (with delays and bip-boop sounds ^^) - seems raw
    // string phrase - text to type
    // float startDelay - delay before typing
    // float endDelay - delay after typing
    public IEnumerator TypePhraseCoroutine(string phrase, float startDelay = 0.0f, float endDelay = 0.0f)
    {
        // On start it turn off event system for disable button clicks when text typing
        // And turn on Skip Text sign. While it active user can skip phrases (stop active typing coroutines)
        eventSystem.SetActive(false);
        skipText.SetActive(true);
        yield return new WaitForSeconds(startDelay);
        foreach (char i in phrase)
        {
            if (_stopText)
            {
                _stopText = false;
                _text.text = "";
                eventSystem.SetActive(true);
                skipText.SetActive(false);
                yield break;
            }
            _text.text += i;
            _textSound.Play();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(endDelay);
        _text.text = "";
        _stopText = false;
        eventSystem.SetActive(true);
        skipText.SetActive(false);
    }
    // Works non-stop. Execute the coroutine queue as they arrive
    public IEnumerator CoroutineQueueExecutor()
    {
        while (true)
        {
            while (_coroutineQueue.Count > 0)
                yield return _coroutineQueue.Dequeue();
            yield return new WaitForEndOfFrame();
        }
    }

    // Runs for each frame
    private void Update()
    {
        // Press F to stop text typing (‘-‘*ゞ 
        if (Input.GetKeyDown(KeyCode.F) && skipText.activeInHierarchy)
        {
            _coroutineQueue.Clear();
            _stopText = true;
        }
    }
}
