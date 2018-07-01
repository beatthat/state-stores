using UnityEditor;
using UnityEngine;

namespace BeatThat.StateStores
{
    [CustomEditor(typeof(StateStore), editorForChildClasses: true)]
    public class StateStoreEditor : UnityEditor.Editor
    {
        private bool showStored { get; set; }

        override public void OnInspectorGUI()
        {
            base.OnInspectorGUI();

			if (!Application.isPlaying)
			{
				return;
			}
   
            // TODO: load status
        }


		#pragma warning disable 414
		private static readonly Color IN_PROGRESS = Color.cyan;
		private static readonly Color ERROR = Color.red;
		private static readonly Color PENDING = Color.yellow;
		private static readonly Color NONE = Color.gray;
		#pragma warning restore 414

    }
}




