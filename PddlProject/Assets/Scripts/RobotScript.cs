using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SimpleJSON;

public class RobotScript : MonoBehaviour {

    bool atLocation;
    bool readyForNewAction;
    Transform target;
    NavMeshAgent agent;
    public GameObject hand;
    public List<GameObject> targets = new List<GameObject>();
    int iterator = 0;
    public int maxIterations = 10;
    public GameObject desiredInteractable;

    JSONObject pddl = new JSONObject();
    public string functionName;
    public string parameter1;
    public string parameter2;
    public string parameter3;
    private string currentJsonFile ="";
    public bool moving;
    //string url = "http://solver.planning.domains/solve-and-validate";
    string url = "http://solver.planning.domains/solve";


    // Use this for initialization
    void Start () {
        StartCoroutine("Upload");
        agent = GetComponent<NavMeshAgent>();
        target = targets[0].GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update ()
    {        
       // Debug.Log(Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.transform.position.x, 0, target.transform.position.z)));
        if (checkIfAtLocation() == false && moving)
        {
            agent.destination = target.position;
        }
	}
    bool orderedToMove = false;
    bool checkIfAtLocation()
    {
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.transform.position.x, 0, target.transform.position.z)) == 0)
        {
            atLocation = true;
            moving = false;
            if (orderedToMove)
            {
                Debug.Log("MoveENDED");
                StartCoroutine("nextAction");
                orderedToMove = false;
            }
        }
        else
        {
            //readyForNewAction = false;
            atLocation = false;
            moving = true;
        }
        return atLocation;
    }

    public void moverobot()
    {
        foreach (GameObject t in targets)
        {
            if (t.name == parameter3)
            {
                target = t.GetComponent<Transform>();
                moving = true;
                orderedToMove = true;
            }
        }
    }

    public void pickupobject()
    {
        StartCoroutine("pickUp");
    }

    IEnumerator pickUp()
    {

        yield return new WaitForSeconds(3);

        Debug.Log("CALLED PICKUP");
        desiredInteractable.GetComponent<Interactable>().isGrabbed = true;
        desiredInteractable.transform.position = hand.transform.position;
        desiredInteractable.transform.parent = gameObject.transform;
        StartCoroutine("nextAction");
        
    }
    public void dropobject()
    {
        Debug.Log("CALLED dropobject");
        if(hand.GetComponentInChildren<GameObject>().name == parameter2)
        {
            hand.GetComponentInChildren<GameObject>().transform.position = target.transform.position;
            hand.GetComponentInChildren<GameObject>().transform.parent = null;
            hand.GetComponentInChildren<Interactable>().isGrabbed = false;
        }

        StartCoroutine("nextAction");
    }
    IEnumerator nextAction()
    {
        Debug.Log("NEXT ACTION");
        yield return new WaitForSeconds(3);
        iterator++;
        readyForNewAction = true;
        StartCoroutine(iteratable(currentJsonFile));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            if(other.name == parameter2)
            {
                desiredInteractable = other.gameObject;
            }
        }
    }




    public IEnumerator Upload()
    {
        var domain = Resources.Load<TextAsset>("UnityDomain3");
        var problem = Resources.Load<TextAsset>("UnityProblem3");
        /*
        WWWForm form = new WWWForm();
        form.AddField(domain.text, "domain");
        form.AddField(problem.text, "problem");

        UnityWebRequest www = UnityWebRequest.Post("http://solver.planning.domains/solve-and-validate", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        */
        //Debug.Log(domain.text);
        //Debug.Log(problem.text);

        using (WWW www = new WWW(url))
        {
            yield return www;
            WWWForm form = new WWWForm();
            form.AddField("problem", problem.text);
            form.AddField("domain", domain.text);
            WWW w = new WWW(url, form);
            yield return new WaitForSeconds(5);
            Debug.Log(w.text);
            currentJsonFile = w.text;
            StartCoroutine(iteratable(w.text));
            /*

            //JSONString s = (JSONString)JSONString.Parse(w.text);
            //JSONObject pddl = (JSONObject)JSONString.Parse(w.text);
            JSONNode node = JSON.Parse(w.text);
            // Debug.Log(node);
            //Debug.Log("node: " +node[1].ToString());
            JSONNode node2 = JSON.Parse(node[1].ToString());
            // Debug.Log("node2: " + node2[4].ToString());
            JSONNode node3 = JSON.Parse(node2[4].ToString());
            // Debug.Log("node3: " + node3[0].ToString());
            int i = 0;
            JSONNode node4 = JSON.Parse(node3[i].ToString());
            Debug.Log(node4[1]);
            string temp = node4[1];
            //  Debug.Log(node4["action"]);
            functionName = temp.Split(new char[] { ' ' })[0];
            functionName = functionName.Split(new char[] { '(' })[1];
            parameter1 = temp.Split(new char[] { ' ' })[1];
            parameter2 = temp.Split(new char[] { ' ' })[2];
            parameter3 = temp.Split(new char[] { ' ' })[3];
            parameter3 = parameter3.Split(new char[] { ')' })[0];

            StartCoroutine(functionName);
            yield return new WaitForSeconds(5);
            i++;
            node4 = JSON.Parse(node3[i].ToString());
            temp = node4[1];
            Debug.Log(temp);
            if(temp != "Null")
            {
                functionName = temp.Split(new char[] { ' ' })[0];
                functionName = functionName.Split(new char[] { '(' })[1];
                parameter1 = temp.Split(new char[] { ' ' })[1];
                parameter2 = temp.Split(new char[] { ' ' })[2];
                parameter3 = temp.Split(new char[] { ' ' })[3];
                parameter3 = parameter3.Split(new char[] { ')' })[0];
                StartCoroutine(functionName);
            }
            */
        }

    }

    public IEnumerator iteratable(string jsonFile)
    {
        Debug.Log(iterator);
        emptyParameters();
        JSONNode node = JSON.Parse(jsonFile);
        // Debug.Log(node);
        //Debug.Log("node: " +node[1].ToString());
        JSONNode node2 = JSON.Parse(node[1].ToString());
        // Debug.Log("node2: " + node2[4].ToString());
        JSONNode node3 = JSON.Parse(node2[4].ToString());
        // Debug.Log("node3: " + node3[0].ToString());

        JSONNode node4 = JSON.Parse(node3[iterator].ToString());
        
        Debug.Log(node4[1]);
        string temp = node4[1];
        if (node4[1] == null || node4[1] == "null" || iterator == maxIterations)
        {
            Debug.Log("END");
            yield break;
        }

        //  Debug.Log(node4["action"]);
        functionName = temp.Split(new char[] { ' ' })[0];
        functionName = functionName.Split(new char[] { '(' })[1];
        parameter1 = temp.Split(new char[] { ' ' })[1];
        parameter2 = temp.Split(new char[] { ' ' })[2];
        parameter3 = temp.Split(new char[] { ' ' })[3];
        parameter3 = parameter3.Split(new char[] { ')' })[0];

        StartCoroutine(functionName);
        /*
        yield return new WaitForSeconds(5);
        iterator++;
        StartCoroutine(iteratable(jsonFile));
        */
    }

    void emptyParameters()
    {
        functionName = "";
        parameter1 = "";
        parameter2 = "";
        parameter3 = "";
    }
}
