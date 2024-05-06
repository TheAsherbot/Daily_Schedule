using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

public class TimeField : TextField
{

    private static readonly char[] ALLOWED_CHARACTERS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ' ', 'p', 'a', 'm' }; 
    private static readonly char[] ALLOWED_CHARACTERS_FILTERED = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }; 


    public new class UxmlFactory : UxmlFactory<TimeField, UxmlTraits> { }

    

    public TimeField()
    {
        text = RemoveUnwantedCharacters(text, ALLOWED_CHARACTERS);
        RegisterCallback<InputEvent>(OnInput);
    }

    private void OnInput(InputEvent inputEvent)
    {
        Debug.Log("inputEvent.newData: " + inputEvent.newData);
        Debug.Log("inputEvent.previousData: " + inputEvent.previousData);
        text = RemoveUnwantedCharacters(text, ALLOWED_CHARACTERS);
    }


    private string RemoveUnwantedCharacters(string currentString, params char[] allowedCharacters)
    {
        return new string(currentString.Where(c => allowedCharacters.Contains(c)).ToArray());
    }

}
