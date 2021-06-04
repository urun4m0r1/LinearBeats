using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lean.Pool;
using LinearBeats.Audio;
using LinearBeats.Script;
using Sirenix.OdinInspector;

namespace LinearBeats.Scrolling
{
    public sealed class TimingObjectLoader
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly TimingObject _timingObject;
        [ShowInInspector, ReadOnly] [NotNull] private readonly LeanGameObjectPool _notesPool;
        [ShowInInspector, ReadOnly] [NotNull] private readonly LeanGameObjectPool _dividerPool;

        public TimingObjectLoader(
            [NotNull] TimingObject timingObject,
            [NotNull] LeanGameObjectPool notesPool,
            [NotNull] LeanGameObjectPool dividerPool)
        {
            _timingObject = timingObject;
            _notesPool = notesPool;
            _dividerPool = dividerPool;
        }

        [CanBeNull] public DividerBehaviour InstantiateDivider(Divider divider)
        {
            var dividerObject = _dividerPool.Spawn(_dividerPool.transform);
            var dividerBehaviour = dividerObject.GetComponent<DividerBehaviour>();

            var ignoreOptions = ParseIgnoreOptions(divider.IgnoreScrollEvent);
            dividerBehaviour.RailObject = new DividerRail(_timingObject, ignoreOptions, divider.Pulse);
            return dividerBehaviour;
        }

        [CanBeNull] public NoteBehaviour InstantiateNote(Note note, Note nextNote,
            [NotNull] Dictionary<ushort, AudioPlayer> audioPlayers)
        {
            var noteObject = _notesPool.Spawn(_notesPool.transform);
            var noteBehaviour = noteObject.GetComponent<NoteBehaviour>();

            var ignoreOptions = ParseIgnoreOptions(note.IgnoreScrollEvent);
            noteBehaviour.RailObject = new NoteRail(_timingObject, ignoreOptions, note.Trigger);
            noteBehaviour.NoteShape = note.Shape;
            noteBehaviour.Judgement = _timingObject.Judgement;
            noteBehaviour.AudioPlayer = audioPlayers[note.Trigger.Channel];

            var intervalPulse = nextNote.Trigger.Pulse - note.Trigger.Pulse;
            noteBehaviour.AudioLength = _timingObject.Factory.Create(intervalPulse);

            return noteBehaviour;
        }

        // ReSharper restore Unity.ExpensiveCode
        private static ScrollEvent ParseIgnoreOptions([CanBeNull] string options)
        {
            if (string.IsNullOrWhiteSpace(options)) return ScrollEvent.None;

            var result = ScrollEvent.None;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var option in (ScrollEvent[]) Enum.GetValues(typeof(ScrollEvent)))
                if (options.Contains(option.ToString())) result |= option;

            return result;
        }
    }
}
