using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;

namespace ModelHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathToSolution, projectName, classFullName;

#if !DEBUG
            if (args.Count() != 3)
            {
                Console.WriteLine("Generates json model for given type");
                Console.WriteLine("Usage: ModelHelper (solution path) (project name) (class name including namespace)");
                return;
            }

            pathToSolution = args[0];
            projectName = args[1];
            classFullName = args[2];
#endif


#if DEBUG
            pathToSolution = @"";
            projectName = "";
            classFullName = "";
#endif

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Solution solutionToAnalyze = workspace.OpenSolutionAsync(pathToSolution).Result;
            Project sampleProjectToAnalyze = solutionToAnalyze.Projects.Single((proj) => proj.Name == projectName);
            Compilation compilation = sampleProjectToAnalyze.GetCompilationAsync().Result;

            var theType = compilation.GetTypeByMetadataName(classFullName);
            var baseType = theType.BaseType;

            IEnumerable<ISymbol> typeMembers = theType.GetMembers().Where(m => m.Kind == SymbolKind.Property);

            var baseTypeNames = new List<string>();
            var baseTypesList = new StringBuilder();

            while (baseType.Name != "Object")
            {
                baseTypesList.Append($"{baseType.Name}, ");
                baseTypeNames.Add(baseType.Name);
                typeMembers = typeMembers.Union(baseType.GetMembers().Where(m => m.Kind == SymbolKind.Property));
                baseType = baseType.BaseType;
            }

            WriteLine("{");
            PrintClassProperties(theType.MetadataName, compilation);
            foreach (var tn in baseTypeNames)
            {
                PrintClassProperties(tn, compilation);
            }
            WriteLine("}");
            WriteLine("\n\nInherits from: " + baseTypesList);

#if DEBUG
            ReadKey();
#endif
        }

        private static void PrintClassProperties(string className, Compilation compilation)
        {
            var y = compilation.SyntaxTrees
                               .SelectMany(
                                 x => x.GetRoot()
                                       .DescendantNodes()
                                       .OfType<ClassDeclarationSyntax>()
                                  ).Single(c => c.Identifier.ToString() == className);

            foreach (var m in y.Members.OfType<PropertyDeclarationSyntax>())
            {
                Console.WriteLine($"{m.Identifier}: null,".PadRight(40) + " // " + m.Type.ToString().PadLeft(10) + $" from {className}");
            }
        }
    }
}
