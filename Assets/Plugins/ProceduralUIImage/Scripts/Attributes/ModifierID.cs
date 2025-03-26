namespace UnityEngine.UI.ProceduralImage
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ModifierID : System.Attribute
    {
        public ModifierID(string name) => Name = name;

        public string Name { get; }
    }
}