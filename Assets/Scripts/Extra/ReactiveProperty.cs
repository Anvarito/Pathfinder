using System;

namespace Infrastructure.Extras
{
    public class ReactiveProperty<T>
    {
        private T m_Value;
        public event Action<T> Changed;

        public ReactiveProperty(T value)
        {
            SetWithoutNotification(value);
        }
        public T value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                Changed?.Invoke(m_Value);
            }
            
        }

        public void SetWithoutNotification(T value)
        {
            m_Value = value;
        }
    }
}
