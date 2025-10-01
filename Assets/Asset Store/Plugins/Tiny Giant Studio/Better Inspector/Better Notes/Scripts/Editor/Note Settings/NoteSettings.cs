using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.BetterInspector
{
    [FilePath("ProjectSettings/BetterInspector NoteSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class NoteSettings : ScriptableSingleton<NoteSettings>
    {
        public bool showNotes = true;
        public bool showNotesGizmo = true;
        public bool showNotesGizmoDuringPlayMode = false;
        public Vector2 notesGizmoOffset = Vector2.zero;

        public NoteClickActions noteClickActions = NoteClickActions.OpenNoteEditor;

        public void Save()
        {
            Save(true);
        }

        [System.Serializable]
        public enum NoteClickActions
        {
            DoNothing,
            OpenNoteEditor
            //OpenQuickEditor //TODO
        }
    }
}