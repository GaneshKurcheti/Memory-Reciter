using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
    class NamespaceTable
    {
        public List<NamespaceClass> NamespaceRecords { get; set; }
        public NamespaceTable()
        {
            NamespaceRecords = new List<NamespaceClass>();
        }
    }
    class NamespaceClass
    {
        public string NamespaceName { get; set; }
        public string Item { get; set; }
        public string ItemType { get; set; }
        public bool IsGAC { get; set; }
        public string EntryPoint { get; set; }

    }
    class MethodMethodsTable
    {
        public List<MethodMethodsClass> MethodInternalMethods { get; set; }
        public MethodMethodsTable()
        {
            MethodInternalMethods = new List<MethodMethodsClass>();
        }
    }
    class MethodMethodsClass
    {
        public string MethodName;
        public string InternalMethods;
    }
    class MethodVariablesTable
    {
        public List<MethodVariables> MethodVariables { get; set; }
        public MethodVariablesTable()
        {
            MethodVariables = new List<MethodVariables>();
        }
    }
    class MethodVariables
    {
        public string MethodName { get; set; }
        public string Index { get; set; }
        public string VariableType { get; set; }
        public string LocalOrReference { get; set; }
        public string Value { get; set; }
    }
    class MethodIlCodeTable
    {
        public List<MethodIlCode> MethodIlCodes { get; set; }
        public MethodIlCodeTable()
        {
            MethodIlCodes = new List<MethodIlCode>();
        }
    }
    class MethodIlCode
    {
        public string MethodName { get; set; }
        public string MethodIl { get; set; }
    }
    class ItemExternalInfoTable
    {
        public List<ItemExternalInfo> ItemExternalDetails { get; set; }
        public ItemExternalInfoTable()
        {
            ItemExternalDetails = new List<ItemExternalInfo>();
        }
    }
    class ItemExternalInfo
    {
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public string InternalItems { get; set; }
        public string InternalItemType { get; set;}
        public string BaseType { get; set; }
    }
    class ItemInternalInfoTable
    {
        public List<ItemInteranlDetails> ItemInternalDetails { get; set; }
        public ItemInternalInfoTable()
        {
            ItemInternalDetails = new List<ItemInteranlDetails>();
        }
    }
    class ItemInteranlDetails
    {
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public string MemberName { get; set;}
        public string MemberType { get; set; }
        public string FieldOrProperty { get; set; }
    }
}
