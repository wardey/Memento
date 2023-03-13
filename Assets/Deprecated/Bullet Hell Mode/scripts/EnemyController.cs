using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// contains the bullet patterns that can be used
/// need to make new patterns
/// </summary>
public class EnemyController : MonoBehaviour {

    public GameObject player;
    public Vector3 initialPos;
    public List<GameObject> bullets = new List<GameObject>();
    public GameObject ireliaBlade;
    public List<GameObject> ireliaPatternSpawn = new List<GameObject>();

    //private string sequenceFile;
    //private List<string> fileContents = new List<string>();
    //private int fileLength;
    //private int currindex;

    //given right is (1,0,0), going clockwise, so down is (0,0,-1), left is (-1,0,0), up is (0,0,1)
    private List<Vector3> degrees = new List<Vector3>();
    private Vector3 Deg15 = new Vector3(0.966f, 0.0f, -0.259f);  //-Deg195
    private Vector3 Deg30 = new Vector3(Mathf.Sqrt(3)/2, 0.0f, -0.5f);  //-Deg210, and so on...
    private Vector3 Deg45 = new Vector3(Mathf.Sqrt(2)/2, 0.0f,-Mathf.Sqrt(2) / 2);
    private Vector3 Deg60 = new Vector3(0.5f, 0.0f,-Mathf.Sqrt(3) / 2);
    private Vector3 Deg75 = new Vector3(0.259f, 0.0f, -0.966f);
    private Vector3 Deg105 = new Vector3(-0.259f, 0.0f, -0.966f);
    private Vector3 Deg120 = new Vector3(-0.5f, 0.0f, -Mathf.Sqrt(3) / 2);
    private Vector3 Deg135 = new Vector3(-Mathf.Sqrt(2) / 2, 0.0f, -Mathf.Sqrt(2) / 2);
    private Vector3 Deg150 = new Vector3(-Mathf.Sqrt(3) / 2, 0.0f, -0.5f);
    private Vector3 Deg165 = new Vector3(-0.966f, 0.0f, -0.259f);
    //
    //bullets now continuously fly forward, so need to change the z value of its rotation to determine its trajectory, -90 is left, 0 is down, 90 is right
    //
    private float leftBound;
    private float rightBound;
    private bool left;
    public bool active;

    public List<GameObject> optionObjects = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //currindex = 0;  //start the file from line 0(1)
        degrees.Add(Vector3.right);
        degrees.Add(Deg15);
        degrees.Add(Deg30);
        degrees.Add(Deg45);
        degrees.Add(Deg60);
        degrees.Add(Deg75);
        degrees.Add(Vector3.back);
        degrees.Add(Deg105);
        degrees.Add(Deg120);
        degrees.Add(Deg135);
        degrees.Add(Deg150);
        degrees.Add(Deg165);
        degrees.Add(Vector3.left);
        //readFile();
        leftBound = transform.position.x - 2.5f;
        rightBound = transform.position.x + 2.5f;
        left = Random.Range(0, 2) == 0;
        //StartCoroutine("TimedSequence");
	}

    void Update()
    {
        if (!active) return;
        if (left)
        {
            transform.position += Time.deltaTime * Vector3.left;
            if (transform.position.x < leftBound)
            {
                left = false;
            }
        }
        else
        {
            transform.position += Time.deltaTime * Vector3.right;
            if (transform.position.x > rightBound)
            {
                left = true;
            }
        }
    }

    public void center()    //centers the player on the screen
    {
        active = false;
        StartCoroutine(returntocenter());
        left = Random.Range(0, 2) == 0;
    }

    IEnumerator returntocenter()
    {
        //Debug.Log(initialPos);
        //transform.localPosition = initialPos;
        //yield return null;
        while (transform.position != initialPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPos, 0.1f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    //file reading is done in logic frenzy controller
/*    public void setSequenceFile(string filename)    //sets the file for instance of logic frenzy
    {
        sequenceFile = filename;
        DialogueParser.instance.LoadDialogue(sequenceFile);
        fileContents = DialogueParser.instance.getLines();
        fileLength = fileContents.Count;
        readfile();
    }
    */
    /// <summary>
    /// section for pattern calls for controller
    /// </summary>

    public void semicircle(int bulletType)   //aoe semicircle down
    {
        for(int i = -90; i <= 90; i += 10)
        {
            CreateProjectile(transform.position, bullets[bulletType - 1], i);
        }
    }

    public void rightspiral(int bulletType)   //spin a few times while shooting (not literally spinning, but spiral pattern)
    {
        StartCoroutine(rightspiralPattern(bulletType));
    }

    public void leftspiral(int bulletType)   //spin a few times while shooting (not literally spinning, but spiral pattern)
    {
        StartCoroutine(leftspiralPattern(bulletType));
    }

    public void fullspiral(int bulletType)
    {
        StartCoroutine(fullSpiralPattern(bulletType));
    }

    public void fan(int bulletType)  //5 in a fan shape
    {
        CreateProjectile(transform.position, bullets[bulletType - 1], -30);
        CreateProjectile(transform.position, bullets[bulletType - 1], -20);
        CreateProjectile(transform.position, bullets[bulletType - 1], -10);
        CreateProjectile(transform.position, bullets[bulletType - 1], 0);
        CreateProjectile(transform.position, bullets[bulletType - 1], 10);
        CreateProjectile(transform.position, bullets[bulletType - 1], 20);
        CreateProjectile(transform.position, bullets[bulletType - 1], 30);
    }

    public void targetBurst(int bulletType)  // shoots 5 aimed at the player, not right, shoots straight down for some reason
    {
        GameObject tmp = Instantiate(bullets[0], transform.position, transform.rotation);
        tmp.transform.LookAt(player.transform.position);
        StartCoroutine(burstShot(transform.position, bulletType, tmp.transform.eulerAngles.z));
    }

    public void optionBurst(int bulletType)
    {
        StartCoroutine(optionBurstPattern(bulletType));
    }

    public void irelia()
    {
        StartCoroutine(ireliaPattern());
    }

    public void custom(Vector3 pos, int bulletType, float zValue)
    {
        CreateProjectile(pos, bullets[bulletType - 1], zValue);
    }

    GameObject CreateProjectile(Vector3 position, GameObject bulletType, float z)
    {
        GameObject tmp = Instantiate(bulletType);
        tmp.transform.position = position;
        tmp.transform.eulerAngles = new Vector3(tmp.transform.eulerAngles.x, tmp.transform.eulerAngles.y, z);
        Bullet bullet = tmp.GetComponent<Bullet>();
        tmp.SetActive(true);
        return tmp;
    }

    IEnumerator delay(float time)
    {
        yield return new WaitForSeconds(time);
        //do something
    }

    IEnumerator TimedSequence()
    {
        while (true)
        {
            irelia();
            semicircle(1);
            yield return new WaitForSeconds(0.5f);
            targetBurst(3);
            yield return new WaitForSeconds(0.5f);
            fan(3);
            //semicircle(2);
            yield return new WaitForSeconds(1.0f);
            targetBurst(3);
            yield return new WaitForSeconds(0.5f);
            fan(1);
            //semicircle(2);
            yield return new WaitForSeconds(0.5f);
            targetBurst(3);
            yield return new WaitForSeconds(0.5f);
            optionBurst(1);
            yield return new WaitForSeconds(1f);
            fan(3);
            semicircle(1);
            rightspiral(3);
            yield return new WaitForSeconds(1.5f);
            targetBurst(3);
            yield return new WaitForSeconds(0.5f);
            semicircle(2);
            yield return new WaitForSeconds(0.5f);
            fan(1);
            semicircle(1);
            yield return new WaitForSeconds(2.0f);
            leftspiral(3);
            yield return new WaitForSeconds(0.5f);
            rightspiral(3);
            yield return new WaitForSeconds(2.0f);
        }
    }

    /// actual implementation of patterns

    IEnumerator burstShot(Vector3 pos, int bulletType, float z)
    {
        for (int i = 0; i < 5; i++)
        {
            CreateProjectile(pos, bullets[bulletType - 1], z);
            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator fullSpiralPattern(int bulletType)
    {
        StartCoroutine(rightspiralPattern(bulletType));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(leftspiralPattern(bulletType));
    }

    IEnumerator rightspiralPattern(int bulletType)
    {
        for(int j = 0; j <= 90; j += 10)
        {
            CreateProjectile(transform.position, bullets[bulletType - 1], j);
            CreateProjectile(transform.position, bullets[bulletType - 1], j + 90);
            CreateProjectile(transform.position, bullets[bulletType - 1], j + 180);
            CreateProjectile(transform.position, bullets[bulletType - 1], j + 270);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator leftspiralPattern(int bulletType)
    {
        for (int j = 45; j >= -45; j -= 10)
        {
            CreateProjectile(transform.position, bullets[bulletType - 1], j);
            CreateProjectile(transform.position, bullets[bulletType - 1], j + 90);
            CreateProjectile(transform.position, bullets[bulletType - 1], j + 180);
            CreateProjectile(transform.position, bullets[bulletType - 1], j + 270);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator optionBurstPattern(int bulletType)
    {
        for(int i = 0; i < optionObjects.Count; i++)
        {
            for(int j = i*5; j < 360; j +=30)
            {
                CreateProjectile(optionObjects[i].transform.position, bullets[bulletType - 1], j);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator ireliaPattern()
    {
        for(int i = 0; i < 4; i++)
        {
            GameObject tmp = Instantiate(bullets[0], ireliaPatternSpawn[i].transform);
            tmp.transform.localPosition = Vector3.zero;
            tmp.transform.LookAt(player.transform.position);
            GameObject blade = CreateProjectile(tmp.transform.position, ireliaBlade, 180 -tmp.transform.eulerAngles.y);
            StartCoroutine(blade.GetComponent<Bullet>().delayStart(0.15f));
            Destroy(tmp);
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }
}
