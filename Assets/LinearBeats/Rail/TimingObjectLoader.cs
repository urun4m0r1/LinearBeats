using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lean.Pool;
using LinearBeats.Media;
using LinearBeats.Script;
using LinearBeats.Scrolling;
using Sirenix.OdinInspector;

namespace LinearBeats.Rail
{
    public sealed class TimingObjectLoader
    {
        [ShowInInspector, ReadOnly] [NotNull] private readonly TimingInfo _timingInfo;
        [ShowInInspector, ReadOnly] [NotNull] private readonly LeanGameObjectPool _notesPool;
        [ShowInInspector, ReadOnly] [NotNull] private readonly LeanGameObjectPool _dividerPool;

        public TimingObjectLoader(
            [NotNull] TimingInfo timingInfo,
            [NotNull] LeanGameObjectPool notesPool,
            [NotNull] LeanGameObjectPool dividerPool)
        {
            _timingInfo = timingInfo;
            _notesPool = notesPool;
            _dividerPool = dividerPool;
        }

        [CanBeNull] public DividerBehaviour InstantiateDivider(Divider divider)
        {
            var dividerObject = _dividerPool.Spawn(_dividerPool.transform);
            var dividerBehaviour = dividerObject.GetComponent<DividerBehaviour>();

            var ignoreOptions = ParseIgnoreOptions(divider.IgnoreScrollEvent);
            dividerBehaviour.RailObject = new DividerRail(_timingInfo, ignoreOptions, divider.Pulse);
            return dividerBehaviour;
        }

        [CanBeNull] public NoteBehaviour InstantiateNote(Note note, Note nextNote,
            [NotNull] Dictionary<ushort, IMediaPlayer> mediaPlayers)
        {
            var noteObject = _notesPool.Spawn(_notesPool.transform);
            var noteBehaviour = noteObject.GetComponent<NoteBehaviour>();

            var ignoreOptions = ParseIgnoreOptions(note.IgnoreScrollEvent);
            noteBehaviour.RailObject = new NoteRail(_timingInfo, ignoreOptions, note.Trigger);
            noteBehaviour.Note = note;
            noteBehaviour.Judgement = _timingInfo.Judgement;
            noteBehaviour.MediaPlayer = mediaPlayers[note.Trigger.Channel];

            var intervalPulse = nextNote.Trigger.Pulse - note.Trigger.Pulse;
            noteBehaviour.AudioLength = _timingInfo.Factory.Create(intervalPulse);

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
