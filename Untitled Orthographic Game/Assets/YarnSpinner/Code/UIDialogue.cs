using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : Yarn.Unity.DialogueUIBehaviour {

    /// The UI element that displays lines
    public Text defaultLineText;

    /// A UI element that appears after lines have finished appearing
    public GameObject continuePrompt;

    /// A delegate (ie a function-stored-in-a-variable) that
    /// we call to tell the dialogue system about what option
    /// the user selected
    private Yarn.OptionChooser SetSelectedOption;

    /// How quickly to show the text, in seconds per character
    [Tooltip("How quickly to show the text, in seconds per character")]
    public float textSpeed = 0.025f;

    private bool isWaitingToContinue = false;
    private bool continueDialogue = false;

    /// The buttons that let the user choose an option
    public List<Button> optionButtons;

    public DialogueVariableStorage variableStorage;

    void Awake() {
        defaultLineText?.gameObject.SetActive(false);

        foreach (var button in optionButtons) {
            button.gameObject.SetActive(false);
        }

        // Hide the continue prompt if it exists
        if (continuePrompt != null) {
            continuePrompt.SetActive(false);
        }
    }

    /// Show a line of dialogue, gradually
    public override IEnumerator RunLine(Yarn.Line line, Text lineText) {
        if (lineText == null) {
            lineText = defaultLineText;
        }

        // Replaces variables with their values.
        string moddedLine = CheckVars(line.text);

        // Show the text
        lineText.gameObject.SetActive(true);

        if (textSpeed > 0.0f) {
            // Display the line one character at a time
            var stringBuilder = new StringBuilder();

            foreach (char c in moddedLine) {
                stringBuilder.Append(c);
                lineText.text = stringBuilder.ToString();
                yield return new WaitForSeconds(textSpeed);
            }
        } else {
            // Display the line immediately if textSpeed == 0
            lineText.text = moddedLine;
        }

        // Show the 'press any key' prompt when done, if we have one
        if (continuePrompt != null) {
            continuePrompt.SetActive(true);
        }

        // Wait for any user input
        isWaitingToContinue = true;
        while (continueDialogue == false) {
            yield return null;
        }
        continueDialogue = false;
        isWaitingToContinue = false;

        // Hide the text and prompt
        lineText.gameObject.SetActive(false);

        if (continuePrompt != null) {
            continuePrompt.SetActive(false);
        }
    }

    /// Show a list of options, and wait for the player to make a selection.
    public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                            Yarn.OptionChooser optionChooser) {
        // Do a little bit of safety checking
        if (optionsCollection.options.Count > optionButtons.Count) {
            Debug.LogWarning("There are more options to present than there are" +
                                "buttons to present them in. This will cause problems.");
        }

        MouseStateController.instance?.SetMouseState(true, false);

        // Display each option in a button, and make it visible
        int i = 0;
        foreach (var optionString in optionsCollection.options) {
            optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].GetComponentInChildren<Text>().text = optionString;
            i++;
        }

        // Record that we're using it
        SetSelectedOption = optionChooser;

        // Wait until the chooser has been used and then removed (see SetOption below)
        while (SetSelectedOption != null) {
            yield return null;
        }

        MouseStateController.instance?.SetMouseState(false, false);

        // Hide all the buttons
        foreach (var button in optionButtons) {
            button.gameObject.SetActive(false);
        }
    }

    /// Called by buttons to make a selection.
    public void SetOption(int selectedOption) {

        // Call the delegate to tell the dialogue system that we've
        // selected an option.
        SetSelectedOption(selectedOption);

        // Now remove the delegate so that the loop in RunOptions will exit
        SetSelectedOption = null;
    }

    /// Run an internal command.
    public override IEnumerator RunCommand(Yarn.Command command) {
        // "Perform" the command
        Debug.Log("Command: " + command.text);

        yield break;
    }

    /// Called when the dialogue system has started running.
    public override IEnumerator DialogueStarted() {
        Debug.Log("Dialogue starting!");

        // Enables the in-game mouse.
        //MouseStateController.instance?.SetUIMouse(true);

        yield break;
    }

    public void DialogueContinue() {
        if (isWaitingToContinue) {
            continueDialogue = true;
        }
    }

    /// Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete() {
        Debug.Log("Complete!");

        // Disables the in-game mouse.
        //MouseStateController.instance?.SetUIMouse(false);

        yield break;
    }

    private string CheckVars(string input) {
        string output = string.Empty;
        bool checkingVar = false;
        string currentVar = string.Empty;

        int index = 0;
        while (index < input.Length) {
            if (input[index] == '[') {
                checkingVar = true;
                currentVar = string.Empty;
            } else if (input[index] == ']') {
                checkingVar = false;
                output += ParseVariable(currentVar);
                currentVar = string.Empty;
            } else if (checkingVar) {
                currentVar += input[index];
            } else {
                output += input[index];
            }
            index += 1;
        }

        return output;
    }

    private string ParseVariable(string varName) {
        //Check YarnSpinner's variable storage first
        if (variableStorage.GetValue(varName) != Yarn.Value.NULL) {
            return variableStorage.GetValue(varName).AsString;
        }

        //Handle other variables here
        if (varName == "$time") {
            return Time.time.ToString();
        }

        //If no variables are found, return the variable name
        return varName;
    }

}
