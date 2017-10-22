using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DNE;

public class DNEExample : MonoBehaviour {

    public AudioSource source;
    public Button button;
    public RectTransform panel;
    public Text title;
    public BuildObject build;
    public int current_index;

    private List<Button> buttons;

	// Use this for initialization
	void Start () {
        build = Resources.Load("Builds/Build") as BuildObject;
        build = build.Get(); //creates clone so that the build object does not get overwritten ie stays the same
        current_index = build.current_index;

        setTitle();
        createButtons();
        setAudio();
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void createButtons() {
        if (buttons != null) {
            for (int i = 0; i < buttons.Count; i++) {
                Destroy(buttons[i].gameObject);
            }
        }

        buttons = new List<Button>();

        List<string> trigs = build.GetCurrent().triggers;
        for (int i = 0; i < trigs.Count; i++) {
            Button t = Instantiate(button, panel, false);
            t.transform.position = new Vector3(t.transform.position.x + (t.GetComponent<RectTransform>().rect.width * i) + 20, t.transform.position.y, t.transform.position.z);
            t.GetComponentInChildren<Text>().text = trigs[i];
            t.GetComponentInChildren<Button>().onClick.AddListener(() => OnButtonClick(t.GetComponentInChildren<Text>().text));
            buttons.Add(t);
        }
    }

    private void setTitle() {
        title.text = build.GetCurrent().title;
    }

    private void setAudio() {
        source.clip = build.GetCurrent().clip;
        source.Play();
    }

    private void OnButtonClick(string trigger) {
        build.Next(trigger);
        if (build.current_index >= 0) {
            setTitle();
            createButtons();
            setAudio();
        } else {
            for (int i = 0; i < buttons.Count; i++) {
                Destroy(buttons[i].gameObject);
            }
            buttons = new List<Button>();
        }
        
    }
}
