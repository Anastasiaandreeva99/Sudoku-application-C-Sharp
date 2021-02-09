using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokoSisi
{
   public  class SingleField
    {
        public int Value { get; set; }
        public bool Constant { get; set; }

        public SingleField(int _value, bool _constant)
        {
            Value = _value;
            Constant = _constant;
        }
        public SingleField() : this(0, true)
        {

        }
        public SingleField(SingleField _singleField):this(_singleField.Value,_singleField.Constant)
        {

        }

    }
}
