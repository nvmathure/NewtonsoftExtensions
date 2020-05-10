#region LICENSE
// Copyright(c) 2020 Nandan Mathure

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudNDevOps.Newtonsoft.Extensions
{
    /// <summary>
    /// Defines customer convertor for Newtonsoft.Json to convert families of types into List/Array
    /// </summary>
    /// <typeparam name="TBase">Type of base class of family</typeparam>
    /// <typeparam name="TBaseClassifier">Data type of classifier which can identify derived class</typeparam>
    public class TypeFamilyConverter<TBase, TBaseClassifier> : JsonConverter
    {
        private readonly Func<TBase, TBaseClassifier> _typeValueFunc;
        private readonly IDictionary<TBaseClassifier, Type> _typeLookupDictionary;
        private readonly IEnumerable<Type> _supportedTypes;

        public TypeFamilyConverter(Func<TBase, TBaseClassifier> typeValueFunc, IDictionary<TBaseClassifier, Type> typeLookupDictionary)
        {
            _typeValueFunc = typeValueFunc ?? throw new ArgumentNullException(nameof(typeValueFunc));
            _typeLookupDictionary = typeLookupDictionary ?? throw new ArgumentNullException(nameof(typeLookupDictionary));
            _supportedTypes = _typeLookupDictionary.Keys.Select(k => _typeLookupDictionary[k]).ToArray();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TBase);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var baseObject = jo.ToObject<TBase>();
            var classifier = _typeValueFunc(baseObject);
            if (!_typeLookupDictionary.TryGetValue(classifier, out Type derivedType)) 
                throw new InvalidOperationException($"Classifier {classifier.ToString()} is not recognized");
            return jo.ToObject(derivedType, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
