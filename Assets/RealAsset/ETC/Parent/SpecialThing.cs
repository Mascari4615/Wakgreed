using UnityEngine;
using UnityEditor;

public abstract class SpecialThing : ScriptableObject
{
    public int ID;
    public new string name;
    public Sprite sprite;
    [TextArea] public string description;
    [TextArea] public string comment;  
    [SerializeField] private Effect[] effects;

    public virtual void OnEquip()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i]._Effect();
        }
    }
    
    public virtual void OnRemove()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Return();
        }
    }
}

/*[CustomPropertyDrawer(typeof(Sprite))]
public class CombinableSpritesAttributeEditor : PropertyDrawer
{
    float spriteSize = 64;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var labelRect = new Rect(position.x, position.y - (spriteSize/2) + 10, position.width, position.height);
        EditorGUI.LabelField(labelRect, "Sprite");

        var spriteRect = new Rect(position.xMax - spriteSize, position.y, spriteSize, spriteSize);
        property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, property.objectReferenceValue, typeof(Sprite), false);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return spriteSize;
    }
}*/