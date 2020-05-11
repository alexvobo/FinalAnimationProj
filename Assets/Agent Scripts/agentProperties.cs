using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class agentProperties : MonoBehaviour
{
    public Boolean mask = false;
    public Boolean infected = false;
    // Start is called before the first frame update
    void Start()
    {
        GetMask();
        CheckInfected();
    }
    #region mask stuff
    private Boolean DeservesMask()
    {
        if (UnityEngine.Random.value > 0.7) //%30 percent chance
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void GetMask()
    {
        var tag = this.tag;
        if (CompareTag("Normal") || CompareTag("Infected"))
        {
            mask = DeservesMask();
        }
        else
        {
            mask = true;
        }

    }
    #endregion
    #region infected stuff
    public void CheckInfected()
    {
        if (CompareTag("Infected"))
        {
            infected = true;
        }
        else
        {
            infected = false;
        }
    }
    #endregion
    // Update is called once per frame
    void Update()
    {

    }
}
