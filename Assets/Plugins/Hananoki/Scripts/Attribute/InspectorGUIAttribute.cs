using System;

namespace Hananoki {
	/// <summary>
	/// Attribute to create a button in the inspector for calling the method it is attached to.
	/// The method must have no arguments.
	/// </summary>
	/// <example>
	/// [Button]
	/// public void MyMethod()
	/// {
	///     Debug.Log("Clicked!");
	/// }
	/// </example>
	[AttributeUsage( AttributeTargets.Method, Inherited = true, AllowMultiple = false )]
	public sealed class InspectorGUIAttribute : Attribute {

		public InspectorGUIAttribute() {
		}

	}
}
