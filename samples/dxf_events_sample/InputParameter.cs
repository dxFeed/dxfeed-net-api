#region License

/*
Copyright (c) 2010-2023 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace dxf_events_sample
{
    /// <summary>
    ///     Simple class describing input parameters
    /// </summary>
    /// <typeparam name="T">Input parameter type</typeparam>
    public class InputParameter<T>
    {
        private T value;

        private InputParameter()
        {
            IsSet = false;
        }

        public InputParameter(T defaultValue) : this()
        {
            value = defaultValue;
        }

        public bool IsSet { get; private set; }

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                IsSet = true;
            }
        }
    }
}