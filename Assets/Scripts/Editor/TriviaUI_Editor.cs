using UnityEditor;

[CustomEditor(typeof(TriviaUIManager))]
public class TriviaUI_Editor : Editor
{
    // The various categories the editor will display the variables in 
    public enum DisplayCategory
    {
        Login, Trivia
    }

    // The enum field that will determine what variables to display in the Inspector
    public DisplayCategory categoryToDisplay;

    // The function that makes the custom editor work
    public override void OnInspectorGUI() {
        // Display the enum popup in the inspector
        categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Display", categoryToDisplay);

        // Create a space to separate this enum popup from other variables 
        EditorGUILayout.Space();

        // Switch statement to handle what happens for each category
        switch (categoryToDisplay) {
            case DisplayCategory.Login:
                DisplayLoginElements();
                break;

            case DisplayCategory.Trivia:
                DisplayTriviaElements();
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    // When the categoryToDisplay enum is at "Login"
    void DisplayLoginElements() {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mainLoginWindow"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("signupWindow"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("createRoomWindow"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("joinRoomWindow"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("errorMessage"));
    }

    // When the categoryToDisplay enum is at "Trivia"
    void DisplayTriviaElements() {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("triviaWindow"));
    }
}