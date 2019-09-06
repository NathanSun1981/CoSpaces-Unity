using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TablePlus.ElementsDB.DBBridge;
using TablePlus.ElementsDB.DBBridge.Extra;
using TablePlus.ElementsDB.DBBridge.Extra.ResourceHandlers;
using UnityEngine.UI;

public class ElementDBConnector : MonoBehaviour
{
    // Start is called before the first frame update

    public Text text;
    void Start()
    {
        ResourceHandler.Instance.Handlers.Clear();
        ResourceHandler.Instance.RegisterHandler(new KeyPhotoHandler());
        ResourceHandler.Instance.RegisterHandler(new IsoModelHandler());
        ResourceHandler.Instance.RegisterHandler(new MassingDaeGenerator());
        ResourceHandler.Instance.RegisterHandler(new HDDaeGenerator());
        StartCoroutine(Resourcehaddlling());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Resourcehaddlling()
    {
        // load all cases

        List<DBCase> cases = Elements.ListAllCasesSummary();
        yield return null;

        int count = 0;
        foreach (DBCase c in cases)
        {
            count++;
            text.text = string.Format("Loading Cases ({0} of {1})...", count, cases.Count);
            DBCase cc = Elements.GetCaseByID(c._CaseId);

            string warehoused = "";
            cc.Properties.TryGetValue("is_warehoused", out warehoused);
            if (warehoused == "1")
                continue;
            ResourceHandler.Instance.HandleCase(cc, cc.ResourceList, "./Cache",
                "case-resources", cc._CaseNumber, false);
            yield return null;
        }

    }

}
