using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class RealWakgood : BossMonster
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private TMP_Text textMesh2;
    [SerializeField] private PostProcessVolume postProcessVolume;
    Mesh mesh;
    Vector3[] vertices;
    List<int> wordIndexes;
    List<int> wordLengths;
    public Gradient rainbow;
    Mesh mesh2;
    Vector3[] vertices2;
    List<int> wordIndexes2;
    List<int> wordLengths2;
    public Gradient rainbow2;

    protected override void Awake()
    {
        base.Awake();
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
    }

    void Start()
    {
        wordIndexes = new List<int> { 0 };
        wordLengths = new List<int>();

        string s = textMesh.text;
        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths.Add(index - wordIndexes[wordIndexes.Count - 1]);
            wordIndexes.Add(index + 1);
        }
        wordLengths.Add(s.Length - wordIndexes[wordIndexes.Count - 1]);


        wordIndexes2 = new List<int> { 0 };
        wordLengths2 = new List<int>();
        s = textMesh2.text;
        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths2.Add(index - wordIndexes2[wordIndexes2.Count - 1]);
            wordIndexes2.Add(index + 1);
        }
        wordLengths2.Add(s.Length - wordIndexes2[wordIndexes2.Count - 1]);
    }

    void Update()
    {
        if (textMesh.gameObject.activeSelf)
        {

            textMesh.ForceMeshUpdate();
            mesh = textMesh.mesh;
            vertices = mesh.vertices;

            Color[] colors = mesh.colors;

            for (int w = 0; w < wordIndexes.Count; w++)
            {
                int wordIndex = wordIndexes[w];
                Vector3 offset = Wobble(Time.time + w);

                for (int i = 0; i < wordLengths[w]; i++)
                {
                    TMP_CharacterInfo c = textMesh.textInfo.characterInfo[wordIndex + i];

                    int index = c.vertexIndex;

                    colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x * 0.001f, 1f));
                    colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x * 0.001f, 1f));
                    colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x * 0.001f, 1f));
                    colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x * 0.001f, 1f));

                    vertices[index] += offset;
                    vertices[index + 1] += offset;
                    vertices[index + 2] += offset;
                    vertices[index + 3] += offset;
                }
            }

            mesh.vertices = vertices;
            mesh.colors = colors;
            textMesh.canvasRenderer.SetMesh(mesh);
        }

        if (textMesh2.gameObject.activeSelf)
        {

            textMesh2.ForceMeshUpdate();
            mesh2 = textMesh2.mesh;
            vertices2 = mesh2.vertices;

            Color[] colors2 = mesh2.colors;

            for (int w = 0; w < wordIndexes2.Count; w++)
            {
                int wordIndex = wordIndexes2[w];
                Vector3 offset = Wobble(Time.time + w);

                for (int i = 0; i < wordLengths2[w]; i++)
                {
                    TMP_CharacterInfo c = textMesh2.textInfo.characterInfo[wordIndex + i];

                    int index = c.vertexIndex;

                    colors2[index] = rainbow2.Evaluate(Mathf.Repeat(Time.time + vertices2[index].x * 0.001f, 1f));
                    colors2[index + 1] = rainbow2.Evaluate(Mathf.Repeat(Time.time + vertices2[index + 1].x * 0.001f, 1f));
                    colors2[index + 2] = rainbow2.Evaluate(Mathf.Repeat(Time.time + vertices2[index + 2].x * 0.001f, 1f));
                    colors2[index + 3] = rainbow2.Evaluate(Mathf.Repeat(Time.time + vertices2[index + 3].x * 0.001f, 1f));

                    vertices2[index] += offset;
                    vertices2[index + 1] += offset;
                    vertices2[index + 2] += offset;
                    vertices2[index + 3] += offset;
                }
            }

            mesh2.vertices = vertices2;
            mesh2.colors = colors2;
            textMesh2.canvasRenderer.SetMesh(mesh2);
        }
    }

    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f * 3), Mathf.Cos(time * 2.5f * 3));
    }

    protected override IEnumerator Attack()
    {
        textMesh.text = "모두 '!드롭스' 를 외쳐주세요!";
        textMesh.gameObject.SetActive(true);
        StartCoroutine(Drops());

        yield return new WaitForSeconds(3f);
        textMesh.gameObject.SetActive(false);

        while (true)
        {
            int i = Random.Range(0, 2 + 1);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(TheShip());
                    break;
                case 1:
                    yield return StartCoroutine(GTA());
                    break;
                case 2:
                    yield return StartCoroutine(Waktyhall());
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Drops()
    {
        while (true)
        {

            Vector3 randomPos = transform.position + (Vector3)Random.insideUnitCircle * 30f;

            ObjectManager.Instance.PopObject("Drop", randomPos);

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private IEnumerator TheShip()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            postProcessVolume.weight = i;
            yield return null;
        }
        postProcessVolume.weight = 1;

        textMesh.text = "오뱅내는 하루종일 '더 쉽' 입니다.";
        textMesh2.text = "왁인마를 조심하세요!";
        textMesh.gameObject.SetActive(true);
        textMesh2.gameObject.SetActive(true);
        StartCoroutine(Drops());

        yield return new WaitForSeconds(3f);
        textMesh.gameObject.SetActive(false);
        textMesh2.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);

        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            postProcessVolume.weight = i;
            yield return null;
        }
        postProcessVolume.weight = 0;

        yield break;
    }

    private IEnumerator GTA()
    {
        yield break;
    }

    private IEnumerator Waktyhall()
    {
        yield break;
    }
    
    protected override IEnumerator Collapse()
    {        
        GameManager.Instance.StartCoroutine(GameManager.Instance.Ending());
        yield return base.Collapse();
    }
}
