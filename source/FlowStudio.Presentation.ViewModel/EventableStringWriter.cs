// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel
{
    using System;
    using System.IO;
    using System.Text;

    public class EventableStringWriter : StringWriter
    {
        public EventableStringWriter(StringBuilder sb)
            : base(sb)
        {
        }

        public event EventHandler TextChanged;

        private void OnTextChanged()
        {
            if (TextChanged != null)
            {
                TextChanged(this, new EventArgs());
            }
        }

        public override void Write(char value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(char[] buffer)
        {
            base.Write(buffer);
            OnTextChanged();
        }

        public override void Write(string format, object arg0, object arg1)
        {
            base.Write(format, arg0, arg1);
            OnTextChanged();
        }

        public override void Write(bool value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            OnTextChanged();
        }

        public override void Write(decimal value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(double value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(float value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(int value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(long value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(object value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(uint value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void Write(ulong value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void WriteLine(bool value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(char value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(char[] buffer)
        {
            base.WriteLine(buffer);
            OnTextChanged();
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            base.WriteLine(buffer, index, count);
            OnTextChanged();
        }

        public override void WriteLine(decimal value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(double value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(float value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(int value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(long value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(object value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            base.WriteLine(format, arg0, arg1);
            OnTextChanged();
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            base.WriteLine(format, arg0, arg1, arg2);
            OnTextChanged();
        }

        public override void WriteLine(string format, params object[] arg)
        {
            base.WriteLine(format, arg);
            OnTextChanged();
        }

        public override void WriteLine(uint value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(ulong value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }

        public override void WriteLine(string format, object arg0)
        {
            base.WriteLine(format, arg0);
            OnTextChanged();
        }

        public override void Write(string value)
        {
            base.Write(value);
            OnTextChanged();
        }

        public override void WriteLine()
        {
            base.WriteLine();
            OnTextChanged();
        }

        public override void Write(string format, params object[] arg)
        {
            base.Write(format, arg);
            OnTextChanged();
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            base.Write(format, arg0, arg1, arg2);
            OnTextChanged();
        }

        public override void Write(string format, object arg0)
        {
            base.Write(format, arg0);
            OnTextChanged();
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            OnTextChanged();
        }
    }
}
