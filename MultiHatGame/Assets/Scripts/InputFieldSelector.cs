using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldSelector : MonoBehaviour
{
    [SerializeField] private List<TMP_InputField> _inputFields;
    
    private int _inputFieldNumber;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _inputFieldNumber++;
            
            if (_inputFieldNumber > 1) _inputFieldNumber = 0;
            SelectInputField();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKeyDown(KeyCode.LeftShift))
        {
            _inputFieldNumber--;
            
            if (_inputFieldNumber < 0) _inputFieldNumber = 1;
            SelectInputField();
        }
    }

    private void SelectInputField()
    {
        switch (_inputFieldNumber)
        {
            case 0: _inputFields[0].Select();
                break;
            case 1: _inputFields[1].Select();
                break;
        }
    }
}
