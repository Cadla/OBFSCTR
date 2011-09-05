#region

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace JanuszLembicz.PW.UI.Form.Utils
{
    public static class ComboBoxExtensions
    {
        #region Nested type: ComboBoxValue

        private class ComboBoxValue<T> where T : struct
        {
            public string Text { get; set; }

            public T? Value { get; set; }
        }

        #endregion

        #region Nested type: ComboBoxValueListBuilder

        private class ComboBoxValueListBuilder<T> where T : struct
        {
            private List<ComboBoxValue<T>> _valueList;

            public void CreateValueList()
            {
                _valueList = new List<ComboBoxValue<T>>();
            }

            public void AddEmptyValue()
            {
                _valueList.Add(new ComboBoxValue<T>() {Text = "dowolna wartość", Value = null});
            }

            public void AddEnumValues()
            {
                Array enumValues = Enum.GetValues(typeof(T));

                foreach(var enumValue in enumValues)
                {
                    _valueList.Add(new ComboBoxValue<T>() {Text = enumValue.ToString(), Value = (T)enumValue});
                }
            }

            public List<ComboBoxValue<T>> GetValueList()
            {
                return _valueList;
            }
        }

        #endregion

        public static void Initialize<T>(this ComboBox comboBox) where T : struct
        {
            ComboBoxValueListBuilder<T> builder = new ComboBoxValueListBuilder<T>();
            builder.CreateValueList();
            builder.AddEmptyValue();
            builder.AddEnumValues();

            comboBox.DataSource = builder.GetValueList();
            comboBox.DisplayMember = "Text";
            comboBox.ValueMember = "Value";
        }
    }
}