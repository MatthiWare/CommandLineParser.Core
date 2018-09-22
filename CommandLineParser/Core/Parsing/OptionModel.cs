using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Core.Parsing
{
    public class OptionModel
    {
        private string m_value = null;


        public string Key { get; set; }


        public string Value
        {
            get => m_value;
            set
            {
                HasValue = true;
                m_value = value;
            }
        }

        public bool HasValue { get; private set; }

        public OptionModel(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public OptionModel()
        {

        }

    }
}
