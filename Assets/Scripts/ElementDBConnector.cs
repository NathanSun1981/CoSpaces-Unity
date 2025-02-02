﻿using System.Collections;
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
    private int currentIndex;
    private int indexs;
    private bool bStop = false;

    void Start()
    {
        ResourceHandler.Instance.Handlers.Clear();
        ResourceHandler.Instance.RegisterHandler(new KeyPhotoHandler());
        ResourceHandler.Instance.RegisterHandler(new IsoModelHandler());
        ResourceHandler.Instance.RegisterHandler(new MassingDaeGenerator());
        ResourceHandler.Instance.RegisterHandler(new HDDaeGenerator());
    }

    // Update is called once per frame
    void Update()
    {
        if (bStop)
            text.text = string.Format("Processing Stop at Case {0}", currentIndex);
        else
            text.text = string.Format("Loading Cases ({0} of {1})...", currentIndex, indexs);
    }

    private IEnumerator Resourcehaddlling()
    {
        // load all cases

        List<DBCase> cases = Elements.ListAllCasesSummary();
        yield return null;

        int count = 0;
        foreach (DBCase c in cases)
        {
            if (!bStop)
            {
                count++;
                currentIndex = count;
                indexs = cases.Count;

           
                DBCase cc = Elements.GetCaseByID(c._CaseId);

                string warehoused = "";
                cc.Properties.TryGetValue("is_warehoused", out warehoused);
                if (warehoused == "1")
                    continue;
                ResourceHandler.Instance.HandleCase(cc, cc.ResourceList, "./Cache",
                    "case-resources", cc._CaseNumber, false);
            }          
            yield return null;
        }

    }

    public void OnClickDownloading()
    {
        bStop = false;
        StartCoroutine(Resourcehaddlling());
    }


    public void OnClickExit()
    {
        StopCoroutine(Resourcehaddlling());
        Application.Quit();
    }

    public void OnClickStop()
    {
        bStop = true;
        StopCoroutine(Resourcehaddlling());
    }
}
