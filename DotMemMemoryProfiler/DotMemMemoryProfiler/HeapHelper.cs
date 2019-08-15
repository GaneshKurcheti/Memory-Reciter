using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRMD
{
    public static class HeapHelper
    {
        public class Object : Field
        {
            public Segment Segment { get; }
            public int Generation { get; }

            public ObservableCollection<Field> Fields { get; }

            public ObservableCollection<string> Methods { get; }
            public Object()
            {

            }

            public Object(ulong obj, ClrType type, Segment segment, int generation, string name = "") : base(obj, type, name)
            {
                Fields = new ObservableCollection<Field>();
                Methods = new ObservableCollection<string>();
                Size = (int)type.GetSize(obj);
                Segment = segment;
                Generation = generation;

                var instanceFields = type.Fields;
                //var staticFields = Type.StaticFields;
                //var threadStaticField = Type.ThreadStaticFields;
                // var methods = Type.Methods;

                foreach (var field in type.Fields)
                    Fields.Add(new Field(obj, field));
                //foreach (var field in staticFields)
                //    Fields.Add(new Field(obj, field));
                //foreach (var field in threadStaticField)
                //    Fields.Add(new Field(obj, field));

                foreach (var method in type.Methods)
                    Methods.Add(method.ToString());              

            }
        }


        public class Field
        {
            public string Name { get; }
            public int Size { get; internal set; }
            public ulong Address { get; }

            public string Value { get; }

            public string Type { get; }
            public Field()
            {

            }
            public Field(ulong obj, ClrInstanceField field)
            {
                Name = field.Name;
                try
                {
                    Size = field.Size;
                }
                catch (Exception e)
                {
                    Size = 516125;
                }
                Address = field.GetAddress(obj);

                Type = field.Type.ToString();

                Value = GetValue(obj, field);
            }

            //public Field(ulong obj, ClrStaticField field)
            //{
            //    var appDomain = Type.Heap.Runtime.AppDomains[0];
            //    Name = field.Name;
            //    Size = field.Size;
            //    Address = field.GetAddress(appDomain);

            //    Type = field.Type;

            //    Value = GetValue(obj, field, appDomain);
            //}

            //public Field(ulong obj, ClrThreadStaticField field)
            //{
            //    var appDomain = Type.Heap.Runtime.AppDomains[0];
            //    Name = field.Name;
            //    Size = field.Size;
            //    Address = field.GetAddress(appDomain);

            //    Type = field.Type;

            //    Value = GetValue(obj, field, appDomain);
            //}

            internal Field(ulong obj, ClrType type, string name = "")
            {
                Name = name;
                Type = type.ToString();

                Address = obj;
            }
        }





        public enum Segment
        {
            Ephemeral,  //Gen 0 + 1
            Large,      //Gen 2, size < 85,000 bytes
            Segment     //size > 85,000
        }

      

        public class HeapData
        {
            public string Type { get; }
            public int Count { get { return objects.Count; } }
            public int Size { get {
                    int value=0;
                    foreach (var obj in objects)
                    {
                        value = value + obj.Size;
                    }
                    return value; } }
            public int Gen0ObjectsCount
            {
                get
                {
                    int count = 0;
                    foreach (var obj in objects)
                    {
                        if(obj.Generation==0)
                        {
                            count++;
                        }
                    }
                    return count;
                }
            }
            public int Gen0ObjectsSize
            {
                get
                {
                    int count = 0;
                    foreach (var obj in objects)
                    {
                        if (obj.Generation == 0)
                        {
                            count+=obj.Size;
                        }
                    }
                    return count;
                }
            }
            public int Gen1ObjectsCount
            {
                get
                {
                    int count = 0;
                    foreach (var obj in objects)
                    {
                        if (obj.Generation == 1)
                        {
                            count ++;
                        }
                    }
                    return count;
                }
            }
            public int Gen1ObjectsSize
            {
                get
                {
                    int count = 0;
                    foreach (var obj in objects)
                    {
                        if (obj.Generation == 1)
                        {
                            count+=obj.Size;
                        }
                    }
                    return count;
                }
            }
            public int Gen2ObjectsCount
            {
                get
                {
                    int count = 0;
                    foreach (var obj in objects)
                    {
                        if (obj.Generation == 2)
                        {
                            count ++;
                        }
                    }
                    return count;
                }
            }
            public int Gen2ObjectsSize
            {
                get
                {
                    int count = 0;
                    foreach (var obj in objects)
                    {
                        if (obj.Generation == 2)
                        {
                            count+=obj.Size;
                        }
                    }
                    return count;
                }
            }
            public bool UserDefinedOrNot { get { return ((Type.StartsWith("System")||Type.StartsWith("Microsoft")|| Type.StartsWith("Free")||Type.StartsWith("MS.")||Type.StartsWith("<") || Type.StartsWith("_") || Type.StartsWith("Windows.")) ?false:true); } }

            public ObservableCollection<Object> objects { get; set; }

            public HeapData(ClrType type)
            {
                Type = type.ToString();
                objects = new ObservableCollection<Object>();
            }
            public HeapData()
            {
                objects = new ObservableCollection<Object>();
            }
        }

        static private string GetValue(ulong obj, ClrInstanceField field)
        {
            if (!field.HasSimpleValue)
                return field.GetAddress(obj).ToString();
            object value;
            try
            {
                value = field.GetValue(obj);
            }
            catch
            {
                return "{error}";
            }

            switch (field.ElementType)
            {
                case ClrElementType.String:
                    return (string)value;

                case ClrElementType.Array:
                case ClrElementType.SZArray:
                case ClrElementType.Object:
                case ClrElementType.Class:
                case ClrElementType.FunctionPointer:
                case ClrElementType.NativeInt:
                case ClrElementType.NativeUInt:
                    return string.Format("{0:X}", value);

                default:
                    return value.ToString();
            }
        }
        static private string GetValue(ulong obj, ClrStaticField field, ClrAppDomain appDomain)
        {
            if (!field.HasSimpleValue)
                return field.GetAddress(appDomain).ToString();

            object value = field.GetValue(appDomain);
            if (value == null)
                return "{error}";

            switch (field.ElementType)
            {
                case ClrElementType.String:
                    return (string)value;

                case ClrElementType.Array:
                case ClrElementType.SZArray:
                case ClrElementType.Object:
                case ClrElementType.Class:
                case ClrElementType.FunctionPointer:
                case ClrElementType.NativeInt:
                case ClrElementType.NativeUInt:
                    return string.Format("{0:X}", value);

                default:
                    return value.ToString();
            }
        }
    }
}
