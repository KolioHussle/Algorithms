using System.Collections;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public RectInt room = new RectInt(0, 0, 100, 50);

    void Start()
    {
        StartCoroutine(GenerateDungeon());
    }

    void Update()
    {

    }

    private IEnumerator GenerateDungeon()
    {
        yield return new WaitForSeconds(1f);
        AlgorithmsUtils.DebugRectInt(room, Color.yellow);
    }
}
