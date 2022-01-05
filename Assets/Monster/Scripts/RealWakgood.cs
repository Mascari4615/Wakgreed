using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RealWakgood : BossMonster
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private TMP_Text textMesh2;
    [SerializeField] private GameObject icecream;
    [SerializeField] private LineRenderer lineRenderer;
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

    private Coroutine GTACO;
    private Coroutine TheShipCO;
    private Coroutine DropsCO;

    void Start()
    {
        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));

        wordIndexes = new List<int> { 0 };
        wordLengths = new List<int>();

        string s = textMesh.text;
        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths.Add(index - wordIndexes[^1]);
            wordIndexes.Add(index + 1);
        }
        wordLengths.Add(s.Length - wordIndexes[^1]);


        wordIndexes2 = new List<int> { 0 };
        wordLengths2 = new List<int>();
        s = textMesh2.text;
        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths2.Add(index - wordIndexes2[^1]);
            wordIndexes2.Add(index + 1);
        }
        wordLengths2.Add(s.Length - wordIndexes2[^1]);
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

    protected override void OnEnable()
    {
        if (DropsCO != null) StopCoroutine(DropsCO);
        if (TheShipCO != null) StopCoroutine(TheShipCO);
        if (GTACO != null) StopCoroutine(GTACO);

        if (StageManager.Instance.CurrentRoom != null)
        {
            if (StageManager.Instance.CurrentRoom is BossRoom)
            {
                if (StageManager.Instance.currentStage.id == 666)
                {
                    AudioManager.Instance.PlayRealMusic();
                }
            }
        }

        base.OnEnable();
    }

    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f * 3), Mathf.Cos(time * 2.5f * 3));
    }

    protected override IEnumerator Attack()
    {
        textMesh.text = "모두 '!드롭스' 를 외쳐주세요!";
        textMesh.gameObject.SetActive(true);
        DropsCO = StartCoroutine(Drops());

        yield return new WaitForSeconds(3f);
        textMesh.gameObject.SetActive(false);

        while (true)
        {
            int i = Random.Range(0, 2 + 1);
            switch (i)
            {
                case 0:
                    yield return TheShipCO = StartCoroutine(TheShip());
                    break;
                case 1:
                    yield return GTACO = StartCoroutine(GTA());
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
            Vector3 randomPos = Wakgood.Instance.transform.position + (Vector3)Random.insideUnitCircle * Random.Range(0, 20f);
            ObjectManager.Instance.PopObject("Drop", randomPos);
            yield return new WaitForSeconds(Random.Range(.1f, 1.5f));
        }
    }

    private IEnumerator TheShip()
    {
        textMesh.text = "오뱅내는 하루종일 '더 쉽' 입니다.";
        textMesh2.text = "왁인마를 조심하세요!";
        textMesh.gameObject.SetActive(true);
        textMesh2.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        textMesh.gameObject.SetActive(false);
        textMesh2.gameObject.SetActive(false);
        cinemachineTargetGroup.m_Targets[1].target = null;

        Animator.SetTrigger("THE");
        transform.position = Wakgood.Instance.transform.position + (Vector3)Random.insideUnitCircle.normalized * 25;
        for (float i = 0; i <= 5; i += Time.deltaTime)
        {
            Rigidbody2D.velocity = (Wakgood.Instance.transform.position - transform.position).normalized * (MoveSpeed += Time.deltaTime * 3f);
            yield return null;
        }
        Animator.SetTrigger("SHIP");
        Vector3 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
        Vector3 rot = new(0, 0, Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 0.8f), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90);
        for (int i = 0; i < 3; i++)
        {
            ObjectManager.Instance.PopObject("PanchiSlash", transform.position + Vector3.up * 0.8f + direction * 1.5f, rot);
            yield return new WaitForSeconds(.3f);
        }
        MoveSpeed = 5;
        cinemachineTargetGroup.m_Targets[1].target = transform;
        yield break;
    }

    private IEnumerator GTA()
    {
        textMesh.text = "오뱅내는 하루종일 '그 타' 입니다.";
        textMesh2.text = "빠르게 다가오는 자동차들을 조심하세요!";
        textMesh.gameObject.SetActive(true);
        textMesh2.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);
        textMesh.gameObject.SetActive(false);
        textMesh2.gameObject.SetActive(false);

        lineRenderer.positionCount = 2;
        int rand = Random.Range(6, 8 + 1);
        for (int i = 0; i < rand; i++)
        {
            Vector3 pos1 = Wakgood.Instance.transform.position + ((Vector3)Random.insideUnitCircle * 10f).normalized * 100; 
            Vector3 pos2 = Wakgood.Instance.transform.position + (Wakgood.Instance.transform.position - pos1).normalized * 100;

            lineRenderer.SetPosition(0, pos1);
            lineRenderer.SetPosition(1, pos2);
            lineRenderer.gameObject.SetActive(true);

            yield return new WaitForSeconds(i == 0 ? 1f : .5f);
            lineRenderer.gameObject.SetActive(false);

            var temp = Instantiate(icecream, pos1 + (pos2 - pos1) / 2, Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(pos2.y - pos1.y, pos2.x - pos1.x))));
            temp.transform.localScale = new Vector3(Vector3.Distance(pos1, pos2) * 0.25f, temp.transform.localScale.y, 1);

            yield return new WaitForSeconds(.3f);
        }

        yield return new WaitForSeconds(3f);
    }

    private IEnumerator Waktyhall()
    {
        yield break;
    }
}
