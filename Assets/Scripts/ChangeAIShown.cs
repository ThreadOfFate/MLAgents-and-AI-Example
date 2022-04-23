using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeAIShown : MonoBehaviour
{

    [SerializeField] private Transform[] _agents;
    [SerializeField] private Text _text;
    private int _currentAI;
    // Start is called before the first frame update
    private void Start()
    {
        SetActiveAgent();
    }

    public void NextAgent()
    {
        _currentAI = (_currentAI + 1) % _agents.Length;
        SetActiveAgent();
    }
    
    public void LastAgent()
    {
        _currentAI = (_currentAI - 1) % _agents.Length;
        SetActiveAgent();
    }

    private void SetActiveAgent()
    {
        _text.text = _agents[_currentAI].name;
        foreach (var agent in _agents)
        {
            agent.gameObject.SetActive(false);
        }
        _agents[_currentAI].gameObject.SetActive(true);
    }
}
