using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotMemMemoryProfiler
{
    class AssemblyExaminer
    {
        public NamespaceTable namespaceTable = new NamespaceTable();
        public MethodMethodsTable methodMethodsTable = new MethodMethodsTable();
        public MethodVariablesTable methodVariablesTable = new MethodVariablesTable();
        public MethodIlCodeTable methodIlCodeTable = new MethodIlCodeTable();
        public ItemExternalInfoTable itemExternalInfoTable = new ItemExternalInfoTable();
        public ItemInternalInfoTable itemInternalInfoTable = new ItemInternalInfoTable();
        private void AssemblyAnalizer(Assembly assembly)
        {
            Assembly requiredAssembly = assembly;
            var temptypes = requiredAssembly.GetTypes();
            var definedTypes = requiredAssembly.DefinedTypes;
            if (definedTypes != null)
            {
                foreach (var defType in definedTypes)
                {
                    try
                    {
                        if (defType != null && !defType.ToString().Contains("MemProfiler"))
                        {
                            NamespaceClass namespaceClass = new NamespaceClass()
                            {
                                NamespaceName = assembly.GetName().Name.ToString(),
                                Item = defType.FullName.ToString(),
                                ItemType = (defType.IsPublic ? "Public" : (defType.IsSealed ? "Sealed" : (defType.IsNotPublic ? "NotPublic" : "undefined")) + " " + (defType.IsGenericType ? "Generic" : " ") + " " + (defType.IsClass ? "Class" : (defType.IsInterface ? "Interface" : (defType.IsEnum ? "Enum" : (defType.IsValueType ? "Struct" : "ALIEN"))))),
                                IsGAC = requiredAssembly.GlobalAssemblyCache,
                                EntryPoint = requiredAssembly.EntryPoint.ToString()
                            };
                            namespaceTable.NamespaceRecords.Add(namespaceClass);
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            foreach (var temptype in temptypes)
            {
                if (!temptype.ToString().Contains("MemProfiler"))
                {
                    var interfaces = temptype.GetInterfaces();
                    if (interfaces != null)
                    {
                        foreach (var interface1 in interfaces)
                        {
                            ItemExternalInfo itemExternalInfo = new ItemExternalInfo()
                            {
                                ItemName = temptype.FullName,
                                ItemType = ((temptype.IsPublic ? "Public" : (temptype.IsSealed ? "Sealed" : (temptype.IsNotPublic ? "NotPublic" : "undefined"))) + " " + (temptype.IsGenericType ? "Generic" : " ") + " " + (temptype.IsClass ? "Class" : (temptype.IsInterface ? "Interface" : (temptype.IsEnum ? "Enum" : (temptype.IsValueType ? "Struct" : "ALIEN"))))),
                                InternalItems = interface1.FullName,
                                InternalItemType = "Interface",
                                BaseType = temptype.BaseType.Name,
                            };
                            itemExternalInfoTable.ItemExternalDetails.Add(itemExternalInfo);
                        }
                    }
                    var events = temptype.GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (events != null)
                    {
                        foreach (var event1 in events)
                        {
                            ItemExternalInfo itemExternalInfo = new ItemExternalInfo()
                            {
                                ItemName = temptype.FullName,
                                ItemType = ((temptype.IsPublic ? "Public" : (temptype.IsSealed ? "Sealed" : (temptype.IsNotPublic ? "NotPublic" : "undefined"))) + " " + (temptype.IsGenericType ? "Generic" : " ") + " " + (temptype.IsClass ? "Class" : (temptype.IsInterface ? "Interface" : (temptype.IsEnum ? "Enum" : (temptype.IsValueType ? "Struct" : "ALIEN"))))),
                                InternalItems = event1.Name,
                                InternalItemType = "Event",
                                BaseType = temptype.BaseType.Name,
                            };
                            itemExternalInfoTable.ItemExternalDetails.Add(itemExternalInfo);

                        }
                    }
                    var tempmethods = temptype.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly);
                    foreach (var tempmethod in tempmethods)
                    {
                        if (!tempmethod.ToString().Equals("System.String ToString()") && !tempmethod.ToString().Equals("System.Type GetType()") && !tempmethod.ToString().Equals("Int32 GetHashCode()") && !tempmethod.ToString().Equals("Boolean Equals(System.Object)"))
                        {
                            ItemExternalInfo itemExternalInfo = new ItemExternalInfo()
                            {
                                ItemName = temptype.FullName,
                                ItemType = (temptype.IsPublic ? "Public" : (temptype.IsSealed ? "Sealed" : (temptype.IsNotPublic ? "NotPublic" : "undefined")) + " " + (temptype.IsGenericType ? "Generic" : " ") + " " + (temptype.IsClass ? "Class" : (temptype.IsInterface ? "Interface" : (temptype.IsEnum ? "Enum" : (temptype.IsValueType ? "Struct" : "ALIEN"))))),
                                InternalItems = (temptype.ToString() + "." + tempmethod.Name),
                                InternalItemType = ((tempmethod.IsGenericMethod ? "generic " : "") + " " + (tempmethod.IsVirtual ? "virtual" : "") + " " + (tempmethod.IsStatic ? "static" : "") + " " + (tempmethod.IsPublic ? " public" : (tempmethod.IsPrivate ? "private" : "protected")) + " " + "Method"),
                                BaseType = temptype.BaseType.Name,
                            };
                            itemExternalInfoTable.ItemExternalDetails.Add(itemExternalInfo);
                            string column1 = temptype.ToString() + "." + tempmethod.Name;
                            int totalStringCounter = 0, totalarrayCounter = 0;
                            List<int> listVariableToStoreMedaToken = new List<int>();
                            Dictionary<long, List<String>> objectsFieldsMapper = new Dictionary<long, List<String>>();
                            Dictionary<long, string> localValuesMapper = new Dictionary<long, string>();
                            HashSet<long> newMetadataIdentifier = new HashSet<long>();
                            MethodBase targetMethod = tempmethod;
                            MethodBody body = tempmethod.GetMethodBody();
                            HashSet<string> heapFilterer = new HashSet<string>();
                            if (body != null)
                            {
                                var instructionsInTargetMethod = MethodBodyReader.GetInstructions(targetMethod);
                                string ilcode = "";
                                foreach (var instructionInMain in instructionsInTargetMethod)
                                {
                                    ilcode = ilcode + "\n" + instructionInMain.ToString();
                                    MethodInfo methodInMain = instructionInMain.Operand as MethodInfo;
                                    try
                                    {
                                        Type typeInfo = methodInMain.DeclaringType;
                                        string column2 = typeInfo.FullName + "." + methodInMain.Name + "( )";
                                        MethodMethodsClass methodMethodClass = new MethodMethodsClass()
                                        {
                                            MethodName = column1.ToString(),
                                            InternalMethods = column2.ToString()
                                        };
                                        methodMethodsTable.MethodInternalMethods.Add(methodMethodClass);
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                                foreach (LocalVariableInfo variableInfo in body.LocalVariables)
                                {
                                    string s = "";
                                    try
                                    {
                                        s = localValuesMapper[variableInfo.LocalIndex];
                                    }
                                    catch (Exception)
                                    {
                                        s = "Not Assaigned";
                                    }
                                    MethodVariables methodVariables = new MethodVariables()
                                    {
                                        MethodName = column1,
                                        Index = variableInfo.LocalIndex.ToString(),
                                        VariableType = variableInfo.LocalType.ToString(),
                                        LocalOrReference = (variableInfo.LocalType.IsClass ? "Reference Type" : "Local Variable"),
                                        Value = s.ToString()
                                    };
                                    methodVariablesTable.MethodVariables.Add(methodVariables);
                                    if (variableInfo.LocalType.IsClass && !variableInfo.LocalType.ToString().Contains("System.String") && variableInfo.LocalType.MetadataToken != 33554432 && variableInfo.LocalType.MetadataToken != 33555611 && !variableInfo.LocalType.ToString().Contains("System.Object"))
                                    {
                                        foreach (FieldInfo field in variableInfo.LocalType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                                        {
                                            ItemInteranlDetails itemInternalInfo = new ItemInteranlDetails()
                                            {
                                                ItemName = variableInfo.LocalType.ToString(),
                                                ItemType = (temptype.IsPublic ? "Public" : (temptype.IsSealed ? "Sealed" : (temptype.IsNotPublic ? "NotPublic" : "undefined")) + " " + (temptype.IsGenericType ? "Generic" : " ") + " " + (temptype.IsClass ? "Class" : (temptype.IsInterface ? "Interface" : (temptype.IsEnum ? "Enum" : (temptype.IsValueType ? "Struct" : "ALIEN"))))),
                                                MemberName = field.Name,
                                                MemberType = field.FieldType.ToString(),
                                                FieldOrProperty = "Field"
                                            };
                                            itemInternalInfoTable.ItemInternalDetails.Add(itemInternalInfo);
                                        }

                                        List<string> tempList = new List<string>();
                                        foreach (FieldInfo field in variableInfo.LocalType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                                        {
                                            tempList.Add(field.Name);
                                        }
                                        listVariableToStoreMedaToken.Add(variableInfo.LocalType.MetadataToken);
                                        var length = newMetadataIdentifier.Count;
                                        newMetadataIdentifier.Add(variableInfo.LocalType.MetadataToken);
                                        if (length < newMetadataIdentifier.Count)
                                        {
                                            objectsFieldsMapper.Add((long)variableInfo.LocalType.MetadataToken, tempList);
                                        }
                                        foreach (PropertyInfo property in variableInfo.LocalType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                                        {                                            
                                            ItemInteranlDetails itemInternalInfo = new ItemInteranlDetails()
                                            {
                                                ItemName = variableInfo.LocalType.ToString(),
                                                ItemType = (temptype.IsPublic ? "Public" : (temptype.IsSealed ? "Sealed" : (temptype.IsNotPublic ? "NotPublic" : "undefined")) + " " + (temptype.IsGenericType ? "Generic" : " ") + " " + (temptype.IsClass ? "Class" : (temptype.IsInterface ? "Interface" : (temptype.IsEnum ? "Enum" : (temptype.IsValueType ? "Struct" : "ALIEN"))))),
                                                MemberName = property.Name,
                                                MemberType = property.PropertyType.ToString(),
                                                FieldOrProperty = "Property"
                                            };
                                            itemInternalInfoTable.ItemInternalDetails.Add(itemInternalInfo);

                                        }
                                    }
                                    if (variableInfo.LocalType.MetadataToken == 33554432)
                                    {
                                        totalarrayCounter++;
                                    }
                                    if (variableInfo.LocalType.Name.EndsWith("String") && !variableInfo.LocalType.Name.Contains("[]"))
                                    {
                                        totalStringCounter++;
                                    }
                                }
                                Process process = Process.GetCurrentProcess();
                                ilcode = ilcode.Replace("\"", "!!");
                                ilcode = ilcode.Replace("'", "!");
                                MethodIlCode methodIlCode = new MethodIlCode()
                                {
                                    MethodName = temptype.FullName + "." + targetMethod.Name,
                                    MethodIl = ilcode,
                                };
                                methodIlCodeTable.MethodIlCodes.Add(methodIlCode);
                            }
                        }
                    }
                }
            }
        }
        public int nonDecodedAssemblies { get; set; }
        public void AssemblyReader(List<string> Paths)
        {
            foreach (var item in Paths)
            {
                try
                {
                    Assembly assemblyParam = Assembly.LoadFrom(@"" + item + "");
                    AssemblyAnalizer(assemblyParam);
                }
                catch
                {
                    nonDecodedAssemblies++;
                }
            }
        }        
    }
}
