using System;

//Modified from: https://github.com/FuzzyHobo/UnityCallbackAndEventTutorial/blob/master/Assets/Scenes/EventCallbackScene/Event.cs
//Now doesn't generate garbage
//How to at bottom

namespace _Project.Scripts.Tools.Other
{
    public class GlobalEvent<T> where T : GlobalEvent<T>, IDisposable, new()
    {
        private static T instance = new T();

        public static T Get()
        {
            //if (instance == null)
            instance = new T();

            if (InUse)
                throw new Exception("Cannot get object twice! Please fire event before grabbing the instance again");

            instance.inUse = true;
            return instance;
        }

        private bool inUse = false;
        public static bool InUse => instance.inUse;

        public delegate void EventListener(T info);

        private static event EventListener Listeners;

        public static void RegisterListener(EventListener listener) => Listeners += listener;

        public static void UnregisterListener(EventListener listener) => Listeners -= listener;

        public void FireEvent()
        {
            if (!inUse)
                throw new Exception("This event has already fired, to prevent infinite loops you can't refire an event");

            Listeners?.Invoke(this as T);

            inUse = false;
            Reset();
        }

        protected virtual void Reset() { }

        #region IDisposable Implementation

        private bool Disposed
        {
            get => !inUse;
            set => inUse = !value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing) { }

            FireEvent();
            Disposed = true;
        }

        #endregion
    }
}

/*Example event
 *
 *	public class DebugEvent : GlobalEvent<DebugEvent>, IDisposable {
 *		public int VerbosityLevel;
 *	}
 */

/* Example use case
 *	using (var e = DebugEvent.Get()) { //Grab ownership of event instance
 *		e.VerbosityLevel = 3;
 *		e.Description = "Something happened!";
 *	} //Event get's fired on dispose
 */