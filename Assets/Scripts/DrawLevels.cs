using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLevels : MonoBehaviour
{
    [SerializeField]
    Transform player = null;
    [SerializeField]
    float groundLevel = 2.97f;

    static GameObject water;
    static GameObject firstPath;
    static GameObject secondPath;
    static GameObject thirdPath;
    static GameObject firstBridge;
    static GameObject secondBridge;
    static GameObject background;

    GameObject firstTypeEnemy;

    int pathsLoaded = 0;
    int bridgesLoaded = 0;
    int backgroundsLoaded = 0;

    [SerializeField]
    GameObject[] enemy = null;

    float pathSize;
    float bgSize;

    float nextPath = 0;
    float nextBG = 0; 


    LinkedList<GameObject> paths;
    LinkedList<GameObject> bridges;
    LinkedList<GameObject> backgrounds;
    LinkedList<GameObject> waters;

    private void Start()
    {
        waters = new LinkedList<GameObject>();
        paths = new LinkedList<GameObject>();
        bridges = new LinkedList<GameObject>();
        backgrounds = new LinkedList<GameObject>();

        water = Resources.Load<GameObject>("Prefabs/Water");
        firstPath = Resources.Load<GameObject>("Prefabs/FirstPath");
        secondPath = Resources.Load<GameObject>("Prefabs/SecondPath");
        thirdPath = Resources.Load<GameObject>("Prefabs/ThirdPath");

        firstBridge = Resources.Load<GameObject>("Prefabs/FirstBridge");
        secondBridge = Resources.Load<GameObject>("Prefabs/SecondBridge");

        firstTypeEnemy = Resources.Load<GameObject>("Prefabs/Enemy1");

        background = Resources.Load<GameObject>("Prefabs/Background"); 

        loadNextPaths();
        loadNextWaters();
        loadNextBridge();
        loadNextBackground();

        pathSize = firstPath.GetComponent<SpriteRenderer>().bounds.size.x- 0.1f; 
        bgSize = background.GetComponent<SpriteRenderer>().bounds.size.x;

        nextPath += pathSize;
        nextBG += bgSize;
    }

    void Update()
    {
        if (player.position.x >= nextPath - pathSize*2)
        {
            loadNextPaths();
            loadNextWaters();
            loadNextBridge();
            nextPath += pathSize;
        }

        if(player.position.x >= nextBG - bgSize)
        {
            loadNextBackground();
            nextBG += bgSize;
        }

        if (bridgesLoaded >=14)
        {
            destroyLateBridge();
        }

        if(pathsLoaded >=5)
        {
            destroyLatePaths();
            destroyLateWaters();
        }

        if (backgroundsLoaded >= 3)
        {
            destroyLateBackground();
        }
    }

    private void loadNextPaths()
    {
        pathsLoaded++;

        Vector3 addDistance = new Vector3(nextPath, 0, 0);

        paths.AddLast(Instantiate(firstPath));
        paths.Last.Value.transform.Translate(addDistance);
        paths.AddLast(Instantiate(secondPath));
        paths.Last.Value.transform.Translate(addDistance);
        paths.AddLast(Instantiate(thirdPath));
        paths.Last.Value.transform.Translate(addDistance);
    }

    private void loadNextWaters() // Water are loaded at the same time than path
    {
        Vector3 addDistance = new Vector3(nextPath, 0, 0);

        waters.AddLast(Instantiate(water));
        waters.Last.Value.transform.Translate(addDistance);
        addDistance.z = secondPath.transform.position.z;
        waters.AddLast(Instantiate(water));
        waters.Last.Value.transform.Translate(addDistance);
        addDistance.z = thirdPath.transform.position.z;
        waters.AddLast(Instantiate(water));
        waters.Last.Value.transform.Translate(addDistance);
    }

    private void destroyLateWaters()
    {
        Destroy(waters.First.Value);
        waters.RemoveFirst();
        Destroy(waters.First.Value);
        waters.RemoveFirst();
        Destroy(waters.First.Value);
        waters.RemoveFirst();
    }

    private void destroyLatePaths()
    {
        pathsLoaded--;

        Destroy(paths.First.Value);
        paths.RemoveFirst();
        Destroy(paths.First.Value);
        paths.RemoveFirst();
        Destroy(paths.First.Value);
        paths.RemoveFirst();
    }

    private void loadNextBridge()
    {
        Vector3 addDistance = new Vector3(nextPath, 0, 0);

        bridgesLoaded += 4;

        bridges.AddLast(Instantiate(secondBridge));
        bridges.Last.Value.transform.Translate(addDistance);
        bridges.AddLast(Instantiate(firstBridge));
        bridges.Last.Value.transform.Translate(addDistance);

        addDistance.x = nextPath + (pathSize / 2);

        bridges.AddLast(Instantiate(secondBridge));
        bridges.Last.Value.transform.Translate(addDistance);
        bridges.AddLast(Instantiate(firstBridge));
        bridges.Last.Value.transform.Translate(addDistance);

    }

    private void destroyLateBridge()
    {
        bridgesLoaded-=2;

        Destroy(bridges.First.Value);
        bridges.RemoveFirst();
        Destroy(bridges.First.Value);
        bridges.RemoveFirst();
    }

    public void LoadEnemy(int path, bool lying) // Enemies selfdestruct
    {
        if (lying)
        {
            if (path == 2)
            {
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, firstPath.transform.position.z), Quaternion.identity);
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, secondPath.transform.position.z), Quaternion.identity);
            } else if (path == 1)
            {
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, firstPath.transform.position.z), Quaternion.identity);
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, thirdPath.transform.position.z), Quaternion.identity);
            } else
            {
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, secondPath.transform.position.z), Quaternion.identity);
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, thirdPath.transform.position.z), Quaternion.identity);
            }
        } else
        {
            if (path == 0)
            {
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, firstPath.transform.position.z), Quaternion.identity);
            } else if (path == 1)
            {
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, secondPath.transform.position.z), Quaternion.identity);
            } else
            {
                Instantiate(RandomEnemy(), new Vector3(player.transform.position.x + 90, groundLevel, thirdPath.transform.position.z), Quaternion.identity);
            }
        }
    }

    private void loadNextBackground()
    {
        backgroundsLoaded++;
        backgrounds.AddLast(Instantiate(background));
        backgrounds.Last.Value.transform.Translate(new Vector3(nextBG, 0, 0));
    }

    private void destroyLateBackground()
    {
        backgroundsLoaded--;
        Destroy(backgrounds.First.Value);
        backgrounds.RemoveFirst();
    }

    public GameObject RandomEnemy()
    {
        int enemyChoosed = Random.Range(0, enemy.Length);

        return enemy[enemyChoosed];
    }
}