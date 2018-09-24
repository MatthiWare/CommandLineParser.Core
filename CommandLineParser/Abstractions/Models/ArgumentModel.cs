using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Models
{
    public class ArgumentModel
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

        public ArgumentModel(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public ArgumentModel()
        {

        }

    }
}
