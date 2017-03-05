//Makes the internal members visible to to the editor scripts, useful for unit-testing internal classes
#if UNITY_EDITOR
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
#endif