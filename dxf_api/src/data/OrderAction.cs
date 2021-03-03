#region License

/*
Copyright (c) 2010-2021 Devexperts LLC

This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion

namespace com.dxfeed.api.data
{
    /// <summary>
    /// Action enum for the Full Order Book (FOB) Orders. Action describes business meaning of the NativeOrder event:
    /// whether order was added or replaced, partially or fully executed, etc.
    /// </summary>
    public enum OrderAction
    {
        /// <summary>
        /// Default enum value for orders that do not support "Full Order Book" and for backward compatibility -
        /// action must be derived from other NativeOrder fields
        ///
        /// All Full Order Book related fields for this action will be empty.
        ///
        /// Integer value = 0
        /// </summary>
        Undefined = 0,
        
        /// <summary>
        /// New Order is added to Order Book.
        ///
        /// Full Order Book fields:
        /// - NativeOrder.OrderId - always present
        /// - NativeOrder.AuxOrderId - ID of the order replaced by this new order - if available.
        /// - Trade fields will be empty
        ///
        /// Integer value = 1
        /// </summary>
        New = 1,
        
        /// <summary>
        /// 
        /// </summary>
        Replace = 2,
        
        Modify = 3,
        
        Delete = 4,
        
        Partial = 5,
        
        Execute = 6,
        
        Trade = 7,
        
        Bust = 8
    }
}