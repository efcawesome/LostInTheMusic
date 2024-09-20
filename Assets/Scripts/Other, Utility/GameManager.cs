using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    public static List<string> wallsBroken = new List<string>();
    private string currPianoTilesGame;
    private string currHatGame;
    private string currCatGame;
    private GameObject vcam;
    private GameObject player;
    private GameObject cam;
    public GameObject blackSquare;
    public string goToScene = "";

    public TMP_Text unlockHatText;
    public TMP_Text unlockAbilityText;
    public TMP_Text conductInstText;

    private AudioSource audioSource;
    public AudioClip inventorySound;

    public static float initTime;
    public static float endTime;

    private PlayerMovement pm;

    private GameObject stupidGodDamnGlobalLight;
    private GameObject worldSpaceCanvas;

    public static bool hasDonePianoTutorial = false;
    public static bool hasDoneHatTutorial = false;
    public static bool hasDoneCatTutorial = false;

    public static List<string> hasDefeated = new List<string>();

    public Image dashIcon;
    public Image glideIcon;

    // CHANGE LATER
    public static bool leftWallBroken;
    public static bool rightWallBroken;

    #region Dialogue Saving
    public static bool talkedToOwl = false;
    public static bool talkedToSand = false;
    public static bool talkedToNigel = false;
    public static bool talkedToHat = false;
    public static bool talkedToSpirit = false;

    #endregion

    #region Help Text Saving
    public static bool seenInventoryHelp = false;
    public static bool seenDashHelp = false;
    public static bool seenGlideHelp = false;
    public static bool seenMapHelp = false;
    #endregion

    public static bool hasSeenPogoVid = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(inventoryCanvas);
        DontDestroyOnLoad(map);
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        initTime = Time.realtimeSinceStartup;
        SceneManager.sceneLoaded += OnSceneLoaded;
        vcam = GameObject.Find("Virtual Camera");
        cam = GameObject.Find("Main Camera");
        player = GameObject.Find("Player");
        blackSquare = GameObject.Find("BlackSquare");
        blackSquare.SetActive(false);
        inventoryCanvas.SetActive(false);
        map.SetActive(false);
        pm = player.GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();

        unlockHatText.alpha = 0f;
        unlockAbilityText.alpha = 0f;
        conductInstText.alpha = 0f;
        dashIcon.color = new Color(1, 1, 1, 0);
        glideIcon.color = new Color(1, 1, 1, 0);

        foreach (string s in sceneNames) visitedRooms.Add(false); // propogate visitedRooms
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "PianoTiles":
                vcam.SetActive(false);
                cam.SetActive(false);
                worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
                worldSpaceCanvas.SetActive(false);
                GameObject.Find("GameController").GetComponent<PianoTilesGame>().endGameAction += EndPianoTilesGame;
                GameObject.Find("GameController").GetComponent<PianoTilesGame>().currGame = currPianoTilesGame;
                break;
            case "HatGameScene":
                vcam.SetActive(false);
                cam.SetActive(false);
                worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
                worldSpaceCanvas.SetActive(false);
                GameObject.Find("HatGameControllerPrefab").GetComponent<HatGameController>().endAction += EndHatGame;
                GameObject.Find("HatGameControllerPrefab").GetComponent<HatGameController>().currGame = currHatGame;
                break;
            case "KittenBasketGame":
                vcam.SetActive(false);
                cam.SetActive(false);
                worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
                worldSpaceCanvas.SetActive(false);
                GameObject.Find("CatBasketController").GetComponent<CatGameController>().endAction += EndCatGame;
                GameObject.Find("CatBasketController").GetComponent<CatGameController>().currGame = currCatGame;
                break;
            case "Credits":
                vcam.SetActive(false);
                cam.SetActive(false);
                //worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
                //worldSpaceCanvas.SetActive(false);
                break;
            case "17_EndingScene":
                if(talkedToSpirit)
                {
                    GameObject.Find("SpiritOfTheOrchestra").GetComponent<StartCreditsDialogueManager>().currDialogueInd = 1;
                }
                break;
            case "2_BatonSceneCopy":
                vcam.GetComponent<Cinemachine.CinemachineConfiner2D>().m_BoundingShape2D = GameObject.Find("SceneBounds").GetComponent<PolygonCollider2D>();

                if (pm.slowFallUnlocked)
                {
                    GameObject.Find("OwlGuide").GetComponent<StartGameDialogueManager>().dialogueTitles[1] = "Congratulate";
                    GameObject.Find("OwlGuide").GetComponent<StartGameDialogueManager>().currDialogueInd = 1;
                }
                else if (talkedToOwl || hasDonePianoTutorial) 
                {
                    GameObject.Find("OwlGuide").GetComponent<StartGameDialogueManager>().currDialogueInd = 1;
                }

                if (leftWallBroken) GameObject.Find("BatonBreakableWallL (1)").GetComponent<BreakableWall>().BreakWall(false);
                if (rightWallBroken) StartCoroutine(GameObject.Find("BatonBreakableWallL").GetComponent<BreakableWall>().WaitBreakWall());

                break;
            case "3_Sand":
                if (talkedToSand)
                {
                    GameObject.Find("Sandpiper").GetComponent<DialogueManager>().currDialogueInd = 1;
                }
                break;
            case "14_Grove":
                if (hatUnlocked[HatManager.GetHatIndex("cat")])
                {
                    GameObject.Find("Nigel").GetComponent<StartCatGameDialogueManager>().dialogueTitles[1] = "Congratulate";
                    GameObject.Find("Nigel").GetComponent<StartCatGameDialogueManager>().currDialogueInd = 1;
                }
                else if (talkedToNigel)
                {
                    GameObject.Find("Nigel").GetComponent<StartCatGameDialogueManager>().currDialogueInd = 1;
                }
                break;
            case "7_HatRoom":
                if(pm.canDash)
                {
                    GameObject.Find("HatBird").GetComponent<StartHatGameDialogueManager>().dialogueTitles[1] = "GoodLuck";
                    GameObject.Find("HatBird").GetComponent<StartHatGameDialogueManager>().currDialogueInd = 1;
                }
                if(talkedToHat)
                {
                    GameObject.Find("HatBird").GetComponent<StartHatGameDialogueManager>().currDialogueInd = 1;
                }
                break;
        }

        if(scene.name != "PianoTiles" && scene.name != "HatGameScene" && scene.name != "KittenBasketGame" && scene.name != "Credits" && scene.name != "2_BatonSceneCopy")
        {
            vcam.GetComponent<Cinemachine.CinemachineConfiner2D>().m_BoundingShape2D = GameObject.Find("SceneBounds").GetComponent<PolygonCollider2D>(); // Swap Camera scene bounds
        }

        if (sceneNames.Contains(scene.name))
        {
            visitedRooms[sceneNames.IndexOf(scene.name)] = true; // Set visited to true
            headLocation = headLocations[sceneNames.IndexOf(scene.name)];
        }
    }

    public void StartPianoTilesGame(string gameName)
    {
        currPianoTilesGame = gameName;
        stupidGodDamnGlobalLight = GameObject.Find("Global Light");
        stupidGodDamnGlobalLight.SetActive(false);
        SceneManager.LoadScene("PianoTiles", LoadSceneMode.Additive); // Load piano tiles scene on top
    }

    public void StartHatGame(string gameName)
    {
        currHatGame = gameName;
        stupidGodDamnGlobalLight = GameObject.Find("Global Light");
        stupidGodDamnGlobalLight.SetActive(false);
        SceneManager.LoadScene("HatGameScene", LoadSceneMode.Additive);
    }

    public void StartCatGame(string gameName)
    {
        currCatGame = gameName;
        stupidGodDamnGlobalLight = GameObject.Find("Global Light");
        stupidGodDamnGlobalLight.SetActive(false);
        SceneManager.LoadScene("KittenBasketGame", LoadSceneMode.Additive);
    }

    public void StartCredits()
    {
        //stupidGodDamnGlobalLight = GameObject.Find("Global Light");
        //stupidGodDamnGlobalLight.SetActive(false);
        endTime = Time.realtimeSinceStartup;
        SceneManager.LoadScene("Credits");
    }

    private void Update()
    {
        if (goToScene != "")
        {
            SceneManager.LoadScene(goToScene);
            goToScene = "";
        }

        if ((pm.hasControl || inventoryCanvas.activeInHierarchy || mapOpen) && Input.GetButtonDown("Inventory"))
        {
            ToggleInventory();
        }

        if((pm.hasControl || inventoryCanvas.activeInHierarchy || mapOpen) && Input.GetButtonDown("Map"))
        {
            SwapMap();
        }

        if(inventoryCanvas.activeInHierarchy)
        {
            if(Input.GetButtonDown("UpUI"))
            {
                NextScreen();
            }
            else if(Input.GetButtonDown("DownUI"))
            {
                PrevScreen();
            }

            if(Input.GetButtonDown("RightUI"))
            {
                if(currScreenInd == 0)
                {
                    NextHat();
                }
                else
                {
                    NextInst();
                }
            }
            else if(Input.GetButtonDown("LeftUI"))
            {
                if (currScreenInd == 0)
                {
                    PrevHat();
                }
                else
                {
                    PrevInst();
                }
            }
        }
    }

    public void EndPianoTilesGame()
    {
        if (ButtonController.score > GameObject.Find("GameController").GetComponent<PianoTilesGame>().fullMoonScore)
        {
            if (currPianoTilesGame == "IntroGame") player.GetComponent<PlayerMovement>().slowFallUnlocked = true;
            GameObject.Find("Dialogue System").GetComponent<DialogueRunner>().StartDialogue("FinishGame");
            GameObject.Find("OwlGuide").GetComponent<StartGameDialogueManager>().dialogueTitles[1] = "Congratulate";
            UnlockAbility(glideIcon);
        }
        else
        {
            GameObject.Find("Dialogue System").GetComponent<DialogueRunner>().StartDialogue("Dissapointment");
        }
        worldSpaceCanvas.SetActive(true);
        SceneManager.UnloadSceneAsync("PianoTiles");
        stupidGodDamnGlobalLight.SetActive(true);
        vcam.SetActive(true);
        cam.SetActive(true);
        
    }

    public void EndHatGame()
    {
        // Dialogue and stuff + rewards
        if (HatGameController.score > 50)
        {
            GameObject.Find("Dialogue System").GetComponent<DialogueRunner>().StartDialogue("Congratulate");
            pm.canDash = true;
            GameObject.Find("HatBird").GetComponent<StartHatGameDialogueManager>().dialogueTitles[1] = "GoodLuck";
            UnlockAbility(dashIcon);
        }
        else
        {
            GameObject.Find("Dialogue System").GetComponent<DialogueRunner>().StartDialogue("FailHatGame");
        }
        

        worldSpaceCanvas.SetActive(true);
        SceneManager.UnloadSceneAsync("HatGameScene");
        stupidGodDamnGlobalLight.SetActive(true);
        vcam.SetActive(true);
        cam.SetActive(true);
    }

    public void EndCatGame()
    {
        // Dialogue and stuff + rewards
        if (CatGameController.score > 35)
        {
            GameObject.Find("Dialogue System").GetComponent<DialogueRunner>().StartDialogue("Congratulate");
            GameObject.Find("Nigel").GetComponent<StartCatGameDialogueManager>().dialogueTitles[1] = "Congratulate";
            UnlockHat();
            hatUnlocked[HatManager.GetHatIndex("cat")] = true;
            // Spawn Hat
        }
        else
        {
            GameObject.Find("Dialogue System").GetComponent<DialogueRunner>().StartDialogue("AngryNigel");
        }

        worldSpaceCanvas.SetActive(true);
        SceneManager.UnloadSceneAsync("KittenBasketGame");
        stupidGodDamnGlobalLight.SetActive(true);
        vcam.SetActive(true);
        cam.SetActive(true);
    }

    public static IEnumerator FadeTextIn(TMP_Text text, float fadeInSpeed)
    {
        while (text.alpha < 1f)
        {
            text.alpha += fadeInSpeed * Time.deltaTime;
            yield return null;
        }

        text.alpha = 1;
    }

    public IEnumerator WaitFadeTextOut(TMP_Text text, float fadeOutSpeed)
    {
        yield return StartCoroutine(FadeTextOut(text, fadeOutSpeed));
    }

    public static IEnumerator FadeTextOut(TMP_Text text, float fadeOutSpeed)
    {
        while (text.alpha > 0f)
        {
            text.alpha -= fadeOutSpeed * Time.deltaTime;
            yield return null;
        }

        text.alpha = 0f;
    }

    public void GuaranteedFadeTextOut(TMP_Text text, float fadeOutSpeed)
    {
        StartCoroutine(FadeTextOut(text, fadeOutSpeed));
    }


    public void StartHideScreen(float time)
    {
        StartCoroutine(GameObject.Find("GameManager").GetComponent<GameManager>().HideScreen(time));
    }

    private IEnumerator HideScreen(float time)
    {
        blackSquare.SetActive(true);

        yield return new WaitForSeconds(time);

        blackSquare.SetActive(false);
    }

    public bool HasAllSpirits()
    {
        foreach(bool b in instCaptured)
        {
            if (!b) return false;
        }

        return true;
    }

    public int HatCount()
    {
        int count = 0;
        foreach(bool b in hatUnlocked)
        {
            if (b) count++;
        }

        return count - 1;
    }

    public void UnlockHat()
    {
        StartCoroutine(UH_CO());
    }

    private IEnumerator UH_CO()
    {
        yield return StartCoroutine(FadeTextIn(unlockHatText, 5f));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadeTextOut(unlockHatText, 5f));
    }

    public void UnlockAbility(Image i)
    {
        StartCoroutine(UA_CO(i));
    }

    private IEnumerator UA_CO(Image i)
    {
        StartCoroutine(FadeTextIn(unlockAbilityText, 5f));
        while (i.color.a < 1f)
        {
            var col = i.color;
            col.a += 5f * Time.deltaTime;
            i.color = col;
            yield return null;
        }
        i.color = Color.white;
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeTextOut(unlockAbilityText, 5f));
        while (i.color.a > 0f)
        {
            var col = i.color;
            col.a -= 5f * Time.deltaTime;
            i.color = col;
            yield return null;
        }
        i.color = new Color(1, 1, 1, 0);
    }

    public void ConductInst()
    {
        StartCoroutine(CI_CO());
    }

    private IEnumerator CI_CO()
    {
        yield return StartCoroutine(FadeTextIn(conductInstText, 5f));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadeTextOut(conductInstText, 5f));
    }

    // ========================= INVENTORY SYSTEM =========================

    public GameObject inventoryCanvas;
    public static bool inventoryOpen;

    public List<GameObject> screens;
    private int currScreenInd = 0;

    // Hat screen
    public Image hatIm;
    public TMP_Text hatTitle;
    public TMP_Text hatDescrip;
    public List<Sprite> hatSprites;
    public List<string> hatNames;
    public List<string> hatText;
    public List<bool> hatUnlocked;
    private int currHatInd = 0;

    // Bestiary screen
    public Image instIm;
    public TMP_Text instTitle;
    public TMP_Text instDescrip;
    public List<Sprite> instSprites;
    public List<string> instNames;
    public List<string> instText;
    public List<bool> instCaptured;
    private int currInstInd = 0;

    // Help screen
    public TMP_Text glideText;
    public TMP_Text dashText;
    public TMP_Text batonText;

    public void ToggleInventory()
    {
        inventoryCanvas.SetActive(!inventoryCanvas.activeInHierarchy);
        inventoryOpen = inventoryCanvas.activeInHierarchy;

        audioSource.PlayOneShot(inventorySound);
        Cursor.visible = inventoryCanvas.activeInHierarchy;

        if (inventoryCanvas.activeInHierarchy)
        {
            mapOpen = false;
            map.SetActive(false);
            pm.RevokeControl();
            for(int i = 0; i < screens.Count; i++)
            {
                if(i != currScreenInd) screens[i].SetActive(false);
                else screens[i].SetActive(true);
            }

            switch (currScreenInd)
            {
                case InventoryMenus.HatMenu:
                    currHatInd = player.GetComponentInChildren<HatManager>().GetCurrentHatIndex();
                    SetHatScreen();
                    break;
                case InventoryMenus.Bestiary:
                    SetInstScreen();
                    break;
                case InventoryMenus.Help:
                    SetHelpScreen();
                    break;
            }
        }
        else
        {
            pm.GiveControl();
        }
    }

    public void NextScreen()
    {
        screens[currScreenInd].SetActive(false);
        currScreenInd++;
        if (currScreenInd >= screens.Count) currScreenInd = 0;
        screens[currScreenInd].SetActive(true);

        switch (currScreenInd)
        {
            case InventoryMenus.HatMenu:
                currHatInd = player.GetComponentInChildren<HatManager>().GetCurrentHatIndex();
                SetHatScreen();
                break;
            case InventoryMenus.Bestiary:
                SetInstScreen();
                break;
            case InventoryMenus.Help:
                SetHelpScreen();
                break;
        }
    }

    public void PrevScreen()
    {
        screens[currScreenInd].SetActive(false);
        currScreenInd--;
        if (currScreenInd < 0) currScreenInd = screens.Count - 1;
        screens[currScreenInd].SetActive(true);

        switch(currScreenInd)
        {
            case InventoryMenus.HatMenu:
                currHatInd = player.GetComponentInChildren<HatManager>().GetCurrentHatIndex();
                SetHatScreen();
                break;
            case InventoryMenus.Bestiary:
                SetInstScreen();
                break;
            case InventoryMenus.Help:
                SetHelpScreen();
                break;
        }
    }

    public void NextHat()
    {
        currHatInd++;
        if (currHatInd >= hatSprites.Count) currHatInd = 0;

        SetHatScreen();
    }

    public void PrevHat()
    {
        currHatInd--;
        if (currHatInd < 0) currHatInd = hatSprites.Count - 1;

        SetHatScreen();
    }

    public void SetHatScreen()
    {
        hatIm.sprite = hatSprites[currHatInd];
        hatIm.rectTransform.sizeDelta = new Vector2(hatSprites[currHatInd].texture.width, hatSprites[currHatInd].texture.height);

        if (hatUnlocked[currHatInd])
        {
            player.GetComponentInChildren<HatManager>().SetHat(currHatInd);
            hatIm.color = Color.white;
            hatTitle.text = hatNames[currHatInd];
            hatDescrip.text = hatText[currHatInd];
        }
        else
        {
            hatIm.color = Color.black;
            hatTitle.text = "???";
            hatDescrip.text = "???";
        }
    }

    public void NextInst()
    {
        currInstInd++;
        if (currInstInd >= instSprites.Count) currInstInd = 0;

        SetInstScreen();
    }

    public void PrevInst()
    {
        currInstInd--;
        if (currInstInd < 0) currInstInd = instSprites.Count - 1;

        SetInstScreen();
    }

    public void SetInstScreen()
    {
        instIm.sprite = instSprites[currInstInd];
        instIm.rectTransform.sizeDelta = new Vector2(instSprites[currInstInd].texture.width, instSprites[currInstInd].texture.height);

        if (instCaptured[currInstInd])
        {
            instIm.color = Color.white;
            instTitle.text = instNames[currInstInd];
            instDescrip.text = instText[currInstInd];
        }
        else
        {
            instIm.color = Color.black;
            instTitle.text = "???";
            instDescrip.text = "???";
        }
    }

    public void SetHelpScreen()
    {
        if (player.GetComponent<PlayerMovement>().slowFallUnlocked) glideText.text = "Glide";
        else glideText.text = "???";

        if (player.GetComponent<PlayerMovement>().canDash) dashText.text = "Dash";
        else dashText.text = "???";

        if (player.GetComponent<PlayerAttack>().hasBaton) batonText.text = "Swing Baton";
        else batonText.text = "???";
    }

    // ========================= MAP SYSTEM =========================
    public static bool mapOpen;
    public GameObject map;
    public List<string> sceneNames;
    private List<bool> visitedRooms = new List<bool>();
    public List<RectMask2D> mapMasks;
    public List<Vector2> headLocations;
    private Vector2 headLocation = new Vector2(-483.6f, 144f);
    public GameObject headToken;

    public void SwapMap() {

        audioSource.PlayOneShot(inventorySound);

        if (mapOpen)
        {
            mapOpen = false;
            map.SetActive(false);
            pm.GiveControl();
        }
        else
        {
            inventoryCanvas.SetActive(false);
            inventoryOpen = false;
            mapOpen = true;
            headToken.transform.localPosition = headLocation;
            map.SetActive(true);
            pm.RevokeControl();

            for (int i = 0; i < visitedRooms.Count; i++)
            {
                if(visitedRooms[i]) mapMasks[i].enabled = false;
            }
        }
    }

    public void PlaySoundNoMusic(AudioClip clip)
    {
        StartCoroutine(PlaySoundNoMusicCo(clip));
    }

    private IEnumerator PlaySoundNoMusicCo(AudioClip clip)
    {
        cam.GetComponent<AudioSource>().Pause();
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        cam.GetComponent <AudioSource>().Play();
    }
}

class InventoryMenus
{
    public const int HatMenu = 0;
    public const int Bestiary = 1;
    public const int Help = 2;
}