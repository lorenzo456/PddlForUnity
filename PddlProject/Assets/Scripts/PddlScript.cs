using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class PddlScript : MonoBehaviour {

    JSONObject pddl = new JSONObject();
    public string functionName;
    public string parameter1;
    public string parameter2;
    public string parameter3;

	// Use this for initialization
	void Start () {
        StartCoroutine("Upload");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public string url = "http://solver.planning.domains/solve-and-validate";

    public IEnumerator Upload()
    {
        var domain = Resources.Load<TextAsset>("UnityDomain");
        var problem = Resources.Load<TextAsset>("UnityProblem");
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
        Debug.Log(domain.text);
        Debug.Log(problem.text);
        
        using (WWW www = new WWW(url))
        {
            yield return www;
            WWWForm form = new WWWForm();
            form.AddField("domain", domain.text);
            form.AddField("problem", problem.text);
            WWW w = new WWW(url,form);
            yield return new WaitForSeconds(5);
            Debug.Log(w.text);  
            //JSONString s = (JSONString)JSONString.Parse(w.text);
            //JSONObject pddl = (JSONObject)JSONString.Parse(w.text);
            JSONNode node = JSON.Parse(w.text);
           // Debug.Log(node);
            //Debug.Log("node: " +node[1].ToString());
            JSONNode node2 = JSON.Parse(node[1].ToString());
           // Debug.Log("node2: " + node2[4].ToString());
            JSONNode node3 = JSON.Parse(node2[4].ToString());
           // Debug.Log("node3: " + node3[0].ToString());
            JSONNode node4 = JSON.Parse(node3[0].ToString());
            Debug.Log(node4[1]);
            string temp = node4[1];
            //  Debug.Log(node4["action"]);
            functionName = temp.Split(new char[] {' '})[0];
            functionName = functionName.Split(new char[] { '(' })[1];
            parameter1 = temp.Split(new char[] { ' ' })[1];
            parameter2 = temp.Split(new char[] { ' ' })[2];
            parameter3 = temp.Split(new char[] { ' ' })[3];
            parameter3 = parameter3.Split(new char[] { ')' })[0];

            Debug.Log(functionName);
            Debug.Log(parameter1);
            Debug.Log(parameter2);
            Debug.Log(parameter3);
        }

    }
}
