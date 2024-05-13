using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public static EnemyController CurrentController;

    [SerializeField] SizeDefinition size;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject animPrefab;

    private void Start()
    {
        CurrentController = this;

        deadCopies = new List<Vector2Int>();
        currentScreenPos = new Vector2Int();
        copies = new GameObject[3][];

        for (int i = -1; i <= 1; i++)
        {

            copies[i + 1] = new GameObject[3];

            for (int j = -1; j <= 1; j++)
            {
                Vector2 pos = transform.position;

                pos.x += i * size.width;
                pos.y += j * size.height;

                //Spawn copies
                GameObject copy = Instantiate(enemyPrefab, pos, Quaternion.identity, transform);

                EnemyInstance inst = copy.GetComponent<EnemyInstance>();

                inst.x = i;
                inst.y = j;

                copies[i + 1][j + 1] = copy;
            }
        }

    }

    GameObject[][] copies;

    List<Vector2Int> deadCopies;

    Vector2Int currentScreenPos;


    public void wrapAroundX (bool positive)
    {

        currentScreenPos.x += (positive ? 1 : -1);
        

        updateCopies();
    }

    public void wrapAroundY (bool positive)
    {

        currentScreenPos.x += (positive ? 1 : -1);
        

        updateCopies();
    }

    private void updateCopies ()
    {

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {

                Vector2Int checkPos = currentScreenPos;
                checkPos.x += i;
                checkPos.y += j;

                copies[i + 1][j + 1].SetActive(!deadCopies.Contains(checkPos));

            }
        }

    }

    public void KillCopy (int x, int y)
    {
        //TODO only play sound / particle when it makes sense
        AudioManager.instance.PlaySound("die");
        GameObject breaking = Instantiate(animPrefab, copies[x + 1][y + 1].transform.position, Quaternion.identity);

        breaking.GetComponent<BreakEffect>().playEffect(transform.lossyScale);

        Vector2Int killed = currentScreenPos;

        killed.x += x;
        killed.y += y;

        deadCopies.Add(killed);

        updateCopies();
    }
}
