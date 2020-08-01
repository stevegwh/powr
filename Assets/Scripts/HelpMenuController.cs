using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenuController
{

    private string[] _helpText = new[]
    {
        "Welcome! To play the game you\n will need to 'import' at least\n one 'crate' item and one 'door'.",
        "To 'import' them you will need\n to click the 'A' button on the bottom\n left corner of the object and then\n the top right corner.",
        "It cannot be the other way around.\n Bottom left and then top right.",
        "This will then draw a plane that\n is a placeholder for the front\n face of your object.",
        "Run your hand along the edges\n to make sure it has lined up\n correctly.",
        "A crate is simply the front face\n of a low-down horizontal object.\n E.g. a bed or couch.",
        " To import a door you should click\n on the bottom left of the inner frame\n and the top right of the inner frame."
    };

    private int _idx = 0;

    private void Increment()
    {
        _idx = _idx + 1 == _helpText.Length ? 0 : _idx + 1;
    }
    private void Decrement()
    {
        _idx = _idx - 1 < 0 ? _helpText.Length - 1 : _idx - 1;
    }

    public string FirstMessage()
    {
        _idx = 0;
        return _helpText[0];
    }

    public string NextMessage()
    {
        Increment();
        return _helpText[_idx];
    }
    public string PreviousMessage()
    {
        Decrement();
        return _helpText[_idx];
    }

}
