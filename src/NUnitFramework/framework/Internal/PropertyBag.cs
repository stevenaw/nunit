// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// A PropertyBag represents a collection of name value pairs
    /// that allows duplicate entries with the same key. Methods
    /// are provided for adding a new pair as well as for setting
    /// a key to a single value. All keys are strings but values
    /// may be of any type. Null values are not permitted, since
    /// a null entry represents the absence of the key.
    /// </summary>
    public class PropertyBag : IPropertyBag
    {
        private readonly Dictionary<string, IList<object>> inner = new Dictionary<string, IList<object>>();

        #region IPropertyBagMembers

        /// <summary>
        /// Adds a key/value pair to the property set.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void Add(string key, object value)
        {
            Guard.ArgumentNotNull(value, nameof(value));

            if (!inner.TryGetValue(key, out var list))
            {
                list = new List<object>();
                inner.Add(key, list);
            }
            list.Add(value);
        }

        /// <summary>
        /// Adds a key and set of values to the property bag.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="values">The values to add</param>
        public void AddRange(string key, IList<object> values)
        {
            Guard.ArgumentNotNull(key, nameof(key));
            Guard.ArgumentNotNull(values, nameof(values));

            if (inner.TryGetValue(key, out var existing))
                ((List<object>)existing).AddRange(values);
            else
                inner.Add(key, new List<object>(values));
        }

        /// <summary>
        /// Copies the contents to another instance.
        /// </summary>
        /// <param name="other">The instance to copy to</param>
        public void CopyTo(IPropertyBag other)
        {
            Guard.ArgumentNotNull(other, nameof(other));

            foreach (var kvp in inner)
                other.AddRange(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Sets the value for a key, removing any other
        /// values that are already in the property set.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            // Guard against mystery exceptions later!
            Guard.ArgumentNotNull(key, nameof(key));
            Guard.ArgumentNotNull(value, nameof(value));

            var list = new List<object>();
            list.Add(value);
            inner[key] = list;
        }

        /// <summary>
        /// Gets a single value for a key, using the first
        /// one if multiple values are present and returning
        /// null if the value is not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object? Get(string key)
        {
            return inner.TryGetValue(key, out var list) && list.Count > 0
                ? list[0]
                : null;
        }

        /// <summary>
        /// Gets a flag indicating whether the specified key has
        /// any entries in the property set.
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>
        /// True if their are values present, otherwise false
        /// </returns>
        public bool ContainsKey(string key)
        {
            return inner.ContainsKey(key);
        }

        /// <summary>
        /// Gets a collection containing all the keys in the property set
        /// </summary>
        /// <value></value>
        public ICollection<string> Keys
        {
            get { return inner.Keys; }
        }

        /// <summary>
        /// Gets or sets the list of values for a particular key
        /// </summary>
        public IList<object> this[string key]
        {
            get
            {
                if (!inner.TryGetValue(key, out var list))
                {
                    list = new List<object>();
                    inner.Add(key, list);
                }
                return list;
            }
            set
            {
                inner[key] = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of values for a particular key
        /// </summary>
        public bool TryGetValue(string key, out IList<object> value)
        {
            return inner.TryGetValue(key, out value);
        }

        /// <summary>
        /// Tries to get the first value at the specified key.
        /// </summary>
        /// <param name="key">The key to retrieve values for</param>
        /// <param name="value">The value at the specified key</param>
        public bool TryGetSingleValue<T>(string key, [MaybeNullWhen(false)] out T value)
        {
            if (inner.TryGetValue(key, out var list) && list.Count > 0 && list[0] is T result)
            {
                value = result;
                return true;
            }

            value = default;
            return false;
        }

        #endregion

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns an XmlNode representing the current PropertyBag.
        /// </summary>
        /// <param name="recursive">Not used</param>
        /// <returns>An XmlNode representing the PropertyBag</returns>
        public TNode ToXml(bool recursive)
        {
            return AddToXml(new TNode("dummy"), recursive);
        }

        /// <summary>
        /// Returns an XmlNode representing the PropertyBag after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">Not used</param>
        /// <returns></returns>
        public TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode properties = parentNode.AddElement("properties");

            foreach (var key in Keys)
            {
                foreach (object value in this[key])
                {
                    TNode prop = properties.AddElement("property");

                    // TODO: Format as string
                    prop.AddAttribute("name", key);
                    prop.AddAttribute("value", value.ToString());
                }
            }

            return properties;
        }

        #endregion
    }
}
