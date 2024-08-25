using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Timeline;
using UnityEditor.U2D.Aseprite;
using Unity.VisualScripting;

public class MusicTileController : MonoBehaviour
{
    [SerializeField] public SequenceGameController GameController;
    [SerializeField] private MusicTile selectedTile;
    [SerializeReference] private MusicTile hoveredTile;
    [SerializeField] private GridLayoutGroup grid;

    [SerializeField] private GameObject musicTile;

    [Header("Spawn Settings")]
    [SerializeField] private int tilesToSpawn = 4;
    [SerializeField] private int size = 150;
    public List<MusicTile> tiles = new List<MusicTile>(4);
    public List<MusicTile> selectedTiles = new List<MusicTile>(4);
    public List<Sprite> images = new List<Sprite>(4);
    public List<AudioClip> audioSequence;

    [Header("Animations")]
    public Color glowColor = Color.white;
    public float glowDuration = 0.5f;
    public int glowTimes = 1;
    public float delayBetweenGlows = 0.5f;
    public float repeatInterval = 10f;

    public void Initialize()
    {
        audioSequence = new List<AudioClip>(4);
        if (GameController.GetAudioSequence() == null)
        {
            audioSequence = GameController.GenerateRandomAudioSequence();
            GameController.SetAudioSequence(audioSequence);
        }
        selectedTile = null;
        hoveredTile = null;
        GameController = FindObjectOfType<SequenceGameController>();
        images = new List<Sprite>(4);
        audioSequence = GameController.GetAudioSequence();
        grid = GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(size, size);
        grid.spacing = new Vector2(20, 20);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 2;
        selectedTiles = new List<MusicTile>(4);

        for (int i = 0; i < tilesToSpawn; i++)
        {
            Instantiate(musicTile, transform);
        }

        tiles = GetComponentsInChildren<MusicTile>().ToList();
        LoadSequence();
        int tileCount = 0;
        List<int> sequence = GameController.GetSequence();
        foreach (MusicTile tile in tiles)
        {
            if (images[tileCount].name.Contains("RED")) tile.SetColor("RED");
            if (images[tileCount].name.Contains("BLUE")) tile.SetColor("BLUE");
            if (images[tileCount].name.Contains("GREEN")) tile.SetColor("GREEN");
            if (images[tileCount].name.Contains("YELLOW")) tile.SetColor("YELLOW");
            tile.SetClip(audioSequence[tileCount]);
            tile.PointerEnterEvent.AddListener(TilePointerEnter);
            tile.PointerExitEvent.AddListener(TilePointerExit);
            tile.PointerClickEvent.AddListener(TilePointerClick);
            tile.name = "Music Tile" + tile.GetColor();
            tile.SetIndex(tileCount);
            tile.SetBase();
            tileCount++;
        }

        StartCoroutine(RepeatGlowSequence());
    }

    private IEnumerator RepeatGlowSequence()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return StartCoroutine(GlowSequence());
            yield return new WaitForSeconds(repeatInterval);
        }
    }

    private IEnumerator GlowSequence()
    {
        GetComponentInParent<Image>().sprite = Resources.Load<Sprite>("SecuenciaImages/LISTEN");
        foreach (int order in GameController.GetSequence())
        {
            MusicTile tile = tiles[order];
            tile.GetImageComponent().DOColor(tile.GetColorGlow(), 1.5f);
            tile.GetIconComponent().DOColor(tile.baseColor, 1.5f);
            tile.PlayClip();
            yield return new WaitForSeconds(3f);
            tile.GetImageComponent().DOColor(tile.baseColor, 1.5f);
            tile.GetIconComponent().DOColor(tile.GetColorGlow(), 1.5f);
            yield return new WaitForSeconds(1.5f);
            yield return null;
        }
        GetComponentInParent<Image>().sprite = Resources.Load<Sprite>("SecuenciaImages/TURN");
    }

    private IEnumerator GlowTile(MusicTile tile)
    {
        string color = tile.GetColor().ToUpper();
        if (tile.GetColor() == "RED") tile.SetGlow();
        if (tile.GetColor() == "BLUE") tile.SetGlow();
        if (tile.GetColor() == "GREEN") tile.SetGlow();
        if (tile.GetColor() == "YELLOW") tile.SetGlow();
        tile.PlayClip();
        yield return new WaitForSeconds(tile.GetAudioSource().clip.length);
        yield return new WaitForSeconds(0.5f);
        tile.SetBase();

        yield return null;
    }

    public void TilePointerEnter(MusicTile tile)
    {
        hoveredTile = tile;
    }

    public void TilePointerExit(MusicTile tile)
    {
        hoveredTile = null;
    }

    public void TilePointerClick(MusicTile tile)
    {
        selectedTile = tile;
        StartCoroutine(GlowTile(tile));
        SetSelectedTile();

    }

    private void CheckIfSequenceIsCorrect()
    {
        List<int> sequence = GameController.GetSequence();
        bool correctSequence = false;
        for (int i = 0; i < sequence.Count; i++)
        {
            if (selectedTiles[i].GetIndex() == sequence[i])
            {
                selectedTiles[i].SetIsCorrect(true);
                //play-sound-correct in order
                correctSequence = true;
            }
            else
            {
                selectedTiles[i].SetIsCorrect(false);
                correctSequence = false;
                break;
            }
        }
        if (correctSequence)
        {
            GameController.AddPoint();
            ResetSelectedTiles();
            ClearMusicTiles();
            GameController.FinishRound();
        }
        else
        {
            GameController.AddPlayerMiss();
        }
    }

    void Update()
    {
        if (selectedTile == null) return;
    }

    void LoadSequence()
    {
        Sprite[] resources = Resources.LoadAll<Sprite>("SecuenciaImages/MusicTile/Base");
        if (resources == null || resources.Length == 0)
        {
            Debug.LogError("No images found in folder: " + "SecuenciaImages/MusicTile/Base");
            return;
        }
        resources = resources.OrderBy(x => Guid.NewGuid()).ToArray();
        List<Sprite> resourcesList = new List<Sprite>(resources);
        int capacity = images.Capacity;
        for (int i = 0; i < capacity; i++)
        {
            images.Add(resourcesList[i]);
        }
    }

    public List<int> GenerateRandomSequence()
    {
        return Enumerable.Range(0, tilesToSpawn).OrderBy(x => UnityEngine.Random.value).ToList();
    }

    public void SetSelectedTile()
    {
        if (selectedTile == null) return;
        if (selectedTiles.Count > 4)
        {
            ResetSelectedTiles();
        }
        selectedTiles.Add(selectedTile);
        if (selectedTiles.Count == 4)
        {
            CheckIfSequenceIsCorrect();
            ResetSelectedTiles();
            return;
        }
        selectedTile = null;
    }

    public void ResetSelectedTiles()
    {
        selectedTiles.Clear();
    }

    public void ClearMusicTiles()
    {
        transform.DetachChildren();
        foreach (MusicTile tile in tiles)
        {
            tile.StopAllCoroutines();
            DOTween.Kill(tile.GetImageComponent());
            DOTween.Kill(tile.GetIconComponent());
            DOTween.Kill(tile.transform);

            tile.enabled = false;
            tile.transform.SetParent(null);
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }
}