using System;

namespace TestClient.Utility;
internal static class Events {
    internal class BindableEvent {
        public delegate void EventHandler(object[] args = null);
        public event EventHandler OnEvent;
        private Action<object[]> CustomTrigger;

        public BindableEvent(Action<object[]> customTrigger = null) {
            CustomTrigger = customTrigger;
        }

        public void Trigger(object[]args = null) {
            CustomTrigger?.Invoke(args);
            OnEvent?.Invoke(args);
        }
    }
}
