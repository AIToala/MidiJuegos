using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;
using UnityEngine.UI;

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

    [Header("Animations")]
    public Color glowColor = Color.magenta;
    public float glowDuration = 0.5f;
    public int glowTimes = 1;
    public float delayBetweenGlows = 0.5f;
    public float repeatInterval = 10f;

    private Material[] originalMaterials;


    public void Initialize()
    {
        selectedTile = null;
        hoveredTile = null;
        GameController = FindObjectOfType<SequenceGameController>();
        images = new List<Sprite>(4);
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
        foreach (MusicTile tile in tiles)
        {
            tile.SetImage(images[tileCount]);
            tile.PointerEnterEvent.AddListener(TilePointerEnter);
            tile.PointerExitEvent.AddListener(TilePointerExit);
            tile.PointerClickEvent.AddListener(TilePointerClick);
            tile.name = "Music Tile" + tileCount.ToString();
            tile.SetIndex(tileCount);
            tileCount++;
        }

        originalMaterials = new Material[tiles.Count];
        for (int i = 0; i < tiles.Count; i++)
        {
            originalMaterials[i] = tiles[i].GetComponent<Image>().material;
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
        foreach (int order in GameController.GetSequence())
        {
            tiles[order].GetComponent<Image>().DOColor(glowColor, glowDuration);
            yield return new WaitForSeconds(delayBetweenGlows);
            tiles[order].GetComponent<Image>().DOColor(Color.white, glowDuration);
        }
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
        //glowTile(tile);
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
                //play-sound-error
                correctSequence = false;
                GameController.AddPlayerMiss();
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
    }

    void Update()
    {
        if (selectedTile == null) return;
    }

    void LoadSequence()
    {
        Sprite[] resources = Resources.LoadAll<Sprite>("SecuenciaImages/MusicTile");
        if (resources == null || resources.Length == 0)
        {
            Debug.LogError("No images found in folder: " + "SecuenciaImages/MusicTile");
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
            DOTween.Kill(tile.transform);
            tile.enabled = false;
            tile.transform.SetParent(null);
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }
}