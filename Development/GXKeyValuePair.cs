namespace Gurux.DLMS
{
    using System;

    [Serializable]
    public struct GXKeyValuePair<K, V>
    {
        private K _key;
        private V _value;

        public K Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        public V Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public GXKeyValuePair(K key, V value)
        {
            if (key == null)
            {
                throw new InvalidOperationException("Invalid key.");
            }
            _key = key;
            _value = value;
        }
    }
}
