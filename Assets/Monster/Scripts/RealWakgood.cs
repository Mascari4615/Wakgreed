using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RealWakgood : BossMonster
{
    [SerializeField] private TMP_Text textComponent;

    private void Update()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; ++j)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 50f, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
    protected override IEnumerator Attack()
    {

        StartCoroutine(Drops());
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

        }

        yield break;
    }

    private IEnumerator TheShip()
    {
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
